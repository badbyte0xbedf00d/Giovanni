using System;
using System.Net.Http;
using Azure.Storage.Blobs;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Contrib.WaitAndRetry;
using Polly.Extensions.Http;
using Azure.Data.Tables;
using Giovanni.Task1.AutoMapper;
using NodaTime;
using Giovanni.Task1.Models;
using Giovanni.Task1.Services;

[assembly: FunctionsStartup(typeof(Giovanni.Task1.Startup))]

namespace Giovanni.Task1;

public class Startup : FunctionsStartup
{
    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        var delay = Backoff.DecorrelatedJitterBackoffV2(medianFirstRetryDelay: TimeSpan.FromSeconds(1), retryCount: 5);

        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(delay);
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        builder
            .Services
            .AddOptions<FunctionSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("FunctionSettings").Bind(settings);
            })
            .Services
            .AddHttpClient<IPublicApiFetchService, PublicApiFetchService>((provider, client) =>
                client.BaseAddress = provider.GetRequiredService<IOptions<FunctionSettings>>().Value.PublicApiFetchUri)
            .SetHandlerLifetime(TimeSpan.FromSeconds(30))
            .AddPolicyHandler(GetRetryPolicy())
            .Services
            .AddAutoMapper(typeof(EntitiesMappingProfile))
            .AddSingleton(_ => new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")))
            .AddSingleton(_ => new TableServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage")))
            .AddSingleton<IClock>(SystemClock.Instance)
            .AddSingleton<IPayloadBlobRepository, PayloadBlobRepository>()
            .AddSingleton<ILogTableRepository, LogTableRepository>();
    }
}