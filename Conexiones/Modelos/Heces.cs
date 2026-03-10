using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.Modelos
{
    public class Heces
    {
        public int IdHeces { get; set; }
        public int IdOrden { get; set; }
        public string Color { get; set; }
        public string Moco { get; set; }
        public string Reaccion { get; set; }
        public string Aspecto { get; set; }
        public string Sangre { get; set; }
        public string Ph { get; set; }
        public string Consistencia { get; set; }
        public string RestosAlimenticios { get; set; }
        public string Hematies { get; set; }
        public string Leucocitos { get; set; }
        public string Parasitos { get; set; }
        public string Amilorrea { get; set; }
        public string Creatorrea { get; set; }
        public string Polisacaridos { get; set; }
        public string Gotas { get; set; }
        public string Levaduras { get; set; }
        public string Comentario { get; set; }
        public string EstadoResultado { get; set; }

        public string IdUsuario { get; set; }

    }
}
