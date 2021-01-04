using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Access;
using SCADA.DB.Models;
using SCADA.DB.Providers;
using SCADA.DB.Repositories;
using SCADA.Services.Common;

namespace SCADA.Services.Providers
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

        public List<HistoryDbModel> GetByTimestamp(DateTime timestamp)
        {
            return historyRepository.GetByTimestamp(timestamp);
        }

        public List<HistoryDbModel> GetInInverval(DateTime from, DateTime to)
        {
            return historyRepository.GetInInverval(from, to);
        }
    }
}
