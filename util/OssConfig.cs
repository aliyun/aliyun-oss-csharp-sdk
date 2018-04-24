using System;
using System.Linq;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.InteropServices;

namespace osstestcsharp
{
    public class OssConfig
    {
        public static string Version = "oss-perf-test-1.0.0 ";
        public static string Endpoint = "";
        public static string AccessKeyId = "";
        public static string AccessKeySecret = "";
        public static string BucketName = "";
        public static string OssCfgFile = "oss.ini";
        
        public static string Command = "";
        public static string BaseLocalFile = "";
        public static string BaseRemoteKey = "";
        public static string Sql = null;

        public static int PartSize = 100*1024*1024;
        public static int Parallel = 4;
        public static int Multithread = 0;
        public static int LoopTimes = 1;
        public static bool Persistent = false;
        public static bool DifferentSource = false;
        public static bool CrcCheck = true;
        public static long ReadFrom;
        public static long ReadTo;
        public static long ConcurrentReqCount = 1;
        public static long MultiPartSelectCount = 1;

        public static string HelpExampleString = "";
        public static string CsvNewLine = "\n";
        public static string CsvHeader = "Ignore";
        public static bool KeepAllColumns { get; internal set; }

        public static void config(string[] args)
        {
            parseCmdArgs(args); 
            parseCfgFile();
            printCfgInfo();
        }

        public static void parseCmdArgs(string[] args)
        {
            if (args.Length == 0)
                printHelp();
            CmdLineArgParser cmdParser = new CmdLineArgParser(args);
            cmdParser.Parse(); 
        }

        public static void parseCfgFile() 
        {
            if (!File.Exists(OssConfig.OssCfgFile)) 
            {
                FileStream fs = new FileStream(OssConfig.OssCfgFile, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                StreamWriter sw = new StreamWriter(fs);

                sw.Write("Endpoint=\nAccessKeyId=\nAccessKeySecret=");
                sw.Flush();
                sw.Close();
                fs.Close(); 
            }

            StreamReader sr = new StreamReader(OssConfig.OssCfgFile, Encoding.Default); 
            String line;
            while ((line = sr.ReadLine()) != null)
            {
                line = line.Trim();
                String[] arrKeyValue;
                if (line.StartsWith("Endpoint="))
                {
                    arrKeyValue = line.Split('=');
                    OssConfig.Endpoint = arrKeyValue[1]; 
                }
                else if (line.StartsWith("AccessKeyId="))
                {
                    arrKeyValue = line.Split('=');
                    OssConfig.AccessKeyId = arrKeyValue[1]; 
                }
                else if (line.StartsWith("AccessKeySecret="))
                {
                    arrKeyValue = line.Split('=');
                    OssConfig.AccessKeySecret = arrKeyValue[1];
                }
                else if (line.StartsWith("BucketName="))
                {
                    arrKeyValue = line.Split('=');
                    OssConfig.BucketName = arrKeyValue[1];
                }
            }
            sr.Close();

            if (OssConfig.Endpoint == "" || OssConfig.BucketName=="")
            {
                Console.WriteLine("Error Configuration : Endpoint or BucketName is empty . ");
                Console.WriteLine("Please check the config file {0} in the directory {1} . ", OssConfig.OssCfgFile, Environment.CurrentDirectory);
                Environment.Exit(0);
            }
        }

        public static void printCfgInfo()
        {
            Console.WriteLine("");
            Console.WriteLine("version         : " + OssConfig.Version);
            Console.WriteLine("endpoint        : " + OssConfig.Endpoint);
            Console.WriteLine("accessKeyId     : " + OssConfig.AccessKeyId);
            Console.WriteLine("bucketName      : " + OssConfig.BucketName);
            Console.WriteLine("command         : " + OssConfig.Command);
            Console.WriteLine("localfile       : " + OssConfig.BaseLocalFile);
            Console.WriteLine("remotekey       : " + OssConfig.BaseRemoteKey);
            if (OssConfig.Command == "upload_resumable")
            {
                Console.WriteLine("parallel        : " + OssConfig.Parallel);
                Console.WriteLine("partsize        : " + OssConfig.PartSize);
            }
            Console.WriteLine("multithread     : " + OssConfig.Multithread);
            Console.WriteLine("looptimes       : " + OssConfig.LoopTimes);
            Console.WriteLine("persistent      : " + OssConfig.Persistent);
            Console.WriteLine("differentsource : " + OssConfig.DifferentSource);
        }

        public static void printHelp()
        {
            string strHelp = "\n";
            strHelp += "Usage: mono osstestcsharp.exe  [-h] [-v] [-c COMMAND] [-f LOCALFILE]      \n";
            strHelp += "                               [-k REMOTEKEY] [-p PARALLEL]      \n";
            strHelp += "                               [-m MULTITHREAD] [-partsize PARTSIZE]      \n";
            strHelp += "                               [-loop LOOPTIMES] [--persistent]      \n";
            strHelp += "                               [--differentsource]      \n";
            strHelp += "Optional arguments:      \n";
            strHelp += "  -h, --help          show this help message and exit.           \n";
            strHelp += "  -v                  show program's version number and exit.    \n";
            strHelp += "  -c COMMAND          Command Type : upload, upload_resumable, upload_async, download .  \n";
            strHelp += "  -f LOCALFILE        local filename to transfer.                \n";
            strHelp += "  -k REMOTEKEY        remote object key.                         \n";
            strHelp += "  -partsize PARTSIZE  part size parameter for resume breakpoint transfer.                \n";
            strHelp += "  -p PARALLEL         parallel threads parameter for resumable breakpoint transfer.      \n";
            strHelp += "  -m MULTITHREAD      multithread number for transfer.           \n";
            strHelp += "  -loop LOOPTIMES     loop times for transfer.                   \n";
            strHelp += "  --persistent        Whether run the command persistantly.      \n";
            strHelp += "  --differentsource   Whether transfer from different source files.  \n";

            string strExample = "\nExamples :  \n";
            strExample += "    mono osstestcsharp.exe -c upload -f mylocalfilename -k myobjectkeyname \n";
            strExample += "    mono osstestcsharp.exe -c upload_resumable -f mylocalfilename -k myobjectkeyname -p 5 \n";
            strExample += "    mono osstestcsharp.exe -c upload_async -f mylocalfilename -k myobjectkeyname \n";
            strExample += "    mono osstestcsharp.exe -c upload -f mylocalfilename -k myobjectkeyname -m 5 \n";
            strExample += "    mono osstestcsharp.exe -c download -f mylocalfilename -k myobjectkeyname \n";
            strExample += "    mono osstestcsharp.exe -c download -f mylocalfilename -k myobjectkeyname -m 5 \n";
            strExample += "    mono osstestcsharp.exe -c select -f mylocalfilename -k myobjectkeyname -sql \"sqlquery\" -read_from start -read_to end \n";
            strExample += "    mono osstestcsharp.exe -c append -f mylocalfilename -k myobjectkeyname\n"; 

            OssConfig.HelpExampleString = strHelp + strExample;
            Console.WriteLine(OssConfig.HelpExampleString);
            Environment.Exit(0);
        }

        public static void main(string[] args)
        {
            OssConfig.config(args);
        }
    }


