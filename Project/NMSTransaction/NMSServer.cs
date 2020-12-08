using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace NMSTransaction
{
    public class NMSServer
    {
        private ServiceHost serviceHost;

        public NMSServer()
        {
            serviceHost = new ServiceHost(typeof(NMSTransactionProvider));
            serviceHost.AddServiceEndpoint(typeof(ITransactionSteps), new NetTcpBinding(), new Uri("net.tcp://localhost:4001/ITransactionSteps"));
        }

        public void Open()
        {
            try
            {
                serviceHost.Open();
            }
            catch(Exception e)
            {

            }
        }

        public void Close()
        {
            try
            {
                serviceHost.Close();
            }
            catch(Exception e)
            {

            }
        }
    }
}
