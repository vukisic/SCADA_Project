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

namespace HistoryService
{
    public class HistoryServiceProvider : IHistoryService
    {
        private StatelessServiceContext _context;
        private IHistoryRepository _repo;

        public HistoryServiceProvider(StatelessServiceContext context)
        {
            _context = context;
            _repo = new HistoryRepository(new SCADA.Common.ScadaDb.Access.ScadaDbContext());
        }

        public async Task Add(HistoryDbModel model)
        {
            await Task.Factory.StartNew(() => _repo.Add(model));
        }

        public async Task AddRange(List<HistoryDbModel> list)
        {
            await Task.Factory.StartNew(() => _repo.AddRange(list));
        }

        public async Task<List<HistoryDbModel>> GetAll()
        {
            return await Task.FromResult(_repo.GetAll());
        }

        public async Task<List<HistoryDbModel>> GetByTimestamp(DateTime timestamp)
        {
            return await Task.FromResult(_repo.GetByTimestamp(timestamp));
        }

        public async Task<List<HistoryDbModel>> GetInInverval(DateTime from, DateTime to)
        {
            return await Task.FromResult(_repo.GetInInverval(from, to));
        }

        public async Task<HistoryGraph> GetGraph()
        {
            return await Task.FromResult(_repo.GetGraph());
        }
    }
}
