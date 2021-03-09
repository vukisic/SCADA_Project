using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Security.Principal;
using System.Threading;
using FTN.Common;
using System.Collections;
using FTN.ServiceContracts;
using FTN.Services.NetworkModelService.DataModel.Core;

namespace FTN.Services.NetworkModelService
{
	public class GenericDataAccess : INetworkModelGDAContract
	{
		protected static NetworkModel nm = null;

		public GenericDataAccess()
		{
		}

		public static NetworkModel NetworkModel
		{
			set
			{
				nm = value;
			}
		}

		public UpdateResult ApplyUpdate(Delta delta)
		{
			return nm.ApplyDelta(delta);
		}

        public IdentifiedObject GetValue(long globalId)
        {
            return nm.GetValue(globalId);
        }

        public List<IdentifiedObject> GetValues(List<long> globalId)
        {
            return nm.GetValues(globalId);
        }
    }
}
