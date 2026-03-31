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

// --- Orquestación de la API (.NET) ---
var api = builder.AddProject<Projects.SistemaSatHospitalario_WebAPI>("api")
    .WithHttpsEndpoint(port: 7019, name: "endpoint-api")
    .WithEnvironment("DatabaseProvider", dbProviderName)
    // Inyectamos como variables de entorno directas para compatibilidad total
    .WithEnvironment("ConnectionStrings__mysql-system", systemConStr)
    .WithEnvironment("ConnectionStrings__mysql-identity", identityConStr)
    .WithEnvironment("ConnectionStrings__LegacyConnection", legacyConStr)
    .WithEnvironment("JwtConfig__Secret", jwtSecret)
    .WithEnvironment("EmailSettings__SmtpUser", smtpUser)
    .WithEnvironment("EmailSettings__SmtpPass", smtpPass)
    .WithEnvironment("JwtConfig__Issuer", builder.Configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI")
    .WithEnvironment("JwtConfig__Audience", builder.Configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA")
    .WithEnvironment("AllowedOrigins", "https://localhost:4200,http://localhost:4200");

// --- Orquestación del Frontend (Angular 19) ---
var frontend = builder.AddNpmApp("frontend", "../SistemaSatHospitalario.Frontend", "start")
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    // En Producción, el Endpoint debe ser el puerto expuesto del contenedor/servicio
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889")
    .PublishAsDockerFile();

builder.Build().Run();
