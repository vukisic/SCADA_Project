using System;
using System.Collections.Generic;
using System.Text;

namespace Contracts
{
    public interface ITransactionSteps
    {
        bool Prepare();
        bool Commit();
        void Rollback();
    }
}
