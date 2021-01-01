using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Services.Providers;
using SCADA.Services.Common;

namespace SCADA.Services.Services
{
    public class AlarmingKruncingHost
    {
        private ServiceHost serviceHost;

        public AlarmingKruncingHost()
        {
            serviceHost = new ServiceHost(typeof(AlarmKruncingProvider));
            serviceHost.AddServiceEndpoint(typeof(IAlarmKruncing), new NetTcpBinding(),
                new Uri("net.tcp://localhost:7001/IAlarmKruncing"));
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
