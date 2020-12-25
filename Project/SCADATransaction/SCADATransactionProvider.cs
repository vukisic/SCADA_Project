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
            var result = converter.Convert(DataBase.CimModel);
            DataBase.TransactionModel = result.Points;
            DataBase.Dom = result.Equipment.Values.ToList();
            return true;
        }

        public bool Commit()
        {
            Console.WriteLine("Commited? YES");
            DataBase.Model = DataBase.TransactionModel;
            SCADAServer.updateEvent?.Invoke(this, null);
            return true;
        }

        public void Rollback()
        {
            Console.WriteLine("Request for rollback!");
            DataBase.TransactionModel = null;
        }
    }
}
