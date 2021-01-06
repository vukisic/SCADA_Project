using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FTN.Common;
using FTN.Services.NetworkModelService;
using FTN.Services.NetworkModelService.DataModel.Meas;
using FTN.Services.NetworkModelService.DataModel.Wires;
using SCADA.Common.DataModel;

namespace SCADA.Common
{
    public class ScadaModelConverter
    {
        private int analogInputsCount;
        private int analogOutputsCount;
        private int binaryInputsCount;
        private int binaryOutputsCount;

        public ScadaModelConverter()
        {

        }

        public Dictionary<string,ushort> GetMrdidIndexPairs(List<BasePoint> list)
        {
            var result = new Dictionary<string, ushort>();
            foreach (var item in list)
            {
                result.Add(item.Mrid, (ushort)(item.Index));
            }
            return result;
        }

        public ConversionResult Convert(Dictionary<DMSType,Container> model)
        {
            ConversionResult result = new ConversionResult();
            try
            {   
                Container discreteContainer = model[DMSType.DISCRETE];
                Container analogContainer = model[DMSType.ANALOG];
                Container breakerContainer = model[DMSType.BREAKER];
                Container disconnectorContainer = model[DMSType.DISCONNECTOR];
                result.Points = ConvertPoints(analogContainer, discreteContainer);
                result.Equipment = ConvertSwitchingEquipment(breakerContainer, disconnectorContainer);
                result.MridIndexPairs = GetMrdidIndexPairs(result.Points.Values.ToList());
                result.Success = true;
                
            }
            catch (Exception)
            {
                result.Success = false;
            }
            return result;

        }

        private Dictionary<Tuple<RegisterType, int>, BasePoint> ConvertPoints(Container analogContainer, Container discreteContainer)
        {
            Dictionary<Tuple<RegisterType, int>, BasePoint> points = new Dictionary<Tuple<RegisterType, int>, BasePoint>();

            foreach (var item in analogContainer.Entities.Values)
            {
                if (item is Analog)
                {
                    Analog analog = item as Analog;

                    AnalogPoint analogPoint = new AnalogPoint()
                    {
                        ClassType = ClassType.CLASS_1,
                        Direction = analog.Direction,
                        RegisterType = GetRegistryType(analog.Direction),
                        Index = GetIndex(GetRegistryType(analog.Direction)),
                        MaxValue = analog.MaxValue,
                        MinValue = analog.MinValue,
                        MeasurementType = analog.MeasurementType,
                        Mrid = analog.MRID,
                        NormalValue = analog.NormalValue,
                        ObjectMrid = analog.ObjectMRID ?? null,
                        TimeStamp = String.Empty,
                        Value = analog.NormalValue,
                        Alarm = AlarmType.NO_ALARM
                    };
                    points.Add(new Tuple<RegisterType, int>(analogPoint.RegisterType, analogPoint.Index), analogPoint);
                }
            }

            foreach (var item in discreteContainer.Entities.Values)
            {
                if (item is Discrete)
                {
                    Discrete discrete = item as Discrete;
                    if (discrete.MRID.Contains("Discrete_Tap"))
                    {
                        AnalogPoint discretePoint = new AnalogPoint()
                        {
                            ClassType = ClassType.CLASS_2,
                            Direction = discrete.Direction,
                            RegisterType = RegisterType.ANALOG_OUTPUT,
                            Index = GetIndex(RegisterType.ANALOG_OUTPUT),
                            MaxValue = discrete.MaxValue,
                            MinValue = discrete.MinValue,
                            MeasurementType = discrete.MeasurementType,
                            Mrid = discrete.MRID,
                            NormalValue = discrete.NormalValue,
                            ObjectMrid = discrete.ObjectMRID ?? null,
                            TimeStamp = String.Empty,
                            Value = discrete.NormalValue,
                            Alarm = AlarmType.NO_ALARM
                        };
                        points.Add(new Tuple<RegisterType, int>(discretePoint.RegisterType, discretePoint.Index), discretePoint);
                    }
                    else
                    {
                        DiscretePoint discretePoint = new DiscretePoint()
                        {
                            ClassType = ClassType.CLASS_2,
                            Direction = discrete.Direction,
                            RegisterType = GetRegistryType(discrete.Direction, true),
                            Index = GetIndex(GetRegistryType(discrete.Direction, true)),
                            MaxValue = discrete.MaxValue,
                            MinValue = discrete.MinValue,
                            MeasurementType = discrete.MeasurementType,
                            Mrid = discrete.MRID,
                            NormalValue = discrete.NormalValue,
                            ObjectMrid = discrete.ObjectMRID ?? null,
                            TimeStamp = String.Empty,
                            Value = discrete.NormalValue,
                            Alarm = AlarmType.NO_ALARM
                        };
                        points.Add(new Tuple<RegisterType, int>(discretePoint.RegisterType, discretePoint.Index), discretePoint);
                    }
                }
            }

            return points;
        }

        private int GetIndex(RegisterType registerType)
        {
            switch (registerType)
            {
                case RegisterType.ANALOG_INPUT : return analogInputsCount++;
                case RegisterType.ANALOG_OUTPUT: return analogOutputsCount++;
                case RegisterType.BINARY_INPUT : return binaryInputsCount++;
                case RegisterType.BINARY_OUTPUT: return binaryOutputsCount++;
                default: return 0;
            }
        }

        private RegisterType GetRegistryType(SignalDirection direction, bool discrete = false)
        {
            switch (direction)
            {
                case SignalDirection.Read: return discrete ? RegisterType.BINARY_INPUT : RegisterType.ANALOG_INPUT;
                case SignalDirection.ReadWrite: return discrete ? RegisterType.BINARY_OUTPUT : RegisterType.ANALOG_OUTPUT;
                case SignalDirection.Write: return discrete ? RegisterType.BINARY_OUTPUT : RegisterType.ANALOG_OUTPUT;
                default: return RegisterType.ANALOG_INPUT;
            }
        }

        private Dictionary<string, SwitchingEquipment> ConvertSwitchingEquipment(Container breakerContainer, Container disconnectorContainer)
        {
            Dictionary<string, SwitchingEquipment>  equipment = new Dictionary<string, SwitchingEquipment>();
            foreach (var item in breakerContainer.Entities.Values)
            {
                if(item is Breaker)
                {
                    Breaker breaker = item as Breaker;
                    if (!equipment.ContainsKey(breaker.MRID))
                        equipment.Add(breaker.MRID, new SwitchingEquipment() { Mrid = breaker.MRID, ManipulationConut = breaker.ManipulationCount });
                }
            }

            foreach (var item in disconnectorContainer.Entities.Values)
            {
                if (item is Disconnector)
                {
                    Disconnector disconnector = item as Disconnector;
                    if (!equipment.ContainsKey(disconnector.MRID))
                        equipment.Add(disconnector.MRID, new SwitchingEquipment() { Mrid = disconnector.MRID, ManipulationConut = disconnector.ManipulationCount });
                }
            }
            return equipment;
        }
    }
}
