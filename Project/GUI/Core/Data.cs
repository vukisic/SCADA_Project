using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.ServiceBus.Events;
using GUI.Models;

namespace GUI.Core
{
    public class Data
    {
        public static List<BasePointDto> Points = new List<BasePointDto>();
        public static List<SwitchingEquipmentDto> Dom = new List<SwitchingEquipmentDto>();
        public static List<HistoryDto> History = new List<HistoryDto>();
        public static List<string> Times = new List<string>();
        public static List<double> Income = new List<double>();
        public static List<float> FluidLevel = new List<float>();
        public static List<PumpsFlows> Flows = new List<PumpsFlows>();
        public static List<PumpsHours> Hours = new List<PumpsHours>();
    }
}
