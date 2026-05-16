using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands
{
    public class LiberarTurnoCommand : IRequest<bool>
    {
        public Guid MedicoId { get; set; }
        public DateTime HoraPautada { get; set; }
        public string UsuarioId { get; set; }
    }

    public class LiberarTurnoCommandHandler : IRequestHandler<LiberarTurnoCommand, bool>
    {
        private readonly IBillingRepository _repository;

        public LiberarTurnoCommandHandler(IBillingRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(LiberarTurnoCommand request, CancellationToken cancellationToken)
        {
            await _repository.LiberarReservaTemporalAsync(request.MedicoId, request.HoraPautada, request.UsuarioId, cancellationToken);
            return true;
        }
    }
}
