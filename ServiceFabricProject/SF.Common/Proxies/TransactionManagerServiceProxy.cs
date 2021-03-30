using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;

namespace SF.Common.Proxies
{
    public class TransactionManagerServiceProxy : ClientBase<IEnlistManagerAsync>
    {
        public TransactionManagerServiceProxy() : base(new NetTcpBinding(SecurityMode.None), 
            new EndpointAddress("net.tcp://localhost:22330/TransactionManagerService"))
        {

        }

        public async Task<bool> StartEnlist()
        {
            return await Task.Run(async () => { return await Channel.StartEnlist(); });
        }

        public async Task Enlist()
        {
            await Task.Run(async () => { await Channel.Enlist(); });
        }

        public async Task EndEnlist(bool isSuccessful)
        {
            await Task.Run(async () => { await Channel.EndEnlist(isSuccessful); });
        }
    }
}
