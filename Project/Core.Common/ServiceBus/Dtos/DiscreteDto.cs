﻿using System.Runtime.Serialization;
using FTN.Common;

namespace Core.Common.ServiceBus.Dtos
{
    [DataContract]
    public class DiscreteDto : IIdentifiedObject
    {
        [DataMember]
        public int MaxValue { get; set; }
        [DataMember]
        public int MinValue { get; set; }
        [DataMember]
        public int NormalValue { get; set; }
        [DataMember]
        public int BaseAddress { get; set; }
        [DataMember]
        public SignalDirection Direction { get; set; }
        [DataMember]
        public MeasurementType MeasurementType { get; set; }
        [DataMember]
        public string ObjectMRID { get; set; }
        [DataMember]
        public long PSR { get; set; } = 0;
        [DataMember]
        public long Terminals { get; set; } = 0;
        [DataMember]
        public string TimeStamp { get; set; }
        [DataMember]
        public string Description { get; set; }
        [DataMember]
        public long GID { get; set; }
        [DataMember]
        public string MRID { get; set; }
        [DataMember]
        public string Name { get; set; }
    }
}
