using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class SettleARCommandHandler : IRequestHandler<SettleARCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public SettleARCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(SettleARCommand request, CancellationToken cancellationToken)
        {
            var ar = await _context.CuentasPorCobrar.FindAsync(new object[] { request.ARId }, cancellationToken);

            if (ar == null) throw new Exception("Cuenta por cobrar no encontrada.");
            if (ar.Estado == "Cobrada") throw new Exception("Esta cuenta ya ha sido cobrada.");

            // Liquidar
            ar.MarcarComoCobrada();

            // Aquí podríamos registrar un DetallePago final asociado al recibo original o a la CXC
            // Por ahora, simplemente actualizamos el estado.
            
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }
    }
}
