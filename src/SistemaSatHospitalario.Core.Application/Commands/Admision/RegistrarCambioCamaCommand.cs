using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegistrarCambioCamaCommand : IRequest<RegistrarCambioCamaResult>
    {
        public Guid CuentaId { get; set; }
        public Guid CamaDestinoId { get; set; }
        public Guid? UsuarioCarga { get; set; } 
    }

    public class RegistrarCambioCamaResult
    {
        public Guid CuentaId { get; set; }
        public Guid CamaDestinoId { get; set; }
        public bool Exitoso { get; set; }
    }
}
