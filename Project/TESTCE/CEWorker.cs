using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Calculations;
using CE.Common.Proxies;
using CE.Data;
using Core.Common.WeatherApi;
using SCADA.Common.DataModel;

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
        private WeatherAPI weatherAPI;

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
            weatherAPI = new WeatherAPI();
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
                if(points > 0 && points < 4)
                {
                    if (pointUpdateOccures)
                    {
                        ChangeStrategy();
                    }

                    var forecastResult = new CeForecast();
                    var area = GetSurfaceArea();
                    var weatherForecast = weatherAPI.GetResultsForNext6Hours();
                    weatherForecast.ForEach(x => x = x * area);
                    float current = GetCurrentFluidLevel();
                    for (int i = 0; i < weatherForecast.Count; i++)
                    {
                        current += (float)weatherForecast[i];
                        //result = algorithm.Start(current);
                        //var processedResult = ProcessResults(current, result);
                        //forecastResult.Results.AddRange(processedResult);
                        //current = processedResult.Last().EndFluidLevel;
                    }

                    // Update & Command
                    //Thread.Sleep(10800000); // 3hrs
                    // Test
                    Thread.Sleep(2000);
                }
               
            }
        }

        private List<CeForecastResult> ProcessResults(float current, DNA<float> result)
        {
            var results = new List<CeForecastResult>();
            float totalPerIteration = (GetTotalFromResults(result.Genes) / 4);
            for(int i = 0; i< 4; i++)
            {
                var item = new CeForecastResult();
                item.Result = result;
                item.StartFluidLevel = current;
                item.EndFluidLevel = current - totalPerIteration;
                current -= totalPerIteration;
                
                for (int j = 0; j < result.Genes.Count(); j+=3)
                {
                    item.Pumps.Add(result.Genes[j]);
                    item.Times.Add(result.Genes[j+1]);
                    item.Flows.Add(result.Genes[j+2]);
                }
                results.Add(item);
            }
            return results;
        }

        private float GetCurrentFluidLevel()
        {
            ScadaExportProxy proxy = new ScadaExportProxy();
            var point = (proxy.GetData()["FluidLevel_Tank"] as AnalogPoint);
            return point.Value;
        }

        private float GetTotalFromResults(float[] results)
        {
            float total = 0;
            for (int i = 0; i < results.Count(); i+=3)
            {
                total += (results[i] * results[i+1] * results[i+2]);
            }

            return total;
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

        private float GetSurfaceArea()
        {
            float area;
            if (!float.TryParse(ConfigurationManager.AppSettings["Surface"], out area))
            {
                area = 10000f;
            }
            return area;
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
