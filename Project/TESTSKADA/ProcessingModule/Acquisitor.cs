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
        private HistoryProxy historian;
        private int acquisitionInterval;
        private int seconds;
        private int historyInterval;

        /// <summary>
        /// Initializes a new instance of the <see cref="Acquisitor"/> class.
        /// </summary>
        /// <param name="acquisitionTrigger">The acquisition trigger.</param>
        /// <param name="processingManager">The processing manager.</param>
        /// <param name="stateUpdater">The state updater.</param>
		public Acquisitor(IProcessingManager processingManager)
        {
            this.processingManager = processingManager;
            historian = ScadaProxyFactory.Instance().HistoryProxy();
            if (!Int32.TryParse(ConfigurationManager.AppSettings["AcquisitionInterval"], out acquisitionInterval))
                acquisitionInterval = 1000;
            if (!Int32.TryParse(ConfigurationManager.AppSettings["HistoryInterval"], out historyInterval))
                acquisitionInterval = 30;
            seconds = 0;
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
                    //processingManager.ExecuteWriteCommand(SCADA.Common.DataModel.RegisterType.BINARY_OUTPUT, 3, 1);
                    //processingManager.ExecuteWriteCommand(SCADA.Common.DataModel.RegisterType.ANALOG_OUTPUT, 3, 1234);
                    processingManager.ExecuteReadClass0Command();
                    if (++seconds == historyInterval)
                        UpdateHistory();
                }
            }
            catch (Exception ex)
            {
                string message = $"{ex.TargetSite.ReflectedType.Name}.{ex.TargetSite.Name}: {ex.Message}";
            }
        }

        private void UpdateHistory()
        {
            var points = ScadaProxyFactory.Instance().ScadaStorageProxy().GetModel();
            var history = new List<HistoryDbModel>();
            foreach (var item in points.Values)
            {
                if(item.RegisterType == RegisterType.ANALOG_INPUT || item.RegisterType == RegisterType.ANALOG_OUTPUT)
                {
                    history.Add((item as AnalogPoint).ToHistoryDbModel());
                }
                else
                {
                    history.Add((item as DiscretePoint).ToHistoryDbModel());
                }
            }
            historian.AddRange(history);
        }

        #endregion Private Methods

        /// <inheritdoc />
        public void Dispose()
        {
            acquisitionWorker.Abort();
        }
    }
}
