using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AuditARCommandHandler : IRequestHandler<AuditARCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public AuditARCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(AuditARCommand request, CancellationToken cancellationToken)
        {
            var ar = await _context.CuentasPorCobrar
                .FirstOrDefaultAsync(x => x.Id == request.ArId, cancellationToken);

            if (ar == null)
                throw new Exception("La cuenta por cobrar no existe.");

            ar.MarcarComoAuditada(request.UsuarioAuditor);
            
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
