using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Queries.Admision
{
    public class GetOpenAccountQueryHandler : IRequestHandler<GetOpenAccountQuery, OpenAccountDto?>
    {
        private readonly IApplicationDbContext _context;

        public GetOpenAccountQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<OpenAccountDto?> Handle(GetOpenAccountQuery request, CancellationToken cancellationToken)
        {
            var query = _context.CuentasServicios
                .Include(c => c.Detalles)
                .AsNoTracking()
                .Where(c => c.PacienteId == request.PacienteId && c.Estado == EstadoConstants.Abierta);

            if (!string.IsNullOrEmpty(request.TipoIngreso))
            {
                query = query.Where(c => c.TipoIngreso == request.TipoIngreso);
            }

            var cuenta = await query.FirstOrDefaultAsync(cancellationToken);
            if (cuenta == null) return null;

            var detallesDto = new List<OpenAccountDetailDto>();
            foreach (var d in cuenta.Detalles)
            {
                string? medicoNombre = null;
                if (d.MedicoResponsableId.HasValue)
                {
                    var medico = await _context.Medicos.FindAsync(new object[] { d.MedicoResponsableId.Value }, cancellationToken);
                    medicoNombre = medico?.Nombre;
                }

                detallesDto.Add(new OpenAccountDetailDto
                {
                    Id = d.Id,
                    ServicioId = d.ServicioId,
                    Descripcion = d.Descripcion,
                    Precio = d.Precio,
                    Honorario = d.Honorario,
                    Cantidad = d.Cantidad,
                    TipoServicio = d.TipoServicio,
                    FechaCarga = d.FechaCarga,
                    LegacyMappingId = d.LegacyMappingId,
                    MedicoResponsableId = d.MedicoResponsableId,
                    MedicoNombre = medicoNombre
                });
            }

            return new OpenAccountDto
            {
                Id = cuenta.Id,
                PacienteId = cuenta.PacienteId,
                TipoIngreso = cuenta.TipoIngreso,
                ConvenioId = cuenta.ConvenioId,
                Detalles = detallesDto
            };
        }
    }
}
