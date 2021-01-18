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
    public class ScadaStorageService
    {
        private ServiceHost serviceHost;

        public ScadaStorageService()
        {
            serviceHost = new ServiceHost(typeof(ScadaStorageProvider));
            serviceHost.AddServiceEndpoint(typeof(IScadaStorage), new NetTcpBinding(),
                new Uri("net.tcp://localhost:8033/IScadaStorage"));
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
