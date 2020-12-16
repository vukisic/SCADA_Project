using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using SCADA.Common.DataModel;
using FTN.Services.NetworkModelService;

namespace SCADA.Common
{
    public class DataBase
    {
        public static Dictionary<Tuple<RegisterType, int>, BasePoint> Model { get; set; }
        public static Dictionary<Tuple<RegisterType, int>, BasePoint> TransactionModel { get; set; }
        public static Dictionary<DMSType, Container> CimModel { get; set; }
    }
}
