using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging.Parameters;

namespace SCADA.Common.Messaging.Messages
{
    public class ReadClass0 : DNP3Function
    {
        private MessageHeaderBuilder headerBuilder;

        public ReadClass0(DNP3CommandParameters commandParameters) : base(commandParameters)
        {
            headerBuilder = new MessageHeaderBuilder();
        }
        public override byte[] PackRequest()
        {
            byte[] request = new byte[18];

            DNP3ReadCommandParameters commandParam = (DNP3ReadCommandParameters)CommandParameters;

            CommandParameters.Length = 0x0b;  
            Buffer.BlockCopy(headerBuilder.Build(CommandParameters), 0, request, 0, 10);
            request[10] = commandParam.TransportControl;
            request[11] = commandParam.AplicationControl;
            request[12] = commandParam.FunctionCode;
            Buffer.BlockCopy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)commandParam.ObjectTypeField)), 0, request, 13, 2);
            request[15] = commandParam.Qualifier;
            ushort crc = 0;
            for (int i = 10; i < 16; i++)
            {
                CrcCalculator.computeCRC(request[i], ref crc);
            }
            crc = (ushort)(~crc);
            Buffer.BlockCopy(BitConverter.GetBytes(crc), 0, request, 16, 2);

            return request;
        }

        public override Dictionary<Tuple<RegisterType, ushort>, BasePoint> PareseResponse(byte[] response)
        {
            if (!CrcCalculator.CheckCRC(response))
                return null;

            byte[] dataObjects = MessagesHelper.GetResponseDataObjects(response);

            Dictionary<Tuple<RegisterType, ushort>, BasePoint> retVal = new Dictionary<Tuple<RegisterType, ushort>, BasePoint>();

            ushort typeField = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(response, 0));
            byte startIndex = dataObjects[3];
            byte stopIndex = dataObjects[4];
            int numberOfItems = stopIndex - startIndex + 1;
            switch (typeField)
            {
                case (ushort)TypeField.BINARY_INPUT_PACKED_FORMAT:
                case (ushort)TypeField.BINATY_OUTPUT_WITH_STATUS:
                    {
                        int byteCount = numberOfItems % 8 == 0 ? numberOfItems / 8 : numberOfItems / 8 + 1;
                        byteCount += 5; //type(2) + qual + satrt + stop index
                        byte[] binaryObject = new byte[byteCount];
                        Buffer.BlockCopy(response, 0, binaryObject, 0, byteCount);
                        ParseBinaryObject(binaryObject, typeField, ref retVal);
                        break;
                    }
                case (ushort)TypeField.ANALOG_INPUT_16BIT:
                    {
                        int shortCount = numberOfItems * 2;
                        shortCount += 5;
                        byte[] analogInputObject = new byte[shortCount];
                        Buffer.BlockCopy(response, 0, analogInputObject, 0, shortCount);
                        ParseAnalogInputObject(analogInputObject, ref retVal);
                        break; 
                    }
                case (ushort)TypeField.ANALOG_OUTPUT_16BIT:
                    {

                        break;
                    }
            }
            return retVal;
        }

        private void ParseAnalogInputObject(byte[] analogInputObject, ref Dictionary<Tuple<RegisterType, ushort>, BasePoint> points)
        {
            throw new NotImplementedException();
        }

        private void ParseBinaryObject(byte[] binaryObject, ushort typeField, ref Dictionary<Tuple<RegisterType, ushort>, BasePoint> points)
        {
            RegisterType registerType = typeField == (ushort)TypeField.BINARY_INPUT_PACKED_FORMAT ? RegisterType.BINARY_INPUT : RegisterType.BINARY_OUTPUT;

            ushort currentIndex = binaryObject[3];
            for (int i = 5; i < binaryObject.Length ; i++)
            {
                byte currentByte = binaryObject[i];
                byte mask = 0x01;
                for (int j = 0; j < 8; j++)
                {
                    DiscretePoint point = new DiscretePoint();
                    point.Value = currentByte & mask;
                    currentByte >>= 1;
                    points.Add(new Tuple<RegisterType, ushort>(registerType, currentIndex++), point);
                    if (currentIndex > binaryObject[4])
                        break;
                }
            }
        } 
    }
}
