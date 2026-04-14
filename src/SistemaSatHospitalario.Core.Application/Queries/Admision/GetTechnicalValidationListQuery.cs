using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetTechnicalValidationListQuery : IRequest<ValidationDashboardDto>
    {
        public string Role { get; set; } = string.Empty; // Rx, Tomografia, Particular
        public DateTime? Fecha { get; set; }
    }

    public class ValidationDashboardDto
    {
        public List<AppointmentValidationDto> Citas { get; set; } = new();
        public List<ServiceValidationDto> Estudios { get; set; } = new();
    }

    public class AppointmentValidationDto
    {
        public Guid Id { get; set; }
        public string Paciente { get; set; } = string.Empty;
        public string Medico { get; set; } = string.Empty;
        public DateTime Hora { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Comentario { get; set; }
    }

    public class ServiceValidationDto
    {
        public Guid Id { get; set; }
        public Guid CuentaId { get; set; }
        public string Paciente { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string TipoSeguro { get; set; } = string.Empty;
        public DateTime FechaCarga { get; set; }
        public bool Realizado { get; set; }
    }

    public class GetTechnicalValidationListQueryHandler : IRequestHandler<GetTechnicalValidationListQuery, ValidationDashboardDto>
    {
        private readonly IApplicationDbContext _context;

        public GetTechnicalValidationListQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ValidationDashboardDto> Handle(GetTechnicalValidationListQuery request, CancellationToken ct)
        {
            var response = new ValidationDashboardDto();
            var targetDate = request.Fecha?.Date ?? DateTime.Today;
            var nextDate = targetDate.AddDays(1);

            // 1. Filtrar Citas (Para Asistente Particular o Seguros)
            if (request.Role.Contains("Particular", StringComparison.OrdinalIgnoreCase) || 
                request.Role.Contains("Seguros", StringComparison.OrdinalIgnoreCase) || 
                request.Role.Contains("Admin", StringComparison.OrdinalIgnoreCase))
            {
                response.Citas = await _context.CitasMedicas
                    .Include(c => c.Medico)
                    .Where(c => c.HoraPautada >= targetDate && c.HoraPautada < nextDate)
                    .Select(c => new AppointmentValidationDto
                    {
                        Id = c.Id,
                        Medico = c.Medico.Nombre,
                        Hora = c.HoraPautada,
                        Estado = c.Estado,
                        Comentario = c.Comentario
                    })
                    .ToListAsync(ct);
                
                // Nota: El nombre del paciente se resolvería via join o post-proceso si no está en la entidad CitaMedica directamente.
                // En este sistema, CitaMedica tiene PacienteId.
            }

            // 2. Filtrar Estudios de Imagen
            var query = _context.DetallesServicioCuenta.AsQueryable();

            if (request.Role.Contains("RX", StringComparison.OrdinalIgnoreCase))
            {
                // Se asume que el catálogo vincula DetalleServicio con su categoría original
                // Como Detalle no tiene Categoría directa, usamos TipoServicio o Join con Clinico
                var rxItems = await (from d in _context.DetallesServicioCuenta
                                     join s in _context.ServiciosClinicos on d.ServicioId equals s.Id
                                     join c in _context.CuentasServicios on d.CuentaServicioId equals c.Id
                                     where s.Category == ServiceCategory.Radiology 
                                        && d.FechaCarga >= targetDate && d.FechaCarga < nextDate
                                     select new ServiceValidationDto
                                     {
                                         Id = d.Id,
                                         CuentaId = d.CuentaServicioId,
                                         Descripcion = d.Descripcion,
                                         FechaCarga = d.FechaCarga,
                                         Realizado = d.Realizado,
                                         TipoSeguro = c.TipoIngreso
                                     }).ToListAsync(ct);
                response.Estudios.AddRange(rxItems);
            }
            else if (request.Role.Contains("Tomografía", StringComparison.OrdinalIgnoreCase))
            {
                var tomoItems = await (from d in _context.DetallesServicioCuenta
                                     join s in _context.ServiciosClinicos on d.ServicioId equals s.Id
                                     join c in _context.CuentasServicios on d.CuentaServicioId equals c.Id
                                     where s.Category == ServiceCategory.Tomography
                                        && d.FechaCarga >= targetDate && d.FechaCarga < nextDate
                                     select new ServiceValidationDto
                                     {
                                         Id = d.Id,
                                         CuentaId = d.CuentaServicioId,
                                         Descripcion = d.Descripcion,
                                         FechaCarga = d.FechaCarga,
                                         Realizado = d.Realizado,
                                         TipoSeguro = c.TipoIngreso
                                     }).ToListAsync(ct);
                response.Estudios.AddRange(tomoItems);
            }

            return response;
        }
    }
}
