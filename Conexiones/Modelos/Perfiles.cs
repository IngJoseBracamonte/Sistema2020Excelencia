using Conexiones.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Perfil : IPerfil
    {
        public int IdPerfil { get; set; }
        public string NombrePerfil { get; set; }
        public string Precio { get; set; }
        public double PrecioDolar { get; set; }
        public int Activo { get; set; }
        public int idMetodo { get; set; }
        public int IdImpresionAgrupado { get; set; }

        public List<AnalisisLaboratorio> analisisLaboratorios { get; set; } = new List<AnalisisLaboratorio>();
        public List<ResultadosPorAnalisisVet> resultados { get; set; } = new List<ResultadosPorAnalisisVet>();


    }
}
