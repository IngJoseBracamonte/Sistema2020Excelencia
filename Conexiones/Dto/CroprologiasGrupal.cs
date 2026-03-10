using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.Dto
{
    public class CroprologiasGrupal
    {
        public int IdOrden { get; set; }
        public string TECNICAWILLYS { get; set; } = string.Empty;
        public string TECNICADIRECTA { get; set; } = string.Empty;
        public string TECNICAMAC { get; set; } = string.Empty;
        public string SEDIMENTACION { get; set; } = string.Empty;
        public string Comentarios { get; set; } = string.Empty;
        public DatosPacienteVet datosPaciente { get; set; } = new DatosPacienteVet();
        public Usuarios Bioanalista { get; set; } = new Usuarios();

    }
}
