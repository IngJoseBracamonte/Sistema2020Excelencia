using Conexiones.DbConnect;
using Conexiones.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio
{
    public partial class BancosForm : Form
    {
        Bancos bancoSeleccionado = new Bancos();
        List<Bancos> bancos = new List<Bancos>();
        public BancosForm()
        {
            InitializeComponent();
            ActualizarDGV();
        }
        private void ActualizarDGV()
        {
            bancos = new Conexion().ObtenerBancos();
            bancosBindingSource.DataSource = new BindingList<Bancos>();
            bancosBindingSource.DataSource = bancos;
            bancosBindingSource.ResetBindings(true);
        }

        private void BancosForm_Load(object sender, EventArgs e)
        {

        }

        private void Actualizar_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TNombreBanco.Text))
            {
                MessageBox.Show("El Campo no puede estar vacio");
            }
            bancoSeleccionado.NombreBanco = TNombreBanco.Text;
            Conexion.ActualizarBanco(bancoSeleccionado);
  
            Actualizar.Visible = false;
            Agregar.Visible = true;
            bancoSeleccionado = new Bancos();
            ActualizarDGV();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;

            if (bancoSeleccionado.IdBancos > 0)
            {
                string mensaje = $"¿Esta seguro de borrar el banco {bancoSeleccionado.NombreBanco}";
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    Conexion.BorrarBanco(bancoSeleccionado);
                    ActualizarDGV();
                }
            }
        }

        private void Agregar_Click(object sender, EventArgs e)
        {
            bancoSeleccionado = new Bancos();
            if (string.IsNullOrEmpty(TNombreBanco.Text))
            {
                MessageBox.Show("El Campo no puede estar vacio");
            }
            bancoSeleccionado.NombreBanco = TNombreBanco.Text;
            bancoSeleccionado = Conexion.InsertarBancos(bancoSeleccionado);
            if (bancoSeleccionado.IdBancos != 0)
            {
                MessageBox.Show("Agregado Satisfactoriamente");
            }
            else
            {
                MessageBox.Show("Ha ocurrido un erroor");
            }
            ActualizarDGV();
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
           
        }

        private void advancedDataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (advancedDataGridView1.SelectedRows.Count < 0)
            {
                MessageBox.Show("Seleccione el banco");
            }

            int index = advancedDataGridView1.CurrentCell.RowIndex;
            int.TryParse(advancedDataGridView1.Rows[index].Cells["IdBancos"].Value.ToString(), out int Idbanco);
            bancoSeleccionado = bancos.FirstOrDefault(b => b.IdBancos == Idbanco);
            Actualizar.Visible = true;
            Agregar.Visible = false;
            TNombreBanco.Text = bancoSeleccionado.NombreBanco;
        }
    }
}
