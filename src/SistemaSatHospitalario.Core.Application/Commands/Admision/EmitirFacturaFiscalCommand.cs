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
        public Guid? UsuarioEmision { get; set; } = null;
    }

    public class EmitirFacturaFiscalCommandHandler : IRequestHandler<EmitirFacturaFiscalCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public EmitirFacturaFiscalCommandHandler(IApplicationDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;   
        }

        public async Task<bool> Handle(EmitirFacturaFiscalCommand request, CancellationToken cancellationToken)
        {
            var recibo = await _context.RecibosFactura
                .FirstOrDefaultAsync(r => r.Id == request.ReciboId, cancellationToken);

            if (recibo == null) return false;

            recibo.Emitir(request.NroControlFiscal, _currentUserService.UserId);
            
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
