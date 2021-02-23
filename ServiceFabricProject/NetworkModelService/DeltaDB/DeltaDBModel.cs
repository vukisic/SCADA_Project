using System;
using System.Collections.Generic;
using System.Linq;
using FTN.Common;
using System.Configuration;


namespace FTN.Services.NetworkModelService.DeltaDB
{
	public class DeltaDBModel
	{
		public long Id { get; set; }
		public byte[] Data { get; set; }
	}
}
