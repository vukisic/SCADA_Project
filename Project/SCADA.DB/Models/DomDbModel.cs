using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.DB.Models
{
    public class DomDbModel
    {
        public long Id { get; set; }
        public string Mrid { get; set; }
        public int ManipulationConut { get; set; }
        public string TimeStamp { get; set; }
    }
}
