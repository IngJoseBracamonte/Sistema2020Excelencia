using System;
using MediatR;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class RegisterSurgicalPatientCommand : IRequest<PatientDto>
    {
        public string Cedula { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Apellidos { get; set; } = string.Empty;
        public string Celular { get; set; } = string.Empty;
        public DateTime? FechaNacimiento { get; set; }
        public string Direccion { get; set; } = string.Empty;
    }
}