using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSatHospitalario.Core.Domain.Entities.Legacy
{
    public class ConvenioEmpresaLegacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IDConvenio { get; set; }
        
        public string? Nombre { get; set; }
        
        // Propiedad técnica para mapeo de empresas en Aiven
        public int? IDTasa { get; set; }
    }
}
