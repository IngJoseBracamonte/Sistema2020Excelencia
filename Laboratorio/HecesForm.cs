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
using FontAwesome.Sharp;


namespace Laboratorio
{
    public partial class HecesForm : Form
    {
        Heces heces = new Heces();
        List<string> Ordenes = new List<string>();
        int IdOrden = 0, IdAnalisis = 0, IdUser;
        int PoscionActual;
        public HecesForm(int idUser, int idOrden, int idAnalisis)
        {
            IdUser = idUser;
            IdOrden = idOrden;
            IdAnalisis = idAnalisis;
            InitializeComponent();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void panel4_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton2.Checked)
            {
                listBox1.Enabled = true;
                iconButton3.Enabled = true;
            }
            else
            {
                listBox1.Enabled = false;
                iconButton3.Enabled = false;
            }

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            Parasitos.Enabled = true;
            if (listBox1.SelectedIndex != 3)
            {
                Parasitos.AppendText(string.Format("{0} {1}", listBox1.SelectedItem.ToString(), comboBox10.Text) + Environment.NewLine);
            }
            else
            {
                Parasitos.AppendText(string.Format("{0}", listBox1.SelectedItem.ToString(), comboBox10.Text) + Environment.NewLine);
            }

        }

        private void Desplazar(string Fecha)
        {
            DataSet Posiciones = new DataSet();
            Posiciones = Conexion.CantidadesDeExamenes(IdAnalisis, Fecha);
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
            if (Ordenes.Count != 0)
            {
                if (Ordenes[0] == Ordenes[PoscionActual])
                {
                    Derecha.Visible = false;
                }
                if (Ordenes[Ordenes.Count - 1] == Ordenes[PoscionActual])
                {
                    Izquierda.Visible = false;
                }
            }
            else
            {
                Derecha.Visible = false;
                Izquierda.Visible = false;
            }

        }

