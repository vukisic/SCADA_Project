using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Services.Common;
using SCADA.Services.Providers;

namespace SCADA.Services.Services
{
    public class LoggingHost
    {
        private ServiceHost serviceHost;

        public LoggingHost()
        {
            serviceHost = new ServiceHost(typeof(LoggingProvider));
            serviceHost.AddServiceEndpoint(typeof(ILogging), new NetTcpBinding(),
                new Uri("net.tcp://localhost:7007/ILogging"));
        }

        public void Open()
        {
            try
            {
                serviceHost.Open();
            }
            catch (Exception e)
            {

            }
        }

        public void Close()
        {
            try
            {
                serviceHost.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
