﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;
using SCADA.Common.DataModel;
using SCADA.Services.Common;

namespace SCADA.Services.Providers
{
    public class ScadaExport : IScadaExport
    {
        public Dictionary<string, BasePoint> GetData()
        {
            Dictionary<string, BasePoint> points = new Dictionary<string, BasePoint>();
            foreach (var item in DataBase.Model.Values)
            {
                points.Add(item.Mrid, item);
            }
            return points;
        }
    }
}
