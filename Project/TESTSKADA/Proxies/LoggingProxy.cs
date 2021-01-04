using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Services.Common;
using SCADA.Common.Logging;

namespace NDS.Proxies
{
    public class LoggingProxy
    {
        private ILogging proxy;

        public LoggingProxy()
        {

            ChannelFactory<ILogging> channelFactory = new ChannelFactory<ILogging>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:7007/ILogging"));
            proxy = channelFactory.CreateChannel();

        }

        public void Log(LogEventModel logModel)
        {
            proxy.Log(logModel);
        }
    }
}
