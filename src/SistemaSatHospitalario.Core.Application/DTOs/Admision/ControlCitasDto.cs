using System;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class ControlCitasDto
    {
        public Guid Id { get; set; } // Cita Id
        public DateTime Hora { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public string PacienteTelefono { get; set; }
        public int? PacienteEdad { get; set; }
        public string Especialidad { get; set; }
        public string Medico { get; set; }
        public string FormaPago { get; set; } // Convenio o Particular
        public decimal MontoUSD { get; set; } // Total facturado en USD
        public string Estado { get; set; } // Pendiente, Atendida, Cancelada
        public string Observaciones { get; set; }
        public int Turno { get; set; } // Calculado: Orden de llegada/cita
        public Guid CuentaServicioId { get; set; } // Para navegación a Expediente
    }
}
