using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateConfiguracionCommand : IRequest<bool>
    {
        public string NombreEmpresa { get; set; }
        public string Rif { get; set; }
        public decimal Iva { get; set; }
    }

    public class UpdateConfiguracionCommandHandler : IRequestHandler<UpdateConfiguracionCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateConfiguracionCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateConfiguracionCommand request, CancellationToken cancellationToken)
        {
            var config = await _context.ConfiguracionGeneral.FirstOrDefaultAsync(cancellationToken);

            if (config == null) return false;

            config.Actualizar(request.NombreEmpresa, request.Rif, request.Iva);
            
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
