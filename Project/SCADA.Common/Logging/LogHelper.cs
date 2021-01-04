using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Layout;
using log4net.Repository.Hierarchy;

namespace SCADA.Common.Logging
{
    public class LogHelper
    {
        public ILog Logger { get; set; }
        public string loggerName = "LogEvents";
        public LogHelper()
        {
            BasicConfigurator.Configure();
            Logger = LogManager.GetLogger(loggerName);
            SetLevel(loggerName, "ALL");
            AddAppender(Logger, CreateFileAppender(loggerName + "appender", $"{loggerName}.txt"));

        }
        private void SetLevel(string loggerName, string levelName)
        {
            ILog log = LogManager.GetLogger(loggerName);
            Logger l = (Logger)log.Logger;

            l.Level = l.Hierarchy.LevelMap[levelName];
        }

        private void AddAppender(ILog log, IAppender appender)
        {
            Logger l = (Logger)log.Logger;

            l.AddAppender(appender);
        }

        private IAppender CreateFileAppender(string name, string fileName)
        {
            FileAppender appender = new FileAppender
            {
                Name = name,
                File = fileName,
                AppendToFile = true,
                LockingModel = new FileAppender.MinimalLock(),
            };

            PatternLayout layout = new PatternLayout
            {
                ConversionPattern = "%d-[%t]-%-5p-[%logger]-%m%n"
            };
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            return appender;
        }
    }
}
