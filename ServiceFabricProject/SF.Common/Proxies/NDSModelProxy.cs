using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Services.NetworkModelService;

namespace SF.Common.Proxies
{
    public class NDSModelProxy : ClientBase<IModelUpdateAsync>
    {
        public NDSModelProxy() : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress("net.tcp://localhost:20101/NDSService")) { }
        public NDSModelProxy(string uri) : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress(uri)) { }

        public async Task<bool> ModelUpdate(AffectedEntities model)
        {
            return await Task.Run(async () => { return await Channel.ModelUpdate(model); });
        }
    }
}
