using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    public class SCADATransactionProxy
    {
        private ITransactionSteps proxy;

        public SCADATransactionProxy()
        {
            ChannelFactory<ITransactionSteps> channelFactory = new ChannelFactory<ITransactionSteps>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4002/ITransactionSteps"));
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
