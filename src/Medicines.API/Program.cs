using Medicines.API.Extensions;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment.EnvironmentName;
IConfiguration configuration = new ConfigurationBuilder()
              .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile($"appsettings.{environment}.json", optional: false, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();

builder.Host.UseSerilog();

var logFilePath = configuration.GetSection("AppSettings")["LogFilePath"];

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File($"{logFilePath}/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Services.AddSettings(configuration);
builder.Services.AddServices(configuration, builder.Environment.IsDevelopment());

var app = builder.Build();

app.AddUsings();

app.Run();
