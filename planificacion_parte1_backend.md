# PLANIFICACIÓN: Módulo Honorarios Pro — PARTE 1 (Backend)

## OBJETIVO
Implementar un sistema de asignación de honorarios médicos con 2 modos:
1. **DEFAULT:** Configuración global que pre-asigna un médico por categoría (ej: "Informes → Dr. Jorge").
2. **MANUAL:** Panel tipo CxC donde el usuario asigna médico a cada servicio individualmente.
3. **AUDITORÍA:** Registro de TODA acción (quién asignó, a qué hora, qué cambió).

---

## PASO 1: Crear Entidad HonorarioConfig

**Archivo:** `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/HonorarioConfig.cs`

```csharp
using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class HonorarioConfig
    {
        public Guid Id { get; private set; }
        public string CategoriaServicio { get; private set; } // RX, INFORME, CITOLOGIA, BIOPSIA, CONSULTA
        public Guid? MedicoDefaultId { get; private set; }
        public virtual Medico MedicoDefault { get; private set; }
        public string UsuarioConfiguro { get; private set; }
        public DateTime FechaConfiguracion { get; private set; }
        public string? NotasConfig { get; private set; }

        protected HonorarioConfig() { }

        public HonorarioConfig(string categoriaServicio, string usuario)
        {
            Id = Guid.NewGuid();
            CategoriaServicio = categoriaServicio ?? throw new ArgumentNullException(nameof(categoriaServicio));
            UsuarioConfiguro = usuario;
            FechaConfiguracion = DateTime.UtcNow;
        }

        public void AsignarMedicoDefault(Guid medicoId, string usuario, string? notas = null)
        {
            MedicoDefaultId = medicoId;
            UsuarioConfiguro = usuario;
            FechaConfiguracion = DateTime.UtcNow;
            NotasConfig = notas;
        }

        public void LimpiarMedicoDefault(string usuario)
        {
            MedicoDefaultId = null;
            UsuarioConfiguro = usuario;
            FechaConfiguracion = DateTime.UtcNow;
            NotasConfig = "Limpiado por " + usuario;
        }
    }
}
```

---

## PASO 2: Crear Entidad LogAsignacionHonorario

**Archivo:** `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/LogAsignacionHonorario.cs`

```csharp
using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class LogAsignacionHonorario
    {
        public Guid Id { get; private set; }
        public Guid DetalleServicioId { get; private set; }
        public string NombreServicio { get; private set; }
        public string TipoAccion { get; private set; } // ASIGNACION_MANUAL, ASIGNACION_DEFAULT, REASIGNACION, CONFIG_DEFAULT
        public Guid? MedicoAnteriorId { get; private set; }
        public string? MedicoAnteriorNombre { get; private set; }
        public Guid? MedicoNuevoId { get; private set; }
        public string? MedicoNuevoNombre { get; private set; }
        public string UsuarioOperador { get; private set; }
        public DateTime FechaAccion { get; private set; }
        public string? Observaciones { get; private set; }

        protected LogAsignacionHonorario() { }

        public LogAsignacionHonorario(
            Guid detalleServicioId, string nombreServicio, string tipoAccion,
            Guid? medicoAnteriorId, string? medicoAnteriorNombre,
            Guid? medicoNuevoId, string? medicoNuevoNombre,
            string usuarioOperador, string? observaciones = null)
        {
            Id = Guid.NewGuid();
            DetalleServicioId = detalleServicioId;
            NombreServicio = nombreServicio;
            TipoAccion = tipoAccion;
            MedicoAnteriorId = medicoAnteriorId;
            MedicoAnteriorNombre = medicoAnteriorNombre;
            MedicoNuevoId = medicoNuevoId;
            MedicoNuevoNombre = medicoNuevoNombre;
            UsuarioOperador = usuarioOperador;
            FechaAccion = DateTime.UtcNow;
            Observaciones = observaciones;
        }
    }
}
```

---

## PASO 3: Modificar DetalleServicioCuenta

**Archivo:** `src/SistemaSatHospitalario.Core.Domain/Entities/Admision/DetalleServicioCuenta.cs`

