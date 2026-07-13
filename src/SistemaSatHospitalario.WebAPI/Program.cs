using Serilog;
using Serilog.Events;
using SistemaSatHospitalario.Core.Application;
using SistemaSatHospitalario.Infrastructure;
using SistemaSatHospitalario.WebAPI.Extensions;
using SistemaSatHospitalario.WebAPI.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using Scalar.AspNetCore;

// [SEC-004] Standardize Claim Mapping (V14.1 Senior Patch)
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

// [PHASE-1] Serilog Configuration
if (!Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.Equals("Testing", StringComparison.OrdinalIgnoreCase) ?? true)
{
    Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
        .CreateLogger();
}
else
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();
}
try 
{
    Log.Information("Iniciando Sistema Sat Hospitalario v1.2.92 (Cloud-Native Mode)...");
    
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // V12.6 Dynamic App.config Connection Override Strategy
    try
    {
        var connectionStrings = builder.Configuration.GetSection("ConnectionStrings");
        foreach (var cs in connectionStrings.GetChildren())
        {
            var originalValue = cs.Value;
            if (string.IsNullOrEmpty(originalValue)) continue;

            var resolvedValue = SistemaSatHospitalario.Infrastructure.Common.Helpers.ConnectionStringHelper.ResolveConnectionStringWithAppConfig(originalValue);
            if (originalValue != resolvedValue)
            {
                builder.Configuration[$"ConnectionStrings:{cs.Key}"] = resolvedValue;
                Log.Information("[CONFIG] Overrode ConnectionStrings:{Key} with values from App.config", cs.Key);
            }
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Failed to resolve connection strings from App.config");
    }

    // Core Services
    builder.AddServiceDefaults(); // Aspire
    builder.Services.AddOpenApi();
    builder.Services.AddControllers()
        .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        });

    // Business & Data Layers
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    // Abstractions
    builder.Services.AddCustomRateLimiting();
    builder.Services.AddCustomForwardedHeaders();
    builder.Services.AddCustomCors(builder.Configuration, "AngularPolicy");
    builder.Services.AddCustomIdentityAndJwt(builder.Configuration, builder.Environment.IsDevelopment());
    builder.Services.AddCustomHealthChecks();
    builder.Services.AddCustomCaching(builder.Configuration);
    builder.Services.AddSignalR();
    
    // Notification Services
    builder.Services.AddScoped<SistemaSatHospitalario.Core.Application.Common.Interfaces.ITasaNotificationService, TasaNotificationService>();

    // Background Processing
    builder.Services.AddHostedService<SystemOptimizationService>();
    builder.Services.AddHostedService<LegacyOrderStatusWorker>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // Middleware Pipeline
    app.MapDefaultEndpoints();
    app.UseForwardedHeaders();
    app.UseExceptionHandler();

    var deployMode = app.Configuration["DeploymentSettings:Mode"] ?? "Local";

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => 
        {
            options.WithTitle("Sistema Sat Hospitalario API (Senior Dashboard)")
                   .WithDefaultHttpClient(Scalar.AspNetCore.ScalarTarget.CSharp, Scalar.AspNetCore.ScalarClient.HttpClient);
        });
    }
    else if (!deployMode.Equals("Docker", StringComparison.OrdinalIgnoreCase))
    {
        // Solo redirigir a HTTPS si NO estamos en Docker (ej. Cloud Native manual)
        app.UseHttpsRedirection();
        app.UseHsts();
    }

    app.UseCustomSecurityHeaders();
    app.UseRateLimiter();

    // Database Initialization (Robust Loop)
    if (!app.Environment.IsEnvironment("Testing"))
    {
        var skipMigrations = app.Configuration.GetValue<bool>("DeploymentSettings:SkipMigrations");
        if (skipMigrations)
        {
            Log.Information("[DEPLOYMENT] Se omitió la inicialización y migración automática de la base de datos por configuración.");
        }
        else
        {
            await app.UseDatabaseInitializationAsync();
        }
    }

    app.UseRouting();
    app.UseCors("AngularPolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapCustomHubs();

    // [TELEMETRY] Direct Minimal API for Frontend Logs (Bypass Controller Issues)
    app.MapPost("/api/diagnostics/logs", (LogPayload payload, ILogger<Program> logger) => {
        switch (payload.Level?.ToLower())
        {
            case "error": logger.LogError("[FE-ERROR] {Message} | Context: {Context}", payload.Message, payload.Context); break;
            case "warning": logger.LogWarning("[FE-WARN] {Message}", payload.Message); break;
            default: logger.LogInformation("[FE-INFO] {Message}", payload.Message); break;
        }
        return Results.Ok(new { Success = true });
    }).AllowAnonymous();

    app.Run();
}
catch (Exception ex)
{
    File.WriteAllText("startup_error.txt", ex.ToString());
    Log.Fatal(ex, "El sistema falló durante el arranque.");
    throw;
}
finally
{
    Log.CloseAndFlush();
}

public class LogPayload {
    public string Level { get; set; } = "info";
    public string Message { get; set; }
    public object Context { get; set; }
}

public partial class Program { }
