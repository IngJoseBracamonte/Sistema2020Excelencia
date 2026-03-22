using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class PatientDto
    {
        // Se añadió el Id numérico reflejando el IdPersona del legacy
        public int Id { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Sexo { get; set; }
        public string Correo { get; set; }
        public string Celular { get; set; }
        public bool EsLegacy { get; set; }
        public string Source { get; set; } // "Nativo" o "Legacy"
    }
}
