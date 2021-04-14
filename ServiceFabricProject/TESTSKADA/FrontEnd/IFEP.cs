using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;

namespace NDS.FrontEnd
{
    [ServiceContract]
    public interface IFEP
    {
        [OperationContract]
        void Start();
        [OperationContract]
        void ExecuteCommand(ScadaCommand command);
    }
}
