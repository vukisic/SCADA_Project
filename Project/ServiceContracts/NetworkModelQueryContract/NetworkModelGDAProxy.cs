using System.Collections.Generic;
using FTN.Common;
using System.ServiceModel;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.ServiceContracts
{
	public class NetworkModelGDAProxy : ClientBase<INetworkModelGDAContract>, INetworkModelGDAContract
	{
		public NetworkModelGDAProxy(string endpointName)
			: base(endpointName)
		{
		}

		public UpdateResult ApplyUpdate(Delta delta)
		{
			return Channel.ApplyUpdate(delta);
		}

        public IdentifiedObject GetValue(long globalId)
        {
            return Channel.GetValue(globalId);
        }

        public List<IdentifiedObject> GetValues(List<long> globalId)
        {
            return Channel.GetValues(globalId);
        }
    }
}
