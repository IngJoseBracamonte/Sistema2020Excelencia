using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.Modelos
{
    public class TipoPago
    {
        public int IdTipoPago { get; set; }
        public string Descripcion { get; set; }
        public List<TipoPago> MapearLista(DataTable data)
        {
            List<TipoPago> Lista = new List<TipoPago>();

            foreach (DataRow r in data.Rows)
            {
                TipoPago tipoPago = new TipoPago();
                int.TryParse(r["idTipodePago"].ToString(), out int Id);
                tipoPago.IdTipoPago = Id;
                tipoPago.Descripcion = r["Descripcion"].ToString();
                Lista.Add(tipoPago);
            }
            return Lista;
        }
        public TipoPago Mapear(DataTable data)
        {
            TipoPago banco = new TipoPago();
            int.TryParse(data.Rows[0]["idTipodePago"].ToString(), out int Id);
            banco.IdTipoPago = Id;
            banco.Descripcion = data.Rows[0]["Descripcion"].ToString();
            return banco;
        }
    }
}
