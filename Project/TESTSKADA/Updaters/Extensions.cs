using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.DB.Models;

namespace NDS.Updaters
{
    public static class Extensions
    {
        public static List<SwitchingEquipment> ToSwitchingEquipment(this List<DomDbModel> list)
        {
            List<SwitchingEquipment> ret = new List<SwitchingEquipment>();
            foreach (var item in list)
            {
                var temp = new SwitchingEquipment()
                {
                    Mrid = item.Mrid,
                    ManipulationConut = item.ManipulationConut
                };
                ret.Add(temp);
            }
            return ret;
        }
    }
}
