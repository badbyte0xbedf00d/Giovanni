using System;
using Azure;
using Azure.Data.Tables;

namespace Giovanni.Task1.Models;

public class LogEntity: ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
    public DateTime LogDateTimeUtc { get; set; }
    public string BlobName { get; set; }
}