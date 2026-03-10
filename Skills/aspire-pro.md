---
name: aspire-pro
description: Patrones avanzados para la orquestación de aplicaciones distribuidas y cloud-native con .NET Aspire.
---

# Aspire Pro: Distributed Application Orchestration

.NET Aspire es un stack de herramientas opinionado para construir aplicaciones distribuidas observables y resilientes. Este skill se enfoca en patrones avanzados de orquestación y desarrollo cloud-native.

## Modelo Mental: El Conductor (AppHost)
El proyecto **AppHost** no es un servicio en sí mismo; es el *director de orquesta*. No ejecuta la lógica de negocio, sino que coordina cuándo inician los servicios, cómo se descubren entre sí y cómo se configuran los recursos de infraestructura (Bases de datos, Caches, Colas).

## Principios "Pro"
1. **Infrastructure as Code (C#)**: Define tu topología de red y dependencias usando C# puro. Evita configuraciones manuales en archivos YAML externos cuando sea posible.
2. **Observabilidad por Defecto**: Aspire integra OpenTelemetry de forma nativa. Cada petición entre servicios genera trazas automáticas visibles en el Dashboard.
3. **Orquestación Políglota**: Aunque el AppHost es .NET, puede orquestar contenedores de Python, Node.js, Go, etc., inyectando automáticamente las variables de entorno necesarias para el Service Discovery.

## Patrones de Orquestación Avanzados

### 1. Composición de Recursos
```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Recursos de infraestructura
var postgres = builder.AddPostgres("db-server").AddDatabase("ordersdb");
var redis = builder.AddRedis("cache-server");

// Servicios .NET
var api = builder.AddProject<Projects.Orders_API>("orders-api")
                 .WithReference(postgres)
                 .WithReference(redis);

// Contenedores de terceros o legacy
var legacyService = builder.AddContainer("legacy-api", "mcr.microsoft.com/dotnet/samples:aspnetapp")
                           .WithHttpEndpoint(targetPort: 8080);
```

### 2. Gestión del Ciclo de Vida y Resiliencia
Usa `.WaitFor()` para asegurar que los servicios dependientes solo inicien cuando su dependencia esté lista y saludable (Health Checks).
```csharp
var worker = builder.AddProject<Projects.WorkerService>("worker")
                    .WithReference(api)
                    .WaitFor(api); // Espera a que la API pase su Health Check
```

### 3. Service Discovery & Connection Strings
Aspire maneja automáticamente las cadenas de conexión. En el servicio receptor, simplemente usa:
```csharp
builder.AddSqlServerClient("ordersdb"); // Nombre coincidente con el AppHost
```

## Dashboard y Telemetría
El Dashboard de Aspire es la herramienta central durante el desarrollo:
- **Traces**: Visualiza el flujo de una petición a través de múltiples microservicios.
- **Metrics**: Monitorea el consumo de recursos y latencia en tiempo real.
- **Logs**: Centraliza los logs de todos los procesos orquestados en una sola vista.

## Comandos Críticos (CLI)
| Comando | Propósito |
|---|---|
| `aspire run` | Inicia toda la solución localmente (Orquestación + Dashboard). |
| `aspire init` | Agrega soporte de Aspire a una solución existente. |
| `aspire add <integration>` | Añade componentes (Redis, SQL, Mongo) pre-configurados. |
| `aspire publish` | Genera manifiestos para despliegue en Azure, Kubernetes o Docker. |

---
> [!TIP]
> Usa el proyecto `ServiceDefaults` para centralizar la configuración de resiliencia (Polly) y telemetría en todos tus microservicios de forma consistente.
