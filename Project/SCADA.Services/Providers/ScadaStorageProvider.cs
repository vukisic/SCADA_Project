﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common.DataModel;
using SCADA.Services.Common;

namespace SCADA.Services.Providers
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ScadaStorageProvider : IScadaStorage
    {
        public static Dictionary<Tuple<RegisterType, int>, BasePoint> Model { get; set; }
        public static Dictionary<Tuple<RegisterType, int>, BasePoint> TransactionModel { get; set; }
        public static Dictionary<DMSType, Container> CimModel { get; set; }
        public static List<SwitchingEquipment> Dom { get; set; }

        public Dictionary<DMSType, Container> GetCimModel()
        {
            return CimModel;
        }

        public List<SwitchingEquipment> GetDomModel()
        {
            return Dom;
        }

        public Dictionary<Tuple<RegisterType, int>, BasePoint> GetModel()
        {
            return Model;
        }

        public Dictionary<Tuple<RegisterType, int>, BasePoint> GetTransactionModel()
        {
            return TransactionModel;
        }

        public void SetCimModel(Dictionary<DMSType, Container> model)
        {
            CimModel = model;
        }

        public void SetDomModel(List<SwitchingEquipment> model)
        {
            Dom = model;
        }

        public void SetModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            Model = model;
        }

        public void SetTransactionModel(Dictionary<Tuple<RegisterType, int>, BasePoint> model)
        {
            TransactionModel = model;
        }
    }
}
