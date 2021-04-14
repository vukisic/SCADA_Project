using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NDS.ProcessingModule
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

        public bool CheckState()
        {
            return this.socket.Poll(30000, SelectMode.SelectWrite);
        }

        public void Connect()
        {
            this.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Blocking = false;
            try
            {
                socket.Connect(remoteEndpoint);
            }
            catch { }
        }

        public void Disconect()
        {
            if (socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
            }

            socket.Close();
            socket = null;
        }

        public byte[] Recv(int numberOfBytes)
        {
            int numberOfReceivedBytes = 0;
            byte[] retval = new byte[numberOfBytes];
            int numOfReceived;
            while (numberOfReceivedBytes < numberOfBytes)
            {
                numOfReceived = 0;
                if (socket.Poll(1623, SelectMode.SelectRead))
                {
                    numOfReceived = socket.Receive(retval, numberOfReceivedBytes, (int)numberOfBytes - numberOfReceivedBytes, SocketFlags.None);
                    if (numOfReceived > 0)
                    {
                        numberOfReceivedBytes += numOfReceived;
                    }
                }
            }
            return retval;
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
