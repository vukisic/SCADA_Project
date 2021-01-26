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
using SCADA.Common.Proxies;

namespace SCADA.Common.Connection
{
    public class FunctionExecutor : IDisposable, IFunctionExecutor
    {
        private IConnection connection;
        private IDNP3Function currentCommand;
        private AutoResetEvent processConnection;
        private Thread connectionProcessorThread;
        private ConnectionState connectionState = ConnectionState.DISCONNECTED;
        private uint numberOfConnectionRetries = 0;
        private ConcurrentQueue<IDNP3Function> commandQueue = new ConcurrentQueue<IDNP3Function>();
        private bool threadCancellationSignal = true;
        public FunctionExecutor()
        {
            connection = new TCPConnection();
            processConnection = new AutoResetEvent(false);
            connectionProcessorThread = new Thread(new ThreadStart(ConnectionProcessorThread));
            connectionProcessorThread.Name = "Communication Thread";
            connectionProcessorThread.Start();
        }

        private void ConnectionProcessorThread()
        {
            while (threadCancellationSignal)
            {
                try
                {
                    if(connectionState == ConnectionState.DISCONNECTED)
                    {
                        numberOfConnectionRetries = 0;
                        connection.Connect();
                        while(numberOfConnectionRetries < 10)
                        {
                            if (connection.CheckState())
                            {
                                this.connectionState = ConnectionState.CONNECTED;
                                numberOfConnectionRetries = 0;
                                break;
                            }
                            else
                            {
                                numberOfConnectionRetries++;
                                if(numberOfConnectionRetries == 10)
                                {
                                    connection.Disconect();
                                    connectionState = ConnectionState.DISCONNECTED;
                                }
                            }
                        }                      
                    }
                    else
                    {
                        processConnection.WaitOne();
                        while(commandQueue.TryDequeue(out currentCommand))
                        {
                            connection.Send(currentCommand.PackRequest());
                            byte[] message;
                            byte[] header = connection.Recv(10);
                            //trebao bi proveriti checksum u hederu //ili ovde ili u parseresponse(mozda je bolje ovde)
                            int recvLen = CalculateRecvLength(header[2]); //len
                            byte[] dataChunks = connection.Recv(recvLen);
                            //proveriti checksum za datachunk(16-2)
                            message = new byte[header.Length + recvLen];
                            Buffer.BlockCopy(header, 0, message, 0, 10);
                            Buffer.BlockCopy(dataChunks, 0, message, 10, recvLen);

                            HandleReceivedBytes(message);
                            currentCommand = null;
                        }
                    }
                }
                catch(SocketException se)
                {
                    currentCommand = null;
                    connectionState = ConnectionState.DISCONNECTED;
                }
                catch(Exception ex)
                {
                    currentCommand = null;
                }
            }
        }

        private void HandleReceivedBytes(byte[] message)
        {
            Dictionary<Tuple<RegisterType, int>, BasePoint> pointsToUpdate = currentCommand?.PareseResponse(message);
            ScadaProxyFactory.Instance().ScadaStorageProxy().UpdateModel(pointsToUpdate);

        }

        private int CalculateRecvLength(byte lenByte)
        {
            int recvLen;
            int dataChunksLen = lenByte - 9; //((startbytes, len)ovo neulazi u len) - ostatak hedera - checksum
            if (dataChunksLen % 16 == 0)
                recvLen = dataChunksLen / 16 * 2;
            else
                recvLen = dataChunksLen / 16 * 2 + 2;
            return recvLen;
        }

        public void Dispose()
        {
            connection.Disconect();
            connectionProcessorThread.Abort();
        }

        public void EnqueueCommand(IDNP3Function command)
        {
            if(this.connectionState == ConnectionState.CONNECTED)
            {
                commandQueue.Enqueue(command);
                processConnection.Set();
            }
        }
    }
}
