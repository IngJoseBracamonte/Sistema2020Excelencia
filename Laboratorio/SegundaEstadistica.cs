using Conexiones;
using Conexiones.DbConnect;
using SpreadsheetLight;
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
    public partial class SegundaEstadistica : Form
    {
        bool columa0,columna1,columna2,columna3,columna4,columna5,columna6,columna7;
        DataTable ds = new DataTable();
        public SegundaEstadistica()
        {
            ds.Columns.Add("Nombre");
            ds.Columns.Add("Telefono");
            ds.Columns.Add("Dia", typeof(int));
            ds.Columns.Add("Mes");
            ds.Columns.Add("Año", typeof(int));
            ds.Columns.Add("Edad", typeof(int));
            ds.Columns.Add("NombrePerfil");
            ds.Columns.Add("Convenio");
            InitializeComponent();
        }

        private void OrdenarTelefono_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columna1)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[1].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columna1 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[1].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columna1 = true;
            }

            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.
            dataGridView1.DataSource = ds;

            CambiarIconos(1, Type);
        }

        private void OrdenarDia_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columna2)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[2].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columna2 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[2].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columna2 = true;
            }

            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.
            dataGridView1.DataSource = ds;

            CambiarIconos(2, Type);
        }

        private void OrdenarMes_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columna3)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[3].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columna3 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[3].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columna3 = true;
            }

            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.
            dataGridView1.DataSource = ds;

            CambiarIconos(3, Type);
        }

        private void OrdenarAño_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columna4)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[4].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columna4 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[4].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columna4 = true;
            }

            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.

            CambiarIconos(4, Type);
        }

        private void OrdenarEdad_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columna5)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[5].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columna5 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[5].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columna5 = true;
            }
            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.
            dataGridView1.DataSource = ds;

            CambiarIconos(5, Type);
        }

        private void OrdenarPerfil_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columna6)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[6].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columna6 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[6].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columna6 = true;
            }

            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.
            dataGridView1.DataSource = ds;

            CambiarIconos(6, Type);
        }

        private void OrdenarConvenio_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columna7)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[7].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columna7 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[7].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columna7 = true;
            }

            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.
            dataGridView1.DataSource = ds;

            CambiarIconos(7, Type);
        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                dataGridView1.DataSource = ds.DefaultView.Table.AsEnumerable().GroupBy(r => r.Field<string>("Nombre")).Select(g => g.First()).CopyToDataTable();
            }
            else
            {
                dataGridView1.DataSource=ds;
            }
           
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            ds.Clear();

            List<OrdenesEstadistica> ordenesEstadistica = new List<OrdenesEstadistica>();
            string cmd, cmd2;
            cmd = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            cmd2 = dateTimePicker2.Value.ToString("yyyy-MM-dd");
            ordenesEstadistica = Conexion.Estadistica2(cmd, cmd2);
            this.Cursor = Cursors.WaitCursor;
            foreach (var orden in ordenesEstadistica)
            {
                foreach (var Perfiles in orden.perfiles)
                {
                    //Nombre,Telefono,Fecha,Edad,Analisis,Convenio
                    DateTime fecha = orden.datosPaciente.Fecha;
                    ds.Rows.Add($"{orden.datosPaciente.Nombre} {orden.datosPaciente.Apellidos}", $"{orden.datosPaciente.CodigoCelular}-{orden.datosPaciente.Celular}", fecha.ToString("dd"), fecha.ToString("MMMM"), fecha.ToString("yyyy"), orden.datosPaciente.Edad, Perfiles.NombrePerfil, orden.convenio.Nombre);
                }

            }

            dataGridView1.DataSource = ds;
            if (dataGridView1.Rows.Count > 0)
            {
                DataGridViewColumn NombreCompleto = dataGridView1.Columns[0];
                NombreCompleto.Width = 200;
                DataGridViewColumn Telefono = dataGridView1.Columns[1];
                Telefono.Width = 119;
                DataGridViewColumn Dia = dataGridView1.Columns[2];
                Dia.Width = 50;
                DataGridViewColumn Mes = dataGridView1.Columns[3];
                Mes.Width = 50;
                DataGridViewColumn Anio = dataGridView1.Columns[4];
                Anio.Width = 50;
                DataGridViewColumn Edad = dataGridView1.Columns[5];
                Edad.Width = 150;
                DataGridViewColumn Perfil = dataGridView1.Columns[6];
                Perfil.Width = 300;
                DataGridViewColumn Convenio = dataGridView1.Columns[7];
                Convenio.Width = 100;
            }
           this.Cursor = Cursors.Default;

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            try
            {
                if (dataGridView1.Rows.Count != 0)
                {

                    int C = 1;
                    int R = 2;
                    SLDocument sl = new SLDocument();
                    SLStyle style = new SLStyle();
                    style.Font.FontSize = 12;
                    style.Font.Bold = true;


                    foreach (DataGridViewColumn colum in dataGridView1.Columns)
                    {
                        sl.SetCellValue(1, C, colum.HeaderText.ToString());
                        C++;
                    }
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        for (int i = 1; i < C; i++)
                        {
                            sl.SetCellValue(R, i, row.Cells[i - 1].Value.ToString());
                        }
                        R++;
                    }
                    sl.SetColumnWidth(1, 11);
                    sl.SetColumnWidth(2, 9);
                    sl.SetColumnWidth(3, 6);
                    sl.SetColumnWidth(4, 12);
                    sl.SetColumnWidth(5, 40);
                    sl.SetColumnWidth(6, 20);
                    sl.SetColumnWidth(7, 20);
                    sl.SetColumnWidth(20, 20);
                    saveFileDialog1.Filter = "Excel|*.xlsx";
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        sl.SaveAs(saveFileDialog1.FileName);
                        saveFileDialog1.FileName = "";
                    }
                }
                else
                {
                    MessageBox.Show("No hay datos para mostrar en la tabla");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            string Type = "";
            if (columa0)
            {
                ds.DefaultView.Sort = dataGridView1.Columns[0].Name + " DESC"; // Get Sorted Column name and sort it in Descending order
                Type = "DESC";
                columa0 = false;
            }
            else
            {
                ds.DefaultView.Sort = dataGridView1.Columns[0].Name + " ASC";  // Otherwise sort it in Ascending order
                Type = "ASC";
                columa0 = true;
            }
         
            ds = ds.DefaultView.ToTable(); // The Sorted View converted to DataTable and then assigned to table object.
            dataGridView1.DataSource = ds;

            CambiarIconos(0, Type);
        }

        private void CambiarIconos(int Numero,string Type)
        {
            //Ordenar Por Nombre de Persona
            if (Numero == 0 && Type == "ASC")
            {
                OrdenarNombre.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            } else if(Numero == 0 && Type == "DESC")
            {
                OrdenarNombre.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarNombre.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }



            //Ordenar Por Telefono
            if (Numero == 1 && Type == "ASC")
            {
                OrdenarTelefono.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            }
            else if (Numero == 1 && Type == "DESC")
            {
                OrdenarTelefono.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarTelefono.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }

            //Ordenar Por Dia
            if (Numero == 2 && Type == "ASC")
            {
                OrdenarDia.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            }
            else if (Numero == 2 && Type == "DESC")
            {
                OrdenarDia.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarDia.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }

            //Ordenar Por Mes
            if (Numero == 3 && Type == "ASC")
            {
                OrdenarMes.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            }
            else if (Numero == 3 && Type == "DESC")
            {
                OrdenarMes.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarMes.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }

            //Ordenar Por Mes
            if (Numero == 4 && Type == "ASC")
            {
                OrdenarAño.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            }
            else if (Numero == 4 && Type == "DESC")
            {
                OrdenarAño.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarAño.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }


            //Ordenar Por Edad
            if (Numero == 5 && Type == "ASC")
            {
                OrdenarEdad.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            }
            else if (Numero == 5 && Type == "DESC")
            {
                OrdenarEdad.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarEdad.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }


            //Ordenar Por Edad
            if (Numero == 6 && Type == "ASC")
            {
                OrdenarPerfil.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            }
            else if (Numero == 6 && Type == "DESC")
            {
                OrdenarPerfil.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarPerfil.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }


            //Ordenar Por Edad
            if (Numero == 7 && Type == "ASC")
            {
                OrdenarConvenio.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltUp;
            }
            else if (Numero == 7 && Type == "DESC")
            {
                OrdenarConvenio.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltDown;
            }
            else
            {
                OrdenarConvenio.IconChar = FontAwesome.Sharp.IconChar.LongArrowAltRight;
            }


        }

        private void SegundaEstadistica_Load(object sender, EventArgs e)
        {

        }

        private void dataGridView1_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
           

        }
    }
}
