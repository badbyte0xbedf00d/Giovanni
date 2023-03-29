using Giovanni.Task2.AutoMapper;
using Giovanni.Task2.Infrastructure;
using Giovanni.Task2.Infrastructure.BackgroundServices;
using Giovanni.Task2.Infrastructure.HttpClient;
using Giovanni.Task2.Infrastructure.Models;
using Giovanni.Task2.Repositories;
using Giovanni.Task2.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NodaTime;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GiovanniDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("GiovanniDatabase")));

builder.Services.AddHttpClient<IWeatherFetchService, WeatherFetchService>().ConfigureHttpClient((sp, client) =>
{
    var weatherApiSettings = sp.GetRequiredService<IOptions<WeatherApiSettings>>().Value;
    client.BaseAddress = weatherApiSettings.WeatherApiUri;
}).AddHttpMessageHandler<ApiKeyAppender>();
builder.Services.AddTransient<ApiKeyAppender>();

builder.Services.AddOptions<WeatherApiSettings>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection("AppSettings:WeatherApiSettings").Bind(settings);
    });

builder.Services.AddOptions<WeatherSettings>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.GetSection("AppSettings:WeatherSettings").Bind(settings);
    });

builder.Services.AddTransient<IWeatherBackgroundService, WeatherBackgroundService>();
builder.Services.AddHostedService<WeatherBackgroundWorker>();
builder.Services.AddSingleton<IClock>(SystemClock.Instance);
builder.Services.AddScoped<IWeatherRepository, WeatherRepository>();
builder.Services.AddAutoMapper(typeof(EntitiesMappingProfile));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

using (var serviceScope = app.Services.CreateScope())
{
    var dbContext = serviceScope.ServiceProvider.GetRequiredService<GiovanniDbContext>();
    dbContext.Database.EnsureCreated();
    dbContext.Database.Migrate();
}

await app.RunAsync();
