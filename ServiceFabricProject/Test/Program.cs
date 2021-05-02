using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using SCADA.Common.DataModel;
using SF.Common.Proxies;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            /* --- WeatherService --- */
            //WeatherServiceProxy proxy = new WeatherServiceProxy();
            //proxy.GetForecast();
            /* --- NetworkModelService --- */
            //NetworkModelServiceProxy proxy = new NetworkModelServiceProxy();
            //proxy.ApplyDelta(null);
            /* --- DomService --- */
            //DomServiceProxy Dproxy = new DomServiceProxy();
            //Dproxy.AddOrUpdate(new SCADA.Common.Models.DomDbModel() { ManipulationConut = 0, Mrid = "Test", TimeStamp = DateTime.Now.ToString() }).GetAwaiter().GetResult();
            //Dproxy.GetAll().GetAwaiter().GetResult().ForEach(x => Console.WriteLine(x.Mrid));
            ///* --- HistoryService --- */
            //HistoryServiceProxy Hproxy = new HistoryServiceProxy();
            //Hproxy.Add(new SCADA.Common.Models.HistoryDbModel()
            //{
            //    ClassType = SCADA.Common.DataModel.ClassType.CLASS_0,
            //    Index = 0,
            //    MeasurementType = "Power",
            //    Mrid = "Test",
            //    RegisterType = SCADA.Common.DataModel.RegisterType.BINARY_OUTPUT,
            //    TimeStamp = DateTime.Now.ToString(),
            //    Value = 1
            //}).GetAwaiter().GetResult();
            //Hproxy.GetAll().GetAwaiter().GetResult().ForEach(x => Console.WriteLine(x.Mrid));
            ///* --- ScadaStorageService --- */
            //ScadaStorageProxy proxy = new ScadaStorageProxy();
            //var dict = GetScadaModel();
            //proxy.SetModel(dict).GetAwaiter().GetResult();
            //var m1 = proxy.GetModel().GetAwaiter().GetResult();
            //PrintScadaModel(m1);
            //proxy.UpdateModelValue(GetScadaModel(true)).GetAwaiter().GetResult();
            //var m2 = proxy.GetModel().GetAwaiter().GetResult();
            //PrintScadaModel(m2);

            //Console.WriteLine("DOM\n\n");
            //proxy.SetDomModel(new List<SwitchingEquipment> { new SwitchingEquipment() { Mrid = "SW1", ManipulationConut = 5 } }).GetAwaiter().GetResult();
            //proxy.GetDomModel().GetAwaiter().GetResult().ForEach(x => Console.WriteLine($"{x.Mrid} - {x.ManipulationConut}"));

            //Console.WriteLine("CIM\n");
            //Dictionary<DMSType, Container> dr = new Dictionary<DMSType, Container>();
            //dr[DMSType.BREAKER] = new Container();
            //dr[DMSType.DISCRETE] = new Container();

            //proxy.SetCimModel(dr).GetAwaiter().GetResult();
            //var cim = proxy.GetCimModel().GetAwaiter().GetResult();
            //foreach (var item in cim)
            //{
            //    Console.WriteLine($"{item.Key}");
            //}

            //Console.WriteLine();
            //var single = proxy.GetSingle(RegisterType.ANALOG_OUTPUT, 1).GetAwaiter().GetResult();
            //Console.WriteLine($"SINGLE {single.Mrid}");

            /*NetworkModelServiceTransactionProxy nmsProxy = new NetworkModelServiceTransactionProxy();
            nmsProxy.Rollback().GetAwaiter().GetResult();*/
            //Console.WriteLine("CEEE");
            //CEModelProxy nmsProxy = new CEModelProxy();
            //Dictionary<DMSType, Container> pom = new Dictionary<DMSType, Container>();
            //pom[DMSType.ANALOG] = new Container() { Entities = new Dictionary<long, FTN.Services.NetworkModelService.DataModel.Core.IdentifiedObject>() };
            //nmsProxy.ModelUpdate(pom).GetAwaiter().GetResult();
            //Console.WriteLine("\n\nALL DONE\n\n");

            //Console.WriteLine("Scada Model");
            //ScadaStorageProxy proxy = new ScadaStorageProxy();
            //var m1 = proxy.GetModel().GetAwaiter().GetResult();
            //PrintScadaModel(m1);

            //Console.WriteLine("Scada Export Proxy");
            //ScadaExportProxy exportProxy = new ScadaExportProxy();
            //var m2 = exportProxy.GetData().GetAwaiter().GetResult();
            //m2.Values.ToList().ForEach(x => Console.WriteLine($"{x.Mrid}"));

            //Console.WriteLine("Log");
            //LogServiceProxy log = new LogServiceProxy();
            //log.Log(new SCADA.Common.Logging.LogEventModel() { EventType = SCADA.Common.Logging.LogEventType.DEBUG, Message = "A" }).GetAwaiter().GetResult();

            //Console.WriteLine("TM");
            //TransactionManagerServiceProxy tm = new TransactionManagerServiceProxy();
            //tm.Enlist().GetAwaiter().GetResult();

            //Console.WriteLine("Alarm");
            //var alarm = new AlarmingProxy();
            //var result = alarm.Check(GetScadaModel()).GetAwaiter().GetResult();
            //PrintScadaModel(result);

            //var commanding = new CommandingProxy();
            //commanding.Commmand(new SCADA.Common.ScadaCommand(RegisterType.BINARY_OUTPUT, 1, 1, 3000));
            //Console.WriteLine("All Done!");

            //var tProxy = new CEServiceProxy();
            //tProxy.SetPoints(5).GetAwaiter().GetResult();

            try
            {
                //PubSubServiceProxy pubsub = new PubSubServiceProxy();
                //pubsub.SendMessage(null).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {

                throw;
            }
           

            Console.ReadLine();
        }

        private static Dictionary<Tuple<RegisterType,int>, BasePoint> GetScadaModel(bool test = false)
        {
            var dict = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            if (!test)
            {
                var analog = new AnalogPoint() { Alarm = AlarmType.NO_ALARM, ClassType = ClassType.CLASS_0, Direction = FTN.Common.SignalDirection.ReadWrite, Index = 1, MeasurementType = FTN.Common.MeasurementType.ActiveEnergy, Mrid = "TEST_ANALOG", ObjectMrid = null, RegisterType = RegisterType.ANALOG_OUTPUT, TimeStamp = DateTime.Now.ToString(),MaxValue = 5,MinValue =0, NormalValue= 0, Value=10 };
                var binary = new DiscretePoint() { Alarm = AlarmType.NO_ALARM, ClassType = ClassType.CLASS_0, Direction = FTN.Common.SignalDirection.ReadWrite, Index = 3, MeasurementType = FTN.Common.MeasurementType.Status, Mrid = "TEST_BINARY", ObjectMrid = null, RegisterType = RegisterType.BINARY_OUTPUT, TimeStamp = DateTime.Now.ToString(), MaxValue=1,MinValue=0, NormalValue=0, Value=1 };
                dict[Tuple.Create(analog.RegisterType, analog.Index)] = analog;
                dict[Tuple.Create(binary.RegisterType, binary.Index)] = binary;
            }
            else
            {
                var analog = new AnalogPoint() { Alarm = AlarmType.NO_ALARM, ClassType = ClassType.CLASS_0, Direction = FTN.Common.SignalDirection.ReadWrite, Index = 1, MeasurementType = FTN.Common.MeasurementType.ActiveEnergy, Mrid = "TEST_ANALOG", ObjectMrid = null, RegisterType = RegisterType.ANALOG_OUTPUT, TimeStamp = DateTime.Now.ToString(), MaxValue = 7, MinValue = 0, NormalValue = 0, Value = 7 };
                var binary = new DiscretePoint() { Alarm = AlarmType.NO_ALARM, ClassType = ClassType.CLASS_0, Direction = FTN.Common.SignalDirection.ReadWrite, Index = 3, MeasurementType = FTN.Common.MeasurementType.Status, Mrid = "TEST_BINARY", ObjectMrid = null, RegisterType = RegisterType.BINARY_OUTPUT, TimeStamp = DateTime.Now.ToString(), MaxValue = 1, MinValue = 0, NormalValue = 0, Value = 0 };
                dict[Tuple.Create(analog.RegisterType, analog.Index)] = analog;
                dict[Tuple.Create(binary.RegisterType, binary.Index)] = binary;
            }
            return dict; 
        }

        private static void PrintScadaModel(Dictionary<Tuple<RegisterType, int>, BasePoint> valuePairs)
        {
            Console.WriteLine();
            foreach (var item in valuePairs)
            {
                if(item.Key.Item1 == RegisterType.ANALOG_INPUT || item.Key.Item1 == RegisterType.ANALOG_OUTPUT)
                {
                    var point = item.Value as AnalogPoint;
                    Console.WriteLine($"Analog| {point.Mrid} - {point.Value} - {point.Alarm}");
                }
                else
                {
                    var point = item.Value as DiscretePoint;
                    Console.WriteLine($"Binary| {point.Mrid} - {point.Value} - {point.Alarm}");
                }
            }
            Console.WriteLine();
        }
    }
}
