using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Legacy
{
    public class OrdenLegacy
    {
        public int IdOrden { get; set; }
        public int IdPersona { get; set; } // Referencia a DatosPersonales
        public DateTime Fecha { get; set; }
        public string HoraIngreso { get; set; }
        public int Usuario { get; set; }
        public int EstadoDeOrden { get; set; }
        public int NumeroDia { get; set; }
        public decimal PrecioF { get; set; }
        public int VALIDADA { get; set; }
        public int Muestra { get; set; }
        public int IDConvenio { get; set; }
        public int IDTasa { get; set; }
        public int IdCierreDeCaja { get; set; }
    }
}
