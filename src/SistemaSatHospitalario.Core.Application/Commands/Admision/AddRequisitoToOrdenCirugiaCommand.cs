using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AddRequisitoToOrdenCirugiaCommand : IRequest<Guid>
    {
        public Guid OrdenCirugiaId { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class AddRequisitoToOrdenCirugiaCommandHandler : IRequestHandler<AddRequisitoToOrdenCirugiaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public AddRequisitoToOrdenCirugiaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Guid> Handle(AddRequisitoToOrdenCirugiaCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre del requisito preoperatorio es obligatorio.");

            // 1. Buscar o Crear Requisito en el catálogo
            var reqMaestro = await _context.RequisitosCirugia
                .FirstOrDefaultAsync(r => r.Nombre.ToLower() == request.Nombre.Trim().ToLower(), cancellationToken);

            if (reqMaestro == null)
            {
                reqMaestro = new RequisitoCirugia(request.Nombre.Trim(), request.Descripcion?.Trim() ?? string.Empty, esActivo: true);
                _context.RequisitosCirugia.Add(reqMaestro);
                await _context.SaveChangesAsync(cancellationToken);
            }

            // 2. Verificar si ya está anexado a la orden
            var itemExistente = await _context.OrdenesCirugiaRequisitos
                .FirstOrDefaultAsync(r => r.OrdenCirugiaId == request.OrdenCirugiaId && r.RequisitoCirugiaId == reqMaestro.Id, cancellationToken);

            if (itemExistente == null)
            {
                var nuevoItem = new OrdenCirugiaRequisito(request.OrdenCirugiaId, reqMaestro.Id, cumplido: false);
                _context.OrdenesCirugiaRequisitos.Add(nuevoItem);

                var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "admin" : request.UsuarioId;
                var log = new CirugiaLog(request.OrdenCirugiaId, usuario, "AgregarRequisito",
                    $"Se agregó el requisito preoperatorio '{reqMaestro.Nombre}'.");
                _context.CirugiaLogs.Add(log);

                await _context.SaveChangesAsync(cancellationToken);
                return nuevoItem.Id;
            }

            return itemExistente.Id;
        }
    }
}
