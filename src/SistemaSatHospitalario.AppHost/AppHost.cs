using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// --- Configuración Global de Infraestructura ---
// Configuración de base de datos orquestada
var dbProviderName = builder.Configuration["DatabaseProvider"] ?? "MySql";

// Proteger contra variables de entorno del sistema que puedan forzar SqlServer si queremos MySql por defecto
if (string.IsNullOrEmpty(builder.Configuration["DatabaseProvider"]))
{
    dbProviderName = "MySql";
}

// --- Gestión de Parámetros y Secretos (Separación Dev/Prod) ---
// Aspire leerá estos valores de:
// 1. dotnet user-secrets (Recomendado para Dev)
// 2. Variables de Entorno (Recomendado para Prod/Docker)
// 3. Azure Key Vault (Opcional en Prod)

// --- Configuración de Modo de Ejecución (Docker vs Local) ---
// Para forzar Docker: dotnet run --UseDocker=true (o en appsettings.json/user-secrets)
var useDocker = builder.Configuration.GetValue<bool>("UseDocker", false);

// Parámetros de Base de Datos
var systemConStr = builder.AddParameter("mysql-system-query", secret: true);
var identityConStr = builder.AddParameter("mysql-identity-query", secret: true);
var legacyConStr = builder.AddParameter("mysql-legacy-query", secret: true);

// Parámetros de Seguridad y Comunicaciones
var jwtSecret = builder.AddParameter("jwt-secret", secret: true);
var smtpUser = builder.AddParameter("smtp-user", secret: true);
var smtpPass = builder.AddParameter("smtp-pass", secret: true);

if (useDocker)
{
    var apiDocker = builder.AddDockerfile("api", "..", "SistemaSatHospitalario.WebAPI/Dockerfile")
        .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "endpoint-api");
    
    var frontendDocker = builder.AddDockerfile("frontend", "../SistemaSatHospitalario.Frontend")
        .WithHttpEndpoint(port: 4200, targetPort: 80, name: "http")
        .WithReference(apiDocker.GetEndpoint("endpoint-api"))
        .WithExternalHttpEndpoints()
        .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");

    AppHostConfiguration.ConfigureApiEnvironment(apiDocker, frontendDocker, builder.Configuration, dbProviderName, useDocker);
}
else
{
    var apiProject = builder.AddProject<Projects.SistemaSatHospitalario_WebAPI>("api")
        .WithHttpEndpoint(port: 8080, name: "endpoint-api")
        .WithExternalHttpEndpoints();

    var frontendNpm = builder.AddNpmApp("frontend", "../SistemaSatHospitalario.Frontend", "start:aspire")
        .WithHttpEndpoint(port: 4200, env: "PORT", name: "http")
        .WithExternalHttpEndpoints()
        .WithReference(apiProject.GetEndpoint("endpoint-api"))
        .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");

    AppHostConfiguration.ConfigureApiEnvironment(apiProject, frontendNpm, builder.Configuration, dbProviderName, useDocker);
}

builder.Build().Run();
