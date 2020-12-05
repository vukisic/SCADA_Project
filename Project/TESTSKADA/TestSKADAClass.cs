using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace TESTSKADA
{
    public class TestSKADAClass //tu mi je host 
    {
        ServiceHost serviceHost = null;
        public TestSKADAClass()
        {
        }
        public void CreateHost()
        {
            serviceHost = new ServiceHost(typeof(SkadaService));
            NetTcpBinding tcpBinding = new NetTcpBinding();

            serviceHost.AddServiceEndpoint(typeof(ITransactionSteps), tcpBinding,
                                "net.tcp://localhost:20006/SCADA");
            serviceHost.Open();
        }

        public class SkadaService : ITransactionSteps, IModelUpdate
        {
            public bool Commit()
            {
                Console.WriteLine("SCADA commited changes!");
                return true;
            }

            public bool Prepare()
            {
                Console.WriteLine("SCADA OK!");
                return true;
            }

            public void Rollback()
            {
                throw new NotImplementedException();
            }

            public bool UpdateModel(Dictionary<string, List<string>> model)
            {
                Console.WriteLine("Model primljen!");

                NetTcpBinding myBinding = new NetTcpBinding();
                EndpointAddress myEndpoint = new EndpointAddress("net.tcp://localhost:20000/TM");
                ChannelFactory<IEnlistManager> myChannelFactory = new ChannelFactory<IEnlistManager>(myBinding, myEndpoint);

                IEnlistManager proxy = myChannelFactory.CreateChannel();
                // javi se TM-u da si u transakciji
                proxy.Enlist();

                return true;
            }
        }   
    }
}
