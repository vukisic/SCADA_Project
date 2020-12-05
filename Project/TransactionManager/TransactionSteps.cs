using System;
using System.Collections.Generic;
using System.Text;
using Contracts;

namespace TransactionManager
{
    public class TransactionSteps : ITransactionSteps
    {
        public bool Commit()
        {
            throw new NotImplementedException();
        }

        public bool Prepare()
        {
            throw new NotImplementedException();
        }

        public void Rollback()
        {
            throw new NotImplementedException();
        }
    }
}
