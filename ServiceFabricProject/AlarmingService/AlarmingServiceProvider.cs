using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using SCADA.Common.DataModel;
using SCADA.Common.ScadaServices.Providers;

namespace AlarmingService
{
    public class AlarmingServiceProvider : IAlarmingService
    {
        private StatelessServiceContext _context;
        private AlarmKruncingProvider _alarmKruncingProvider;
        public AlarmingServiceProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<Tuple<RegisterType, int>, BasePoint>> Check(Dictionary<Tuple<RegisterType, int>, BasePoint> points)
        {
            return await Task.Factory.StartNew(() => _alarmKruncingProvider.Check(points));
        }
    }
}
