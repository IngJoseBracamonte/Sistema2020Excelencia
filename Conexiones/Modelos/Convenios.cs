using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Convenios
    {
        public int IdConvenio {get;set;}
        public string Nombre  {get;set;}
        public string Direccion { get; set; }
        public string Correo { get; set; }
        public string Ruta { get; set; }
        public bool Activos { get; set; }
        public string Descuento { get; set; } 
    }
}
