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
using System.Web.UI;

namespace Simulator.Core
{

    public class Simulator : ISimulator, IDisposable
    {
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
        private dnp3_protocol.dnp3types.sDNPServerDatabase db;
        private static  Dictionary<string, ushort> pairs;
        private static  Dictionary<string, ushort> incomingPairs;
        private ISimulatorConfiguration simulatorConfiguration;
        private SimulationLogic simLogic;
        public int interval { get; set; }

        public Simulator(ISimulatorConfiguration simulatorConfiguration)
        {
            prevList = new List<Point>();
            operateCallback = new dnp3_protocol.dnp3types.DNP3ControlOperateCallback(cbOperate);
            debugCallback = new dnp3_protocol.dnp3types.DNP3DebugMessageCallback(cbDebug);
            interval = Int32.Parse(ConfigurationManager.AppSettings["interval"]);
            db = new dnp3_protocol.dnp3types.sDNPServerDatabase();
            this.simulatorConfiguration = simulatorConfiguration;
            simLogic = new SimulationLogic(this);

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
           
            if (Prepare())
            {
                while (executionFlag)
                {
                    try
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
                    catch (Exception)
                    {

                        throw;
                    }
                   
                }
            }
           
        }
        
        private void Simulation()
        {
            dnp3_protocol.dnp3api.DNP3GetServerDatabaseValue(DNP3serverhandle, ref db, ref ptErrorValue);
            simLogic.Simulate(db , pairs); 
        }

        private bool Prepare()
        {
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

            // Configure
            sDNP3Config = simulatorConfiguration.Configure(ConfigurationManager.AppSettings["address"] ?? "127.0.0.1", ushort.Parse(ConfigurationManager.AppSettings["port"]));

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
            //if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_RX) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_RX)
            //{
            //    if (psDebugData.eCommMode == dnp3_protocol.dnp3types.eCommunicationMode.TCP_IP_MODE)
            //    {
            //        Trace.TraceInformation("Rx IP " + psDebugData.ai8IPAddress + " Port " + psDebugData.u16PortNumber + " <- ");
            //    }
            //    for (ushort i = 0; i < psDebugData.u16RxCount; i++)
            //        Trace.TraceInformation("{0:X2} ", psDebugData.au8RxData[i]);
            //}

            //if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_TX) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_TX)
            //{
            //    if (psDebugData.eCommMode == dnp3_protocol.dnp3types.eCommunicationMode.TCP_IP_MODE)
            //    {
            //        Trace.TraceInformation("Tx IP " + psDebugData.ai8IPAddress + " Port " + psDebugData.u16PortNumber + " -> ");
            //    }
            //    for (ushort i = 0; i < psDebugData.u16TxCount; i++)
            //        Trace.TraceInformation("{0:X2} ", psDebugData.au8TxData[i]);

            //}

            //if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_ERROR) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_ERROR)
            //{
            //    Trace.TraceError("Error message " + psDebugData.au8ErrorMessage);
            //    Trace.TraceError("ErrorCode " + psDebugData.i16ErrorCode);
            //    Trace.TraceError("ErrorValue " + psDebugData.tErrorValue);
            //}

            //if ((psDebugData.u32DebugOptions & (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_WARNING) == (uint)dnp3_protocol.tgtcommon.eDebugOptionsFlag.DEBUG_OPTION_WARNING)
            //{
            //    Trace.TraceWarning("Warning message " + psDebugData.au8WarningMessage);
            //    Trace.TraceWarning("ErrorCode " + psDebugData.i16ErrorCode);
            //    Trace.TraceWarning("ErrorValue " + psDebugData.tErrorValue);
            //}

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
                iErrorCode = dnp3_protocol.dnp3api.DNP3Update(DNP3serverhandle, ref psOperateID, ref psOperateValue, 1, dnp3_protocol.dnp3types.eUpdateClassID.UPDATE_NO_CLASS, ref ptErrorValue);
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
                    SingleInt32Union digitalValue = new SingleInt32Union();
                    digitalValue.f = System.Runtime.InteropServices.Marshal.ReadInt16(psOperateValue.pvData);
                    UpdateMP(psOperateID.u16IndexNumber, dnp3types.eDNP3GroupID.ANALOG_OUTPUTS, tgttypes.eDataSizes.FLOAT32_SIZE, tgtcommon.eDataTypes.FLOAT32_DATA, digitalValue, ref ptErrorValue);

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
                SingleInt32Union digitalValue = new SingleInt32Union();
                digitalValue.i = System.Runtime.InteropServices.Marshal.ReadInt16(psOperateValue.pvData) > 1?0:1;
                UpdateMP(psOperateID.u16IndexNumber, dnp3types.eDNP3GroupID.BINARY_OUTPUT, tgttypes.eDataSizes.SINGLE_POINT_SIZE, tgtcommon.eDataTypes.SINGLE_POINT_DATA, digitalValue, ref ptErrorValue);
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
        public void MarshalUnmananagedArray2Struct(IntPtr unmanagedArray, int length, out dnp3_protocol.dnp3types.sServerDatabasePoint[] mangagedArray)
        {
            var size = Marshal.SizeOf(typeof(dnp3_protocol.dnp3types.sServerDatabasePoint));
            mangagedArray = new dnp3_protocol.dnp3types.sServerDatabasePoint[length];

            for (int i = 0; i < length; i++)
            {
                IntPtr ins = new IntPtr(unmanagedArray.ToInt64() + i * size);
                mangagedArray[i] = Marshal.PtrToStructure<dnp3_protocol.dnp3types.sServerDatabasePoint>(ins);
            }
        }

        public List<Point> ConvertToPoints(dnp3_protocol.dnp3types.sServerDatabasePoint[] mangagedArray)
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
                {
                    var single = prevList.SingleOrDefault(x => x.GroupId == group && x.Index == index) as AnalogPoint;
                    return single != null? single.Value : 0;
                }
                    
            }
            return value;
        }

        private int CheckBinaryValueExp(int value, dnp3types.eDNP3GroupID group, ushort index)
        {
            if (value != 0 && value != 1)
            {
                if (prevList != null && prevList.Count > 0)
                {
                    var single = prevList.SingleOrDefault(x => x.GroupId == group && x.Index == index) as BinaryPoint;
                    return single != null? single.Value: 0;
                }
                    
            }
            return value;
        }

        private static void UpdateMP(ushort index, dnp3_protocol.dnp3types.eDNP3GroupID group, dnp3_protocol.tgttypes.eDataSizes dataSize, dnp3_protocol.tgtcommon.eDataTypes dataType, SingleInt32Union value, ref short ptErrorValue)
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
            Console.WriteLine("Config Change!");
        }
    }
}
