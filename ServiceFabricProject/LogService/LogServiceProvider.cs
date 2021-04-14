using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common.Logging;

namespace LogService
{
    public class LogServiceProvider : ILogService
    {
        private StatelessServiceContext _context;
        private TableStorageProvider _provider;

        public LogServiceProvider(StatelessServiceContext context)
        {
            _context = context;
            _provider = new TableStorageProvider("log", false);
        }

        public async Task Log(LogEventModel logModel)
        {
            Trace.WriteLine($"{logModel.EventType} - {logModel.Message}");
            ServiceEventSource.Current.ServiceMessage(_context, $"{logModel.EventType} - {logModel.Message}");
            _provider.Add(new TableLog(logModel.EventType, logModel.Message));
        }
    }
}
