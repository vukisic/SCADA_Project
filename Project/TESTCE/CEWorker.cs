﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Calculations;
using CE.Common.Proxies;
using CE.Data;
using CE.ServiceBus;
using Core.Common.ServiceBus.Events;
using Core.Common.WeatherApi;
using NServiceBus;
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
        private IEndpointInstance endpoint;

        private DNA<float> result;

        public CEWorker()
        {
            _updateEvent += OnPointUpdate;
            endpoint = ServiceBusStartup.StartInstance("CE").GetAwaiter().GetResult();
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
                if (points > 0 && points < 4)
                {
                    if (pointUpdateOccures)
                    {
                        ChangeStrategy();
                    }


                    var forecastResult = new CeForecast();
                    var area = GetSurfaceArea();
                    var weatherForecast = weatherAPI.GetResultsForNext6Hours();
                    var weather = new List<double>();
                    weatherForecast.ForEach(x => weather.Add(x * area));
                    float current = GetCurrentFluidLevel();

                    for (int i = 0; i < weatherForecast.Count; i++)
                    {
                        current += (float)weather[i];
                        ChangeStrategy();

                        result = algorithm.Start(current);
                        var processedResult = ProcessResults(current, result);
                        forecastResult.Results.AddRange(processedResult);
                        current -= GetTotalFromResults(result.Genes);
                    }

                    // Update and Command

                    SendCommand(forecastResult);
                    Update(forecastResult, weather);

                    //Thread.Sleep(10800000); // 3hrs
                    // Test

                    Thread.Sleep(10800000);
                }

            }
        }

        private void SendCommand(CeForecast forecastResult)
        {
            ScadaExportProxy proxy = new ScadaExportProxy();
            var points = proxy.GetData();

            float counter = 0;
            foreach (var item in forecastResult.Results.Take(12))
            {
                for (int i = 0; i < item.Pumps.Count(); i++)
                {
                    var onOff = item.Pumps[i];
                    var time = item.Times[i];
                    var flow = item.Flows[i];

                    if (points.ContainsKey($"Breaker_2{i + 1}Status"))
                    {
                        var breaker2 = points[$"Breaker_2{i + 1}Status"];

                        var command1 = new ScadaCommandingEvent()
                        {
                            Index = (uint)breaker2.Index,
                            RegisterType = breaker2.RegisterType,
                            Milliseconds = (uint)((counter) * 60 * 1000),
                            Value = (uint)onOff
                        };

                        endpoint.Publish(command1).ConfigureAwait(false);

                        if (onOff == 1)
                        {
                            var command3 = new ScadaCommandingEvent()
                            {
                                Index = (uint)breaker2.Index,
                                RegisterType = breaker2.RegisterType,
                                Value = 0,
                                Milliseconds = (uint)((counter + time) * 60 * 1000)
                            };

                            endpoint.Publish(command3).ConfigureAwait(false);
                        }
                    }

                    if (points.ContainsKey($"Discrete_Tap{i + 1}"))
                    {
                        var tap = points[$"Discrete_Tap{i + 1}"];



                        var command2 = new ScadaCommandingEvent()
                        {
                            Index = (uint)tap.Index,
                            RegisterType = tap.RegisterType,
                            Milliseconds = (uint)((counter) * 60 * 1000),
                            Value = (uint)(flow / 100)
                        };

                        endpoint.Publish(command2).ConfigureAwait(false);
                    }
                }
                counter += 15.0f;
            }
        }
        private void SendCommandHelpMethod(CeForecast forecastResult)
        {
            ScadaExportProxy proxy = new ScadaExportProxy();
            var points = proxy.GetData();

            int counter = 0;
            foreach (var item in forecastResult.Results.Take(12))
            {
                for (int i = 0; i < item.Pumps.Count(); i++)
                {
                    var onOff = item.Pumps[i];
                    var time = item.Times[i];
                    var flow = item.Flows[i];

                    var disc01 = points[$"Discrete_Disc01"];
                    var disc02 = points[$"Discrete_Disc02"];
                    var dis1_ = points[$"Discrete_Disc1{i + 1}"];
                    var dis2_ = points[$"Discrete_Disc2{i + 1}"];

                    var breaker01 = points[$"Breaker_01Status"];
                    var breaker1_ = points[$"Breaker_1{i + 1}Status"];


                    var breaker2 = points[$"Breaker_2{i + 1}Status"];
                    var tap = points[$"Discrete_Tap{i + 1}"];

                    try
                    {
                        var command1 = new ScadaCommandingEvent()
                        {
                            Index = (uint)breaker2.Index,
                            RegisterType = breaker2.RegisterType,
                            Milliseconds = 0,
                            Value = (uint)onOff
                        };

                        var command2 = new ScadaCommandingEvent()
                        {
                            Index = (uint)tap.Index,
                            RegisterType = tap.RegisterType,
                            Milliseconds = 0,
                            Value = (uint)(flow / 100)
                        };

                        endpoint.Publish(command1).ConfigureAwait(false);
                        endpoint.Publish(command2).ConfigureAwait(false);

                        if (onOff == 1)
                        {
                            var command3 = new ScadaCommandingEvent()
                            {
                                Index = (uint)breaker2.Index,
                                RegisterType = breaker2.RegisterType,
                                Value = 0,
                                Milliseconds = (uint)((counter + time) * 60 * 1000)
                            };

                            var command4 = new ScadaCommandingEvent()
                            {
                                Index = (uint)disc01.Index,
                                RegisterType = disc01.RegisterType,
                                Milliseconds = 0,
                                Value = (uint)onOff
                            };

                            var command5 = new ScadaCommandingEvent()
                            {
                                Index = (uint)disc02.Index,
                                RegisterType = disc02.RegisterType,
                                Milliseconds = 0,
                                Value = (uint)onOff
                            };

                            var command6 = new ScadaCommandingEvent()
                            {
                                Index = (uint)dis1_.Index,
                                RegisterType = dis1_.RegisterType,
                                Milliseconds = 0,
                                Value = (uint)onOff
                            };

                            var command7 = new ScadaCommandingEvent()
                            {
                                Index = (uint)dis2_.Index,
                                RegisterType = dis2_.RegisterType,
                                Milliseconds = 0,
                                Value = (uint)onOff
                            };

                            var command8 = new ScadaCommandingEvent()
                            {
                                Index = (uint)breaker01.Index,
                                RegisterType = breaker01.RegisterType,
                                Milliseconds = 0,
                                Value = (uint)onOff
                            };

                            var command9 = new ScadaCommandingEvent()
                            {
                                Index = (uint)breaker1_.Index,
                                RegisterType = breaker1_.RegisterType,
                                Milliseconds = 0,
                                Value = (uint)onOff
                            };

                            endpoint.Publish(command3).ConfigureAwait(false);
                            endpoint.Publish(command4).ConfigureAwait(false);
                            endpoint.Publish(command5).ConfigureAwait(false);
                            endpoint.Publish(command6).ConfigureAwait(false);
                            endpoint.Publish(command7).ConfigureAwait(false);
                            endpoint.Publish(command8).ConfigureAwait(false);
                            endpoint.Publish(command9).ConfigureAwait(false);

                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                counter += 15;
            }
        }
        private void Update(CeForecast forecastResult, List<double> weather)
        {
            CeUpdateEvent update = new CeUpdateEvent();
            update.Income = weather;
            update.Times = GetTimes();
            update.FluidLevel = new List<float>();
            foreach (var item in forecastResult.Results)
            {
                update.FluidLevel.Add(item.EndFluidLevel);
            }
            update.Hours = new List<PumpsHours>();
            update.Flows = new List<PumpsFlows>();


            for (int i = 0; i < points; i++)
            {
                var hours = new PumpsHours();
                var flows = new PumpsFlows();
                foreach (var item in forecastResult.Results)
                {
                    if (item.Pumps[i] == 1)
                    {
                        hours.Hours.Add(item.Times[i]);
                        flows.Flows.Add(item.Flows[i]);
                    }
                    else
                    {
                        hours.Hours.Add(0);
                        flows.Flows.Add(0);
                    }
                }
                update.Hours.Add(hours);
                update.Flows.Add(flows);
            }
            endpoint.Publish(update).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        private List<string> GetTimes()
        {
            List<string> list = new List<string>();
            DateTime nextQ = DateTime.Now;
            for (int i = 0; i < 24; i++)
            {
                var str = String.Format("{0}:{1}", nextQ.Hour, nextQ.Minute);
                list.Add(str);
                nextQ = nextQ.AddMinutes(15);
            }

            return list;
        }

        private List<CeForecastResult> ProcessResults(float current, DNA<float> result)
        {
            var results = new List<CeForecastResult>();
            float totalPerIteration = (GetTotalFromResults(result.Genes) / 4);
            for (int i = 0; i < 4; i++)
            {
                var item = new CeForecastResult();
                item.Result = result;
                item.StartFluidLevel = current;
                item.EndFluidLevel = current - totalPerIteration;
                current -= totalPerIteration;

                for (int j = 0; j < result.Genes.Count(); j += 3)
                {
                    item.Pumps.Add(result.Genes[j]);
                    item.Times.Add(result.Genes[j + 2]);
                    item.Flows.Add(result.Genes[j + 1]);
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
            for (int i = 0; i < results.Count(); i += 3)
            {
                total += (results[i] * results[i + 1] * results[i + 2]);
            }

            return total;
        }

        private void ChangeStrategy()
        {
            var results = ReadConfiguration();
            switch (points)
            {
                case 1: algorithm = new FluidLevelOptimization1(results.OptimalFluidLevel, results.Percetage, results.TimeFactor, results.Iterations); break;
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