        private void CargarDatosPaciente()
        {
            DataSet ds = new DataSet();
            ds = Conexion.Heces(IdOrden, IdAnalisis);
            if (ds.Tables.Count != 0)
            {
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                string edad = Conexion.Fecha(nacimiento);
                Edad.Text = edad.ToString();
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Color"].ToString()))
                {
                   heces.Color = ColorBox.Text = ds.Tables[0].Rows[0]["Color"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Moco"].ToString()))
                {
                    heces.Moco = MocoBox.Text = ds.Tables[0].Rows[0]["Moco"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Reaccion"].ToString()))
                {
                    heces.Reaccion = ReaccionBox.Text = ds.Tables[0].Rows[0]["Reaccion"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Aspecto"].ToString()))
                {
                    heces.Aspecto = AspectoBox.Text = ds.Tables[0].Rows[0]["Aspecto"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Sangre"].ToString()))
                {
                    heces.Sangre = SangreBox.Text = ds.Tables[0].Rows[0]["Sangre"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Ph"].ToString()))
                {
                    heces.Ph = PH.Text = ds.Tables[0].Rows[0]["Ph"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Consistencia"].ToString()))
                {
                    heces.Consistencia = ConsistenciaBox.Text = ds.Tables[0].Rows[0]["Consistencia"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["RestosAlimenticios"].ToString()))
                {
                    heces.RestosAlimenticios = RestosBox.Text = ds.Tables[0].Rows[0]["RestosAlimenticios"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Hematies"].ToString()))
                {
                    heces.Hematies = HematiesBox.Text = ds.Tables[0].Rows[0]["Hematies"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Leucocitos"].ToString()))
                {
                    heces.Leucocitos = LeucocitosBox.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Parasitos"].ToString()))
                {
                    heces.Parasitos = Parasitos.Text = ds.Tables[0].Rows[0]["Parasitos"].ToString();
                }
                //Desde aqui
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Amilorrea"].ToString()))
                {
                    heces.Amilorrea = cAmilorrea.Text = ds.Tables[0].Rows[0]["Amilorrea"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Creatorrea"].ToString()))
                {
                    heces.Creatorrea = cCreatorrea.Text = ds.Tables[0].Rows[0]["Creatorrea"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Polisacaridos"].ToString()))
                {
                    heces.Polisacaridos = cPolisacaridos.Text = ds.Tables[0].Rows[0]["Polisacaridos"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Gotas"].ToString()))
                {
                    heces.Gotas = cGotas.Text = ds.Tables[0].Rows[0]["Gotas"].ToString();
                }
                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Levaduras"].ToString()))
                {
                    heces.Gotas = cGotas.Text = ds.Tables[0].Rows[0]["Levaduras"].ToString();
                }


                if (!string.IsNullOrWhiteSpace(ds.Tables[0].Rows[0]["Comentario"].ToString()))
                {
                    Comentario.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
                }

            }
        }
        private void HecesForm_Load(object sender, EventArgs e)
        {
            AcceptButton = Validar;
            DataSet Orden = new DataSet();
            Orden = Conexion.FechaDeOrden(IdOrden);
            string Fecha;
            Fecha = Convert.ToDateTime(Orden.Tables[0].Rows[0]["Fecha"].ToString()).ToString("yyyy-MM-dd");
            Desplazar(Fecha);
            CargarDatosPaciente();
            radioButton1.Checked = true;


        }

        private void listBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void panel5_Paint(object sender, PaintEventArgs e)
        {

        }

        private void radioButton2_CheckedChanged_1(object sender, EventArgs e)
        {
            listBox1.Enabled = true;

        }

        private void listBox1_MouseClick_1(object sender, MouseEventArgs e)
        {
            iconButton3.Enabled = true;
            if (listBox1.SelectedIndex == 0 || listBox1.SelectedIndex == 1 || listBox1.SelectedIndex == 11 || listBox1.SelectedIndex == 14 || listBox1.SelectedIndex == 13 || listBox1.SelectedIndex == 19 || listBox1.SelectedIndex == 23 || listBox1.SelectedIndex == 17)
            {
                comboBox10.Items.Clear();

                comboBox10.Items.Add("Huevos");
                comboBox10.Items.Add("Adultos");
                comboBox10.Items.Add("Huevos y larvas");
                comboBox10.Items.Add("Huevos y Adultos");
                comboBox10.Enabled = true;
                comboBox10.SelectedIndex = 0;

            }
            else if (listBox1.SelectedIndex == 7 || listBox1.SelectedIndex == 8 || listBox1.SelectedIndex == 9 || listBox1.SelectedIndex == 10 || listBox1.SelectedIndex == 12 || listBox1.SelectedIndex == 2 || listBox1.SelectedIndex == 15 || listBox1.SelectedIndex == 4 || listBox1.SelectedIndex == 24)
            {
                comboBox10.Items.Clear();
                comboBox10.Enabled = true;
                comboBox10.Items.Add("Quistes");
                comboBox10.Items.Add("Trofozoitos");
                comboBox10.Items.Add("Quistes y Trofozoitos");
                comboBox10.SelectedIndex = 0;
            }
            else if (listBox1.SelectedIndex == 21 || listBox1.SelectedIndex == 22)
            {
                comboBox10.Items.Clear();
                comboBox10.Enabled = true;
                comboBox10.Items.Add("Huevos");
                comboBox10.Items.Add("Adultos");
                comboBox10.Items.Add("Progloties");
                comboBox10.SelectedIndex = 0;
            }
            else if (listBox1.SelectedIndex == 5 || listBox1.SelectedIndex == 6 || listBox1.SelectedIndex == 16)
            {
                comboBox10.Items.Clear();
                comboBox10.Enabled = true;
                comboBox10.Items.Add("Ooquistes");
                comboBox10.SelectedIndex = 0;
            }
            else if (listBox1.SelectedIndex == 18)
            {
                comboBox10.Items.Clear();
                comboBox10.Enabled = true;
                comboBox10.Items.Add("Huevos");
                comboBox10.SelectedIndex = 0;
            }
            else if (listBox1.SelectedIndex == 20)
            {
                comboBox10.Items.Clear();
                comboBox10.Enabled = true;
                comboBox10.Items.Add("larvas");
                comboBox10.SelectedIndex = 0;
            }
            else if (listBox1.SelectedIndex == 3)
            {
                comboBox10.Items.Clear();
                comboBox10.Enabled = true;
                comboBox10.Items.Add("FORMA CON CUERPO CENTRAL");
                comboBox10.Items.Add("FORMA GRANULADA");
                comboBox10.Items.Add("FORMA AMEBOIDE");
                comboBox10.SelectedIndex = 0;
            }
        }

        private void iconButton3_Click_1(object sender, EventArgs e)
        {
            Parasitos.Enabled = true;
            if (listBox1.SelectedIndex == 3)
            {
                Parasitos.AppendText(string.Format("Se Observaron {0} {1}", listBox1.SelectedItem.ToString(), comboBox10.Text) + Environment.NewLine);
            }
            else
            {
                Parasitos.AppendText(string.Format("Se Observaron {1} de {0} ", listBox1.SelectedItem.ToString(), comboBox10.Text) + Environment.NewLine);
            }

        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            Parasitos.Clear();
            Parasitos.Enabled = false;
            listBox1.Enabled = false;
            iconButton3.Enabled = false;
            comboBox10.Items.Clear();
            comboBox10.Enabled = false;
            if (radioButton1.Checked == true)
            {
                Parasitos.AppendText(@"NO SE OBSERVARON HUEVOS, LARVAS, NI ADULTOS DE HELMINTOS.
NO SE OBSERVARON QUISTES, NI TROFOZOITOS DE PROTOZOARIOS.
NO SE OBSERVARON FORMAS EVOLUTIVAS DEL CROMISTA BLASTOCYSTIS SPP.");
            }

        }
            
        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {

        }
        private void CargarSoloHeces()
        {
            ColorBox.Text = heces.Color;
            MocoBox.Text = heces.Moco;
            ReaccionBox.Text = heces.Reaccion;
            AspectoBox.Text  = heces.Aspecto;
            SangreBox.Text = heces.Sangre;
            PH.Text  = heces.Ph;
            ConsistenciaBox.Text = heces.Consistencia;
            RestosBox.Text = heces.RestosAlimenticios;
            HematiesBox.Text = heces.Hematies;
            LeucocitosBox.Text = heces.Leucocitos;
            Parasitos.Text = heces.Parasitos;
            cAmilorrea.Text = heces.Amilorrea;
            cCreatorrea.Text = heces.Creatorrea;
            cPolisacaridos.Text = heces.Polisacaridos;
            cGotas.Text = heces.Gotas;
            cLevaduras.Text = heces.Levaduras;
        }

        private void AsignarHeces()
        {
            heces.IdOrden = IdOrden;
            heces.Color = ColorBox.Text;
            heces.Moco = MocoBox.Text;
            heces.Reaccion = ReaccionBox.Text;
            heces.Aspecto = AspectoBox.Text;
            heces.Sangre = SangreBox.Text;
            heces.Ph = PH.Text;
            heces.Consistencia = ConsistenciaBox.Text;
            heces.RestosAlimenticios = RestosBox.Text;
            heces.Hematies = HematiesBox.Text;
            heces.Leucocitos = LeucocitosBox.Text;
            heces.Parasitos = Parasitos.Text;
            heces.Amilorrea = cAmilorrea.Text;
            heces.Creatorrea = cCreatorrea.Text;
            heces.Polisacaridos = cPolisacaridos.Text;
            heces.Gotas = cGotas.Text;
            heces.Levaduras = cLevaduras.Text;
            heces.Comentario = Comentario.Text;
            heces.IdUsuario = IdUser.ToString();
        }
        private void iconButton2_Click(object sender, EventArgs e)
        {
            AsignarHeces();
            DataSet ds = new DataSet();
            ds = Conexion.VerificarHeces(IdOrden);
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            try
            {
                if (Permisos.Tables[0].Rows[0]["Validar"].ToString() == "1")
                {
                    string cmd3 = "EstadoDeResultado = 2";
                    Conexion.ActualizarOrden(cmd3, IdOrden, IdAnalisis);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        heces.EstadoResultado = "2";
                        string MS= Conexion.InsertarHeces(heces);
                        MessageBox.Show(MS);
                    }
                    else
                    {
                        Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                        if (Permisos.Tables[0].Rows[0]["Modificar"].ToString() == "1")
                        {
                            heces.EstadoResultado = "2";
                            string MS = Conexion.ActualizarHeces(heces);
                            MessageBox.Show(MS);
                        }
                    }
                }
                else
                {
                    string cmd3 = "EstadoDeResultado = 1";
                    Conexion.ActualizarOrden(cmd3, IdOrden, IdAnalisis);
                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        heces.EstadoResultado = "1";
                        string MS = Conexion.InsertarHeces(heces);
                        MessageBox.Show(MS);
                    }
                    else
                    {
                        heces.EstadoResultado = "1";
                        string MS = Conexion.ActualizarHeces(heces);
                        MessageBox.Show(MS);
                    }
                }
            }
            catch (Exception ex)
            {
                Conexion.CrearEvento(ex.ToString());
            }

        }



        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
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

        private void iconButton4_Click(object sender, EventArgs e)
        {
            string cmd3 = "EstadoDeResultado = 1";
            DataSet ds = new DataSet();
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            ds = Conexion.VerificarHeces(IdOrden);
            Conexion.ActualizarOrden(cmd3, IdOrden, IdAnalisis);
            if (ds.Tables[0].Rows.Count == 0)
            {
                string cmd2 = string.Format("Comentario = '{0}'", Comentario.Text);
                string cmd = string.Format("'{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}'", IdOrden, ColorBox.Text, MocoBox.Text, ReaccionBox.Text, AspectoBox.Text, SangreBox.Text, PH.Text, ConsistenciaBox.Text, RestosBox.Text, HematiesBox.Text, LeucocitosBox.Text, Parasitos.Text);
                string MS = Conexion.InsertarHeces(cmd, cmd2, IdOrden, IdAnalisis);
                MessageBox.Show(MS);
            }
            else
            {
                Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
                if (Permisos.Tables[0].Rows[0]["Modificar"].ToString() == "1")
                {
                    Conexion.ActualizarOrden(cmd3, IdOrden, IdAnalisis);
                    string cmd = string.Format("Comentario = '{0}'", Comentario.Text);                                                                                                                                                                                                                                                                                                                                                                                                                                                                  // Color = '{0}' ,Aspecto ='{1}',Densidad = '{2}' ph = '{3}', , Glucosa = '{4}', Bilirrubina = '{5}', Nitritos = '{6}', Leucocitos = '{7}', cetonas = '{8}', Olor = '{9}', hemoglobina = '{10}', Urobilinogeno = '{11}', Benedict = '{12}', Proteinas = '{13}', Robert = '{14}', Bacterias = '{15}', LeucocitosMicro = '{16}',Hematies = '{17}', Mucina = '{18}',CEPLANAS = '{19}',CETRANSICION = '{20}',CREDONDAS = '{21}',BLASTOCONIDAS = '{22}',Cristales = '{23}',Cilindros = '{24}',TiraReactiva = '{25}'
                    string cmd2 = string.Format("Color ='{0}', Moco ='{1}', Reaccion ='{2}', Aspecto ='{3}', Sangre ='{4}', Ph ='{5}', Consistencia ='{6}', RestosAlimenticios ='{7}', Hematies ='{8}', Leucocitos ='{9}', Parasitos ='{10}'", ColorBox.Text, MocoBox.Text, ReaccionBox.Text, AspectoBox.Text, SangreBox.Text, PH.Text, ConsistenciaBox.Text, RestosBox.Text, HematiesBox.Text, LeucocitosBox.Text, Parasitos.Text);
                    string MS = Conexion.ActualizarHeces(cmd, cmd2, IdOrden, IdAnalisis);
                    MessageBox.Show(MS);
                }
            }
        }

        private void comboBox10_SelectedIndexChanged(object sender, EventArgs e)
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
                AcceptButton = Validar;
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
        private void iconButton6_Click(object sender, EventArgs e)
        {
            Moverizquierda();
            CargarHeces();
        }
        private void CargarHeces()
        {

            radioButton1.Checked = true;
            DataSet ds = new DataSet();
            ds = Conexion.Heces(IdOrden, IdAnalisis);
            if (ds.Tables.Count != 0)
            {
                NPaciente.Text = "# " + ds.Tables[0].Rows[0]["NumeroDia"].ToString();
                Sexo.Text = ds.Tables[0].Rows[0]["Sexo"].ToString();
                Nombre.Text = ds.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds.Tables[0].Rows[0]["Apellidos"].ToString();
                DateTime nacimiento = new DateTime(); //Fecha de nacimiento
                nacimiento = DateTime.Parse(ds.Tables[0].Rows[0]["Fecha"].ToString());
                string edad = Conexion.Fecha(nacimiento);
                Edad.Text = edad.ToString();
                ColorBox.Text = ds.Tables[0].Rows[0]["Color"].ToString();
                MocoBox.Text = ds.Tables[0].Rows[0]["Moco"].ToString();
                ReaccionBox.Text = ds.Tables[0].Rows[0]["Reaccion"].ToString();
                AspectoBox.Text = ds.Tables[0].Rows[0]["Aspecto"].ToString();
                SangreBox.Text = ds.Tables[0].Rows[0]["Sangre"].ToString();
                PH.Text = ds.Tables[0].Rows[0]["Ph"].ToString();
                ConsistenciaBox.Text = ds.Tables[0].Rows[0]["Consistencia"].ToString();
                RestosBox.Text = ds.Tables[0].Rows[0]["RestosAlimenticios"].ToString();
                HematiesBox.Text = ds.Tables[0].Rows[0]["Hematies"].ToString();
                LeucocitosBox.Text = ds.Tables[0].Rows[0]["Leucocitos"].ToString();
                Parasitos.Text = ds.Tables[0].Rows[0]["Parasitos"].ToString();
                Comentario.Text = ds.Tables[0].Rows[0]["Comentario"].ToString();
            }
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            MoverDerecha();
            CargarHeces();  
        }
    }
}
