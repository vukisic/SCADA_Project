using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;

namespace NDS.ProcessingModule
{
    public interface IProcessingManager
    {
        void ExecuteWriteCommand(RegisterType type, uint index, uint value);
        void ExecuteReadCommand(RegisterType type, uint index);
        void ExecuteReadClass0Command();
    }
}
