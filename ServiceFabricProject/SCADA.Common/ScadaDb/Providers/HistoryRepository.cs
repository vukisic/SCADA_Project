using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;
using SCADA.Common.ScadaDb.Access;
using SCADA.Common.ScadaDb.Repositories;

namespace SCADA.Common.ScadaDb.Providers
{
    public class HistoryRepository : IHistoryRepository
    {
        private ScadaDbContext _context;
        private object _lockObj;
        public HistoryRepository(ScadaDbContext context)
        {
            _context = context;
            _lockObj = new object();
        }
        public void Add(HistoryDbModel model)
        {
            lock (_lockObj)
            {
                _context.History.Add(model);
                _context.SaveChanges();
            }
            
        }

        public void AddRange(List<HistoryDbModel> list)
        {
            lock (_lockObj)
            {
                foreach (var model in list)
                {
                    _context.History.Add(model);
                }

                _context.SaveChanges();
            }
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

        public HistoryGraph GetGraph()
        {
            var history = new HistoryGraph();
            var pump1list = _context.History.Where(x => x.Mrid == "Flow_AM1").ToList();
            pump1list.Skip(Math.Max(0, pump1list.Count() - 100)).ToList().ForEach(x => {
                history.Pump1.XAxe.Add(DateTime.Parse(x.TimeStamp));
                history.Pump1.YAxe.Add(x.Value);
            });
            var pump2list = _context.History.Where(x => x.Mrid == "Flow_AM2").ToList();
            pump2list.Skip(Math.Max(0, pump2list.Count() - 100)).ToList().ForEach(x => {
                history.Pump2.XAxe.Add(DateTime.Parse(x.TimeStamp));
                history.Pump2.YAxe.Add(x.Value);
            });
            var pump3list = _context.History.Where(x => x.Mrid == "Flow_AM3").ToList();
            pump3list.Skip(Math.Max(0, pump3list.Count() - 100)).ToList().ForEach(x => {
                history.Pump3.XAxe.Add(DateTime.Parse(x.TimeStamp));
                history.Pump3.YAxe.Add(x.Value);
            });

            var fluidlist = _context.History.Where(x => x.Mrid == "FluidLevel_Tank").ToList();
            fluidlist.Skip(Math.Max(0, fluidlist.Count() - 100)).ToList().ForEach(x => {
                history.FluidLevel.XAxe.Add(DateTime.Parse(x.TimeStamp));
                history.FluidLevel.YAxe.Add(x.Value);
            });
            return history;
        }
    }
}
