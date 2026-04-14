using Serilog;
using Serilog.Events;
using SistemaSatHospitalario.Core.Application;
using SistemaSatHospitalario.Infrastructure;
using SistemaSatHospitalario.WebAPI.Extensions;
using SistemaSatHospitalario.WebAPI.Infrastructure;
using System.IdentityModel.Tokens.Jwt;

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

    // Abstractions
    builder.Services.AddCustomRateLimiting();
    builder.Services.AddCustomCors(builder.Configuration, "AngularPolicy");
    builder.Services.AddCustomIdentityAndJwt(builder.Configuration, builder.Environment.IsDevelopment());
    builder.Services.AddCustomHealthChecks();
    builder.Services.AddCustomCaching(builder.Configuration);

    // Business & Data Layers
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);
    builder.Services.AddSignalR();
    
    // Notification Services
    builder.Services.AddScoped<SistemaSatHospitalario.Core.Application.Common.Interfaces.ITasaNotificationService, TasaNotificationService>();
    builder.Services.AddScoped<SistemaSatHospitalario.Core.Application.Common.Interfaces.INotificationService, NotificationService>();

    // Background Processing
    builder.Services.AddHostedService<SystemOptimizationService>();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // Middleware Pipeline
    app.MapDefaultEndpoints();
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }
    else
    {
        app.UseHttpsRedirection();
        app.UseHsts();
    }

    app.UseCustomSecurityHeaders();
    app.UseRateLimiter();
    app.MapHealthChecks("/health");

    // Database Initialization (Robust Loop)
    await app.UseDatabaseInitializationAsync();

    app.UseRouting();
    app.UseCors("AngularPolicy");
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();
    app.MapCustomHubs();

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
