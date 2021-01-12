using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Access;
using SCADA.DB.Models;
using SCADA.DB.Repositories;

namespace SCADA.DB.Providers
{
    public class HistoryRepository : IHistoryRepository
    {
        private ScadaDbContext _context;

        public HistoryRepository(ScadaDbContext context)
        {
            _context = context;
        }
        public void Add(HistoryDbModel model)
        {
            _context.History.Add(model);
            _context.SaveChanges();
        }

        public void AddRange(List<HistoryDbModel> list)
        {
            foreach (var model in list)
            {
                _context.History.Add(model);
            }

            _context.SaveChanges();
        }

        public List<HistoryDbModel> GetAll()
        {
            return _context.History.ToList();
        }

        public List<HistoryDbModel> GetByTimestamp(DateTime timestamp)
        {
            string strTimeStamp = timestamp.ToString();
            return _context.History.Where(dbm => dbm.TimeStamp == strTimeStamp).ToList();
        }

        public List<HistoryDbModel> GetInInverval(DateTime from, DateTime to)
        {
            List<HistoryDbModel> allModels = _context.History.ToList();
            List<HistoryDbModel> filterModels = new List<HistoryDbModel>();
            foreach(HistoryDbModel model in allModels)
            {
                if (DateTime.Parse(model.TimeStamp) >= from && DateTime.Parse(model.TimeStamp) <= to)
                    filterModels.Add(model);
            }

            return filterModels;
        }
    }
}
