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
using FTN.Services.NetworkModelService.DataModel.Core;
using Microsoft.ServiceFabric.Data;

namespace NetworkModelServiceSF
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class NetworkModelServiceProvider : INetworkModelService
    {
        private IReliableStateManager _stateManager;
        private StatefulServiceContext _context;
        private Func<Delta, Task<UpdateResult>> _applyDelta;
        private Func<long, Task<IdentifiedObject>> _getValue;
        private Func<List<long>, Task<List<IdentifiedObject>>> _getValues;
        public NetworkModelServiceProvider(IReliableStateManager stateManager, StatefulServiceContext context, Func<Delta,Task<UpdateResult>> applyDelta, Func<long, Task<IdentifiedObject>> getValue, Func<List<long>, Task<List<IdentifiedObject>>> getValues)
        {
            _stateManager = stateManager;
            _context = context;
            _applyDelta = applyDelta;
            _getValue = getValue;
            _getValues = getValues;
        }

        public Task<UpdateResult> ApplyDelta(Delta delta)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "NMS - ApplyDelta");
            return _applyDelta(delta);
        }

        public Task<IdentifiedObject> GetValue(long globalId)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "NMS - GetValue");
            return _getValue(globalId);
        }

        public Task<List<IdentifiedObject>> GetValues(List<long> globalIds)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "NMS - GetValues");
            return _getValues(globalIds);
        }
    }
}
