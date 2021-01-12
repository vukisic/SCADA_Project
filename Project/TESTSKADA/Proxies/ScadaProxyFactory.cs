
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.Proxies
{
    public class ScadaProxyFactory
    {
        private static ScadaProxyFactory instance;
        private static object obj = new object();

        private ScadaProxyFactory() { }

        public static ScadaProxyFactory Instance()
        {
            lock (obj)
            {
                if (instance == null)
                    instance = new ScadaProxyFactory();
                return instance;
            }
        }

        public AlarmKruncingProxy AlarmKruncingProxy()
        {
            return new AlarmKruncingProxy();
        }

        public DOMProxy DOMProxy()
        {
            return new DOMProxy();
        }

        public HistoryProxy HistoryProxy()
        {
            return new HistoryProxy();
        }

        public LoggingProxy LoggingProxy()
        {
            return new LoggingProxy();
        }

    }
}
