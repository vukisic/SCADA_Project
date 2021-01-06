using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.WeatherApi;
using dnp3_protocol;
using Simulator.Core.Model;

namespace Simulator.Core
{
    public class SimulationLogic
    {
        #region Fields
        AnalogPoint pump1temp = null;
        AnalogPoint pump1flow = null;
        AnalogPoint tapChanger1 = null;

        AnalogPoint pump2temp = null;
        AnalogPoint pump2flow = null;
        AnalogPoint tapChanger2 = null;

        AnalogPoint pump3temp = null;
        AnalogPoint pump3flow = null;
        AnalogPoint tapChanger3 = null;

        BinaryPoint dis01 = null;
        BinaryPoint dis02 = null;
        BinaryPoint dis11 = null;
        BinaryPoint dis12 = null;
        BinaryPoint dis13 = null;
        BinaryPoint dis21 = null;
        BinaryPoint dis22 = null;
        BinaryPoint dis23 = null;
        BinaryPoint breaker01 = null;
        BinaryPoint breaker11 = null;
        BinaryPoint breaker12 = null;
        BinaryPoint breaker13 = null;
        BinaryPoint breaker21 = null;
        BinaryPoint breaker22 = null;
        BinaryPoint breaker23 = null;

        AnalogPoint TRVoltage1 = null;
        AnalogPoint TRVoltage2 = null;
        AnalogPoint TRVoltage3 = null;
        AnalogPoint TRCurrent1 = null;
        AnalogPoint TRCurrent2 = null;
        AnalogPoint TRCurrent3 = null;

        AnalogPoint fluidLever = null;

        private float MaxTemp;
        private float MinTemp;
        private bool colding1Pump = false;
        private bool colding2Pump = false;
        private bool colding3Pump = false;
        private int hourIndex = 0;
        private WeatherAPI WA;
        private float FullTank;
        private float EmptyTank;
        private float HeatingConst;
        private float VoltageFactor;
        private float ColdingConst;
        private float ConstPumpFlow;
        private float TankSurface;
        private dnp3types.sDNPServerDatabase db;
        private int secondsCount = 0;
        private List<double> hours = new List<double>();
        private Dictionary<string, ushort> pairs;
        ISimulator simulator;
        #endregion

        public SimulationLogic(ISimulator simulator)
        {
            this.simulator = simulator;
            FullTank = float.Parse(ConfigurationManager.AppSettings["FullTank"]);
            EmptyTank = float.Parse(ConfigurationManager.AppSettings["EmptyTank"]);
            MaxTemp = float.Parse(ConfigurationManager.AppSettings["MaxTemp"]);
            MinTemp = float.Parse(ConfigurationManager.AppSettings["MinTemp"]);
            HeatingConst = float.Parse(ConfigurationManager.AppSettings["HeatingConst"]);
            VoltageFactor = float.Parse(ConfigurationManager.AppSettings["VoltageFactor"]);
            ColdingConst = float.Parse(ConfigurationManager.AppSettings["ColdingConst"]);
            ConstPumpFlow = float.Parse(ConfigurationManager.AppSettings["ConstPumpFlow"]);
            TankSurface = float.Parse(ConfigurationManager.AppSettings["TankSurface"]);
            WA = new WeatherAPI();
            hours = WA.GetResultsForNext6Hours();
            db = new dnp3_protocol.dnp3types.sDNPServerDatabase();
        }

