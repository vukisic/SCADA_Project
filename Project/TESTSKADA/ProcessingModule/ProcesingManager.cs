using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Connection;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging;
using SCADA.Common.Messaging.Parameters;
using SCADA.Common.Proxies;

namespace NDS.ProcessingModule
{
    public class ProcessingManager : IProcessingManager
    {
        private IFunctionExecutor functionExecutor;
        private byte applicationSequence;
        private byte transportSequence;
        private DOMProxy dom;
        private LoggingProxy log;
        private ScadaStorageProxy storage;
        public ProcessingManager(IFunctionExecutor functionExecutor)
        {
            this.functionExecutor = functionExecutor;
            applicationSequence = 0;
            transportSequence = 0;
            dom = ScadaProxyFactory.Instance().DOMProxy();
            log = ScadaProxyFactory.Instance().LoggingProxy();
            storage = ScadaProxyFactory.Instance().ScadaStorageProxy();
        }

        public void ExecuteReadCommand(RegisterType type, uint index)
        {
            log.Log(new SCADA.Common.Logging.LogEventModel() { EventType = SCADA.Common.Logging.LogEventType.INFO, Message = $"ReadCommand ({type},{index})" });
            DNP3ReadCommandParameters dnp3CommandParam = new DNP3ReadCommandParameters(GetApplicationSequence(), (byte)DNP3FunctionCode.READ, GetTypeField(type),
                (byte)Qualifier.PREFIX_2_OCTET_COUNT_OF_OBJECTS_2_OCTET, index, GetTransportSequence());
            IDNP3Function dnp3Fn = DNP3FunctionFactory.CreateReadFunction(dnp3CommandParam);
            this.functionExecutor.EnqueueCommand(dnp3Fn);
        }

        public void ExecuteWriteCommand(RegisterType type, uint index, uint value)
        {
            DNP3WriteCommandParameters dnp3CommandParam = new DNP3WriteCommandParameters(GetApplicationSequence(), (byte)DNP3FunctionCode.DIRECT_OPERATE, GetTypeField(type, true),
                (byte)Qualifier.PREFIX_2_OCTET_COUNT_OF_OBJECTS_2_OCTET, 1, index, value, GetTransportSequence());
            IDNP3Function dnp3Fn = DNP3FunctionFactory.CreateWriteFunction(dnp3CommandParam);
            this.functionExecutor.EnqueueCommand(dnp3Fn);

            var point = storage.GetSingle(type, (int)index);
            if (point != null && point.RegisterType == RegisterType.BINARY_OUTPUT)
                dom.AddOrUpdate(new SCADA.Common.Models.DomDbModel() { Mrid = point.Mrid, TimeStamp = DateTime.Now.ToString() });
        }

        public void ExecuteReadClass0Command()
        {
            DNP3ReadClass0CommandParameters dnp3CommandParam = new DNP3ReadClass0CommandParameters(GetApplicationSequence(), (byte)DNP3FunctionCode.READ, (ushort)TypeField.CLASS_0_DATA,
                (byte)Qualifier.PREFIX_NONE_NO_RANGE_FIELD, GetTransportSequence());
            IDNP3Function dnp3Fn = DNP3FunctionFactory.CreateReadClass0Function(dnp3CommandParam);
            this.functionExecutor.EnqueueCommand(dnp3Fn);
        }

        private ushort GetTypeField(RegisterType type, bool write=false)
        {
            if (!write)
            {
                switch (type)
                {
                    case RegisterType.ANALOG_INPUT: return (ushort)TypeField.ANALOG_INPUT_16BIT;
                    case RegisterType.ANALOG_OUTPUT: return (ushort)TypeField.ANALOG_OUTPUT_16BIT;
                    case RegisterType.BINARY_INPUT: return (ushort)TypeField.BINARY_INPUT_WITH_STATUS;
                    case RegisterType.BINARY_OUTPUT: return (ushort)TypeField.BINATY_OUTPUT_WITH_STATUS;
                    default: return 0;
                }
            }
            else
            {
                switch (type)
                {
                    case RegisterType.ANALOG_INPUT: return (ushort)TypeField.ANALOG_INPUT_16BIT;
                    case RegisterType.ANALOG_OUTPUT: return (ushort)TypeField.ANALOG_OUTPUT_16BIT;
                    case RegisterType.BINARY_INPUT: return (ushort)TypeField.BINARY_INPUT_WITH_STATUS;
                    case RegisterType.BINARY_OUTPUT: return (ushort)TypeField.BINARY_COMMAND;
                    default: return 0;
                }
            }
           
        }

        private byte GetApplicationSequence()
        {
            byte sq = (byte)(0xc0 | applicationSequence);
            applicationSequence = (byte)((applicationSequence % 16) + 1);
            return sq;
        }

        private byte GetTransportSequence()
        {
            byte sq = (byte)(0xc0 | transportSequence);
            transportSequence = (byte)((transportSequence % 64) + 1);
            return sq;
        }
    }
}
