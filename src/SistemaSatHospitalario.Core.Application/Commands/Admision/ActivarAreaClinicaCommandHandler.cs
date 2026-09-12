using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ActivarAreaClinicaCommandHandler : IRequestHandler<ActivarAreaClinicaCommand>
    {
        private readonly IApplicationDbContext _context;

        public ActivarAreaClinicaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task Handle(ActivarAreaClinicaCommand request, CancellationToken cancellationToken)
        {
            var area = await _context.AreasClinicas
                .FirstOrDefaultAsync(a => a.Id == request.Id, cancellationToken);

            if (area == null)
                throw new InvalidOperationException($"No se encontró el área clínica con ID {request.Id}.");

            area.SetEstado(true);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
