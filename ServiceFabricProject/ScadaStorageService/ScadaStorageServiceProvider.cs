using System;
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
using SCADA.Common.DataModel;
using SCADA.Common.Models;
using SCADA.Common.ScadaDb.Providers;
using SCADA.Common.ScadaDb.Repositories;
using SF.Common;
using SF.Common.Proxies;

namespace ScadaStorageService
{
    public class ScadaStorageServiceProvider : IScadaStorageService
    {
        #region Fields
        private StatefulServiceContext _context;
        private static IReliableStateManager _stateManager;
        private const string modelName = "model";
        private const string transactionModelName = "tmodel";
        private const string swName = "swe";
        private const string cimName = "cim";
        private static IReplicationRepository _repo;
        private HistoryServiceProxy historian;
        #endregion
        public ScadaStorageServiceProvider(StatefulServiceContext context, IReliableStateManager stateManager)
        {
            _context = context;
            _stateManager = stateManager;
            _repo = new ReplicationRepository(new SCADA.Common.ScadaDb.Access.ScadaDbContext());
        }

        public static async Task Initialize()
        {
            var dictionary = _repo.Get();
            var result = await _stateManager.GetOrAddAsync<IReliableDictionary<int, BasePoint>>(modelName);
            if (dictionary == null)
            {
                await result.ClearAsync();
                return;
            }
            using (var tx = _stateManager.CreateTransaction())
            {
                foreach (var item in dictionary)
                {
                    await result.SetAsync(tx, MakeKey(item.Key.Item1,item.Key.Item2), item.Value);
                }
                await tx.CommitAsync();
            }
        }

        public async Task<Dictionary<DMSType, FTN.Services.NetworkModelService.Container>> GetCimModel()
        {
            var result = await GetCim();
            return await ConvertCimModel(result);
        }

        public async Task<List<SwitchingEquipment>> GetDomModel()
        {
            return await GetDom();
        }

        public async Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> GetModel()
        {
            var result = await GetScadaModel(modelName);
            return await ConvertScadaModel(result);
        }

        public async Task<BasePoint> GetSingle(RegisterType type, int index)
        {
            var result = await GetScadaModel(modelName);
            using(var tx = _stateManager.CreateTransaction())
            {
                ConditionalValue<BasePoint> point = await result.TryGetValueAsync(tx, MakeKey(type,index));
                if (point.HasValue)
                {
                    if(point.Value == null)
                    {
                        await tx.CommitAsync();
                        return null;
                    }
                    else
                    {
                        await tx.CommitAsync();
                        return point.Value;
                    }
                }
                else
                {
                    await tx.CommitAsync();
                    return null;
                }
            }
        }

        public async Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> GetTransactionModel()
        {
            var tmodel = await GetScadaModel(transactionModelName);
            return await ConvertScadaModel(tmodel);
        }

        public async Task SetCimModel(Dictionary<DMSType, FTN.Services.NetworkModelService.Container> model)
        {
            await SetCim(model);
        }

        public async Task SetDomModel(List<SwitchingEquipment> model)
        {
            await SetDom(model);
        }

        public async Task SetModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            _repo.Set(model.Values.ToList());
            await SetScadaModel(modelName, model);
        }

