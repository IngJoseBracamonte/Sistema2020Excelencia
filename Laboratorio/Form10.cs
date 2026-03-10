using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laboratorio
{
    public partial class Form10 : Form
    {
        public Form10()
        {
            InitializeComponent();
        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            textBox1.AppendText(listBox1.SelectedItem.ToString()+ " "+ comboBox1.Text + Environment.NewLine); 
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form10_FormClosing(object sender, FormClosingEventArgs e)
        {
          
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AcceptButton = iconButton7;
        }

        private void Form10_Load(object sender, EventArgs e)
        {
            AcceptButton = iconButton2;
        }
    }
}
