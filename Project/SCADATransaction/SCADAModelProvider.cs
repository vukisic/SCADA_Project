using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using SCADA.Common;
using TMContracts;

namespace SCADATransaction
{
    public class SCADAModelProvider : IModelUpdate
    {
        public bool ModelUpdate(Dictionary<DMSType, Container> model)
        {
            Console.WriteLine("New update request!");
            var converter = new ScadaModelConverter();
            var result = converter.Convert(model);
            foreach (var item in result.Equipment.Values)
            {
                Console.WriteLine($"{item.Mrid} - {item.ManipulationConut}");
            }
            foreach (var item in result.Points.Values)
            {
                Console.WriteLine($"{item.Mrid} - {item.RegisterType} - {item.Index}");
            }
            //dobio si model, javi se da ucestvujes u transakciji
            EnList();
            return true;
        }
        public void EnList()
        {
            TransactionManagerProxy proxyForTM = new TransactionManagerProxy();
            proxyForTM.Enlist();
        }
    }
}
