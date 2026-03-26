using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AdminCancelAppointmentCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; set; }
    }

    public class AdminCancelAppointmentCommandHandler : IRequestHandler<AdminCancelAppointmentCommand, bool>
    {
        private readonly IBillingRepository _repository;

        public AdminCancelAppointmentCommandHandler(IBillingRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(AdminCancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            await _repository.CancelarCitaPorIdAsync(request.AppointmentId, cancellationToken);
            await _repository.GuardarCambiosAsync(cancellationToken);
            return true;
        }
    }
}
