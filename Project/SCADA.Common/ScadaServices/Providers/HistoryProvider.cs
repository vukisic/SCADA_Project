using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;
using SCADA.Common.ScadaDb.Access;
using SCADA.Common.ScadaDb.Providers;
using SCADA.Common.ScadaDb.Repositories;
using SCADA.Common.ScadaServices.Common;

namespace SCADA.Common.ScadaServices.Providers
{
    public class HistoryProvider : IHistory
    {
        IHistoryRepository historyRepository;

        public HistoryProvider()
        {
            historyRepository = new HistoryRepository(new ScadaDbContext());
        }

        public void Add(HistoryDbModel model)
        {
            historyRepository.Add(model);
        }

        public void AddRange(List<HistoryDbModel> list)
        {
            historyRepository.AddRange(list);
        }

        public List<HistoryDbModel> GetAll()
        {
            var all = historyRepository.GetAll();
            return all.Skip(Math.Max(0, all.Count() - 30)).ToList();
        }

        public List<HistoryDbModel> GetByTimestamp(DateTime timestamp)
        {
            return historyRepository.GetByTimestamp(timestamp);
        }

        public List<HistoryDbModel> GetInInverval(DateTime from, DateTime to)
        {
            return historyRepository.GetInInverval(from, to);
        }

        public HistoryGraph GetGraph()
        {
            return historyRepository.GetGraph();
        }
    }
}
