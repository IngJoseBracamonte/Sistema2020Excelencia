using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form28 : Form
    {
        public Double Neutrofilos = 0, linfocitos = 0, Monocitos = 0, Eosinofilos = 0, Basofilos = 0, Neutrofilos2 = 0, linfocitos2 = 0, Monocitos2 = 0, Eosinofilos2 = 0, Basofilos2 = 0, Hematies = 0, Hemoglobina = 0, Hematocritos = 0, VCM = 0, HCM = 0, CHCM = 0, Plaquetas = 0;
        public string TeclaLeu, TeclaNeu, TeclaLin, TeclaMono, TeclaEos, TeclaBaso, TeclaPla;
        private int IdUser;
        private int IdOrden;
        private int IdAnalisis;
        private Double Total = 0, Neu = 0, Lin = 0, Mono = 0, Eos = 0, Baso = 0;
        bool Reportar = false;
        public Form28(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void label4_Click(object sender, EventArgs e)
        {

        }

        private void label28_Click(object sender, EventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (Reportar == true)
            {
                if (keyData.ToString() == TeclaNeu.ToUpper() && TeclaNeu.ToUpper() != "")
                {
                    Double Sumar;
                    Total++;
                    Neu++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox1.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox2.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox3.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox4.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox10.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;

                }
                if (keyData.ToString() == TeclaLin.ToUpper() && TeclaLin.ToUpper() != "")
                {
                    Double Sumar;
                    Total++;
                    Lin++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox1.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox2.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox3.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox4.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox10.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaMono.ToUpper() && TeclaMono.ToUpper() != "")
                {
                    Double Sumar;
                    Total++;
                    Mono++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox1.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox2.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox3.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox4.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox10.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaEos.ToUpper() && TeclaEos.ToUpper() != "")
                {

                    Double Sumar;
                    Total++;
                    Eos++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox1.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox2.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox3.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox4.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox10.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaBaso.ToUpper() && TeclaBaso.ToUpper() != "")
                {

                    Double Sumar;
                    Total++;
                    Baso++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox1.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox2.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox3.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox4.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        textBox10.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaLeu.ToUpper() && TeclaLeu.ToUpper() != "")
                {
                    if (textBox14.Text != "")
                    {
                        Double Sumar;
                        Sumar = Convert.ToDouble(textBox14.Text) + 1;
                        textBox14.Text = Sumar.ToString();


                    }
                    else
                    {
                        Double Sumar;
                        Sumar = 1;
                        textBox14.Text = Sumar.ToString();
                    }
                    return true;
                }
                if (keyData.ToString() == TeclaPla.ToUpper() && TeclaPla.ToUpper() != "")
                {

                    if (textBox16.Text != "")
                    {
                        Double Sumar;
                        Sumar = Convert.ToDouble(textBox16.Text) + 1;
                        textBox16.Text = Sumar.ToString();

                    }
                    else
                    {
                        int Sumar;
                        Sumar = 1;
                        textBox16.Text = Sumar.ToString();
                    }
                    return true;
                }

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void Conteo()
        {
            if (Total == 100)
            {
                checkBox1.Checked = false;
                MessageBox.Show("La Cantidad de Celulas contadas es 100");
            }
        }

        private void Form28_Load(object sender, EventArgs e)
        {

            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            ds = Conexion.HematologiaEspecial(IdOrden, IdAnalisis);
            ds1 = Conexion.TeclasYPrivilegios(IdUser);
            if (ds1.Tables[0].Rows.Count != 0)
            {

                TeclaLeu = ds1.Tables[0].Rows[0]["Leucocitos"].ToString().ToUpper();
                TeclaNeu = ds1.Tables[0].Rows[0]["Neutrofilos"].ToString().ToUpper();
                TeclaLin = ds1.Tables[0].Rows[0]["Linfocitos"].ToString().ToUpper();
                TeclaMono = ds1.Tables[0].Rows[0]["Monocitos"].ToString().ToUpper();
                TeclaEos = ds1.Tables[0].Rows[0]["Eosinofilos"].ToString().ToUpper();
                TeclaBaso = ds1.Tables[0].Rows[0]["Basofilos"].ToString().ToUpper();
                TeclaPla = ds1.Tables[0].Rows[0]["Plaquetas"].ToString().ToUpper();
                LabelLeu.Text = "T: " + ds1.Tables[0].Rows[0]["Leucocitos"].ToString().ToUpper();
                LabelNeu.Text = "T: " + ds1.Tables[0].Rows[0]["Neutrofilos"].ToString().ToUpper();
                LabelLin.Text = "T: " + ds1.Tables[0].Rows[0]["Linfocitos"].ToString().ToUpper();
                LabelMono.Text = "T: " + ds1.Tables[0].Rows[0]["Monocitos"].ToString().ToUpper();
                LabelEos.Text = "T: " + ds1.Tables[0].Rows[0]["Eosinofilos"].ToString().ToUpper();
                LabelBaso.Text = "T: " + ds1.Tables[0].Rows[0]["Basofilos"].ToString().ToUpper();
                LabelPla.Text = "T: " + ds1.Tables[0].Rows[0]["Plaquetas"].ToString().ToUpper();

            }
            else
            {
                checkBox1.Enabled = false;
            }

            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = edad.ToString();
                textBox14.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                textBox1.Text = ds.Tables[0].Rows[0]["Neutrofilos"].ToString();
                textBox2.Text = ds.Tables[0].Rows[0]["Linfocitos"].ToString();
                textBox4.Text = ds.Tables[0].Rows[0]["Monocitos"].ToString();
                textBox3.Text = ds.Tables[0].Rows[0]["Eosinofilos"].ToString();
                textBox10.Text = ds.Tables[0].Rows[0]["Basofilos"].ToString();
                textBox20.Text = ds.Tables[0].Rows[0]["hematies"].ToString();
                textBox19.Text = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                textBox18.Text = ds.Tables[0].Rows[0]["Hematocritos"].ToString();
                textBox17.Text = ds.Tables[0].Rows[0]["VCM"].ToString();
                textBox12.Text = ds.Tables[0].Rows[0]["HCM"].ToString();
                textBox11.Text = ds.Tables[0].Rows[0]["CHCM"].ToString();
                textBox16.Text = ds.Tables[0].Rows[0]["plaquetas"].ToString();
                textBox7.Text = ds.Tables[0].Rows[0]["Neutrofilos2"].ToString();
                textBox8.Text = ds.Tables[0].Rows[0]["Linfocitos2"].ToString();
                textBox6.Text = ds.Tables[0].Rows[0]["Monocitos2"].ToString();
                textBox5.Text = ds.Tables[0].Rows[0]["Eosinofilos2"].ToString();
                textBox9.Text = ds.Tables[0].Rows[0]["Basofilos2"].ToString();
                textBox15.Text = ds.Tables[0].Rows[0]["Frotis"].ToString();
            }
            catch
            {
                
            }

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (textBox1.Text != " " && textBox1.Text != "")
            {
                if (textBox1.Text == ",")
                {
                    textBox1.Text = "0,";
                }
                //NEUTROFILOS
                Neutrofilos = Convert.ToDouble(textBox1.Text);
                if (Neutrofilos < 40.0)
                {
                    textBox1.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Neutrofilos > 70.0)
                {
                    textBox1.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox1.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox1.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox1.Text)) / 100;
                textBox7.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //NEUTROFILOS
            if (textBox2.Text != " " && textBox2.Text != "")
            {
                if (textBox2.Text == ",")
                {
                    textBox2.Text = "0,";
                }
                //NEUTROFILOS
                linfocitos = Convert.ToDouble(textBox2.Text);
                if (linfocitos < 18.0)
                {
                    textBox2.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos > 48.0)
                {
                    textBox2.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox2.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox2.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox2.Text)) / 100;
                textBox8.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            //Monocitos
            if (textBox4.Text != " " && textBox4.Text != "")
            {
                if (textBox4.Text == ",")
                {
                    textBox4.Text = "0,";
                }
                Monocitos = Convert.ToDouble(textBox4.Text);
                //Monocitos
                if (Monocitos < 3.0)
                {
                    textBox4.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos > 12.0)
                {
                    textBox4.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox4.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox4.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox4.Text)) / 100;
                textBox6.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //Eosinofilos 
            if (textBox3.Text != " " && textBox3.Text != "")
            {
                if (textBox3.Text == ",")
                {
                    textBox3.Text = "0,";
                }
                Eosinofilos = Convert.ToDouble(textBox3.Text);

                //Eosinofilos
                if (Eosinofilos < 0.6)
                {
                    textBox3.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos > 7.3)
                {
                    textBox3.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox3.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox3.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox3.Text)) / 100;
                textBox5.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (textBox10.Text != " " && textBox10.Text != "")
            {
                if (textBox10.Text == ",")
                {
                    textBox10.Text = "0,";
                }
                Basofilos = Convert.ToDouble(textBox10.Text);
                //Basofilos
                if (Basofilos < 0)
                {
                    textBox10.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos > 1.7)
                {
                    textBox10.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox10.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox10.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox10.Text)) / 100;
                textBox9.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {

            if (textBox20.Text != " " && textBox20.Text != "")
            {
                if (textBox20.Text == ",")
                {
                    textBox20.Text = "0,";
                }

                Hematies = Convert.ToDouble(textBox20.Text);
                //Hematies
                if (Hematies < 3.5)
                {
                    textBox20.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematies > 5.5)
                {
                    textBox20.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox20.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }
        private void textBox19_TextChanged(object sender, EventArgs e)
        {

            if (textBox19.Text != " " && textBox19.Text != "")
            {
                if (textBox19.Text == ",")
                {
                    textBox19.Text = "0,";
                }
                Hemoglobina = Convert.ToDouble(textBox19.Text);
                //Hemoglobina
                if (Hemoglobina < 11)
                {
                    textBox19.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hemoglobina > 16)
                {
                    textBox19.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox19.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }
        private void textBox18_TextChanged(object sender, EventArgs e)
        {

            if (textBox18.Text != " " && textBox18.Text != "")
            {
                if (textBox18.Text == ",")
                {
                    textBox18.Text = "0,";
                }
                Hematocritos = Convert.ToDouble(textBox18.Text);
                //Hematocritos
                if (Hematocritos < 35)
                {
                    textBox18.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematocritos > 54)
                {
                    textBox18.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox18.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox18.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox18.Text) * 0.32);
                textBox19.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox18.Text) * 0.11);
                textBox20.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox18.Text) * 10) / Convert.ToDouble(textBox20.Text);
                textBox17.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox19.Text) * 100) / Convert.ToDouble(textBox18.Text);
                textBox11.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox19.Text) * 10) / Convert.ToDouble(textBox20.Text);
                textBox12.Text = Resultado.ToString("0.##");

            }
        }
        private void textBox17_TextChanged(object sender, EventArgs e)
        {

            if (textBox17.Text != " " && textBox17.Text != "")
            {
                if (textBox17.Text == ",")
                {
                    textBox17.Text = "0,";
                }
                VCM = Convert.ToDouble(textBox17.Text);
                //VCM
                if (VCM < 80)
                {
                    textBox17.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (VCM > 100)
                {
                    textBox17.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox17.ForeColor = Color.FromArgb(0, 0, 0);

            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

            if (textBox12.Text != " " && textBox12.Text != "")
            {

                if (textBox12.Text == ",")
                {
                    textBox12.Text = "0,";
                }
                HCM = Convert.ToDouble(textBox12.Text);
                //HCM
                if (HCM < 27)
                {
                    textBox12.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (HCM > 34)
                {
                    textBox12.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox12.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if (textBox11.Text != " " && textBox11.Text != "")
            {

                if (textBox11.Text == ",")
                {
                    textBox11.Text = "0,";
                }
                CHCM = Convert.ToDouble(textBox11.Text);
                //CHCM
                if (CHCM < 31)
                {
                    textBox11.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (CHCM > 34)
                {
                    textBox11.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox11.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            if (textBox16.Text != " " && textBox16.Text != "")
            {
                if (textBox16.Text == ",")
                {
                    textBox16.Text = "0,";
                }

                Plaquetas = Convert.ToDouble(textBox16.Text);
                //Plaquetas
                if (Plaquetas < 150)
                {
                    textBox16.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Plaquetas > 450)
                {
                    textBox16.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox16.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            string cmd3 = "EstadoDeResultado = 2";

            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();

            ds = Conexion.VerificarHematologiaEspecial(IdOrden);
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {

                        string MS;
                        MS = Conexion.InsertarHematologiaEspecial(IdOrden.ToString(),DateTime.Now.ToString("hh:mm:ss"),IdAnalisis.ToString(), textBox15.Text, textBox1.Text, textBox2.Text, textBox4.Text, textBox3.Text, textBox10.Text, textBox20.Text, textBox19.Text, textBox18.Text, textBox17.Text, textBox12.Text, textBox11.Text, textBox16.Text, textBox7.Text, textBox8.Text, textBox6.Text,IdUser.ToString(), textBox5.Text, textBox9.Text, textBox14.Text);
                        MessageBox.Show(MS);
                }
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta operacion");
            }

            this.Close();
        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

            if (textBox7.Text != " " && textBox7.Text != "")
            {
                if (textBox7.Text != " " && textBox7.Text != "")
                {
                    if (textBox7.Text == ",")
                    {
                        textBox7.Text = "0,";
                    }

                    Neutrofilos2 = Convert.ToDouble(textBox7.Text);
                    //Neutrofilos2
                    if (Neutrofilos2 < 2)
                    {
                        textBox7.ForeColor = Color.FromArgb(0, 110, 242);

                    }
                    else if (Neutrofilos2 > 7)
                    {
                        textBox7.ForeColor = Color.FromArgb(150, 40, 130);
                    }
                    else textBox7.ForeColor = Color.FromArgb(0, 0, 0);
                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox8.Text != " " && textBox8.Text != "")
            {
                if (textBox8.Text == ",")
                {
                    textBox8.Text = "0,";
                }

                linfocitos2 = Convert.ToDouble(textBox8.Text);
                //linfocitos2
                if (linfocitos2 < 1.1)
                {
                    textBox8.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos2 > 2.9)
                {
                    textBox8.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox8.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (textBox6.Text != " " && textBox6.Text != "")
            {
                if (textBox6.Text == ",")
                {
                    textBox6.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(textBox6.Text);
                //Monocitos2
                if (Monocitos2 < 0.12)
                {
                    textBox6.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 1.2)
                {
                    textBox6.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox6.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text != " " && textBox5.Text != "")
            {
                if (textBox5.Text == ",")
                {
                    textBox5.Text = "0,";
                }
                Eosinofilos2 = Convert.ToDouble(textBox5.Text);
                //Eosinofilos2
                if (Eosinofilos2 < 0.02)
                {
                    textBox5.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos2 > 0.50)
                {
                    textBox5.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox5.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (textBox9.Text != " " && textBox9.Text != "")
            {
                if (textBox9.Text == ",")
                {
                    textBox9.Text = "0,";
                }
                Basofilos2 = Convert.ToDouble(textBox9.Text);
                //Basofilos2
                if (Basofilos2 < 0.0)
                {
                    textBox9.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos2 > 0.1)
                {
                    textBox9.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox9.ForeColor = Color.FromArgb(0, 0, 0);
            }
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

        private void textBox1_MouseClick(object sender, MouseEventArgs e)
        {

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

        private void textBox1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void textBox2_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void textBox4_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void textBox3_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {

        }

        private void Form28_Load_1(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            ds = Conexion.HematologiaEspecial(IdOrden, IdAnalisis);
            ds1 = Conexion.TeclasYPrivilegios(IdUser);
            if (ds1.Tables[0].Rows.Count != 0)
            {

                TeclaLeu = ds1.Tables[0].Rows[0]["Leucocitos"].ToString().ToUpper();
                TeclaNeu = ds1.Tables[0].Rows[0]["Neutrofilos"].ToString().ToUpper();
                TeclaLin = ds1.Tables[0].Rows[0]["Linfocitos"].ToString().ToUpper();
                TeclaMono = ds1.Tables[0].Rows[0]["Monocitos"].ToString().ToUpper();
                TeclaEos = ds1.Tables[0].Rows[0]["Eosinofilos"].ToString().ToUpper();
                TeclaBaso = ds1.Tables[0].Rows[0]["Basofilos"].ToString().ToUpper();
                TeclaPla = ds1.Tables[0].Rows[0]["Plaquetas"].ToString().ToUpper();
                LabelLeu.Text = "T: " + ds1.Tables[0].Rows[0]["Leucocitos"].ToString().ToUpper();
                LabelNeu.Text = "T: " + ds1.Tables[0].Rows[0]["Neutrofilos"].ToString().ToUpper();
                LabelLin.Text = "T: " + ds1.Tables[0].Rows[0]["Linfocitos"].ToString().ToUpper();
                LabelMono.Text = "T: " + ds1.Tables[0].Rows[0]["Monocitos"].ToString().ToUpper();
                LabelEos.Text = "T: " + ds1.Tables[0].Rows[0]["Eosinofilos"].ToString().ToUpper();
                LabelBaso.Text = "T: " + ds1.Tables[0].Rows[0]["Basofilos"].ToString().ToUpper();
                LabelPla.Text = "T: " + ds1.Tables[0].Rows[0]["Plaquetas"].ToString().ToUpper();

            }
            else
            {
                checkBox1.Enabled = false;
            }

            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                textBox14.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                textBox1.Text = ds.Tables[0].Rows[0]["Neutrofilos"].ToString();
                textBox2.Text = ds.Tables[0].Rows[0]["Linfocitos"].ToString();
                textBox4.Text = ds.Tables[0].Rows[0]["Monocitos"].ToString();
                textBox3.Text = ds.Tables[0].Rows[0]["Eosinofilos"].ToString();
                textBox10.Text = ds.Tables[0].Rows[0]["Basofilos"].ToString();
                textBox20.Text = ds.Tables[0].Rows[0]["hematies"].ToString();
                textBox19.Text = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                textBox18.Text = ds.Tables[0].Rows[0]["Hematocritos"].ToString();
                textBox17.Text = ds.Tables[0].Rows[0]["VCM"].ToString();
                textBox12.Text = ds.Tables[0].Rows[0]["HCM"].ToString();
                textBox11.Text = ds.Tables[0].Rows[0]["CHCM"].ToString();
                textBox16.Text = ds.Tables[0].Rows[0]["plaquetas"].ToString();
                textBox7.Text = ds.Tables[0].Rows[0]["Neutrofilos2"].ToString();
                textBox8.Text = ds.Tables[0].Rows[0]["Linfocitos2"].ToString();
                textBox6.Text = ds.Tables[0].Rows[0]["Monocitos2"].ToString();
                textBox5.Text = ds.Tables[0].Rows[0]["Eosinofilos2"].ToString();
                textBox9.Text = ds.Tables[0].Rows[0]["Basofilos2"].ToString();
                textBox15.Text = ds.Tables[0].Rows[0]["Frotis"].ToString();
            }
            catch
            {

            }
        }

        private void checkBox1_CheckedChanged_1(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                string mensaje = "Se borraran los valores de los Neutrofilos,Eosinofilos,Monocitos,Basofilos y Linfocitos";
                string titulo = "Alarma";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    Reportar = true;
                    Total = 0;
                    Neu = 0;
                    Lin = 0;
                    Mono = 0;
                    Eos = 0;
                    Baso = 0;
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox4.Clear();
                    textBox3.Clear();
                    textBox10.Clear();
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox6.Clear();
                    textBox5.Clear();
                    textBox9.Clear();
                }
                else
                {
                    checkBox1.Checked = false;
                }
            }
            else
            {
                Total = 0;
                Neu = 0;
                Lin = 0;
                Mono = 0;
                Eos = 0;
                Baso = 0;
                Reportar = false;
            }
        }

        private void iconButton1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox18_TextChanged_1(object sender, EventArgs e)
        {

            if (textBox18.Text != " " && textBox18.Text != "")
            {
                if (textBox18.Text == ",")
                {
                    textBox18.Text = "0,";
                }
                Hematocritos = Convert.ToDouble(textBox18.Text);
                //Hematocritos
                if (Hematocritos < 35)
                {
                    textBox18.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematocritos > 54)
                {
                    textBox18.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox18.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox18.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox18.Text) * 0.32);
                textBox19.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox18.Text) * 0.11);
                textBox20.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox18.Text) * 10) / Convert.ToDouble(textBox20.Text);
                textBox17.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox19.Text) * 100) / Convert.ToDouble(textBox18.Text);
                textBox11.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(textBox19.Text) * 10) / Convert.ToDouble(textBox20.Text);
                textBox12.Text = Resultado.ToString("0.##");

            }
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox1.Text != " " && textBox1.Text != "")
            {
                if (textBox1.Text == ",")
                {
                    textBox1.Text = "0,";
                }
                //NEUTROFILOS
                Neutrofilos = Convert.ToDouble(textBox1.Text);
                if (Neutrofilos < 40.0)
                {
                    textBox1.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Neutrofilos > 70.0)
                {
                    textBox1.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox1.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox1.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox1.Text)) / 100;
                textBox7.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            //NEUTROFILOS
            if (textBox2.Text != " " && textBox2.Text != "")
            {
                if (textBox2.Text == ",")
                {
                    textBox2.Text = "0,";
                }
                //NEUTROFILOS
                linfocitos = Convert.ToDouble(textBox2.Text);
                if (linfocitos < 18.0)
                {
                    textBox2.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos > 48.0)
                {
                    textBox2.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox2.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox2.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox2.Text)) / 100;
                textBox8.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {
            //Monocitos
            if (textBox4.Text != " " && textBox4.Text != "")
            {
                if (textBox4.Text == ",")
                {
                    textBox4.Text = "0,";
                }
                Monocitos = Convert.ToDouble(textBox4.Text);
                //Monocitos
                if (Monocitos < 3.0)
                {
                    textBox4.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos > 12.0)
                {
                    textBox4.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox4.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox4.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox4.Text)) / 100;
                textBox6.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
            //Eosinofilos 
            if (textBox3.Text != " " && textBox3.Text != "")
            {
                if (textBox3.Text == ",")
                {
                    textBox3.Text = "0,";
                }
                Eosinofilos = Convert.ToDouble(textBox3.Text);

                //Eosinofilos
                if (Eosinofilos < 0.6)
                {
                    textBox3.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos > 7.3)
                {
                    textBox3.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox3.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox3.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox3.Text)) / 100;
                textBox5.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox10_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox10.Text != " " && textBox10.Text != "")
            {
                if (textBox10.Text == ",")
                {
                    textBox10.Text = "0,";
                }
                Basofilos = Convert.ToDouble(textBox10.Text);
                //Basofilos
                if (Basofilos < 0)
                {
                    textBox10.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos > 1.7)
                {
                    textBox10.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox10.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (textBox14.Text != "" && textBox10.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox10.Text)) / 100;
                textBox9.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox14_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox14.Text != " " && textBox14.Text != "")
            {
                if (textBox14.Text == ",")
                {
                    textBox14.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(textBox14.Text);
                //Monocitos2
                if (Monocitos2 < 4.0)
                {
                    textBox14.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 10.0)
                {
                    textBox14.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox14.ForeColor = Color.FromArgb(0, 0, 0);
                Double Resultado;
                if (textBox10.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox10.Text)) / 100;
                    textBox9.Text = Resultado.ToString("0.##");
                }
                if (textBox3.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox3.Text)) / 100;
                    textBox5.Text = Resultado.ToString("0.##");
                }
                if (textBox4.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox4.Text)) / 100;
                    textBox6.Text = Resultado.ToString("0.##");
                }
                if (textBox2.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox2.Text)) / 100;
                    textBox8.Text = Resultado.ToString("0.##");
                }
                if (textBox1.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox1.Text)) / 100;
                    textBox7.Text = Resultado.ToString("0.##");
                }
            }

        }

        private void textBox7_TextChanged_1(object sender, EventArgs e)
        {

            if (textBox7.Text != " " && textBox7.Text != "")
            {
                if (textBox7.Text != " " && textBox7.Text != "")
                {
                    if (textBox7.Text == ",")
                    {
                        textBox7.Text = "0,";
                    }

                    Neutrofilos2 = Convert.ToDouble(textBox7.Text);
                    //Neutrofilos2
                    if (Neutrofilos2 < 2)
                    {
                        textBox7.ForeColor = Color.FromArgb(0, 110, 242);

                    }
                    else if (Neutrofilos2 > 7)
                    {
                        textBox7.ForeColor = Color.FromArgb(150, 40, 130);
                    }
                    else textBox7.ForeColor = Color.FromArgb(0, 0, 0);
                }
            }
        }

        private void textBox8_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox8.Text != " " && textBox8.Text != "")
            {
                if (textBox8.Text == ",")
                {
                    textBox8.Text = "0,";
                }

                linfocitos2 = Convert.ToDouble(textBox8.Text);
                //linfocitos2
                if (linfocitos2 < 1.1)
                {
                    textBox8.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos2 > 2.9)
                {
                    textBox8.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox8.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox6_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox6.Text != " " && textBox6.Text != "")
            {
                if (textBox6.Text == ",")
                {
                    textBox6.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(textBox6.Text);
                //Monocitos2
                if (Monocitos2 < 0.12)
                {
                    textBox6.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 1.2)
                {
                    textBox6.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox6.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox5.Text != " " && textBox5.Text != "")
            {
                if (textBox5.Text == ",")
                {
                    textBox5.Text = "0,";
                }
                Eosinofilos2 = Convert.ToDouble(textBox5.Text);
                //Eosinofilos2
                if (Eosinofilos2 < 0.02)
                {
                    textBox5.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos2 > 0.50)
                {
                    textBox5.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox5.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox9_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox9.Text != " " && textBox9.Text != "")
            {
                if (textBox9.Text == ",")
                {
                    textBox9.Text = "0,";
                }
                Basofilos2 = Convert.ToDouble(textBox9.Text);
                //Basofilos2
                if (Basofilos2 < 0.0)
                {
                    textBox9.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos2 > 0.1)
                {
                    textBox9.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox9.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox16_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox16.Text != " " && textBox16.Text != "")
            {
                if (textBox16.Text == ",")
                {
                    textBox16.Text = "0,";
                }

                Plaquetas = Convert.ToDouble(textBox16.Text);
                //Plaquetas
                if (Plaquetas < 150)
                {
                    textBox16.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Plaquetas > 450)
                {
                    textBox16.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox16.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox20_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox20.Text != " " && textBox20.Text != "")
            {
                if (textBox20.Text == ",")
                {
                    textBox20.Text = "0,";
                }

                Hematies = Convert.ToDouble(textBox20.Text);
                //Hematies
                if (Hematies < 3.5)
                {
                    textBox20.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematies > 5.5)
                {
                    textBox20.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox20.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox19_TextChanged_1(object sender, EventArgs e)
        {

            if (textBox19.Text != " " && textBox19.Text != "")
            {
                if (textBox19.Text == ",")
                {
                    textBox19.Text = "0,";
                }
                Hemoglobina = Convert.ToDouble(textBox19.Text);
                //Hemoglobina
                if (Hemoglobina < 11)
                {
                    textBox19.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hemoglobina > 16)
                {
                    textBox19.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox19.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox17_TextChanged_1(object sender, EventArgs e)
        {

            if (textBox17.Text != " " && textBox17.Text != "")
            {
                if (textBox17.Text == ",")
                {
                    textBox17.Text = "0,";
                }
                VCM = Convert.ToDouble(textBox17.Text);
                //VCM
                if (VCM < 80)
                {
                    textBox17.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (VCM > 100)
                {
                    textBox17.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox17.ForeColor = Color.FromArgb(0, 0, 0);

            }
        }

        private void textBox12_TextChanged_1(object sender, EventArgs e)
        {

            if (textBox12.Text != " " && textBox12.Text != "")
            {

                if (textBox12.Text == ",")
                {
                    textBox12.Text = "0,";
                }
                HCM = Convert.ToDouble(textBox12.Text);
                //HCM
                if (HCM < 27)
                {
                    textBox12.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (HCM > 34)
                {
                    textBox12.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox12.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox11_TextChanged_1(object sender, EventArgs e)
        {
            if (textBox11.Text != " " && textBox11.Text != "")
            {

                if (textBox11.Text == ",")
                {
                    textBox11.Text = "0,";
                }
                CHCM = Convert.ToDouble(textBox11.Text);
                //CHCM
                if (CHCM < 31)
                {
                    textBox11.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (CHCM > 34)
                {
                    textBox11.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox11.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox2_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void textBox4_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void textBox3_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void textBox10_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void textBox7_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void textBox16_KeyPress_1(object sender, KeyPressEventArgs e)
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {

            string cmd4 = "";
            string cmd3 = "EstadoDeResultado = 2";

            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();

            ds = Conexion.VerificarHematologiaEspecial(IdOrden);
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {

                    string MS;
                    MS = Conexion.InsertarHematologiaEspecialSinValidar(IdOrden.ToString(), DateTime.Now.ToString("hh:mm:ss"), IdAnalisis.ToString(), textBox15.Text, textBox1.Text, textBox2.Text, textBox4.Text, textBox3.Text, textBox10.Text, textBox20.Text, textBox19.Text, textBox18.Text, textBox17.Text, textBox12.Text, textBox11.Text, textBox16.Text, textBox7.Text, textBox8.Text, textBox6.Text, IdUser.ToString(), textBox5.Text, textBox9.Text, textBox14.Text);
                    MessageBox.Show(MS);
                }
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta operacion");
            }

            this.Close();
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

        private void textBox10_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox8_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox7_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox6_KeyPress(object sender, KeyPressEventArgs e)
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

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                string mensaje = "Se borraran los valores de los Neutrofilos,Eosinofilos,Monocitos,Basofilos y Linfocitos";
                string titulo = "Alarma";
                MessageBoxButtons button = MessageBoxButtons.YesNo;
                DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                if (dialog == DialogResult.Yes)
                {
                    Reportar = true;
                    Total = 0;
                    Neu = 0;
                    Lin = 0;
                    Mono = 0;
                    Eos = 0;
                    Baso = 0;
                    textBox1.Clear();
                    textBox2.Clear();
                    textBox4.Clear();
                    textBox3.Clear();
                    textBox10.Clear();
                    textBox7.Clear();
                    textBox8.Clear();
                    textBox6.Clear();
                    textBox5.Clear();
                    textBox9.Clear();
                }
                else
                {
                    checkBox1.Checked = false;
                }
            }
            else
            {
                Total = 0;
                Neu = 0;
                Lin = 0;
                Mono = 0;
                Eos = 0;
                Baso = 0;
                Reportar = false;
            }

        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox9_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox20_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox19_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox18_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox17_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox12_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox11_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox16_KeyPress(object sender, KeyPressEventArgs e)
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void textBox14_KeyPress(object sender, KeyPressEventArgs e)
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

        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            if (textBox14.Text != " " && textBox14.Text != "")
            {
                if (textBox14.Text == ",")
                {
                    textBox14.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(textBox14.Text);
                //Monocitos2
                if (Monocitos2 < 4.0)
                {
                    textBox14.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 10.0)
                {
                    textBox14.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else textBox14.ForeColor = Color.FromArgb(0, 0, 0);
                Double Resultado;
                if (textBox10.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox10.Text)) / 100;
                    textBox9.Text = Resultado.ToString("0.##");
                }
                if (textBox3.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox3.Text)) / 100;
                    textBox5.Text = Resultado.ToString("0.##");
                }
                if (textBox4.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox4.Text)) / 100;
                    textBox6.Text = Resultado.ToString("0.##");
                }
                if (textBox2.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox2.Text)) / 100;
                    textBox8.Text = Resultado.ToString("0.##");
                }
                if (textBox1.Text != "")
                {
                    Resultado = (Convert.ToDouble(textBox14.Text) * Convert.ToDouble(textBox1.Text)) / 100;
                    textBox7.Text = Resultado.ToString("0.##");
                }
            }

        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox4_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox10_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void textBox18_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void panel2_Enter(object sender, EventArgs e)
        {

        }
    }
}
