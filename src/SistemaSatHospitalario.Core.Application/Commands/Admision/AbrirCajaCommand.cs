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
        public string UsuarioId { get; set; }
        public string NombreUsuario { get; set; }
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
            // Verificamos si este usuario ya tiene una caja abierta
            var cajaAbierta = await _repository.ObtenerCajaAbiertaPorUsuarioAsync(request.UsuarioId, cancellationToken);
            if (cajaAbierta != null)
            {
                // Si ya tiene una abierta, devolvemos esa misma o lanzamos error según la lógica de negocio.
                // Para apertura automática, devolvemos el ID existente.
                return cajaAbierta.Id;
            }

            var nuevaCaja = new CajaDiaria(
                request.MontoInicialDivisa, 
                request.MontoInicialBs, 
                request.UsuarioId, 
                request.NombreUsuario
            );

            await _repository.AgregarCajaAsync(nuevaCaja, cancellationToken);
            await _repository.GuardarCambiosAsync(cancellationToken);

            return nuevaCaja.Id;
        }
    }
}
