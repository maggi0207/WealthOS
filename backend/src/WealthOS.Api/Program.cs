using Serilog;
using WealthOS.Api.Extensions;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("Starting WealthOS API");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) =>
        configuration
            .ReadFrom.Configuration(context.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console());

    builder.Services.AddApiServices(builder.Configuration);

    var app = builder.Build();
    app.ConfigurePipeline();

    app.Run();
}
catch (Exception exception)
{
    Log.Fatal(exception, "WealthOS API terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;
