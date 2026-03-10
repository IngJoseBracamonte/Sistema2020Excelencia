using Conexiones.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class ResultadosPorAnalisis
    {
        public int IdAnalisis { get; }
        public string NombreAnalisis { get; }
        public double ValorResultado { get; set; }
        public string unidad { get; }
        public double ValorMenor { get; }
        public double ValorMayor { get; }
        public int TipoAnalisis { get; set; }
        public int IdOrganizador { get; set; }
        public int EstadoDeResultado { get; set; }

    }
    public class ResultadosPorAnalisisVet
    {

        public int IdAnalisis { get; set; }
        public int IdOrden { get; set; }
        public string ValorResultado { get; set; }
        public string unidad { get; set; }
        public double ValorMenor { get; set; }
        public double ValorMayor { get; set; }
        public int IdEstadoDeResultado { get; set; }
        public string Comentario { get; set; }
        public string MultiplesValores { get; set; }
        public int IdUsuario { get; set; }
        public bool Enviado { get; set; }
        public bool Recibido { get; set; }
        public DateTime FechaIngreso { get; set; }
        public string HoraIngreso { get; set; }
        public Usuarios bioanalista { get; set; }
        public AnalisisLaboratorio analisisLaboratorio { get; set; }
        public int IdOrganizador { get; set; }
        public bool PorImprimir { get; set; }
        public int Lineas { get; set; }
        public List<Hemo> hemo = new List<Hemo>();

    }
}
