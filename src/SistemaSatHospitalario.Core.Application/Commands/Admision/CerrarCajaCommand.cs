using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CerrarCajaCommand : IRequest<bool>
    {
    }

    public class CerrarCajaCommandHandler : IRequestHandler<CerrarCajaCommand, bool>
    {
        private readonly ICajaAdministrativaRepository _repository;

        public CerrarCajaCommandHandler(ICajaAdministrativaRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(CerrarCajaCommand request, CancellationToken cancellationToken)
        {
            var cajaAbierta = await _repository.ObtenerCajaAbiertaAsync(cancellationToken);
            if (cajaAbierta == null)
            {
                throw new InvalidOperationException("No se encontró ninguna caja abierta para cerrar.");
            }

            cajaAbierta.CerrarCaja();
            await _repository.GuardarCambiosAsync(cancellationToken);

            return true;
        }
    }
}
