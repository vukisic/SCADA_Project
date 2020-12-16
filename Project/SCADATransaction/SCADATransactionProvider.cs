using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;
using TMContracts;

namespace SCADATransaction
{
    public class SCADATransactionProvider : ITransactionSteps
    {
        public bool Prepare()
        {
            Console.WriteLine("Prepared? YES");
            var converter = new ScadaModelConverter();
            DataBase.TransactionModel = converter.Convert(DataBase.CimModel).Points;
            return true;
        }

        public bool Commit()
        {
            Console.WriteLine("Commited? YES");
            DataBase.Model = DataBase.TransactionModel;
            return true;
        }

        public void Rollback()
        {
            Console.WriteLine("Request for rollback!");
            DataBase.TransactionModel = null;
        }
    }
}
