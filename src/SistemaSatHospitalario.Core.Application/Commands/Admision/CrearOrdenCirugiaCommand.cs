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

                await EnsurePacienteExisteAsync(request.PacienteId, cancellationToken);
                var cuentaId = await ResolverCuentaIdAsync(request, cancellationToken);
                var orden = CrearOrden(request, cuentaId);

                await AsignarCirujanoPrincipalAsync(orden, request, cancellationToken);
                RegistrarAuditoriaInicial(orden, request);
                await AgregarRequisitosActivosAsync(orden, cancellationToken);

            _context.OrdenesCirugia.Add(orden);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Orden de cirugía {OrdenId} creada exitosamente.", orden.Id);
            return orden.Id;
        }

            private async Task EnsurePacienteExisteAsync(Guid pacienteId, CancellationToken cancellationToken)
            {
                var pacienteExiste = pacienteId != Guid.Empty && await _context.PacientesAdmision
                    .AsNoTracking()
                    .AnyAsync(patient => patient.Id == pacienteId, cancellationToken);

                if (!pacienteExiste)
                {
                    throw new InvalidOperationException("El paciente seleccionado no existe en el sistema hospitalario.");
                }
            }

            private async Task<Guid> ResolverCuentaIdAsync(CrearOrdenCirugiaCommand request, CancellationToken cancellationToken)
            {
                var cuentaId = request.CuentaServicioId ?? Guid.Empty;
                var cuentaExiste = cuentaId != Guid.Empty && await _context.CuentasServicios
                    .AnyAsync(cuenta => cuenta.Id == cuentaId, cancellationToken);

                if (cuentaExiste)
                {
                    return cuentaId;
                }

                var cuentaAbierta = await _context.CuentasServicios
                    .FirstOrDefaultAsync(cuenta => cuenta.PacienteId == request.PacienteId && cuenta.Estado == "Abierta", cancellationToken);
                if (cuentaAbierta != null)
                {
                    return cuentaAbierta.Id;
                }

                var nuevaCuenta = new CuentaServicios(
                    request.PacienteId,
                    ObtenerUsuarioCreacion(request),
                    "Cirugia",
                    convenioId: null,
                    areaClinicaId: request.AreaClinicaId,
                    subAreaClinica: "Pabellón Quirúrgico",
                    medicoId: request.MedicoId != Guid.Empty ? request.MedicoId : null);

                _context.CuentasServicios.Add(nuevaCuenta);
                await _context.SaveChangesAsync(cancellationToken);
                return nuevaCuenta.Id;
            }

            private static OrdenCirugia CrearOrden(CrearOrdenCirugiaCommand request, Guid cuentaId) => new(
                cuentaId,
                request.PacienteId,
                request.DescripcionCirugia,
                request.PrecioBaseUsd,
                request.MedicoId,
                request.FechaHoraProgramada,
                ObtenerUsuarioCreacion(request),
                request.AreaClinicaId,
                string.IsNullOrWhiteSpace(request.SalaQuirofano) ? "Quirófano 1" : request.SalaQuirofano,
                string.IsNullOrWhiteSpace(request.ModalidadAnestesia) ? "General" : request.ModalidadAnestesia,
                request.EsAlquilado,
                request.PrecioDerechoSalaUsd,
                request.SedeQuirofanoId);

            private async Task AsignarCirujanoPrincipalAsync(OrdenCirugia orden, CrearOrdenCirugiaCommand request, CancellationToken cancellationToken)
            {
                if (request.MedicoId == Guid.Empty)
                {
                    return;
                }

                var medico = await _context.Medicos
                    .Include(item => item.Especialidad)
                    .FirstOrDefaultAsync(item => item.Id == request.MedicoId, cancellationToken);
                var especialidadId = medico?.EspecialidadId ?? Guid.Empty;

                if (especialidadId == Guid.Empty)
                {
                    especialidadId = (await _context.Especialidades.FirstOrDefaultAsync(cancellationToken))?.Id ?? Guid.Empty;
                }

                if (especialidadId != Guid.Empty)
                {
                    orden.AsignarMedicoHonorario(request.MedicoId, especialidadId, request.PrecioBaseUsd, esPrincipal: true);
                }
            }

            private static void RegistrarAuditoriaInicial(OrdenCirugia orden, CrearOrdenCirugiaCommand request)
            {
                orden.AgregarLog(request.UsuarioCreacion, CirugiaEventoConstants.Creacion,
                    $"Cirugía '{request.DescripcionCirugia}' programada para {request.FechaHoraProgramada:yyyy-MM-dd HH:mm} en {orden.SalaQuirofano}.");
                orden.AgregarHistorialObservacion(
                    $"Programación inicial en {orden.SalaQuirofano} para {request.FechaHoraProgramada:dd/MM/yyyy HH:mm}.",
                    "ProgramacionInicial",
                    request.UsuarioCreacion);
            }

            private async Task AgregarRequisitosActivosAsync(OrdenCirugia orden, CancellationToken cancellationToken)
            {
                var requisitos = await _context.RequisitosCirugia
                    .Where(requisito => requisito.EsActivo)
                    .ToListAsync(cancellationToken);

                foreach (var requisito in requisitos)
                {
                    orden.AgregarRequisito(requisito.Id, cumplido: false);
                }
            }

            private static string ObtenerUsuarioCreacion(CrearOrdenCirugiaCommand request) =>
                string.IsNullOrWhiteSpace(request.UsuarioCreacion) ? "admin" : request.UsuarioCreacion;
    }
}
