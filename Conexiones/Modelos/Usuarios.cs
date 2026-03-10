using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Usuarios
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Contraseña { get; set; }
        public int Cargo { get; set; }
        public string CB { get; set; }
        public string MPPS { get; set; }
        public string SIGLAS { get; set; }
        public string Firma { get; set; }
        public bool Activo { get; set; }
        public Privilegios Privilegios { get; set; } = new Privilegios();
        public List<Convenios> Convenios { get; set; } = new List<Convenios>();
    }
}
