using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class ToggleRequisitoCirugiaCommand : IRequest<bool>
    {
        public Guid OrdenCirugiaId { get; set; }
        public Guid RequisitoCirugiaId { get; set; }
        public bool Cumplido { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
    }

    public class ToggleRequisitoCirugiaCommandHandler : IRequestHandler<ToggleRequisitoCirugiaCommand, bool>
    {
        private readonly IApplicationDbContext _context;
        private readonly ILogger<ToggleRequisitoCirugiaCommandHandler> _logger;

        public ToggleRequisitoCirugiaCommandHandler(IApplicationDbContext context, ILogger<ToggleRequisitoCirugiaCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> Handle(ToggleRequisitoCirugiaCommand request, CancellationToken cancellationToken)
        {
            var itemReq = await _context.OrdenesCirugiaRequisitos
                .Include(r => r.RequisitoCirugia)
                .FirstOrDefaultAsync(r => r.OrdenCirugiaId == request.OrdenCirugiaId && r.RequisitoCirugiaId == request.RequisitoCirugiaId, cancellationToken);

            if (itemReq == null)
            {
                var reqMaestro = await _context.RequisitosCirugia.FirstOrDefaultAsync(r => r.Id == request.RequisitoCirugiaId, cancellationToken);
                if (reqMaestro == null)
                {
                    throw new InvalidOperationException($"No se encontró el requisito quirúrgico con ID: {request.RequisitoCirugiaId}");
                }

                itemReq = new OrdenCirugiaRequisito(request.OrdenCirugiaId, reqMaestro.Id, request.Cumplido);
                itemReq.SetCumplido(request.Cumplido, request.UsuarioId);
                await _context.OrdenesCirugiaRequisitos.AddAsync(itemReq, cancellationToken);
            }
            else
            {
                itemReq.SetCumplido(request.Cumplido, request.UsuarioId);
            }

            var nombreReq = itemReq.RequisitoCirugia?.Nombre ?? "Requisito";
            var estadoStr = request.Cumplido ? "Cumplido [V]" : "Pendiente [X]";

            var usuario = string.IsNullOrWhiteSpace(request.UsuarioId) ? "admin" : request.UsuarioId;
            var log = new CirugiaLog(request.OrdenCirugiaId, usuario, "VerificacionRequisito", $"Requisito '{nombreReq}' marcado como: {estadoStr}");
            await _context.CirugiaLogs.AddAsync(log, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
