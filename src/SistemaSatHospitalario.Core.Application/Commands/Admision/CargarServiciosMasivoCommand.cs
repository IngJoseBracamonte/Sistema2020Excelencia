using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CargarServiciosMasivoCommand : IRequest<List<CargarServicioResult>>
    {
        public Guid PacienteId { get; set; }
        public string TipoIngreso { get; set; } = string.Empty;
        public int? ConvenioId { get; set; }
        public List<ServicioMasivoItemDto> Items { get; set; } = new List<ServicioMasivoItemDto>();
    }

    public class ServicioMasivoItemDto
    {
        public string ServicioId { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public decimal Precio { get; set; }
        public decimal Honorario { get; set; }
        public decimal Cantidad { get; set; }
        public string TipoServicio { get; set; } = string.Empty;
        public Guid? MedicoId { get; set; }
        public DateTime? HoraCita { get; set; }
        public Guid? AreaClinicaId { get; set; }
        public decimal? PrecioModificado { get; set; }
        public decimal? HonorarioModificado { get; set; }
        public string? SupervisorKey { get; set; }
    }

    public class CargarServiciosMasivoCommandHandler : IRequestHandler<CargarServiciosMasivoCommand, List<CargarServicioResult>>
    {
        private readonly IRequestHandler<CargarServicioACuentaCommand, CargarServicioResult> _singleHandler;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<CargarServiciosMasivoCommandHandler> _logger;

        public CargarServiciosMasivoCommandHandler(
            IRequestHandler<CargarServicioACuentaCommand, CargarServicioResult> singleHandler,
            IApplicationDbContext context,
            ILogger<CargarServiciosMasivoCommandHandler> logger)
        {
            _singleHandler = singleHandler;
            _context = context;
            _logger = logger;
        }

        public async Task<List<CargarServicioResult>> Handle(CargarServiciosMasivoCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Iniciando CargarServiciosMasivo para Paciente {PacienteId} con {Count} items.", request.PacienteId, request.Items.Count);
            
            var results = new List<CargarServicioResult>();
            
            using var transaction = await _context.BeginTransactionAsync(cancellationToken);
            try
            {
                foreach (var item in request.Items)
                {
                    var singleCommand = new CargarServicioACuentaCommand
                    {
                        PacienteId = request.PacienteId,
                        TipoIngreso = request.TipoIngreso,
                        ConvenioId = request.ConvenioId,
                        ServicioId = item.ServicioId,
                        Descripcion = item.Descripcion,
                        Precio = item.Precio,
                        Honorario = item.Honorario,
                        Cantidad = item.Cantidad,
                        TipoServicio = item.TipoServicio,
                        UsuarioCarga = "NursingAssistant",
                        MedicoId = item.MedicoId,
                        HoraCita = item.HoraCita,
                        AreaClinicaId = item.AreaClinicaId,
                        PrecioModificado = item.PrecioModificado,
                        HonorarioModificado = item.HonorarioModificado,
                        SupervisorKey = item.SupervisorKey,
                        IsPrivilegedUser = false
                    };

                    var res = await _singleHandler.Handle(singleCommand, cancellationToken);
                    results.Add(res);
                }

                if (transaction != null)
                {
                    await transaction.CommitAsync(cancellationToken);
                }

                _logger.LogInformation("Carga masiva completada exitosamente. Total cargados: {Count}", results.Count);
                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error durante la carga masiva. Revirtiendo transacción.");
                if (transaction != null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                }
                throw;
            }
        }
    }
}