        public async Task SetTransactionModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            await SetScadaModel(transactionModelName, model);
        }

        public async Task UpdateModelValue(Dictionary<Tuple<RegisterType, int>, BasePoint> updateModel)
        {
            await SetScadaModel(modelName, updateModel);
        }

        private async Task<IReliableDictionary<int, BasePoint>> GetScadaModel(string name)
        {
            return await _stateManager.GetOrAddAsync<IReliableDictionary<int, BasePoint>>(name);
        }

        private async Task SetScadaModel(string name, Dictionary<Tuple<RegisterType,int>, BasePoint> dictionary)
        {
            historian = new HistoryServiceProxy(ConfigurationReader.ReadValue(_context,"Settings","History"));
            var result = await _stateManager.GetOrAddAsync<IReliableDictionary<int, BasePoint>>(name);
            if (dictionary == null)
            {
                await result.ClearAsync();
                return;
            }
            using (var tx = _stateManager.CreateTransaction())
            {
                var count = await result.GetCountAsync(tx);
                var historyData = new List<HistoryDbModel>();
                foreach (var item in dictionary)
                {
                    var point = await result.TryGetValueAsync(tx, MakeKey(item.Key.Item1, item.Key.Item2));
                    if (point.HasValue)
                    {
                        switch (item.Key.Item1)
                        {
                            case RegisterType.BINARY_INPUT:
                            case RegisterType.BINARY_OUTPUT:
                                {
                                    ((DiscretePoint)point.Value).TimeStamp = (item.Value as DiscretePoint).TimeStamp;
                                    if (((DiscretePoint)point.Value).Value != (item.Value as DiscretePoint).Value)
                                    {
                                        ((DiscretePoint)point.Value).Value = (item.Value as DiscretePoint).Value;
                                        historyData.Add((((DiscretePoint)point.Value)).ToHistoryDbModel());
                                    }
                                    break;
                                }
                            case RegisterType.ANALOG_INPUT:
                            case RegisterType.ANALOG_OUTPUT:
                                {
                                    ((AnalogPoint)point.Value).TimeStamp = (item.Value as AnalogPoint).TimeStamp;
                                    if (((AnalogPoint)point.Value).Value != (item.Value as AnalogPoint).Value)
                                    {
                                        ((AnalogPoint)point.Value).Value = (item.Value as AnalogPoint).Value;
                                        historyData.Add((((AnalogPoint)point.Value)).ToHistoryDbModel());
                                    }
                                   
                                    break;
                                }
                        }
                        await result.SetAsync(tx, MakeKey(item.Key.Item1, item.Key.Item2), point.Value);
                    }
                    else
                    {
                        await result.SetAsync(tx, MakeKey(item.Key.Item1, item.Key.Item2), item.Value);
                    }
                }
                await tx.CommitAsync();
                if(historyData.Count > 0)
                    await historian.AddRange(historyData);
            }
        }

        private async Task<Dictionary<Tuple<RegisterType,int>, BasePoint>> ConvertScadaModel(IReliableDictionary<int, BasePoint> dictionary)
        {
            var result = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            using (var tx = _stateManager.CreateTransaction())
            {
                var enumerable = await dictionary.CreateEnumerableAsync(tx);
                using (var enumerator = enumerable.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        result.Add(Tuple.Create(enumerator.Current.Value.RegisterType, enumerator.Current.Value.Index), enumerator.Current.Value);
                    }
                }
                await tx.CommitAsync();
            }
            return result;
        }

        private async Task<List<SwitchingEquipment>> GetDom()
        {
            var result =  await _stateManager.GetOrAddAsync<IReliableDictionary<string, List<SwitchingEquipment>>>(swName);
            using(var tx = _stateManager.CreateTransaction())
            {
               ConditionalValue<List<SwitchingEquipment>> value = await result.TryGetValueAsync(tx, swName);
                if (value.HasValue)
                {
                    if(value.Value == null)
                    {
                        await tx.CommitAsync();
                        return null;
                    }
                    else
                    {
                        await tx.CommitAsync();
                        return value.Value;
                    }
                }
                else
                {
                    await tx.CommitAsync();
                    return null;
                }
            }
        }

        private async Task SetDom(List<SwitchingEquipment> list)
        {
            var result = await _stateManager.GetOrAddAsync<IReliableDictionary<string, List<SwitchingEquipment>>>(swName);
            if (list == null)
            {
                await result.ClearAsync();
                return;
            }
            using (var tx = _stateManager.CreateTransaction())
            {
                await result.ClearAsync();
                await result.SetAsync(tx, swName, list);
                await tx.CommitAsync();
            }
        }

        private async Task<IReliableDictionary<CimModelKey, Container>> GetCim()
        {
            using (var tx = _stateManager.CreateTransaction())
            {
                var result = await _stateManager.GetOrAddAsync<IReliableDictionary<CimModelKey, Container>>(tx,cimName,TimeSpan.FromSeconds(60));
                await tx.CommitAsync();
                return result;
            }
            
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

        private async Task SetCim(Dictionary<DMSType, Container> dictionary)
        {

            var result = await _stateManager.GetOrAddAsync<IReliableDictionary<CimModelKey, Container>>(cimName);
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
                    await result.SetAsync(tx, new CimModelKey(item.Key), item.Value);
                }
                await tx.CommitAsync();
            }
        }

        private static int MakeKey(RegisterType registerType, int index)
        {
            var bytes = new byte[4];
            var regBytes = BitConverter.GetBytes((short)registerType);
            var indexBytes = BitConverter.GetBytes((short)index);
            Buffer.BlockCopy(regBytes, 0, bytes, 0, 2);
            Buffer.BlockCopy(indexBytes, 0, bytes, 2, 2);
            return BitConverter.ToInt32(bytes, 0);
        }


    }
}
