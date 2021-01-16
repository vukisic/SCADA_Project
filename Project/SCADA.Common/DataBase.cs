using System;

namespace SCADA.Common
{
    public static class DataBase
    {
        private static readonly Lazy<DataBaseInstance> instance = new Lazy<DataBaseInstance>(() => new DataBaseInstance());
        public static IDataBaseInstance Instance
        {
            get
            {
                return instance.Value;
            }
        }
    }
}
