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
using Conexiones.Modelos;

namespace Laboratorio
{
    public partial class BancosTipoPago : Form
    {

        int idTipoBancoSeleccionado = 0;
        public BancosTipoPago()
        {
            InitializeComponent();
            cargarTiposDePago();
            cargarBancos();
           
        }

        private void BancosTipoPago_Load(object sender, EventArgs e)
        {
            
        }

        private void cargarTiposDePago()
        {
            tipoPagoBindingSource.DataSource = new Conexion().ObtenerTiposDePago();
            tipoPagoBindingSource.ResetBindings(true);
        }
        private void cargarBancos()
        {
            bancosBindingSource.DataSource = new Conexion().ObtenerBancos();
            bancosBindingSource.ResetBindings(true);
        }
        private void cargarBancosPorTipoDepago(int idTipoPago)
        {
            bancosBindingSource1.DataSource = new Conexion().ObtenerBancosPorTipoDepago(idTipoPago);
            bancosBindingSource1.ResetBindings(true);
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (BancosAsignadosDGV.SelectedRows.Count < 0)
            {
                MessageBox.Show("Seleccione un tipo de pago");
                return;
            }

            int Index = BancosAsignadosDGV.CurrentCell.RowIndex;
            int.TryParse(BancosAsignadosDGV.Rows[Index].Cells["idBancosAgregado"].Value.ToString(), out int IdBanco);


            string mensaje = $"Ha seleccionado el {BancosAsignadosDGV.Rows[Index].Cells["nombreBancoAsignado"].Value},¿Desea Eliminarlo?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
            if (dialog == DialogResult.Yes)
            {
                Conexion.BorrarBancoEnTipoDepago(idTipoBancoSeleccionado, IdBanco);
                cargarBancosPorTipoDepago(idTipoBancoSeleccionado);
            }


        }

        private void advancedDataGridView3_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            if (BancosDGV.SelectedRows.Count < 0)
            {
                MessageBox.Show("Seleccione un tipo de pago");
                return;
            }

 

            int Index = BancosDGV.CurrentCell.RowIndex;
            int.TryParse(BancosDGV.Rows[Index].Cells["IdBancos"].Value.ToString(), out int IdBanco);
            var Banco = bancosBindingSource1.List.OfType<Bancos>().ToList().Find(B => B.IdBancos == IdBanco);
            if (Banco != null)
            {
                MessageBox.Show("Banco ya ingresado");
                return;
            }


            string mensaje = $"Ha seleccionado el {BancosDGV.Rows[Index].Cells["nombreBanco"].Value},¿Desea Ingresarlo?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
            if (dialog == DialogResult.Yes)
            {
                Conexion.InsertarBancosPorTipoDePago(idTipoBancoSeleccionado, IdBanco);
                cargarBancosPorTipoDepago(idTipoBancoSeleccionado);
            }
             

        }

        private void tipoPagoDGV_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (tipoPagoDGV.SelectedRows.Count < 0)
            {
                MessageBox.Show("Seleccione un tipo de pago");
                return;
            }

            int Index = tipoPagoDGV.CurrentCell.RowIndex;
            int.TryParse(tipoPagoDGV.Rows[Index].Cells["IdTipoPago"].Value.ToString(), out int IdTipoPago);
            MessageBox.Show($"Has Seleccionado: '{tipoPagoDGV.Rows[Index].Cells["descripcion"].Value}'");
            idTipoBancoSeleccionado = IdTipoPago;
            cargarBancosPorTipoDepago(IdTipoPago);
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
