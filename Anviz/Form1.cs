using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace OA99_PLUS_DEMO
{
    public partial class Form1 : Form
    {
        int intentos;
        string path = "finger.bmp";
        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //char[,] pName = new char[8, 128];
            byte[] pName = new byte[8 * 128];
            int i, iNum;

            comboBox1.Items.Clear();
            iNum = Win32.AvzFindDevice(pName);

            for (i = 0; i < iNum; i++)
            {
                comboBox1.Items.Add(Encoding.Default.GetString(pName, i * 128, 128));
            }

            if (iNum > 0)
                comboBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            long lRet;

            lRet = Win32.AvzOpenDevice(Convert.ToInt16(comboBox1.SelectedIndex), 0);

            if (lRet == 0)
            {
                MessageBox.Show("Open Device OK");
            }
            else
            {
                MessageBox.Show("Open Device Fail");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        { 
           
            File.Delete(path);
            UInt16 uStatus = 0;
            Win32.AvzGetImage(Convert.ToInt16(comboBox1.SelectedIndex), Win32.gpImage, ref uStatus);

            Win32.AvzSaveHueBMPFile(path, Win32.gpImage);
            
            Image bitmap1 = Image.FromFile(path);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmap1.Dispose();
            Image bitmap2 = Image.FromStream(ms);

            //Win32.AvzShowImage(m_btnImage.Handle, Win32.gpImage, 10, 10, 236, 270, 10, 10, 236, 270);

            m_btnImage.Image = bitmap2;    
            ms.Close();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            File.Delete(path);
            //IntPtr p = IntPtr.Zero;
            //IntPtr p = System.Runtime.InteropServices.Marshal.AllocHGlobal(256 * 280);
            //Win32.AvzGetImage(Convert.ToInt16(comboBox1.SelectedIndex), p, ref uStatus);

            UInt16 uStatus = 0;
            Win32.AvzGetImage(Convert.ToInt16(comboBox1.SelectedIndex), Win32.gpImage, ref uStatus);
            Win32.AvzProcess(Win32.gpImage, Win32.gpFeature, Win32.gpBin, 1, 1,94);


            //Win32.AvzSaveHueBMPFile(path, Win32.gpBin);
            Win32.AvzSaveClrBMPFile(path, Win32.gpBin);

            Image photo = Image.FromFile(path);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            photo.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            photo.Dispose();
            Image bitmap2 = Image.FromStream(ms);
            m_btnImage.Image = bitmap2;

        }

        private void button8_Click(object sender, EventArgs e)
        {
            UInt16 Ret = 0;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Feature files|*.anv";
            DialogResult result;

            // Displays the MessageBox.

            result = dlg.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Text = dlg.FileName;
                using (BinaryWriter binWriter = new BinaryWriter(File.Open(textBox1.Text, FileMode.Create)))
                {
                    binWriter.Write(Win32.gpFeatureBuf, 0, Ret);
                    Win32.AvzUnpackFeature(Win32.gpFeatureBuf, Win32.gpFeatureA, Win32.gpFeatureB);
                }
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            Int32 Ret;
            //File.Delete(path);
            UInt16 uStatus = 0;
            string Prubite = "19220000016929122276716111632721291081557211011491353394237419115622512986149741618518719378192021781193977413515922566140196335925224334715894161451431583326132209718151971291161541973410515013516278339519472219698671620198604372521417134518195344718135162391119716213219234121351946600000000000000000000000000000000000000000000000000004214519219280000016929122276716111632721291081557211011491353394237419115622512986149741618518719378192021781193977413515922566140196335925224334715894161451431583326132209718151971291161541973410515013516278339519472219698671620198604372521417134518195344718135162391119716213219234121351946600000000000000000000000000000000000000000000000000004215119200000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000";
            Win32.AvzGetImage(Convert.ToInt16(comboBox1.SelectedIndex), Win32.gpImage, ref uStatus);
            Win32.gpFeature = Encoding.ASCII.GetBytes(Prubite);
            Win32.AvzProcess(Win32.gpImage, Win32.gpFeature, Win32.gpBin, 1, 1,94);

            Ret = Win32.AvzMatch(Win32.gpFeature, Win32.gpFeatureA, 9,60);

            if (Ret == 0)
            {
                textBox2.Text = "OK";

            }
            else
            {
                textBox2.Text = "Fail";
            }

        }

        private void button7_Click(object sender, EventArgs e)
        {
            UInt16 Ret;
            intentos++;
            string targetDirectory = @"C:\Users\J_Bra\source\repos\Sistema2020\Laboratorio\bin\Debug\2\";
            Ret = Win32.AvzPackFeature(Win32.gpFeature, Win32.gpFeature, Win32.gpFeatureBuf);
            using (BinaryWriter binWriter = new BinaryWriter(File.Open($"{targetDirectory}\\{intentos}.anv", FileMode.Create)))
            {
                binWriter.Write(Win32.gpFeatureBuf, 0, Ret);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {

            using (BinaryWriter binWriter = new BinaryWriter(File.Open("Mybin.row", FileMode.Create)))
            {
                binWriter.Write(Win32.gpBin, 0, Win32.gpBin.Length);
            }
            
            //Win32.AvzSaveHueBMPFile("Mybin.bmp", Win32.gpBin);
            Win32.AvzSaveClrBMPFile("Mybin.bmp", Win32.gpBin);
            
            Image bitmap1 = Image.FromFile("Mybin.bmp");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmap1.Dispose();
            Image bitmap2 = Image.FromStream(ms);
            m_btnImage.Image = bitmap2;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            UInt16 uStatus = 0;
            Win32.AvzGetImage(Convert.ToInt16(comboBox1.SelectedIndex), Win32.gpImage, ref uStatus);
            Win32.AvzProcess(Win32.gpImage, Win32.gpFeature, Win32.gpBin, 1, 1,94);

            Win32.AvzSaveHueBMPFile("Mybmp.bmp", Win32.gpImage);
            Image bitmap1 = Image.FromFile("Mybmp.bmp");
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            bitmap1.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            bitmap1.Dispose();
            Image bitmap2 = Image.FromStream(ms);
            m_btnImage.Image = bitmap2;
        }
        UInt32 pFeatureNum = 0;
        string[] fileEntries;
        // Process all files in the directory passed in, recurse on any directories 
        // that are found, and process the files they contain.
        public void ProcessDirectory(string targetDirectory)
        {
            targetDirectory = @"C:\Users\J_Bra\source\repos\Sistema2020\Laboratorio\bin\Debug\";
            // Process the list of files found in the directory.
            fileEntries = Directory.GetFiles(targetDirectory, "*.anv",SearchOption.AllDirectories);
            foreach (string fileName in fileEntries)
            {
                ProcessFile(fileName);
                pFeatureNum++;
            }
        }

        // Insert logic for processing found files here.
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


        private void button10_Click(object sender, EventArgs e)
        {
            pFeatureNum = 0;
            string Directory = @"C:\Users\J_Bra\source\repos\Sistema2020\Laboratorio\bin\Debug\";
            ProcessDirectory(Directory);
            textBox3.Text = Convert.ToString(pFeatureNum);

            Int32 Ret;
            UInt16 uStatus = 0;
            Win32.AvzGetImage(Convert.ToInt16(comboBox1.SelectedIndex), Win32.gpImage, ref uStatus);
            Win32.AvzProcess(Win32.gpImage, Win32.gpFeature, Win32.gpBin, 1, 1,94);

            Ret = Win32.AvzMatchN(Win32.gpFeature, Win32.gpFeatureLib1, pFeatureNum, 9, 60);

            if (Ret >= 0)
            {
                textBox4.Text = "OK " + Path.GetFileName(fileEntries[Ret]);
            }
            else
            {
                Ret = Win32.AvzMatchN(Win32.gpFeature, Win32.gpFeatureLib2, pFeatureNum, 9, 60);

                if (Ret >= 0)
                {
                    textBox4.Text = "OK " + Path.GetFileName(fileEntries[Ret]);

                }
                else

                    textBox4.Text = "Fail";
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (comboBox1.SelectedIndex != -1) 
            {
                Win32.AvzCloseDevice(Convert.ToInt16(comboBox1.SelectedIndex));
            }

        }

        private void button11_Click(object sender, EventArgs e)
        {
            UInt32 CardID=0;
            Win32.AvzGetCard(Convert.ToInt16(comboBox1.SelectedIndex), ref CardID);
            textBox6.Text = Convert.ToString(CardID);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
