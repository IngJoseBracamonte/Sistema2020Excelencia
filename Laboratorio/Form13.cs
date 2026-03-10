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
    public partial class Form13 : Form
    {
        Double calculo = 0;
        string Ordencmd;
        string Analisis1;
       string Analisis2;
        string  Analisis3;
        string Analisis4;
        int IdOrden,IdUser, IdAnalisis = 0;
        bool Asignado1 = false, Asignado2 = false, Asignado3 = false;
        Double PrimeroVM, PrimeroVm, SegundoVM, SegundoVm, TerceroVM, TerceroVm;
        List<string> Ordenes = new List<string>();
        int PoscionActual;
        string MultiplesValores;

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (textBox5.Text != "")
            {
                if (Asignado1)
                {
                    if (PrimeroVm > Convert.ToDouble(textBox5.Text))
                    {
                        textBox5.ForeColor = Color.Blue;
                    }
                    else if (PrimeroVM < Convert.ToDouble(textBox5.Text))
                    {
                        textBox5.ForeColor = Color.Red;
                    }
                    else
                    {
                        textBox5.ForeColor = Color.Black;
                    }
                }
                if (textBox2.Text != "")
                {
                    textBox3.Text = formula(textBox5.Text, textBox2.Text).ToString();
                }
            }
           
        }
           
        
        private Double formula(string cmd1, string cmd2)
        {
            switch (IdAnalisis)
            {
                case 151:
                    calculo = (Convert.ToDouble(cmd1) - Convert.ToDouble(cmd2));
                    break;
                case 188:
                    calculo = (Convert.ToDouble(cmd1) * Convert.ToDouble(cmd2)) / 100;
                    break;
                case 206:
                    calculo = (Convert.ToDouble(cmd1) * Convert.ToDouble(cmd2)) / 100;
                    break;
                case 209:
                    calculo = (Convert.ToDouble(cmd1) / Convert.ToDouble(cmd2));
                    break;
                case 211:
                    calculo = (Convert.ToDouble(cmd1) / Convert.ToDouble(cmd2));
                    break;
                case 213:
                    calculo = (Convert.ToDouble(cmd1) / Convert.ToDouble(cmd2));
                    break;
                case 230:
                    calculo = (Convert.ToDouble(cmd1) * Convert.ToDouble(cmd2)) / 100;
                    break;
                case 235:
                    calculo = (Convert.ToDouble(cmd1) * Convert.ToDouble(cmd2)) / 100;
                    break;
                case 236: 
                    calculo = (Convert.ToDouble(cmd1) * Convert.ToDouble(cmd2)) / 100000;
                    break;
                case 237:
                    calculo = (Convert.ToDouble(cmd1) * Convert.ToDouble(cmd2)) / 100;
                    break;
                case 238:
                    calculo = 100*(Convert.ToDouble(cmd2) / Convert.ToDouble(cmd1));
                    break;
                case 225:
                    calculo = (Convert.ToDouble(cmd1) /Convert.ToDouble(cmd2)) * 100;
                    break;
                case 364:
                    calculo = Convert.ToDouble(cmd1) / Convert.ToDouble(cmd2);
                    break;
            }
            calculo = Math.Round(calculo, 2);
            return calculo;
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

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {


            //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)

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
                    string cmd = String.Format("HoraValidacion = '{0}',EstadoDeResultado = 1, IdUsuario = {1}", DateTime.Now.ToString("hh:mm:ss"), IdUser);
                    string MS = Conexion.InsertarSinValidar("", "", IdUser, IdOrden, Convert.ToInt16(Analisis1));
                    MS = Conexion.InsertarSinValidar(textBox5.Text, "", IdUser, IdOrden, Convert.ToInt16(Analisis2));
                    MS = Conexion.InsertarSinValidar(textBox2.Text, "", IdUser, IdOrden, Convert.ToInt16(Analisis3));
                    MS = Conexion.InsertarSinValidar(textBox3.Text, "", IdUser, IdOrden, Convert.ToInt16(Analisis4));
                    MessageBox.Show(MS);
                }
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {

        }

        private void iconButton4_Click(object sender, EventArgs e)
        {

        }

        private void iconButton4_Click_1(object sender, EventArgs e)
        {
                Moverizquierda();
            CargarDatosPaciente();
        }
        private void CargarDatosPaciente()
        {
            DataSet ds3 = new DataSet();
            try
            {
                DataSet ds = new DataSet();
                ds = Conexion.SELECTAnalisisFinal2(IdOrden, IdAnalisis).Result;
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                MultiplesValores = ds.Tables[0].Rows[0]["MultiplesValores"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();

                switch (IdAnalisis)
                {
                    case 151:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "151";
                        Analisis2 = "152";
                        Analisis3 = "153";
                        Analisis4 = "154";
                        break;
                    case 188:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "188";
                        Analisis2 = "133";
                        Analisis3 = "205";
                        Analisis4 = "189";
                        break;
                    case 206:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "206";
                        Analisis2 = "133";
                        Analisis3 = "208";
                        Analisis4 = "207";
                        break;
                    case 207:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "207";
                        Analisis2 = "208";
                        Analisis3 = "133";
                        Analisis4 = "206";
                        break;
                    case 209:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "209";
                        Analisis2 = "8";
                        Analisis3 = "14";
                        Analisis4 = "210";
                        break;
                    case 211:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "211";
                        Analisis2 = "2";
                        Analisis3 = "14";
                        Analisis4 = "212";
                        break;
                    case 213:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "213";
                        Analisis2 = "53";
                        Analisis3 = "14";
                        Analisis4 = "214";
                        break;
                    case 230:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "230";
                        Analisis2 = "133";
                        Analisis3 = "229";
                        Analisis4 = "33";
                        break;
                    case 235:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "235";
                        Analisis2 = "133";
                        Analisis3 = "180";
                        Analisis4 = "181";
                        break;
                    case 236:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "236";
                        Analisis2 = "133";
                        Analisis3 = "157";
                        Analisis4 = "156";
                        break;
                    case 237:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "237";
                        Analisis2 = "133";
                        Analisis3 = "155";
                        Analisis4 = "7";
                        break;
                    case 238:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "238";
                        Analisis2 = "83";
                        Analisis3 = "82";
                        Analisis4 = "184";
                        break;
                    case 225:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "225";
                        Analisis2 = "246";
                        Analisis3 = "182";
                        Analisis4 = "204";
                        break;
                    case 364:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "364";
                        Analisis2 = "365";
                        Analisis3 = "366";
                        Analisis4 = "367";
                        break;
                }

                ds3 = Conexion.SelectOrinas24(Ordencmd, Analisis1, Analisis2, Analisis3, Analisis4);
                if (ds3.Tables[0].Rows.Count != 0)
                {
                    for (int j = 0; j <= ds3.Tables[0].Rows.Count - 1; j++)
                    {
                        if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis1)
                        {
                            Analisis.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                        }
                        else if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis2)
                        {
                            label7.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                            ProteinasV.Text = string.Format("{0} {1} - {2}", ds3.Tables[0].Rows[j]["Unidad"].ToString(), ds3.Tables[0].Rows[j]["ValorMenor"].ToString(), ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                            if (ds3.Tables[0].Rows[j]["ValorMenor"].ToString() != "" && ds3.Tables[0].Rows[j]["ValorMayor"].ToString() != "")
                            {
                                PrimeroVm = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMenor"].ToString());
                                PrimeroVM = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                                Asignado1 = true;
                            }
                            textBox5.Text = ds3.Tables[0].Rows[j]["ValorResultado"].ToString();
                        }
                        else if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis3)
                        {
                            label6.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                            AlbuminaV.Text = string.Format("{0} {1} - {2}", ds3.Tables[0].Rows[j]["Unidad"].ToString(), ds3.Tables[0].Rows[j]["ValorMenor"].ToString(), ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                            if (ds3.Tables[0].Rows[j]["ValorMenor"].ToString() != "" && ds3.Tables[0].Rows[j]["ValorMayor"].ToString() != "")
                            {
                                SegundoVm = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMenor"].ToString());
                                SegundoVM = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                                Asignado2 = true;
                            }
                            textBox2.Text = ds3.Tables[0].Rows[j]["ValorResultado"].ToString();
                        }
                        else if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis4)
                        {
                            label5.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                            GlobulinasV.Text = string.Format("{0} {1} - {2}", ds3.Tables[0].Rows[j]["Unidad"].ToString(), ds3.Tables[0].Rows[j]["ValorMenor"].ToString(), ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                            if (ds3.Tables[0].Rows[j]["ValorMenor"].ToString() != "" && ds3.Tables[0].Rows[j]["ValorMayor"].ToString() != "")
                            {
                                TerceroVm = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMenor"].ToString());
                                TerceroVM = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                                Asignado3 = true;
                            }
                            textBox3.Text = ds3.Tables[0].Rows[j]["ValorResultado"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void iconButton5_Click_1(object sender, EventArgs e)
        {
            MoverDerecha();
            CargarDatosPaciente();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
          
                    if (textBox2.Text != "")
                {
                if (Asignado2)
                {
                    if (SegundoVm > Convert.ToDouble(textBox2.Text))
                    {
                        textBox2.ForeColor = Color.Blue;
                    }
                    else if (SegundoVM < Convert.ToDouble(textBox2.Text))
                    {
                        textBox2.ForeColor = Color.Red;
                    }
                    else
                    {
                        textBox2.ForeColor = Color.Black;
                    }
                }
                if (textBox5.Text != "")
                    {
                        textBox3.Text = formula(textBox5.Text, textBox2.Text).ToString();
                    }
               
                
            }
           
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
           
                if (textBox3.Text != "")
                {
                if (Asignado3)
                {
                    if (TerceroVM > Convert.ToDouble(textBox3.Text))
                    {
                        textBox3.ForeColor = Color.Blue;
                    }
                    else if (TerceroVM < Convert.ToDouble(textBox3.Text))
                    {
                        textBox3.ForeColor = Color.Red;
                    }
                    else
                    {
                        textBox3.ForeColor = Color.Black;
                    }
                }
            }
        }

        public Form13(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
    
            //(ValorResultado,Unidad,HoraValidacion,EstadoDeResultado,Comentario)
       
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
                    string cmd = String.Format("HoraValidacion = '{0}',EstadoDeResultado = 2, IdUsuario = {1}",  DateTime.Now.ToString("hh:mm:ss"), IdUser);
                    string MS = Conexion.InsertarFinal("","",IdUser, IdOrden, Convert.ToInt16(Analisis1));
                    MS = Conexion.InsertarFinal(textBox5.Text,"", IdUser, IdOrden, Convert.ToInt16(Analisis2));
                    MS = Conexion.InsertarFinal(textBox2.Text, "", IdUser, IdOrden, Convert.ToInt16(Analisis3));
                    MS = Conexion.InsertarFinal(textBox3.Text, "", IdUser, IdOrden, Convert.ToInt16(Analisis4));
                    MessageBox.Show(MS);
                }
            }
        }

        private void Form13_Load(object sender, EventArgs e)
        {
            DataSet Posiciones = new DataSet();
            DataSet Fecha = new DataSet();
            Fecha = Conexion.FechaDeOrden(IdOrden);
            Posiciones = Conexion.CantidadesDeExamenes(IdAnalisis, Convert.ToDateTime(Fecha.Tables[0].Rows[0]["Fecha"].ToString()).ToString("yyyy-MM-dd"));
           var  t = Task.Run(() =>
            {
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
            }
            );
            t.Wait();
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
            AcceptButton = iconButton2;
            DataSet ds3 = new DataSet();
            try
            {
                
                DataSet ds = new DataSet();
                ds = Conexion.SELECTAnalisisFinal2(IdOrden, IdAnalisis).Result;    
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                MultiplesValores = ds.Tables[0].Rows[0]["MultiplesValores"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
                int edad = Hoy - nacimiento.Year;
                Edad.Text = Conexion.Fecha(nacimiento);
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();

                switch(IdAnalisis)
                {
                    case 151:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "151";
                        Analisis2 = "152";
                        Analisis3 = "153";
                        Analisis4 = "154";
                        break;
                    case 188:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "188";
                        Analisis2 = "133";
                        Analisis3 = "205";
                        Analisis4 = "189";
                        break;
                    case 206:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "206";
                        Analisis2 = "133";
                        Analisis3 = "208";
                        Analisis4 = "207";
                        break;
                    case 207:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "207";
                        Analisis2 = "208";
                        Analisis3 = "133";
                        Analisis4 = "206";
                        break;
                    case 209:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "209";
                        Analisis2 = "8";
                        Analisis3 = "14";
                        Analisis4 = "210";
                        break;
                    case 211:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "211";
                        Analisis2 = "2";
                        Analisis3 = "14";
                        Analisis4 = "212";
                        break;
                    case 213:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "213";
                        Analisis2 = "53";
                        Analisis3 = "14";
                        Analisis4 = "214";
                        break;
                    case 230:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "230";
                        Analisis2 = "264";
                        Analisis3 = "229";
                        Analisis4 = "33";
                        break;
                    case 235:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 ="235";
                        Analisis2 = "133";
                        Analisis3 =  "180";
                        Analisis4 = "181";
                        break;
                    case 236:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "236";
                        Analisis2 = "133";
                        Analisis3 = "157";
                        Analisis4 = "156";
                        break;
                    case 237:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "237";
                        Analisis2 = "133";
                        Analisis3 = "155";
                        Analisis4 = "7";
                        break;
                    case 238:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "238";
                        Analisis2 = "83";
                        Analisis3 = "82";
                        Analisis4 = "184";
                        break;
                    case 225:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "225";
                        Analisis2 = "246";
                        Analisis3 = "182";
                        Analisis4 = "204";
                        break;
                    case 364:
                        Ordencmd = IdOrden.ToString();
                        Analisis1 = "364";
                        Analisis2 = "365";
                        Analisis3 = "366";
                        Analisis4 = "367";
                        break;
                }
                   
             ds3 = Conexion.SelectOrinas24(Ordencmd, Analisis1, Analisis2, Analisis3, Analisis4    );
                if (ds3.Tables[0].Rows.Count != 0)
                {
                    for (int j = 0; j <= ds3.Tables[0].Rows.Count - 1; j++)
                    {
                        if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis1)
                        {
                            Analisis.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                        }
                        else if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis2)
                        {
                            label7.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                            ProteinasV.Text = string.Format("{0} {1} - {2}", ds3.Tables[0].Rows[j]["Unidad"].ToString(), ds3.Tables[0].Rows[j]["ValorMenor"].ToString(), ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                            if (ds3.Tables[0].Rows[j]["ValorMenor"].ToString() != "" && ds3.Tables[0].Rows[j]["ValorMayor"].ToString() != "")
                            {
                                PrimeroVm = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMenor"].ToString());
                                PrimeroVM = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                                Asignado1 = true;
                            }
                            textBox5.Text = ds3.Tables[0].Rows[j]["ValorResultado"].ToString();
                        }
                        else if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis3)
                        {
                            label6.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                            AlbuminaV.Text = string.Format("{0} {1} - {2}", ds3.Tables[0].Rows[j]["Unidad"].ToString(), ds3.Tables[0].Rows[j]["ValorMenor"].ToString(), ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                            if (ds3.Tables[0].Rows[j]["ValorMenor"].ToString() != "" && ds3.Tables[0].Rows[j]["ValorMayor"].ToString() != "")
                            {
                                SegundoVm = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMenor"].ToString());
                                SegundoVM = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                                Asignado2 = true;
                            }
                            textBox2.Text = ds3.Tables[0].Rows[j]["ValorResultado"].ToString();
                        }
                        else if (ds3.Tables[0].Rows[j]["IdAnalisis"].ToString() == Analisis4)
                        {
                            label5.Text = ds3.Tables[0].Rows[j]["NombreAnalisis"].ToString();
                            GlobulinasV.Text = string.Format("{0} {1} - {2}", ds3.Tables[0].Rows[j]["Unidad"].ToString(), ds3.Tables[0].Rows[j]["ValorMenor"].ToString(), ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                            if (ds3.Tables[0].Rows[j]["ValorMenor"].ToString() != "" && ds3.Tables[0].Rows[j]["ValorMayor"].ToString() != "")
                            {
                                TerceroVm = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMenor"].ToString());
                                TerceroVM = Convert.ToDouble(ds3.Tables[0].Rows[j]["ValorMayor"].ToString());
                                Asignado3 = true;
                            }
                            textBox3.Text = ds3.Tables[0].Rows[j]["ValorResultado"].ToString();
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
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
                AcceptButton = iconButton2;
            }
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
    }
}
