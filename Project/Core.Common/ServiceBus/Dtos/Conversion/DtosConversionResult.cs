using System.Collections.Generic;

namespace Core.Common.ServiceBus.Dtos.Conversion
{
    public class DtosConversionResult
    {
        public List<BreakerDto> Breakers { get; set; }
        public List<DisconnectorDto> Disconnectors { get; set; }
        public List<TerminalDto> Terminals { get; set; }
        public List<ConnectivityNodeDto> ConnectivityNodes { get; set; }
        public List<AnalogDto> Analogs { get; set; }
        public List<DiscreteDto> Discretes { get; set; }
        public List<AsynchronousMachineDto> AsynchronousMachines { get; set; }
        public List<PowerTransformerDto> PowerTransformers { get; set; }
        public List<TransformerWindingDto> TransformerWindings { get; set; }
        public List<RatioTapChangerDto> RatioTapChangers { get; set; }
        public List<SubstationDto> Substations { get; set; }
    }
}
