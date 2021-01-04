using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Logging;
using SCADA.Services.Common;

namespace SCADA.Services.Providers
{
    public class LoggingProvider : ILogging
    {
        private LogHelper logHelper;
        private static LoggingProvider instance;
        private static object obj = new object();

        private LoggingProvider() { }

        public static LoggingProvider Instance()
        {
            lock (obj)
            {
                if (instance == null)
                    instance = new LoggingProvider();
                return instance;
            }
        }
        public void Log(string level, string message)
        {
            lock (obj)
            {
                logHelper = new LogHelper();
                switch (level)
                {
                    case "DEBUG": logHelper.Logger.Debug($"{level} {message}"); break;
                    case "INFO": logHelper.Logger.Info($"{level} {message}"); break;
                    case "WARN": logHelper.Logger.Warn($"{level} {message}"); break;
                    case "ERROR": logHelper.Logger.Error($"{level} {message}"); break;
                    case "FATAL": logHelper.Logger.Fatal($"{level} {message}"); break;

                }
                logHelper = null;
            }
        }
    }
}
