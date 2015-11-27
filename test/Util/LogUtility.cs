using System;
using System.Diagnostics;

namespace Aliyun.OSS.Test.Util
{
    public static class LogUtility
    {
        public static void LogMessage(string message)
        {
            Trace.TraceInformation(DateTime.Now + ": " + message);
        }

        public static void LogMessage(string message, params object[] args)
        {
            LogMessage(string.Format(message, args));
        }

        public static void LogWarning(string message)
        {
            Trace.TraceWarning(DateTime.UtcNow + ": " + message);
        }

        public static void LogWarning(string message, params object[] args)
        {
            LogWarning(string.Format(message, args));
        }
    }
}
