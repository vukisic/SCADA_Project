using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace FTN.Services.NetworkModelService
{
    public class DeltaDto : TableEntity
    {
        public byte[] Data { get; set; }
        public DateTime TimeStamp { get; set; }
        public DeltaDto()
        {
            TimeStamp = DateTime.Now;
        }
        public DeltaDto(DateTime timeStamp)
        {
            PartitionKey = "Delta";
            RowKey = timeStamp.GetHashCode().ToString();
            TimeStamp = timeStamp;
            Timestamp = DateTime.Now;
        }
    }
}
