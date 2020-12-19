using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public abstract class DNP3FunctionParameters
    {
        /// <summary>
        /// Data Link Layer
        /// </summary>
        private ushort start;
        private byte length;
        private byte control;
        private ushort destination;
        private ushort source;
        private ushort crc;
        /// <summary>
        /// Transport Function
        /// </summary>
        private byte transportControl;

        public DNP3FunctionParameters(byte length, ushort destination, ushort source, byte transportControl)
        {
            Start = 0x6405;
            Length = length;
            Control = 0x04;
            Destination = destination;
            Source = source;
            TransportControl = transportControl;
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

        public byte TransportControl
        {
            get
            {
                return transportControl;
            }
            set
            {
                transportControl = value;
            }
        }
        #endregion
    }
}
