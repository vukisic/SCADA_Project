using System.Collections.Generic;
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
            foreach (var item in DataBase.Instance.Model.Values)
            {
                points.Add(item.Mrid, item);
            }
            return points;
        }
    }
}