**Añadir después de la línea `public string? LegacyMappingId`:**
```csharp
public Guid? MedicoResponsableId { get; private set; }
public string? CategoriaHonorario { get; private set; }
```

**Añadir al final de la clase (antes del cierre `}`):**
```csharp
public void AsignarMedicoResponsable(Guid medicoId, string categoria)
{
    MedicoResponsableId = medicoId;
    CategoriaHonorario = categoria;
}
```

---

## PASO 4: Registrar en IApplicationDbContext

**Archivo:** `src/SistemaSatHospitalario.Core.Application/Common/Interfaces/IApplicationDbContext.cs`

**Añadir después de la línea `DbSet<Notification> Notifications { get; }`:**
```csharp
DbSet<HonorarioConfig> HonorariosConfig { get; }
DbSet<LogAsignacionHonorario> LogsAsignacionHonorario { get; }
```

---

## PASO 5: Registrar en SatHospitalarioDbContext

**Archivo:** `src/SistemaSatHospitalario.Infrastructure/Persistence/Contexts/SatHospitalarioDbContext.cs`

**Añadir DbSets después de `DbSet<Notification> Notifications`:**
```csharp
public DbSet<HonorarioConfig> HonorariosConfig { get; set; }
public DbSet<LogAsignacionHonorario> LogsAsignacionHonorario { get; set; }
```

**Añadir en OnModelCreating (seguir el patrón existente de `entity.ToTable()`):**
```csharp
builder.Entity<HonorarioConfig>(entity =>
{
    entity.ToTable("HonorariosConfig");
    entity.HasKey(h => h.Id);
    entity.Property(h => h.CategoriaServicio).IsRequired().HasMaxLength(50);
    entity.HasOne(h => h.MedicoDefault).WithMany().HasForeignKey(h => h.MedicoDefaultId).OnDelete(DeleteBehavior.SetNull);
    entity.HasIndex(h => h.CategoriaServicio).IsUnique();
});

builder.Entity<LogAsignacionHonorario>(entity =>
{
    entity.ToTable("LogsAsignacionHonorario");
    entity.HasKey(l => l.Id);
    entity.Property(l => l.TipoAccion).IsRequired().HasMaxLength(50);
    entity.HasIndex(l => l.FechaAccion);
    entity.HasIndex(l => l.DetalleServicioId);
});

builder.Entity<DetalleServicioCuenta>()
    .HasOne<Medico>()
    .WithMany()
    .HasForeignKey(d => d.MedicoResponsableId)
    .OnDelete(DeleteBehavior.SetNull);
```

---

## PASO 6: Generar Migración EF Core

Ejecutar desde `src/SistemaSatHospitalario.WebAPI`:
```bash
dotnet ef migrations add AddHonorarioConfigYAsignacion --project ../SistemaSatHospitalario.Infrastructure --startup-project . --context SatHospitalarioDbContext
```

---

## PASO 7: Crear Comandos y Queries

### 7A: SetHonorarioDefaultCommand.cs

