using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using SCADA.Common.Models;

namespace Core.Common.Contracts
{
    [ServiceContract]
    public interface IHistoryService
    {
        [OperationContract]
        Task Add(HistoryDbModel model);
        [OperationContract]
        Task AddRange(List<HistoryDbModel> list);
        [OperationContract]
        Task<List<HistoryDbModel>> GetByTimestamp(DateTime timestamp);
        [OperationContract]
        Task<List<HistoryDbModel>> GetInInverval(DateTime from, DateTime to);
        [OperationContract]
        Task<List<HistoryDbModel>> GetAll();
    }
}
