using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    public class ProxyForScada
    {
        ITransactionSteps _proxy;
        public ProxyForScada()
        {
            NetTcpBinding myBinding = new NetTcpBinding();
            EndpointAddress myEndpoint = new EndpointAddress("net.tcp://localhost:20006/SCADA");
            ChannelFactory<ITransactionSteps> myChannelFactory = new ChannelFactory<ITransactionSteps>(myBinding, myEndpoint);

            _proxy = myChannelFactory.CreateChannel();
            // Call Service. 
           // myChannelFactory.Close();
        }
        public bool Prepare()
        {
            return _proxy.Prepare();
        }
        public bool Commit()
        {
            return _proxy.Commit();
        }
    }
}
