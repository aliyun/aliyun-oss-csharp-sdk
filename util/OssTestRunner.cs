using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Common;
using Aliyun.OSS.Util;

namespace osstestcsharp
{
    public class OssTestRunner
    {
        public static double totalTransferSize = 0.000;
        public static double totalTransferRate = 0.000;
        public static double totalTransferDuration = 0.000;
        private static DateTime totalStartTime;
        private static DateTime totalStopTime;
        public static string CallbackServer = "";
        public static string CallbackBody = "bucket=${bucket}&object=${object}&etag=${etag}&size=${size}&mimeType=${mimeType}&" +
                                     "my_var1=${x:var1}&my_var2=${x:var2}";
        public AutoResetEvent _event = new AutoResetEvent(false);
        public OssClient ossClient = null;
        public string taskID = "0";
        public string strLocalFile = "";
        public string strRemoteKey = "";
        private DateTime dtStartTime;
        private DateTime dtStopTime;
        private double transferSize;
        private double transferDuration;
        private double transferRate;

        
        public OssTestRunner()
        {
            ClientConfiguration conf = new ClientConfiguration();
            conf.ProgressUpdateInterval = 1024 * 1024 * 10;
            //conf.MaxResumableUploadThreads = OssConfig.Parallel;
            //conf.PreReadBufferCount = 0;
            //conf.EnalbeMD5Check = true;
            ossClient = new OssClient(OssConfig.Endpoint, OssConfig.AccessKeyId, OssConfig.AccessKeySecret, conf);
        }

        ~OssTestRunner()
        {
        }
        
        public void runSingleTask(object obj)
        {
            if (OssConfig.LoopTimes == 0)
            {
                OssConfig.Persistent = true;
            }

            int runIndex = 1;
            while ((runIndex <= OssConfig.LoopTimes) || OssConfig.Persistent)
            {
                switch (OssConfig.Command)
                {
                    case "upload":
                        upload();
                        break;
                    case "upload_resumable":
                        upload_resumble();
                        break;
                    case "upload_async":
                        upload_async();
                        break;
                    case "download":
                        download();
                        break;
                    case "select":
                        if (OssConfig.ConcurrentReqCount > 1)
                        {
                            MultipleSelect();
                        }
                        else
                        {
                            if (OssConfig.MultiPartSelectCount > 1)
                            {
                                MultipartSelect();
                            }
                            else
                            {
                                select();
                            }
                        }
                        break;
                    case "append":
                        append();
                        break;
                    case "ci":
                        sql_ci();
                        break;
                    case "head":
                        Head();
                        break;
                    default:
                        Console.WriteLine("The Command Type Error : {0}", OssConfig.Command);
                        OssConfig.printHelp();
                        break;
                }

                runIndex++;
            }

            if (OssConfig.Multithread>=1)
            {
                ManualResetEvent mre = (ManualResetEvent)obj;
                mre.Set();
            }
        }

        public void upload()
        {
            transfer_start_initialize(); 

            try
            {
                this.ossClient.PutObject(OssConfig.BucketName, this.strRemoteKey, this.strLocalFile);
                Console.WriteLine("     Put object : {0} succeeded ! ", this.strRemoteKey);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.ToString());
            }

            transfer_stop_statistic();
        }

        public void append()
        {

            transfer_start_initialize();

            try
            {
                long pos = 0;
                try
                {
                    var meta = this.ossClient.GetObjectMetadata(OssConfig.BucketName, this.strRemoteKey);
                    pos = meta.ContentLength;
                }
                catch(Exception){
                }
                AppendObjectRequest appendReq = new AppendObjectRequest(OssConfig.BucketName, this.strRemoteKey);
                appendReq.Content = new FileStream(this.strLocalFile, FileMode.Open);
                appendReq.Position = pos;
                this.ossClient.AppendObject(appendReq);
                Console.WriteLine("     Append object : {0} succeeded ! ", this.strRemoteKey);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.ToString());
            }

