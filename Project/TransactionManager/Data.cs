using System;
using System.Collections.Generic;
using System.Text;

namespace TransactionManager
{
    public class Data
    {
       /* public static SynchronizedCollection<ITransactionSteps> CurrentlyEnlistedServices = new SynchronizedCollection<ITransactionSteps>();

        public static List<ITransactionSteps> CompleteEnlistedServices = new List<ITransactionSteps>();

        public static INotifyNMS NotifyNMSProxy = null;

        public static void CreateNMSProxy()
        {
            NetTcpBinding netTcpbinding = new NetTcpBinding(SecurityMode.None);
            EndpointAddress endpointAddress = new EndpointAddress("net.tcp://localhost:10010/NetworkModelService/NotifyNMS");
            ChannelFactory<INotifyNMS> channelFactory = new ChannelFactory<INotifyNMS>(netTcpbinding, endpointAddress);
            NotifyNMSProxy = channelFactory.CreateChannel();
        }*/
    }
}
