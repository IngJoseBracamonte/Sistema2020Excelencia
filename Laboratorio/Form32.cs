using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form32 : Form
    {
        bool contraseñausada;
        Bitmap destImage;
        List<Servidores> Server = new List<Servidores>();
        List<Task> Tareas = new List<Task>();
        public static DataSet ds = new DataSet();
        public Form32()
        {
            DatosDeServidores();
            InitializeComponent();
        }
        public class Servidores
        {
            public string iPServer { get; set; }
            public int idServer { get; set; }
            public string estado { get; set; }
        }

        private void Form32_Load(object sender, EventArgs e)
        {
            ds = Conexion.SelectCargo();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                comboBox2.Items.Add(r["idCargo"].ToString());
                comboBox1.Items.Add(r["NombreCargo"].ToString());
            }
        }
      

        private void textBox3_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
           
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;";
            dialog.Title = "Seleccione una Imagen";

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Image img = Image.FromFile(dialog.FileName);
                CambiarTamaño(img, 80, 50);
            }
        }
        private void CambiarTamaño(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            pictureBox1.Image = destImage;

        }
        private void DatosDeServidores()
        {
            Server.Add(new Servidores() { idServer = 0, iPServer = "RIVANA" });
            Server.Add(new Servidores() { idServer = 1, iPServer = "ARCOS PARADA" });
            Server.Add(new Servidores() { idServer = 2, iPServer = "HOSPITALAB" });
            Server.Add(new Servidores() { idServer = 3, iPServer = "NARDO" });
            Server.Add(new Servidores() { idServer = 4, iPServer = "ARO" });
            Server.Add(new Servidores() { idServer = 5, iPServer = "ESPECIALES" });
            Server.Add(new Servidores() { idServer = 9, iPServer = "ARCOS PARADA 2" });
        }
        private void iconButton1_Click(object sender, EventArgs e)
        {

            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            if (Empresa.Tables.Count != 0)
            {
                if (Empresa.Tables[0].Rows.Count != 0)
                {
                    if (Empresa.Tables[0].Rows[0]["IdEmpresa"].ToString() == "5")
                    {
                        foreach (var items in Server)
                        {
                            Tareas.Add(ConexionAlServer(items.iPServer));
                        }
                        Task t = Task.WhenAll(Tareas);
                    }
                    else
                    {
                        BtnGuardar();
                    }
                }
            }
           
        }

        private async Task<string> ConexionAlServer(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                BtnGuardar();
            }
            return results;
        }

        private void BtnGuardar()
        {
            if (!contraseñausada)
            {
                if (textBox1.Text != "" && textBox5.Text != "" && comboBox2.Text != "")
                {
                    bool Guardado = false;
                    //(NombreUsuario, Contraseña, Cargo, CB, MPPS, SIGLAS,Activo)
                    string cmd = string.Format("'{0}','{1}',{2},'{3}','{4}','{5}','1'", textBox1.Text, textBox5.Text, comboBox2.Text, textBox2.Text, textBox3.Text, textBox4.Text);
                    if (pictureBox1.Image != null)
                    {
                        Guardado = true;
                    }

                    string MS = Conexion.InsertarUsuario(cmd, destImage, comboBox2.Text, Guardado);
                    MessageBox.Show(MS);
                }
                else
                {
                    MessageBox.Show("Los campos Nombre,Contraseña y Cargo no pueden estar vacios");
                }
            }
            else
            {
                MessageBox.Show("Ingrese una contraseña que no este en uso");
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            comboBox2.SelectedIndex = comboBox1.SelectedIndex;
            panel1.Paint += new PaintEventHandler(panel1_Paint);
            panel1.Refresh();
           if(comboBox1.Text == "Bioanalista")
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
                textBox4.Enabled = true;
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {
            if (comboBox1.SelectedIndex != -1)
            {
                SizeF sf = e.Graphics.MeasureString(ds.Tables[0].Rows[comboBox1.SelectedIndex]["Descripcion"].ToString(),
                                               new Font(new FontFamily("Bookman Old Style"), 18F), panel1.Width);
                e.Graphics.DrawString(ds.Tables[0].Rows[comboBox1.SelectedIndex]["Descripcion"].ToString(), panel1.Font, Brushes.Black, new RectangleF(new PointF(0F, 0F), sf));
            }
            }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox5_KeyPress(object sender, KeyPressEventArgs e)
        {
          
        }

        private void textBox5_KeyUp(object sender, KeyEventArgs e)
        {
            DataSet ds1 = new DataSet();
            ds1 = Conexion.Contrasena(textBox5.Text);
            if (ds1.Tables[0].Rows.Count != 0)
            {
                if (textBox5.Text == ds1.Tables[0].Rows[0]["Contraseña"].ToString())
                {
                    MessageBox.Show("Contraseña ya en uso");
                    contraseñausada = true;
                }
                else
                {
                   
                }
            }
            else 
            {
                contraseñausada = false;
            }
        }
    }
    }
   


