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
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try 
{
    Log.Information("Iniciando Sistema Sat Hospitalario (Cloud-Native Mode)...");
    
    var builder = WebApplication.CreateBuilder(args);
    builder.Host.UseSerilog();

    // Core Services
    builder.AddServiceDefaults(); // Aspire
    builder.Services.AddOpenApi();
    builder.Services.AddControllers();

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

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
        app.MapScalarApiReference(options => 
        {
            options.WithTitle("Sistema Sat Hospitalario API (Senior Dashboard)")
                   .WithDefaultHttpClient(Scalar.AspNetCore.ScalarTarget.CSharp, Scalar.AspNetCore.ScalarClient.HttpClient);
        });
    }
    else
    {
        app.UseHttpsRedirection();
        app.UseHsts();
    }

    app.UseCustomSecurityHeaders();
    app.UseRateLimiter();

    // Database Initialization (Robust Loop)
    await app.UseDatabaseInitializationAsync();

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
    Log.Fatal(ex, "El sistema falló durante el arranque.");
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
