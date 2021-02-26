using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using Microsoft.ServiceFabric.Data;

namespace NetworkModelServiceSF
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class NetworkModelServiceProvider : INetworkModelService
    {
        public static NetworkModel _networkModel;
        private IReliableStateManager _stateManager;
        public NetworkModelServiceProvider(IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
           // _networkModel = new NetworkModel(_stateManager);
        }

        public Task<UpdateResult> ApplyDelta(Delta delta)
        {
            return Task.FromResult(_networkModel.ApplyDelta(delta));
        }
    }
}
