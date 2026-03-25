using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateConvenioCommand : IRequest<int>
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PorcentajeCobertura { get; set; }
    }

    public class CreateConvenioCommandHandler : IRequestHandler<CreateConvenioCommand, int>
    {
        private readonly IApplicationDbContext _context;

        public CreateConvenioCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(CreateConvenioCommand request, CancellationToken cancellationToken)
        {
            // Verificar si ya existe para evitar duplicados en legacy sync
            if (await _context.SegurosConvenios.AnyAsync(s => s.Id == request.Id, cancellationToken))
            {
                return 0;
            }

            var convenio = new SeguroConvenio(request.Id, request.Nombre, request.PorcentajeCobertura);
            _context.SegurosConvenios.Add(convenio);
            
            await _context.SaveChangesAsync(cancellationToken);
            return convenio.Id;
        }
    }
}
