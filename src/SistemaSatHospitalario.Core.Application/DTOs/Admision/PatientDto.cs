using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class PatientDto
    {
        // Identidad nativa del sistema nuevo (V11.1 Standard)
        public Guid Id { get; set; }
        
        // Referencia al Sistema 2020 Legacy (Opcional)
        public int? IdPacienteLegacy { get; set; }
        
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
