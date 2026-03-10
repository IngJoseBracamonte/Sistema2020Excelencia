using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class EstadisticaPersonascs : Form
    {
        public EstadisticaPersonascs()
        {
            InitializeComponent();
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void EstadisticaPersonascs_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            ds= Conexion.SelectPersonasEstadistica();
            dGPersonasFacturadas.DataSource = ds.Tables[0];
            if (dGPersonasFacturadas.Rows.Count > 0)
            {
                DataGridViewColumn column = dGPersonasFacturadas.Columns["IdPersona"];
                column.Visible = false;
            }
              
        }

        private void dataGridView2_CellClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dGPersonasFacturadas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
          
        }

        private void dGOrdenesPersona_CellClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void dGPersonasFacturadas_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
        
        }

        private void dGPersonasFacturadas_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int Index = dGPersonasFacturadas.CurrentCell.RowIndex;
            int IdPersona = 0;
            int.TryParse(dGPersonasFacturadas.Rows[Index].Cells["IdPersona"].Value.ToString(), out IdPersona);
            if (IdPersona > 0)
            {
                DataSet ds = new DataSet();
                ds = Conexion.SelectOrdenesPorPersona(IdPersona);
                dGOrdenesPersona.DataSource = ds.Tables[0];
            }
        }

        private void dGOrdenesPersona_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int Index = dGOrdenesPersona.CurrentCell.RowIndex;
            int IdOrden = 0;
            int.TryParse(dGOrdenesPersona.Rows[Index].Cells["IdOrden"].Value.ToString(), out IdOrden);
            if (IdOrden > 0)
            {
                DataSet ds = new DataSet();
                ds = Conexion.SelectPerfilesPorOrden(IdOrden);
                dGExamenes.DataSource = ds.Tables[0];
            }
        }
    }
}
