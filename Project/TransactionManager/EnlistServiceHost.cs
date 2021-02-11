using System;
using System.ServiceModel;
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
            catch (Exception)
            {

            }
        }

        public void Close()
        {
            try
            {
                serviceHost.Close();
            }
            catch (Exception)
            {

            }
        }
    }
}
