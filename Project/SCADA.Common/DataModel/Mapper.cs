using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.DataModel
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

        public static List<TDestination> MapCollection<TSource, TDestination>(List<TSource> list) where TSource : class where TDestination : class, new()
        {
            var result = new List<TDestination>();
            list.ForEach(x => result.Add(Map<TDestination>(x)));
            return result;
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
