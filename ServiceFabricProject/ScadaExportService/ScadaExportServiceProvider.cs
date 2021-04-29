using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common.DataModel;
using SF.Common;
using SF.Common.Proxies;

namespace ScadaExportService
{
    public class ScadaExportServiceProvider : IScadaExportServiceAsync
    {
        private StatelessServiceContext _context;
        public ScadaExportServiceProvider(StatelessServiceContext context)
        {
            _context = context;
        }
        public async Task<Dictionary<string, BasePoint>> GetData()
        {
            try
            {
                ScadaStorageProxy proxy = new ScadaStorageProxy(ConfigurationReader.ReadValue(_context,"Settings","Storage"));
                var model = await proxy.GetModel();
                var data = new Dictionary<string, BasePoint>();
                foreach (var item in model.Values)
                {
                    if(item.Mrid != null)
                        data.Add(item.Mrid, item);
                }
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
