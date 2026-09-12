using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class DeleteAreaClinicaCommandHandler : IRequestHandler<DeleteAreaClinicaCommand>
    {
        private readonly IApplicationDbContext _context;

        public DeleteAreaClinicaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Handle(DeleteAreaClinicaCommand request, CancellationToken cancellationToken)
        {
            var area = await _context.AreasClinicas
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (area == null)
                throw new InvalidOperationException($"No se encontró el área clínica con ID {request.Id}.");

            // Soft-delete: desactiva el área sin eliminar el registro (integridad histórica)
            area.SetEstado(false);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
