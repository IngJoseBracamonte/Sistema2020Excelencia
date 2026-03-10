using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form27 : Form
    {
        string ruta;
        public Bitmap bitmap2;
        public Form27()
        {
            InitializeComponent();
        }

        private void iconButton9_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            openFileDialog1.ShowDialog();
            if (openFileDialog1.FileName != "" && openFileDialog1.FileName != "openFileDialog1") 
            {
                bitmap2 = new Bitmap(openFileDialog1.FileName, true); 
                pictureBox1.Image = Bitmap.FromFile(openFileDialog1.FileName);
                ruta = "C:\\Rivana\\Nuevo\\Logos\\" + textBox2.Text+".JPEG";
               
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            string cmd = String.Format("'{0}','{1}','{2}'",textBox4.Text,textBox2.Text,ruta);
            string MS;
           MS= Conexion.EmpresaLogo(cmd);
            if (MS == "")
            {
                MessageBox.Show("Ha Ocurrido un Error");
            }
            else 
            {
                bitmap2.Save(ruta, ImageFormat.Jpeg);
                MessageBox.Show(MS);
            }
            
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
