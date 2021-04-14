using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common;

namespace FEPService
{
    public class FEPProvider : IFEPServiceAsync
    {
        private Func<ScadaCommand, Task> _addCommand;
        private StatelessServiceContext _context;
        public FEPProvider(StatelessServiceContext context, Func<ScadaCommand, Task> addCommand)
        {
            _context = context;
            _addCommand = addCommand;
        }
        public Task ExecuteCommand(ScadaCommand command)
        {
            _addCommand(command);
            return Task.CompletedTask;
        }
    }
}
