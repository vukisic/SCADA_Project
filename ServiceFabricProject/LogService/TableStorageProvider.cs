using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using SCADA.Common.Logging;

namespace LogService
{
    public class TableLog : TableEntity
    {
        public LogEventType Type { get; set; }
        public string Message  { get; set; }
        public TableLog(LogEventType type, string message)
        {
            PartitionKey = "logging";
            RowKey = DateTime.Now.ToString().GetHashCode().ToString();
            Type = type;
            Message = message;
        }

        public TableLog()
        {
            PartitionKey = "logging";
            RowKey = DateTime.Now.ToString().GetHashCode().ToString();
        }
    }

    public class TableStorageProvider
    {
        private CloudStorageAccount account;
        private CloudTable table;
        
        public TableStorageProvider(string tableName, bool deleteOldData)
        {
            account = CloudStorageAccount.Parse("UseDevelopmentStorage=true");
            CloudTableClient tableClient = new CloudTableClient(new Uri(account.TableEndpoint.AbsoluteUri), account.Credentials);
            table = tableClient.GetTableReference(tableName);
            
            if (deleteOldData)
            {
                if (table.Exists())
                    table.Delete();
            }

            table.CreateIfNotExists();
        }

        public void Add(TableLog item)
        {
            try
            {
                TableOperation insert = TableOperation.InsertOrReplace(item);
                table.Execute(insert);
            }
            catch (Exception ex)
            {

                throw new Exception("Error! Duplicated primary key 'index'!");
            }
        }
    }
}
