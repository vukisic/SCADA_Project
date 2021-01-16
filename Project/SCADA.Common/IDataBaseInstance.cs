using System;
using System.Collections.Generic;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;

namespace SCADA.Common
{
    public interface IDataBaseInstance
    {
        Dictionary<DMSType, Container> CimModel { get; set; }
        List<SwitchingEquipment> Dom { get; set; }
        Dictionary<Tuple<RegisterType, int>, BasePoint> Model { get; set; }
        Dictionary<Tuple<RegisterType, int>, BasePoint> TransactionModel { get; set; }
    }
}