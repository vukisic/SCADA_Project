using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Common.Connection
{
    public class TCPConnection : IConnection
    {
        private Socket socket;
        private IPEndPoint remoteEndpoint;

        public TCPConnection()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Blocking = false;
            remoteEndpoint = CreateRemoteEndpoint();

        }
        public void Connect()
        {
            try
            {
                if (socket != null && socket.Connected)
                {
                    socket.Disconnect(false);
                    socket = null;
                    
                }
                this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Blocking = false;
                remoteEndpoint = CreateRemoteEndpoint();
                socket.Connect(remoteEndpoint);
            }
            catch { }
        }

        public void Disconect()
        {
            throw new NotImplementedException();
        }

        public byte[] Recv(int numberOfBytes)
        {
            throw new NotImplementedException();
        }

        public void Send(byte[] message)
        {
            int currentlySent = 0;

            while (currentlySent < message.Count())
            {
                if (socket.Poll(1623, SelectMode.SelectWrite))
                {
                    currentlySent += socket.Send(message, currentlySent, message.Length - currentlySent, SocketFlags.None);
                }
            }
        }

        private IPEndPoint CreateRemoteEndpoint()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry("localhost");
            IPAddress ipAddress = null;
            foreach (IPAddress ip in ipHostInfo.AddressList)
                if ("127.0.0.1".Equals(ip.ToString()))
                    ipAddress = ip;
            return new IPEndPoint(ipAddress, 20000);
        }
    }
}
