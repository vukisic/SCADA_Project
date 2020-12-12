using System.ServiceModel;

namespace TMContracts
{
    public class NMSProxy
    {
        private readonly ITransactionSteps proxy;

        public NMSProxy()
        {
            ChannelFactory<ITransactionSteps> channelFactory = new ChannelFactory<ITransactionSteps>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4001/ITransactionSteps"));
            proxy = channelFactory.CreateChannel();
        }

        public bool Prepare()
        {
            return proxy.Prepare();
        }

        public bool Commit()
        {
            return proxy.Commit();
        }

        public void Rollback()
        {
            proxy.Rollback();
        }
    }
}
