using Core.Common.WeatherApi;
using dnp3_protocol;
using Simulator.Core.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Simulator.Core
{

    public class Simulator : ISimulator, IDisposable
    {
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
        private List<double> hours = new List<double>();
        private Thread worker;
        private bool executionFlag;
        private static IntPtr DNP3serverhandle;
        private static dnp3_protocol.dnp3types.sDNP3Object[] psDNP3Objects;
        private static short iErrorCode = 0;
        private static short ptErrorValue = 0;
        public event EventHandler<UpdateEventArgs> updateEvent;
        public dnp3_protocol.dnp3types.DNP3ControlOperateCallback operateCallback;
        public dnp3_protocol.dnp3types.DNP3DebugMessageCallback debugCallback;
        private List<Point> prevList;
        private dnp3_protocol.dnp3types.sDNP3ConfigurationParameters sDNP3Config;
        private static bool configChange;
        private static Tuple<ushort, ushort, ushort, ushort> config;
        private WeatherAPI WA;
        private float FullTank;
        private float EmptyTank;
        private float HeatingConst;
        private float VoltageFactor;
        private float ColdingConst;
        private float ConstPumpFlow;
        private float TankSurface;
        private dnp3_protocol.dnp3types.sDNPServerDatabase db;
        private int secondsCount = 0;
        private static  Dictionary<string, ushort> pairs;
        private static  Dictionary<string, ushort> incomingPairs;
        public int interval { get; set; }

        public Simulator()
        {
            prevList = new List<Point>();
            operateCallback = new dnp3_protocol.dnp3types.DNP3ControlOperateCallback(cbOperate);
            debugCallback = new dnp3_protocol.dnp3types.DNP3DebugMessageCallback(cbDebug);
            interval = Int32.Parse(ConfigurationManager.AppSettings["interval"]);
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

        public void LoadConfifg(Tuple<ushort, ushort, ushort, ushort> pointsNum)
        {
            dnp3_protocol.dnp3api.DNP3GetServerDatabaseValue(DNP3serverhandle, ref db, ref ptErrorValue);
            MarshalUnmananagedArray2Struct(db.psServerDatabasePoint, (int)db.u32TotalPoints, out dnp3_protocol.dnp3types.sServerDatabasePoint[] points);
            var result = ConvertToPoints(points);
            GeneratePoints(ref psDNP3Objects, pointsNum);
            iErrorCode = dnp3_protocol.dnp3api.DNP3LoadConfiguration(DNP3serverhandle, ref sDNP3Config, ref ptErrorValue);
            if (iErrorCode != 0)
            {
                Trace.TraceError("DNP3 Load failed");
                Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));
            }
            pairs = incomingPairs;
            configChange = false;
            SetValues(result);
        }

        private void SetValues(List<Point> oldPoints)
        {
            foreach (var item in oldPoints)
            {
                if(item.GroupId == dnp3types.eDNP3GroupID.ANALOG_INPUT || item.GroupId == dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
                {
                    var point = item as AnalogPoint;
                    SingleInt32Union analogValue = new SingleInt32Union();
                    analogValue.f = point.Value;
                    Update(item.Index, item.GroupId, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA,analogValue, ref ptErrorValue);
                }
                else
                {
                    var point = item as BinaryPoint;
                    SingleInt32Union binaryValue = new SingleInt32Union();
                    binaryValue.i = point.Value;
                    Update(item.Index, item.GroupId, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, binaryValue, ref ptErrorValue);
                }
            }
        }

        public void Start()
        {
            this.worker = new Thread(DoWork);
            executionFlag = true;
            this.worker.Name = "Simulator thread";
            worker.Start();
        }

        public void Stop()
        {
            executionFlag = false;
            Dispose();
            iErrorCode = dnp3_protocol.dnp3api.DNP3Stop(DNP3serverhandle, ref ptErrorValue);
            if (iErrorCode != 0)
            {
                Trace.TraceError("DNP3 stop failed");
                Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));
            }

            iErrorCode = dnp3_protocol.dnp3api.DNP3Free(DNP3serverhandle, ref ptErrorValue);
            if (iErrorCode != 0)
            {
                Trace.TraceError("DNP3 free failed");
                Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));
            }
        }

        private void DoWork()
        {
            Thread.Sleep(2000);
            if (Prepare())
            {
                while (executionFlag)
                {
                    dnp3_protocol.dnp3types.sDNPServerDatabase db = new dnp3_protocol.dnp3types.sDNPServerDatabase();
                    dnp3_protocol.dnp3api.DNP3GetServerDatabaseValue(DNP3serverhandle, ref db, ref ptErrorValue);
                    MarshalUnmananagedArray2Struct(db.psServerDatabasePoint, (int)db.u32TotalPoints, out dnp3_protocol.dnp3types.sServerDatabasePoint[] points);
                    var result = ConvertToPoints(points);
                    foreach (var item in result)
                    {
                        if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT || item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
                        {
                            var point = item as AnalogPoint;
                        }
                        else
                        {
                            var point = item as BinaryPoint;
                        }
                    }
                    updateEvent?.Invoke(this, new UpdateEventArgs() { Points = result });
                    Simulation();
                    prevList = result;
                    if (configChange)
                        LoadConfifg(config);
                    Thread.Sleep(interval);
                }
            }
           
        }
        private void OnEveryHour()
        {
            if (hourIndex == 5)
            {
                hourIndex = 0;
                hours = WA.GetResultsForNext6Hours();
            }

            dnp3_protocol.dnp3api.DNP3GetServerDatabaseValue(DNP3serverhandle, ref db, ref ptErrorValue);

            if (db.u32TotalPoints != 15 && db.u32TotalPoints != 24 && db.u32TotalPoints != 33)
                return;

            MarshalUnmananagedArray2Struct(db.psServerDatabasePoint, (int)db.u32TotalPoints, out dnp3_protocol.dnp3types.sServerDatabasePoint[] points);

            var result = ConvertToPoints(points);


            foreach (var item in result)
            {
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT || item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
                {
                    var point = item as AnalogPoint;
                    if (point.Index == 1 && point.GroupId==dnp3types.eDNP3GroupID.ANALOG_INPUT) //FLUID LEVER - AI
                    {
                        SingleInt32Union analogValue = new SingleInt32Union();

                        analogValue.f = point.Value + (float)hours[hourIndex] * TankSurface;
                        //analogValue.f = point.Value + 1000; -- TEST
                        Update(1, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue, ref ptErrorValue);

                        if (point.Value > FullTank) //FULL TENK
                        {
                            SingleInt32Union digitalValue = new SingleInt32Union();
                            digitalValue.i = 1;
                            Update(1, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue, ref ptErrorValue);
                        }                   
                    }
                }
            }
            hourIndex++;
        }

        private void Simulation()
        {
            if (secondsCount == 3600)
            {
                OnEveryHour();
                secondsCount = 0;
            }
            else
            {
                //emptying the tank

                dnp3_protocol.dnp3api.DNP3GetServerDatabaseValue(DNP3serverhandle, ref db, ref ptErrorValue);

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
            MarshalUnmananagedArray2Struct(db.psServerDatabasePoint, (int)db.u32TotalPoints, out dnp3_protocol.dnp3types.sServerDatabasePoint[] points);

            var result = ConvertToPoints(points);

            foreach (var item in result)
            {
                /////// BINARY OUTPUT

                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 0)
                    breaker01 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 1)
                    breaker12 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 2)
                    breaker22 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 3)
                    dis01 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 4)
                    dis02 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 5)
                    dis12 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 6)
                    dis22 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 7)
                    breaker11 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 8)
                    breaker13 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 9)
                    breaker21 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 10)
                    breaker23 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 11)
                    dis11 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 12)
                    dis13 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 13)
                    dis21 = item as BinaryPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT && item.Index == 14)
                    dis23 = item as BinaryPoint;

                /////// ANALOG OUTPUT
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 0)
                    pump2flow = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 1)
                    pump2temp = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 2)
                    pump1flow = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 3)
                    pump3flow = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 4)
                    pump1temp = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 5)
                    pump3temp = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 6)
                    tapChanger2 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 7)
                    tapChanger1 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS && item.Index == 8)
                    tapChanger3 = item as AnalogPoint;

                /////// ANALOG INPUT

                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == 0)
                    TRCurrent2 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == 1)
                    fluidLever = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == 2)
                    TRVoltage2 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == 3)
                    TRCurrent1 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == 4)
                    TRCurrent3 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == 5)
                    TRVoltage1 = item as AnalogPoint;
                if (item.GroupId == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT && item.Index == 6)
                    TRVoltage3 = item as AnalogPoint;

            }
        }

        private void UpdatePoints()
        {
            /////// ANALOG INPUT

            SingleInt32Union analogValue = new SingleInt32Union();
            analogValue.f = TRCurrent1.Value;
            Update(3, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue, ref ptErrorValue);

            SingleInt32Union analogValue2 = new SingleInt32Union();
            analogValue2.f = TRCurrent2.Value;
            Update(0, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue2, ref ptErrorValue);

            SingleInt32Union analogValue3 = new SingleInt32Union();
            analogValue3.f = fluidLever.Value;
            Update(1, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue3, ref ptErrorValue);

            SingleInt32Union analogValue4 = new SingleInt32Union();
            analogValue4.f = TRVoltage2.Value;
            Update(2, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue4, ref ptErrorValue);

            SingleInt32Union analogValue5 = new SingleInt32Union();
            analogValue5.f = TRCurrent3.Value;
            Update(4, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue5, ref ptErrorValue);

            SingleInt32Union analogValue6 = new SingleInt32Union();
            analogValue6.f = TRVoltage1.Value;
            Update(5, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue6, ref ptErrorValue);
            
            SingleInt32Union analogValue7 = new SingleInt32Union();
            analogValue7.f = TRVoltage3.Value;
            Update(6, dnp3types.eDNP3GroupID.ANALOG_INPUT, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue7, ref ptErrorValue);


            /////// ANALOG OUTPUT

            SingleInt32Union analogValue8 = new SingleInt32Union();
            analogValue8.f = tapChanger2.Value;
            Update(6, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue8, ref ptErrorValue);

            SingleInt32Union analogValue9 = new SingleInt32Union();
            analogValue9.f = tapChanger1.Value;
            Update(7, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue9, ref ptErrorValue);

            SingleInt32Union analogValue10 = new SingleInt32Union();
            analogValue10.f = tapChanger3.Value;
            Update(8, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue10, ref ptErrorValue);

            SingleInt32Union analogValue11 = new SingleInt32Union();
            analogValue11.f = pump3flow.Value;
            Update(3, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue11, ref ptErrorValue);

            SingleInt32Union analogValue12 = new SingleInt32Union();
            analogValue12.f = pump1temp.Value;
            Update(4, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue12, ref ptErrorValue);

            SingleInt32Union analogValue13 = new SingleInt32Union();
            analogValue13.f = pump3temp.Value;
            Update(5, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue13, ref ptErrorValue);

            SingleInt32Union analogValue14 = new SingleInt32Union();
            analogValue14.f = pump2flow.Value;
            Update(0, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue14, ref ptErrorValue);

            SingleInt32Union analogValue15 = new SingleInt32Union();
            analogValue15.f = pump2temp.Value;
            Update(1, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue15, ref ptErrorValue);

            SingleInt32Union analogValue16 = new SingleInt32Union();
            analogValue16.f = pump1flow.Value;
            Update(2, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, analogValue16, ref ptErrorValue);


            /////// BINARY OUTPUT

            SingleInt32Union digitalValue = new SingleInt32Union();
            digitalValue.i = breaker01.Value;
            Update(0, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue, ref ptErrorValue);

            SingleInt32Union digitalValue2 = new SingleInt32Union();
            digitalValue2.i = breaker12.Value;
            Update(1, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue2, ref ptErrorValue);

            SingleInt32Union digitalValue3 = new SingleInt32Union();
            digitalValue3.i = breaker22.Value;
            Update(2, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue3, ref ptErrorValue);

            SingleInt32Union digitalValue4 = new SingleInt32Union();
            digitalValue4.i = dis01.Value;
            Update(3, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue4, ref ptErrorValue);

            SingleInt32Union digitalValue5 = new SingleInt32Union();
            digitalValue5.i = dis02.Value;
            Update(4, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue5, ref ptErrorValue);

            SingleInt32Union digitalValue6 = new SingleInt32Union();
            digitalValue6.i = dis12.Value;
            Update(5, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue6, ref ptErrorValue);

            SingleInt32Union digitalValue7 = new SingleInt32Union();
            digitalValue7.i = dis22.Value;
            Update(6, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue7, ref ptErrorValue);

            SingleInt32Union digitalValue8 = new SingleInt32Union();
            digitalValue8.i = breaker11.Value;
            Update(7, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue8, ref ptErrorValue);

            SingleInt32Union digitalValue9 = new SingleInt32Union();
            digitalValue9.i = breaker13.Value;
            Update(8, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue9, ref ptErrorValue);

            SingleInt32Union digitalValue10 = new SingleInt32Union();
            digitalValue10.i = breaker21.Value;
            Update(9, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue10, ref ptErrorValue);

            SingleInt32Union digitalValue11 = new SingleInt32Union();
            digitalValue11.i = breaker23.Value;
            Update(10, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue11, ref ptErrorValue);

            SingleInt32Union digitalValue12 = new SingleInt32Union();
            digitalValue12.i = dis11.Value;
            Update(11, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue12, ref ptErrorValue);

            SingleInt32Union digitalValue13 = new SingleInt32Union();
            digitalValue13.i = dis13.Value;
            Update(12, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue13, ref ptErrorValue);

            SingleInt32Union digitalValue14 = new SingleInt32Union();
            digitalValue14.i = dis21.Value;
            Update(13, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue14, ref ptErrorValue);

            SingleInt32Union digitalValue15 = new SingleInt32Union();
            digitalValue15.i = dis23.Value;
            Update(14, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue15, ref ptErrorValue);

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
                pump2flow.Value = tapChanger2.Value * VoltageFactor * ConstPumpFlow; // 1 * 100 * 1 => 100 l/s

                if (fluidLever.Value - pump2flow.Value >= EmptyTank)
                {
                    pump2temp.Value = (float)(pump2temp.Value + HeatingConst * tapChanger2.Value * VoltageFactor); // 0.1 * 1 * 100
                    fluidLever.Value -= pump2flow.Value;
                }
            }
            else
            {
                if (colding2Pump)
                {
                    pump2temp.Value -= ColdingConst;
                }
            }

            if (fluidLever.Value < EmptyTank) //EMPTY TENK
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 1;
                Update(0, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
            }

            if (fluidLever.Value >= EmptyTank && fluidLever.Value <= FullTank)
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 0;
                Update(0, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
                Update(1, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
            }

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
                pump2flow.Value = tapChanger2.Value * VoltageFactor * ConstPumpFlow; // 1 * 100 * 1 => 100 l/s

                if (fluidLever.Value - pump2flow.Value >= EmptyTank)
                {
                    pump2temp.Value = (float)(pump2temp.Value + HeatingConst * tapChanger2.Value * VoltageFactor); // 0.1 * 1 * 100
                    fluidLever.Value -= pump2flow.Value;
                }
            }
            else
            {
                if (colding2Pump)
                {
                    pump2temp.Value -= ColdingConst;
                }
            }


            if (fluidLever.Value > EmptyTank && breaker01.Value == 1 && dis01.Value == 1 && dis02.Value == 1 &&      
                dis11.Value == 1 && dis21.Value == 1 && breaker21.Value == 1 && breaker11.Value == 1 && !colding1Pump )
            {
                pump1flow.Value = tapChanger1.Value * VoltageFactor * ConstPumpFlow;

                if (fluidLever.Value - pump1flow.Value >= EmptyTank)
                {
                    pump1temp.Value = (float)(pump1temp.Value + HeatingConst * tapChanger1.Value * VoltageFactor);
                    fluidLever.Value -= pump1flow.Value;
                }
            }
            else
            {
                if (colding1Pump)
                {
                    pump1temp.Value -= ColdingConst;
                }
            }

            if (fluidLever.Value < EmptyTank) //EMPTY TENK
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 1;
                Update(0, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
            }

            if (fluidLever.Value >= EmptyTank && fluidLever.Value <= FullTank)
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 0;
                Update(0, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
                Update(1, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
            }

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
                pump2flow.Value = tapChanger2.Value * VoltageFactor * ConstPumpFlow; // 1 * 100 * 1 => 100 l/s

                if (fluidLever.Value - pump2flow.Value >= EmptyTank)
                {
                    pump2temp.Value = (float)(pump2temp.Value + HeatingConst * tapChanger2.Value * VoltageFactor); // 0.1 * 1 * 100
                    fluidLever.Value -= pump2flow.Value;
                }
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
                pump1flow.Value = tapChanger1.Value * VoltageFactor * ConstPumpFlow;

                if (fluidLever.Value - pump1flow.Value >= EmptyTank)
                {
                    pump1temp.Value = (float)(pump1temp.Value + HeatingConst * tapChanger1.Value * VoltageFactor);
                    fluidLever.Value -= pump1flow.Value;
                }
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
                pump3flow.Value = tapChanger3.Value * VoltageFactor * ConstPumpFlow;

                if (fluidLever.Value - pump3flow.Value >= EmptyTank)
                {
                    pump3temp.Value = (float)(pump3temp.Value + HeatingConst * tapChanger3.Value * VoltageFactor);
                    fluidLever.Value -= pump3flow.Value;
                }
            }
            else
            {
                if (colding3Pump)
                {
                    pump3temp.Value -= ColdingConst;
                }
            }

            if (fluidLever.Value < EmptyTank) //EMPTY TENK
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 1;
                Update(0, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
            }

            if (fluidLever.Value >= EmptyTank && fluidLever.Value <= FullTank)
            {
                SingleInt32Union digVal = new SingleInt32Union();
                digVal.i = 0;
                Update(0, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
                Update(1, dnp3types.eDNP3GroupID.BINARY_INPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digVal, ref ptErrorValue);
            }

            UpdatePoints();
        }

        private bool Prepare()
        {
           
            DateTime date;
            dnp3_protocol.dnp3types.sDNP3Parameters sParameters;    // DNP3 Server object callback paramters 

            try
            {
                if (String.Compare(System.Runtime.InteropServices.Marshal.PtrToStringAnsi(dnp3_protocol.dnp3api.DNP3GetLibraryVersion()), dnp3_protocol.dnp3api.DNP3_VERSION, true) != 0)
                {
                    return false;
                }
            }
            catch (DllNotFoundException e)
            {
                Trace.TraceError(e.Message);
                return false;
            }

            DNP3serverhandle = System.IntPtr.Zero;
            sParameters = new dnp3_protocol.dnp3types.sDNP3Parameters();

            // Initialize parameters
            sParameters.eAppFlag = dnp3_protocol.tgtcommon.eApplicationFlag.APP_SERVER;			                // This is a DNP3 Server   
            sParameters.ptReadCallback = null;					                                                // Read Callback
            sParameters.ptWriteCallback = null;                                 			                    // Write Callback           
            sParameters.ptUpdateCallback = null;					                                            // Update Callback
            sParameters.ptSelectCallback = null;                                                        	    // Select commands
            sParameters.ptOperateCallback = operateCallback;                                                    // Operate commands
            sParameters.ptDebugCallback = debugCallback;                                                        // Debug Callback
            sParameters.ptColdRestartCallback = null;                                                           // ColdRestart Callback
            sParameters.ptWarmRestartCallback = null;                                                           // WarmRestart Callback
            sParameters.ptClientStatusCallback = null;                                                          // Client connection status callback
            sParameters.ptDeviceAttrCallback = null;                                                            // Device attribute callback
            sParameters.ptUpdateIINCallback = null;                                                             // IIN internal indication update callback
            sParameters.u16ObjectId = 1;				                                                        // Server ID which used in callbacks to identify the DNP3 server object   

            sParameters.u32Options = 0;

            DNP3serverhandle = dnp3_protocol.dnp3api.DNP3Create(ref sParameters, ref iErrorCode, ref ptErrorValue);
            if (DNP3serverhandle == System.IntPtr.Zero)
            {
                Trace.TraceError("DNP3 Library API Function - Create failed");
                return false;
            }


            // Server load configuration - communication and protocol configuration parameters
            sDNP3Config = new dnp3_protocol.dnp3types.sDNP3ConfigurationParameters();
            // tcp communication settings
            sDNP3Config.sDNP3ServerSet.sServerCommunicationSet.eCommMode = dnp3_protocol.dnp3types.eCommunicationMode.TCP_IP_MODE;
            sDNP3Config.sDNP3ServerSet.sServerCommunicationSet.sEthernetCommsSet.sEthernetportSet.ai8FromIPAddress = ConfigurationManager.AppSettings["address"]??"127.0.0.1";
            sDNP3Config.sDNP3ServerSet.sServerCommunicationSet.sEthernetCommsSet.sEthernetportSet.u16PortNumber = ushort.Parse(ConfigurationManager.AppSettings["port"]);

            //protocol communication settings
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u16SlaveAddress = 1;               // slave address
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u16MasterAddress = 2;              // master address
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u32LinkLayerTimeout = 2000;        // link layer time out in ms
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u32ApplicationLayerTimeout = 6000; // app link layer time out in ms
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u32TimeSyncIntervalSeconds = 90;   // time sync bit will set for every 90 seconds

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarBI = dnp3_protocol.dnp3types.eDefaultStaticVariationBinaryInput.BI_PACKED_FORMAT;                     // Default Static variation Binary Input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarDBI = dnp3_protocol.dnp3types.eDefaultStaticVariationDoubleBitBinaryInput.DBBI_PACKED_FORMAT;         // Default Static variation Double Bit Binary Input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarBO = dnp3_protocol.dnp3types.eDefaultStaticVariationBinaryOutput.BO_PACKED_FORMAT;                    // Default Static variation Double Bit Binary Output
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarCI = dnp3_protocol.dnp3types.eDefaultStaticVariationCounterInput.CI_32BIT_WITHFLAG;                   // Default Static variation counter Input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarFzCI = dnp3_protocol.dnp3types.eDefaultStaticVariationFrozenCounterInput.FCI_32BIT_WITHFLAGANDTIME;   // Default Static variation Frozen counter Input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarAI = dnp3_protocol.dnp3types.eDefaultStaticVariationAnalogInput.AI_SINGLEPREC_FLOATWITHFLAG;          // Default Static variation Analog Input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarFzAI = dnp3_protocol.dnp3types.eDefaultStaticVariationFrozenAnalogInput.FAI_SINGLEPRECFLOATWITHFLAG;  // Default Static variation frozen Analog Input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarAID = dnp3_protocol.dnp3types.eDefaultStaticVariationAnalogInputDeadBand.DAI_SINGLEPRECFLOAT;         // Default Static variation Analog Input Deadband
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sStaticVariation.eDeStVarAO = dnp3_protocol.dnp3types.eDefaultStaticVariationAnalogOutput.AO_SINGLEPRECFLOAT_WITHFLAG;         // Default Static variation Analog Output

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarBI = dnp3_protocol.dnp3types.eDefaultEventVariationBinaryInput.BIE_WITHOUT_TIME;                       // Default event variation for binary input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarDBI = dnp3_protocol.dnp3types.eDefaultEventVariationDoubleBitBinaryInput.DBBIE_WITH_ABSOLUTETIME;      // Default event variation for double bit binary input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarCI = dnp3_protocol.dnp3types.eDefaultEventVariationCounterInput.CIE_32BIT_WITHFLAG_WITHTIME;           // Default event variation for Counter input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarAI = dnp3_protocol.dnp3types.eDefaultEventVariationAnalogInput.AIE_SINGLEPREC_WITHTIME;                // Default event variation for Analog input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarFzCI = dnp3_protocol.dnp3types.eDefaultEventVariationFrozenCounterInput.FCIE_32BIT_WITHFLAG_WITHTIME;  // Default event variation for Frozen counter input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarFzAI = dnp3_protocol.dnp3types.eDefaultEventVariationFrozenAnalogInput.FAIE_SINGLEPREC_WITHTIME;       // Default event variation for Frozen Analog input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarBO = dnp3_protocol.dnp3types.eDefaultEventVariationBinaryOutput.BOE_WITHOUT_TIME;                      // Default event variation for binary Output
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sEventVariation.eDeEvVarAO = dnp3_protocol.dnp3types.eDefaultEventVariationAnalogOutput.AOE_SINGLEPREC_WITHTIME;               // Default event variation for Analog Output

            sDNP3Config.sDNP3ServerSet.sServerProtSet.u16Class1EventBufferSize = 100;             // class 1 buffer size number of events to store
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u8Class1EventBufferOverFlowPercentage = 90;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u16Class2EventBufferSize = 100;             // class 2 buffer size number of events to store
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u8Class2EventBufferOverFlowPercentage = 90;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u16Class3EventBufferSize = 100;             // class 3 buffer size number of events to store
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u8Class3EventBufferOverFlowPercentage = 90;

            date = DateTime.Now;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u8Day = (byte)date.Day;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u8Month = (byte)date.Month;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u16Year = (ushort)date.Year;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u8Hour = (byte)date.Hour;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u8Minute = (byte)date.Minute;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u8Seconds = (byte)date.Second;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u16MilliSeconds = (ushort)date.Millisecond;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u16MicroSeconds = 0;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.i8DSTTime = 0;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sTimeStamp.u8DayoftheWeek = (byte)date.DayOfWeek;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddBIinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddDBIinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddBOinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddCIinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddFzCIinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddAIinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddFzAIinClass0 = 0;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddAIDinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddAOinClass0 = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddOSinClass0 = 1;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddBIEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddDBIEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddBOEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddCIEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddFzCIEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddAIEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddFzAIEvent = 0;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddAIDEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddAOEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddOSEvent = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bAddVTOEvent = 1;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.eAIDeadbandMethod = dnp3_protocol.dnp3types.eAnalogInputDeadbandMethod.DEADBAND_FIXED;    // Analog Input Deadband Calculation method
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bFrozenAnalogInputSupport = 0;                                                            // False- stack will not create points for frozen analog input
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bEnableSelfAddressSupport = 1;                                                            // Enable Self Address Support
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bEnableFileTransferSupport = 0;                                                           // Enable File Transfr Support*/
            sDNP3Config.sDNP3ServerSet.sServerProtSet.u8IntialdatabaseQualityFlag = (byte)dnp3_protocol.dnp3types.eDNP3QualityFlags.ONLINE;     // 0- OFFLINE, 1 BIT- ONLINE, 2 BIT-RESTART, 3 BIT -COMMLOST, MAX VALUE -7
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bLocalMode = 0;                                                                           // if local mode set true, then -all remote command for binary output/ analog output control statusset to not supported
            sDNP3Config.sDNP3ServerSet.sServerProtSet.bUpdateCheckTimestamp = 0;                                                                // if it true ,the timestamp change also generate event  during the dnp3update

            //Unsolicited Response Setttings
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.bEnableUnsolicited = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.bEnableResponsesonStartup = 1;   // enable or disable unsolicited response to master
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u32Timeout = 5000;               // unsolicited response timeout in ms

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u16Class1TriggerNumberofEvents = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u16Class1HoldTimeAfterResponse = 1;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u16Class2TriggerNumberofEvents = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u16Class2HoldTimeAfterResponse = 1;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u16Class3TriggerNumberofEvents = 1;
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u16Class3HoldTimeAfterResponse = 1;

            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u8Retries = 5;                   // Unsolicited message retries
            sDNP3Config.sDNP3ServerSet.sServerProtSet.sUnsolicitedResponseSet.u16MaxNumberofEvents = 1;

            // Debug option settings
            sDNP3Config.sDNP3ServerSet.sDebug.u32DebugOptions = (uint)(dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_TX | dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_RX);

            // Define number of objects
            sDNP3Config.sDNP3ServerSet.u16NoofObject = 4;

            // Allocate memory for objects
            psDNP3Objects = new dnp3_protocol.dnp3types.sDNP3Object[sDNP3Config.sDNP3ServerSet.u16NoofObject];
            sDNP3Config.sDNP3ServerSet.psDNP3Objects = System.Runtime.InteropServices.Marshal.AllocHGlobal(
                sDNP3Config.sDNP3ServerSet.u16NoofObject * System.Runtime.InteropServices.Marshal.SizeOf(psDNP3Objects[0]));

            GeneratePoints(ref psDNP3Objects, Tuple.Create<ushort, ushort, ushort, ushort>(5, 5, 5, 5));

            // Load configuration
            iErrorCode = dnp3_protocol.dnp3api.DNP3LoadConfiguration(DNP3serverhandle, ref sDNP3Config, ref ptErrorValue);
            if (iErrorCode != 0)
            {
                Trace.TraceError("DNP3 Load failed");
                Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));
                return false;
            }

            // Start server
            iErrorCode = dnp3_protocol.dnp3api.DNP3Start(DNP3serverhandle, ref ptErrorValue);
            if (iErrorCode != 0)
            {
                Trace.TraceError("DNP3 Start failed");
                Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));
                return false;
            }

            return true;
        }

        private void GeneratePoints(ref dnp3types.sDNP3Object[] psDNP3Objects, Tuple<ushort,ushort,ushort,ushort> tuple)
        {
            for (int i = 0; i < 4; ++i)
            {
                switch (i)
                {

                    case 0:
                        psDNP3Objects[0].ai8Name = "binary input 0-9";
                        psDNP3Objects[0].eGroupID = dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_INPUT;
                        psDNP3Objects[0].u16NoofPoints = tuple.Item1;
                        psDNP3Objects[0].eClassID = dnp3_protocol.dnp3types.eDNP3ClassID.CLASS_THREE;
                        psDNP3Objects[0].eControlModel = dnp3_protocol.dnp3types.eDNP3ControlModelConfig.INPUT_STATUS_ONLY;
                        psDNP3Objects[0].u32SBOTimeOut = 0;
                        psDNP3Objects[0].f32AnalogInputDeadband = 0;
                        psDNP3Objects[0].eAnalogStoreType = dnp3_protocol.dnp3types.eAnalogStorageType.AS_FLOAT;
                        break;

                    case 1:
                        psDNP3Objects[1].ai8Name = "binary output 0-9";
                        psDNP3Objects[1].eGroupID = dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT;
                        psDNP3Objects[1].u16NoofPoints = tuple.Item2;
                        psDNP3Objects[1].eClassID = dnp3_protocol.dnp3types.eDNP3ClassID.NO_CLASS;
                        psDNP3Objects[1].eControlModel = dnp3_protocol.dnp3types.eDNP3ControlModelConfig.DIRECT_OPERATION;
                        psDNP3Objects[1].u32SBOTimeOut = 0;
                        psDNP3Objects[1].f32AnalogInputDeadband = 0;
                        psDNP3Objects[1].eAnalogStoreType = dnp3_protocol.dnp3types.eAnalogStorageType.AS_INTEGER32;
                        break;

                    case 2:
                        psDNP3Objects[2].ai8Name = "analog input 0-9";
                        psDNP3Objects[2].eGroupID = dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT;
                        psDNP3Objects[2].u16NoofPoints = tuple.Item3;
                        psDNP3Objects[2].eClassID = dnp3_protocol.dnp3types.eDNP3ClassID.CLASS_ONE;
                        psDNP3Objects[2].eControlModel = dnp3_protocol.dnp3types.eDNP3ControlModelConfig.INPUT_STATUS_ONLY;
                        psDNP3Objects[2].u32SBOTimeOut = 0;
                        psDNP3Objects[2].f32AnalogInputDeadband = 0;
                        psDNP3Objects[2].eAnalogStoreType = dnp3_protocol.dnp3types.eAnalogStorageType.AS_FLOAT;
                        break;

                    case 3:
                        psDNP3Objects[3].ai8Name = "analog output 0-9";
                        psDNP3Objects[3].eGroupID = dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS;
                        psDNP3Objects[3].u16NoofPoints = tuple.Item4;
                        psDNP3Objects[3].eClassID = dnp3_protocol.dnp3types.eDNP3ClassID.NO_CLASS;
                        psDNP3Objects[3].eControlModel = dnp3_protocol.dnp3types.eDNP3ControlModelConfig.DIRECT_OPERATION;
                        psDNP3Objects[3].u32SBOTimeOut = 0;
                        psDNP3Objects[3].f32AnalogInputDeadband = 0;
                        psDNP3Objects[3].eAnalogStoreType = dnp3_protocol.dnp3types.eAnalogStorageType.AS_FLOAT;
                        break;
                }
                IntPtr tmp = new IntPtr(sDNP3Config.sDNP3ServerSet.psDNP3Objects.ToInt32() + i * System.Runtime.InteropServices.Marshal.SizeOf(psDNP3Objects[0]));
                System.Runtime.InteropServices.Marshal.StructureToPtr(psDNP3Objects[i], tmp, true);
            }
        }

        private static short cbDebug(ushort u16ObjectId, ref dnp3_protocol.dnp3types.sDNP3DebugData psDebugData, ref short ptErrorValue)
        {
            if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_RX) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_RX)
            {
                if (psDebugData.eCommMode == dnp3_protocol.dnp3types.eCommunicationMode.TCP_IP_MODE)
                {
                    Trace.TraceInformation("Rx IP " + psDebugData.ai8IPAddress + " Port " + psDebugData.u16PortNumber + " <- ");
                }
                for (ushort i = 0; i < psDebugData.u16RxCount; i++)
                    Trace.TraceInformation("{0:X2} ", psDebugData.au8RxData[i]);
            }

            if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_TX) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_TX)
            {
                if (psDebugData.eCommMode == dnp3_protocol.dnp3types.eCommunicationMode.TCP_IP_MODE)
                {
                    Trace.TraceInformation("Tx IP " + psDebugData.ai8IPAddress + " Port " + psDebugData.u16PortNumber + " -> ");
                }
                for (ushort i = 0; i < psDebugData.u16TxCount; i++)
                    Trace.TraceInformation("{0:X2} ", psDebugData.au8TxData[i]);

            }

            if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_ERROR) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_ERROR)
            {
                Trace.TraceError("Error message " + psDebugData.au8ErrorMessage);
                Trace.TraceError("ErrorCode " + psDebugData.i16ErrorCode);
                Trace.TraceError("ErrorValue " + psDebugData.tErrorValue);
            }

            if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_WARNING) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_WARNING)
            {
                Trace.TraceWarning("Warning message " + psDebugData.au8WarningMessage);
                Trace.TraceWarning("ErrorCode " + psDebugData.i16ErrorCode);
                Trace.TraceWarning("ErrorValue " + psDebugData.tErrorValue);
            }

            return (short)dnp3_protocol.tgterrorcodes.eTgtErrorCodes.EC_NONE;
        }

        private static short cbOperate(ushort u16ObjectId, ref dnp3_protocol.dnp3types.sDNP3DataAttributeID psOperateID, ref dnp3_protocol.dnp3types.sDNP3DataAttributeData psOperateValue, ref dnp3_protocol.dnp3types.sDNP3CommandParameters psOperateParams, ref short ptErrorValue)
        {
            if (psOperateID.eGroupID == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
            {
                Trace.TraceInformation("GROUP ID : ANALOG_OUTPUT");
                Trace.TraceInformation("Index Number " + psOperateID.u16IndexNumber);
                psOperateValue.eDataSize = dnp3_protocol.tgttypes.eDataSizes.FLOAT32_SIZE;
                psOperateValue.eDataType = dnp3_protocol.tgtcommon.eDataTypes.FLOAT32_DATA;
                iErrorCode = dnp3_protocol.dnp3api.DNP3Update(DNP3serverhandle, ref psOperateID, ref psOperateValue, 1, dnp3_protocol.dnp3types.eUpdateClassID.UPDATE_DEFAULT_EVENT, ref ptErrorValue);
                if (iErrorCode != 0)
                {
                    Trace.TraceError("dnp3 Library API Function - DNP3DirectOperate() failed: {0:D} {1:D}", iErrorCode, ptErrorValue);
                    Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                    Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));
                    return (short)dnp3_protocol.tgterrorcodes.eTgtErrorCodes.EC_NONE;
                }

                if (psOperateParams.eCommandVariation == dnp3_protocol.dnp3types.eCommandObjectVariation.ANALOG_OUTPUT_BLOCK_FLOAT32)
                {
                    Trace.TraceInformation("Variation : ANALOG_OUTPUT_BLOCK_FLOAT32");
                    SingleInt32Union f32Data;
                    f32Data.f = 0;
                    f32Data.i = System.Runtime.InteropServices.Marshal.ReadInt32(psOperateValue.pvData);
                    Trace.TraceInformation("Data : {0:F3}", f32Data.f);

                }
                else if (psOperateParams.eCommandVariation == dnp3_protocol.dnp3types.eCommandObjectVariation.ANALOG_OUTPUT_BLOCK_INTEGER32)
                {
                    Trace.TraceInformation("Variation : ANALOG_OUTPUT_BLOCK_INTEGER32");
                    Trace.TraceInformation("Data : {0:D}", System.Runtime.InteropServices.Marshal.ReadInt32(psOperateValue.pvData));

                }
                else if (psOperateParams.eCommandVariation == dnp3_protocol.dnp3types.eCommandObjectVariation.ANALOG_OUTPUT_BLOCK_INTEGER16)
                {
                    Trace.TraceInformation("Variation : ANALOG_OUTPUT_BLOCK_INTEGER16");
                    Trace.TraceInformation("Data : {0:D}", System.Runtime.InteropServices.Marshal.ReadInt16(psOperateValue.pvData));
                }
                else
                    Trace.TraceInformation("Invalid variation");
            }

            if (psOperateID.eGroupID == dnp3_protocol.dnp3types.eDNP3GroupID.BINARY_OUTPUT)
            {
                Trace.TraceInformation("GROUP ID : BINARY_OUTPUT");
                Trace.TraceInformation("Index Number " + psOperateID.u16IndexNumber);
                psOperateValue.eDataSize = dnp3_protocol.tgttypes.eDataSizes.SINGLE_POINT_SIZE;
                psOperateValue.eDataType = dnp3_protocol.tgtcommon.eDataTypes.SINGLE_POINT_DATA;
                iErrorCode = dnp3_protocol.dnp3api.DNP3Update(DNP3serverhandle, ref psOperateID, ref psOperateValue, 1, dnp3_protocol.dnp3types.eUpdateClassID.UPDATE_DEFAULT_EVENT, ref ptErrorValue);
                if (iErrorCode != 0)
                {
                    Trace.TraceError("dnp3 Library API Function - DNP3DirectOperate() failed: {0:D} {1:D}", iErrorCode, ptErrorValue);
                    Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                    Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));
                    return (short)dnp3_protocol.tgterrorcodes.eTgtErrorCodes.EC_NONE;
                }

                if (psOperateParams.eCommandVariation == dnp3_protocol.dnp3types.eCommandObjectVariation.CROB_G12V1)
                {
                    Trace.TraceInformation("Variation : CROB_G12V1");
                }
                else
                    Trace.TraceInformation("Invalid variation");

                Trace.TraceInformation("Operation Type " + psOperateParams.eOPType);
                Trace.TraceInformation("Count " + psOperateParams.u8Count);
                Trace.TraceInformation("On time " + psOperateParams.u32ONtime);
                Trace.TraceInformation("Off time " + psOperateParams.u32OFFtime);
                Trace.TraceInformation("CR {0}", psOperateParams.bCR);
            }
            Trace.TraceInformation("Date : {0:D}-{1:D}-{2:D}", psOperateValue.sTimeStamp.u8Day, psOperateValue.sTimeStamp.u8Month, psOperateValue.sTimeStamp.u16Year);
            Trace.TraceInformation("Time : {0:D}:{1:D2}:{2:D2}:{3:D3}", psOperateValue.sTimeStamp.u8Hour, psOperateValue.sTimeStamp.u8Minute, psOperateValue.sTimeStamp.u8Seconds, psOperateValue.sTimeStamp.u16MilliSeconds);
            return (short)dnp3_protocol.tgterrorcodes.eTgtErrorCodes.EC_NONE;
        }


        #region Error Handlers
        static string errorcodestring(short errorcode)
        {
            dnp3_protocol.dnp3types.sDNP3ErrorCode sDNP3ErrorCodeDes;
            sDNP3ErrorCodeDes = new dnp3_protocol.dnp3types.sDNP3ErrorCode();

            sDNP3ErrorCodeDes.iErrorCode = errorcode;

            dnp3_protocol.dnp3api.DNP3ErrorCodeString(ref sDNP3ErrorCodeDes);

            string returnmessage = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(sDNP3ErrorCodeDes.LongDes);

            return returnmessage;
        }


        static string errorvaluestring(short errorvalue)
        {
            dnp3_protocol.dnp3types.sDNP3ErrorValue sDNP3ErrorValueDes;
            sDNP3ErrorValueDes = new dnp3_protocol.dnp3types.sDNP3ErrorValue();

            sDNP3ErrorValueDes.iErrorValue = errorvalue;

            dnp3_protocol.dnp3api.DNP3ErrorValueString(ref sDNP3ErrorValueDes);

            string returnmessage = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(sDNP3ErrorValueDes.LongDes);

            return returnmessage;
        }
        #endregion

        #region Utils
        public static void MarshalUnmananagedArray2Struct(IntPtr unmanagedArray, int length, out dnp3_protocol.dnp3types.sServerDatabasePoint[] mangagedArray)
        {
            var size = Marshal.SizeOf(typeof(dnp3_protocol.dnp3types.sServerDatabasePoint));
            mangagedArray = new dnp3_protocol.dnp3types.sServerDatabasePoint[length];

            for (int i = 0; i < length; i++)
            {
                IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
                mangagedArray[i] = Marshal.PtrToStructure<dnp3_protocol.dnp3types.sServerDatabasePoint>(ins);
            }
        }

        private List<Point> ConvertToPoints(dnp3_protocol.dnp3types.sServerDatabasePoint[] mangagedArray)
        {
            var list = new List<Point>();
            foreach (var item in mangagedArray)
            {
                if (item.eGroupID == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_INPUT || item.eGroupID == dnp3_protocol.dnp3types.eDNP3GroupID.ANALOG_OUTPUTS)
                {
                    var point = new AnalogPoint()
                    {
                        GroupId = item.eGroupID,
                        Index = item.u16IndexNumber,
                        TimeStamp = new DateTime(item.sTimeStamp.u16Year, item.sTimeStamp.u8Month, item.sTimeStamp.u8Day, item.sTimeStamp.u8Hour, item.sTimeStamp.u8Minute, item.sTimeStamp.u8Seconds).ToString()
                    };
                    SingleInt32Union value = new SingleInt32Union(Marshal.ReadInt32(item.pvData,0));
                    point.Value = CheckAnalogValueExp(value.f, point.GroupId, point.Index);
                    list.Add(point);
                }
                else
                {
                    var point = new BinaryPoint()
                    {
                        GroupId = item.eGroupID,
                        Index = item.u16IndexNumber,
                        TimeStamp = new DateTime(item.sTimeStamp.u16Year, item.sTimeStamp.u8Month, item.sTimeStamp.u8Day, item.sTimeStamp.u8Hour, item.sTimeStamp.u8Minute, item.sTimeStamp.u8Seconds).ToString()
                    };
                    SingleInt32Union value = new SingleInt32Union(Marshal.ReadInt32(item.pvData,0));
                    point.Value = CheckBinaryValueExp(value.i, point.GroupId, point.Index); ;
                    list.Add(point);
                }

            }
            return list;
        }

        private float CheckAnalogValueExp(float value, dnp3types.eDNP3GroupID group, ushort index)
        {
            if(value > 10000000)
            {
                if (prevList != null && prevList.Count > 0)
                    return (prevList.Single(x => x.GroupId == group && x.Index == index) as AnalogPoint).Value;
            }
            return value;
        }

        private int CheckBinaryValueExp(int value, dnp3types.eDNP3GroupID group, ushort index)
        {
            if (value > 10000000)
            {
                if (prevList != null && prevList.Count > 0)
                    return (prevList.Single(x => x.GroupId == group && x.Index == index) as BinaryPoint).Value;
            }
            return value;
        }

        private void Update(ushort index, dnp3_protocol.dnp3types.eDNP3GroupID group, dnp3_protocol.tgttypes.eDataSizes dataSize, dnp3_protocol.tgtcommon.eDataTypes dataType, SingleInt32Union value, ref short ptErrorValue)
        {
            ushort uiCount = 1;
            dnp3_protocol.dnp3types.sDNP3DataAttributeID[] psDAID = new dnp3_protocol.dnp3types.sDNP3DataAttributeID[uiCount];
            dnp3_protocol.dnp3types.sDNP3DataAttributeData[] psNewValue = new dnp3_protocol.dnp3types.sDNP3DataAttributeData[uiCount];
            psDAID[0].u16SlaveAddress = 1;
            psDAID[0].eGroupID = group;
            psDAID[0].u16IndexNumber = index;

            psNewValue[0].eDataSize = dataSize;
            psNewValue[0].eDataType = dataType;

            psDAID[0].pvUserData = IntPtr.Zero;
            psNewValue[0].tQuality = (ushort)(dnp3_protocol.dnp3types.eDNP3QualityFlags.ONLINE);
            psNewValue[0].pvData = System.Runtime.InteropServices.Marshal.AllocHGlobal((int)dataSize);

            DateTime date = DateTime.Now;
            psNewValue[0].sTimeStamp.u8Day = (byte)date.Day;
            psNewValue[0].sTimeStamp.u8Month = (byte)date.Month;
            psNewValue[0].sTimeStamp.u16Year = (ushort)date.Year;
            psNewValue[0].sTimeStamp.u8Hour = (byte)date.Hour;
            psNewValue[0].sTimeStamp.u8Minute = (byte)date.Minute;
            psNewValue[0].sTimeStamp.u8Seconds = (byte)date.Second;
            psNewValue[0].sTimeStamp.u16MilliSeconds = (ushort)date.Millisecond;
            psNewValue[0].sTimeStamp.u16MicroSeconds = 0;
            psNewValue[0].sTimeStamp.i8DSTTime = 0;
            psNewValue[0].sTimeStamp.u8DayoftheWeek = (byte)date.DayOfWeek;

            Marshal.WriteInt32(psNewValue[0].pvData, value.i);
            iErrorCode = dnp3_protocol.dnp3api.DNP3Update(DNP3serverhandle, ref psDAID[0], ref psNewValue[0], uiCount, dnp3_protocol.dnp3types.eUpdateClassID.UPDATE_DEFAULT_EVENT, ref ptErrorValue);
            if (iErrorCode != 0)
            {
                Trace.TraceError("dnp3 Library API Function - DNP3Update() failed: {0:D} {1:D}", iErrorCode, ptErrorValue);
                Trace.TraceError("iErrorCode {0:D}: {1}", iErrorCode, errorcodestring(iErrorCode));
                Trace.TraceError("iErrorValue {0:D}: {1}", ptErrorValue, errorvaluestring(ptErrorValue));

            }
        }
        #endregion

        public void UpdatePoint(ushort index, dnp3types.eDNP3GroupID group, tgttypes.eDataSizes dataSize, tgtcommon.eDataTypes dataType, SingleInt32Union value)
        {
            Update(index, group, dataSize, dataType, value, ref ptErrorValue);
        }

        public void Dispose()
        {
            worker.Abort();
        }

        public static void UpdateConfig(Tuple<ushort, ushort, ushort, ushort> points, Dictionary<string,ushort> pairs)
        {
            configChange = true;
            config = points;
            incomingPairs = pairs;
        }
    }
}
