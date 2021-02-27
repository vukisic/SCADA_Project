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

        public NetworkModelServiceTransactionProvider(StatefulServiceContext context, NetworkModel networkModel)
        {
            _context = context;
            _networkModel = networkModel;
        }

        public Task<bool> Commit()
        {
            try
            {
                //NetworkModel.eventHandler?.Invoke(this, "Commit");
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
                //NetworkModel.eventHandler?.Invoke(this, "Commit");
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
                //NetworkModel.eventHandler?.Invoke(this, "Commit");
                return _networkModel.Rollback();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
