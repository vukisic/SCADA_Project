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
            //NetworkModelServiceProxy proxy = new NetworkModelServiceProxy(new System.ServiceModel.EndpointAddress("net.tcp://localhost:22330/NetworkModelServiceSF"));
            //proxy.ApplyDelta(null);
            /* --- DomService --- */
            //DomServiceProxy proxy = new DomServiceProxy();
            //proxy.AddOrUpdate(new SCADA.Common.Models.DomDbModel() { ManipulationConut = 0, Mrid = "Test", TimeStamp = DateTime.Now.ToString() }).GetAwaiter().GetResult();
            //proxy.GetAll().GetAwaiter().GetResult().ForEach(x => Console.WriteLine(x.Mrid));
            /* --- HistoryService --- */
            //HistoryServiceProxy proxy = new HistoryServiceProxy();
            //proxy.Add(new SCADA.Common.Models.HistoryDbModel() 
            //{ 
            //    ClassType = SCADA.Common.DataModel.ClassType.CLASS_0, 
            //    Index = 0, 
            //    MeasurementType = "Power", 
            //    Mrid = "Test", 
            //    RegisterType = SCADA.Common.DataModel.RegisterType.BINARY_OUTPUT, 
            //    TimeStamp = DateTime.Now.ToString(), Value = 1 
            //}).GetAwaiter().GetResult();
            //proxy.GetAll().GetAwaiter().GetResult().ForEach(x => Console.WriteLine(x.Mrid));
            /* --- ScadaStorageService --- */
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

            NetworkModelServiceTransactionProxy nmsProxy = new NetworkModelServiceTransactionProxy();
            nmsProxy.Rollback().GetAwaiter().GetResult();

            Console.WriteLine("\n\nALL DONE\n\n");


            Console.ReadLine();
        }

        private static Dictionary<Tuple<RegisterType,int>, BasePoint> GetScadaModel(bool test = false)
        {
            var dict = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            if (!test)
            {
                var analog = new AnalogPoint() { Alarm = AlarmType.NO_ALARM, ClassType = ClassType.CLASS_0, Direction = FTN.Common.SignalDirection.ReadWrite, Index = 1, MeasurementType = FTN.Common.MeasurementType.ActiveEnergy, Mrid = "TEST_ANALOG", ObjectMrid = null, RegisterType = RegisterType.ANALOG_OUTPUT, TimeStamp = DateTime.Now.ToString(),MaxValue = 5,MinValue =0, NormalValue= 0, Value=1 };
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
                    Console.WriteLine($"Analog| {point.Mrid} - {point.Value}");
                }
                else
                {
                    var point = item.Value as DiscretePoint;
                    Console.WriteLine($"Binary| {point.Mrid} - {point.Value}");
                }
            }
            Console.WriteLine();
        }
    }
}
