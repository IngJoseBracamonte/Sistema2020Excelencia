using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSatHospitalario.Core.Domain.Entities.Legacy
{
    public class DatosPersonalesLegacy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPersona { get; set; }
        public string Cedula { get; set; }
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string Celular { get; set; }
        public string Telefono { get; set; }
        public string Sexo { get; set; }
        public string Fecha { get; set; } // DOB as string in legacy
        public string Correo { get; set; }
        public string TipoCorreo { get; set; } // @gmail.com, etc.
        public string CodigoCelular { get; set; }
        public string CodigoTelefono { get; set; }
        public int Visible { get; set; } // Nueva columna detectada
    }
}
