using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    public class CalculationEngineTransactionProxy
    {
        private ITransactionSteps proxy;

        public CalculationEngineTransactionProxy()
        {
            ChannelFactory<ITransactionSteps> channelFactory = new ChannelFactory<ITransactionSteps>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4003/ITransactionSteps"));
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
