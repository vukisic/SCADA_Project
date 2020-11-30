using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using FTN.Common;
using System.Configuration;

namespace FTN.Services.NetworkModelService.DeltaDB
{
    [ServiceContract]
    public interface IDeltaDBRepository
    {
        [OperationContract]
        void AddDelta(DeltaDBModel delta);

        [OperationContract]
        DeltaDBModel FindDeltaById(long id);

        [OperationContract]
        List<DeltaDBModel> GetAllDeltas();

        [OperationContract]
        void DeleteAllDeltas();
    }
}
