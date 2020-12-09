using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace TMContracts
{
    [ServiceContract]
    public interface IEnlistManager
    {
        [OperationContract]
        bool StartEnlist();

        [OperationContract]
        void Enlist();

        [OperationContract]
        void EndEnlist(bool isSuccessful);
    }
}
