using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class CEModelProxy : ClientBase<IModelUpdateAsync>
    {
        public CEModelProxy() : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress("net.tcp://localhost:33331/CEDynamicsService"))
        {

        }

        public CEModelProxy(string uri) : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress(uri))
        {

        }

        public async Task<bool> ModelUpdate(AffectedEntities model)
        {
            return await Task.Run(async () => { return await Channel.ModelUpdate(model); });
        }
    }
}
