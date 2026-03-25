using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateConvenioCommand : IRequest<bool>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PorcentajeCobertura { get; set; }
        public bool Activo { get; set; }
    }

    public class UpdateConvenioCommandHandler : IRequestHandler<UpdateConvenioCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateConvenioCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateConvenioCommand request, CancellationToken cancellationToken)
        {
            var convenio = await _context.SegurosConvenios.FirstOrDefaultAsync(s => s.Id == request.Id, cancellationToken);
            if (convenio == null) return false;

            convenio.Actualizar(request.Nombre, request.PorcentajeCobertura);
            convenio.SetActivo(request.Activo);
            
            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
