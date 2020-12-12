using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using TMContracts;

namespace CETransaction
{
    public class CEModelProvider : IModelUpdate
    {
        public bool ModelUpdate(Dictionary<DMSType, Container> model)
        {
            Console.WriteLine("New update request!");
            return true;
        }
    }
}
