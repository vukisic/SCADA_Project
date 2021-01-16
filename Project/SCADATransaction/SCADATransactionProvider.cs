using System;
using System.Linq;
using SCADA.Common;
using TMContracts;

namespace SCADATransaction
{
    public class SCADATransactionProvider : ITransactionSteps
    {
        private ConversionResult result;

        public bool Prepare()
        {
            Console.WriteLine("Prepared? YES");

            var converter = new ScadaModelConverter();
            result = converter.Convert(DataBase.Instance.CimModel);
            DataBase.Instance.TransactionModel = result.Points;
            DataBase.Instance.Dom = result.Equipment.Values.ToList();

            return true;
        }

        public bool Commit()
        {
            Console.WriteLine("Commited? YES");

            DataBase.Instance.Model = DataBase.Instance.TransactionModel;

            SCADAServer.updateEvent?.Invoke(this, null);

            ConfigurationChangeInvoker invoker = new ConfigurationChangeInvoker();
            invoker.Update(result.MridIndexPairs);

            return true;
        }

        public void Rollback()
        {
            Console.WriteLine("Request for rollback!");

            DataBase.Instance.TransactionModel = null;
        }
    }
}
