using System;
using System.Collections.Generic;
using Core.Common.ServiceBus.Dtos;

namespace GUI.Models.Schema.Helpers
{
    public static class IdentifiedObjectToSchemaModelMapper
    {
        private static readonly Dictionary<Type, Func<IIdentifiedObject, ISchemaModel>> modelFactoryByType;

        static IdentifiedObjectToSchemaModelMapper()
        {
            modelFactoryByType = new Dictionary<Type, Func<IIdentifiedObject, ISchemaModel>>
            {
                [typeof(TransformerModel)] = CastToSchemaModel,
                [typeof(BreakerDto)] = MapToSwitchModel,
                [typeof(DisconnectorDto)] = MapToSwitchModel,
            };
        }

        public static ISchemaModel Map(IIdentifiedObject item, Type itemType)
        {
            if (itemType != null && modelFactoryByType.TryGetValue(itemType, out var factory))
            {
                return factory(item);
            }

            return DefaultMap(item);
        }

        private static ISchemaModel CastToSchemaModel(IIdentifiedObject item)
        {
            return item as ISchemaModel;
        }

        private static ISchemaModel DefaultMap(IIdentifiedObject item)
        {
            return new BaseSchemaModel(item);
        }

        private static ISchemaModel MapToSwitchModel(IIdentifiedObject item)
        {
            return new SwitchModel(item);
        }
    }
}
