using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common
{
    public class WcfClient<T> : ServicePartitionClient<WcfCommunicationClient<T>> where T : class
    {
        public WcfClient(ICommunicationClientFactory<WcfCommunicationClient<T>> communicationClientFactory,
                                                   Uri serviceUri,
                                                   ServicePartitionKey partitionKey = null,
                                                   TargetReplicaSelector targetReplicaSelector = TargetReplicaSelector.Default,
                                                   string listenerName = null,
                                                   OperationRetrySettings retrySettings = null)
            : base(communicationClientFactory, serviceUri, partitionKey, targetReplicaSelector, listenerName, retrySettings)
        {

        }
    }
}
