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
    public partial class HematologiaForm : Form
    {
        List<string> Ordenes = new List<string>();
        Double Neutrofilos = 0, linfocitos = 0, Monocitos = 0, Eosinofilos = 0, Basofilos = 0, Neutrofilos2 = 0, linfocitos2 = 0, Monocitos2 = 0, Eosinofilos2 = 0, Basofilos2 = 0, Hematies = 0, Hemoglobina = 0, Hematocritos = 0, VCM = 0, HCM = 0, CHCM = 0, Plaquetas = 0;
        string TeclaLeu,TeclaNeu,TeclaLin,TeclaMono,TeclaEos,TeclaBaso,TeclaPla;
        int IdOrden, IdAnalisis, IdUser;
        private Double Total=0,Neu=0,Lin=0,Mono=0,Eos=0,Baso=0;
        int PoscionActual;
        bool Reportar = false;
        public HematologiaForm(int idUser,int idOrden, int idAnalisis)
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
                    textBox15.Text = Total.ToString();
                    Neu++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0) 
                    {
                    tNeutroPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0) 
                    { 
                        tLinfoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0) 
                    {
                        tEosPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0) 
                    {
                    tMonoPor.Text = Sumar.ToString("0.##");   
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0) 
                    { 
                    tBasoPor.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                      
               }
                if (keyData.ToString() == TeclaLin.ToUpper() && TeclaLin.ToUpper() != "")
                {
                    Double Sumar;
                        Total++;
                    textBox15.Text = Total.ToString();
                    Lin++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        tNeutroPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasoPor.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaMono.ToUpper() && TeclaMono.ToUpper() != "")
                {
                    Double Sumar;
                        Total++;
                    textBox15.Text = Total.ToString();
                    Mono++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        tNeutroPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasoPor.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaEos.ToUpper() && TeclaEos.ToUpper() != "")
                {

                    Double Sumar;
                        Total++;
                    textBox15.Text = Total.ToString();
                    Eos++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        tNeutroPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasoPor.Text = Sumar.ToString("0.##");
                    }
                    Conteo();
                    return true;
                }
                if (keyData.ToString() == TeclaBaso.ToUpper() && TeclaBaso.ToUpper() != "")
                {

                    Double Sumar;
                        Total++;
                    textBox15.Text = Total.ToString();
                        Baso++;
                    Sumar = (Neu / Total) * 100;
                    if (Sumar != 0)
                    {
                        tNeutroPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Lin / Total) * 100;
                    if (Sumar != 0)
                    {
                        tLinfoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Eos / Total) * 100;
                    if (Sumar != 0)
                    {
                        tEosPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Mono / Total) * 100;
                    if (Sumar != 0)
                    {
                        tMonoPor.Text = Sumar.ToString("0.##");
                    }
                    Sumar = (Baso / Total) * 100;
                    if (Sumar != 0)
                    {
                        tBasoPor.Text = Sumar.ToString("0.##");
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
                textBox15.Text = Total.ToString();
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

        private void Form4_Load(object sender, EventArgs e)
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
            DataSet ds1 = new DataSet();
            ds = Conexion.Hematologia(IdOrden);
            ds1 = Conexion.TeclasYPrivilegios(IdUser);
            if (ds1.Tables.Count != 0)
            { 
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
            }
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                NPaciente.Text ="# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                string edad = Conexion.Fecha(nacimiento);
                Edad.Text = edad;
                tComentario.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
                tLeucocitos.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                tNeutroPor.Text = ds.Tables[0].Rows[0]["Neutrofilos"].ToString();
                tLinfoPor.Text = ds.Tables[0].Rows[0]["Linfocitos"].ToString();
                tMonoPor.Text = ds.Tables[0].Rows[0]["Monocitos"].ToString();
                tEosPor.Text = ds.Tables[0].Rows[0]["Eosinofilos"].ToString();
                tBasoPor.Text = ds.Tables[0].Rows[0]["Basofilos"].ToString();
                tHematies.Text = ds.Tables[0].Rows[0]["hematies"].ToString();
                tHemoglobina.Text = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                tHematocrito.Text = ds.Tables[0].Rows[0]["Hematocritos"].ToString();
                tVCM.Text = ds.Tables[0].Rows[0]["VCM"].ToString();
                tHCM.Text = ds.Tables[0].Rows[0]["HCM"].ToString();
                tCHCM.Text = ds.Tables[0].Rows[0]["CHCM"].ToString();
                tPlaquetas.Text = ds.Tables[0].Rows[0]["plaquetas"].ToString();
                tNeutroU.Text = ds.Tables[0].Rows[0]["Neutrofilos2"].ToString();
                tLinfoU.Text = ds.Tables[0].Rows[0]["Linfocitos2"].ToString();
                tMonoU.Text = ds.Tables[0].Rows[0]["Monocitos2"].ToString();
                tEosU.Text = ds.Tables[0].Rows[0]["Eosinofilos2"].ToString();
                tBasoU.Text = ds.Tables[0].Rows[0]["Basofilos2"].ToString();
            }
            catch 
            {

            }

        }


        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (tNeutroPor.Text != " " && tNeutroPor.Text != "")
            {
                if (tNeutroPor.Text == ",")
                {
                    tNeutroPor.Text = "0,";
                }
                //NEUTROFILOS
                Neutrofilos = Convert.ToDouble(tNeutroPor.Text);
            if (Neutrofilos < 40.0)
            {
                tNeutroPor.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Neutrofilos > 70.0)
            {
                tNeutroPor.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tNeutroPor.ForeColor = Color.FromArgb(0, 0, 0);
           }
            if (tLeucocitos.Text != "" && tNeutroPor.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tNeutroPor.Text)) / 100;
                tNeutroU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            //NEUTROFILOS
            if (tLinfoPor.Text != " " && tLinfoPor.Text != "")
            {
                if (tLinfoPor.Text == ",")
                {
                    tLinfoPor.Text = "0,";
                }
                //NEUTROFILOS
                linfocitos = Convert.ToDouble(tLinfoPor.Text);
                if (linfocitos < 18.0)
            {
                tLinfoPor.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (linfocitos > 48.0)
            {
                tLinfoPor.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tLinfoPor.ForeColor = Color.FromArgb(0, 0, 0);
            }
            if (tLeucocitos.Text != "" && tLinfoPor.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tLinfoPor.Text)) / 100;
                tLinfoU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

            //Monocitos
            if (tMonoPor.Text != " " && tMonoPor.Text != "")
            {
                if (tMonoPor.Text == ",")
                {
                    tMonoPor.Text = "0,";
                }
                Monocitos = Convert.ToDouble(tMonoPor.Text);
                //Monocitos
                if (Monocitos < 3.0)
            {
                tMonoPor.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Monocitos > 12.0)
            {
                tMonoPor.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tMonoPor.ForeColor = Color.FromArgb(0, 0, 0);
        }
            if (tLeucocitos.Text != "" && tMonoPor.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tMonoPor.Text)) / 100;
                tMonoU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            //Eosinofilos 
            if (tEosPor.Text != " " && tEosPor.Text != "")
            {
                if (tEosPor.Text == ",")
                {
                    tEosPor.Text = "0,";
                }
                Eosinofilos = Convert.ToDouble(tEosPor.Text);
          
                //Eosinofilos
                if (Eosinofilos < 0.6)
            {
                tEosPor.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Eosinofilos > 7.3)
            {
                tEosPor.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tEosPor.ForeColor = Color.FromArgb(0, 0, 0);
        }
            if (tLeucocitos.Text != ""  && tEosPor.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tEosPor.Text)) / 100;
                tEosU.Text = Resultado.ToString("0.##");
            }
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            if (tBasoPor.Text != " " && tBasoPor.Text != "")
            {
                if (tBasoPor.Text == ",")
                {
                    tBasoPor.Text = "0,";
                }
                Basofilos = Convert.ToDouble(tBasoPor.Text);
                //Basofilos
                if (Basofilos < 0)
            {
                tBasoPor.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Basofilos > 1.7)
            {
                tBasoPor.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tBasoPor.ForeColor = Color.FromArgb(0, 0, 0);
        }
            if (tLeucocitos.Text != "" && tBasoPor.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tBasoPor.Text)) / 100;
                tBasoU.Text = Resultado.ToString("0.##");
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
                else
                {
                    tHematies.ForeColor = Color.FromArgb(0, 0, 0);
                }
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
                else 
                {
                    tHemoglobina.ForeColor = Color.FromArgb(0, 0, 0);
                }
            }
        }
        private void textBox18_TextChanged(object sender, EventArgs e)
        {

            if (tHematocrito.Text != " " && tHematocrito.Text != "")
            {
                if (tHematocrito.Text == ",")
                {
                    tHematocrito.Text = "0,";
                }
                Hematocritos = Convert.ToDouble(tHematocrito.Text);
                //Hematocritos
                if (Hematocritos < 35)
            {
                tHematocrito.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Hematocritos > 54)
            {
                tHematocrito.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tHematocrito.ForeColor = Color.FromArgb(0, 0, 0);
        }
            if (tHematocrito.Text != "")
            {
                Double Resultado;
                Resultado = (Convert.ToDouble(tHematocrito.Text) * 0.32);
                tHemoglobina.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(tHematocrito.Text) * 0.11);
                tHematies.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(tHematocrito.Text) * 10)/Convert.ToDouble(tHematies.Text);
                tVCM.Text = Resultado.ToString("0.##");
                Resultado = (Convert.ToDouble(tHemoglobina.Text) * 100) / Convert.ToDouble(tHematocrito.Text);
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
            double Leuco,Linfo,Neutro,Hemoglobina,Hematocrito,plaquetas,Hematies; 
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
        
            ds = Conexion.VerificarHematologia(IdOrden);
            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUser.ToString());
            string mensaje = "Al momento de Guardar estos Valores se tomara los datos como validados ¿Desea Validar?";
            string titulo = "Alarma";
            try
            {

                Leuco = Convert.ToDouble(tLeucocitos.Text);
                Linfo = Convert.ToDouble(tNeutroPor.Text);
                Neutro = Convert.ToDouble(tLinfoPor.Text);
                Hemoglobina = Convert.ToDouble(tHematocrito.Text);
                Hematocrito = Convert.ToDouble(tHematies.Text);
                plaquetas = Convert.ToDouble(tPlaquetas.Text);;
                if (Leuco < 1)
                {
                    MessageBox.Show("Leucocitos = " + Leuco + " Por Favor coloque otro valor");
                } else if(Linfo < 1)
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
                else
                {
                    MessageBoxButtons button = MessageBoxButtons.YesNo;
                    if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
                    {
                        DialogResult dialog = MessageBox.Show(mensaje, titulo, button, MessageBoxIcon.Warning);
                        if (dialog == DialogResult.Yes)
                        {
                            string cmd = Conexion.InsertarHematologia(IdOrden.ToString(), DateTime.Now.ToString("h:mm:ss tt"), IdAnalisis.ToString(), tComentario.Text, tNeutroPor.Text, tLinfoPor.Text, tMonoPor.Text, tEosPor.Text, tBasoPor.Text, tHematies.Text, tHemoglobina.Text, tHematocrito.Text, tVCM.Text, tHCM.Text, tCHCM.Text, tPlaquetas.Text, tNeutroU.Text, tLinfoU.Text, tMonoU.Text, IdUser.ToString(), tEosU.Text, tBasoU.Text, tLeucocitos.Text);
                            MessageBox.Show(cmd);
                        }
                    }
                    else
                    {
                        MessageBox.Show("No tiene privilegios para realizar esta operacion");
                    }
                }
            }
            catch
            {
                MessageBox.Show("Ha Ocurrido un error, Por favor verificar los valores de los Leucocitos,Neutrofilos,Linfocitos,Hematocrito y/o Hemoglobina");
            }
         

        }

        private void textBox8_TextChanged(object sender, EventArgs e)
        {

            if (tNeutroU.Text != " " && tNeutroU.Text != "")
            {
                if (tNeutroU.Text != " " && tNeutroU.Text != "")
                {
                    if (tNeutroU.Text == ",")
                    {
                        tNeutroU.Text = "0,";
                    }

                    Neutrofilos2 = Convert.ToDouble(tNeutroU.Text);
                    //Neutrofilos2
                    if (Neutrofilos2 < 2)
                    {
                        tNeutroU.ForeColor = Color.FromArgb(0, 110, 242);

                    }
                    else if (Neutrofilos2 > 7)
                    {
                        tNeutroU.ForeColor = Color.FromArgb(150, 40, 130);
                    }
                    else tNeutroU.ForeColor = Color.FromArgb(0, 0, 0);
                }
            }
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            if (tLinfoU.Text != " " && tLinfoU.Text != "")
            {
                if (tLinfoU.Text == ",")
                {
                    tLinfoU.Text = "0,";
                }

                linfocitos2 = Convert.ToDouble(tLinfoU.Text);
                //linfocitos2
                if (linfocitos2 < 1.1)
            {
                tLinfoU.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (linfocitos2 > 2.9)
            {
                tLinfoU.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tLinfoU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (tMonoU.Text != " " && tMonoU.Text != "")
            {
                if (tMonoU.Text == ",")
                {
                    tMonoU.Text = "0,";
                }
                Monocitos2 = Convert.ToDouble(tMonoU.Text);
                //Monocitos2
                if (Monocitos2 < 0.12)
            {
                tMonoU.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Monocitos2 > 1.2)
            {
                tMonoU.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tMonoU.ForeColor = Color.FromArgb(0, 0, 0);
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (tEosU.Text != " " && tEosU.Text != "")
            {
                if (tEosU.Text == ",")
                {
                    tEosU.Text = "0,";
                }
                Eosinofilos2 = Convert.ToDouble(tEosU.Text);
                //Eosinofilos2
                if (Eosinofilos2 < 0.02)
            {
                tEosU.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Eosinofilos2 > 0.50)
            {
                tEosU.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tEosU.ForeColor = Color.FromArgb(0, 0, 0);
        }
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            if (tBasoU.Text != " " && tBasoU.Text != "")
            {
                if (tBasoU.Text == ",")
                {
                    tBasoU.Text = "0,";
                }
                Basofilos2 = Convert.ToDouble(tBasoU.Text);
                //Basofilos2
                if (Basofilos2 < 0.0)
            {
                tBasoU.ForeColor = Color.FromArgb(0, 110, 242);

            }
            else if (Basofilos2 > 0.1)
            {
                tBasoU.ForeColor = Color.FromArgb(150, 40, 130);
            }
            else tBasoU.ForeColor = Color.FromArgb(0, 0, 0);
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {

            string cmd4 = "";
            string cmd3 = "EstadoDeResultado = 1";

            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();

            ds = Conexion.VerificarHematologia(IdOrden);
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
                    string cmd = Conexion.InsertarHematologiaSinValidar(IdOrden.ToString(), DateTime.Now.ToString("h:mm:ss tt"), IdAnalisis.ToString(), tComentario.Text, tNeutroPor.Text, tLinfoPor.Text, tMonoPor.Text, tEosPor.Text, tBasoPor.Text, tHematies.Text, tHemoglobina.Text, tHematocrito.Text, tVCM.Text, tHCM.Text, tCHCM.Text, tPlaquetas.Text, tNeutroU.Text, tLinfoU.Text, tMonoU.Text, IdUser.ToString(), tEosU.Text, tBasoU.Text, tLeucocitos.Text);
                    MessageBox.Show(cmd);
                }
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta operacion");
            }

        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            MoverDerecha();
        
            cargarDatosPaciente();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            MoverIzquierda();
            cargarDatosPaciente();
        }
        private void MoverDerecha()
        {
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
        private void MoverIzquierda()
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

        private void cargarDatosPaciente()
        {
            DataSet ds = new DataSet();
            DataSet ds1 = new DataSet();
            ds = Conexion.Hematologia(IdOrden);
            ds1 = Conexion.TeclasYPrivilegios(IdUser);
            if (ds1.Tables.Count != 0)
            {
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
            }
            try
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                string edad = Conexion.Fecha(nacimiento);
                Edad.Text = edad;
                tComentario.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
                tLeucocitos.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                tNeutroPor.Text = ds.Tables[0].Rows[0]["Neutrofilos"].ToString();
                tLinfoPor.Text = ds.Tables[0].Rows[0]["Linfocitos"].ToString();
                tMonoPor.Text = ds.Tables[0].Rows[0]["Monocitos"].ToString();
                tEosPor.Text = ds.Tables[0].Rows[0]["Eosinofilos"].ToString();
                tBasoPor.Text = ds.Tables[0].Rows[0]["Basofilos"].ToString();
                tHematies.Text = ds.Tables[0].Rows[0]["hematies"].ToString();
                tHemoglobina.Text = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                tHematocrito.Text = ds.Tables[0].Rows[0]["Hematocritos"].ToString();
                tVCM.Text = ds.Tables[0].Rows[0]["VCM"].ToString();
                tHCM.Text = ds.Tables[0].Rows[0]["HCM"].ToString();
                tCHCM.Text = ds.Tables[0].Rows[0]["CHCM"].ToString();
                tPlaquetas.Text = ds.Tables[0].Rows[0]["plaquetas"].ToString();
                tNeutroU.Text = ds.Tables[0].Rows[0]["Neutrofilos2"].ToString();
                tLinfoU.Text = ds.Tables[0].Rows[0]["Linfocitos2"].ToString();
                tMonoU.Text = ds.Tables[0].Rows[0]["Monocitos2"].ToString();
                tEosU.Text = ds.Tables[0].Rows[0]["Eosinofilos2"].ToString();
                tBasoU.Text = ds.Tables[0].Rows[0]["Basofilos2"].ToString();
            }
            catch
            {

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
                    textBox15.Text = Total.ToString();
                    Neu = 0;
                    Lin = 0;
                    Mono = 0;
                    Eos = 0;
                    Baso = 0;        
                    tNeutroPor.Clear();
                    tLinfoPor.Clear();
                    tMonoPor.Clear();
                    tEosPor.Clear();
                    tBasoPor.Clear();
                    tNeutroU.Clear();
                    tLinfoU.Clear();
                    tMonoU.Clear();
                    tEosU.Clear();
                    tBasoU.Clear();
                }
                else 
                {
                    checkBox1.Checked = false;
                }
            }
            else
            {
                Total = 0;
                textBox15.Text = Total.ToString();
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
                if (tBasoPor.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tBasoPor.Text)) / 100;
                    tBasoU.Text = Resultado.ToString("0.##");
                }
                if (tEosPor.Text != "") 
                { 
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tEosPor.Text)) / 100;
                tEosU.Text = Resultado.ToString("0.##");
                }
                if (tMonoPor.Text != "") 
                { 
                Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tMonoPor.Text)) / 100;
                tMonoU.Text = Resultado.ToString("0.##");
                }
                if (tLinfoPor.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tLinfoPor.Text)) / 100;
                    tLinfoU.Text = Resultado.ToString("0.##");
                }
                if (tNeutroPor.Text != "")
                {
                    Resultado = (Convert.ToDouble(tLeucocitos.Text) * Convert.ToDouble(tNeutroPor.Text)) / 100;
                    tNeutroU.Text = Resultado.ToString("0.##");
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
