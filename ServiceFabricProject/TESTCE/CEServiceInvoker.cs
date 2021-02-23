using System;
using System.Collections.Generic;
using CETransaction;
using FTN.Common;
using FTN.Services.NetworkModelService;

namespace CE
{
    public class CEServiceInvoker
    {
        private CEServer _server;
        private CEWorker _worker;
        public static EventHandler<Dictionary<DMSType, Container>> _pointUpdate = delegate { };

        public CEServiceInvoker()
        {
            _pointUpdate += UpdatePoints;
            _server = new CEServer(_pointUpdate);
            _worker = new CEWorker();
        }

        public void Start()
        {
            _server.OpenModel();
            _server.OpenTransaction();
            _worker.Start();
        }

        public void Stop()
        {
            _server.CloseModel();
            _server.CloseTransaction();
            _worker.Stop();
        }

        private void UpdatePoints(object sender, Dictionary<DMSType, Container> e)
        {
            int points = GetPointsConut(e);
            _worker._updateEvent?.Invoke(this, points);
        }

        private int GetPointsConut(Dictionary<DMSType, Container> collection)
        {
            return collection[DMSType.ASYNCHRONOUSMACHINE] == null ? 0 : collection[DMSType.ASYNCHRONOUSMACHINE].Count;
        }
    }
}