    class CmdLineArgParser
    {
        public static ArrayList alArgs = new ArrayList();

        public CmdLineArgParser(string[] args)
        {
            for (int i = 0; i<args.Length; i++)
            {
                alArgs.Add(args[i]);
            }
        }

        public string getArgValueByName(string argumentName)
        {
            int iPos = alArgs.IndexOf(argumentName);
            return alArgs[iPos + 1].ToString();
        }

        public bool has(string argumentName)
        {
            return (alArgs.IndexOf(argumentName) >= 0);
        }

        public bool Parse()
        {
            if (CmdLineArgParser.alArgs.Count==0)
            {
                OssConfig.printHelp();                
            }

            if (this.has("-h") || this.has("help"))
            {
                OssConfig.printHelp();
            }
            if (this.has("-v") || this.has("--version"))
            {
                Console.WriteLine(OssConfig.Version);
                Environment.Exit(0);
            }
            if (this.has("-c"))
            {
                OssConfig.Command = this.getArgValueByName("-c");
                if (OssConfig.Command == "up") 
                {
                    OssConfig.Command = "upload";
                }
                else if (OssConfig.Command == "upr")
                {
                    OssConfig.Command = "upload_resumable";
                }
                else if (OssConfig.Command == "upa")
                {
                    OssConfig.Command = "upload_async";
                }
                else if (OssConfig.Command == "dn")
                {
                    OssConfig.Command = "download";
                }
                else if (OssConfig.Command == "dnr")
                {
                    OssConfig.Command = "download_resumable";
                }
                else if (OssConfig.Command == "head")
                {
                    OssConfig.Command = "head";
                }
            }
            else
            {
                OssConfig.printHelp();
            }
            if (this.has("-f"))
            {
                OssConfig.BaseLocalFile = this.getArgValueByName("-f");
            }
            if (this.has("-k"))
            {
                OssConfig.BaseRemoteKey = this.getArgValueByName("-k");
            }
            if (this.has("-p"))
            {
                OssConfig.Parallel = Convert.ToInt32(this.getArgValueByName("-p"));
            }
            if (this.has("-m"))
            {
                OssConfig.Multithread = Convert.ToInt32(this.getArgValueByName("-m"));
            }
            if (this.has("-partsize"))
            {
                OssConfig.PartSize = Convert.ToInt32(this.getArgValueByName("-partsize"));
            }
            if (this.has("-loop"))
            {
                OssConfig.LoopTimes = Convert.ToInt32(this.getArgValueByName("-looptimes"));
            }
            if (this.has("--persistent"))
            {
                OssConfig.Persistent = true;
            }
            if (this.has("--differentsource"))
            {
                OssConfig.DifferentSource = true;
            }
            if (this.has("-sql"))
            {
                OssConfig.Sql = this.getArgValueByName("-sql");
            }
            if (this.has("-read_from"))
            {
                OssConfig.ReadFrom = Convert.ToInt64(this.getArgValueByName("-read_from"));
            }
            if (this.has("-read_to"))
            {
                OssConfig.ReadTo = Convert.ToInt64(this.getArgValueByName("-read_to"));
            }
            if (this.has("-creq"))
            {
                OssConfig.ConcurrentReqCount = Convert.ToInt64(this.getArgValueByName("-creq"));
            }

            if (this.has("-keep_all"))
            {
                OssConfig.KeepAllColumns = Convert.ToBoolean(this.getArgValueByName("-keep_all"));
            }

            if (this.has("-multipart_select")){
                OssConfig.MultiPartSelectCount = Convert.ToInt64(this.getArgValueByName("-multipart_select"));
            }

            if (this.has("-newline")){
                OssConfig.CsvNewLine = this.getArgValueByName("-newline");
            }

            if (this.has("-csvheader")){
                OssConfig.CsvHeader = this.getArgValueByName("-csvheader");
            }

            return true;
        }

    }
}
