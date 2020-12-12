using System.ServiceModel;

namespace TMContracts
{
    public class TransactionManagerProxy
    {
        private readonly IEnlistManager proxy;

        public TransactionManagerProxy()
        {
            ChannelFactory<IEnlistManager> channelFactory = new ChannelFactory<IEnlistManager>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:6001/IEnlistManager"));
            proxy = channelFactory.CreateChannel();
        }

        public bool StartEnlist()
        {
            return proxy.StartEnlist();
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
