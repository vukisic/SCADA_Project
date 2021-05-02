using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using Calculations;
using CE.Common.Proxies;
using CE.Data;
using Core.Common.Json;
using Core.Common.PubSub;
using Core.Common.ServiceBus.Events;
using Core.Common.WeatherApi;
using NServiceBus;
using SCADA.Common.DataModel;
using SF.Common;
using SF.Common.Proxies;

namespace CE
{
    public class CEWorker : IDisposable
    {
        private long seconds = 0;
        public int TPoints = 0;
        private IFitnessFunction algorithm;
        private Thread _worker;
        public EventHandler<int> _updateEvent = delegate { };
        private bool pointUpdateOccures;
        private bool endFlag;
        private int points = 0;
        private SF.Common.Proxies.WeatherServiceProxy weatherAPI;
        private static bool skip = false;
        private int secundsForWeather = 60;
        private int hourIndex = 0;
        private int hourIndexChanged = 0;
        private CeGraphicalEvent graph;
        private Publisher pubsub;
        private DNA<float> result;
        private CeForecast forecastResult;
        private int pumpConstant = 1;
        private int indexGraph = 0;
        private int indexUpdate = 0;
        private List<List<ScadaCommandingEvent>> commands = new List<List<ScadaCommandingEvent>>();
        private bool simulation = false;

        public CEWorker()
        {
        }

        public void Start()
        {
            _worker = new Thread(DoWorkNew);
            endFlag = true;
            _worker.Name = "CE Worker";
            var api = ConfigurationManager.AppSettings["WeatherApi"];
            weatherAPI = new SF.Common.Proxies.WeatherServiceProxy(api);
            var subscription = new Subscription();
            pubsub = new Publisher(subscription.Topic, subscription.ConnectionString);
            _worker.Start();
        }

