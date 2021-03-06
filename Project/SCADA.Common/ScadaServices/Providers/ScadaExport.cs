﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;
using SCADA.Common.DataModel;
using SCADA.Common.Proxies;
using SCADA.Common.ScadaServices.Common;

namespace SCADA.Common.ScadaServices.Providers
{
    public class ScadaExport : IScadaExport
    {
        public Dictionary<string, BasePoint> GetData()
        {
            ScadaStorageProxy proxy = ScadaProxyFactory.Instance().ScadaStorageProxy();
            Dictionary<string, BasePoint> points = new Dictionary<string, BasePoint>();
            var model = proxy.GetModel();
            foreach (var item in model.Values)
            {
                points.Add(item.Mrid, item);
            }
            return points;
        }
    }
}
