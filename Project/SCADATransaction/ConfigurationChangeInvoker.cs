﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common;

namespace SCADATransaction
{
    public class ConfigurationChangeInvoker
    {
        private readonly IConfigurationChange proxy;

        public ConfigurationChangeInvoker()
        {
            ChannelFactory<IConfigurationChange> channelFactory = new ChannelFactory<IConfigurationChange>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:30000/IConfigurationChange"));
            proxy = channelFactory.CreateChannel();
        }

        public void Update()
        {
            ushort aiCount = (ushort)(DataBase.Model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_INPUT).Count());
            ushort aoCount = (ushort)(DataBase.Model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_OUTPUT).Count());
            ushort biCount = (ushort)(DataBase.Model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_INPUT).Count());
            ushort boCount = (ushort)(DataBase.Model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_OUTPUT).Count());

            proxy.UpdateConfig(Tuple.Create<ushort, ushort, ushort, ushort>(biCount,boCount,aiCount,aoCount));
        }
    }
}