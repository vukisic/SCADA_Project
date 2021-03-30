using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common.Logging;
using SCADA.Common.ScadaServices.Providers;

namespace LogService
{
    public class LogServiceProvider : ILogService
    {
        private StatelessServiceContext _context;
        private LoggingProvider _loggingProvider;

        public LogServiceProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task Log(LogEventModel logModel)
        {
            await Task.Factory.StartNew(() => _loggingProvider.Log(logModel));
        }
    }
}
