using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace ConsoleApp2
{
    
    public class ModelUpdate : IModelUpdate
    {
        public bool UpdateModel(Dictionary<string, List<string>> model)
        {
            Console.WriteLine("New update request from NMS");
            Console.WriteLine("Checking if SCADA is prepared..");

            ProxyForScada proxy1 = new ProxyForScada();

            bool ret = proxy1.Prepare();

            if (ret)
                Console.WriteLine("SCADA returned OK!");

            return true;
        }
    }
}
