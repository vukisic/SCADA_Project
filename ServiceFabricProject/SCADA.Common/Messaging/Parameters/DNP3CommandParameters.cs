using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Messaging.Parameters
{
    public abstract class DNP3CommandParameters
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

        public DNP3CommandParameters(byte transportControl)
        {
            Start = 0x0564;
            Length = 0;
            Control = 0xc4;
            Destination = UInt16.Parse(ConfigurationManager.AppSettings["Destination"]);
            Source = UInt16.Parse(ConfigurationManager.AppSettings["Source"]);
            TransportControl = transportControl;
        }

        #region properties
        public ushort Start { get => start; set => start = value; }
        public byte Length { get => length; set => length = value; }
        public byte Control { get => control; set => control = value; }
        public ushort Destination { get => destination; set => destination = value; }
        public ushort Source { get => source; set => source = value; }
        public ushort Crc { get => crc; set => crc = value; }
        public byte TransportControl { get => transportControl; set => transportControl = value; }
        #endregion
    }
}
