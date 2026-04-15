using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSatHospitalario.Core.Domain.Entities.Legacy
{
    public class ResultadosPacienteLegacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdResultadoPaciente { get; set; }

        public int IdOrden { get; set; }
        public int IdPaciente { get; set; } // Referencia DatosPersonales
        public int IdConvenio { get; set; }
        public int IDOrganizador { get; set; } // Grupo de PerfilesAnalisis
        public int IdAnalisis { get; set; } // Prueba individual
        
        public string? ValorResultado { get; set; }
        public string? Unidad { get; set; }
        public string? Comentario { get; set; }
        
        public string HoraIngreso { get; set; }
        public DateTime? FechaIngreso { get; set; }
        
        public string? HoraValidacion { get; set; }
        public DateTime? FechaValidacion { get; set; }
        public string? HoraImpresion { get; set; }
        
        public int? EstadoDeResultado { get; set; }
        public int? IdUsuario { get; set; } // Técnico de Laboratorio
        
        public int? Enviado { get; set; }
        public int? Recibido { get; set; }
        
        public string? ValorMenor { get; set; }
        public string? ValorMayor { get; set; }
        
        public int? MultiplesValores { get; set; }
        public int? Lineas { get; set; }
        public int PorEnviar { get; set; } = 0;
    }
}
