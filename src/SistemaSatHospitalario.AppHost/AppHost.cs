using Aspire.Hosting.ApplicationModel;
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

// Funciones de utilidad para cadenas de conexión
string ProcessConnStr(string? connStr) 
{
    if (string.IsNullOrEmpty(connStr)) return "";
    
    // Forzamos 127.0.0.1 en lugar de localhost para evitar problemas IPv6 en Windows
    var processed = connStr.Replace("localhost", useDocker ? "host.docker.internal" : "127.0.0.1")
                          .Replace("127.0.0.1", useDocker ? "host.docker.internal" : "127.0.0.1");

    // Aseguramos parámetros críticos para MySql 8.0+ y compatibilidad legacy
    if (!processed.Contains("AllowPublicKeyRetrieval", StringComparison.OrdinalIgnoreCase))
        processed += ";AllowPublicKeyRetrieval=True";
    if (!processed.Contains("SslMode", StringComparison.OrdinalIgnoreCase))
        processed += ";SslMode=None";
    if (!processed.Contains("Allow User Variables", StringComparison.OrdinalIgnoreCase))
        processed += ";Allow User Variables=True";
        
    return processed;
}

// Priorizamos los secretos directos (pueden ser reales) sobre los de Parameters (pueden ser placeholders)
string GetConnectionString(string key) => builder.Configuration[key] 
                                         ?? builder.Configuration[$"Parameters:{key}"] 
                                         ?? "";

// Función local para aplicar la configuración común a la API
void ConfigureApi(IResourceBuilder<IResourceWithEnvironment> resource, IResourceBuilder<IResourceWithEndpoints> frontendResource)
{
    var legacyConn = GetConnectionString("mysql-legacy-query");
    if (!string.IsNullOrEmpty(legacyConn))
    {
        resource.WithEnvironment("ConnectionStrings__LegacyConnection", ProcessConnStr(legacyConn));
    }
    
    resource.WithEnvironment("DatabaseProvider", dbProviderName)
        .WithEnvironment("ConnectionStrings__mysql-system", ProcessConnStr(GetConnectionString("mysql-system-query")))
        .WithEnvironment("ConnectionStrings__mysql-identity", ProcessConnStr(GetConnectionString("mysql-identity-query")))
        .WithEnvironment("JwtConfig__Secret", GetConnectionString("jwt-secret"))
        .WithEnvironment("EmailSettings__SmtpUser", GetConnectionString("smtp-user"))
        .WithEnvironment("EmailSettings__SmtpPass", GetConnectionString("smtp-pass"))
        .WithEnvironment("JwtConfig__Issuer", builder.Configuration["JwtConfig:Issuer"] ?? "SistemaSatHospitalarioAPI")
        .WithEnvironment("JwtConfig__Audience", builder.Configuration["JwtConfig:Audience"] ?? "SistemaSatHospitalario_PWA")
        // Whitelist both localhost and explicit IPs to avoid CORS issues with fixed IP binding
        .WithEnvironment("AllowedOrigins", $"{frontendResource.GetEndpoint("http")},http://localhost:4200,http://127.0.0.1:4200,http://0.0.0.0:4200,http://localhost:80,http://localhost");
}

if (useDocker)
{
    var apiDocker = builder.AddDockerfile("api", "..", "SistemaSatHospitalario.WebAPI/Dockerfile")
        .WithHttpEndpoint(port: 8080, targetPort: 8080, name: "endpoint-api");
    
    var frontendDocker = builder.AddDockerfile("frontend", "../SistemaSatHospitalario.Frontend")
        .WithHttpEndpoint(port: 4200, targetPort: 80, name: "http")
        .WithReference(apiDocker.GetEndpoint("endpoint-api"))
        .WithExternalHttpEndpoints()
        .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");

    ConfigureApi(apiDocker, frontendDocker);
}
else
{
    var apiProject = builder.AddProject<Projects.SistemaSatHospitalario_WebAPI>("api")
        .WithHttpEndpoint(port: 8080, name: "endpoint-api")
        .WithExternalHttpEndpoints();

    var frontendNpm = builder.AddNpmApp("frontend", "../SistemaSatHospitalario.Frontend", "start")
        .WithHttpEndpoint(port: 4200, env: "PORT", name: "http")
        .WithExternalHttpEndpoints()
        .WithReference(apiProject.GetEndpoint("endpoint-api"))
        .WithEnvironment("OTEL_EXPORTER_OTLP_ENDPOINT", "http://localhost:18889");

    ConfigureApi(apiProject, frontendNpm);
}

builder.Build().Run();
