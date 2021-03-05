using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE;
using CE.Common;
using Core.Common.Contracts;

namespace CEService
{
    public class CETransactionProvider : ITransactionStepsAsync
    {
        public static CEWorker cEWorker;
        private StatelessServiceContext _context;

        public CETransactionProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public Task<bool> Commit()
        {
            Console.WriteLine("Commited? YES");
            cEWorker.OnPointUpdate(cEWorker.TPoints);
            return Task.FromResult<bool>(true);
        }

        public Task<bool> Prepare()
        {
            Console.WriteLine("CE Prepared");
            return Task.FromResult<bool>(true);
        }

        public Task Rollback()
        {
            Console.WriteLine("Request for rollback!");
            return Task.CompletedTask;
        }
    }
}
