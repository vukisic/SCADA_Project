using System.Collections.Generic;
using System.Linq;
using Core.Common.ServiceBus.Dtos;
using Core.Common.ServiceBus.Dtos.Conversion;

namespace Core.Common.ServiceBus.Commands
{
    public class ModelUpdateCommand : NServiceBus.ICommand
    {
        public ModelUpdateCommand()
        {

        }

        public ModelUpdateCommand(DtosConversionResult dtos)
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

        public IEnumerable<BreakerDto> Breakers { get; set; } = Enumerable.Empty<BreakerDto>();
        public IEnumerable<DisconnectorDto> Disconnectors { get; set; } = Enumerable.Empty<DisconnectorDto>();
        public IEnumerable<TerminalDto> Terminals { get; set; } = Enumerable.Empty<TerminalDto>();
        public IEnumerable<ConnectivityNodeDto> ConnectivityNodes { get; set; } = Enumerable.Empty<ConnectivityNodeDto>();
        public IEnumerable<AnalogDto> Analogs { get; set; } = Enumerable.Empty<AnalogDto>();
        public IEnumerable<DiscreteDto> Discretes { get; set; } = Enumerable.Empty<DiscreteDto>();
        public IEnumerable<AsynchronousMachineDto> AsynchronousMachines { get; set; } = Enumerable.Empty<AsynchronousMachineDto>();
        public IEnumerable<PowerTransformerDto> PowerTransformers { get; set; } = Enumerable.Empty<PowerTransformerDto>();
        public IEnumerable<TransformerWindingDto> TransformerWindings { get; set; } = Enumerable.Empty<TransformerWindingDto>();
        public IEnumerable<RatioTapChangerDto> RatioTapChangers { get; set; } = Enumerable.Empty<RatioTapChangerDto>();
        public IEnumerable<SubstationDto> Substations { get; set; } = Enumerable.Empty<SubstationDto>();

        /// <summary>
        /// GID of the source item that will be used as root for the tree by GUI
        /// </summary>
        public long SourceGid { get; set; } = 21474836482;
    }
}
