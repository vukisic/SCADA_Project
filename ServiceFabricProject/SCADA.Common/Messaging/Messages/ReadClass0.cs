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

            DNP3ReadClass0CommandParameters commandParam = (DNP3ReadClass0CommandParameters)CommandParameters;

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

        public override Dictionary<Tuple<RegisterType, int>, BasePoint> PareseResponse(byte[] response)
        {
            if (!CrcCalculator.CheckCRC(response))
                return null;

            byte[] dataObjects = MessagesHelper.GetResponseDataObjects(response);

            Dictionary<Tuple<RegisterType, int>, BasePoint> retVal = new Dictionary<Tuple<RegisterType, int>, BasePoint>();
            try
            {
                int typeFieldPosition = 0;
                int startIndexPosition = 3;
                int stopIndexPosition = 4;

                int len = dataObjects.Length;
                int lastRange = 0;
                int pointTypes = 0;
                while (len > 0)
                {
                    ushort typeField = (ushort)IPAddress.NetworkToHostOrder((short)BitConverter.ToUInt16(dataObjects, typeFieldPosition));
                    byte startIndex = dataObjects[startIndexPosition];
                    byte stopIndex = dataObjects[stopIndexPosition];
                    int numberOfItems = stopIndex - startIndex + 1;
                    int range = 5; //type(2) + qual + satrt + stop index

                    switch (typeField)
                    {
                        case (ushort)TypeField.BINARY_INPUT_PACKED_FORMAT:
                        case (ushort)TypeField.BINARY_OUTPUT_PACKED_FORMAT:
                            {
                                pointTypes += 1;
                                range += numberOfItems % 8 == 0 ? numberOfItems / 8 : numberOfItems / 8 + 1;
                                byte[] binaryObject = new byte[range];
                                Buffer.BlockCopy(dataObjects, lastRange, binaryObject, 0, range);
                                ParseBinaryObject(binaryObject, typeField, ref retVal);
                                lastRange += range;
                                break;
                            }
                        case (ushort)TypeField.ANALOG_OUTPUT_STATUS_16BIT:
                            {
                                pointTypes += 1;
                                range += numberOfItems * 3;
                                byte[] analogObject = new byte[range];
                                Buffer.BlockCopy(dataObjects, lastRange, analogObject, 0, range);
                                ParseAnalogOutputObject(analogObject, typeField, ref retVal);
                                lastRange += range;
                                break;
                            }
                        case (ushort)TypeField.ANALOG_INPUT_16BIT:
                            {
                                pointTypes += 1;
                                range += numberOfItems * 2;
                                byte[] analogObject = new byte[range];
                                Buffer.BlockCopy(dataObjects, lastRange, analogObject, 0, range);
                                ParseAnalogInputObject(analogObject, typeField, ref retVal);
                                lastRange += range;
                                break;
                            }
                    }
                    len -= range;
                    typeFieldPosition += range;
                    startIndexPosition += range;
                    stopIndexPosition += range;
                    if (pointTypes == 4)
                        break;
                }
            }
            catch { }
           
            return retVal;
        }

        private void ParseAnalogOutputObject(byte[] analogInputObject, ushort typeField, ref Dictionary<Tuple<RegisterType, int>, BasePoint> points)
        {
            RegisterType registerType =  RegisterType.ANALOG_OUTPUT;

            int currentIndex = analogInputObject[3];
            int numberOfitems = analogInputObject[4] - analogInputObject[3] + 1;
            for(int i = 0; i < numberOfitems; i++)
            {
                AnalogPoint point = new AnalogPoint();
                point.Value = (ushort)BitConverter.ToUInt16(analogInputObject, (5 + i * 3 + 1)); //na 5 je prva vrednost, i * 3 po tri bajta i izdvajamo zadnja dva (+ 1)
                point.Index = currentIndex;
                point.RegisterType = registerType;
                points.Add(new Tuple<RegisterType, int>(registerType, currentIndex++), point);
            }
        }
        
        private void ParseAnalogInputObject(byte[] analogInputObject, ushort typeField, ref Dictionary<Tuple<RegisterType, int>, BasePoint> points)
        {
            RegisterType registerType = RegisterType.ANALOG_INPUT ;

            int currentIndex = analogInputObject[3];
            int numberOfitems = analogInputObject[4] - analogInputObject[3] + 1;
            for(int i = 0; i < numberOfitems; i++)
            {
                AnalogPoint point = new AnalogPoint();
                point.Value = (ushort)BitConverter.ToUInt16(analogInputObject, (5 + i * 2)); //na 5 je prva vrednost, i * 2 idemo short po short
                point.Index = currentIndex;
                point.RegisterType = registerType;
                points.Add(new Tuple<RegisterType, int>(registerType, currentIndex++), point);
            }
        }

        private void ParseBinaryObject(byte[] binaryObject, ushort typeField, ref Dictionary<Tuple<RegisterType, int>, BasePoint> points)
        {
            RegisterType registerType = typeField == (ushort)TypeField.BINARY_INPUT_PACKED_FORMAT ? RegisterType.BINARY_INPUT : RegisterType.BINARY_OUTPUT;

            int currentIndex = binaryObject[3];
            for (int i = 5; i < binaryObject.Length ; i++)
            {
                byte currentByte = binaryObject[i];
                byte mask = 0x01;
                for (int j = 0; j < 8; j++)
                {
                    DiscretePoint point = new DiscretePoint();
                    point.Value = currentByte & mask;
                    point.Index = currentIndex;
                    point.RegisterType = registerType;
                    currentByte >>= 1;
                    points.Add(new Tuple<RegisterType, int>(registerType, currentIndex++), point);
                    if (currentIndex > binaryObject[4])
                        break;
                }
            }
        } 
    }
}
