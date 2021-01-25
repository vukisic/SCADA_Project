﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;
using SCADA.Common.Proxies;
using SCADA.Common.ScadaServices;
using TMContracts;

namespace SCADATransaction
{
    public class SCADATransactionProvider : ITransactionSteps
    {
        ConversionResult result;
        public bool Prepare()
        {
            ScadaStorageProxy proxy = ScadaProxyFactory.Instance().ScadaStorageProxy();
            Console.WriteLine("Prepared? YES");
            var converter = new ScadaModelConverter();
            result = converter.Convert(proxy.GetCimModel());
            proxy.SetTransactionModel(result.Points);
            proxy.SetDomModel(result.Equipment.Values.ToList());
            return true;
        }

        public bool Commit()
        {
            ScadaStorageProxy proxy = ScadaProxyFactory.Instance().ScadaStorageProxy();
            Console.WriteLine("Commited? YES");
            proxy.SetModel(proxy.GetTransactionModel());
            SCADAServer.updateEvent?.Invoke(this, null);
            ConfigurationChangeInvoker invoker = new ConfigurationChangeInvoker();
            invoker.Update(result.MridIndexPairs);
            invoker = null;
            return true;
        }

        public void Rollback()
        {
            ScadaStorageProxy proxy = ScadaProxyFactory.Instance().ScadaStorageProxy();
            Console.WriteLine("Request for rollback!");
            proxy.SetTransactionModel(null);
        }
    }
}
