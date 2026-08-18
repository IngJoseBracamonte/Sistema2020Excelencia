using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RemoveRequisitoFromOrdenCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public Guid RequisitoCirugiaId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class RemoveRequisitoFromOrdenCirugiaCommandHandler : IRequestHandler<RemoveRequisitoFromOrdenCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public RemoveRequisitoFromOrdenCirugiaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(RemoveRequisitoFromOrdenCirugiaCommand request, CancellationToken cancellationToken)
        {
            var itemReq = await _context.OrdenesCirugiaRequisitos
                .Include(r => r.RequisitoCirugia)
                .FirstOrDefaultAsync(r => r.OrdenCirugiaId == request.OrdenCirugiaId && 
                                         (r.RequisitoCirugiaId == request.RequisitoCirugiaId || r.Id == request.RequisitoCirugiaId), cancellationToken);

            if (itemReq != null)
            {
                var nombre = itemReq.RequisitoCirugia?.Nombre ?? "Requisito";
                _context.OrdenesCirugiaRequisitos.Remove(itemReq);

                var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "admin" : request.UsuarioId;
                var log = new CirugiaLog(request.OrdenCirugiaId, usuario, "RemoverRequisito",
                    $"Se removió el requisito preoperatorio '{nombre}'.");
                _context.CirugiaLogs.Add(log);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }
    }
}
