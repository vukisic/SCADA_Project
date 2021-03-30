using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using Microsoft.ServiceFabric.Data;
using TransactionManager;

namespace TransactionManagerService
{
    public class TransactionManagerServiceProvider : IEnlistManagerAsync
    {
        public static EnlistManager _enlistManager;
        private IReliableStateManager _stateManager;
        private StatefulServiceContext _context;

        public TransactionManagerServiceProvider(IReliableStateManager stateManager, StatefulServiceContext context)
        {
            _enlistManager = new EnlistManager(stateManager);
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
