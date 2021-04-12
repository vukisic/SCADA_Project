using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common;
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
            ScadaStorageProxy proxy = new ScadaStorageProxy();
            await proxy.SetModel(await proxy.GetTransactionModel());
            var model = await proxy.GetModel();
            ushort aiCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_INPUT).Count());
            ushort aoCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.ANALOG_OUTPUT).Count());
            ushort biCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_INPUT).Count());
            ushort boCount = (ushort)(model.Values.Where(x => x.RegisterType == SCADA.Common.DataModel.RegisterType.BINARY_OUTPUT).Count());
            SimulatorProxy sim = new SimulatorProxy();
            //await sim.UpdateConfig(Tuple.Create<ushort, ushort, ushort, ushort>(biCount, boCount, aiCount, aoCount), result.MridIndexPairs);
            DomServiceProxy dom = new DomServiceProxy();
            //await dom.Add((await proxy.GetModel()).Values.ToList().ToDbModel());
            // ovo .ToDbModel je iz kalse Extension u ScadaCommon-u negde, nadji i kopiraj u servis tu klasu
            return true;
        }

        public async Task<bool> Prepare()
        {
            ScadaStorageProxy proxy = new ScadaStorageProxy();
            var converter = new ScadaModelConverter();
            result = converter.Convert(await proxy.GetCimModel());
            await proxy.SetTransactionModel(result.Points);
            await proxy.SetDomModel(result.Equipment.Values.ToList());
            return true;
        }

        public async Task Rollback()
        {
            ScadaStorageProxy proxy = new ScadaStorageProxy();
            await proxy.SetTransactionModel(null);
        }
    }
}
