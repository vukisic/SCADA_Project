using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using FTN.Common;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.ServiceContracts
{
	[ServiceContract]
	public interface INetworkModelGDAContract
	{
		[OperationContract]	
		UpdateResult ApplyUpdate(Delta delta);

        [OperationContract]
        IdentifiedObject GetValue(long globalId);


        [OperationContract]
        List<IdentifiedObject> GetValues(List<long> globalId);
    }
}
