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
    public interface IDomService
    {
        [OperationContract]
        Task Add(List<DomDbModel> model);
        [OperationContract]
        Task AddOrUpdate(DomDbModel model);
        [OperationContract]
        Task AddOrUpdateRange(List<DomDbModel> list);
        [OperationContract]
        Task UpdateSingle(DomDbModel model);
        [OperationContract]
        Task<List<DomDbModel>> GetAll();
    }
}
