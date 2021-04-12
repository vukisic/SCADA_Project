using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Models;

namespace NDSService
{
    public static class Extensions
    {
        public static List<DomDbModel> ToDbModel(this List<BasePoint> list)
        {
            List<DomDbModel> model = new List<DomDbModel>();
            foreach (var item in list)
            {
                if (item.RegisterType == RegisterType.BINARY_OUTPUT)
                    model.Add(new DomDbModel() { ManipulationConut = 0, Mrid = item.Mrid });
            }
            return model;
        }
    }
}