        public void Stop()
        {
            endFlag = false;
            _worker.Abort();
            _worker = null;
        }
        private void DoWorkNew()
        {
            Thread.Sleep(10000);
            graph = new CeGraphicalEvent();
            graph.PumpsValues = new Core.Common.ServiceBus.Events.CeGraph();
            while (endFlag)
            {
                try
                {
                    if (points <= 0 || points > 3)
                        continue;
                    CheckState();
                    if (hourIndexChanged == 3600)
                    {
                        hourIndex++;
                    }
                    if (seconds == 10800 || seconds == 0 || pointUpdateOccures)
                    {
                        SimulatorProxy invoker = new SimulatorProxy();
                        invoker.SimulatorSettings(false);
                        indexGraph = indexUpdate = 0;
                        Calculations();
                        if(seconds == 10800)
                            seconds = 0;
                    }
                    if (points > 0 && commands != null && graph != null)
                    {
                        if (!simulation)
                        {
                            SimulatorProxy invoker = new SimulatorProxy();
                            invoker.SimulatorSettings(true);
                            simulation = true;
                        }
                        if (seconds % 45 == 0)
                        {
                            if (indexUpdate < 24)
                                Command();
                            indexUpdate++;
                            if (indexGraph < 24)
                                UpdateGraph();
                            indexGraph++;
                            if (indexGraph == 23)
                            {
                                SimulatorProxy invoker = new SimulatorProxy();
                                invoker.SimulatorSettings(false);
                            }


                        }
                    }

                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    seconds += 1;
                    hourIndexChanged += 1;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }

        private void Command()
        {
            if (commands.Count == 0)
                return;
            CommandingProxy proxy = new CommandingProxy(ConfigurationManager.AppSettings["Command"]);
            foreach (var item in commands[indexUpdate])
            {
                proxy.Commmand(new SCADA.Common.ScadaCommand(item.RegisterType, item.Index, item.Value, 0));
            }
        }

        private void UpdateGraph()
        {
            if (points > 0)
            {
                CeGraphicalEvent newGraph = new CeGraphicalEvent();
                newGraph.PumpsValues.Pump1.XAxes = graph.PumpsValues.Pump1.XAxes.Take(indexGraph).ToList();
                newGraph.PumpsValues.Pump1.YAxes = graph.PumpsValues.Pump1.YAxes.Take(indexGraph).ToList();

                newGraph.PumpsValues.Pump2.XAxes = graph.PumpsValues.Pump2.XAxes.Take(indexGraph).ToList();
                newGraph.PumpsValues.Pump2.YAxes = graph.PumpsValues.Pump2.YAxes.Take(indexGraph).ToList();

                newGraph.PumpsValues.Pump3.XAxes = graph.PumpsValues.Pump3.XAxes.Take(indexGraph).ToList();
                newGraph.PumpsValues.Pump3.YAxes = graph.PumpsValues.Pump3.YAxes.Take(indexGraph).ToList();

                var json = JsonTool.Serialize<CeGraphicalEvent>(newGraph);
                PubSubMessage ev = new PubSubMessage()
                {
                    ContentType = ContentType.CE_HISTORY_GRAPH,
                    Content = json,
                    Sender = Sender.CE
                };
                pubsub.SendMessage(ev).ConfigureAwait(false);
            }
        }

        private void CheckState()
        {
            var proxy = new SF.Common.Proxies.ScadaExportProxy(ConfigurationManager.AppSettings["Scada"]);
            var measurements = proxy.GetData().GetAwaiter().GetResult();
            if (measurements == null || !measurements.ContainsKey("FluidLevel_Tank"))
                return;
            var fluidLevel = measurements["FluidLevel_Tank"] as AnalogPoint;
            if(points == 1)
            {
                var flows = 2 * (measurements["Flow_AM2"] != null ? ((AnalogPoint)(measurements["Flow_AM2"])).Value / 4 : 0);
                if (LevelIsOptimal(fluidLevel.Value - flows))
                    TurnOffPumps();
            }
            else if(points == 2)
            {
                var flows = 2 * ((measurements["Flow_AM1"] != null ? ((AnalogPoint)(measurements["Flow_AM1"])).Value / 4 : 0) +
                        (measurements["Flow_AM2"] != null ? ((AnalogPoint)(measurements["Flow_AM2"])).Value / 4 : 0));
                if (LevelIsOptimal(fluidLevel.Value - flows))
                    TurnOffPumps();
            }
            else if(points == 3)
            {
                var flows = 2 * ((measurements["Flow_AM1"] != null ? ((AnalogPoint)(measurements["Flow_AM1"])).Value / 4 : 0) +
                        (measurements["Flow_AM2"] != null ? ((AnalogPoint)(measurements["Flow_AM2"])).Value / 4 : 0) +
                        (measurements["Flow_AM3"] != null ? ((AnalogPoint)(measurements["Flow_AM3"])).Value / 4 : 0));
                if (LevelIsOptimal(fluidLevel.Value - flows))
                    TurnOffPumps();
            }
            
        }

        private void TurnOffPumps()
        {
            var commanding = new CommandingProxy(ConfigurationManager.AppSettings["Command"]);
            var proxy = new SF.Common.Proxies.ScadaExportProxy(ConfigurationManager.AppSettings["Scada"]);
            var points = proxy.GetData().GetAwaiter().GetResult();
            if (points == null)
                return;
            if (points.ContainsKey("Breaker_21Status"))
            {
                var breaker1 = points["Breaker_21Status"] as DiscretePoint;
                var command1 = new ScadaCommandingEvent()
                {
                    Index = (uint)breaker1.Index,
                    RegisterType = breaker1.RegisterType,
                    Value = 0,
                    Milliseconds = 0
                };
                commanding.Commmand(new SCADA.Common.ScadaCommand(command1.RegisterType, command1.Index, command1.Value, command1.Milliseconds)).GetAwaiter().GetResult();
            }

            if (points.ContainsKey("Breaker_22Status"))
            {
                var breaker2 = points["Breaker_22Status"] as DiscretePoint;
                var command2 = new ScadaCommandingEvent()
                {
                    Index = (uint)breaker2.Index,
                    RegisterType = breaker2.RegisterType,
                    Value = 0,
                    Milliseconds = 0
                };
                commanding.Commmand(new SCADA.Common.ScadaCommand(command2.RegisterType, command2.Index, command2.Value, command2.Milliseconds)).GetAwaiter().GetResult();
            }

            if (points.ContainsKey("Breaker_23Status"))
            {
                var breaker3 = points["Breaker_23Status"] as DiscretePoint;
                var command3 = new ScadaCommandingEvent()
                {
                    Index = (uint)breaker3.Index,
                    RegisterType = breaker3.RegisterType,
                    Value = 0,
                    Milliseconds = 0
                };
                commanding.Commmand(new SCADA.Common.ScadaCommand(command3.RegisterType, command3.Index, command3.Value, command3.Milliseconds)).GetAwaiter().GetResult();
            }
        }
        private void Calculations()
        {

            if (points > 0 && points < 4)
            {
                if (pointUpdateOccures)
                {
                    ChangeStrategy();
                }

                forecastResult = new CeForecast();
                var area = GetSurfaceArea();
                var weatherForecast = weatherAPI.GetForecast();
                var weather = new List<double>();
                weatherForecast.ForEach(x => weather.Add(x * area));
                float current = GetCurrentFluidLevel();

                for (int i = 0; i < weatherForecast.Count; i++)
                {
                    ChangeStrategy();
                    for (int j = 0; j < 4; j++)
                    {
                        var income = ((float)weather[i]) / 4;
                        current += income;

                        if (skip && i == 0)
                        {
                            current -= income;
                            skip = false;
                            break;
                        }

                        result = algorithm.Start(current);
                        var processedResult = ProcessResult(current, result);
                        forecastResult.Results.AddRange(processedResult);
                        current -= GetTotalFromResults(result.Genes);
                    }
                }

                //SendCommand(forecastResult);
                Update(forecastResult, weather);

            }
        }
        private bool LevelIsOptimal(float fluidLevel)
        {
            var results = ReadConfiguration();

            float lowerBound = results.OptimalFluidLevel * (1.0f - (results.Percetage / 100));
            float upperBound = results.OptimalFluidLevel * (1.0f + (results.Percetage / 100));
            bool ret = (fluidLevel <= upperBound && fluidLevel >= lowerBound) || fluidLevel <= upperBound; ;

            return ret;
        }

        public void OnPointUpdate(int tPoints)
        {
            pointUpdateOccures = true;
            if (points > 0)
                skip = true;
            points = tPoints;
            Stop();
            OffSequence();
            Start();
        }

        private void SendCommand(CeForecast forecastResult)
        {
            commands = new List<List<ScadaCommandingEvent>>();
            var proxy = new SF.Common.Proxies.ScadaExportProxy(ConfigurationManager.AppSettings["Scada"]);
            var commanding = new CommandingProxy(ConfigurationManager.AppSettings["Command"]);
            var points = proxy.GetData().GetAwaiter().GetResult();
            if (points == null)
                return;
            foreach (var item in forecastResult.Results.Take(24))
            {
                var cmds = new List<ScadaCommandingEvent>();
                for (int i = 0; i < item.Pumps.Count(); i++)
                {
                    var onOff = item.Pumps[i];
                    var time = item.Times[i];
                    var flow = item.Flows[i];
                    if (points.ContainsKey($"Breaker_2{i + 1}Status"))
                    {
                        var breaker2 = points[$"Breaker_2{i + 1}Status"];
                        if (onOff != 0)
                        {
                            var command1 = new ScadaCommandingEvent()
                            {
                                Index = (uint)breaker2.Index,
                                RegisterType = breaker2.RegisterType,
                                Milliseconds = 0,
                                Value = (uint)onOff
                            };
                            cmds.Add(command1);
                        }

                    }

                    if (points.ContainsKey($"Discrete_Tap{i + 1}") && onOff == 1)
                    {
                        var tap = points[$"Discrete_Tap{i + 1}"];

                        var command2 = new ScadaCommandingEvent()
                        {
                            Index = (uint)tap.Index,
                            RegisterType = tap.RegisterType,
                            Milliseconds = 0,
                            Value = (uint)(flow / 100)
                        };
                        cmds.Add(command2);
                    }
                }
                commands.Add(cmds);
            }
        }

        private void Update(CeForecast forecastResult, List<double> weather)
        {
            CeUpdateEvent update = new CeUpdateEvent();
            update.Income = new List<double>();
            for (int i = 0; i < weather.Count; i++)
            {
                double integralIncome = 0;
                for (int j = 0; j < 4; j++)
                {
                    integralIncome += weather[i] / 4;
                    update.Income.Add(integralIncome);
                }
            }
            update.Times = GetTimes();
            update.FluidLevel = new List<float>();
            foreach (var item in forecastResult.Results)
            {
                update.FluidLevel.Add(item.StartFluidLevel);
                update.FluidLevel.Add(item.EndFluidLevel);
            }
            update.Hours = new List<PumpsHours>();
            update.Flows = new List<PumpsFlows>();

            graph = new CeGraphicalEvent();
            graph.PumpsValues = new Core.Common.ServiceBus.Events.CeGraph();
            List<List<long>> list = new List<List<long>>();
            for (int i = 0; i < points; i++)
            {
                var macList = new List<long>();
                long cumm = 0;
                var hours = new PumpsHours();
                var flows = new PumpsFlows();
                foreach (var item in forecastResult.Results)
                {

                    if (item.Pumps[i] == 1)
                    {
                        hours.Hours.Add(item.Times[i]);
                        flows.Flows.Add(item.Flows[i]);
                        cumm += (long)(item.Times[i]);
                    }
                    else
                    {
                        hours.Hours.Add(0);
                        flows.Flows.Add(0);
                    }
                    macList.Add(cumm);
                }
                update.Hours.Add(hours);
                update.Flows.Add(flows);
                list.Add(macList);
            }
            pubsub.SendMessage(new Core.Common.PubSub.PubSubMessage()
            {
                ContentType = Core.Common.PubSub.ContentType.CE_UPDATE,
                Content = JsonTool.Serialize<CeUpdateEvent>(update),
                Sender = Core.Common.PubSub.Sender.CE

            }).GetAwaiter().GetResult();

            if (points == 1)
            {
                graph.PumpsValues.Pump1.XAxes = update.Times.ConvertAll(x => DateTime.Parse(x));
                graph.PumpsValues.Pump1.YAxes = list[0];
            }
            if (points == 2)
            {
                graph.PumpsValues.Pump1.XAxes = update.Times.ConvertAll(x => DateTime.Parse(x));
                graph.PumpsValues.Pump1.YAxes = list[0];
                graph.PumpsValues.Pump2.XAxes = update.Times.ConvertAll(x => DateTime.Parse(x));
                graph.PumpsValues.Pump2.YAxes = list[1];
            }
            if (points == 3)
            {
                graph.PumpsValues.Pump1.XAxes = update.Times.ConvertAll(x => DateTime.Parse(x));
                graph.PumpsValues.Pump1.YAxes = list[0];
                graph.PumpsValues.Pump2.XAxes = update.Times.ConvertAll(x => DateTime.Parse(x));
                graph.PumpsValues.Pump2.YAxes = list[1];
                graph.PumpsValues.Pump3.XAxes = update.Times.ConvertAll(x => DateTime.Parse(x));
                graph.PumpsValues.Pump3.YAxes = list[2];
            }
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

        private List<CeForecastResult> ProcessResult(float current, DNA<float> result)
        {
            var results = new List<CeForecastResult>();
            float totalPerIteration = GetTotalFromResults(result.Genes);
            var item = new CeForecastResult();
            item.Result = result;
            item.StartFluidLevel = current;
            item.EndFluidLevel = current - totalPerIteration;

            for (int j = 0; j < result.Genes.Count(); j += 3)
            {
                item.Pumps.Add(result.Genes[j]);
                item.Times.Add(result.Genes[j + 2]);
                item.Flows.Add(result.Genes[j + 1]);
            }
            results.Add(item);
            return results;
        }

        private float GetCurrentFluidLevel()
        {
            SF.Common.Proxies.ScadaExportProxy proxy = new SF.Common.Proxies.ScadaExportProxy(ConfigurationManager.AppSettings["Scada"]);
            var data = proxy.GetData().GetAwaiter().GetResult();
            if (data != null)
            {
                var point = (data["FluidLevel_Tank"] as AnalogPoint);
                return point.Value;
            }
            return 0;
           
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

           /* result - najbolje rjesenje (jedinka)
                result.Genes[0] - prvi gen (da li pumpa radi ili ne radi)
                ako je case 1 - bice 3 gena - jedna pumpa
                ako je case 2 - bice 6 gena - dvije pumpe
                ako je case 3 - bice 9 gena - tri pumpe */

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



        private void OffSequence()
        {
            var commanding = new CommandingProxy(ConfigurationManager.AppSettings["Command"]);
            var clearCommand = new ScadaCommandingEvent()
            {
                Index = 1,
                Milliseconds = 1,
                RegisterType = RegisterType.BINARY_INPUT,
                Value = 0
            };
            commanding.Commmand(new SCADA.Common.ScadaCommand(clearCommand.RegisterType, clearCommand.Index, clearCommand.Value, clearCommand.Milliseconds)).GetAwaiter().GetResult();

            var proxy = new SF.Common.Proxies.ScadaExportProxy(ConfigurationManager.AppSettings["Scada"]);
            var points = proxy.GetData().GetAwaiter().GetResult();
            if (points == null)
                return;
            var commands = new List<ScadaCommandingEvent>();
            foreach (var item in points)
            {
                if (item.Key.Contains("Breaker_21Status") || item.Key.Contains("Breaker_22Status") || item.Key.Contains("Breaker_23Status") || item.Key.Contains("Discrete_Tap"))
                {
                    var command = new ScadaCommandingEvent()
                    {
                        Index = (uint)item.Value.Index,
                        RegisterType = item.Value.RegisterType,
                        Milliseconds = 0,
                        Value = 0
                    };
                    commands.Add(command);
                }
            }

            foreach (var item in commands)
            {
                commanding.Commmand(new SCADA.Common.ScadaCommand(item.RegisterType, item.Index, item.Value, item.Milliseconds)).GetAwaiter().GetResult();
            }
        }
    }
}
