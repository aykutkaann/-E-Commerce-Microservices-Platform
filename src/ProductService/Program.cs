using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using ProductService.Data;
using ProductService.Services;
using Serilog;
using Serilog.Events;
using System.Text.Json;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    Log.Information("ProductService starting up...");

    var builder = WebApplication.CreateBuilder(args);

    
    builder.Host.UseSerilog((context, services, config) =>
    {
        var serviceName = context.Configuration["ServiceName"] ?? "ProductService";
        var environment = context.HostingEnvironment.EnvironmentName;

        config
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)

           
            .Enrich.FromLogContext()
            .Enrich.WithEnvironmentName()
            .Enrich.WithProcessId()
            .Enrich.WithThreadId()
            .Enrich.WithProperty("ServiceName", serviceName)
            .Enrich.WithProperty("Environment", environment)

            
            .WriteTo.Console(outputTemplate:
                "[{Timestamp:HH:mm:ss} {Level:u3}] [{ServiceName}] {Message:lj} " +
                "{NewLine}{Exception}")

            
            .WriteTo.File(
                path: "logs/log-.txt",
                rollingInterval: RollingInterval.Day,   
                retainedFileCountLimit: 7,             
                outputTemplate:
                    "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] " +
                    "[{ServiceName}] {Message:lj}{NewLine}{Exception}");
    });

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(
            builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();

    builder.Services.AddHealthChecks()
        .AddDbContextCheck<AppDbContext>(
            name: "database",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "ready" });

    var app = builder.Build();

    // ── Migration
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }

  
    app.UseSerilogRequestLogging(options =>
    {
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} → {StatusCode} ({Elapsed:0.0000} ms)";

        
        options.GetLevel = (httpContext, elapsed, ex) =>
            ex != null || httpContext.Response.StatusCode >= 500
                ? LogEventLevel.Error
                : httpContext.Response.StatusCode >= 400
                    ? LogEventLevel.Warning
                    : LogEventLevel.Information;
    });

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthorization();
    app.MapControllers();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        Predicate = _ => false,
        ResponseWriter = WriteHealthResponse
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = WriteHealthResponse
    });

    app.Run();
}
catch (Exception ex)
{
    
    Log.Fatal(ex, "ProductService terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();  
}

static Task WriteHealthResponse(HttpContext context, HealthReport report)
{
    context.Response.ContentType = "application/json";

    var response = new
    {
        status = report.Status.ToString(),
        checks = report.Entries.Select(e => new
        {
            name = e.Key,
            status = e.Value.Status.ToString(),
            message = e.Value.Exception?.Message ?? "OK"
        }),
        duration = report.TotalDuration
    };

    return context.Response.WriteAsync(
        JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            WriteIndented = true
        })
    );
}