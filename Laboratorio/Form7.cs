using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Configuration;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Form7 : Form
    {
        int i, j, y = 0;
        public string SiguienteEtiqueta;
        public DataSet ds1 = new DataSet();
        int IdUser;
        public Form7(int idUser)
        {
            IdUser = idUser;
            InitializeComponent();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form7_Load(object sender, EventArgs e)
        {
                  DataSet ds = new DataSet();

                ds = Conexion.Etiquetas();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    dataGridView1.DataSource = ds.Tables[0];
                }
    
        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["ImprimirEtiqueta"].ToString() == "1")
            {
                if (dataGridView1.Rows.Count != 0)
                {
                    j = 0;
                    ds1 = Conexion.ImprimirEtiquetas(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["IdOrden"].Value.ToString());
                    if (ds1.Tables[0].Rows.Count != 0)
                    {
                        SiguienteEtiqueta = ds1.Tables[0].Rows[0]["IdSeccion"].ToString();
                        PrintDocument pd = new PrintDocument();
                        pd.PrintPage += new PrintPageEventHandler(printDocument1_PrintPage);
                        string cmd1 = ConfigurationManager.ConnectionStrings["Etiquetas"].ConnectionString;

                        pd.DefaultPageSettings.PrinterSettings.PrinterName = cmd1;
                        pd.Print();
                    }
                    string Cmd = Conexion.ActualizarEtiqueta((int)dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells["IdOrden"].Value);
                    MessageBox.Show(Cmd);
                    DataSet ds = new DataSet();
                    ds = Conexion.Etiquetas();
                    dataGridView1.DataSource = "";
                    if (ds.Tables[0].Rows.Count != 0)
                    {

                        dataGridView1.DataSource = ds.Tables[0];
                    }
                }
                else
                {
                    MessageBox.Show("No hay Etiquetas para imprimir");
                }
            }
            else
            {
                MessageBox.Show("No tiene privilegios para realizar esta accion");
            }
        }

        private void printDocument1_PrintPage(object sender, PrintPageEventArgs e)
        {
            DataSet ds = new DataSet();
            ds = Conexion.Sede();
            if (j < ds1.Tables[0].Rows.Count)
            {
                SiguienteEtiqueta = ds1.Tables[0].Rows[j]["IdSeccion"].ToString();

                i = 28;
                Font drawFont2 = new Font("Arial Rounded MT Bold", 10, FontStyle.Bold);
                Font drawFont = new Font("Arial Rounded MT Bold", 9, FontStyle.Bold);
                Font drawFont3 = new Font("Arial Rounded MT Bold", 8, FontStyle.Bold);
                e.Graphics.DrawString("#" + ds1.Tables[0].Rows[0]["N°"].ToString() + " " + ds1.Tables[0].Rows[0]["Nombre"].ToString() + " " + ds1.Tables[0].Rows[0]["Apellidos"].ToString(), drawFont2, Brushes.Black, 4, 2);
                if (ds1.Tables[0].Rows[j]["Especiales"].ToString() == "1" && (ds1.Tables[0].Rows[j]["IdSeccion"].ToString() != "7") && (ds1.Tables[0].Rows[j]["IdSeccion"].ToString() != "9") && (ds1.Tables[0].Rows[j]["IdSeccion"].ToString() != "10"))
                {
                    //e.Graphics.DrawString("-Especiales " + ds.Tables[0].Rows[0]["Sede"].ToString() + " " + Convert.ToDateTime(ds1.Tables[0].Rows[0]["Fecha"].ToString()).ToString("dd/MM/yyyy"), drawFont3, Brushes.Black, 4, 16);
                    e.Graphics.DrawString("-Especiales " + Convert.ToDateTime(ds1.Tables[0].Rows[0]["Fecha"].ToString()).ToString("dd/MM/yyyy"), drawFont3, Brushes.Black, 4, 16);
                }
                else if (ds1.Tables[0].Rows[j]["Especiales"].ToString() == "1" && (ds1.Tables[0].Rows[j]["IdSeccion"].ToString() == "7") || (ds1.Tables[0].Rows[j]["IdSeccion"].ToString() == "9") || (ds1.Tables[0].Rows[j]["IdSeccion"].ToString() == "10"))
                {
                    //e.Graphics.DrawString("-Especiales " + ds.Tables[0].Rows[0]["Sede"].ToString() + " " + Convert.ToDateTime(ds1.Tables[0].Rows[0]["Fecha"].ToString()).ToString("dd/MM/yyyy"), drawFont3, Brushes.Black, 4, 16);
                     e.Graphics.DrawString("-Centralizado " + Convert.ToDateTime(ds1.Tables[0].Rows[0]["Fecha"].ToString()).ToString("dd/MM/yyyy"), drawFont3, Brushes.Black, 4, 16);
                }
                    else
                {
                    //e.Graphics.DrawString("- "+ds.Tables[0].Rows[0]["Sede"].ToString() + " " + Convert.ToDateTime(ds1.Tables[0].Rows[0]["Fecha"].ToString()).ToString("dd/MM/yyyy"), drawFont3 , Brushes.Black, 4, 16);
                    e.Graphics.DrawString("- " + Convert.ToDateTime(ds1.Tables[0].Rows[0]["Fecha"].ToString()).ToString("dd/MM/yyyy"), drawFont3, Brushes.Black, 4, 16);
                }
                string pr = "-";
                int count = 0;
                for (y = j; (j <= ds1.Tables[0].Rows.Count - 1); j++)
                {
                    if (j == ds1.Tables[0].Rows.Count - 1)
                    {
                        if (SiguienteEtiqueta == ds1.Tables[0].Rows[j]["IdSeccion"].ToString())
                        {
                            pr += "  " + ds1.Tables[0].Rows[j]["Etiquetas"].ToString();
                            if (pr.Length > 30)
                            {
                                e.Graphics.DrawString(pr.ToUpper(), drawFont3, Brushes.Black, 2, i);
                                pr = "";
                                i = i + 10;
                            }
                            e.Graphics.DrawString(pr.ToUpper(), drawFont3, Brushes.Black, 2, i);
                            if (i > 68)
                            {
                                return;
                            }
                        }
                        else
                        {
                            e.HasMorePages = true;
                            return;
                        }

                    }
                    else
                    {
                        if (SiguienteEtiqueta == ds1.Tables[0].Rows[j]["IdSeccion"].ToString())
                        {
                            pr += "  " + ds1.Tables[0].Rows[j]["Etiquetas"].ToString();
                            if (pr.Length > 30)
                            {
                                e.Graphics.DrawString(pr.ToUpper(), drawFont3, Brushes.Black, 2, i);
                                pr = "";
                                i = i + 10;
                            }
                            e.Graphics.DrawString(pr.ToUpper(), drawFont3, Brushes.Black, 2, i);
                            if (i > 68)
                            {
                                e.HasMorePages = true;
                                return;
                            }
                            if (ds1.Tables[0].Rows[j]["IdSeccion"].ToString() == "0")
                            {
                                j++;
                                e.HasMorePages = true;
                                return;
                            }
                        }
                        else
                        {
                            e.HasMorePages = true;
                            return;
                        }
                    }
                }
            }
        }
    }
}

