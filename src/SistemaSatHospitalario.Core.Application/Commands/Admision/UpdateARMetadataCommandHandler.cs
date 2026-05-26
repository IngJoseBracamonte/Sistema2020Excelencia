using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateARMetadataCommandHandler : IRequestHandler<UpdateARMetadataCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateARMetadataCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateARMetadataCommand request, CancellationToken cancellationToken)
        {
            var ar = await _context.CuentasPorCobrar
                .FirstOrDefaultAsync(x => x.Id == request.CuentaPorCobrarId, cancellationToken);

            if (ar == null)
                return false;

            ar.ActualizarMetadataDocumentos(request.QuienAutorizo, request.DoctorProcedimiento, request.InformacionAdicional);
            
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
