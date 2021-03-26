﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;

namespace SF.Common.Proxies
{
    public class SimulatorProxy : ClientBase<IConfigurationChange>
    {
        public SimulatorProxy() : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress("net.tcp://localhost:30007/IConfigurationChange"))
        {

        }

        public void UpdateConfig(Tuple<ushort, ushort, ushort, ushort> points, Dictionary<string, ushort> pairs)
        {
            Channel.UpdateConfig(points, pairs);
        }
    }
}