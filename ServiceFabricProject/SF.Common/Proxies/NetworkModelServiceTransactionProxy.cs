using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class NetworkModelServiceTransactionProxy : ClientBase<ITransactionStepsAsync>
    {
        public NetworkModelServiceTransactionProxy() : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress("net.tcp://localhost:23440/NetworkModelServiceSF"))
        {

        }

        public NetworkModelServiceTransactionProxy(string uri) : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress(uri))
        {

        }


        public async Task<bool> Commit()
        {
            return await Task.Run(async () => { return await Channel.Commit(); });   
        }

        public async Task<bool> Prepare()
        {
            return await Task.Run(async () => { return await Channel.Prepare(); });
        }

        public async Task Rollback()
        {
            await Task.Run(async () => { await Channel.Rollback(); });
        }
    }
}
