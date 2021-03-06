﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;
using SCADA.Common.DataModel;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface ICommandingServiceAsync
    {
        [OperationContract]
        Task Commmand(ScadaCommand command);
    }
}
