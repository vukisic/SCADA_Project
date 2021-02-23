using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using FTN.Common;
using System.Configuration;


namespace FTN.Services.NetworkModelService.DeltaDB
{
	public class DeltaDBRepository : IDeltaDBRepository
	{
		private DeltaDBContext context = new DeltaDBContext();

        public DeltaDBRepository()
        {
            context.Database.CreateIfNotExists();
        }

		public void AddDelta(DeltaDBModel delta)
		{
			context.Deltas.Add(delta);
			context.SaveChanges();
		}

		public void DeleteAllDeltas()
		{
			context.Deltas.RemoveRange(context.Deltas);
			context.SaveChanges();
		}

		public DeltaDBModel FindDeltaById(long id)
		{
			return context.Deltas.Find(id);
		}

		public List<DeltaDBModel> GetAllDeltas()
		{
			return context.Deltas.ToList();
		}
	}
}
