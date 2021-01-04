using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Services.Common;

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

        public void Log(string level, string message)
        {
            proxy.Log(level, message);
        }
    }
}
