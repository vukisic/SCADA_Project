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

        public LogServiceProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task Log(LogEventModel logModel)
        {
            // TO DO !!!
            Trace.WriteLine($"{logModel.EventType} - {logModel.Message}");
        }
    }
}
