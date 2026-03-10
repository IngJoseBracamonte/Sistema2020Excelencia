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
using Conexiones.Dto;

namespace Laboratorio
{
    public partial class ListaDeAnalisis : Form
    {
        List<AnalisisLaboratorio> analisisLaboratorios = new List<AnalisisLaboratorio>();
        public ListaDeAnalisis()
        {
            InitializeComponent();
        }

        private void ListaDeAnalisis_Load(object sender, EventArgs e)
        {
            CargarListaDeAnalisis();
        }

        private void CargarListaDeAnalisis()
        {
            dataGridView2.Rows.Clear();
            analisisLaboratorios = Conexion.SelectListadeAnalisis();
            foreach (var analisis in analisisLaboratorios)
            {
                dataGridView2.Rows.Add(analisis.IdAnalisis, analisis.NombreAnalisis);
            }
        }
        private void iconButton2_Click(object sender, EventArgs e)
        {
        
            if (dataGridView2.SelectedRows.Count > 0)
            {
                if (dataGridView2.CurrentCell.RowIndex < 0)
                {
                    MessageBox.Show("Por favor vuelva a seleccionar el Analisis");
                    return;
                }
                var index = dataGridView2.CurrentCell.RowIndex;
                var Id = dataGridView2.Rows[index].Cells["IdAnalisis"].Value.ToString();
                int.TryParse(Id, out int IdAnalisis);
                var analisisSeleccionado = analisisLaboratorios.Where(a=> a.IdAnalisis == IdAnalisis).FirstOrDefault();
                Form form41 = new Form36(analisisSeleccionado);
                form41.ShowDialog();
                CargarListaDeAnalisis();
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
