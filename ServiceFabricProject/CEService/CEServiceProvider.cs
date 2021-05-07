using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;

namespace CEService
{
    public class CEServiceProvider : ICEServiceAsync
    {
        private Func<int,Task> _addCommand;
        private StatelessServiceContext _context;

        public CEServiceProvider(Func<int,Task> addCommand, StatelessServiceContext context)
        {
            _addCommand = addCommand;
            _context = context;
        }
        public Task SetPoints(int points)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "FEP - ExecuteCommand called!");
            _addCommand(points);
            return Task.CompletedTask;
        }
    }
}
