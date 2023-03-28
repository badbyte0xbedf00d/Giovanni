using System;
using System.Net;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Giovanni.Task1.Models;

namespace Giovanni.Task1.Services;

public interface ILogTableRepository
{

    /// <summary>
    /// 
    /// </summary>
    /// <param name="success"></param>
    /// <param name="logDateTime"></param>
    /// <param name="blobName"></param>
    /// <returns></returns>
    Task SaveAsync(HttpStatusCode statusCode, DateTime logDateTime, string blobName);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    AsyncPageable<LogEntity> GetPages(DateTimeOffset from, DateTimeOffset to);
}

public class LogTableRepository: ILogTableRepository
{
    private readonly ILogger<LogTableRepository> _logger;
    private readonly string _tableName;
    private readonly TableServiceClient _tableServiceClient;

    public LogTableRepository(
        ILogger<LogTableRepository> logger,
        IOptions<FunctionSettings> options,
        TableServiceClient tableServiceClient)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(options.Value.LogTableName);
        ArgumentNullException.ThrowIfNull(tableServiceClient);

        _logger = logger;
        _tableName = options.Value.LogTableName;
        _tableServiceClient = tableServiceClient;
    }

    /// <inheritdoc/>
    public async Task SaveAsync(HttpStatusCode statusCode, DateTime logDateTime, string blobName)
    {
        var tableClient = _tableServiceClient.GetTableClient(_tableName);
        await tableClient.CreateIfNotExistsAsync();

        var partitionKey = ((int)statusCode).ToString();
        var rowKey = logDateTime.ToString("yyyyMMddHHmmss");
        var apiLogEntity = new LogEntity()
        {
            PartitionKey = partitionKey,
            RowKey = rowKey,
            BlobName = blobName,
            LogDateTimeUtc = logDateTime
        };

        await tableClient.AddEntityAsync(apiLogEntity);

        _logger.LogDebug("Log saved to Table Storage as '{partitionKey}' - '{rowKey}'", partitionKey, rowKey);
    }

    /// <inheritdoc/>
    public AsyncPageable<LogEntity> GetPages(DateTimeOffset from, DateTimeOffset to)
    {
        var utcFrom = from.UtcDateTime;
        var utcTo = to.UtcDateTime;

        var tableClient = _tableServiceClient.GetTableClient(_tableName);

        return tableClient.QueryAsync<LogEntity>(filter =>
            filter.LogDateTimeUtc >= utcFrom && filter.LogDateTimeUtc <= utcTo);
    }
}