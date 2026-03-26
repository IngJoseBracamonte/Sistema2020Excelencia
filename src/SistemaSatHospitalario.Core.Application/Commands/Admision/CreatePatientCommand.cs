using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreatePatientCommand : IRequest<PatientDto>
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Sexo { get; set; } = string.Empty;
        public string FechaNacimiento { get; set; } = string.Empty;
        public string Correo { get; set; } = string.Empty;
        public string TipoCorreo { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public string CodigoCelular { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string CodigoTelefono { get; set; } = string.Empty;
    }
}
