using SSMP.Logging;

namespace SSMPEssentials.Utils
{
    internal class FakeLogger : ILogger
    {
        public void Debug(string message){}
        public void Error(string message){}
        public void Info(string message){}
        public void Message(string message){}
        public void Warn(string message){}
    }


    internal static class Log
    {
        static ILogger logger = new FakeLogger();
        public static void SetLogger(ILogger log)
        {
            logger = log;

#if DEBUG
            //FilteredLogs.API.ApplyFilter(ShouldLog);
#endif
        }

#if DEBUG
        private static bool ShouldLog(BepInEx.Logging.LogEventArgs log)
        {
            //Debug.Log(log.Data);
            //return true;
            if (log.Source.SourceName == "SSMP" && log.Data is string data)
            {
                if (data.StartsWith("[SSMPEssentials")) return true;
            }

            return false;
        }
#endif

        public static void LogInfo(params object[] data)
        {
            foreach (object obj in data)
                logger.Info(obj.ToString());
        }
        public static void LogWarning(params object[] data)
        {
            foreach (object obj in data)
                logger.Warn(obj.ToString());
        }
        public static void LogError(params object[] data)
        {
            foreach (object obj in data)
                logger.Error(obj.ToString());
        }
        public static void LogFatal(params object[] data)
        {
            foreach (object obj in data)
                logger.Error(obj.ToString());
        }
        public static void LogDebug(params object[] data)
        {
            foreach (object obj in data)
                logger.Debug(obj.ToString());
        }
        public static void LogMessage(params object[] data)
        {
            foreach (object obj in data)
                logger.Message(obj.ToString());
        }
    }
}
