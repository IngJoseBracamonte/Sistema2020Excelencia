using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class UpdateCatalogItemCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Descripcion { get; set; }
        public string Codigo { get; set; }
        public decimal PrecioUsd { get; set; }
        public string Tipo { get; set; }
        public bool Activo { get; set; }
    }

    public class UpdateCatalogItemCommandHandler : IRequestHandler<UpdateCatalogItemCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateCatalogItemCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateCatalogItemCommand request, CancellationToken cancellationToken)
        {
            var item = await _context.ServiciosClinicos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (item == null) return false;

            item.Descripcion = request.Descripcion;
            item.PrecioBase = request.PrecioUsd;
            item.TipoServicio = request.Tipo;
            item.Codigo = request.Codigo;
            item.Activo = request.Activo;

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
