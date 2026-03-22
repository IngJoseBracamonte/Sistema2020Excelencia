using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class DoctorDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string Especialidad { get; set; }
    }
}
