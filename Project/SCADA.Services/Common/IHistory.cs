using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.DB.Models;

namespace SCADA.Services.Common
{
    [ServiceContract]
    public interface IHistory
    {
        [OperationContract]
        void Add(HistoryDbModel model);

        [OperationContract]
        void AddRange(List<HistoryDbModel> list);

        [OperationContract]
        List<HistoryDbModel> GetByTimestamp(DateTime timestamp);

        [OperationContract]
        List<HistoryDbModel> GetInInverval(DateTime from, DateTime to);
        [OperationContract]
        List<HistoryDbModel> GetAll();
    }
}
