using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.Dto
{
    public class AnalisisLaboratorio
    {
        public int IdAnalisis { get; set; }
        public string NombreAnalisis { get; set; }
        public int TipoAnalisis { get; set; }
        public bool Visible { get; set; }
        public string Etiqueta { get; set; }
        public int IdSeccion { get; set; }
        public int Especiales { get; set; }
        public int Titulo { get; set; }
        public int IdAgrupador { get; set; }
        public int FinalTitulo { get; set; }
        public int Lineas { get; set; }
        public int Modificable { get; set; }
        public int idOrganizador { get; set; }
        public mayoromenorreferencial valoresDeReferencia { get; set; }
    }
}
