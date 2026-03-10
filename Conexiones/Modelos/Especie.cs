using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Especies
    {
        public int IdEspecie { get; set; }
        public string Descripcion { get; set; }
        public List<Hemo> hemoParasitos { get; set; } = new List<Hemo>();
    }
}
