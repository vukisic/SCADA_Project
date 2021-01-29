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
        private CommandingService commandingService;
        private IProcessingManager processingManager;
        private IFunctionExecutor functionExecutor;
        private AutoResetEvent autoResetEvent;
        public FEP()
        {
            functionExecutor = new FunctionExecutor();
            processingManager = new ProcessingManager(functionExecutor);
            autoResetEvent = new AutoResetEvent(false);
            acquisitor = new Acquisitor(autoResetEvent, processingManager);
            commandingService = new CommandingService(autoResetEvent, processingManager);
        }
    }
}
