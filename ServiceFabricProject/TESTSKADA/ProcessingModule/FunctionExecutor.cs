using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SCADA.Common.DataModel;
using SCADA.Common.Logging;
using SCADA.Common.Messaging;
using SCADA.Common.Messaging.Messages;
using SCADA.Common.Messaging.Parameters;
using SCADA.Common.Proxies;
using SF.Common.Proxies;

namespace NDS.ProcessingModule
{
    public class FunctionExecutor : IDisposable, IFunctionExecutor
    {
        private IConnection connection;
        private IDNP3Function currentCommand;
        private Unsolicited unsolicitedCommand;
        private AutoResetEvent processConnection;
        private Thread connectionProcessorThread;
        private ConnectionState connectionState = ConnectionState.DISCONNECTED;
        private uint numberOfConnectionRetries = 0;
        private ConcurrentQueue<IDNP3Function> commandQueue = new ConcurrentQueue<IDNP3Function>();
        private bool threadCancellationSignal = true;
        private object lockObj = new object();
        public FunctionExecutor()
        {
            unsolicitedCommand = new Unsolicited();
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
                            lock (lockObj)
                            {
                                connection.Send(currentCommand.PackRequest());
                                byte[] message;
                                byte[] header = connection.Recv(10);
                                int recvLen = CalculateRecvLength(header[2]);
                                byte[] dataChunks = connection.Recv(recvLen);
                                message = new byte[header.Length + recvLen];
                                Buffer.BlockCopy(header, 0, message, 0, 10);
                                Buffer.BlockCopy(dataChunks, 0, message, 10, recvLen);

                                bool unsolicited = CheckIfUnsolicited(message[11]);
                                if (unsolicited)
                                {
                                    HandleReceivedBytes(message, unsolicited);
                                    DNP3ConfirmCommandParamters dnp3Param = new DNP3ConfirmCommandParamters(0xc0, (byte)DNP3FunctionCode.CONFIRM, 0xc0); //podesiti parametre
                                    IDNP3Function function = DNP3FunctionFactory.CreateConfirmFunction(dnp3Param);
                                    connection.Send(function.PackRequest());
                                }
                                else
                                {
                                    HandleReceivedBytes(message, unsolicited);
                                }
                                currentCommand = null;
                            }
                            
                        }
                    }
                }
                catch(SocketException se)
                {
                    currentCommand = null;
                    connectionState = ConnectionState.DISCONNECTED;
                    Log().Log(new LogEventModel() { EventType = LogEventType.ERROR, Message = $"{se.Message}" }).GetAwaiter().GetResult();
                }
                catch(Exception ex)
                {
                    currentCommand = null;
                    Log().Log(new LogEventModel() { EventType = LogEventType.WARN, Message = $"{ex.Message}" }).GetAwaiter().GetResult();
                }
            }
        }

        private LogServiceProxy Log()
        {
            return new LogServiceProxy();
        }

        private bool CheckIfUnsolicited(byte unsolicited)
        {
            int uns = (unsolicited & 0x10) >> 4;
            return uns == 1 ? true : false;
        }

        private void HandleReceivedBytes(byte[] message, bool unsolicited)
        {
            Dictionary<Tuple<RegisterType, int>, BasePoint> pointsToUpdate;

            if (!unsolicited)
                pointsToUpdate = currentCommand?.PareseResponse(message);
            else
                pointsToUpdate = unsolicitedCommand.PareseResponse(message);

            if (pointsToUpdate != null)
            {
                var alarming = new AlarmingProxy();
                var processedPoints = alarming.Check(pointsToUpdate).GetAwaiter().GetResult();
                var storage = new SF.Common.Proxies.ScadaStorageProxy();
                storage.UpdateModelValue(processedPoints).GetAwaiter().GetResult();
            }
        }

        private int CalculateRecvLength(byte lenByte)
        {
            var payLoadSize = (byte)(lenByte - 5);

            if (payLoadSize % 16 == 0)
            {
                return payLoadSize + (payLoadSize / 16) * 2;
            }
            else
            {
                return (payLoadSize / 16) == 0 ? (byte)(payLoadSize + 2) : (byte)(payLoadSize + (payLoadSize / 16) * 2 + 2);
            }

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
