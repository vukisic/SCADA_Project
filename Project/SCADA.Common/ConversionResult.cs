﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace SCADA.Common
{
    public class ConversionResult
    {
        public bool Success { get; set; }
        public Dictionary<Tuple<RegisterType, int>, BasePoint> Points { get; set; }
        public Dictionary<string, SwitchingEquipment> Equipment { get; set; }
        public Dictionary<string, ushort> MridIndexPairs { get; set; }

        public ConversionResult()
        {
            Success = false;
            Points = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            Equipment = new Dictionary<string, SwitchingEquipment>();
            MridIndexPairs = new Dictionary<string, ushort>();
        }
    }
}
