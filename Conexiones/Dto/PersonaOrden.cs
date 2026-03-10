using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.Dto
{
    public class PersonaOrden
    {
        public int IdOrden { get; set; }
        public int NumeroDia { get; set; }
        public string NombreCompleto { get; set; }
        public string Sexo { get; set; }

        public DateTime fecha { get; set; }

        public PersonaOrden Mapear(DataRow data)
        {
            var personaOrden = new PersonaOrden();
            DateTime.TryParse(data["Fecha"].ToString(), out DateTime Fecha);
            int.TryParse(data["NumeroDia"].ToString(), out int NumeroDia);
            int.TryParse(data["IdOrden"].ToString(), out int IdOrden);
            personaOrden.IdOrden = IdOrden;
            personaOrden.NumeroDia = NumeroDia;
            personaOrden.NombreCompleto = $"{data["Nombre"].ToString()} {data["Apellidos"].ToString()}";
            personaOrden.Sexo = data["Sexo"].ToString();
            personaOrden.fecha = Fecha;

            return personaOrden;
        }

        private bool Validaciones(DataSet data)
        {
            if (data.Tables.Count > 0)
            {
                return false;
            }

            if (data.Tables[0].Rows.Count > 0)
            {
                return false;
            }
            return true;
        }

        public List<PersonaOrden> MapearLista(DataSet data)
        {

            var personasOrden = new List<PersonaOrden>();
            if (!Validaciones(data))
            {
                return personasOrden;
            }
            foreach(DataRow r in data.Tables[0].Rows)
            {
                personasOrden.Add(Mapear(r));
            }
            return personasOrden;
        }

    }
}
