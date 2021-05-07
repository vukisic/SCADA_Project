using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common.Models;
using SCADA.Common.ScadaDb.Providers;
using SCADA.Common.ScadaDb.Repositories;

namespace DomService
{
    public class DomServiceProvider : IDomService
    {
        private StatelessServiceContext _context;
        private IDomRepository _repo;
        public DomServiceProvider(StatelessServiceContext context)
        {
            _context = context;
            _repo = new DomRepository(new SCADA.Common.ScadaDb.Access.ScadaDbContext());
        }

        public async Task Add(List<DomDbModel> model)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "DomService - Add");
            await Task.Factory.StartNew(() => { _repo.Add(model); });
        }

        public async Task AddOrUpdate(DomDbModel model)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "DomService - AddOrUpdate");
            await Task.Factory.StartNew(() => { _repo.AddOrUpdate(model); });
        }

        public async Task AddOrUpdateRange(List<DomDbModel> list)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "DomService - AddOrUpdateRange");
            await Task.Factory.StartNew(() => { _repo.AddOrUpdateRange(list); });
        }

        public async Task<List<DomDbModel>> GetAll()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "DomService - GetAll");
            return await Task.FromResult(_repo.GetAll());
        }

        public async Task UpdateSingle(DomDbModel model)
        {
            ServiceEventSource.Current.ServiceMessage(_context, "DomService - UpdateSingle");
            await Task.Factory.StartNew(() => { _repo.UpdateSingle(model); });
        }
    }
}
