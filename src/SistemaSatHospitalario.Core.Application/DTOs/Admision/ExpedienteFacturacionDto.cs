using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Application.DTOs.Admision
{
    public class ExpedienteFacturacionDto
    {
        public Guid Id { get; set; } // Detalle Id
        public DateTime Fecha { get; set; }
        public string PacienteNombre { get; set; }
        public string PacienteCedula { get; set; }
        public string PacienteTelefono { get; set; }
        public string Estudio { get; set; }
        public string TipoIngreso { get; set; } // Particular / Seguro
        public string SeguroNombre { get; set; }
        public string MetodoPago { get; set; }
        public decimal MontoUSD { get; set; }
        public string FacturadoPor { get; set; } // Nombre Real, Rol
        public string Estado { get; set; } // Pendiente / Facturado
        public string TipoServicio { get; set; } // RX, Tomografia, Laboratorio
    }
}