            transfer_stop_statistic();
        }

        public void upload_resumble()
        {
            transfer_start_initialize(); 

            try
            {
                var metadata = BuildCallbackMetadata(CallbackServer, CallbackBody);
                var uploadResult = this.ossClient.ResumableUploadObject(OssConfig.BucketName, this.strRemoteKey, this.strLocalFile, metadata, "/home/qi.xu/upload", OssConfig.PartSize,
                                                                        (object sender, StreamTransferProgressArgs e) => {
                    Console.WriteLine("%:" + e.PercentDone + " .TransferredBytes:" + e.IncrementTransferred + ". TotalBytes:" + e.TotalBytes + " . TotalTranferredBytes:" + e.TransferredBytes);
                    
                });
                var responseContent = GetCallbackResponse(uploadResult);
                Console.WriteLine("Resumable upload object:{0} succeeded. ", this.strRemoteKey);
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                    ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.Message);
            }

            transfer_stop_statistic();
        }

        public void upload_async()
        {
            transfer_start_initialize();

            try
            {
                using (var fs = File.Open(this.strLocalFile, FileMode.Open))
                {
                    var metadata = new ObjectMetadata();
                    metadata.CacheControl = "No-Cache";
                    metadata.ContentType = "text/html";
                    this.ossClient.BeginPutObject(OssConfig.BucketName, this.strRemoteKey, fs, metadata, putObjectCallback, new string('a', 8));
                    _event.WaitOne();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
            }

            transfer_stop_statistic();
        }

        public void download()
        {
            transfer_start_initialize(); 
            DateTime dtStart = DateTime.Now;
            try
            {
                GetObjectRequest req = new GetObjectRequest(OssConfig.BucketName, this.strRemoteKey);
                if (OssConfig.ReadTo != 0 || OssConfig.ReadFrom != 0)
                {
                    req.SetRange(OssConfig.ReadFrom, OssConfig.ReadTo);
                }

                var obj = this.ossClient.GetObject(req);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[8192 * 4];
                    var fs = File.Open(this.strLocalFile, FileMode.OpenOrCreate);
                    var len = 0;
                    var once_read_bytes = 8192* 4; //Default 1024 Bytes
                    while ((len = requestStream.Read(buf, 0, once_read_bytes)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();
                }   
                Console.WriteLine("Get object succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get object failed. {0}", ex.Message);
            }

            transfer_stop_statistic();
        }
        public void sql_ci()
        {
            while (true)
            {
                string sql = Console.ReadLine();
                Console.WriteLine("Result:");
                if (sql.Equals("q")){
                    break;
                }

                SelectObjectRequest req = new SelectObjectRequest(OssConfig.BucketName, this.strRemoteKey, sql);
                if (OssConfig.ReadTo != 0 || OssConfig.ReadFrom != 0)
                {
                    req.SetRange(OssConfig.ReadFrom, OssConfig.ReadTo);
                }
                req.InputNewLine = OssConfig.CsvNewLine;
                req.OutputNewLine = OssConfig.CsvNewLine;
                req.KeepAllColumns = OssConfig.KeepAllColumns;

                if (OssConfig.CsvHeader == "Use")
                {
                    req.Header = SelectObjectRequest.HeaderInfo.Use;
                }
                else if(OssConfig.CsvHeader == "None")
                {
                    req.Header = SelectObjectRequest.HeaderInfo.None;
                }
                else if (OssConfig.CsvHeader == "Ignore")
                {
                    req.Header = SelectObjectRequest.HeaderInfo.Ignore;
                }

                System.Diagnostics.Stopwatch watch = System.Diagnostics.Stopwatch.StartNew();
                var obj = this.ossClient.SelectObject(req);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[8192 * 4];

                    var len = 0;
                    var once_read_bytes = 8192 * 4; //Default 1024 Bytes

                    while ((len = requestStream.Read(buf, 0, once_read_bytes)) != 0)
                    {
                        string text = Encoding.ASCII.GetString(buf, 0, len);
                        Console.Write(text.Replace("\r\n", "\n").Replace('\r', '\n'));
                    }
                }

                Console.WriteLine();
                Console.WriteLine("Elapse " + watch.ElapsedMilliseconds + "ms.");
            }
        }
        public void select()
        {
            transfer_start_initialize();
            DateTime dtStart = DateTime.Now;
            try
            {
                StringBuilder longsql = new StringBuilder();
                longsql.Append(" and _1 in (");
                int i = 0;
                while(longsql.Length < 32 * 1024){
                    longsql.Append("'Tomaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa" + i);i++;
                    longsql.Append("',");
                }
                longsql.Append("'a')");
                string ss = OssConfig.Sql == null ? "select _1 from ossobject where _1 like 'Lilly*'" + longsql.ToString() : OssConfig.Sql;
                ss = ss.Replace("\\r", "\r");
                SelectObjectRequest req = new SelectObjectRequest(OssConfig.BucketName, this.strRemoteKey, ss);
                if (OssConfig.ReadTo != 0 || OssConfig.ReadFrom != 0)
                {
                    req.StartLine = (int)OssConfig.ReadFrom;
                    req.EndLine = (int)OssConfig.ReadTo;
                }

                if (OssConfig.CsvHeader == "Use")
                {
                    req.Header = SelectObjectRequest.HeaderInfo.Use;
                }
                else if (OssConfig.CsvHeader == "None")
                {
                    req.Header = SelectObjectRequest.HeaderInfo.None;
                }
                else if (OssConfig.CsvHeader == "Ignore")
                {
                    req.Header = SelectObjectRequest.HeaderInfo.Ignore;
                }

                req.InputNewLine = OssConfig.CsvNewLine;
                req.OutputNewLine = OssConfig.CsvNewLine;
                req.KeepAllColumns = OssConfig.KeepAllColumns;

                var obj = this.ossClient.SelectObject(req);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[8192 * 4];
                    var fs = File.Open(this.strLocalFile, FileMode.OpenOrCreate);
                    var len = 0;
                    var once_read_bytes = 8192 * 4; //Default 1024 Bytes
                    while ((len = requestStream.Read(buf, 0, once_read_bytes)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                    fs.Close();
                }
                Console.WriteLine("Get object succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Get object failed. {0}", ex.Message);
            }

            transfer_stop_statistic();
        }

        public void Head()
        {
            var meta = this.ossClient.HeadCsvObjectMetadata(OssConfig.BucketName, this.strRemoteKey);
            Console.WriteLine("TotalLines:{0}", meta.CsvLines);
        }

        public void MultipartSelect()
        {
            transfer_start_initialize();
            Thread[] threads = new Thread[OssConfig.MultiPartSelectCount];
            var meta = this.ossClient.HeadCsvObjectMetadata(OssConfig.BucketName, this.strRemoteKey);
            long length = meta.CsvLines / OssConfig.MultiPartSelectCount;
            for (int i = 0; i < OssConfig.MultiPartSelectCount; i++)
            {
                threads[i] = new Thread((obj1) =>
                {
                    int idx = (int)obj1;

                    DateTime dtStart = DateTime.Now;
                    try
                    {
                        string ss = OssConfig.Sql == null ? "select _1 from ossobject where _1 like 'Lilly*'" : OssConfig.Sql;
                        SelectObjectRequest req = new SelectObjectRequest(OssConfig.BucketName, this.strRemoteKey, ss);
                        req.InputNewLine = OssConfig.CsvNewLine;
                        req.OutputNewLine = OssConfig.CsvNewLine;
                        if (idx == OssConfig.MultiPartSelectCount - 1)
                        {
                            Console.WriteLine("Offset:" + idx * length + "; To: " + (meta.CsvLines-1));
                            req.StartLine = (int)(idx * length);
                            req.EndLine = meta.CsvLines - 1;
                        }
                        else
                        {
                            Console.WriteLine("Offset:" + idx * length + "; To: " + ((idx + 1) * length - 1));
                            //req.SetRange(idx * length, (idx + 1) * length - 1);
                            req.StartLine = (int)(idx * length);
                            req.EndLine = (int)((idx + 1) * length - 1);
                        }

                        var obj = this.ossClient.SelectObject(req);
                        if (obj == null)
                        {
                            Console.WriteLine("return failed.");
                            return;
                        }
                        using (var requestStream = obj.Content)
                        {
                            byte[] buf = new byte[8192 * 4];
                            var len = 0;
                            var once_read_bytes = 8192 * 4; //Default 1024 Bytes
                            var fs = File.Open(this.strLocalFile + idx + ".csv", FileMode.OpenOrCreate);
                            while ((len = requestStream.Read(buf, 0, once_read_bytes)) != 0)
                            {
                                fs.Write(buf, 0, len);
                            }
                            fs.Close();
                        }
                        Console.WriteLine("Req:" + idx + "Get object succeeded ");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get object failed. {0}", ex.Message);
                    }

                });
                threads[i].Start(i);
            }

            for (int i = 0; i < OssConfig.MultiPartSelectCount; i++)
            {
                threads[i].Join();
            }
            transfer_stop_statistic();
        }

        public void MultipleSelect()
        {
            transfer_start_initialize();
            Thread[] threads = new Thread[OssConfig.ConcurrentReqCount];
            for (int i = 0; i < OssConfig.ConcurrentReqCount; i++)
            {
                threads[i] = new Thread((obj1) =>
                {

                    DateTime dtStart = DateTime.Now;
                    try
                    {
                        string ss = OssConfig.Sql == null ? "select _1 from ossobject where _1 like 'Lilly*'" : OssConfig.Sql;
                        SelectObjectRequest req = new SelectObjectRequest(OssConfig.BucketName, this.strRemoteKey, ss);
                        if (OssConfig.ReadTo != 0 || OssConfig.ReadFrom != 0)
                        {
                            req.StartLine = (int)OssConfig.ReadFrom;
                            req.EndLine = (int)OssConfig.ReadTo;
                        }
                        req.InputNewLine = OssConfig.CsvNewLine;
                        req.OutputNewLine = OssConfig.CsvNewLine;
                        var obj = this.ossClient.SelectObject(req);
                        if (obj == null){
                            Console.WriteLine("return failed.");
                            return;
                        }
                        using (var requestStream = obj.Content)
                        {
                            byte[] buf = new byte[8192 * 4];
                            var len = 0;
                            var once_read_bytes = 8192 * 4; //Default 1024 Bytes
                            while ((len = requestStream.Read(buf, 0, once_read_bytes)) != 0)
                            {

                            }
                        }
                        Console.WriteLine("Req:" + obj1 + "Get object succeeded ");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Get object failed. {0}", ex.Message);
                    }

                });
                threads[i].Start(i);
            }

            for (int i = 0; i < OssConfig.ConcurrentReqCount; i++){
                threads[i].Join();
            }
            transfer_stop_statistic();
        }


        public Stream download_async()
        {
            transfer_start_initialize();
            DateTime dtStart = DateTime.Now;
            try
            {
                var obj = this.ossClient.GetObject(OssConfig.BucketName, this.strRemoteKey);
                return obj.Content;

            }
            catch (Exception ex)
            {
                Console.WriteLine("Get object failed. {0}", ex.Message);
            }

            return null;
        }
        
        private void putObjectCallback(IAsyncResult ar)
        {
            try
            {
                var result = this.ossClient.EndPutObject(ar);
                Console.WriteLine("ETag:{0}", result.ETag);
                Console.WriteLine("User Parameter:{0}", ar.AsyncState as string);
                Console.WriteLine("Put object succeeded");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Put object failed, {0}", ex.Message);
            }
            finally
            {
                Console.WriteLine("SN{0}## PutObjectCallback : Set AutoResetEvent ... ", this.taskID);
                _event.Set();
            }
        }

        private void transfer_start_initialize()
        {
            this.dtStartTime = DateTime.Now;
            Console.WriteLine("\n++++ START {0} : taskID={1}, LocalFile={2}, RemoteKey={3}, StartTime={4} . ", OssConfig.Command, this.taskID, this.strLocalFile, this.strRemoteKey, this.dtStartTime.ToString("yyyy-MM-dd HH:mm:ss:ms")); 
        }

        private void transfer_stop_statistic()
        {
            this.dtStopTime = DateTime.Now;
            TimeSpan ts = this.dtStopTime.Subtract(this.dtStartTime).Duration();
            this.transferDuration = ts.TotalSeconds;
            FileInfo fi = new FileInfo(this.strLocalFile);
            this.transferSize = fi.Length/1024/1024; 
            this.transferRate = this.transferSize/this.transferDuration; 
            Console.WriteLine("\n---- STOP  {0} : taskID={1}, LocalFile={2}, RemoteKey={3}, StopTime={4} . ", 
                                OssConfig.Command, this.taskID, this.strLocalFile, this.strRemoteKey, this.dtStopTime.ToString("yyyy-MM-dd HH:mm:ss:ms")); 
            Console.WriteLine("#### STATISTIC : taskID={0}, TransferRate={1}MB/S, TransferSize={2}MB, TransferDuration={3}Seconds. ", 
                                this.taskID, this.transferRate, this.transferSize, this.transferDuration);
            OssTestRunner.totalTransferSize += this.transferSize; 
        }

        private static string GetCallbackResponse(PutObjectResult putObjectResult)
        {
            string callbackResponse = null;
            using (var stream = putObjectResult.ResponseStream)
            {
                var buffer = new byte[4 * 1024];
                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                callbackResponse = Encoding.Default.GetString(buffer, 0, bytesRead);
            }
            return callbackResponse;
        }

        private static ObjectMetadata BuildCallbackMetadata(string callbackUrl, string callbackBody)
        {
            string callbackHeaderBuilder = new CallbackHeaderBuilder(callbackUrl, callbackBody).Build();
            string CallbackVariableHeaderBuilder = new CallbackVariableHeaderBuilder().
                AddCallbackVariable("x:var1", "x:value1").AddCallbackVariable("x:var2", "x:value2").Build();

            var metadata = new ObjectMetadata();
            metadata.AddHeader(HttpHeaders.Callback, callbackHeaderBuilder);
            metadata.AddHeader(HttpHeaders.CallbackVar, CallbackVariableHeaderBuilder);
            return metadata;
        }

        public void setTaskID(int i)
        {
            this.taskID = i.ToString().PadLeft(2, '0');
        }

        public void setLocalFileAndRemoteKey()
        {
            this.strLocalFile = OssConfig.BaseLocalFile;
            this.strRemoteKey = OssConfig.BaseRemoteKey;

            if (OssConfig.Multithread >= 1)
            {
                if (OssConfig.Command.StartsWith("upload"))
                {
                    this.strRemoteKey += this.taskID;
                    if (OssConfig.DifferentSource)
                    {
                        this.strLocalFile += this.taskID;
                    }
                }
                else if (OssConfig.Command.StartsWith("download"))
                {
                    this.strLocalFile += this.taskID;
                    if (OssConfig.DifferentSource)
                    {
                        this.strRemoteKey += this.taskID;
                    }
                }
                else if (OssConfig.Command.StartsWith("select")){
                    this.strLocalFile += this.taskID;
                }
                else
                {
                    throw new Exception("The command type error : " + OssConfig.Command);
                }
            }

            if ((OssConfig.Command.StartsWith("upload")) && (!File.Exists(this.strLocalFile)))
            {
                throw new Exception("Local file does not exist : " + this.strLocalFile);
            }
        }

        public static void start_and_config(String[] args)
        {
            OssConfig.config(args);
            OssTestRunner.totalStartTime = DateTime.Now;
            Console.WriteLine("\nThe Begin : StartTime={0}", OssTestRunner.totalStartTime.ToString("yyyy-MM-dd HH:mm:ss:ms"));
        }

        public static void stop_and_statistic_report()
        {
            OssTestRunner.totalStopTime = DateTime.Now;
            TimeSpan ts = OssTestRunner.totalStopTime.Subtract(OssTestRunner.totalStartTime).Duration();
            OssTestRunner.totalTransferDuration = ts.TotalSeconds;
            OssTestRunner.totalTransferRate = OssTestRunner.totalTransferSize / OssTestRunner.totalTransferDuration;
            Console.WriteLine("\nThe End  : StopTime ={0}, Command={1}, LocalFile={2}, RemoteKey={3}, Parallel={4}, Multithread={5}. ", OssTestRunner.totalStopTime.ToString("yyyy-MM-dd HH:mm:ss:ms"), OssConfig.Command, OssConfig.BaseLocalFile, OssConfig.BaseRemoteKey, OssConfig.Parallel, OssConfig.Multithread);
            Console.WriteLine("#### StatisticReport: AvgTransferRate={0}MB/S, TotalSize={1}MB, TotalDuration={2} Seconds.", OssTestRunner.totalTransferRate, OssTestRunner.totalTransferSize, OssTestRunner.totalTransferDuration);
        }
        
        static void Main(string[] args)
        {
            start_and_config(args);

            try
            {
                if (OssConfig.Command == "download_async")
                {
                    Stream[] streams = new Stream[OssConfig.Multithread];
                    for (int i = 1; i <= OssConfig.Multithread; i++)
                    {
                        OssTestRunner ossTestRunner = new OssTestRunner();
                        ossTestRunner.setTaskID(i);
                        ossTestRunner.setLocalFileAndRemoteKey();
                        streams[i-1] = ossTestRunner.download_async();
                    }

                    bool[] finished = new bool[streams.Length];

                    IAsyncResult[] results = new IAsyncResult[streams.Length];
                    Parallel.For(0, streams.Length, (idx) =>
                    {
                        while (true)
                        {
                            var buffer = new byte[4 * 8092];
                            var bytesRead = streams[idx].Read(buffer, 0, buffer.Length);
                            if(bytesRead <= 0)
                            {
                                break;
                            }
                        }
                    });

                }
                else
                {
                    if (OssConfig.Multithread < 1)
                    {
                        OssTestRunner ossTestRunner = new OssTestRunner();
                        ossTestRunner.setLocalFileAndRemoteKey();
                        ossTestRunner.runSingleTask("");
                    }
                    else
                    {
                        ManualResetEvent[] mre = new ManualResetEvent[OssConfig.Multithread];
                        for (int i = 1; i <= OssConfig.Multithread; i++)
                        {
                            OssTestRunner ossTestRunner = new OssTestRunner();
                            ossTestRunner.setTaskID(i);
                            ossTestRunner.setLocalFileAndRemoteKey();

                            mre[i - 1] = new ManualResetEvent(false);
                            ThreadPool.QueueUserWorkItem(new WaitCallback(ossTestRunner.runSingleTask), mre[i - 1]);
                        }

                        WaitHandle.WaitAll(mre);
                    }
                }
            }
            catch (OssException ex)
            {
                Console.WriteLine("Failed with error code: {0}; Error info: {1}. \nRequestID:{2}\tHostID:{3}",
                                 ex.ErrorCode, ex.Message, ex.RequestId, ex.HostId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed with error info: {0}", ex.ToString());
            }

            stop_and_statistic_report();
        }
    }

}
