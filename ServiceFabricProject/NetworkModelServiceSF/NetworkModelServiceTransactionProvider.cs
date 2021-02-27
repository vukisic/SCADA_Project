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
        public static NetworkModel _networkModel;

        public NetworkModelServiceTransactionProvider(StatefulServiceContext context)
        {
            _context = context;
        }

        public Task<bool> Commit()
        {
            try
            {
                return _networkModel.Commit();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task<bool> Prepare()
        {
            try
            {
                return _networkModel.Prepare();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public Task Rollback()
        {
            try
            {
                return _networkModel.Rollback();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
