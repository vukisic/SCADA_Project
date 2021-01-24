using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging;

namespace SCADA.Common.Connection
{
    public class FunctionExecutor : IDisposable, IFunctionExecutor
    {
        private IConnection connection;
        private IDNP3Function currentCommand;
        private Thread connectionProcessorThread;
        private ConnectionState connectionState = ConnectionState.DISCONNECTED;
        private ConcurrentQueue<IDNP3Function> commandQueue = new ConcurrentQueue<IDNP3Function>();
        private bool threadCancellationSignal = true;
        public FunctionExecutor()
        {
            connection = new TCPConnection();

            connectionProcessorThread = new Thread(new ThreadStart(ConnectionProcessorThread));
        }

        private void ConnectionProcessorThread()
        {
            while (threadCancellationSignal)
            {
                try
                {
                    if(this.connectionState == ConnectionState.DISCONNECTED)
                    {
                        //konektuj
                        this.connection.Connect();
                    }
                    else
                    {

                        while(commandQueue.TryDequeue(out currentCommand))
                        {
                            this.connection.Send(this.currentCommand.PackRequest());
                            //logika prijema
                        }
                    }
                }
                catch(SocketException se)
                {

                }
            }
        }

        public void Dispose()
        {
            this.connection.Disconect();
            this.connectionProcessorThread.Abort();
        }

        public void EnqueueCommand(IDNP3Function command)
        {
            if(this.connectionState == ConnectionState.CONNECTED)
                this.commandQueue.Enqueue(command);
        }
    }
}
