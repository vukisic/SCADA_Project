using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Core.Common.ServiceBus.Dtos;
using Core.Common.ServiceBus.Dtos.Conversion;

namespace Core.Common.ServiceBus.Events
{
    [DataContract]
    public class ModelUpdateEvent
    {
        public ModelUpdateEvent()
        {
        }

        public ModelUpdateEvent(DtosConversionResult dtos)
        {
            Breakers = dtos.Breakers;
            Disconnectors = dtos.Disconnectors;
            Terminals = dtos.Terminals;
            ConnectivityNodes = dtos.ConnectivityNodes;
            Analogs = dtos.Analogs;
            Discretes = dtos.Discretes;
            AsynchronousMachines = dtos.AsynchronousMachines;
            PowerTransformers = dtos.PowerTransformers;
            TransformerWindings = dtos.TransformerWindings;
            RatioTapChangers = dtos.RatioTapChangers;
            Substations = dtos.Substations;
        }

        [DataMember]
        public IEnumerable<BreakerDto> Breakers { get; set; } = Enumerable.Empty<BreakerDto>();

        [DataMember]
        public IEnumerable<DisconnectorDto> Disconnectors { get; set; } = Enumerable.Empty<DisconnectorDto>();

        [DataMember]
        public IEnumerable<TerminalDto> Terminals { get; set; } = Enumerable.Empty<TerminalDto>();

        [DataMember]
        public IEnumerable<ConnectivityNodeDto> ConnectivityNodes { get; set; } = Enumerable.Empty<ConnectivityNodeDto>();

        [DataMember]
        public IEnumerable<AnalogDto> Analogs { get; set; } = Enumerable.Empty<AnalogDto>();

        [DataMember]
        public IEnumerable<DiscreteDto> Discretes { get; set; } = Enumerable.Empty<DiscreteDto>();

        [DataMember]
        public IEnumerable<AsynchronousMachineDto> AsynchronousMachines { get; set; } = Enumerable.Empty<AsynchronousMachineDto>();

        [DataMember]
        public IEnumerable<PowerTransformerDto> PowerTransformers { get; set; } = Enumerable.Empty<PowerTransformerDto>();

        [DataMember]
        public IEnumerable<TransformerWindingDto> TransformerWindings { get; set; } = Enumerable.Empty<TransformerWindingDto>();

        [DataMember]
        public IEnumerable<RatioTapChangerDto> RatioTapChangers { get; set; } = Enumerable.Empty<RatioTapChangerDto>();

        [DataMember]
        public IEnumerable<SubstationDto> Substations { get; set; } = Enumerable.Empty<SubstationDto>();

        /// <summary>
        /// GID of the source item that will be used as root for the tree by GUI
        /// </summary>
        [DataMember]
        public long SourceGid { get; set; } = 21474836482;
    }
}
