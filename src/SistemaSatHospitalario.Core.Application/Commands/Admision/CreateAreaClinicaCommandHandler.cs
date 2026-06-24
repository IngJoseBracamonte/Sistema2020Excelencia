using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateAreaClinicaCommandHandler : IRequestHandler<CreateAreaClinicaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateAreaClinicaCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateAreaClinicaCommand request, CancellationToken cancellationToken)
        {
            var sede = await _context.Sedes
                .FirstOrDefaultAsync(s => s.Id == request.SedeId, cancellationToken);

            if (sede == null || !sede.Activo)
            {
                throw new InvalidOperationException("La sede especificada no existe o no está activa.");
            }

            var duplicateExists = await _context.AreasClinicas
                .AnyAsync(a => a.SedeId == request.SedeId && a.Codigo == request.Codigo && a.Activo, cancellationToken);
            if (duplicateExists)
            {
                throw new InvalidOperationException("Ya existe una área clínica activa con el mismo código en esta sede.");
            }

            var entity = new AreaClinica(request.SedeId, request.Codigo, request.Nombre);
            _context.AreasClinicas.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }
}
