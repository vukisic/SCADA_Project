﻿using System;
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
                using (var context = new ScadaDbContext())
                {
                    context.History.Add(model);
                    context.SaveChanges();
                }
            }
            
        }

        public void AddRange(List<HistoryDbModel> list)
        {
            lock (_lockObj)
            {
                using (var context = new ScadaDbContext())
                {
                    foreach (var model in list)
                    {
                        context.History.Add(model);
                    }

                    context.SaveChanges();
                }
                
            }
        }

        public List<HistoryDbModel> GetAll()
        {
            List<HistoryDbModel> list;
            lock (_lockObj)
            {
                using (var context = new ScadaDbContext())
                {
                    list = context.History.ToList().Take(100).ToList();
                }
            }
            return list;
        }

        public List<HistoryDbModel> GetByTimestamp(DateTime timestamp)
        {
            string strTimeStamp = timestamp.ToString();
            List<HistoryDbModel> list;
            lock (_lockObj)
            {
                using(var context = new ScadaDbContext())
                {
                    list = context.History.Where(dbm => dbm.TimeStamp == strTimeStamp).ToList();
                }
            }
            return list;
        }

        public List<HistoryDbModel> GetInInverval(DateTime from, DateTime to)
        {
            List<HistoryDbModel> allModels;
            List<HistoryDbModel> filterModels;
            lock (_lockObj)
            {
                using(var context = new ScadaDbContext())
                {
                    allModels = context.History.ToList();
                    filterModels = new List<HistoryDbModel>();
                    foreach (HistoryDbModel model in allModels)
                    {
                        if (DateTime.Parse(model.TimeStamp) >= from && DateTime.Parse(model.TimeStamp) <= to)
                            filterModels.Add(model);
                    }
                }
            }
           
            return filterModels;
        }

        public HistoryGraph GetGraph()
        {
            var history = new HistoryGraph();
            lock (_lockObj)
            {
                using (var context = new ScadaDbContext())
                {
                    var pump1list = context.History.Where(x => x.Mrid == "Flow_AM1").ToList();
                    pump1list.Skip(Math.Max(0, pump1list.Count() - 30)).ToList().ForEach(x => {
                        history.Pump1.XAxe.Add(DateTime.Parse(x.TimeStamp));
                        history.Pump1.YAxe.Add(x.Value);
                    });
                    var pump2list = context.History.Where(x => x.Mrid == "Flow_AM2").ToList();
                    pump2list.Skip(Math.Max(0, pump2list.Count() - 30)).ToList().ForEach(x => {
                        history.Pump2.XAxe.Add(DateTime.Parse(x.TimeStamp));
                        history.Pump2.YAxe.Add(x.Value);
                    });
                    var pump3list = context.History.Where(x => x.Mrid == "Flow_AM3").ToList();
                    pump3list.Skip(Math.Max(0, pump3list.Count() - 30)).ToList().ForEach(x => {
                        history.Pump3.XAxe.Add(DateTime.Parse(x.TimeStamp));
                        history.Pump3.YAxe.Add(x.Value);
                    });

                    var fluidlist = context.History.Where(x => x.Mrid == "FluidLevel_Tank").ToList();
                    fluidlist.Skip(Math.Max(0, fluidlist.Count() - 30)).ToList().ForEach(x => {
                        history.FluidLevel.XAxe.Add(DateTime.Parse(x.TimeStamp));
                        history.FluidLevel.YAxe.Add(x.Value);
                    });
                }
            }
            return history;
        }
    }
}
