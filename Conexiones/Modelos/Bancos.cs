using System;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.Modelos
{
    public class Bancos
    {
        public int IdBancos { get; set; }
        public string NombreBanco { get; set; }

        public List<Bancos> MapearLista(DataTable data)
        {
            List<Bancos> Lista = new List<Bancos>();

            foreach (DataRow r in data.Rows)
            {
                Bancos banco = new Bancos();
                int.TryParse(r["IdBancos"].ToString(), out int Id);
                banco.IdBancos = Id;
                banco.NombreBanco = r["NombreBanco"].ToString();
                Lista.Add(banco);
            }
            return Lista;
        }
        public Bancos Mapear(DataTable data)
        {
            Bancos banco = new Bancos();
            int.TryParse(data.Rows[0]["IdBancos"].ToString(), out int Id);
            banco.IdBancos = Id;
            banco.NombreBanco = data.Rows[0]["NombreBanco"].ToString();
            return banco;
        }
    }
}
