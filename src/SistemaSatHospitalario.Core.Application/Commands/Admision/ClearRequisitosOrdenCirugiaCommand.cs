using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ClearRequisitosOrdenCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class ClearRequisitosOrdenCirugiaCommandHandler : IRequestHandler<ClearRequisitosOrdenCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public ClearRequisitosOrdenCirugiaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<bool> Handle(ClearRequisitosOrdenCirugiaCommand request, CancellationToken cancellationToken)
        {
            var items = await _context.OrdenesCirugiaRequisitos
                .Where(r => r.OrdenCirugiaId == request.OrdenCirugiaId)
                .ToListAsync(cancellationToken);

            if (items.Any())
            {
                _context.OrdenesCirugiaRequisitos.RemoveRange(items);
                var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "admin" : request.UsuarioId;
                var log = new CirugiaLog(request.OrdenCirugiaId, usuario, "LimpiarRequisitos",
                    $"Se eliminaron todos los requisitos preoperatorios ({items.Count} ítems).");
                _context.CirugiaLogs.Add(log);

                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }
    }
}
