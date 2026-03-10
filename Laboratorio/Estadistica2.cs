using Conexiones;
using Conexiones.DbConnect;
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
    public partial class Estadistica_2 : Form
    {
        public Estadistica_2()
        {
            InitializeComponent();
        }

        private void Estadistica_2_Load(object sender, EventArgs e)
        {

        }

        private void advancedDataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            List<OrdenesEstadistica> ordenesEstadistica = new List<OrdenesEstadistica>();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            ordenesEstadistica = Conexion.Estadistica2(cmd, cmd2);
            advancedDataGridView1.Rows.Clear();
            this.Cursor = Cursors.WaitCursor;
            foreach (var orden in ordenesEstadistica)
            {
                foreach (var Perfiles in orden.perfiles)
                {
                    //Nombre,Telefono,Fecha,Edad,Analisis,Convenio
                    DateTime fecha = orden.datosPaciente.Fecha;
                    advancedDataGridView1.Rows.Add($"{orden.datosPaciente.Nombre} {orden.datosPaciente.Apellidos}", $"{orden.datosPaciente.CodigoCelular}-{orden.datosPaciente.Celular}", fecha.ToString("dd"), fecha.ToString("MMMM"), fecha.ToString("yyyy"), orden.datosPaciente.Edad,Perfiles.NombrePerfil,orden.convenio.Nombre);
                }
               
            }

            this.Cursor = Cursors.Default;

        }
    }
}
