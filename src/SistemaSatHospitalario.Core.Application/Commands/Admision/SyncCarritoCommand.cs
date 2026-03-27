using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class SyncCarritoCommand : IRequest<SyncCarritoResult>
    {
        public int PacienteId { get; set; }
        public string TipoIngreso { get; set; } = "Particular";
        public int? ConvenioId { get; set; }
        public string UsuarioCarga { get; set; } = string.Empty;
        public List<ServicioCarritoDto> Items { get; set; } = new();
    }

    public class ServicioCarritoDto
    {
        public Guid ServicioId { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public int Cantidad { get; set; } = 1;
        public string TipoServicio { get; set; } = string.Empty;
        
        // Datos para Citas
        public Guid? MedicoId { get; set; }
        public DateTime? HoraCita { get; set; }
        public string? Comentario { get; set; }
    }

    public record SyncCarritoResult(Guid CuentaId, List<DetalleSyncDto> Detalles);
    public record DetalleSyncDto(Guid ServicioId, Guid DetalleId);

    public class SyncCarritoCommandHandler : IRequestHandler<SyncCarritoCommand, SyncCarritoResult>
    {
        private readonly IBillingRepository _repository;
        private readonly IApplicationDbContext _context;

        public SyncCarritoCommandHandler(IBillingRepository repository, IApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }

        public async Task<SyncCarritoResult> Handle(SyncCarritoCommand request, CancellationToken ct)
        {
            // 1. Asegurar Existencia Local del Paciente (V11.0 Sync Pro)
            // Traducimos el ID de la base de datos vieja a la identidad GUID de la nueva
            var paciente = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                _context.PacientesAdmision, p => p.IdPacienteLegacy == request.PacienteId, ct);

            if (paciente == null)
            {
                // Auto-Stub: Registro mínimo para integridad referencial
                paciente = new PacienteAdmision("LEGACY", $"Paciente del Legado {request.PacienteId}", "", request.PacienteId);
                _context.PacientesAdmision.Add(paciente);
                // No guardamos aún, lo haremos al final con el GuardarCambios del repo (misma transacción EF)
            }

            // 2. Sincronizar Cuenta usando el GUID local
            var cuenta = await _repository.ObtenerCuentaAbiertaPorPacienteAsync(paciente.Id, ct);
            if (cuenta == null)
            {
                cuenta = new CuentaServicios(paciente.Id, request.UsuarioCarga, request.TipoIngreso, request.ConvenioId);
                await _repository.AgregarCuentaAsync(cuenta, ct);
            }

            var detallesRes = new List<DetalleSyncDto>();

            foreach (var item in request.Items)
            {
                if (EsTipoConsulta(item.TipoServicio) && item.MedicoId.HasValue && item.HoraCita.HasValue)
                {
                    await ProcesarCitaMedicaAsync(item, paciente.Id, cuenta.Id, ct);
                }

                var detalle = cuenta.AgregarServicio(
                    item.ServicioId, 
                    item.Descripcion, 
                    item.Precio, 
                    item.Cantidad, 
                    item.TipoServicio, 
                    request.UsuarioCarga);
                
                detallesRes.Add(new DetalleSyncDto(item.ServicioId, detalle.Id));
            }

            await _repository.GuardarCambiosAsync(ct);

            return new SyncCarritoResult(cuenta.Id, detallesRes);
        }

        private async Task ProcesarCitaMedicaAsync(ServicioCarritoDto item, Guid pacienteId, Guid cuentaId, CancellationToken ct)
        {
            // Normalización de Horario (V3.1)
            var horaNormalizada = new DateTime(
                item.HoraCita!.Value.Year, item.HoraCita.Value.Month, item.HoraCita.Value.Day,
                item.HoraCita.Value.Hour, item.HoraCita.Value.Minute, 0);

            // Validar disponibilidad
            if (await _repository.ExisteCitaSimultaneaAsync(item.MedicoId!.Value, horaNormalizada, ct))
                throw new InvalidOperationException($"El médico ya tiene una cita ocupada a las {horaNormalizada:HH:mm}.");

            var cita = new CitaMedica(item.MedicoId.Value, pacienteId, cuentaId, horaNormalizada, item.Comentario);
            await _repository.AgregarCitaMedicaAsync(cita, ct);
        }

        private bool EsTipoConsulta(string tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return false;
            var t = tipo.ToUpper();
            return t.Contains("CONSULTA") || t.Contains("MEDICO") || t.Contains("MÉDICO") || 
                   t.Contains("GINECO") || t.Contains("OBSTETRI");
        }
    }
}
