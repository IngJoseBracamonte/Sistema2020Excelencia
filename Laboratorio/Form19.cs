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
using FontAwesome.Sharp;

namespace Laboratorio
{
    public partial class Form19 : Form
    {
        private int IdUser;
        int IdOrden = 0;
        private int IdAnalisis;
        private string Comentario;

        public Form19(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }



        private bool ValidacionesLipidoGrama()
        {
            if (string.IsNullOrWhiteSpace(Trigliceridos.Text))
            {

                return false;
            }

            if (string.IsNullOrWhiteSpace(ColesterolTotal.Text))
            {

                return false;
            }
            if (string.IsNullOrWhiteSpace(ColesterolHDL.Text))
            {

                return false;
            }

            double.TryParse(Trigliceridos.Text, out double ETrigliceridos);
            if (ETrigliceridos > 150)
            {

                return false;
            }
            if (string.IsNullOrWhiteSpace(IndiceRiesgo.Text))
            {
                return false;
            }
            double.TryParse(ColesterolTotal.Text, out double EColesterolTotal);
            if (EColesterolTotal > 200)
            {

                return false;
            }
            return true;
        }
        private void textBox4_TextChanged(object sender, EventArgs e)
        {


            if (ValidacionesLipidoGrama())
            {
                ColesterolVLDL.Text = Convert.ToString(Math.Round(Convert.ToDouble(Trigliceridos.Text) / 5, 1));
            }
            else
            {
                ColesterolVLDL.Text = "";
                ColesterolLDL.Text = "";
                IndiceRiesgo.Text = "";
            }


        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (ValidacionesLipidoGrama())
            {
                ColesterolLDL.Text = Convert.ToString(Convert.ToDouble(ColesterolTotal.Text) - Convert.ToDouble(ColesterolHDL.Text) - Convert.ToDouble(ColesterolVLDL.Text));
                IndiceRiesgo.Text = Math.Round(Convert.ToDouble(ColesterolTotal.Text) / Convert.ToDouble(ColesterolHDL.Text), 2).ToString();
            }

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            if (ColesterolHDL.Text != "")
            {
                if (ColesterolVLDL.Text != "" && ColesterolTotal.Text != "")
                {
                    ColesterolLDL.Text = Convert.ToString(Convert.ToDouble(ColesterolTotal.Text) - Convert.ToDouble(ColesterolHDL.Text) - Convert.ToDouble(ColesterolVLDL.Text));
                }
                if (ColesterolTotal.Text != "")
                {
                    IndiceRiesgo.Text = Math.Round(Convert.ToDouble(ColesterolTotal.Text) / Convert.ToDouble(ColesterolHDL.Text), 2).ToString();
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            if (ValidacionesLipidoGrama())
            {
                Double Resultado = Math.Round(Convert.ToDouble(ColesterolTotal.Text) - Convert.ToDouble(ColesterolHDL.Text) - Convert.ToDouble(ColesterolVLDL.Text), 2);
                ColesterolLDL.Text = Resultado.ToString();
            }
        }


        private void AsignarPersona()
        {

            //Query para seleccionar paciente
            DataSet ds = new DataSet();
            ds = Conexion.SelectPersonaOrden(IdOrden);

            //Mapeo de datos
            PersonaOrden persona = new PersonaOrden();
            persona = persona.Mapear(ds.Tables[0].Rows[0]);

            //Asignacion en Labels
            Sexo.Text = persona.Sexo;
            Nombre.Text = persona.NombreCompleto;
            NPaciente.Text = persona.NumeroDia.ToString();
            //Calculo con fecha
            CalcularyAsignarFechaNacimiento(persona);



        }

        private void CalcularyAsignarFechaNacimiento(PersonaOrden persona)
        {
            DateTime nacimiento = new DateTime(); //Fecha de nacimiento
            int Hoy = Convert.ToInt32(DateTime.Now.ToString("yyyy"));
            int edad = Hoy - persona.fecha.Year;
            Edad.Text = edad.ToString();
        }

        private void AsignarExamenes()
        {
            DataSet ds1 = new DataSet();
            ds1 = Conexion.SELECTLipidoGrama(IdOrden);
            try
            {
                foreach (DataRow r in ds1.Tables[0].Rows)
                {
                    if (r["IdAnalisis"].ToString() == "10")
                    {
                        ColesterolTotal.Text = r["ValorResultado"].ToString();
                        label11.Text = String.Format("{0} - {1}", r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                    }
                    if (r["IdAnalisis"].ToString() == "11")
                    {
                        ColesterolHDL.Text = r["ValorResultado"].ToString();
                        label6.Text = String.Format("{0} - {1}", r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                    }
                    if (r["IdAnalisis"].ToString() == "130")
                    {
                        Trigliceridos.Text = r["ValorResultado"].ToString();
                        label9.Text = String.Format("{0} - {1}", r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                    }
                    if (r["IdAnalisis"].ToString() == "120")
                    {
                        ColesterolVLDL.Text = r["ValorResultado"].ToString();
                        label7.Text = String.Format("{0} - {1}", r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                    }
                    if (r["IdAnalisis"].ToString() == "40")
                    {
                        ColesterolLDL.Text = r["ValorResultado"].ToString();
                        label17.Text = String.Format("{0} - {1}", r["ValorMenor"].ToString(), r["ValorMayor"].ToString());
                    }

                }
            }
            catch
            {

            }

        }
        private void Form19_Load(object sender, EventArgs e)
        {

            AsignarPersona();
            AsignarExamenes();
            AcceptButton = iconButton2;


        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

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
                    InsertarFinal();
                }
            }
        }


        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            if (ValidacionesLipidoGrama())
            {
                IndiceRiesgo.Text = Math.Round(Convert.ToDouble(IndiceRiesgo.Text), 2).ToString();
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

        private void iconButton3_Click(object sender, EventArgs e)
        {
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
                    InsertarSinValidar();
                }
            }
        }

        private void InsertarSinValidar()
        {
            try
            {
               Conexion.InsertarSinValidar(ColesterolTotal.Text, "", IdUser, IdOrden, 10);
               Conexion.InsertarSinValidar(ColesterolHDL.Text, "", IdUser, IdOrden, 11);
               Conexion.InsertarSinValidar(Trigliceridos.Text, "", IdUser, IdOrden, 130);
                Dependencia();
               Conexion.InsertarSinValidar("", "", IdUser, IdOrden, 9);
            }
            finally
            {
                MessageBox.Show("agregado satisfactoriamente");
                this.Close();
            }
        }
        private void InsertarFinal()
        {
            try
            {
                Conexion.InsertarFinal(ColesterolTotal.Text, "", IdUser, IdOrden, 10);
                Conexion.InsertarFinal(ColesterolHDL.Text, "", IdUser, IdOrden, 11);
                Conexion.InsertarFinal(Trigliceridos.Text, "", IdUser, IdOrden, 130);
                Dependencia();
               
                Conexion.InsertarFinal("", "", IdUser, IdOrden, 9);
            }
            finally
            {
                MessageBox.Show("agregado satisfactoriamente");
                this.Close();
            }
        }

        private void Dependencia()
        {
            if (string.IsNullOrWhiteSpace(ColesterolLDL.Text))
            {
                Conexion.InsertarSinValidar(ColesterolLDL.Text, "", IdUser, IdOrden, 40);
            }
            else
            {
                Conexion.InsertarFinal(ColesterolLDL.Text, "", IdUser, IdOrden, 40);
            }

            if (string.IsNullOrWhiteSpace(ColesterolVLDL.Text))
            {
                Conexion.InsertarSinValidar(ColesterolVLDL.Text, "", IdUser, IdOrden, 120);
            }
            else
            {
                Conexion.InsertarFinal(ColesterolVLDL.Text, "", IdUser, IdOrden, 120);
            }

            if (string.IsNullOrWhiteSpace(IndiceRiesgo.Text))
            {
                Conexion.InsertarSinValidar(IndiceRiesgo.Text, "", IdUser, IdOrden, 132);
            }
            else
            {
                Conexion.InsertarFinal(IndiceRiesgo.Text, "", IdUser, IdOrden, 132);
            }
        }
    }
}
