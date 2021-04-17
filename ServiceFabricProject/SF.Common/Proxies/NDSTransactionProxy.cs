using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;

namespace SF.Common.Proxies
{
    public class NDSTransactionProxy : ClientBase<ITransactionStepsAsync>
    {
        public NDSTransactionProxy() : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress("net.tcp://localhost:20102/NDSService")) { }
        public NDSTransactionProxy(string uri) : base(new NetTcpBinding(SecurityMode.None), new EndpointAddress(uri)) { }

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
