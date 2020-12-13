using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace FTN.Services.NetworkModelService
{
    public class TransactionProvider : ITransactionSteps
    {
        public bool Commit()
        {
            try
            {
                NetworkModel.eventHandler?.Invoke(this, "Commit");
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
          
        }

        public bool Prepare()
        {
            try
            {
                NetworkModel.eventHandler?.Invoke(this, "Prepare");
                return true;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public void Rollback()
        {
            try
            {
                NetworkModel.eventHandler?.Invoke(this, "Rollback");
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
