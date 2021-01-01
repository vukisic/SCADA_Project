using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace NDS.FrontEnd
{
    public class UpdateArgs : EventArgs
    {
        public List<BasePoint> Points { get; set; }
    }
}
