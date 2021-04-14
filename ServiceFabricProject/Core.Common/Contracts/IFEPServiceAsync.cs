﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface IFEPServiceAsync
    {
        [OperationContract]
        Task ExecuteCommand(ScadaCommand commnad);
    }
}
