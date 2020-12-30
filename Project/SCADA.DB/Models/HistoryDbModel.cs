using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using SCADA.Common.DataModel;

namespace SCADA.DB.Models
{
    public class HistoryDbModel
    {
        public long Id { get; set; }
        public ClassType ClassType { get; set; }
        public int Index { get; set; }
        public string Mrid { get; set; }
        public RegisterType RegisterType { get; set; }
        public string TimeStamp { get; set; }
        public string MeasurementType { get; set; }
        public float Value { get; set; }
    }
}
