using Conexiones.DbConnect;
using Conexiones.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio
{
    public partial class Login : Form
    {
        List<Servidores> Server = new List<Servidores>();
        bool login = false, esperar;
        string UserName, Mensaje;
        int UserID, IdPaciente;
        System.Windows.Forms.Timer ti;
        string p1;
        string ruta = ConfigurationManager.ConnectionStrings["Probando"].ConnectionString;
        string LocalServer = ConfigurationManager.ConnectionStrings["TipoServer"].ConnectionString;
        Color color;
        string path = "finger.bmp";
        UInt32 pFeatureNum = 0;
        string[] fileEntries;

        public Login()
        {
            ti = new System.Windows.Forms.Timer();
            ti.Interval = 1200;
            InitializeComponent();
            ti.Tick += new EventHandler(ActualizarSecuencia);
            ti.Enabled = false;
        }

        public class Servidores
        {
            public string iPServer { get; set; }
            public int idServer { get; set; }
            public string tiposerver { get; set; }
        }

        public void CrearEvento(string cmd)
        {
            Conexion.CrearEvento(cmd);
        }
        private void LoginCaptahuellas(int IdUser)
        {
            string Test;
                Test = Conexion.TestConnection();
                if (Test == "Conectado")
                {
                    login = true;
                    UserID = IdUser;
                    ti.Enabled = false;
                    Conexion.InsertarHoraDeLlegada(UserID.ToString());
                    Form form2 = new Form2(UserID);
                    form2.Show();
                    this.Visible = false;
                 }
        
        }
        private void ActualizarSecuencia(object ob, EventArgs evt)
        {
 
                string Server;
                esperar = false;
                string combo = ServidoresCBox.Text;
                Cursor.Current = Cursors.WaitCursor;
                Server = Task.Run(() =>
                 ConexionAlServer("Server")
                ).Result;
         

        }
        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click_1(object sender, EventArgs e)
        {
            string Test;
            DataSet DS = new DataSet();
            SeleccionarServer(ServidoresCBox.SelectedIndex);
                DS = Conexion.Login(PasswordBox.Text);
                Test = Conexion.TestConnection();
                if (Test == "Conectado")
                {
                    if (DS.Tables.Count != 0)
                    {
                        if (DS.Tables[0].Rows.Count == 0)
                        {
                            MessageBox.Show("Usuario No Encontrado");
                        }
                        else
                        {
                            login = true;
                            UserID = int.Parse(DS.Tables[0].Rows[0][0].ToString());
                            UserName = DS.Tables[0].Rows[0][1].ToString();
                            ti.Enabled = false;
                            Form form2 = new Form2(UserID);
                            form2.Show();
                            this.Visible = false;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Usuario No Encontrado");
                    }
                }
                else
                {
                    MessageBox.Show("No hay conexion con el servidor al que intenta ingresar");
                }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            this.Visible = true;
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i].Name != "Form1")
                    Application.OpenForms[i].Close();
            }
        }


        private void label3_Click(object sender, EventArgs e)
        {

        }

        private async Task<string> ConexionAlServer(string cmd)
        {
            string results;
            //Conexion Al Servidor
            results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                ServidoresCheck.IconChar = FontAwesome.Sharp.IconChar.Check;
                ServidoresCheck.IconColor = Color.White;
                esperar = true;
            }
            else
            {
                ServidoresCheck.IconChar = FontAwesome.Sharp.IconChar.ExclamationCircle;
                esperar = false;
                ServidoresCheck.IconColor = Color.Red;
            }
            return results;
        }
        private void SeleccionarServer(int Server)
        {
            if (Server == 0)
            {
                LocalServer = "Laboratorio";
                Conexion.Connection(ConfigurationManager.ConnectionStrings["RIVANA"].ConnectionString, "Rivana");
                Mensaje = "Se ha conectado a Rivana";
            } //RIVANA
            else if (Server == 1) //ArcosParada
            {
                LocalServer = "Laboratorio";
                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARCOS PARADA"].ConnectionString, "ARCOS PARADA");
                Mensaje = "Se ha conectado a Arcos Parada";
            }//ArcosParada
            else if (Server == 2)
            {
                LocalServer = "Laboratorio";
                Conexion.Connection(ConfigurationManager.ConnectionStrings["HOSPITALAB"].ConnectionString, "HOSPITALAB");
                Mensaje = "Se ha conectado a Hospitalab";
            }//Hospitalab
            else if (Server == 3)
            {
                LocalServer = "Laboratorio";
                Conexion.Connection(ConfigurationManager.ConnectionStrings["NARDO"].ConnectionString, "NARDO");
                Mensaje = "Se ha conectado a Nardo";

            }//Nardo
            else if (Server == 4)
            {
                LocalServer = "Laboratorio";
                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARO"].ConnectionString, "ARO");
                Mensaje = "Se ha conectado a Aro";

            }//Aro
            else if (Server == 5)
            {
                LocalServer = "Laboratorio";
                Conexion.Connection(ConfigurationManager.ConnectionStrings["ESPECIALES"].ConnectionString, "ESPECIALES");
                Mensaje = "Se ha conectado a Especiales";
            }//Especiales
            else if (Server == 6)
            {
                LocalServer = "Veterinaria";
                Conexion.Connection(ConfigurationManager.ConnectionStrings["PARAVET REMOTO"].ConnectionString, "PARAVET REMOTO");
                Mensaje = "Se ha conectado a Divetech";
            }

        }
        private void DatosDeServidores()
        {
            Server.Add(new Servidores() { idServer = 0, iPServer = "RIVANA", tiposerver = "Laboratorio" });
            Server.Add(new Servidores() { idServer = 1, iPServer = "ARCOS PARADA", tiposerver = "Laboratorio" });
            Server.Add(new Servidores() { idServer = 2, iPServer = "HOSPITALAB", tiposerver = "Laboratorio" });
            Server.Add(new Servidores() { idServer = 3, iPServer = "NARDO", tiposerver = "Laboratorio" });
            Server.Add(new Servidores() { idServer = 4, iPServer = "ARO", tiposerver = "Laboratorio" });
            Server.Add(new Servidores() { idServer = 5, iPServer = "ESPECIALES", tiposerver = "Laboratorio" });
        }


        private void iconButton2_MouseEnter(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.CerrarBtn, "Salir");

        }

        private void iconButton1_MouseEnter(object sender, EventArgs e)
        {
            ToolTip toolTip1 = new ToolTip();
            toolTip1.SetToolTip(this.LoginBtn, "Ingresar");
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
        }

        private void iconButton2_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void textBox1_Click(object sender, EventArgs e)
        {
            this.AcceptButton = LoginBtn;
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

            esperar = false;
            ServidoresCheck.IconChar = FontAwesome.Sharp.IconChar.PauseCircle;
            ServidoresCheck.IconColor = Color.BurlyWood;
            string combo = ServidoresCBox.Text;
            Cursor.Current = Cursors.WaitCursor;
            if (ServidoresCBox.SelectedIndex >= 0)
            {
                int index = 0;
                index = ServidoresCBox.SelectedIndex;
                SeleccionarServer(index);
                Task.Run(() => ConexionAlServer(combo));
            }

        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                //Allows only one Dot Char
            }
            else
            {
                e.Handled = true;
            }
        }

        private void iconButton2_Click_2(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_Load(object sender, EventArgs e)
        {
            
            DatosDeServidores();
            var cmd1 = ConfigurationManager.ConnectionStrings["Server"].ConnectionString;
            Conexion.Connection(cmd1, "Server");
            if (ServidoresCBox.Visible == false)
            {
                label3.Visible = true;
                ActualizarSecuencia(sender, e);
            }
        }

        private void CaptahuellasBtn_Click(object sender, EventArgs e)
        {

            //BUSCAR DISPOSITIVOS CONECTADOS

            //char[,] pName = new char[8, 128];
            byte[] pName = new byte[8 * 128];
            int i, iNum;
            Win32.AvzCloseDevice(Convert.ToInt16(0));
            comboBox1.Items.Clear();
            iNum = Win32.AvzFindDevice(pName);

            for (i = 0; i < iNum; i++)
            {
                comboBox1.Items.Add(Encoding.Default.GetString(pName, i * 128, 128));
            }

            if (iNum > 0)
            {
                comboBox1.SelectedIndex = 0; 
            }

            string archivo = "";
            //SI HAY DISPOSITIVOS CONECTADOS
            if (comboBox1.Items.Count > 0)
            {

                long lRet;

                lRet = Win32.AvzOpenDevice(Convert.ToInt16(0), 0);
                bool encontrado = false;
                int intentos = 0;
                Int32 Ret = 0;
                UInt16 uStatus = 0;
                int IdUser = 0;
         
                while (intentos < 6 && !encontrado)
                {

                    //ENCONTRAR IMAGEN DEL DEDO
                    pFeatureNum = 0;
                    ProcessDirectory(Directory.GetCurrentDirectory());
                    File.Delete(path);
                    Win32.AvzGetImage(Convert.ToInt16(comboBox1.SelectedIndex), Win32.gpImage, ref uStatus);

                    Win32.AvzSaveHueBMPFile(path, Win32.gpImage);

                    Win32.AvzProcess(Win32.gpImage, Win32.gpFeature, Win32.gpBin, 1, 1, 94);

                    Ret = Win32.AvzMatchN(Win32.gpFeature, Win32.gpFeatureLib1, pFeatureNum, 9, 60);

                    if (Ret >= 0)
                    {
                        archivo = Path.GetDirectoryName(fileEntries[Ret]);
                        encontrado = true;
                    }
                    else
                    {
                        Ret = Win32.AvzMatchN(Win32.gpFeature, Win32.gpFeatureLib2, pFeatureNum, 9, 60);

                        if (Ret >= 0)
                        {
                            archivo = Path.GetDirectoryName(fileEntries[Ret]);
                            encontrado = true;
                        }
 
                    }
                    if (Ret >= 0)
                    {
                        string Parse = archivo.Replace(Application.StartupPath + @"\", "");
                        int.TryParse(Parse, out IdUser);

                    }
                    else
                    {
                        Ret = Win32.AvzMatchN(Win32.gpFeature, Win32.gpFeatureLib2, pFeatureNum, 9, 60);

                        if (Ret >= 0)
                        {
                            string Parse = archivo.Replace(Application.StartupPath + @"\", "");
                            int.TryParse(Parse, out IdUser);
                            encontrado = true;
                        }
                        else
                        {
                            MessageBox.Show("Usuario no encontrado");
                        }
                    }

                    intentos++;
                }

                if (encontrado)
                {
                    Win32.AvzCloseDevice(Convert.ToInt16(comboBox1.SelectedIndex));
                    LoginCaptahuellas(IdUser);
                }
         
            }
        }

        private void Form1_Leave(object sender, EventArgs e)
        {

        }
        public void ProcessFile(string path)
        {
            FileStream fs = File.OpenRead(path);
            int i = Convert.ToInt32(fs.Length);
            fs.Read(Win32.gpFeatureBuf, 0, i);
            fs.Close();
            Win32.AvzUnpackFeature(Win32.gpFeatureBuf, Win32.gpFeatureA, Win32.gpFeatureB);
            for (i = 0; i < 256; i++)
            {
                Win32.gpFeatureLib1[pFeatureNum * 256 + i] = Win32.gpFeatureA[i];
                Win32.gpFeatureLib2[pFeatureNum * 256 + i] = Win32.gpFeatureB[i];
            }
        }
        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        public void ProcessDirectory(string targetDirectory)
        {
            // Process the list of files found in the directory.
            fileEntries = Directory.GetFiles(targetDirectory, "*.anv",SearchOption.AllDirectories);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
                pFeatureNum++;
            }
        }

        private void textBox1_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == ',' && !((TextBox)sender).Text.Contains(','))
            {
                //Allows only one Dot Char
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
