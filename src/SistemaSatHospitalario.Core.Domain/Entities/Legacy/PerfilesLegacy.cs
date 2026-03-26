using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Legacy
{
    public class PerfilLegacy
    {
        public int IdPerfil { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; } // Precio base HNL
        public decimal PrecioDOlar { get; set; }
        public int Estado { get; set; } // Activo o inactivo
    }

    public class PerfilesAnalisisLegacy
    {
        public int IdDetalle { get; set; }
        public int IdPerfil { get; set; } // Referencia al título de facturación (PerfilLegacy)
        public int IdOrganizador { get; set; } // Vincula con IDOrganizador en ResultadosPaciente
        public int IdAnalisis { get; set; } // Vincula con IdAnalisis en ResultadosPaciente
    }

    public class PerfilesFacturadosLegacy
    {
        public int IdFacturado { get; set; }
        public int IdOrden { get; set; }
        public int IdPerfil { get; set; }
        public decimal PrecioTotal { get; set; }
    }
}
