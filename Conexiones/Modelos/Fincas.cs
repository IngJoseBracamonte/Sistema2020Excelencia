using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Finca
    {
        public int IdFinca { get; set; }
        public string RIF { get; set; }
        public string NombreFinca { get; set; }
        public Estado estado = new Estado();
        public Ciudades ciudad = new Ciudades();
        public Parroquia parroquia = new Parroquia();
        public Municipio municipio = new Municipio();
    }
}
