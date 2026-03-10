using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AbrirCajaCommand : IRequest<Guid>
    {
        public decimal MontoInicialDivisa { get; set; }
        public decimal MontoInicialBs { get; set; }
    }

    public class AbrirCajaCommandHandler : IRequestHandler<AbrirCajaCommand, Guid>
    {
        private readonly ICajaAdministrativaRepository _repository;

        public AbrirCajaCommandHandler(ICajaAdministrativaRepository repository)
        {
            _repository = repository;
        }

        public async Task<Guid> Handle(AbrirCajaCommand request, CancellationToken cancellationToken)
        {
            var cajaAbierta = await _repository.ObtenerCajaAbiertaAsync(cancellationToken);
            if (cajaAbierta != null)
            {
                throw new InvalidOperationException("Ya existe una caja abierta en el sistema. Debe cerrarse antes de abrir una nueva.");
            }

            var nuevaCaja = new CajaDiaria(request.MontoInicialDivisa, request.MontoInicialBs);
            await _repository.AgregarCajaAsync(nuevaCaja, cancellationToken);
            await _repository.GuardarCambiosAsync(cancellationToken);

            return nuevaCaja.Id;
        }
    }
}
