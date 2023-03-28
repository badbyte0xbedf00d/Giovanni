using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NodaTime;
using Giovanni.Task1.Extensions;
using Giovanni.Task1.Models;
using Giovanni.Task1.Services;

namespace Giovanni.Task1;

public class PublicApiFetchFunction
{
    private readonly IClock _clock;
    private readonly IPublicApiFetchService _publicApiFetchService;
    private readonly IPayloadBlobRepository _payloadBlobRepository;
    private readonly ILogTableRepository _logTableRepository;
    private readonly IMapper _mapper;

    public PublicApiFetchFunction(
        IClock clock,
        IPublicApiFetchService publicApiFetchService, 
        IPayloadBlobRepository payloadBlobRepository,
        ILogTableRepository logTableRepository,
        IMapper mapper)
    {
        ArgumentNullException.ThrowIfNull(clock);
        ArgumentNullException.ThrowIfNull(publicApiFetchService);
        ArgumentNullException.ThrowIfNull(payloadBlobRepository);
        ArgumentNullException.ThrowIfNull(logTableRepository);
        ArgumentNullException.ThrowIfNull(mapper);

        _clock = clock;
        _publicApiFetchService = publicApiFetchService;
        _payloadBlobRepository = payloadBlobRepository;
        _logTableRepository = logTableRepository;
        _mapper = mapper;
    }

    [FunctionName("PublicApiFetchFunction")]
    public async Task RunFetchAsync([TimerTrigger("0 */1 * * * *")] TimerInfo myTimer, ILogger logger)
    {
        var currentInstant = _clock.GetCurrentInstant();
        var dateTime = currentInstant.ToDateTimeUtc();
        var key = Guid.NewGuid();
        
        var (payload, statusCode) = await _publicApiFetchService.FetchAsync("/random?auth=null");
        
        if (statusCode.IsSuccessStatusCode())
        {
            var blobName =
                $"{key}_{dateTime.ToString("yyyyMMddHHmmss")}.json";

            await _payloadBlobRepository.SaveAsync(payload, blobName);
            await _logTableRepository.SaveAsync(statusCode, dateTime, blobName);

            logger.LogDebug("Success with payload: {payload}", payload);
        }
        else
        {
            await _logTableRepository.SaveAsync(statusCode, dateTime, null);

            logger.LogDebug("F with payload: {payload}", payload);
        }
    }

    [FunctionName("PublicApiFetchFunction-HttpGetLogs")]
    public async Task<IActionResult> RunGetHttpLogAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "publicapi/log")] HttpRequest request, ILogger logger)
    {
        string startParam = request.Query["from"];
        string endParam = request.Query["to"];

        if (string.IsNullOrEmpty(startParam) || string.IsNullOrEmpty(endParam))
        {
            return new BadRequestObjectResult("Please provide both 'from' and 'to' query parameters.");
        }

        if (!DateTimeOffset.TryParse(startParam, out var from) || !DateTimeOffset.TryParse(endParam, out var to))
        {
            return new BadRequestObjectResult("Please provide valid 'from' and 'to' query parameters.");
        }

        if (to < from)
        {
            return new BadRequestObjectResult("To date must be greater than or equal to the from date.");
        }

        var logs = _logTableRepository.GetPages(from, to);
        var logEntities = new List<LogEntity>();

        await foreach (var log in logs)
        {
            logEntities.Add(log);
        }

        using var ms = new MemoryStream();
        await JsonSerializer.SerializeAsync(ms, _mapper.Map<IEnumerable<LogOutputDto>>(logEntities));
        ms.Seek(0, SeekOrigin.Begin);

        using var streamReader = new StreamReader(ms);

        return new OkObjectResult(await streamReader.ReadToEndAsync());
    }

    [FunctionName("PublicApiFetchFunction-HttpGetPayload")]
    public async Task<IActionResult> RunGetHttpPayloadAsync(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "publicapi/payload/{blobName}")]
        HttpRequest request, string blobName, ILogger logger)
    {
        if (string.IsNullOrEmpty(blobName))
        {
            return new BadRequestObjectResult("Please provide a blob name.");
        }
        var payload = await _payloadBlobRepository.GetAsync(blobName);

        if (string.IsNullOrEmpty(payload))
        {
            return new NotFoundObjectResult("Blob not found.");
        }

        return new OkObjectResult(payload);
    }
}