using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Calculations;
using CE.Common.Proxies;

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

        private DNA<float> result;

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
                if (pointUpdateOccures)
                {
                    ChangeStrategy();
                }
                 
                Thread.Sleep(2000);
            }
        }

        private void ChangeStrategy()
        {
            var results = ReadConfiguration();
            switch (points)
            {
                case 1: algorithm = new FluidLevelOptimization1(results.OptimalFluidLevel,results.Percetage,results.TimeFactor,results.Iterations); break;
                case 2: algorithm = new FluidLevelOptimization2(results.OptimalFluidLevel, results.Percetage, results.TimeFactor, results.Iterations); break;
                case 3: algorithm = new FluidLevelOptimization3(results.OptimalFluidLevel, results.Percetage, results.TimeFactor, results.Iterations); break;
            }
               
            result = algorithm.Start();

            //result - najbolje rjesenje (jedinka)
            // result.Genes[0] - prvi gen (da li pumpa radi ili ne radi)
            //ako je case 1 - bice 3 gena - jedna pumpa
            //ako je case 2 - bice 6 gena - dvije pumpe
            //ako je case 3 - bice 9 gena - tri pumpe

            pointUpdateOccures = false;
        }

        private ReadConfigResults ReadConfiguration()
        {
            float percentage;
            float optimalFluidLevel;
            float timeFactor;
            int iterations;
            if (!float.TryParse(ConfigurationManager.AppSettings["Percetage"], out percentage))
            {
                percentage = 5;
            }
            if (!float.TryParse(ConfigurationManager.AppSettings["OptimalFluidLevel"], out optimalFluidLevel))
            {
                optimalFluidLevel = 1000;
            }
            if (!float.TryParse(ConfigurationManager.AppSettings["TimeFactor"], out timeFactor))
            {
                timeFactor = 1800;
            }
            if (!Int32.TryParse(ConfigurationManager.AppSettings["Iterations"], out iterations))
            {
                iterations = 1000;
            }
            return new ReadConfigResults(percentage, optimalFluidLevel, timeFactor, iterations);
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
