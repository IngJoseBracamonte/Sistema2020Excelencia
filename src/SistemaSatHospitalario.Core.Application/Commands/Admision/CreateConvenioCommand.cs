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
        public string Nombre { get; set; }
        public string Rtn { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }
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
            var convenio = new SeguroConvenio(
                request.Nombre, 
                request.Rtn, 
                request.Direccion, 
                request.Telefono, 
                request.Email);
                
            _context.SegurosConvenios.Add(convenio);
            
            await _context.SaveChangesAsync(cancellationToken);
            return convenio.Id;
        }
    }
}
