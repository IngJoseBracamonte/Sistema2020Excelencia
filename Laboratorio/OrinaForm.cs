using Conexiones.DbConnect;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;

namespace Laboratorio
{
    public partial class OrinaForm : Form
    {
        List<string> Ordenes = new List<string>();
        int PoscionActual,IdOrden,IdAnalisis,IdUser;
        public OrinaForm(int idUser,int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void label22_Click(object sender, EventArgs e)
        {

        }

        private void label25_Click(object sender, EventArgs e)
        {

        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.F2))
            {
                Form form10 = new Form10();
                form10.ShowDialog();
                return true;
            }
            else if (keyData == (Keys.F3))
            {
                MessageBox.Show("Presione F3");
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Desplazar()
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
                    iconButton4.Visible = false;
                }
                if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                {
                    iconButton5.Visible = false;
                }
            }
            else
            {
                iconButton4.Visible = false;
                iconButton5.Visible = false;
            }
        }

        private void CargarDatosOrina()
        {
            DataSet ds = new DataSet();
            ds = Conexion.Orina(IdOrden, IdAnalisis);
            if (ds.Tables.Count != 0)
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                string edad = Conexion.Fecha(nacimiento);
                Edad.Text = edad.ToString();

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Color"].ToString()))
                {
                    ColorBox.Text = ds.Tables[0].Rows[0]["Color"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Aspecto"].ToString()))
                {
                    AspectoBox.Text = ds.Tables[0].Rows[0]["Aspecto"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Densidad"].ToString()))
                {
                    Densidad.Text = ds.Tables[0].Rows[0]["Densidad"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["ph"].ToString()))
                {
                    PH.Text = ds.Tables[0].Rows[0]["ph"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Glucosa"].ToString()))
                {
                    GlucosaBox.Text = ds.Tables[0].Rows[0]["Glucosa"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Bilirrubina"].ToString()))
                {
                    BilliBox.Text = ds.Tables[0].Rows[0]["Bilirrubina"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Nitritos"].ToString()))
                {
                    NitritoBox.Text = ds.Tables[0].Rows[0]["Nitritos"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Leucocitos"].ToString()))
                {
                    LeucocitosBox.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["cetonas"].ToString()))
                {
                    CetoBox.Text = ds.Tables[0].Rows[0]["cetonas"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Olor"].ToString()))
                {
                    OlorBox.Text = ds.Tables[0].Rows[0]["Olor"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Hemoglobina"].ToString()))
                {
                    HemoBox.Text = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Urobilinogeno"].ToString()))
                {
                    UroBox.Text = ds.Tables[0].Rows[0]["Urobilinogeno"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Benedict"].ToString()))
                {
                    BenedicBox.Text = ds.Tables[0].Rows[0]["Benedict"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Proteinas"].ToString()))
                {
                    ProteinasBox.Text = ds.Tables[0].Rows[0]["Proteinas"].ToString();
                }

                if (!string.IsNullOrEmpty(ds.Tables[0].Rows[0]["Robert"].ToString()))
                {
                    RobertBox.Text = ds.Tables[0].Rows[0]["Robert"].ToString();
                }
                comboBox26.Text = ds.Tables[0].Rows[0]["Bacterias"].ToString();
                comboBox25.Text = ds.Tables[0].Rows[0]["LeucocitosMicro"].ToString();
                comboBox23.Text = ds.Tables[0].Rows[0]["Hematies"].ToString();
                comboBox22.Text = ds.Tables[0].Rows[0]["Mucina"].ToString();
                comboBox14.Text = ds.Tables[0].Rows[0]["CEPLANAS"].ToString();
                comboBox19.Text = ds.Tables[0].Rows[0]["CETRANSICION"].ToString();
                comboBox18.Text = ds.Tables[0].Rows[0]["CREDONDAS"].ToString();
                comboBox17.Text = ds.Tables[0].Rows[0]["BLASTOCONIDAS"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                if (ds.Tables[0].Rows[0]["Cristales"].ToString() != "")
                {
                    textBox3.Text = ds.Tables[0].Rows[0]["Cristales"].ToString();
                }
                else
                {
                    textBox3.Text = "NO SE OBSERVARON";
                }
                if (ds.Tables[0].Rows[0]["Cilindros"].ToString() != "")
                {
                    textBox4.Text = ds.Tables[0].Rows[0]["Cilindros"].ToString();
                }
                else
                {
                    textBox4.Text = "NO SE OBSERVARON";
                }

                TiraBox.Text = ds.Tables[0].Rows[0]["TiraReactiva"].ToString();
                textBox5.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
            }

        }
        private void OrinaForm_Load(object sender, EventArgs e)
        {
            Desplazar();
            CargarDatosOrina();


        }


        private void button2_Click(object sender, EventArgs e)
        {
            Form11 form11 = new Form11();
            form11.ShowDialog();
            textBox4.Clear();
            textBox4.Text = form11.textBox1.Text;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form10 form10 = new Form10();
            form10.ShowDialog();
            textBox3.Clear();
            textBox3.Text = form10.textBox1.Text;
        }

        private void OrinaForm_Enter(object sender, EventArgs e)
        {

        }

        private void button1_Enter(object sender, EventArgs e)
        {

        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

            DataSet ds = new DataSet();
            string cmd3 = string.Format("Comentario = '{0}',IdUsuario = {1},HoraValidacion = '{2}'", textBox5.Text, IdUser, DateTime.Now.ToString("HH:mm:ss"));
            ds = Conexion.VerificarOrina(IdOrden);
            DataSet ds2 = new DataSet();
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
                    string MS = Conexion.InsertarOrinas(IdOrden.ToString(), IdAnalisis.ToString(), IdUser.ToString(), textBox5.Text, ColorBox.Text, AspectoBox.Text, Densidad.Text, PH.Text, GlucosaBox.Text, BilliBox.Text, NitritoBox.Text, LeucocitosBox.Text, CetoBox.Text, OlorBox.Text, HemoBox.Text, UroBox.Text, BenedicBox.Text, ProteinasBox.Text, RobertBox.Text, comboBox26.Text, comboBox25.Text, comboBox23.Text, comboBox22.Text, comboBox14.Text, comboBox19.Text, comboBox18.Text, comboBox17.Text, textBox3.Text, textBox4.Text, TiraBox.Text, "2");
                    MessageBox.Show(MS);
                }
            }
            else
            {
                string MS = Conexion.InsertarOrinas(IdOrden.ToString(), IdAnalisis.ToString(), IdUser.ToString(), textBox5.Text, ColorBox.Text, AspectoBox.Text, Densidad.Text, PH.Text, GlucosaBox.Text, BilliBox.Text, NitritoBox.Text, LeucocitosBox.Text, CetoBox.Text, OlorBox.Text, HemoBox.Text, UroBox.Text, BenedicBox.Text, ProteinasBox.Text, RobertBox.Text, comboBox26.Text, comboBox25.Text, comboBox23.Text, comboBox22.Text, comboBox14.Text, comboBox19.Text, comboBox18.Text, comboBox17.Text, textBox3.Text, textBox4.Text, TiraBox.Text, "1");
                MessageBox.Show(MS);
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

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void label14_Click(object sender, EventArgs e)
        {

        }

        private void comboBox9_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {

            DataSet ds = new DataSet();
            string cmd3 = "EstadoDeResultado = 2";
            ds = Conexion.VerificarOrina(IdOrden);
            DataSet ds2 = new DataSet();
            string titulo = "Alarma";
            MessageBoxButtons button = MessageBoxButtons.YesNo;
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["Validar"].ToString() == "1")
            {
                string MS = Conexion.InsertarOrinas(IdOrden.ToString(), IdAnalisis.ToString(), IdUser.ToString(), textBox5.Text, ColorBox.Text, AspectoBox.Text, Densidad.Text, PH.Text, GlucosaBox.Text, BilliBox.Text, NitritoBox.Text, LeucocitosBox.Text, CetoBox.Text, OlorBox.Text, HemoBox.Text, UroBox.Text, BenedicBox.Text, ProteinasBox.Text, RobertBox.Text, comboBox26.Text, comboBox25.Text, comboBox23.Text, comboBox22.Text, comboBox14.Text, comboBox19.Text, comboBox18.Text, comboBox17.Text, textBox3.Text, textBox4.Text, TiraBox.Text, "1");
                MessageBox.Show(MS);
            }
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {

            if (PoscionActual > 0)
            {
                PoscionActual = PoscionActual - 1;
                IdOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
                if (Ordenes.Count != 0)
                {
                    if (Ordenes[0] == Ordenes[PoscionActual])
                    {
                        iconButton4.Visible = false;
                    }
                    else
                    {
                        iconButton4.Visible = true;
                    }
                    if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                    {
                        iconButton5.Visible = false;
                    }
                    else
                    {
                        iconButton5.Visible = true;
                    }
                }
                else
                {
                    iconButton4.Visible = false;
                    iconButton5.Visible = false;
                }
            }
            DataSet ds = new DataSet();
            ds = Conexion.Orina(IdOrden, IdAnalisis);
            if (ds.Tables.Count != 0)
            {
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                string edad = Conexion.Fecha(nacimiento);
                Edad.Text = edad.ToString();
                ColorBox.Text = ds.Tables[0].Rows[0]["Color"].ToString();
                AspectoBox.Text = ds.Tables[0].Rows[0]["Aspecto"].ToString();
                Densidad.Text = ds.Tables[0].Rows[0]["Densidad"].ToString();
                PH.Text = ds.Tables[0].Rows[0]["ph"].ToString();
                GlucosaBox.Text = ds.Tables[0].Rows[0]["Glucosa"].ToString();
                BilliBox.Text = ds.Tables[0].Rows[0]["Bilirrubina"].ToString();
                NitritoBox.Text = ds.Tables[0].Rows[0]["Nitritos"].ToString();
                LeucocitosBox.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                CetoBox.Text = ds.Tables[0].Rows[0]["cetonas"].ToString();
                OlorBox.Text = ds.Tables[0].Rows[0]["Olor"].ToString();
                HemoBox.Text = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                UroBox.Text = ds.Tables[0].Rows[0]["Urobilinogeno"].ToString();
                BenedicBox.Text = ds.Tables[0].Rows[0]["Benedict"].ToString();
                ProteinasBox.Text = ds.Tables[0].Rows[0]["Proteinas"].ToString();
                RobertBox.Text = ds.Tables[0].Rows[0]["Robert"].ToString();
                comboBox26.Text = ds.Tables[0].Rows[0]["Bacterias"].ToString();
                comboBox25.Text = ds.Tables[0].Rows[0]["LeucocitosMicro"].ToString();
                comboBox23.Text = ds.Tables[0].Rows[0]["Hematies"].ToString();
                comboBox22.Text = ds.Tables[0].Rows[0]["Mucina"].ToString();
                comboBox14.Text = ds.Tables[0].Rows[0]["CEPLANAS"].ToString();
                comboBox19.Text = ds.Tables[0].Rows[0]["CETRANSICION"].ToString();
                comboBox18.Text = ds.Tables[0].Rows[0]["CREDONDAS"].ToString();
                comboBox17.Text = ds.Tables[0].Rows[0]["BLASTOCONIDAS"].ToString();
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                if (ds.Tables[0].Rows[0]["Cristales"].ToString() != "")
                {
                    textBox3.Text = ds.Tables[0].Rows[0]["Cristales"].ToString();
                }
                else
                {
                    textBox3.Text = "NO SE OBSERVARON";
                }
                if (ds.Tables[0].Rows[0]["Cilindros"].ToString() != "")
                {
                    textBox4.Text = ds.Tables[0].Rows[0]["Cilindros"].ToString();
                }
                else
                {
                    textBox4.Text = "NO SE OBSERVARON";
                }

                TiraBox.Text = ds.Tables[0].Rows[0]["TiraReactiva"].ToString();
                textBox5.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
            }


        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            if (PoscionActual < Ordenes.Count - 1)
            {
                PoscionActual = PoscionActual + 1;
                IdOrden = Convert.ToInt32(Ordenes[PoscionActual].ToString());
                if (Ordenes.Count != 0)
                {
                    if (Ordenes[0] == Ordenes[PoscionActual])
                    {
                        iconButton4.Visible = false;
                    }
                    else
                    {
                        iconButton4.Visible = true;
                    }
                    if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                    {
                        iconButton5.Visible = false;
                    }
                    else
                    {
                        iconButton5.Visible = true;
                    }
                }
                else
                {
                    iconButton4.Visible = false;
                    iconButton5.Visible = false;
                }
                DataSet ds = new DataSet();
                ds = Conexion.Orina(IdOrden, IdAnalisis);
                if (ds.Tables.Count != 0)
                {
                    Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                    Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                    DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                    nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                    string edad = Conexion.Fecha(nacimiento);
                    Edad.Text = edad.ToString();
                    ColorBox.Text = ds.Tables[0].Rows[0]["Color"].ToString();
                    AspectoBox.Text = ds.Tables[0].Rows[0]["Aspecto"].ToString();
                    Densidad.Text = ds.Tables[0].Rows[0]["Densidad"].ToString();
                    PH.Text = ds.Tables[0].Rows[0]["ph"].ToString();
                    GlucosaBox.Text = ds.Tables[0].Rows[0]["Glucosa"].ToString();
                    BilliBox.Text = ds.Tables[0].Rows[0]["Bilirrubina"].ToString();
                    NitritoBox.Text = ds.Tables[0].Rows[0]["Nitritos"].ToString();
                    LeucocitosBox.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                    CetoBox.Text = ds.Tables[0].Rows[0]["cetonas"].ToString();
                    OlorBox.Text = ds.Tables[0].Rows[0]["Olor"].ToString();
                    HemoBox.Text = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                    UroBox.Text = ds.Tables[0].Rows[0]["Urobilinogeno"].ToString();
                    BenedicBox.Text = ds.Tables[0].Rows[0]["Benedict"].ToString();
                    ProteinasBox.Text = ds.Tables[0].Rows[0]["Proteinas"].ToString();
                    RobertBox.Text = ds.Tables[0].Rows[0]["Robert"].ToString();
                    comboBox26.Text = ds.Tables[0].Rows[0]["Bacterias"].ToString();
                    comboBox25.Text = ds.Tables[0].Rows[0]["LeucocitosMicro"].ToString();
                    comboBox23.Text = ds.Tables[0].Rows[0]["Hematies"].ToString();
                    comboBox22.Text = ds.Tables[0].Rows[0]["Mucina"].ToString();
                    comboBox14.Text = ds.Tables[0].Rows[0]["CEPLANAS"].ToString();
                    comboBox19.Text = ds.Tables[0].Rows[0]["CETRANSICION"].ToString();
                    comboBox18.Text = ds.Tables[0].Rows[0]["CREDONDAS"].ToString();
                    comboBox17.Text = ds.Tables[0].Rows[0]["BLASTOCONIDAS"].ToString();
                    NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                    if (ds.Tables[0].Rows[0]["Cristales"].ToString() != "")
                    {
                        textBox3.Text = ds.Tables[0].Rows[0]["Cristales"].ToString();
                    }
                    else
                    {
                        textBox3.Text = "NO SE OBSERVARON";
                    }
                    if (ds.Tables[0].Rows[0]["Cilindros"].ToString() != "")
                    {
                        textBox4.Text = ds.Tables[0].Rows[0]["Cilindros"].ToString();
                    }
                    else
                    {
                        textBox4.Text = "NO SE OBSERVARON";
                    }

                    TiraBox.Text = ds.Tables[0].Rows[0]["TiraReactiva"].ToString();
                    textBox5.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
                }

            }

        }
    }
}
