using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;

namespace Core.Common.ServiceBus.Commands
{
    public class ModelUpdateCommand : NServiceBus.ICommand
    {
        public Dictionary<DMSType,Container> Model { get; set; }
    }
}
