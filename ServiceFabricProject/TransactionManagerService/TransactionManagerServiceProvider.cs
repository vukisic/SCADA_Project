using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Microsoft.ServiceFabric.Data;
using TransactionManager;

namespace TransactionManagerService
{
    [ServiceBehavior(AddressFilterMode = AddressFilterMode.Any)]
    public class TransactionManagerServiceProvider : IEnlistManagerAsync
    {
        public static EnlistManager _enlistManager;
        private IReliableStateManager _stateManager;
        private StatefulServiceContext _context;

        public TransactionManagerServiceProvider(IReliableStateManager stateManager, StatefulServiceContext context)
        {
            _stateManager = stateManager;
            _context = context;
        }

        public async Task EndEnlist(bool isSuccessful)
        {
            await _enlistManager.EndEnlist(isSuccessful);
        }

        public async Task Enlist()
        {
            await _enlistManager.Enlist();
        }

        public async Task<bool> StartEnlist()
        {
            return await _enlistManager.StartEnlist();
        }
    }
}
