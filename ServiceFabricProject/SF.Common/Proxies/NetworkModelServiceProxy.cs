using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class NetworkModelServiceProxy : ClientBase<INetworkModelService>
    {
        public NetworkModelServiceProxy(EndpointAddress address):base(new NetTcpBinding(SecurityMode.None), address)
        {

        }

        public UpdateResult ApplyDelta(Delta delta)
        {
            return Channel.ApplyDelta(delta).ConfigureAwait(false).GetAwaiter().GetResult();
        }
    }
}
