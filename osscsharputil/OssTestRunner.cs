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
            ossClient = new OssClient(OssConfig.Endpoint, OssConfig.AccessKeyId, OssConfig.AccessKeySecret);
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

        public void upload_resumble()
        {
            transfer_start_initialize(); 

            try
            {
                var metadata = BuildCallbackMetadata(CallbackServer, CallbackBody);
                var uploadResult = this.ossClient.ResumableUploadObject(OssConfig.BucketName, this.strRemoteKey, this.strLocalFile, metadata, null, OssConfig.PartSize);
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
                var obj = this.ossClient.GetObject(OssConfig.BucketName, this.strRemoteKey);
                using (var requestStream = obj.Content)
                {
                    byte[] buf = new byte[8192 * 4];
                    var fs = File.Open(this.strLocalFile, FileMode.OpenOrCreate);
                    var len = 0;
                    var once_read_bytes = 8192* 4; //Default 8192 * 4 Bytes
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

            Console.ReadKey();
        }
        
        static void Main(string[] args)
        {
            start_and_config(args);

            try
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
