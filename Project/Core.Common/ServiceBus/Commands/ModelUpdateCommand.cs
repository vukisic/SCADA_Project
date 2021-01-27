using System.Collections.Generic;
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

        public IEnumerable<BreakerDto> Breakers { get; set; }
        public IEnumerable<DisconnectorDto> Disconnectors { get; set; }
        public IEnumerable<TerminalDto> Terminals { get; set; }
        public IEnumerable<ConnectivityNodeDto> ConnectivityNodes { get; set; }
        public IEnumerable<AnalogDto> Analogs { get; set; }
        public IEnumerable<DiscreteDto> Discretes { get; set; }
        public IEnumerable<AsynchronousMachineDto> AsynchronousMachines { get; set; }
        public IEnumerable<PowerTransformerDto> PowerTransformers { get; set; }
        public IEnumerable<TransformerWindingDto> TransformerWindings { get; set; }
        public IEnumerable<RatioTapChangerDto> RatioTapChangers { get; set; }
        public IEnumerable<SubstationDto> Substations { get; set; }

        /// <summary>
        /// GID of the source item that will be used as root for the tree by GUI
        /// </summary>
        public long SourceGid { get; set; } = 21474836482;
    }
}
