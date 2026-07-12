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
using Conexiones;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class HematologiaEspecialForm : Form
    {
        public Double Neutrofilos = 0, linfocitos = 0, Monocitos = 0, Eosinofilos = 0, Basofilos = 0, Neutrofilos2 = 0, linfocitos2 = 0, Monocitos2 = 0, Eosinofilos2 = 0, Basofilos2 = 0, Hematies = 0, Hemoglobina = 0, Hematocritos = 0, VCM = 0, HCM = 0, CHCM = 0, Plaquetas = 0;
        public string TeclaLeu, TeclaNeu, TeclaLin, TeclaMono, TeclaEos, TeclaBaso, TeclaPla;
        private int IdUser;
        private int IdOrden;
        private int IdAnalisis;
        private Double Total = 0, Neu = 0, Lin = 0, Mono = 0, Eos = 0, Baso = 0;
        bool Reportar = false;
        public HematologiaEspecialForm(int idUser, int idOrden, int idAnalisis)
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
                        tNeutrofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosinofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasofilosP.Text = Sumar.ToString("0.##");
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
                        tNeutrofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosinofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasofilosP.Text = Sumar.ToString("0.##");
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
                        tNeutrofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosinofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasofilosP.Text = Sumar.ToString("0.##");
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
                        tNeutrofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosinofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasofilosP.Text = Sumar.ToString("0.##");
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
                        tNeutrofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosinofilosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonocitosP.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasofilosP.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaLeu.ToUpper() && TeclaLeu.ToUpper() != "")
                {
                    if (tLeucocitos.Text != "")
                    {
                        Double Sumar;
                        Sumar = Convert.ToDouble(tLeucocitos.Text) + 1;
                        tLeucocitos.Text = Sumar.ToString();


                    }
                    else
                    {
                        Double Sumar;
                        Sumar = 1;
                        tLeucocitos.Text = Sumar.ToString();
                    }
                    return true;
                }
                if (keyData.ToString() == TeclaPla.ToUpper() && TeclaPla.ToUpper() != "")
                {

                    if (tPlaquetas.Text != "")
                    {
                        Double Sumar;
                        Sumar = Convert.ToDouble(tPlaquetas.Text) + 1;
                        tPlaquetas.Text = Sumar.ToString();

                    }
                    else
                    {
                        int Sumar;
                        Sumar = 1;
                        tPlaquetas.Text = Sumar.ToString();
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
            DatosDePaciente datosDePaciente = Conexion.selectDatosPacientePorId(IdOrden);
            HematologiaEspecial hematologiaEspecial = Conexion.HematologiaEspecial(IdOrden, IdAnalisis);
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
                Sexo.Text = datosDePaciente.Sexo;
                Nombre.Text = datosDePaciente.Nombre + " " + datosDePaciente.Apellidos;
                //NPaciente.Text = "# " + datosDePaciente.NumeroDia;
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(datosDePaciente.Fecha.ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = edad.ToString();
                tLeucocitos.Text = hematologiaEspecial.leucocitos;
                tNeutrofilosP.Text = hematologiaEspecial.Neutrofilos;
                tLinfocitosP.Text = hematologiaEspecial.linfocitos;
                tMonocitosP.Text = hematologiaEspecial.Monocitos;
                tEosinofilosP.Text = hematologiaEspecial.Eosinofilos;
                tBasofilosP.Text = hematologiaEspecial.Basofilos;
                tHematies.Text = hematologiaEspecial.Hematies;
                tHemoglobina.Text = hematologiaEspecial.Hemoglobina;
                tHematocritos.Text = hematologiaEspecial.Hematocritos;
                tVCM.Text = hematologiaEspecial.VCM;
                tHCM.Text = hematologiaEspecial.HCM;
                tCHCM.Text = hematologiaEspecial.CHCM;
                tPlaquetas.Text = hematologiaEspecial.Plaquetas;
                tNeutrofilosU.Text = hematologiaEspecial.Neutrofilos2;
                tLinfocitosU.Text = hematologiaEspecial.Linfocitos2;
                tMonocitosU.Text = hematologiaEspecial.Monocitos2;
                tEosinofilosU.Text = hematologiaEspecial.Eosinofilos2;
                tBasofilosU.Text = hematologiaEspecial.Basofilos2;
                tFrotis.Text = hematologiaEspecial.Comentario;
                tADE.Text = hematologiaEspecial.ADE;
                tADP.Text = hematologiaEspecial.ADP;
                tReticulocitos.Text = hematologiaEspecial.Reticulocitos;
                tVPM.Text = hematologiaEspecial.VPM;
                tPCT.Text = hematologiaEspecial.PCT;
            }
            catch
            {
                
            }

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (tNeutrofilosP.Text != " " && tNeutrofilosP.Text != "")
            {
                if (tNeutrofilosP.Text == ",")
                {
                    tNeutrofilosP.Text = "0,";
                }
                //NEUTROFILOS
                Neutrofilos = Convert.ToDouble(tNeutrofilosP.Text);
                if (Neutrofilos < 40.0)
                {
                    tNeutrofilosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Neutrofilos > 70.0)
                {
                    tNeutrofilosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tNeutrofilosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tNeutrofilosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tNeutrofilosP.Text)) / 100;
                tNeutrofilosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //NEUTROFILOS
            if (tLinfocitosP.Text != " " && tLinfocitosP.Text != "")
            {
                if (tLinfocitosP.Text == ",")
                {
                    tLinfocitosP.Text = "0,";
                }
                //NEUTROFILOS
                linfocitos = Convert.ToDouble(tLinfocitosP.Text);
                if (linfocitos < 18.0)
                {
                    tLinfocitosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos > 48.0)
                {
                    tLinfocitosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tLinfocitosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tLinfocitosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tLinfocitosP.Text)) / 100;
                tLinfocitosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            //Monocitos
            if (tMonocitosP.Text != " " && tMonocitosP.Text != "")
            {
                if (tMonocitosP.Text == ",")
                {
                    tMonocitosP.Text = "0,";
                }
                Monocitos = Convert.ToDouble(tMonocitosP.Text);
                //Monocitos
                if (Monocitos < 3.0)
                {
                    tMonocitosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos > 12.0)
                {
                    tMonocitosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tMonocitosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tMonocitosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tMonocitosP.Text)) / 100;
                tMonocitosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //Eosinofilos 
            if (tEosinofilosP.Text != " " && tEosinofilosP.Text != "")
            {
                if (tEosinofilosP.Text == ",")
                {
                    tEosinofilosP.Text = "0,";
                }
                Eosinofilos = Convert.ToDouble(tEosinofilosP.Text);

                //Eosinofilos
                if (Eosinofilos < 0.6)
                {
                    tEosinofilosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos > 7.3)
                {
                    tEosinofilosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tEosinofilosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tEosinofilosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tEosinofilosP.Text)) / 100;
                tEosinofilosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (tBasofilosP.Text != " " && tBasofilosP.Text != "")
            {
                if (tBasofilosP.Text == ",")
                {
                    tBasofilosP.Text = "0,";
                }
                Basofilos = Convert.ToDouble(tBasofilosP.Text);
                //Basofilos
                if (Basofilos < 0)
                {
                    tBasofilosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos > 1.7)
                {
                    tBasofilosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tBasofilosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tBasofilosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tBasofilosP.Text)) / 100;
                tBasofilosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox20_TextChanged(object sender, EventArgs e)
        {

            if (tHematies.Text != " " && tHematies.Text != "")
            {
                if (tHematies.Text == ",")
                {
                    tHematies.Text = "0,";
                }

                Hematies = Convert.ToDouble(tHematies.Text);
                //Hematies
                if (Hematies < 3.5)
                {
                    tHematies.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematies > 5.5)
                {
                    tHematies.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHematies.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }
        private void textBox19_TextChanged(object sender, EventArgs e)
        {

            if (tHemoglobina.Text != " " && tHemoglobina.Text != "")
            {
                if (tHemoglobina.Text == ",")
                {
                    tHemoglobina.Text = "0,";
                }
                Hemoglobina = Convert.ToDouble(tHemoglobina.Text);
                //Hemoglobina
                if (Hemoglobina < 11)
                {
                    tHemoglobina.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hemoglobina > 16)
                {
                    tHemoglobina.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHemoglobina.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }
        private void textBox18_TextChanged(object sender, EventArgs e)
        {

            if (tHematocritos.Text != " " && tHematocritos.Text != "")
            {
                if (tHematocritos.Text == ",")
                {
                    tHematocritos.Text = "0,";
                }
                Hematocritos = Convert.ToDouble(tHematocritos.Text);
                //Hematocritos
                if (Hematocritos < 35)
                {
                    tHematocritos.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematocritos > 54)
                {
                    tHematocritos.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHematocritos.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tHematocritos.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tHematocritos.Text) * 0.32);
                tHemoglobina.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(tHematocritos.Text) * 0.11);
                tHematies.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(tHematocritos.Text) * 10) / Convert.ToDouble(tHematies.Text);
                tVCM.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(tHemoglobina.Text) * 100) / Convert.ToDouble(tHematocritos.Text);
                tCHCM.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(tHemoglobina.Text) * 10) / Convert.ToDouble(tHematies.Text);
                tHCM.Text = Resultado.ToString("0.##");

            }
        }
        private void textBox17_TextChanged(object sender, EventArgs e)
        {

            if (tVCM.Text != " " && tVCM.Text != "")
            {
                if (tVCM.Text == ",")
                {
                    tVCM.Text = "0,";
                }
                VCM = Convert.ToDouble(tVCM.Text);
                //VCM
                if (VCM < 80)
                {
                    tVCM.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (VCM > 100)
                {
                    tVCM.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tVCM.ForeColor = Color.FromArgb(0, 0, 0);

            }
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {

            if (tHCM.Text != " " && tHCM.Text != "")
            {

                if (tHCM.Text == ",")
                {
                    tHCM.Text = "0,";
                }
                HCM = Convert.ToDouble(tHCM.Text);
                //HCM
                if (HCM < 27)
                {
                    tHCM.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (HCM > 34)
                {
                    tHCM.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHCM.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            if (tCHCM.Text != " " && tCHCM.Text != "")
            {

                if (tCHCM.Text == ",")
                {
                    tCHCM.Text = "0,";
                }
                CHCM = Convert.ToDouble(tCHCM.Text);
                //CHCM
                if (CHCM < 31)
                {
                    tCHCM.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (CHCM > 34)
                {
                    tCHCM.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tCHCM.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            if (tPlaquetas.Text != " " && tPlaquetas.Text != "")
            {
                if (tPlaquetas.Text == ",")
                {
                    tPlaquetas.Text = "0,";
                }

                Plaquetas = Convert.ToDouble(tPlaquetas.Text);
                //Plaquetas
                if (Plaquetas < 150)
                {
                    tPlaquetas.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Plaquetas > 450)
                {
                    tPlaquetas.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tPlaquetas.ForeColor = Color.FromArgb(0, 0, 0);
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
                        var he = AsignarValores();
                         MS = Conexion.InsertarHematologiaEspecial(IdOrden, IdAnalisis, he, IdUser);
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

            if (tNeutrofilosU.Text != " " && tNeutrofilosU.Text != "")
            {
                if (tNeutrofilosU.Text != " " && tNeutrofilosU.Text != "")
                {
                    if (tNeutrofilosU.Text == ",")
                    {
                        tNeutrofilosU.Text = "0,";
                    }

                    Neutrofilos2 = Convert.ToDouble(tNeutrofilosU.Text);
                    //Neutrofilos2
                    if (Neutrofilos2 < 2)
                    {
                        tNeutrofilosU.ForeColor = Color.FromArgb(0, 110, 242);

                    }
                    else if (Neutrofilos2 > 7)
                    {
                        tNeutrofilosU.ForeColor = Color.FromArgb(150, 40, 130);
                    }
                    else tNeutrofilosU.ForeColor = Color.FromArgb(0, 0, 0);
                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (tLinfocitosU.Text != " " && tLinfocitosU.Text != "")
            {
                if (tLinfocitosU.Text == ",")
                {
                    tLinfocitosU.Text = "0,";
                }

                linfocitos2 = Convert.ToDouble(tLinfocitosU.Text);
                //linfocitos2
                if (linfocitos2 < 1.1)
                {
                    tLinfocitosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos2 > 2.9)
                {
                    tLinfocitosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tLinfocitosU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (tMonocitosU.Text != " " && tMonocitosU.Text != "")
            {
                if (tMonocitosU.Text == ",")
                {
                    tMonocitosU.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(tMonocitosU.Text);
                //Monocitos2
                if (Monocitos2 < 0.12)
                {
                    tMonocitosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 1.2)
                {
                    tMonocitosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tMonocitosU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (tEosinofilosU.Text != " " && tEosinofilosU.Text != "")
            {
                if (tEosinofilosU.Text == ",")
                {
                    tEosinofilosU.Text = "0,";
                }
                Eosinofilos2 = Convert.ToDouble(tEosinofilosU.Text);
                //Eosinofilos2
                if (Eosinofilos2 < 0.02)
                {
                    tEosinofilosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos2 > 0.50)
                {
                    tEosinofilosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tEosinofilosU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (tBasofilosU.Text != " " && tBasofilosU.Text != "")
            {
                if (tBasofilosU.Text == ",")
                {
                    tBasofilosU.Text = "0,";
                }
                Basofilos2 = Convert.ToDouble(tBasofilosU.Text);
                //Basofilos2
                if (Basofilos2 < 0.0)
                {
                    tBasofilosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos2 > 0.1)
                {
                    tBasofilosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tBasofilosU.ForeColor = Color.FromArgb(0, 0, 0);
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
            Ordenes orden = new Ordenes();
            orden = Conexion.SELECTordenesPorIdOrden(IdOrden);
            DatosDePaciente datosDePaciente = Conexion.selectDatosPacientePorIdOrden(IdOrden);
            HematologiaEspecial hematologiaEspecial = Conexion.HematologiaEspecial(IdOrden, IdAnalisis);
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
                Sexo.Text = datosDePaciente.Sexo;
                Nombre.Text = datosDePaciente.Nombre + " " + datosDePaciente.Apellidos;
                //NPaciente.Text = "# " + datosDePaciente.NumeroDia;
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = datosDePaciente.Fecha;
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                tLeucocitos.Text = hematologiaEspecial.leucocitos.ToString();
                tNeutrofilosP.Text = hematologiaEspecial.Neutrofilos.ToString();
                tLinfocitosP.Text = hematologiaEspecial.linfocitos.ToString();
                tMonocitosP.Text = hematologiaEspecial.Monocitos.ToString();
                tEosinofilosP.Text = hematologiaEspecial.Eosinofilos.ToString();
                tBasofilosP.Text = hematologiaEspecial.Basofilos.ToString();
                tHematies.Text = hematologiaEspecial.Hematies.ToString();
                tHemoglobina.Text = hematologiaEspecial.Hemoglobina.ToString();
                tHematocritos.Text = hematologiaEspecial.Hematocritos.ToString();
                tVCM.Text = hematologiaEspecial.VCM.ToString();
                tHCM.Text = hematologiaEspecial.HCM.ToString();
                tCHCM.Text = hematologiaEspecial.CHCM.ToString();
                tPlaquetas.Text = hematologiaEspecial.Plaquetas.ToString();
                tNeutrofilosU.Text = hematologiaEspecial.Neutrofilos2.ToString();
                tLinfocitosU.Text = hematologiaEspecial.Linfocitos2.ToString();
                tMonocitosU.Text = hematologiaEspecial.Monocitos2.ToString();
                tEosinofilosU.Text = hematologiaEspecial.Eosinofilos2.ToString();
                tBasofilosU.Text = hematologiaEspecial.Basofilos2.ToString();
                tFrotis.Text = hematologiaEspecial.Comentario.ToString();
                tADE.Text = hematologiaEspecial.ADE.ToString();
                tADP.Text = hematologiaEspecial.ADP.ToString();
                tReticulocitos.Text = hematologiaEspecial.Reticulocitos.ToString();
               tVPM.Text = hematologiaEspecial.VPM.ToString();
                tPCT.Text = hematologiaEspecial.PCT.ToString();

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
                    tNeutrofilosP.Clear();
                    tLinfocitosP.Clear();
                    tMonocitosP.Clear();
                    tEosinofilosP.Clear();
                    tBasofilosP.Clear();
                    tNeutrofilosU.Clear();
                    tLinfocitosU.Clear();
                    tMonocitosU.Clear();
                    tEosinofilosU.Clear();
                    tBasofilosU.Clear();
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

            if (tHematocritos.Text != " " && tHematocritos.Text != "")
            {
                if (tHematocritos.Text == ",")
                {
                    tHematocritos.Text = "0,";
                }
                Hematocritos = Convert.ToDouble(tHematocritos.Text);
                //Hematocritos
                if (Hematocritos < 35)
                {
                    tHematocritos.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematocritos > 54)
                {
                    tHematocritos.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHematocritos.ForeColor = Color.FromArgb(0, 0, 0);
            }
           
        }

        private void textBox1_TextChanged_1(object sender, EventArgs e)
        {
            if (tNeutrofilosP.Text != " " && tNeutrofilosP.Text != "")
            {
                if (tNeutrofilosP.Text == ",")
                {
                    tNeutrofilosP.Text = "0,";
                }
                //NEUTROFILOS
                Neutrofilos = Convert.ToDouble(tNeutrofilosP.Text);
                if (Neutrofilos < 40.0)
                {
                    tNeutrofilosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Neutrofilos > 70.0)
                {
                    tNeutrofilosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tNeutrofilosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tNeutrofilosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tNeutrofilosP.Text)) / 100;
                tNeutrofilosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox2_TextChanged_1(object sender, EventArgs e)
        {
            //NEUTROFILOS
            if (tLinfocitosP.Text != " " && tLinfocitosP.Text != "")
            {
                if (tLinfocitosP.Text == ",")
                {
                    tLinfocitosP.Text = "0,";
                }
                //NEUTROFILOS
                linfocitos = Convert.ToDouble(tLinfocitosP.Text);
                if (linfocitos < 18.0)
                {
                    tLinfocitosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos > 48.0)
                {
                    tLinfocitosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tLinfocitosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tLinfocitosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tLinfocitosP.Text)) / 100;
                tLinfocitosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox4_TextChanged_1(object sender, EventArgs e)
        {
            //Monocitos
            if (tMonocitosP.Text != " " && tMonocitosP.Text != "")
            {
                if (tMonocitosP.Text == ",")
                {
                    tMonocitosP.Text = "0,";
                }
                Monocitos = Convert.ToDouble(tMonocitosP.Text);
                //Monocitos
                if (Monocitos < 3.0)
                {
                    tMonocitosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos > 12.0)
                {
                    tMonocitosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tMonocitosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tMonocitosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tMonocitosP.Text)) / 100;
                tMonocitosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox3_TextChanged_1(object sender, EventArgs e)
        {
            //Eosinofilos 
            if (tEosinofilosP.Text != " " && tEosinofilosP.Text != "")
            {
                if (tEosinofilosP.Text == ",")
                {
                    tEosinofilosP.Text = "0,";
                }
                Eosinofilos = Convert.ToDouble(tEosinofilosP.Text);

                //Eosinofilos
                if (Eosinofilos < 0.6)
                {
                    tEosinofilosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos > 7.3)
                {
                    tEosinofilosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tEosinofilosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tEosinofilosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tEosinofilosP.Text)) / 100;
                tEosinofilosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox10_TextChanged_1(object sender, EventArgs e)
        {
            if (tBasofilosP.Text != " " && tBasofilosP.Text != "")
            {
                if (tBasofilosP.Text == ",")
                {
                    tBasofilosP.Text = "0,";
                }
                Basofilos = Convert.ToDouble(tBasofilosP.Text);
                //Basofilos
                if (Basofilos < 0)
                {
                    tBasofilosP.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos > 1.7)
                {
                    tBasofilosP.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tBasofilosP.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tBasofilosP.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tBasofilosP.Text)) / 100;
                tBasofilosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox14_TextChanged_1(object sender, EventArgs e)
        {
            if (tLeucocitos.Text != " " && tLeucocitos.Text != "")
            {
                if (tLeucocitos.Text == ",")
                {
                    tLeucocitos.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(tLeucocitos.Text);
                //Monocitos2
                if (Monocitos2 < 4.0)
                {
                    tLeucocitos.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 10.0)
                {
                    tLeucocitos.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tLeucocitos.ForeColor = Color.FromArgb(0, 0, 0);
                Double Resultado;
                if (tBasofilosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tBasofilosP.Text)) / 100;
                    tBasofilosU.Text = Resultado.ToString("0.##");
                }
                if (tEosinofilosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tEosinofilosP.Text)) / 100;
                    tEosinofilosU.Text = Resultado.ToString("0.##");
                }
                if (tMonocitosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tMonocitosP.Text)) / 100;
                    tMonocitosU.Text = Resultado.ToString("0.##");
                }
                if (tLinfocitosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tLinfocitosP.Text)) / 100;
                    tLinfocitosU.Text = Resultado.ToString("0.##");
                }
                if (tNeutrofilosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tNeutrofilosP.Text)) / 100;
                    tNeutrofilosU.Text = Resultado.ToString("0.##");
                }
            }

        }

        private void textBox7_TextChanged_1(object sender, EventArgs e)
        {

            if (tNeutrofilosU.Text != " " && tNeutrofilosU.Text != "")
            {
                if (tNeutrofilosU.Text != " " && tNeutrofilosU.Text != "")
                {
                    if (tNeutrofilosU.Text == ",")
                    {
                        tNeutrofilosU.Text = "0,";
                    }

                    Neutrofilos2 = Convert.ToDouble(tNeutrofilosU.Text);
                    //Neutrofilos2
                    if (Neutrofilos2 < 2)
                    {
                        tNeutrofilosU.ForeColor = Color.FromArgb(0, 110, 242);

                    }
                    else if (Neutrofilos2 > 7)
                    {
                        tNeutrofilosU.ForeColor = Color.FromArgb(150, 40, 130);
                    }
                    else tNeutrofilosU.ForeColor = Color.FromArgb(0, 0, 0);
                }
            }
        }

        private void textBox8_TextChanged_1(object sender, EventArgs e)
        {
            if (tLinfocitosU.Text != " " && tLinfocitosU.Text != "")
            {
                if (tLinfocitosU.Text == ",")
                {
                    tLinfocitosU.Text = "0,";
                }

                linfocitos2 = Convert.ToDouble(tLinfocitosU.Text);
                //linfocitos2
                if (linfocitos2 < 1.1)
                {
                    tLinfocitosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (linfocitos2 > 2.9)
                {
                    tLinfocitosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tLinfocitosU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox6_TextChanged_1(object sender, EventArgs e)
        {
            if (tMonocitosU.Text != " " && tMonocitosU.Text != "")
            {
                if (tMonocitosU.Text == ",")
                {
                    tMonocitosU.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(tMonocitosU.Text);
                //Monocitos2
                if (Monocitos2 < 0.12)
                {
                    tMonocitosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 1.2)
                {
                    tMonocitosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tMonocitosU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox5_TextChanged_1(object sender, EventArgs e)
        {
            if (tEosinofilosU.Text != " " && tEosinofilosU.Text != "")
            {
                if (tEosinofilosU.Text == ",")
                {
                    tEosinofilosU.Text = "0,";
                }
                Eosinofilos2 = Convert.ToDouble(tEosinofilosU.Text);
                //Eosinofilos2
                if (Eosinofilos2 < 0.02)
                {
                    tEosinofilosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Eosinofilos2 > 0.50)
                {
                    tEosinofilosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tEosinofilosU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox9_TextChanged_1(object sender, EventArgs e)
        {
            if (tBasofilosU.Text != " " && tBasofilosU.Text != "")
            {
                if (tBasofilosU.Text == ",")
                {
                    tBasofilosU.Text = "0,";
                }
                Basofilos2 = Convert.ToDouble(tBasofilosU.Text);
                //Basofilos2
                if (Basofilos2 < 0.0)
                {
                    tBasofilosU.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Basofilos2 > 0.1)
                {
                    tBasofilosU.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tBasofilosU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox16_TextChanged_1(object sender, EventArgs e)
        {
            if (tPlaquetas.Text != " " && tPlaquetas.Text != "")
            {
                if (tPlaquetas.Text == ",")
                {
                    tPlaquetas.Text = "0,";
                }

                Plaquetas = Convert.ToDouble(tPlaquetas.Text);
                //Plaquetas
                if (Plaquetas < 150)
                {
                    tPlaquetas.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Plaquetas > 450)
                {
                    tPlaquetas.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tPlaquetas.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox20_TextChanged_1(object sender, EventArgs e)
        {
            if (tHematies.Text != " " && tHematies.Text != "")
            {
                if (tHematies.Text == ",")
                {
                    tHematies.Text = "0,";
                }

                Hematies = Convert.ToDouble(tHematies.Text);
                //Hematies
                if (Hematies < 3.5)
                {
                    tHematies.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hematies > 5.5)
                {
                    tHematies.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHematies.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox19_TextChanged_1(object sender, EventArgs e)
        {

            if (tHemoglobina.Text != " " && tHemoglobina.Text != "")
            {
                if (tHemoglobina.Text == ",")
                {
                    tHemoglobina.Text = "0,";
                }
                Hemoglobina = Convert.ToDouble(tHemoglobina.Text);
                //Hemoglobina
                if (Hemoglobina < 11)
                {
                    tHemoglobina.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Hemoglobina > 16)
                {
                    tHemoglobina.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHemoglobina.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox17_TextChanged_1(object sender, EventArgs e)
        {

            if (tVCM.Text != " " && tVCM.Text != "")
            {
                if (tVCM.Text == ",")
                {
                    tVCM.Text = "0,";
                }
                VCM = Convert.ToDouble(tVCM.Text);
                //VCM
                if (VCM < 80)
                {
                    tVCM.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (VCM > 100)
                {
                    tVCM.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tVCM.ForeColor = Color.FromArgb(0, 0, 0);

            }
        }

        private void textBox12_TextChanged_1(object sender, EventArgs e)
        {

            if (tHCM.Text != " " && tHCM.Text != "")
            {

                if (tHCM.Text == ",")
                {
                    tHCM.Text = "0,";
                }
                HCM = Convert.ToDouble(tHCM.Text);
                //HCM
                if (HCM < 27)
                {
                    tHCM.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (HCM > 34)
                {
                    tHCM.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tHCM.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox11_TextChanged_1(object sender, EventArgs e)
        {
            if (tCHCM.Text != " " && tCHCM.Text != "")
            {

                if (tCHCM.Text == ",")
                {
                    tCHCM.Text = "0,";
                }
                CHCM = Convert.ToDouble(tCHCM.Text);
                //CHCM
                if (CHCM < 31)
                {
                    tCHCM.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (CHCM > 34)
                {
                    tCHCM.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tCHCM.ForeColor = Color.FromArgb(0, 0, 0);
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
        private bool VerificarValores()
        {
            Double Leuco = Convert.ToDouble(tLeucocitos.Text);
            Double Linfo = Convert.ToDouble(tNeutrofilosP.Text);
            Double Neutro = Convert.ToDouble(tLinfocitosP.Text);
            Double Hemoglobina = Convert.ToDouble(tHematocritos.Text);
            Double Hematocrito = Convert.ToDouble(tHematies.Text);
            Double plaquetas = Convert.ToDouble(tPlaquetas.Text); ;
            if (Leuco < 1)
            {
                MessageBox.Show("Leucocitos = " + Leuco + " Por Favor coloque otro valor");
            }
            else if (Linfo < 1)
            {
                MessageBox.Show("Neutrofilos = " + Neutro + " Por Favor coloque otro valor");
            }
            else if (Neutro < 1)
            {
                MessageBox.Show("Linfocitos = " + Linfo + " Por Favor coloque otro valor");
            }
            else if (Hematocrito < 1)
            {
                MessageBox.Show("Hematocrito = " + Hematocrito + " Por Favor coloque otro valor");
            }
            else if (Hemoglobina < 1)
            {
                MessageBox.Show("Hemoglobina = " + Hemoglobina + " Por Favor coloque otro valor");
            }
            else if (plaquetas < 1)
            {
                MessageBox.Show("plaquetas = " + plaquetas + " Por Favor coloque otro valor");
            }
            return true;
        }
        private HematologiaEspecial AsignarValores()
        {
            if (VerificarValores() == false)
            {
                return null;
            }
            var he = new Conexiones.HematologiaEspecial
            {
                //Globulos Blancos
                leucocitos = tLeucocitos.Text,

                //Globulos Blancos Porcentajes
                Neutrofilos = tNeutrofilosP.Text,
                linfocitos = tLinfocitosP.Text,
                Monocitos = tMonocitosP.Text,
                Eosinofilos = tEosinofilosP.Text,
                Basofilos = tBasofilosP.Text,

                //Globulos Blancos Unidades
                Neutrofilos2 = tNeutrofilosU.Text,
                Linfocitos2 = tLinfocitosU.Text,
                Monocitos2 = tMonocitosU.Text,
                Eosinofilos2 = tEosinofilosU.Text,
                Basofilos2 = tBasofilosU.Text,

                //Globulos Rojos 
                Hematies = tHematies.Text,
                Hemoglobina = tHemoglobina.Text,
                Hematocritos = tHematocritos.Text,
                VCM = tVCM.Text,
                HCM = tHCM.Text,
                CHCM = tCHCM.Text,
                Plaquetas = tPlaquetas.Text,
                ADE = tADE.Text,
                VPM = tVPM.Text,
                ADP = tADP.Text,
                PCT = tPCT.Text,
                Reticulocitos = tReticulocitos.Text,
                Comentario = tFrotis.Text
            };
            return he;
        }
        private void iconButton3_Click(object sender, EventArgs e)
        {

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
                    var he = AsignarValores();
                    MS = Conexion.InsertarHematologiaEspecialSinValidar(IdOrden, IdAnalisis, he, IdUser);
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
                    tNeutrofilosP.Clear();
                    tLinfocitosP.Clear();
                    tMonocitosP.Clear();
                    tEosinofilosP.Clear();
                    tBasofilosP.Clear();
                    tNeutrofilosU.Clear();
                    tLinfocitosU.Clear();
                    tMonocitosU.Clear();
                    tEosinofilosU.Clear();
                    tBasofilosU.Clear();
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
            if (tLeucocitos.Text != " " && tLeucocitos.Text != "")
            {
                if (tLeucocitos.Text == ",")
                {
                    tLeucocitos.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(tLeucocitos.Text);
                //Monocitos2
                if (Monocitos2 < 4.0)
                {
                    tLeucocitos.ForeColor = Color.FromArgb(0, 110, 242);

                }
                else if (Monocitos2 > 10.0)
                {
                    tLeucocitos.ForeColor = Color.FromArgb(150, 40, 130);
                }
                else tLeucocitos.ForeColor = Color.FromArgb(0, 0, 0);
                Double Resultado;
                if (tBasofilosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tBasofilosP.Text)) / 100;
                    tBasofilosU.Text = Resultado.ToString("0.##");
                }
                if (tEosinofilosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tEosinofilosP.Text)) / 100;
                    tEosinofilosU.Text = Resultado.ToString("0.##");
                }
                if (tMonocitosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tMonocitosP.Text)) / 100;
                    tMonocitosU.Text = Resultado.ToString("0.##");
                }
                if (tLinfocitosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tLinfocitosP.Text)) / 100;
                    tLinfocitosU.Text = Resultado.ToString("0.##");
                }
                if (tNeutrofilosP.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tNeutrofilosP.Text)) / 100;
                    tNeutrofilosU.Text = Resultado.ToString("0.##");
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
