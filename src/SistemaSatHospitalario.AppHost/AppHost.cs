using Aspire.Hosting.ApplicationModel;
using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

// Proveedor de Base de Datos (Estrategia Aspire Pro)
var dbProviderName = builder.Configuration["DatabaseProvider"] ?? "MySql";

// Parámetros Secretos para las Bases de Datos (MySQL Local)
var systemConStr = builder.AddParameter("mysql-system-query", secret: true);
var identityConStr = builder.AddParameter("mysql-identity-query", secret: true);
var legacyConStr = builder.AddParameter("mysql-legacy-query", secret: true);

// Parámetros Secretos de la Aplicación (Backend)
var effectiveJwtSecret = builder.Configuration["Parameters:jwt-secret"] ?? "DevelopmentSecretWithAtLeast32Chars!!!";
var jwtSecret = builder.AddParameter("jwt-secret", effectiveJwtSecret, secret: true);
var smtpUser = builder.AddParameter("smtp-user", secret: true);
var smtpPass = builder.AddParameter("smtp-pass", secret: true);

// Orquestación de la API
// Inyectamos como variables de entorno directas con el prefijo ConnectionStrings__
// Esto evita el error de formato 'uri' de los componentes de Aspire al tratar con recursos externos.
var api = builder.AddProject<Projects.SistemaSatHospitalario_WebAPI>("api")
    .WithHttpsEndpoint(port: 7019, name: "endpoint-api")
    .WithEnvironment("ConnectionStrings__mysql-system", systemConStr)
    .WithEnvironment("ConnectionStrings__mysql-identity", identityConStr)
    .WithEnvironment("ConnectionStrings__LegacyConnection", legacyConStr)
    .WithEnvironment("JwtConfig__Secret", effectiveJwtSecret)
    .WithEnvironment("EmailSettings__SmtpUser", smtpUser)
    .WithEnvironment("EmailSettings__SmtpPass", smtpPass)
    .WithEnvironment("DatabaseProvider", dbProviderName);

// Orquestación del Frontend (Angular 19)
builder.AddNpmApp("frontend", "../SistemaSatHospitalario.Frontend", "start")
    .WithReference(api)
    .WithHttpEndpoint(env: "PORT")
    .WithExternalHttpEndpoints()
    .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889")
    .PublishAsDockerFile();

builder.Build().Run();
