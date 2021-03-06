﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;

namespace SF.Common.Proxies
{
    public class NetworkModelServiceProxy : ClientBase<INetworkModelService>
    {
        public NetworkModelServiceProxy():base(new NetTcpBinding(SecurityMode.None), new EndpointAddress("net.tcp://localhost:22330/NetworkModelServiceSF"))
        {

        }

        public NetworkModelServiceProxy(string uri):base(new NetTcpBinding(SecurityMode.None), new EndpointAddress(uri))
        {

        }

        public UpdateResult ApplyDelta(Delta delta)
        {
            return Channel.ApplyDelta(delta).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public Task<IdentifiedObject> GetValue(long globalId)
        {
            return Channel.GetValue(globalId);
        }

        public Task<List<IdentifiedObject>> GetValues(List<long> globalIds)
        {
            return Channel.GetValues(globalIds);
        }
    }
}
