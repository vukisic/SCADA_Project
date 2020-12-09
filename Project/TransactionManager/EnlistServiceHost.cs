using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace TransactionManager
{
    public class EnlistServiceHost
    {
        private ServiceHost serviceHost;

        public EnlistServiceHost()
        {
            serviceHost = new ServiceHost(typeof(EnlistManager));
            serviceHost.AddServiceEndpoint(typeof(IEnlistManager), new NetTcpBinding(), new Uri("net.tcp://localhost:6001/IEnlistManager"));
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
