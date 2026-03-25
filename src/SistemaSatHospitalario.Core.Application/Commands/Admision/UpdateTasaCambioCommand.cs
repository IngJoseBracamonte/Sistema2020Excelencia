using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateTasaCambioCommand : IRequest<bool>
    {
        public decimal Monto { get; set; }
    }

    public class UpdateTasaCambioCommandHandler : IRequestHandler<UpdateTasaCambioCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateTasaCambioCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateTasaCambioCommand request, CancellationToken cancellationToken)
        {
            // Desactivar tasas anteriores (si el modelo lo requiere) o simplemente agregar nueva
            var tasasActivas = await _context.TasaCambio.Where(t => t.Activo).ToListAsync(cancellationToken);
            foreach (var t in tasasActivas)
            {
                // Como las propiedades son private set, solo si hay método de desactivación
                // Pero por ahora, creamos una nueva y listo (histórico)
            }

            var nuevaTasa = new TasaCambio(request.Monto);
            _context.TasaCambio.Add(nuevaTasa);

            return await _context.SaveChangesAsync(cancellationToken) > 0;
        }
    }
}
