using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    /// <summary>
    /// Comando para crear y agendar una nueva orden de cirugía.
    /// <summary>
    /// Comando para crear y agendar una nueva orden de cirugía.
    /// </summary>
    public class CrearOrdenCirugiaCommand : IRequest<Guid>
    {
        public Guid? CuentaServicioId { get; set; }
        public Guid PacienteId { get; set; }
        public string DescripcionCirugia { get; set; } = string.Empty;
        public decimal PrecioBaseUsd { get; set; }
        public decimal PrecioDerechoSalaUsd { get; set; }
        public Guid MedicoId { get; set; }
        public DateTime FechaHoraProgramada { get; set; }
        public string SalaQuirofano { get; set; } = "Quirófano 1";
        public string ModalidadAnestesia { get; set; } = "General";
        public bool EsAlquilado { get; set; }
        public Guid? AreaClinicaId { get; set; }
        public Guid? SedeQuirofanoId { get; set; }
        public string UsuarioCreacion { get; set; } = string.Empty;
    }

    public class CrearOrdenCirugiaCommandHandler : IRequestHandler<CrearOrdenCirugiaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CrearOrdenCirugiaCommandHandler> _logger;

        public CrearOrdenCirugiaCommandHandler(IApplicationDbContext context, ILogger<CrearOrdenCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Guid> Handle(CrearOrdenCirugiaCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creando orden de cirugía: {Descripcion} para Paciente {PacienteId}",
                request.DescripcionCirugia, request.PacienteId);

            // 1. Resolver o Crear CuentaServicios vinculada al paciente
            Guid cuentaId = request.CuentaServicioId ?? Guid.Empty;
            bool cuentaExiste = cuentaId != Guid.Empty && await _context.CuentasServicios.AnyAsync(c => c.Id == cuentaId, cancellationToken);

            if (!cuentaExiste)
            {
                var cuentaAbierta = await _context.CuentasServicios
                    .FirstOrDefaultAsync(c => c.PacienteId == request.PacienteId && c.Estado == "Abierta", cancellationToken);

                if (cuentaAbierta != null)
                {
                    cuentaId = cuentaAbierta.Id;
                }
                else
                {
                    var nuevaCuenta = new CuentaServicios(
                        request.PacienteId,
                        string.IsNullOrWhiteSpace(request.UsuarioCreacion) ? "admin" : request.UsuarioCreacion,
                        "Cirugia",
                        convenioId: null,
                        areaClinicaId: request.AreaClinicaId,
                        subAreaClinica: "Pabellón Quirúrgico",
                        medicoId: request.MedicoId != Guid.Empty ? request.MedicoId : (Guid?)null
                    );

                    _context.CuentasServicios.Add(nuevaCuenta);
                    await _context.SaveChangesAsync(cancellationToken);
                    cuentaId = nuevaCuenta.Id;
                }
            }

            var orden = new OrdenCirugia(
                cuentaId,
                request.PacienteId,
                request.DescripcionCirugia,
                request.PrecioBaseUsd,
                request.MedicoId,
                request.FechaHoraProgramada,
                string.IsNullOrWhiteSpace(request.UsuarioCreacion) ? "admin" : request.UsuarioCreacion,
                request.AreaClinicaId,
                string.IsNullOrWhiteSpace(request.SalaQuirofano) ? "Quirófano 1" : request.SalaQuirofano,
                string.IsNullOrWhiteSpace(request.ModalidadAnestesia) ? "General" : request.ModalidadAnestesia,
                request.EsAlquilado,
                request.PrecioDerechoSalaUsd,
                request.SedeQuirofanoId);

            // Asignar Cirujano Principal a la lista de honorarios del equipo médico
            if (request.MedicoId != Guid.Empty)
            {
                var medico = await _context.Medicos
                    .Include(m => m.Especialidad)
                    .FirstOrDefaultAsync(m => m.Id == request.MedicoId, cancellationToken);

                var especialidadId = medico?.EspecialidadId ?? Guid.Empty;
                if (especialidadId == Guid.Empty)
                {
                    var primeraEsp = await _context.Especialidades.FirstOrDefaultAsync(cancellationToken);
                    if (primeraEsp != null) especialidadId = primeraEsp.Id;
                }

                if (especialidadId != Guid.Empty)
                {
                    orden.AsignarMedicoHonorario(
                        request.MedicoId,
                        especialidadId,
                        honorarioUsd: request.PrecioBaseUsd,
                        esPrincipal: true);
                }
            }

            // Log de auditoría de creación
            orden.AgregarLog(request.UsuarioCreacion, CirugiaEventoConstants.Creacion,
                $"Cirugía '{request.DescripcionCirugia}' programada para {request.FechaHoraProgramada:yyyy-MM-dd HH:mm} en {orden.SalaQuirofano}.");

            orden.AgregarHistorialObservacion(
                $"Programación inicial en {orden.SalaQuirofano} para {request.FechaHoraProgramada:dd/MM/yyyy HH:mm}.",
                "ProgramacionInicial",
                request.UsuarioCreacion);

            // Cargar requisitos activos desde el catálogo maestro DB-Driven
            var reqsActivos = await _context.RequisitosCirugia
                .Where(r => r.EsActivo)
                .ToListAsync(cancellationToken);

            foreach (var req in reqsActivos)
            {
                orden.AgregarRequisito(req.Id, cumplido: false);
            }

            _context.OrdenesCirugia.Add(orden);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Orden de cirugía {OrdenId} creada exitosamente.", orden.Id);
            return orden.Id;
        }
    }
}
