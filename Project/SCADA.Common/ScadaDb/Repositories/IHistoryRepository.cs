﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;

namespace SCADA.Common.ScadaDb.Repositories
{
    public interface IHistoryRepository
    {
        void Add(HistoryDbModel model);
        void AddRange(List<HistoryDbModel> list);
        List<HistoryDbModel> GetByTimestamp(DateTime timestamp);
        List<HistoryDbModel> GetInInverval(DateTime from, DateTime to);
        List<HistoryDbModel> GetAll();
        HistoryGraph GetGraph();
    }
}
