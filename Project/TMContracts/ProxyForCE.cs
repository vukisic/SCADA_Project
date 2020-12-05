using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    public class ProxyForCE
    {
        IEnlistManager _proxy;
        public ProxyForCE()
        {
            NetTcpBinding myBinding = new NetTcpBinding();
            EndpointAddress myEndpoint = new EndpointAddress("net.tcp://localhost:20002/CE");
            ChannelFactory<ITransactionSteps> myChannelFactory = new ChannelFactory<ITransactionSteps>(myBinding, myEndpoint);

            ITransactionSteps proxy = myChannelFactory.CreateChannel();
            // Call Service. 
            proxy.Prepare();
            myChannelFactory.Close();
        }

        public void EndEnlist(bool v)
        {
            _proxy.EndEnlist(v);
        }

        public void Enlist()
        {
            _proxy.Enlist();
        }
    }
}
