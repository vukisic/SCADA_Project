using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NDS.ProcessingModule
{
    public interface IConnection
    {
        void Connect();
        void Disconect();
        byte[] Recv(int numberOfBytes);
        void Send(byte[] message);
        bool CheckState();
    }
}
