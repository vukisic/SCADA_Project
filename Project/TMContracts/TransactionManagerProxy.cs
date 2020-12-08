using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    public class NMSTransactionManagerProxy
    {
        private IEnlistManager proxy;

        public NMSTransactionManagerProxy()
        {
            ChannelFactory<IEnlistManager> channelFactory = new ChannelFactory<IEnlistManager>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:6001/IEnlistManager"));
            proxy = channelFactory.CreateChannel();
        }

        public void StartEnlist()
        {
            //proxy.StartEnlist();
        }

        public void Enlist()
        {
            proxy.Enlist();
        }

        public void EndEnlist(bool isSuccessful)
        {
            proxy.EndEnlist(isSuccessful);
        }

    }
}
