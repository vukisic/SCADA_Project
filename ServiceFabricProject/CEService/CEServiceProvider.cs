using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;

namespace CEService
{
    public class CEServiceProvider : ICEServiceAsync
    {
        private Func<int,Task> _addCommand;

        public CEServiceProvider(Func<int,Task> addCommand)
        {
            _addCommand = addCommand;
        }
        public Task SetPoints(int points)
        {
            _addCommand(points);
            return Task.CompletedTask;
        }
    }
}
