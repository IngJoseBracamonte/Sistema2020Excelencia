using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class DatosDePaciente
    {
       public int IdPersona { get; set; }
        public string Cedula { get; set; }
         public string  Nombre { get; set; }
         public string  Apellidos { get; set; }
         public string  Celular { get; set; }
         public string  Telefono { get; set; }
         public string  Sexo { get; set; }
         public DateTime  Fecha { get; set; }
         public string  Correo { get; set; }
         public string  TipoCorreo { get; set; }
         public string  CodigoCelular { get; set; }
         public string  CodigoTelefono { get; set; }
         public string Edad { get; set; }
    }
    public class DatosPacienteVet
    {
        public int IdDatosPaciente { get; set; }
        public string  NombrePaciente { get; set; }
        public int IdEspecie { get; set; }
        public string Raza { get; set; }
        public int idDatosRepresentante { get; set; }
        public DatosRepresentante Representante { get; set; }
        public Especies especies { get; set; }
    }

    public class DatosRepresentante
    {
        public List<DatosPacienteVet> datosPacienteVets { get; set; }
        public int  idDatosRepresentante { get; set; }
        public string RIF {get;set;}
        public string TipoRepresentante {get;set;}
        public string NombreRepresentante {get;set;}
        public string ApellidoRepresentante {get;set;}
        public string TipoCelular { get; set; }
        public string Celular {get;set;}
        public string TipoTelefono { get; set; }
        public string Telefono {get;set;}
        public string TipoCorreo {get;set;}
        public string Correo {get;set;}
    }
}
