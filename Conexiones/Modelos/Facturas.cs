using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones
{
    public class Facturas
    {
        public int Id { get; set; }
        public int IdSesion { get; set; }
        public int NumeroDia { get; set; }
        public int IdConvenio { get; set; }
        public int IdUsuario { get; set; }
        public int IdRepresentante { get; set; }
        public int IdTasa { get; set; }
        public string Fecha { get; set; }
        public string Hora { get; set; }
        public string PrecioF { get; set; }
        public int EstadoDeFactura { get; set; }
        public Veterinario datoveterinario { get; set; } = new Veterinario();
        public Finca finca { get; set; } = new Finca();
        public List<Ordenes> Ordenes { get; set; } = new List<Ordenes>();
        public DatosRepresentante datosRepresentante { get; set; } = new DatosRepresentante();
        public Convenios convenios { get; set; } = new Convenios();
    }
}
