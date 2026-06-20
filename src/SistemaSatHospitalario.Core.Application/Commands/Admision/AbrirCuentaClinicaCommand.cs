using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class AbrirCuentaClinicaCommand : IRequest<Guid>
    {
        public Guid PacienteId { get; set; }
        public string TipoIngreso { get; set; } = string.Empty;
        public int? ConvenioId { get; set; }
        public string UsuarioCarga { get; set; } = string.Empty;
    }
}
