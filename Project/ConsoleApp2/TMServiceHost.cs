using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace ConsoleApp2
{
    public class TMServiceHost
    {
        ServiceHost serviceHost = null; 
        public TMServiceHost()
        {
            serviceHost = new ServiceHost(typeof(EnlistManager));
            NetTcpBinding tcpBinding = new NetTcpBinding();

            serviceHost.AddServiceEndpoint(typeof(IEnlistManager), tcpBinding,
                                "net.tcp://localhost:20000/TM");
            serviceHost.Open();
        }
    }
}
