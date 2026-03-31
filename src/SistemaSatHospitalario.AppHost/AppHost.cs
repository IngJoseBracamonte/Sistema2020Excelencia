using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// --- Configuración Global de Infraestructura ---
var dbProviderName = builder.Configuration["DatabaseProvider"] ?? "MySql";

// --- Gestión de Parámetros y Secretos (Separación Dev/Prod) ---
// Aspire leerá estos valores de:
// 1. dotnet user-secrets (Recomendado para Dev)
// 2. Variables de Entorno (Recomendado para Prod/Docker)
// 3. Azure Key Vault (Opcional en Prod)

// Parámetros de Base de Datos (Deben incluir SslMode=Required o VerifyFull en Producción)
var systemConStr = builder.AddParameter("mysql-system-query", secret: true);
var identityConStr = builder.AddParameter("mysql-identity-query", secret: true);
var legacyConStr = builder.AddParameter("mysql-legacy-query", secret: true);

// Parámetros de Seguridad y Comunicaciones
var jwtSecret = builder.AddParameter("jwt-secret", secret: true);
var smtpUser = builder.AddParameter("smtp-user", secret: true);
var smtpPass = builder.AddParameter("smtp-pass", secret: true);

// --- Orquestación de la API (.NET en Docker) ---
var api = builder.AddDockerfile("api", "..", "SistemaSatHospitalario.WebAPI/Dockerfile")
    .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "endpoint-api")
    .WithEnvironment("DatabaseProvider", dbProviderName)
    // --- Traducción Dinámica de Cadenas de Conexión para Docker ---
    .WithEnvironment("ConnectionStrings__mysql-system", 
        (builder.Configuration["Parameters:mysql-system-query"] ?? "").Replace("localhost", "host.docker.internal").Replace("127.0.0.1", "host.docker.internal"))
    .WithEnvironment("ConnectionStrings__mysql-identity", 
        (builder.Configuration["Parameters:mysql-identity-query"] ?? "").Replace("localhost", "host.docker.internal").Replace("127.0.0.1", "host.docker.internal"))
    .WithEnvironment("ConnectionStrings__LegacyConnection", 
        (builder.Configuration["Parameters:mysql-legacy-query"] ?? "").Replace("localhost", "host.docker.internal").Replace("127.0.0.1", "host.docker.internal"))
    // --------------------------------------------------------------
    .WithEnvironment("JwtConfig__Secret", jwtSecret)
    .WithEnvironment("EmailSettings__SmtpUser", smtpUser)
    .WithEnvironment("EmailSettings__SmtpPass", smtpPass)
    .WithEnvironment("JwtConfig__Issuer", builder.Configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI")
    .WithEnvironment("JwtConfig__Audience", builder.Configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA")
    .WithEnvironment("AllowedOrigins", "http://localhost:4200,http://localhost:80,http://localhost");

// --- Orquestación del Frontend (Angular en Docker) ---
var frontend = builder.AddDockerfile("frontend", "../SistemaSatHospitalario.Frontend")
    .WithReference(api.GetEndpoint("endpoint-api"))
    .WithHttpEndpoint(port: 4200, targetPort: 80, name: "http")
    .WithExternalHttpEndpoints()
    // En Producción, el Endpoint debe ser el puerto expuesto del contenedor/servicio
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");

builder.Build().Run();
