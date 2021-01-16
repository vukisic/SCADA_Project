using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;

namespace SCADA.Common
{
    public class DataBaseInstance : IDataBaseInstance
    {
        public Dictionary<Tuple<RegisterType, int>, BasePoint> Model { get; set; }
        public Dictionary<Tuple<RegisterType, int>, BasePoint> TransactionModel { get; set; }
        public Dictionary<DMSType, Container> CimModel { get; set; }
        public List<SwitchingEquipment> Dom { get; set; }
    }
}