        private void OnEveryHour()
        {
            if (hourIndex == 5)
            {
                hourIndex = 0;
                hours = WA.GetResultsForNext6Hours();
            }

            if (db.u32TotalPoints != 15 && db.u32TotalPoints != 24 && db.u32TotalPoints != 33)
                return;

            simulator.MarshalUnmananagedArray2Struct(db.psServerDatabasePoint, (int)db.u32TotalPoints, out dnp3_protocol.dnp3types.sServerDatabasePoint[] points);

            var result = simulator.ConvertToPoints(points);


            foreach (var item in result)
            {
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT || item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
                {
                    var point = item as AnalogPoint;
                    if (point.Index == 1 && point.GroupId == dnp3types.eDNP3GroupID.ANALOG_INPUT) //FLUID LEVER - AI
                    {
                        SingleInt32Union analogValue = new SingleInt32Union();

                        analogValue.f = point.Value + (float)hours[hourIndex] * TankSurface;
                        //analogValue.f = point.Value + 1000; -- TEST
                        simulator.UpdatePoint(pairs["FluidLevel_Tank"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue);

                        if (point.Value > FullTank) //FULL TENK
                        {
                            SingleInt32Union digitalValue = new SingleInt32Union();
                            digitalValue.i = 1;
                            simulator.UpdatePoint(pairs["FullTank"], dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue);
                        }
                    }
                }
            }
            hourIndex++;
        }
        public void Simulate(dnp3types.sDNPServerDatabase db, Dictionary<string, ushort> pairs)
        {
            this.db = db;
            this.pairs = pairs;
            if (secondsCount == 3600)
            {
                OnEveryHour();
                secondsCount = 0;
            }
            else
            {

                if (db.u32TotalPoints == 15)
                {
                    Configuration1();
                }
                else if (db.u32TotalPoints == 24)
                {
                    Configuration2();
                }
                else if (db.u32TotalPoints == 33)
                {
                    Configuration3();
                }
                secondsCount++;
            }
        }

        private void GetPoints()
        {
            simulator.MarshalUnmananagedArray2Struct(db.psServerDatabasePoint, (int)db.u32TotalPoints, out dnp3_protocol.dnp3types.sServerDatabasePoint[] points);

            var result = simulator.ConvertToPoints(points);

            foreach (var item in result)
            {
                /////// BINARY OUTPUT

                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Breaker_01Status"])
                    breaker01 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Breaker_12Status"])
                    breaker12 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Breaker_22Status"])
                    breaker22 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc01"])
                    dis01 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc02"])
                    dis02 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc12"])
                    dis12 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc22"])
                    dis22 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Breaker_11Status"])
                    breaker11 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Breaker_13Status"])
                    breaker13 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Breaker_21Status"])
                    breaker21 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Breaker_23Status"])
                    breaker23 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc11"])
                    dis11 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc13"])
                    dis13 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc21"])
                    dis21 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == pairs["Discrete_Disc23"])
                    dis23 = item as BinaryPoint;

                /////// ANALOG OUTPUT

                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Flow_AM2"])
                    pump2flow = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Temp_AM2"])
                    pump2temp = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Flow_AM1"])
                    pump1flow = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Flow_AM3"])
                    pump3flow = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Temp_AM1"])
                    pump1temp = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Temp_AM3"])
                    pump3temp = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Discrete_Tap2"])
                    tapChanger2 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Discrete_Tap1"])
                    tapChanger1 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == pairs["Discrete_Tap3"])
                    tapChanger3 = item as AnalogPoint;

                /////// ANALOG INPUT

                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == pairs["Current_Tap2"])
                    TRCurrent2 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == pairs["FluidLevel_Tank"])
                    fluidLever = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == pairs["Voltage_Tap2"])
                    TRVoltage2 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == pairs["Current_Tap1"])
                    TRCurrent1 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == pairs["Current_Tap3"])
                    TRCurrent3 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == pairs["Voltage_Tap1"])
                    TRVoltage1 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == pairs["Voltage_Tap3"])
                    TRVoltage3 = item as AnalogPoint;

            }
        }

        private void UpdatePoints()
        {
            /////// ANALOG INPUT

            SingleInt32Union analogValue = new SingleInt32Union();
            analogValue.f = TRCurrent1.Value;
            simulator.UpdatePoint(pairs["Current_Tap1"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue);

            SingleInt32Union analogValue2 = new SingleInt32Union();
            analogValue2.f = TRCurrent2.Value;
            simulator.UpdatePoint(pairs["Current_Tap2"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue2);

            SingleInt32Union analogValue3 = new SingleInt32Union();
            analogValue3.f = fluidLever.Value;
            simulator.UpdatePoint(pairs["FluidLevel_Tank"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue3);

            SingleInt32Union analogValue4 = new SingleInt32Union();
            analogValue4.f = TRVoltage2.Value;
            simulator.UpdatePoint(pairs["Voltage_Tap2"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue4);

            SingleInt32Union analogValue5 = new SingleInt32Union();
            analogValue5.f = TRCurrent3.Value;
            simulator.UpdatePoint(pairs["Current_Tap3"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue5);

            SingleInt32Union analogValue6 = new SingleInt32Union();
            analogValue6.f = TRVoltage1.Value;
            simulator.UpdatePoint(pairs["Voltage_Tap1"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue6);

            SingleInt32Union analogValue7 = new SingleInt32Union();
            analogValue7.f = TRVoltage3.Value;
            simulator.UpdatePoint(pairs["Voltage_Tap3"], dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue7);


            /////// ANALOG OUTPUT

            SingleInt32Union analogValue8 = new SingleInt32Union();
            analogValue8.f = tapChanger2.Value;
            simulator.UpdatePoint(pairs["Discrete_Tap2"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue8);

            SingleInt32Union analogValue9 = new SingleInt32Union();
            analogValue9.f = tapChanger1.Value;
            simulator.UpdatePoint(pairs["Discrete_Tap1"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue9);

            SingleInt32Union analogValue10 = new SingleInt32Union();
            analogValue10.f = tapChanger3.Value;
            simulator.UpdatePoint(pairs["Discrete_Tap3"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue10);

            SingleInt32Union analogValue11 = new SingleInt32Union();
            analogValue11.f = pump3flow.Value;
            simulator.UpdatePoint(pairs["Flow_AM3"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue11);

            SingleInt32Union analogValue12 = new SingleInt32Union();
            analogValue12.f = pump1temp.Value;
            simulator.UpdatePoint(pairs["Temp_AM1"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue12);

            SingleInt32Union analogValue13 = new SingleInt32Union();
            analogValue13.f = pump3temp.Value;
            simulator.UpdatePoint(pairs["Temp_AM3"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue13);

            SingleInt32Union analogValue14 = new SingleInt32Union();
            analogValue14.f = pump2flow.Value;
            simulator.UpdatePoint(pairs["Flow_AM2"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue14);

            SingleInt32Union analogValue15 = new SingleInt32Union();
            analogValue15.f = pump2temp.Value;
            simulator.UpdatePoint(pairs["Temp_AM2"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue15);

            SingleInt32Union analogValue16 = new SingleInt32Union();
            analogValue16.f = pump1flow.Value;
            simulator.UpdatePoint(pairs["Flow_AM1"], dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue16);


            /////// BINARY OUTPUT

            SingleInt32Union digitalValue = new SingleInt32Union();
            digitalValue.i = breaker01.Value;
            simulator.UpdatePoint(pairs["Breaker_01Status"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue);

            SingleInt32Union digitalValue2 = new SingleInt32Union();
            digitalValue2.i = breaker12.Value;
            simulator.UpdatePoint(pairs["Breaker_12Status"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue2);

            SingleInt32Union digitalValue3 = new SingleInt32Union();
            digitalValue3.i = breaker22.Value;
            simulator.UpdatePoint(pairs["Breaker_22Status"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue3);

            SingleInt32Union digitalValue4 = new SingleInt32Union();
            digitalValue4.i = dis01.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc01"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue4);

            SingleInt32Union digitalValue5 = new SingleInt32Union();
            digitalValue5.i = dis02.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc02"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue5);

            SingleInt32Union digitalValue6 = new SingleInt32Union();
            digitalValue6.i = dis12.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc12"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue6);

            SingleInt32Union digitalValue7 = new SingleInt32Union();
            digitalValue7.i = dis22.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc22"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue7);

            SingleInt32Union digitalValue8 = new SingleInt32Union();
            digitalValue8.i = breaker11.Value;
            simulator.UpdatePoint(pairs["Breaker_11Status"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue8);

            SingleInt32Union digitalValue9 = new SingleInt32Union();
            digitalValue9.i = breaker13.Value;
            simulator.UpdatePoint(pairs["Breaker_13Status"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue9);

            SingleInt32Union digitalValue10 = new SingleInt32Union();
            digitalValue10.i = breaker21.Value;
            simulator.UpdatePoint(pairs["Breaker_21Status"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue10);

            SingleInt32Union digitalValue11 = new SingleInt32Union();
            digitalValue11.i = breaker23.Value;
            simulator.UpdatePoint(pairs["Breaker_23Status"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue11);

            SingleInt32Union digitalValue12 = new SingleInt32Union();
            digitalValue12.i = dis11.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc11"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue12);

            SingleInt32Union digitalValue13 = new SingleInt32Union();
            digitalValue13.i = dis13.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc13"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue13);

            SingleInt32Union digitalValue14 = new SingleInt32Union();
            digitalValue14.i = dis21.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc21"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue14);

            SingleInt32Union digitalValue15 = new SingleInt32Union();
            digitalValue15.i = dis23.Value;
            simulator.UpdatePoint(pairs["Discrete_Disc23"], dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue15);

        }

        private void Configuration1()
        {

            GetPoints();

            if (colding2Pump && pump2temp.Value <= MinTemp)
                colding2Pump = false;

            if (pump2temp.Value > MaxTemp)
                colding2Pump = true;


            if (fluidLever.Value > EmptyTank && breaker01.Value == 1 && dis01.Value == 1 && dis02.Value == 1 && dis12.Value == 1 && dis22.Value == 1 && breaker22.Value == 1 && breaker12.Value == 1 && !colding2Pump) //all closed
            {
                PumpRunning(pump2flow, pump2temp, tapChanger2, TRVoltage2, TRCurrent2);
            }
            else
            {
                if (colding2Pump)
                {
                    pump2temp.Value -= ColdingConst;
                }
            }

            CheckFluidLevel();

            UpdatePoints();
        }

        private void Configuration2()
        {
            GetPoints();

            //colding first pump
            if (colding1Pump && pump1temp.Value <= MinTemp)
                colding1Pump = false;

            if (pump1temp.Value > MaxTemp)
                colding1Pump = true;

            //colding second pump
            if (colding2Pump && pump2temp.Value <= MinTemp)
                colding2Pump = false;

            if (pump2temp.Value > MaxTemp)
                colding2Pump = true;

            if (fluidLever.Value > EmptyTank && breaker01.Value == 1 && dis01.Value == 1 && dis02.Value == 1 &&
                dis12.Value == 1 && dis22.Value == 1 && breaker22.Value == 1 && breaker12.Value == 1 && !colding2Pump) //all closed
            {
                PumpRunning(pump2flow, pump2temp, tapChanger2, TRVoltage2, TRCurrent2);
            }
            else
            {
                if (colding2Pump)
                {
                    pump2temp.Value -= ColdingConst;
                }
            }


            if (fluidLever.Value > EmptyTank && breaker01.Value == 1 && dis01.Value == 1 && dis02.Value == 1 &&
                dis11.Value == 1 && dis21.Value == 1 && breaker21.Value == 1 && breaker11.Value == 1 && !colding1Pump)
            {
                PumpRunning(pump1flow, pump1temp, tapChanger1, TRVoltage1, TRCurrent1);
            }
            else
            {
                if (colding1Pump)
                {
                    pump1temp.Value -= ColdingConst;
                }
            }

            CheckFluidLevel();

            UpdatePoints();
        }

        private void Configuration3()
        {
            GetPoints();

            //colding first pump
            if (colding1Pump && pump1temp.Value <= MinTemp)
                colding1Pump = false;

            if (pump1temp.Value > MaxTemp)
                colding1Pump = true;

            //colding second pump
            if (colding2Pump && pump2temp.Value <= MinTemp)
                colding2Pump = false;

            if (pump2temp.Value > MaxTemp)
                colding2Pump = true;

            //colding third pump
            if (colding2Pump && pump2temp.Value <= MinTemp)
                colding2Pump = false;

            if (pump2temp.Value > MaxTemp)
                colding2Pump = true;

            if (fluidLever.Value > EmptyTank && breaker01.Value == 1 && dis01.Value == 1 && dis02.Value == 1 &&
                dis12.Value == 1 && dis22.Value == 1 && breaker22.Value == 1 && breaker12.Value == 1 && !colding2Pump) //all closed
            {
                PumpRunning(pump2flow, pump2temp, tapChanger2, TRVoltage2, TRCurrent2);
            }
            else
            {
                if (colding2Pump)
                {
                    pump2temp.Value -= ColdingConst;
                }
            }


            if (fluidLever.Value > EmptyTank && breaker01.Value == 1 && dis01.Value == 1 && dis02.Value == 1 &&
                dis11.Value == 1 && dis21.Value == 1 && breaker21.Value == 1 && breaker11.Value == 1 && !colding1Pump)
            {
                PumpRunning(pump1flow, pump1temp, tapChanger1, TRVoltage1, TRCurrent1);
            }
            else
            {
                if (colding1Pump)
                {
                    pump1temp.Value -= ColdingConst;
                }
            }

            if (fluidLever.Value > EmptyTank && breaker01.Value == 1 && dis01.Value == 1 && dis02.Value == 1 &&
               dis13.Value == 1 && dis23.Value == 1 && breaker23.Value == 1 && breaker13.Value == 1 && !colding3Pump)
            {
                PumpRunning(pump3flow, pump3temp, tapChanger3, TRVoltage3, TRCurrent3);
            }
            else
            {
                if (colding3Pump)
                {
                    pump3temp.Value -= ColdingConst;
                }
            }

            CheckFluidLevel();

            UpdatePoints();
        }

        private void CheckFluidLevel()
        {
            if (fluidLever.Value < EmptyTank) //EMPTY TENK
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 1;
                simulator.UpdatePoint(pairs["EmptyTank"], dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal);
            }

            if (fluidLever.Value >= EmptyTank && fluidLever.Value <= FullTank)
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 0;
                simulator.UpdatePoint(pairs["EmptyTank"], dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal);
                simulator.UpdatePoint(pairs["FullTank"], dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal);
            }
        }

        private void PumpRunning(AnalogPoint pumpFlow, AnalogPoint pumpTemp, AnalogPoint tapChanger, AnalogPoint voltage, AnalogPoint current)
        {
            pumpFlow.Value = tapChanger.Value * VoltageFactor * ConstPumpFlow;

            if (fluidLever.Value - pumpFlow.Value >= EmptyTank)
            {
                voltage.Value = tapChanger.Value * VoltageFactor;
                current.Value = 100 / voltage.Value;
                voltage.Value = tapChanger.Value * VoltageFactor;
                pumpTemp.Value += HeatingConst * voltage.Value; // 0.1 * 1 * 100
                fluidLever.Value -= pumpFlow.Value;
            }
        }
    }
}
