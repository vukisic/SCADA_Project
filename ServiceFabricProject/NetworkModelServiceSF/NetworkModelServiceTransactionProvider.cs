using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Contracts;
using FTN.Services.NetworkModelService;

namespace NetworkModelServiceSF
{
    public class NetworkModelServiceTransactionProvider : ITransactionStepsAsync
    {
        private StatefulServiceContext _context;
        private Func<Task<bool>> _prepare;
        private Func<Task<bool>> _commit;
        private Func<Task> _rollback;

        public NetworkModelServiceTransactionProvider(StatefulServiceContext context, Func<Task<bool>> prepare, Func<Task<bool>> commit, Func<Task> rollback)
        {
            _context = context;
            _prepare = prepare;
            _commit = commit;
            _rollback = rollback;
        }

        public Task<bool> Commit()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "NMS Transaction - Commit!");
            try
            {
                return _commit();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<bool> Prepare()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "NMS Transaction - Prepare!");
            try
            {
                return _prepare();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task Rollback()
        {
            ServiceEventSource.Current.ServiceMessage(_context, "NMS Transaction - Rollback!");
            try
            {
                return _rollback();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
