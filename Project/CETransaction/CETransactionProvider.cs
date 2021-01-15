﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CE.Common;
using TMContracts;

namespace CETransaction
{
    public class CETransactionProvider : ITransactionSteps
    {
        public bool Prepare()
        {
            Console.WriteLine("Prepared? YES");
            return true;
        }
        public bool Commit()
        {
            Console.WriteLine("Commited? YES");
            CEServer._pointUpdate.Invoke(this, CeDataBase.Model);
            return true;
        }

        public void Rollback()
        {
            Console.WriteLine("Request for rollback!");
        }
    }
}
