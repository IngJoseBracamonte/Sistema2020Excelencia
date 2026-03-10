using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Ordenes
    {
        public int idOrden {get;set;}
        public int IdDatosPaciente { get;set;}
        public int IdDatosRepresentante {get;set;}
        public int IdConvenio { get;set;}
        public int IdTasa { get;set;}
        public int IdUsuario { get;set;}
        public int IDEstadoDeOrden { get;set;}
        public string PrecioF { get;set;}
        public int NumeroDia { get;set;}
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public int IDSesion { get;set;}
       
        public DatosRepresentante datosRepresentante { get; set; }
        public DatosPacienteVet datosPacienteVet { get; set; }
        public Convenios convenio { get; set; }
        public EstadosDeOrden estadosDeOrden { get; set; }
        public List<ResultadosPorAnalisisVet> ResultadosAnalisis { get; set; }
        public Usuarios usuarios { get; set; }

    }
    public class OrdenesEstadistica
    {
        public int idOrden { get; set; }
        public int IdDatosPaciente { get; set; }
        public int IdDatosRepresentante { get; set; }
        public int IdConvenio { get; set; }
        public int IdTasa { get; set; }
        public int IdUsuario { get; set; }
        public int EstadoDeOrden { get; set; }
        public string PrecioF { get; set; }
        public int NumeroDia { get; set; }
        public DateTime Fecha { get; set; }
        public string Hora { get; set; }
        public Convenios convenio { get; set; }
        public DatosDePaciente datosPaciente { get; set; } = new DatosDePaciente();
        public List<Perfil> perfiles = new List<Perfil>();

    }
}
