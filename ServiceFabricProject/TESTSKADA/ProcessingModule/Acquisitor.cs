using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Models;
using SCADA.Common.Proxies;

namespace NDS.ProcessingModule
{
    public class Acquisitor : IDisposable
    {
        private IProcessingManager processingManager;
        private Thread acquisitionWorker;
        private LoggingProxy log;
        private int acquisitionInterval;
        

        /// <summary>
        /// Initializes a new instance of the <see cref="Acquisitor"/> class.
        /// </summary>
        /// <param name="acquisitionTrigger">The acquisition trigger.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="stateUpdater">The state updater.</param>
		public Acquisitor(IProcessingManager processingManager)
        {
            this.processingManager = processingManager;
            log = ScadaProxyFactory.Instance().LoggingProxy();
            if (!Int32.TryParse(ConfigurationManager.AppSettings["AcquisitionInterval"], out acquisitionInterval))
                acquisitionInterval = 1000;
            this.InitializeAcquisitionThread();
            this.StartAcquisitionThread();
        }

        #region Private Methods

        /// <summary>
        /// Initializes the acquisition thread.
        /// </summary>
        private void InitializeAcquisitionThread()
        {
            this.acquisitionWorker = new Thread(Acquisition_DoWork);
            this.acquisitionWorker.Name = "Acquisition thread";
        }

        /// <summary>
        /// Starts the acquisition thread.
        /// </summary>
		private void StartAcquisitionThread()
        {
            acquisitionWorker.Start();
        }

        /// <summary>
        /// Acquisitor thread logic.
        /// </summary>
		private void Acquisition_DoWork()
        {
            try
            {
                while (true)
                {
                    Thread.Sleep(acquisitionInterval);
                    processingManager.ExecuteReadClass0Command();
                }
            }
            catch (Exception ex)
            {
                string message = $"{ex.Message}-{ex.StackTrace}";
                log.Log(new SCADA.Common.Logging.LogEventModel() { EventType = SCADA.Common.Logging.LogEventType.ERROR, Message = message });
            }
        }

        #endregion Private Methods

        /// <inheritdoc />
        public void Dispose()
        {
            acquisitionWorker.Abort();
        }
    }
}
