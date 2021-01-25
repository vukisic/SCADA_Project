using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;
using SCADA.Common.DataModel;
using SCADA.Common.ScadaServices.Common;

namespace SCADA.Common.Proxies
{
    public class AlarmKruncingProxy
    {
        private IAlarmKruncing proxy;

        public AlarmKruncingProxy() {

            ChannelFactory<IAlarmKruncing> channelFactory = new ChannelFactory<IAlarmKruncing>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:7001/IAlarmKruncing"));
            proxy = channelFactory.CreateChannel();

        }

        public List<BasePoint> Check(List<BasePoint> points)
        {
            return proxy.Check(points);
        }
    }
}
