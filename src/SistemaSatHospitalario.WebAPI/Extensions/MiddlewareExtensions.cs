using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Persistence.Seeds;
using SistemaSatHospitalario.WebAPI.Hubs;
using System.Diagnostics;

namespace SistemaSatHospitalario.WebAPI.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomSecurityHeaders(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                await next();
            });

            return app;
        }

        public static async Task UseDatabaseInitializationAsync(this WebApplication app)
        {
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                var context = services.GetRequiredService<SatHospitalarioDbContext>();
                var configuration = services.GetRequiredService<IConfiguration>();

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
                            var builderCon = new MySqlConnector.MySqlConnectionStringBuilder(context.Database.GetConnectionString());
                            logger.LogInformation("Intento {Attempt}/{Total}: Probando conexión a {Server} (DB: {Database})...", i, retries, builderCon.Server, builderCon.Database);

                            await context.Database.OpenConnectionAsync();
                            await context.Database.CloseConnectionAsync();
                            canConnect = true;
                            logger.LogInformation("✅ Conexión a Base de Datos establecida correctamente.");
                            break;
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Unknown database", StringComparison.OrdinalIgnoreCase))
                            {
                                canConnect = true;
                                logger.LogInformation("ℹ️ Respuesta del Servidor: 'Base de Datos inexistente'. El servidor está activo. Procediendo a creación automática.");
                                break;
                            }

                            logger.LogWarning("❌ Intento {Attempt}/{Total} FALLIDO. Motivo: {ErrorMessage}", i, retries, ex.Message);
                            if (i < retries) await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
                        }
                    }

                    if (!canConnect) throw new Exception("No se pudo alcanzar el servidor de base de datos.");

                    logger.LogInformation("Iniciando secuencia de inicialización de esquemas...");
                    var initializers = services.GetServices<IDatabaseInitializer>();
                    
                    foreach (var initializer in initializers)
                    {
                        logger.LogInformation("Ejecutando inicializador: {InitializerType}", initializer.GetType().Name);
                        await initializer.InitializeAsync();
                    }
                    logger.LogInformation("Secuencia de inicialización completada exitosamente.");
                }
                catch (Exception ex)
                {
                    logger.LogCritical(ex, "ERROR CRÍTICO durante la inicialización de bases de datos. La aplicación iniciará en modo degradado.");
                }
            }
        }

        public static void MapCustomHubs(this WebApplication app)
        {
            app.MapHub<DashboardHub>("/hub/dashboard");
            app.MapHub<TasaHub>("/hub/tasa");
            app.MapHub<NotificationHub>("/hub/notifications");
        }
    }
}
