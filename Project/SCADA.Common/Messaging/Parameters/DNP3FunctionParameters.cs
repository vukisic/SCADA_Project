using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public abstract class DNP3FunctionParameters
    {
        private ushort start = 0x0564;
        private byte length;
        private byte control = 0x04;
        private ushort destination;
        private ushort source;
        private ushort crc;

        private byte transportHeader;

        public DNP3FunctionParameters(ushort start, byte length, byte control, ushort destination, ushort source, byte transportHeader)
        {
            Start = start;
            Length = length;
            Control = control;
            Destination = destination;
            Source = source;
            TransportHeader = transportHeader;
        }

        #region properties
        public ushort Start
        {
            get
            {
                return start;
            }
            set
            {
                start = value;
            }
        }

        public byte Length
        {
            get
            {
                return length;
            }
            set
            {
                length = value;
            }
        }

        public byte Control
        {
            get
            {
                return control;
            }
            set
            {
                control = value;
            }
        }

        public ushort Destination
        {
            get
            {
                return destination;
            }
            set
            {
                destination = value;
            }
        }

        public ushort Source
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        public ushort Crc
        {
            get
            {
                return crc;
            }
            set
            {
                crc = value;
            }
        }

        public byte TransportHeader
        {
            get
            {
                return transportHeader;
            }
            set
            {
                transportHeader = value;
            }
        }
        #endregion
    }
}
