using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common;
using SCADA.Common.Proxies;
using SCADA.Common.ScadaServices;

namespace SCADATransaction
{
    public class ConfigurationChangeInvoker
    {
        private readonly IConfigurationChange proxy;

        public ConfigurationChangeInvoker()
        {
            ChannelFactory<IConfigurationChange> channelFactory = new ChannelFactory<IConfigurationChange>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:30007/IConfigurationChange"));
            proxy = channelFactory.CreateChannel();
        }

        public void Update(Dictionary<string,ushort> pairs)
        {
            var storage = ScadaProxyFactory.Instance().ScadaStorageProxy();
            var model = storage.GetModel();
            ushort aiCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_INPUT).Count());
            ushort aoCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_OUTPUT).Count());
            ushort biCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_INPUT).Count());
            ushort boCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_OUTPUT).Count());

            proxy.UpdateConfig(Tuple.Create<ushort, ushort, ushort, ushort>(biCount,boCount,aiCount,aoCount), pairs);
        }
    }
}
