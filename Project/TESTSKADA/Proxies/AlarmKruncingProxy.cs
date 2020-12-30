using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common;
using SCADA.Common.DataModel;
using SCADA.Services.Common;

namespace NDS.Proxies
{
    public class AlarmKruncingProxy
    {
        private IAlarmKruncing proxy;

        public AlarmKruncingProxy() {

            ChannelFactory<IAlarmKruncing> channelFactory = new ChannelFactory<IAlarmKruncing>
               (new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:7001/IAlarmKruncing"));
            proxy = channelFactory.CreateChannel();

        }

        public void Check()
        {
                List<BasePoint> par = new List<BasePoint>();
             /*  AnalogPoint analogPoint = new AnalogPoint()
                {
                    ClassType = ClassType.CLASS_1,
                    Direction = FTN.Common.SignalDirection.ReadWrite,
                    RegisterType = RegisterType.ANALOG_INPUT,
                    Index = 0,
                    MaxValue = 200,
                    MinValue = 100,
                    MeasurementType = FTN.Common.MeasurementType.ActiveEnergy,
                    Mrid = "asdfg",
                    NormalValue = 120,
                    ObjectMrid = null,
                    TimeStamp = String.Empty,
                    Value = 210,
                    Alarm = AlarmType.NO_ALARM
                };
                par.Add(analogPoint);
            */
             
            //proxy.Check(DataBase.Model.Values.ToList());
            proxy.Check(par);
        }
    }
}
