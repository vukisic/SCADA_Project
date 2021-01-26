using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NDS.ProcessingModule;
using SCADA.Common;
using SCADA.Common.Connection;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging;
using SCADA.Common.Messaging.Parameters;
using SCADA.Common.Proxies;
using SCADA.Common.ScadaServices;

namespace NDS.FrontEnd
{
    public class FEP : IFEP
    {
        private Acquisitor acquisitor;
        private IProcessingManager processingManager;
        private IFunctionExecutor functionExecutor;
        public FEP()
        {
            functionExecutor = new FunctionExecutor();
            processingManager = new ProcessingManager(functionExecutor);
            acquisitor = new Acquisitor(new AutoResetEvent(false), new ProcessingManager(new FunctionExecutor()));
        }
    }
}
