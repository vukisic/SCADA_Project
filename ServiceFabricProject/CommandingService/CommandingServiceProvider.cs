using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common;

namespace CommandingService
{
    public class CommandingServiceProvider : ICommandingServiceAsync
    {
        private Func<ScadaCommand, Task> _addCommand;
        private StatefulServiceContext _context;
        public CommandingServiceProvider(StatefulServiceContext context, Func<ScadaCommand,Task> addCommand)
        {
            _context = context;
            _addCommand = addCommand;
        }
        public Task Commmand(ScadaCommand command)
        {
            _addCommand(command);
            return Task.CompletedTask;
        }
    }
}