**Archivo:** `src/SistemaSatHospitalario.Core.Application/Commands/Admin/SetHonorarioDefaultCommand.cs`

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admin
{
    public class SetHonorarioDefaultCommand : IRequest<Unit>
    {
        public string CategoriaServicio { get; set; } = string.Empty;
        public Guid? MedicoId { get; set; }
        public string? Observaciones { get; set; }
    }

    public class SetHonorarioDefaultCommandHandler : IRequestHandler<SetHonorarioDefaultCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public SetHonorarioDefaultCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(SetHonorarioDefaultCommand request, CancellationToken ct)
        {
            var config = await _context.HonorariosConfig
                .FirstOrDefaultAsync(h => h.CategoriaServicio == request.CategoriaServicio, ct);

            var usuario = _currentUser.UserName ?? "Sistema";

            if (config == null)
            {
                config = new HonorarioConfig(request.CategoriaServicio, usuario);
                _context.HonorariosConfig.Add(config);
            }

            if (request.MedicoId.HasValue)
                config.AsignarMedicoDefault(request.MedicoId.Value, usuario, request.Observaciones);
            else
                config.LimpiarMedicoDefault(usuario);

            // Log de auditoría
            string? medicoNombre = null;
            if (request.MedicoId.HasValue)
            {
                var medico = await _context.Medicos.FindAsync(new object[] { request.MedicoId.Value }, ct);
                medicoNombre = medico?.Nombre;
            }

            var log = new LogAsignacionHonorario(
                Guid.Empty, request.CategoriaServicio, "CONFIG_DEFAULT",
                null, null,
                request.MedicoId, medicoNombre,
                usuario, request.Observaciones);
            _context.LogsAsignacionHonorario.Add(log);

            await _context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
```

### 7B: AsignarMedicoAServicioCommand.cs

**Archivo:** `src/SistemaSatHospitalario.Core.Application/Commands/Admin/AsignarMedicoAServicioCommand.cs`

```csharp
using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admin
{
    public class AsignarMedicoAServicioCommand : IRequest<Unit>
    {
        public Guid DetalleServicioId { get; set; }
        public Guid MedicoId { get; set; }
        public string CategoriaHonorario { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class AsignarMedicoAServicioCommandHandler : IRequestHandler<AsignarMedicoAServicioCommand, Unit>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUser;

        public AsignarMedicoAServicioCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
        {
            _context = context;
            _currentUser = currentUser;
        }

        public async Task<Unit> Handle(AsignarMedicoAServicioCommand request, CancellationToken ct)
        {
            var detalle = await _context.DetallesServicioCuenta.FindAsync(new object[] { request.DetalleServicioId }, ct);
            if (detalle == null) throw new InvalidOperationException("Detalle no encontrado.");

            var medico = await _context.Medicos.FindAsync(new object[] { request.MedicoId }, ct);
            if (medico == null) throw new InvalidOperationException("Médico no encontrado.");

            // Guardar estado anterior para auditoría
            var medicoAnteriorId = detalle.MedicoResponsableId;
            string? medicoAnteriorNombre = null;
            if (medicoAnteriorId.HasValue)
            {
                var anterior = await _context.Medicos.FindAsync(new object[] { medicoAnteriorId.Value }, ct);
                medicoAnteriorNombre = anterior?.Nombre;
            }

            var tipoAccion = medicoAnteriorId.HasValue ? "REASIGNACION" : "ASIGNACION_MANUAL";

            detalle.AsignarMedicoResponsable(request.MedicoId, request.CategoriaHonorario);

            var log = new LogAsignacionHonorario(
                request.DetalleServicioId, detalle.Descripcion, tipoAccion,
                medicoAnteriorId, medicoAnteriorNombre,
                request.MedicoId, medico.Nombre,
                _currentUser.UserName ?? "Sistema", request.Observaciones);
            _context.LogsAsignacionHonorario.Add(log);

            await _context.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
```

### 7C: GetServiciosSinAsignarQuery.cs

**Archivo:** `src/SistemaSatHospitalario.Core.Application/Queries/Admin/GetServiciosSinAsignarQuery.cs`

```csharp
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Queries.Admin
{
    public class GetServiciosSinAsignarQuery : IRequest<List<ServicioSinAsignarDto>>
    {
        public DateTime? FechaDesde { get; set; }
        public DateTime? FechaHasta { get; set; }
        public string? EstadoFiltro { get; set; } // PENDIENTE, ASIGNADO, TODOS
    }

    public class ServicioSinAsignarDto
    {
        public Guid DetalleId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string TipoServicio { get; set; } = string.Empty;
        public decimal Honorario { get; set; }
        public string PacienteNombre { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public Guid? MedicoAsignadoId { get; set; }
        public string? MedicoAsignadoNombre { get; set; }
        public string? CategoriaHonorario { get; set; }
        public bool EsAutoAsignado { get; set; }
    }

    public class GetServiciosSinAsignarQueryHandler : IRequestHandler<GetServiciosSinAsignarQuery, List<ServicioSinAsignarDto>>
    {
        private readonly IApplicationDbContext _context;

        public GetServiciosSinAsignarQueryHandler(IApplicationDbContext context) { _context = context; }

        public async Task<List<ServicioSinAsignarDto>> Handle(GetServiciosSinAsignarQuery request, CancellationToken ct)
        {
            var excluir = new[] { "Laboratorio", "LAB", "INSUMO", "Insumo" };
            var query = _context.DetallesServicioCuenta
                .Include(d => d.CuentaServicio).ThenInclude(c => c.Paciente)
                .Where(d => !excluir.Contains(d.TipoServicio) && d.Honorario > 0)
                .AsQueryable();

            if (request.FechaDesde.HasValue)
                query = query.Where(d => d.FechaCarga >= request.FechaDesde.Value.Date);
            if (request.FechaHasta.HasValue)
                query = query.Where(d => d.FechaCarga <= request.FechaHasta.Value.Date.AddDays(1));

            if (request.EstadoFiltro == "PENDIENTE")
                query = query.Where(d => d.MedicoResponsableId == null);
            else if (request.EstadoFiltro == "ASIGNADO")
                query = query.Where(d => d.MedicoResponsableId != null);

            var data = await query.OrderByDescending(d => d.FechaCarga)
                .Select(d => new ServicioSinAsignarDto
                {
                    DetalleId = d.Id,
                    Descripcion = d.Descripcion,
                    TipoServicio = d.TipoServicio,
                    Honorario = d.Honorario,
                    PacienteNombre = d.CuentaServicio.Paciente.NombreCompleto,
                    FechaCarga = d.FechaCarga,
                    MedicoAsignadoId = d.MedicoResponsableId,
                    CategoriaHonorario = d.CategoriaHonorario
                }).ToListAsync(ct);

            // Enrich with medico names
            var medicoIds = data.Where(d => d.MedicoAsignadoId.HasValue).Select(d => d.MedicoAsignadoId.Value).Distinct().ToList();
            var medicos = await _context.Medicos.Where(m => medicoIds.Contains(m.Id)).ToDictionaryAsync(m => m.Id, m => m.Nombre, ct);
            foreach (var item in data)
            {
                if (item.MedicoAsignadoId.HasValue && medicos.ContainsKey(item.MedicoAsignadoId.Value))
                    item.MedicoAsignadoNombre = medicos[item.MedicoAsignadoId.Value];
            }

            return data;
        }
    }
}
```

**NOTA:** `DetalleServicioCuenta` necesita una propiedad de navegación. Añadir en la entidad:
```csharp
public virtual CuentaServicios CuentaServicio { get; private set; }
```
Y `PacienteAdmision` necesita `NombreCompleto`. Si no existe, usar `Nombre + " " + Apellido` o el campo que ya exista.

---

## PASO 8: Crear API Controllers

### 8A: HonorarioConfigController.cs

**Archivo:** `src/SistemaSatHospitalario.WebAPI/Controllers/Admin/HonorarioConfigController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admin;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admin
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles)]
    [ApiController]
    [Route("api/[controller]")]
    public class HonorarioConfigController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;

        public HonorarioConfigController(IMediator mediator, IApplicationDbContext context)
        {
            _mediator = mediator;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var configs = await _context.HonorariosConfig
                .Include(h => h.MedicoDefault)
                .Select(h => new {
                    h.Id, h.CategoriaServicio,
                    h.MedicoDefaultId,
                    MedicoDefaultNombre = h.MedicoDefault != null ? h.MedicoDefault.Nombre : null,
                    h.UsuarioConfiguro, h.FechaConfiguracion, h.NotasConfig
                }).ToListAsync();
            return Ok(configs);
        }

        [HttpPut("{categoria}")]
        public async Task<IActionResult> SetDefault(string categoria, [FromBody] SetHonorarioDefaultCommand command)
        {
            command.CategoriaServicio = categoria;
            await _mediator.Send(command);
            return Ok(new { message = "Configuración actualizada" });
        }

        [HttpGet("logs")]
        public async Task<IActionResult> GetLogs([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta)
        {
            var query = _context.LogsAsignacionHonorario.AsQueryable();
            if (desde.HasValue) query = query.Where(l => l.FechaAccion >= desde.Value.Date);
            if (hasta.HasValue) query = query.Where(l => l.FechaAccion <= hasta.Value.Date.AddDays(1));
            var logs = await query.OrderByDescending(l => l.FechaAccion).Take(200).ToListAsync();
            return Ok(logs);
        }
    }
}
```

### 8B: AsignacionHonorariosController.cs

**Archivo:** `src/SistemaSatHospitalario.WebAPI/Controllers/Admin/AsignacionHonorariosController.cs`

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaSatHospitalario.Core.Application.Commands.Admin;
using SistemaSatHospitalario.Core.Application.Queries.Admin;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.WebAPI.Controllers.Admin
{
    [Authorize(Roles = AuthorizationConstants.AdminRoles)]
    [ApiController]
    [Route("api/[controller]")]
    public class AsignacionHonorariosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AsignacionHonorariosController(IMediator mediator) { _mediator = mediator; }

        [HttpGet("pendientes")]
        public async Task<IActionResult> GetPendientes([FromQuery] DateTime? desde, [FromQuery] DateTime? hasta, [FromQuery] string? estado)
        {
            var result = await _mediator.Send(new GetServiciosSinAsignarQuery
            {
                FechaDesde = desde, FechaHasta = hasta, EstadoFiltro = estado ?? "TODOS"
            });
            return Ok(result);
        }

        [HttpPost("asignar")]
        public async Task<IActionResult> Asignar([FromBody] AsignarMedicoAServicioCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { message = "Médico asignado correctamente" });
        }
    }
}
```

---

## PASO 9: Modificar CargarServicioACuentaCommand (Auto-asignación)

**Archivo:** `src/SistemaSatHospitalario.Core.Application/Commands/Admision/CargarServicioACuentaCommand.cs`

**Después de la línea 120** (`await _repository.GuardarCambiosAsync`), **ANTES del `return`**, añadir:

```csharp
// Auto-asignación de Médico Responsable desde HonorarioConfig
if (detalle.Honorario > 0)
{
    string? categoriaMapeada = MapearCategoria(request.TipoServicio);
    if (categoriaMapeada != null)
    {
        var config = await _context.HonorariosConfig
            .FirstOrDefaultAsync(h => h.CategoriaServicio == categoriaMapeada, cancellationToken);
        if (config?.MedicoDefaultId != null)
        {
            detalle.AsignarMedicoResponsable(config.MedicoDefaultId.Value, categoriaMapeada);
            var medicoNombre = (await _context.Medicos.FindAsync(new object[] { config.MedicoDefaultId.Value }, cancellationToken))?.Nombre;
            _context.LogsAsignacionHonorario.Add(new LogAsignacionHonorario(
                detalle.Id, request.Descripcion, "ASIGNACION_DEFAULT",
                null, null, config.MedicoDefaultId.Value, medicoNombre,
                request.UsuarioCarga, "Auto-asignado por configuración"));
        }
    }
}
```

**Añadir este método privado en la clase handler:**
```csharp
private static string? MapearCategoria(string tipoServicio)
{
    var tipo = tipoServicio?.ToUpperInvariant();
    return tipo switch
    {
        "RX" or "IMAGEN" or "RADIOLOGIA" => "RX",
        "INFORME" => "INFORME",
        "CITOLOGIA" or "GINECOLOGIA" => "CITOLOGIA",
        "BIOPSIA" or "PATOLOGIA" => "BIOPSIA",
        "MEDICO" or "CONSULTA" or "CONS" => "CONSULTA",
        _ => null
    };
}
```

**Añadir los usings necesarios en el archivo:**
```csharp
using SistemaSatHospitalario.Core.Domain.Entities.Admision; // para LogAsignacionHonorario
```

---

## VERIFICACIÓN BACKEND

Ejecutar:
```bash
cd src/SistemaSatHospitalario.WebAPI
dotnet build
```
Debe compilar con 0 errores. Si hay warnings nullable, ignorarlos.
