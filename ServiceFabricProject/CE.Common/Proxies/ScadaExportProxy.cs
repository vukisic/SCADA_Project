using System.Collections.Generic;
using System.ServiceModel;
using SCADA.Common.DataModel;
using SCADA.Common.ScadaServices.Common;

namespace CE.Common.Proxies
{
    public class ScadaExportProxy
    {
        private IScadaExport proxy;

        public ScadaExportProxy()
        {
            ChannelFactory<IScadaExport> channelFactory = new ChannelFactory<IScadaExport>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:21011/IScadaExport"));
            proxy = channelFactory.CreateChannel();
        }

        public Dictionary<string, BasePoint> GetData()
        {
            return proxy.GetData();
        }
    }
}
