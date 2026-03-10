using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Legacy
{
    public class DatosPersonalesLegacy
    {
        public int IdPersona { get; set; }
        
        // Asumiendo los campos base de un paciente en Sistema2020 basados en el dominio del hospital
        public string Identificacion { get; set; } 
        public string Nombre1 { get; set; }
        public string Apellido1 { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
    }
}
