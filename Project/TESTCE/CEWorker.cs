using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Calculations;

namespace CE
{
    public class CEWorker : IDisposable
    {
        IFitnessFunction algorithm;
        private Thread _worker;
        public EventHandler<int> _updateEvent = delegate { };
        private bool pointUpdateOccures;
        private bool endFlag;
        private int points = 0;


        public CEWorker()
        {
            _updateEvent += OnPointUpdate;
        }

        public void Start()
        {
            _worker = new Thread(DoWork);
            endFlag = true;
            _worker.Name = "CE Worker";
            _worker.Start();
        }

        public void Stop()
        {
            endFlag = false;
            _worker.Abort();
            _worker = null;
        }

        private void DoWork()
        {
            while (endFlag)
            {
                //if (pointUpdateOccures)
                //    ChangeStrategy();
                // If u want scada measurments use CeProxyFactory.Instance().ScadaExportProxy().GetData();
                // Call Calculations
                // algorithm.Start();
                Thread.Sleep(2000);
            }
        }

        private void ChangeStrategy()
        {
            switch (points)
            {
                case 1: algorithm = new FluidLevelOptimization1(); break;
                case 2: algorithm = new FluidLevelOptimization2(); break;
                case 3: algorithm = new FluidLevelOptimization3(); break;
            }
            pointUpdateOccures = false;
        }

        public void Dispose()
        {
            Stop();
        }

        private void OnPointUpdate(object sender, int e)
        {
            pointUpdateOccures = true;
            points = e;
        }
    }
}
