using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI.Core
{
    public static class Mapper
    {
        public static TDestination Map<TSource, TDestination>(object source, object destination)
        {
            Map(source, destination);
            return (TDestination)destination;
        }

        public static TDestination Map<TDestination>(object source) where TDestination : new()
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
    }
}
