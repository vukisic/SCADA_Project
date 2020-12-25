﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SCADA.Common.Connection;
using SCADA.Common.DataModel;
using SCADA.Common.Messaging;
using SCADA.Common.Messaging.Parameters;

namespace NDS.FrontEnd
{
    public class FEP : IFEP, IDisposable
    {
        // Trigger this event to update NDS with new data
        public event EventHandler<UpdateArgs> updateEvent;
        private Thread fepWorker;
        private bool executionFlag;
        private IConnection connection;

        public void Command()
        {
            // Implement command logic to ProcessingManager
        }

        public void Start()
        {
            connection = new TCPConnection();
            this.fepWorker = new Thread(Fep_DoWork);
            this.fepWorker.Name = "Fep thread";
            fepWorker.Start();
        }

        private void Fep_DoWork()
        {
            try
            {
                executionFlag = true;
                connection.Connect();
                while (executionFlag)
                {
                    // Logic
                    // TO DO: start FunctionExecutor, ProcessingManager and Acquisitor
                    var param = new DNP3ApplicationObjectParameters(0xc4, (byte)DNP3FunctionCode.DIRECT_OPERATE, (ushort)TypeField.BINARY_COMMAND, 0x28, 0x01, 0, 1, 0, 1, 2, 0xc1);
                    //param = new DNP3ApplicationObjectParameters(0xc4, (byte)DNP3FunctionCode.DIRECT_OPERATE, (ushort)TypeField.ANALOG_OUTPUT_16BIT, 0x28, 0x01, 0, 10, 0, 1, 2, 0xc1);
                    var result = DNP3FunctionFactory.CreateFunction(param);
                    byte[] m = result.PackRequest();
                    connection.Send(m);
                    Thread.Sleep(5000);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Stop()
        {
            executionFlag = false;
            Dispose();
        }

        public void Dispose()
        {
            fepWorker.Abort();
        }
    }
}