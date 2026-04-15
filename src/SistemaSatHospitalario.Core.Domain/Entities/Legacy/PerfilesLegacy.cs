using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSatHospitalario.Core.Domain.Entities.Legacy
{
    public class PerfilLegacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPerfil { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; } // Precio base HNL
        public decimal PrecioDolar { get; set; }
        public int Estado { get; set; } // Activo o inactivo
    }

    public class PerfilesAnalisisLegacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdDetalle { get; set; }
        public int IdPerfil { get; set; } // Referencia al título de facturación (PerfilLegacy)
        public int IdOrganizador { get; set; } // Vincula con IDOrganizador en ResultadosPaciente
        public int IdAnalisis { get; set; } // Vincula con IdAnalisis en ResultadosPaciente
    }

    public class PerfilesFacturadosLegacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdFacturado { get; set; }
        public int IdOrden { get; set; }
        public int IdPersona { get; set; } // Nuevo: Requerido por la tabla legacy
        public int IdPerfil { get; set; }
        public decimal PrecioPerfil { get; set; } // Renombrado: PrecioTotal -> PrecioPerfil
        public string Facturado { get; set; } = "S"; 
    }
}
