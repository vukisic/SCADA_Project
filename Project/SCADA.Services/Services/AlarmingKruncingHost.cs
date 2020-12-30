using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Services.AlarmingKruncingService;
using SCADA.Services.Common;

namespace SCADA.Services.Services
{
    public class AlarmingKruncingHost
    {
        private ServiceHost modelServiceHost;

        public AlarmingKruncingHost()
        {
            modelServiceHost = new ServiceHost(typeof(AlarmKruncingProvider));
            modelServiceHost.AddServiceEndpoint(typeof(IAlarmKruncing), new NetTcpBinding(),
                new Uri("net.tcp://localhost:7001/IAlarmKruncing"));
        }

        public void Open()
        {
            try
            {
                modelServiceHost.Open();
            }
            catch (Exception e)
            {

            }
        }

        public void Close()
        {
            try
            {
                modelServiceHost.Close();
            }
            catch (Exception e)
            {

            }
        }
    }
}
