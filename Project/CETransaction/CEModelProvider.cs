using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMContracts;

namespace CETransaction
{
    public class CEModelProvider : IModelUpdate
    {
        public bool ModelUpdate(Dictionary<string, List<string>> par)
        {
            Console.WriteLine("New update request!");
            return true;
        }
    }
}
