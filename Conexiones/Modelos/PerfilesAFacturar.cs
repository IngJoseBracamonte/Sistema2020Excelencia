using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conexiones;

namespace Conexiones
{
   public class PerfilesAFacturar
    {
        public int idPerfil { get; set; }
        public Perfil perfil { get; set; }
        public int IdSesion { get; set; }
        public string Cantidad { get; set; }
        public string Precio { get; set; }
        public int idRepresentante { get; set; }
        public DatosRepresentante datosRepresentante { get; set; }
    }
}
