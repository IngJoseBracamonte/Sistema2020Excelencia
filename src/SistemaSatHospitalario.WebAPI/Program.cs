using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SistemaSatHospitalario.Core.Application;
using SistemaSatHospitalario.Infrastructure;
using SistemaSatHospitalario.Infrastructure.Identity.Models;
using SistemaSatHospitalario.Infrastructure.Identity.Seeds;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using SistemaSatHospitalario.WebAPI.Infrastructure;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.RateLimiting; 
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire ServiceDefaults for OpenTelemetry and HealthChecks
builder.AddServiceDefaults();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();

// Add Rate Limiting (Phase 3 Security)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddFixedWindowLimiter("fixed", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 100; // 100 peticiones por minuto por IP
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
    });
});

// [SEC-003] CORS Explicit Origins - Whitelist (V13.3)
var allowedOriginsStr = builder.Configuration["AllowedOrigins"] ?? "https://localhost:4200,http://localhost:4200";
var allowedOrigins = allowedOriginsStr.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
Console.WriteLine($"[CORS CONFIG] Allowed Origins: {string.Join(" | ", allowedOrigins)}");

builder.Services.AddCors(options =>
{
    options.DefaultPolicyName = "AngularPolicy";
    options.AddPolicy("AngularPolicy", policy =>
    {
        policy.WithOrigins(allowedOrigins) 
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Add Application Layer Services (MediatR, FluentValidation)
builder.Services.AddApplicationServices();

builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddSignalR();
builder.Services.AddScoped<SistemaSatHospitalario.Core.Application.Common.Interfaces.ITasaNotificationService, SistemaSatHospitalario.WebAPI.Infrastructure.TasaNotificationService>();

// Add JWT Authentication
var jwtSecret = builder.Configuration["JwtConfig:Secret"] ?? "DefaultSecretKey_MustBeChangedInProduction_1234567890123456";
if (string.IsNullOrEmpty(jwtSecret))
{
    throw new InvalidOperationException("JWT Secret is not configured. Please check your environment variables or appsettings.");
}
var key = Encoding.ASCII.GetBytes(jwtSecret);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = !builder.Environment.IsDevelopment(); // Hardened but flexible for dev
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI",
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();

// [DIAGNOSTIC] Validar Parámetros JWT para evitar 401 por Mismatch (V11.8)
Console.WriteLine($"[JWT AUTH CONFIG] Issuer: {builder.Configuration["JwtConfig:Issuer"]}");
Console.WriteLine($"[JWT AUTH CONFIG] Audience: {builder.Configuration["JwtConfig:Audience"]}");
Console.WriteLine($"[JWT AUTH CONFIG] Secret Configured: {!string.IsNullOrEmpty(jwtSecret)}");

// Add Exception Handler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.MapDefaultEndpoints();


// Configure the HTTP request pipeline.
app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Fase 3: Security Headers Middleware
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
    await next();
});

// Fase 3: HSTS en Producción
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseRateLimiter();

// ----- Database Initializer (Robust Check V14.0) -----
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<SatHospitalarioDbContext>(); // Canary context
    
    try
    {
        logger.LogInformation("Iniciando secuencia de robustecimiento: verificando conexión a Base de Datos...");
        
        bool canConnect = false;
        int retries = 5;
        int delaySeconds = 3;

        for (int i = 1; i <= retries; i++)
        {
            try
            {
                // CanConnect no siempre da el error real, OpenConnection es más explícito
                await context.Database.OpenConnectionAsync();
                await context.Database.CloseConnectionAsync();
                canConnect = true;
                logger.LogInformation("Conexión a Base de Datos establecida correctamente.");
                break;
            }
            catch (Exception ex)
            {
                // Si el error es "Unknown database" (1049 en MySql), el servidor está vivo
                // y podemos proceder para que MigrateAsync cree la base de datos.
                if (ex.Message.Contains("Unknown database", StringComparison.OrdinalIgnoreCase) || 
                    ex.InnerException?.Message.Contains("Unknown database", StringComparison.OrdinalIgnoreCase) == true)
                {
                    canConnect = true;
                    logger.LogInformation("Servidor de Base de Datos respondio: 'Base de Datos inexistente'. Procediendo a la creacion de esquemas...");
                    break;
                }

                logger.LogWarning("Intento {Attempt}/{Total}: No se pudo conectar. Motivo: {ErrorMessage}. Reintentando en {Delay}s...", i, retries, ex.Message, delaySeconds);
                await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
            }
        }

        if (!canConnect)
        {
            logger.LogCritical("ERROR CRÍTICO: No se pudo establecer conexión con la Base de Datos tras {Total} intentos. El sistema no puede iniciar.", retries);
            throw new Exception("Database is unavailable after multiple retries.");
        }

        logger.LogInformation("Iniciando secuencia de inicialización de esquemas...");
        var initializers = services.GetServices<IDatabaseInitializer>();
        
        foreach (var initializer in initializers)
        {
            logger.LogInformation("Ejecutando inicializador: {InitializerType}", initializer.GetType().Name);
            await initializer.InitializeAsync();
            logger.LogInformation("Finalizado: {InitializerType}", initializer.GetType().Name);
        }
        logger.LogInformation("Secuencia de inicialización completada exitosamente.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "ERROR CRÍTICO durante la inicialización de bases de datos.");
    }
}
// --------------------------------

app.UseRouting();
app.UseCors("AngularPolicy");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<SistemaSatHospitalario.WebAPI.Hubs.DashboardHub>("/hub/dashboard");
app.MapHub<SistemaSatHospitalario.WebAPI.Hubs.TasaHub>("/hub/tasa");

app.Run();
