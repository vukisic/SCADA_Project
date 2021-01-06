using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnp3_protocol;

namespace Simulator.Core
{
    public class SimulatorConfiguration : ISimulatorConfiguration
    {
        public dnp3types.sDNP3ConfigurationParameters Configure(string address, ushort port)
        {
            // Server load configuration - communication and protocol configuration parameters
            dnp3types.sDNP3ConfigurationParameters  sDNP3Config = new dnp3types.sDNP3ConfigurationParameters();
            // tcp communication settings
            sDNP3Config.sDNP3ServerSet.sServerCommunicationSet.eCommMode = dnp3_protocol.dnp3types.eCommunicationMode.TCP_IP_MODE;
            sDNP3Config.sDNP3ServerSet.sServerCommunicationSet.sEthernetCommsSet.sEthernetportSet.ai8FromIPAddress = address;
            sDNP3Config.sDNP3ServerSet.sServerCommunicationSet.sEthernetCommsSet.sEthernetportSet.u16PortNumber = port;

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

            DateTime date = DateTime.Now;

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

            return sDNP3Config;
        }
    }
}
