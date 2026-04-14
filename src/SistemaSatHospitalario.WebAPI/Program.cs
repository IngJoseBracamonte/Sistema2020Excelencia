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
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

// [SEC-004] Standardize Claim Mapping - Prevents ASP.NET Core from rewriting claim names (V14.1 Senior Patch)
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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

builder.Services.AddCors(options =>
{
    options.DefaultPolicyName = "AngularPolicy";
    options.AddPolicy("AngularPolicy", policy =>
    {
        // [MOD-CORS] Modificado para permitir cualquier origen (Netlify, local, etc) 
        // Compatible con AllowCredentials()
        policy.SetIsOriginAllowed(origin => true) 
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
        ClockSkew = TimeSpan.Zero,
        
        // [SEC-004] Explicit Claim Mapping for Names and Roles (GEN-001 Compliance)
        NameClaimType = ClaimTypes.Name,
        RoleClaimType = ClaimTypes.Role
    };
});
builder.Services.AddAuthorization();

// Global exception handler configuration
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// [DIAGNOSTIC] Startup Security Matrix Report (V1.1 Standardized Logging)
app.Logger.LogInformation("[CORS CONFIG] Allowed Origins: {Origins}", string.Join(" | ", allowedOrigins));
app.Logger.LogInformation("[JWT AUTH CONFIG] Issuer: {Issuer}", builder.Configuration["JwtConfig:Issuer"]);
app.Logger.LogInformation("[JWT AUTH CONFIG] Audience: {Audience}", builder.Configuration["JwtConfig:Audience"]);
app.Logger.LogInformation("[JWT AUTH CONFIG] Secret Configured: {SecretStatus}", !string.IsNullOrEmpty(jwtSecret));

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

        // [CLOUD-FIX] Reparar esquema antes de ejecutar initializers
        await RepairCloudSchemaAsync(context, logger);

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
        logger.LogCritical(ex, "ERROR CRÍTICO durante la inicialización de bases de datos. La aplicación iniciará en modo degradado para permitir diagnósticos (CORS enabled).");
        // No relanzamos la excepción para permitir que el middleware de CORS y ExceptionHandler se activen
        // y el frontend pueda ver el error real en lugar de un bloqueo de red.
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

// ===== CLOUD SCHEMA REPAIR (MySQL Idempotent) =====
static async Task RepairCloudSchemaAsync(SatHospitalarioDbContext context, ILogger logger)
{
    logger.LogInformation("[SCHEMA-REPAIR] Iniciando reparación idempotente de esquema MySQL para Cloud...");
    
    var repairs = new List<string>
    {
        // 0. Compatibilidad Aiven/Managed MySQL (Primary Key Requirement)
        "SET SESSION sql_require_primary_key = 0;",

        // 0.b Tablas Requeridas (Migración awareness - Evitar colisión en DB nuevas)
        @"SET @applied = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='__EFMigrationsHistory') > 0, 
                           (SELECT COUNT(*) FROM `__EFMigrationsHistory` WHERE `MigrationId` LIKE '%AddEspecialidadEntity%'), 0);
          SET @table = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='Especialidades');
          SET @s = IF(@applied > 0 AND @table = 0, 'CREATE TABLE `Especialidades` (`Id` char(36) NOT NULL, `Nombre` longtext NOT NULL, `Activo` tinyint(1) NOT NULL, PRIMARY KEY (`Id`))', 'SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        @"CREATE TABLE IF NOT EXISTS `SegurosConvenios` (
            `Id` int NOT NULL AUTO_INCREMENT,
            `Nombre` varchar(200) NOT NULL,
            `Rtn` varchar(50) NULL,
            `Direccion` varchar(500) NULL,
            `Telefono` varchar(50) NULL,
            `Email` varchar(150) NULL,
            PRIMARY KEY (`Id`)
        ) CHARACTER SET utf8mb4;",

        // 1. Medicos.EspecialidadId (Asegurar tipo CHAR(36) para evitar error de BLOB en INDEX)
        @"SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='Medicos' AND COLUMN_NAME='EspecialidadId');
          SET @type = (SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='Medicos' AND COLUMN_NAME='EspecialidadId');
          SET @s = IF(@col=0,
            'ALTER TABLE `Medicos` ADD COLUMN `EspecialidadId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL DEFAULT ""00000000-0000-0000-0000-000000000000""',
            IF(@type='longtext' OR @type='text',
                'ALTER TABLE `Medicos` MODIFY COLUMN `EspecialidadId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL',
                'SELECT 1')
          );
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 2. Medicos.HonorarioBase (Migration: AddMedicoHonorarioBase)
        @"SET @applied = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='__EFMigrationsHistory') > 0, 
                           (SELECT COUNT(*) FROM `__EFMigrationsHistory` WHERE `MigrationId` LIKE '%AddMedicoHonorarioBase%'), 0);
          SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='Medicos' AND COLUMN_NAME='HonorarioBase');
          SET @s = IF(@applied > 0 AND @col = 0, 'ALTER TABLE `Medicos` ADD COLUMN `HonorarioBase` decimal(18,2) NOT NULL DEFAULT 0.00', 'SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 3. ServiciosClinicos.Category (Sólo si la migración ya figura como aplicada pero la columna falta)
        @"SET @applied = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='__EFMigrationsHistory') > 0, 
                           (SELECT COUNT(*) FROM `__EFMigrationsHistory` WHERE `MigrationId` LIKE '%AddServiceCategoryToServicioClinico%'), 0);
          SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ServiciosClinicos' AND COLUMN_NAME='Category');
          SET @s = IF(@applied > 0 AND @col = 0, 'ALTER TABLE `ServiciosClinicos` ADD COLUMN `Category` int NOT NULL DEFAULT 0', 'SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 4. ServiciosClinicos.HonorarioBase (Migration: AddHonorariumBaseToCatalog)
        @"SET @applied = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='__EFMigrationsHistory') > 0, 
                           (SELECT COUNT(*) FROM `__EFMigrationsHistory` WHERE `MigrationId` LIKE '%AddHonorariumBaseToCatalog%'), 0);
          SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ServiciosClinicos' AND COLUMN_NAME='HonorarioBase');
          SET @s = IF(@applied > 0 AND @col = 0, 'ALTER TABLE `ServiciosClinicos` ADD COLUMN `HonorarioBase` decimal(18,2) NOT NULL DEFAULT 0.00', 'SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 5. DetallesPago.FechaPago
        @"SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='DetallesPago' AND COLUMN_NAME='FechaPago');
          SET @s = IF(@col=0,'ALTER TABLE `DetallesPago` ADD COLUMN `FechaPago` datetime(6) NOT NULL DEFAULT ''0001-01-01 00:00:00''','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 6. ServiciosClinicos.LegacyMappingId (Migration: AddLegacyMappingId)
        @"SET @applied = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='__EFMigrationsHistory') > 0, 
                           (SELECT COUNT(*) FROM `__EFMigrationsHistory` WHERE `MigrationId` LIKE '%AddLegacyMappingId%'), 0);
          SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ServiciosClinicos' AND COLUMN_NAME='LegacyMappingId');
          SET @s = IF(@applied > 0 AND @col = 0, 'ALTER TABLE `ServiciosClinicos` ADD COLUMN `LegacyMappingId` longtext NULL', 'SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 7. DetallesServicioCuenta.LegacyMappingId (Migration: AddLegacyMappingId)
        @"SET @applied = IF((SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='__EFMigrationsHistory') > 0, 
                           (SELECT COUNT(*) FROM `__EFMigrationsHistory` WHERE `MigrationId` LIKE '%AddLegacyMappingId%'), 0);
          SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='DetallesServicioCuenta' AND COLUMN_NAME='LegacyMappingId');
          SET @s = IF(@applied > 0 AND @col = 0, 'ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `LegacyMappingId` longtext NULL', 'SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 8. DetallesServicioCuenta.Honorario
        @"SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='DetallesServicioCuenta' AND COLUMN_NAME='Honorario');
          SET @s = IF(@col=0,'ALTER TABLE `DetallesServicioCuenta` ADD COLUMN `Honorario` decimal(18,2) NOT NULL DEFAULT 0','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 9. RecibosFacturas.MontoVueltoUSD
        @"SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='RecibosFacturas' AND COLUMN_NAME='MontoVueltoUSD');
          SET @s = IF(@col=0,'ALTER TABLE `RecibosFacturas` ADD COLUMN `MontoVueltoUSD` decimal(18,2) NOT NULL DEFAULT 0','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 10. CuentasPorCobrar.FechaCreacion (rename from FechaEmision or add)
        @"SET @cn = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='CuentasPorCobrar' AND COLUMN_NAME='FechaCreacion');
          SET @co = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='CuentasPorCobrar' AND COLUMN_NAME='FechaEmision');
          SET @s = IF(@cn>0,'SELECT 1',IF(@co>0,'ALTER TABLE `CuentasPorCobrar` CHANGE COLUMN `FechaEmision` `FechaCreacion` datetime(6) NOT NULL','ALTER TABLE `CuentasPorCobrar` ADD COLUMN `FechaCreacion` datetime(6) NOT NULL DEFAULT ''0001-01-01 00:00:00'''));
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 10b. CuentasPorCobrar.IsAudited
        @"SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='CuentasPorCobrar' AND COLUMN_NAME='IsAudited');
          SET @s = IF(@col=0,'ALTER TABLE `CuentasPorCobrar` ADD COLUMN `IsAudited` tinyint(1) NOT NULL DEFAULT 0','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 11. ConfiguracionGeneral.ClaveSupervisor
        @"SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='ConfiguracionGeneral' AND COLUMN_NAME='ClaveSupervisor');
          SET @s = IF(@col=0,'ALTER TABLE `ConfiguracionGeneral` ADD COLUMN `ClaveSupervisor` longtext NOT NULL','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 12. Tabla AuditLogsPrecios
        @"CREATE TABLE IF NOT EXISTS `AuditLogsPrecios` (
            `Id` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
            `DetalleServicioId` char(36) CHARACTER SET ascii COLLATE ascii_general_ci NOT NULL,
            `DescripcionServicio` varchar(500) NOT NULL,
            `PrecioOriginal` decimal(18,2) NOT NULL,
            `PrecioModificado` decimal(18,2) NOT NULL,
            `UsuarioOperador` varchar(100) NOT NULL,
            `AutorizadoPor` varchar(100) NOT NULL,
            `FechaModificacion` datetime(6) NOT NULL,
            `HonorarioAnterior` decimal(18,2) NOT NULL DEFAULT 0,
            `NuevoHonorario` decimal(18,2) NOT NULL DEFAULT 0,
            PRIMARY KEY (`Id`)
        ) CHARACTER SET utf8mb4;",

        // 13. Índice Medicos.EspecialidadId
        @"SET @idx = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='Medicos' AND INDEX_NAME='IX_Medicos_EspecialidadId');
          SET @s = IF(@idx=0,'CREATE INDEX `IX_Medicos_EspecialidadId` ON `Medicos` (`EspecialidadId`)','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 14. Índice DetallesPago.FechaPago
        @"SET @idx = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.STATISTICS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='DetallesPago' AND INDEX_NAME='IX_DetallesPago_FechaPago');
          SET @s = IF(@idx=0,'CREATE INDEX `IX_DetallesPago_FechaPago` ON `DetallesPago` (`FechaPago`)','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;",

        // 15. Discriminador TPH para OrdenesDeServicio (Requerido para OrdenRX)
        @"SET @col = (SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA=DATABASE() AND TABLE_NAME='OrdenesDeServicio' AND COLUMN_NAME='Discriminator');
          SET @s = IF(@col=0,'ALTER TABLE `OrdenesDeServicio` ADD COLUMN `Discriminator` longtext NOT NULL','SELECT 1');
          PREPARE st FROM @s; EXECUTE st; DEALLOCATE PREPARE st;"
    };

    int repaired = 0;
    foreach (var sql in repairs)
    {
        try
        {
            await context.Database.ExecuteSqlRawAsync(sql);
            repaired++;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "[SCHEMA-REPAIR] Error en repair (puede ser esperado): {Sql}", sql.Length > 80 ? sql.Substring(0, 80) : sql);
        }
    }

    logger.LogInformation("[SCHEMA-REPAIR] Completado. {Repaired}/{Total} ejecutadas.", repaired, repairs.Count);
}
