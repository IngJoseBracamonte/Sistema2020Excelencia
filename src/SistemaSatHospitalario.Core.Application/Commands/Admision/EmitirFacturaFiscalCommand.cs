using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class EmitirFacturaFiscalCommand : IRequest<bool>
    {
        public Guid ReciboId { get; set; }
        public string NroControlFiscal { get; set; } = string.Empty;
        public string UsuarioEmision { get; set; } = string.Empty;
    }

    public class EmitirFacturaFiscalCommandHandler : IRequestHandler<EmitirFacturaFiscalCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public EmitirFacturaFiscalCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(EmitirFacturaFiscalCommand request, CancellationToken cancellationToken)
        {
            var recibo = await _context.RecibosFactura
                .FirstOrDefaultAsync(r => r.Id == request.ReciboId, cancellationToken);

            if (recibo == null) return false;

            recibo.Emitir(request.NroControlFiscal, request.UsuarioEmision);
            
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
