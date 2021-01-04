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
        public void Log(LogEventModel logModel)
        {
            lock (obj)
            {
                logHelper = new LogHelper();
                switch (logModel.EventType)
                {
                    case LogEventType.DEBUG: logHelper.Logger.Debug($"{logModel.EventType.ToString()} {logModel.Message}"); break;
                    case LogEventType.INFO: logHelper.Logger.Info($"{logModel.EventType.ToString()} {logModel.Message}"); break;
                    case LogEventType.WARN: logHelper.Logger.Warn($"{logModel.EventType.ToString()} {logModel.Message}"); break;
                    case LogEventType.ERROR: logHelper.Logger.Error($"{logModel.EventType.ToString()} {logModel.Message}"); break;
                    case LogEventType.FATAL: logHelper.Logger.Fatal($"{logModel.EventType.ToString()} {logModel.Message}"); break;

                }
                logHelper = null;
            }
        }
    }
}
