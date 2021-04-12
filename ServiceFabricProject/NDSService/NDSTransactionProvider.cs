using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;

namespace NDSService
{
    public class NDSTransactionProvider : ITransactionStepsAsync
    {
        private StatelessServiceContext _context;
        public NDSTransactionProvider(StatelessServiceContext context)
        {
            _context = context;
        }

        public Task<bool> Commit()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Prepare()
        {
            throw new NotImplementedException();
        }

        public Task Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
