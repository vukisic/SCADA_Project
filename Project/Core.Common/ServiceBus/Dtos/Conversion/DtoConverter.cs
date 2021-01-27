using System.Collections.Generic;
using System.Linq;
using FTN.Common;
using FTN.Services.NetworkModelService;
using FTN.Services.NetworkModelService.DataModel.Core;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Topology;
using FTN.Services.NetworkModelService.DataModel.Wires;

namespace Core.Common.ServiceBus.Dtos.Conversion
{
    public static class DtoConverter
    {
        public static DtosConversionResult Convert(Dictionary<DMSType, Container> model)
        {
            DtosConversionResult result = new DtosConversionResult();
            if (model == null)
                return result;
            result.Breakers = MapCollection<BreakerDto, Breaker>(model[DMSType.BREAKER].Entities.Values);
            result.Disconnectors = MapCollection<DisconnectorDto, Disconnector>(model[DMSType.DISCONNECTOR].Entities.Values);
            result.Substations = MapCollection<SubstationDto, Substation>(model[DMSType.SUBSTATION].Entities.Values);
            result.AsynchronousMachines = MapCollection<AsynchronousMachineDto, AsynchronousMachine>(model[DMSType.ASYNCHRONOUSMACHINE].Entities.Values);
            result.PowerTransformers = MapCollection<PowerTransformerDto, PowerTransformer>(model[DMSType.POWERTRANSFORMER].Entities.Values);
            result.TransformerWindings = MapCollection<TransformerWindingDto, TransformerWinding>(model[DMSType.TRANSFORMERWINDING].Entities.Values);
            result.ConnectivityNodes = MapCollection<ConnectivityNodeDto, ConnectivityNode>(model[DMSType.CONNECTIVITYNODE].Entities.Values);
            result.RatioTapChangers = MapCollection<RatioTapChangerDto, RatioTapChanger>(model[DMSType.RATIOTAPCHANGER].Entities.Values);
            result.Terminals = MapCollection<TerminalDto, Terminal>(model[DMSType.TERMINAL].Entities.Values);
            result.Analogs = MapCollection<AnalogDto, Analog>(model[DMSType.ANALOG].Entities.Values);
            result.Discretes = MapCollection<DiscreteDto, Discrete>(model[DMSType.DISCRETE].Entities.Values);
            return result;
        }

        private static TDestination Map<TDestination>(object source) where TDestination : new()
        {
            var destination = new TDestination();
            Map(source, destination);

            return destination;
        }

        private static void Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            foreach (var prop in source.GetType().GetProperties())
            {
                var destProp = destination.GetType().GetProperty(prop.Name);
                if (destProp == null)
                    continue;
                try
                {
                    destProp.SetValue(destination, prop.GetValue(source));
                }
                catch { }
            }
        }

        private static List<T> MapCollection<T, R>(IEnumerable<IdentifiedObject> list) where T : class, new() where R : class
        {
            return list.Select(x => Map<T>(x as R)).ToList();
        }
    }
}
