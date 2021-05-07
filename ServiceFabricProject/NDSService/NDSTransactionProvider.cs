using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common;
using SF.Common;
using SF.Common.Proxies;

namespace NDSService
{
    public class NDSTransactionProvider : ITransactionStepsAsync
    {
        private StatelessServiceContext _context;
        private ConversionResult result;
        public NDSTransactionProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task<bool> Commit()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "SCADA Transaction - Commit!");
            ScadaStorageProxy proxy = new ScadaStorageProxy(ConfigurationReader.ReadValue(_context, "Settings", "Storage"));
            await proxy.SetModel(await proxy.GetTransactionModel());
            var model = await proxy.GetModel();
            ushort aiCount = (ushort)(model.Values.Where(x => !String.IsNullOrEmpty(x.Mrid) && x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_INPUT).Count());
            ushort aoCount = (ushort)(model.Values.Where(x => !String.IsNullOrEmpty(x.Mrid) && x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_OUTPUT).Count());
            ushort biCount = (ushort)(model.Values.Where(x => !String.IsNullOrEmpty(x.Mrid) && x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_INPUT).Count());
            ushort boCount = (ushort)(model.Values.Where(x => !String.IsNullOrEmpty(x.Mrid) && x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_OUTPUT).Count());
            try 
            { 
                SimulatorProxy sim = new SimulatorProxy();
                sim.UpdateConfig(Tuple.Create<ushort, ushort, ushort, ushort>(biCount, boCount, aiCount, aoCount), result.MridIndexPairs);
            } catch { }
            DomServiceProxy dom = new DomServiceProxy(ConfigurationReader.ReadValue(_context,"Settings","Dom"));
            await dom.Add((await proxy.GetModel()).Values.ToList().ToDbModel());
            return true;
        }

        public async Task<bool> Prepare()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "SCADA Transaction - Prepare!");
            ScadaStorageProxy proxy = new ScadaStorageProxy(ConfigurationReader.ReadValue(_context, "Settings", "Storage"));
            var converter = new ScadaModelConverter();
            result = converter.Convert(await proxy.GetCimModel());
            await proxy.SetTransactionModel(result.Points);
            await proxy.SetDomModel(result.Equipment.Values.ToList());
            return true;
        }

        public async Task Rollback()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "SCADA Transaction - Rollback!");
            ScadaStorageProxy proxy = new ScadaStorageProxy(ConfigurationReader.ReadValue(_context, "Settings", "Storage"));
            await proxy.SetTransactionModel(null);
        }
    }
}
