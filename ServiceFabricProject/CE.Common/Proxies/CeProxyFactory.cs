namespace CE.Common.Proxies
{
    public class CeProxyFactory
    {
        private static CeProxyFactory instance;
        private static object obj = new object();

        private CeProxyFactory() { }

        public static CeProxyFactory Instance()
        {
            lock (obj)
            {
                if (instance == null)
                    instance = new CeProxyFactory();
                return instance;
            }
        }

        public ScadaExportProxy ScadaExportProxy()
        {
            return new ScadaExportProxy();
        }
    }
}
