﻿using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Common;
using FTN.Services.NetworkModelService;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace CEStorageService
{
    public class CEStorageProvider : ICEStorageAsync
    {
        private StatefulServiceContext _context;
        private static IReliableStateManager _stateManager;
        private const string modelName = "model";
        private const string transactionModelName = "tmodel";

        public CEStorageProvider(StatefulServiceContext context, IReliableStateManager manager)
        {
            _context = context;
            _stateManager = manager;
        }

        public Task<Dictionary<DMSType, Container>> GetModel()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "CE Storage - GetModel");
            return GetInternalModel(modelName);
        }

        public Task<Dictionary<DMSType, Container>> GetTransactionalModel()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "CE Storage - GetTransactionalModel");
            return GetInternalModel(transactionModelName);
        }

        public async Task SetModel(Dictionary<DMSType, Container> model)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "CE Storage - SetModel");
            await SetInternalModel(modelName, model); 
        }

        public Task SetTransactionalModel(Dictionary<DMSType, Container> model)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "CE Storage - SetTransactionalModel");
            return SetInternalModel(transactionModelName, model);
        }

        private async Task SetInternalModel(string name, Dictionary<DMSType, Container> dictionary)
        {

            var result = await _stateManager.GetOrAddAsync<IReliableDictionary<CimModelKey, Container>>(name,TimeSpan.FromSeconds(60));
            if (dictionary == null)
            {
                await result.ClearAsync();
                return;
            }
            await result.ClearAsync();
            using (var tx = _stateManager.CreateTransaction())
            {
                foreach (var item in dictionary)
                {
                    await result.SetAsync(tx, new CimModelKey(item.Key), item.Value,TimeSpan.FromSeconds(60),CancellationToken.None);
                }
                await tx.CommitAsync();
            }
        }

        private async Task<Dictionary<DMSType, Container>> GetInternalModel(string name)
        {
            var model = await _stateManager.GetOrAddAsync<IReliableDictionary<CimModelKey, Container>>(name,TimeSpan.FromSeconds(60));
            return await ConvertCimModel(model);
        }

        private async Task<Dictionary<DMSType, Container>> ConvertCimModel(IReliableDictionary<CimModelKey, Container> dictionary)
        {
            var result = new Dictionary<DMSType, Container>();
            using (var tx = _stateManager.CreateTransaction())
            {
                var enumerable = await dictionary.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(enumerator.Current.Key.Value, enumerator.Current.Value);
                    }
                }
                await tx.CommitAsync();
            }
            return result;
        }

    }
}
