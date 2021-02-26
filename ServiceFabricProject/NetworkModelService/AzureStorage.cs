using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Table;

namespace FTN.Services.NetworkModelService
{
    internal sealed class TEntity : TableEntity
    {
        public string Uri { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Name { get; set; }
        public TEntity()
        {
            TimeStamp = DateTime.Now;
        }

        public TEntity(string uri, DateTime timeStamp, string name)
        {
            RowKey = DateTime.Now.GetHashCode().ToString();
            PartitionKey = "Delta";
            Uri = uri;
            TimeStamp = timeStamp;
            Name = name;
        }
    }
    public class AzureStorage
    {
        private CloudStorageAccount account;
        private CloudTable table;
        private CloudBlobContainer blob;
        public AzureStorage(string tableName, bool deleteOldData)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudTableClient tableClient = new CloudTableClient(new Uri(account.TableEndpoint.AbsoluteUri), account.Credentials);
            table = tableClient.GetTableReference(tableName);
            CloudBlobClient blobClient = account.CreateCloudBlobClient();
            blob = blobClient.GetContainerReference(tableName.ToLower());
            if (deleteOldData)
            {
                if (table.Exists())
                    table.Delete();
                if (blob.Exists())
                    blob.Delete();
            }

            table.CreateIfNotExists();
            blob.CreateIfNotExists();
        }

        public void Add(DeltaDto item)
        {
            try
            {
                CloudBlockBlob block = blob.GetBlockBlobReference(item.RowKey);
                block.UploadFromByteArray(item.Data, 0, item.Data.Length);
                var uri = block.Uri.AbsoluteUri;
                TEntity entitiy = new TEntity(uri, item.TimeStamp, item.RowKey);
                TableOperation insert = TableOperation.InsertOrReplace(entitiy);
                table.Execute(insert);
            }
            catch (Exception)
            {

                throw new Exception("Error! Duplicated primary key 'index'!");
            }
        }

        public List<DeltaDto> RetrieveAll(string partitionKey)
        {
            List<DeltaDto> dtos = new List<DeltaDto>();
            TableQuery<TEntity> query = new TableQuery<TEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, "Delta"));
            var result = table.ExecuteQuery<TEntity>(query).AsQueryable<TEntity>().ToList();
            foreach (var item in result)
            {
                DeltaDto dto = new DeltaDto();
                MemoryStream mem = new MemoryStream();
                CloudBlockBlob block = blob.GetBlockBlobReference(item.Name);
                block.DownloadToStream(mem);
                mem.Position = 0;
                dto.Data = mem.ToArray();
                dto.TimeStamp = item.TimeStamp;
                dtos.Add(dto);
            }

            return dtos;
        }
    }
}
