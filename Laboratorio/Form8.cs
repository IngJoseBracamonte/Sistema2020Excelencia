using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form8 : Form
    {
        Double Proteinas;
        Double ProteinasVM;
        Double ProteinasVm;
        Double Albuminas;
        Double AlbuminasVM;
        Double AlbuminasVm;
        Double Globulinas;
        Double GlobulinasVM;
        Double GlobulinasVm;
        Double Relacion;
        Double RelacionVM;
        Double RelacionVm;
        List<string> Ordenes = new List<string>();
        int PoscionActual, IdOrden, IdAnalisis,IdUser;
        public Form8(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void Form8_Load(object sender, EventArgs e)
        {
            DataSet Posiciones = new DataSet();
            DataSet Fecha = new DataSet();
            Fecha = Conexion.FechaDeOrden(IdOrden);
            Posiciones = Conexion.CantidadesDeExamenes(IdAnalisis, Convert.ToDateTime(Fecha.Tables[0].Rows[0]["Fecha"].ToString()).ToString("yyyy-MM-dd"));
            if (Posiciones.Tables.Count != 0)
            {
                if (Posiciones.Tables[0].Rows.Count != 0)
                {
                    foreach (DataRow r in Posiciones.Tables[0].Rows)
                    {
                        Ordenes.Add(r["IdOrden"].ToString());
                    }
                }
            }
            PoscionActual = Convert.ToInt32(Ordenes.IndexOf(IdOrden.ToString()));

            if (Ordenes.Count != 0 && PoscionActual > (-1))
            {
                if (Ordenes[0] == Ordenes[PoscionActual])
                {
                    Izquierda.Visible = false;
                }
                if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                {
                    Derecha.Visible = false;
                }
            }
            else
            {
                Izquierda.Visible = false;
                Derecha.Visible = false;
            }
            DataSet ds = new DataSet();
            ds = Conexion.SELECTAnalisisFinal3(IdOrden);
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    switch (Convert.ToInt32(row["IdAnalisis"].ToString()))
                    {

                        case 178:
                            ProteinasVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            ProteinasVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox1.Text = row["ValorResultado"].ToString();
                            Proteina.Text = row["Unidad"].ToString();
                            ProteinasV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();

                            break;
                        case 179:
                            AlbuminasVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            AlbuminasVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox2.Text = row["ValorResultado"].ToString();
                            Albumina.Text = row["Unidad"].ToString();
                            AlbuminaV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();
                            break;
                        case 186:
                            GlobulinasVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            GlobulinasVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox3.Text = row["ValorResultado"].ToString();
                            Globulina.Text = row["Unidad"].ToString();
                            GlobulinasV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();
                            break;
                        case 187:
                            RelacionVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            RelacionVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox4.Text = row["ValorResultado"].ToString();
                            RelAg.Text = row["Unidad"].ToString();
                            RelAgV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }


        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    string cmd = String.Format("HoraValidacion = '{0}',EstadoDeResultado = 2,IdUsuario = {1}", DateTime.Now.ToString("hh:mm:ss"), IdUser);
                    Conexion.InsertarFinal("", "", IdUser, IdOrden, 177);
                    Conexion.InsertarFinal(textBox1.Text, "", IdUser, IdOrden, 178);
                    Conexion.InsertarFinal(textBox2.Text, "", IdUser, IdOrden, 179);
                    Conexion.InsertarFinal(textBox3.Text, "", IdUser, IdOrden, 186);
                    Conexion.InsertarFinal(textBox4.Text, "", IdUser, IdOrden, 187);
                }
            }

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form8_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox3_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox4_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !((TextBox)sender).Text.Contains(','))
            {
                e.KeyChar = ',';
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                e.Handled = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            if (textBox3.Text != "")
            {
                if (Convert.ToDouble(textBox3.Text) < GlobulinasVm)
                {
                    textBox3.ForeColor = Color.Blue;

                }
                else if (Convert.ToDouble(textBox3.Text) > GlobulinasVM)
                {
                    textBox3.ForeColor = Color.Red;
                }
                else
                {
                    textBox3.ForeColor = Color.Black;
                }

            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void textBox2_KeyUp(object sender, KeyEventArgs e)
        {
            if (textBox1.Text != "" && textBox2.Text != "")
            {
                Proteinas = Convert.ToDouble(textBox1.Text);
                Albuminas = Convert.ToDouble(textBox2.Text);
                Globulinas = Proteinas - Albuminas;
                textBox3.Text = Globulinas.ToString();
                if (Globulinas != 0)
                {
                    Relacion = Math.Round(Albuminas / Globulinas, 2);
                    textBox4.Text = Relacion.ToString();
                }
                else
                {
                    textBox4.Text = "0";
                }
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            if (textBox4.Text != "")
            {
                if (Convert.ToDouble(textBox4.Text) < RelacionVm)
                {
                    textBox4.ForeColor = Color.Blue;
                }
                else if (Convert.ToDouble(textBox4.Text) > RelacionVM)
                {
                    textBox4.ForeColor = Color.Red;
                }
                else
                {
                    textBox4.ForeColor = Color.Black;
                }

            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != "")
            {
                if (Convert.ToDouble(textBox1.Text) < ProteinasVm)
                {
                    textBox1.ForeColor = Color.Blue;
                }
                else if (Convert.ToDouble(textBox1.Text) > ProteinasVM)
                {
                    textBox1.ForeColor = Color.Red;
                }
                else
                {
                    textBox1.ForeColor = Color.Black;
                }
                if (textBox2.Text != "")
                {
                    Proteinas = Convert.ToDouble(textBox1.Text);
                    Albuminas = Convert.ToDouble(textBox2.Text);
                    Globulinas = Proteinas - Albuminas;
                    textBox3.Text = Globulinas.ToString();
                    if (Globulinas != 0)
                    {
                        Relacion = Math.Round(Albuminas / Globulinas, 2);
                        textBox4.Text = Relacion.ToString();
                    }
                    else
                    {
                        textBox4.Text = "0";
                    }
                }
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                if (Convert.ToDouble(textBox2.Text) < AlbuminasVm)
                {
                    textBox2.ForeColor = Color.Blue;

                }
                else if (Convert.ToDouble(textBox2.Text) > AlbuminasVM)
                {
                    textBox2.ForeColor = Color.Red;
                }
                else
                {
                    textBox2.ForeColor = Color.Black;
                }
                if (textBox1.Text != "")
                {
                    Proteinas = Convert.ToDouble(textBox1.Text);
                    Albuminas = Convert.ToDouble(textBox2.Text);
                    Globulinas = Proteinas - Albuminas;
                    textBox3.Text = Globulinas.ToString();
                    if (Globulinas != 0)
                    {
                        Relacion = Math.Round(Albuminas / Globulinas, 2);
                        textBox4.Text = Relacion.ToString();
                    }
                }
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            int IdOrden = 0;
            //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    string cmd = String.Format("HoraValidacion = '{0}',EstadoDeResultado = 2,IdUsuario = {1}", DateTime.Now.ToString("hh:mm:ss"), IdUser);
                    Conexion.InsertarSinValidar("", "", IdUser, IdOrden, 177);
                    Conexion.InsertarSinValidar(textBox1.Text, "", IdUser, IdOrden, 178);
                    Conexion.InsertarSinValidar(textBox2.Text, "", IdUser, IdOrden, 179);
                    Conexion.InsertarSinValidar(textBox3.Text, "", IdUser, IdOrden, 186);
                    Conexion.InsertarSinValidar(textBox4.Text, "", IdUser, IdOrden, 187);
                }
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            MoverDerecha();
            cargarDatosPaciente();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            Moverizquierda();
            cargarDatosPaciente();
        }
        private void Moverizquierda()
        {
            if (PoscionActual > 0)
            {
                PoscionActual = PoscionActual - 1;
                IdOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
                if (Ordenes.Count != 0)
                {
                    if (Ordenes[0] == Ordenes[PoscionActual])
                    {
                        Izquierda.Visible = false;
                    }
                    else
                    {
                        Izquierda.Visible = true;
                    }
                    if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                    {
                        Derecha.Visible = false;
                    }
                    else
                    {
                        Derecha.Visible = true;
                    }
                }
                else
                {
                    Izquierda.Visible = false;
                    Derecha.Visible = false;
                }
            }
        }
        private void MoverDerecha()
        {
            //IdOrden
            //Izquierda
            //Derecha
            if (PoscionActual < Ordenes.Count - 1)
            {
                PoscionActual = PoscionActual + 1;
                IdOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
                if (Ordenes.Count != 0)
                {
                    if (Ordenes[0] == Ordenes[PoscionActual])
                    {
                        Izquierda.Visible = false;
                    }
                    else
                    {
                        Derecha.Visible = true;
                    }
                    if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                    {
                        Derecha.Visible = false;
                    }
                    else
                    {
                        Izquierda.Visible = true;
                    }
                }
                else
                {
                    Derecha.Visible = false;
                    Izquierda.Visible = false;
                }

            }
        }
        private void cargarDatosPaciente()
        {
            DataSet ds = new DataSet();
            ds = Conexion.SELECTAnalisisFinal3(IdOrden);
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    switch (Convert.ToInt32(row["IdAnalisis"].ToString()))
                    {

                        case 178:
                            ProteinasVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            ProteinasVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox1.Text = row["ValorResultado"].ToString();
                            Proteina.Text = row["Unidad"].ToString();
                            ProteinasV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();

                            break;
                        case 179:
                            AlbuminasVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            AlbuminasVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox2.Text = row["ValorResultado"].ToString();
                            Albumina.Text = row["Unidad"].ToString();
                            AlbuminaV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();
                            break;
                        case 186:
                            GlobulinasVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            GlobulinasVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox3.Text = row["ValorResultado"].ToString();
                            Globulina.Text = row["Unidad"].ToString();
                            GlobulinasV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();
                            break;
                        case 187:
                            RelacionVM = Convert.ToDouble(row["ValorMayor"].ToString());
                            RelacionVm = Convert.ToDouble(row["ValorMenor"].ToString());
                            textBox4.Text = row["ValorResultado"].ToString();
                            RelAg.Text = row["Unidad"].ToString();
                            RelAgV.Text = row["ValorMenor"].ToString() + " - " + row["ValorMayor"].ToString();
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
