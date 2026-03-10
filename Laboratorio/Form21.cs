using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones.DbConnect;
using SpreadsheetLight;


namespace Laboratorio
{
    public partial class Ordenestxt : Form
    {
        public Ordenestxt()
        {
            InitializeComponent();
        }

        private void Form21_Load(object sender, EventArgs e)
        {
            DataSet ds = new DataSet();
           
            string fecha = DateTime.Now.ToString("yyyy-MM-dd");
            ds = Conexion.SELECTTotalFacturadoFecha(fecha, fecha);
            if (ds.Tables[0].Rows.Count != 0)
            {
                if (ds.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != " " && ds.Tables[0].Rows[0]["SumaDePrecioF"].ToString() != "")
                {
                    Facturado.Text = ds.Tables[0].Rows[0]["SumaDePrecioF"].ToString().Replace(".", ","); ;
                }
                else
                {
                    Facturado.Text = "0";
                }
                DataSet dsT = new DataSet(); 
                dsT = Conexion.SELECTTotalFacturadoDia(fecha, fecha);
                if (dsT.Tables[0].Rows[0]["Total"].ToString() != " " && dsT.Tables[0].Rows[0]["Total"].ToString() != "")
                {
                    cobrado.Text = dsT.Tables[0].Rows[0]["Total"].ToString().Replace(".", ","); ;
                }
                else
                {
                    cobrado.Text = "0";
                }
                double PrecioF=0, Total = 0, Calculo;
                string patron = @"[^\w]";
                Regex regex = new Regex(patron);
                string Text1, Text2;
                Text1 = Facturado.Text.Replace(".",",");
                Text2 = cobrado.Text.Replace(".", ",");
                double.TryParse(Text1, out PrecioF);
                double.TryParse(Text2, out Total);
                Calculo = Total - PrecioF;
                diferencia.Text = Calculo.ToString("N2", CultureInfo.CreateSpecificCulture("es-VE"));
            }
            else 
            {
                Facturado.Text = "0";
            }
            DataSet ds2 = new DataSet();
            ds2 = Conexion.TotalDetallado();
            if (ds2.Tables[0].Rows.Count != 0)
            {
                foreach (DataRow r in ds2.Tables[0].Rows)
                {
                    if (r["tipodepago"].ToString() == "Efectivo")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            efectivoitxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            efectivoitxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            efectivoVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            efectivoVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            efectivoTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            efectivoTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Punto")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            POVitxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            POVitxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            POVVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            POVVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            POVTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            POVTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Pago Movil")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            PAGOMovilItxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            PAGOMovilItxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            PAGOMovilVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            PAGOMovilVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            PAGOMovilTtxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            PAGOMovilTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Transferencia")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            transferenciasitxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            transferenciasitxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            transferenciasVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            transferenciasVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            transferenciasTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            transferenciasTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Dolar")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            Dolaritxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("C", new CultureInfo("en-US"));
                        }
                        else
                        {
                            Dolaritxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            DolarVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("C", new CultureInfo("en-US"));
                        }
                        else
                        {
                            DolarVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            DolarTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("C", new CultureInfo("en-US"));
                        }
                        else
                        {
                            DolarTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Pesos")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            pesositxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            pesositxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            pesosVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            pesosVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            pesosTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            pesosTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Euros")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            eurositxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("C", new CultureInfo("fr-FR"));
                        }
                        else
                        {
                            eurositxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            eurosVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("C", new CultureInfo("fr-FR"));
                        }
                        else
                        {
                            eurosVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            eurosTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("C", new CultureInfo("fr-FR"));
                        }
                        else
                        {
                            eurosTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Otros")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            otrositxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            otrositxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            otrosVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            otrosVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            otrosTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            otrosTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Ordenes")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            Ordenesitxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            Ordenesitxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            OrdenesVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            OrdenesVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            OrdenesTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            OrdenesTtxt.Text = "0";
                        }
                    }
                    else if (r["tipodepago"].ToString() == "Otros Ingresos")
                    {
                        if (r["Entradas"].ToString() != "" && r["Entradas"].ToString() != " ")
                        {
                            Oingresositxt.Text = Convert.ToDouble(r["Entradas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            Oingresositxt.Text = "0";
                        }
                        if (r["Salidas"].ToString() != "" && r["Salidas"].ToString() != " ")
                        {
                            OingresosVtxt.Text = Convert.ToDouble(r["Salidas"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            OingresosVtxt.Text = "0";
                        }
                        if (r["Total"].ToString() != "" && r["Total"].ToString() != " ")
                        {
                            OingresosTtxt.Text = Convert.ToDouble(r["Total"].ToString()).ToString("#,0.00");
                        }
                        else
                        {
                            OingresosTtxt.Text = "0";
                        }
                    }
                }
            }
            else
            {
                efectivoitxt.Text = "0";
                efectivoVtxt.Text = "0";
                efectivoTtxt.Text = "0";
                POVitxt.Text = "0";
                POVVtxt.Text = "0";
                POVTtxt.Text = "0";
                OingresosTtxt.Text = "0";
                OingresosVtxt.Text = "0";
                Oingresositxt.Text = "0";
                OrdenesTtxt.Text = "0";
                OrdenesVtxt.Text = "0";
                Ordenesitxt.Text = "0";
                otrosTtxt.Text = "0";
                otrosVtxt.Text = "0";
                otrositxt.Text = "0";
                eurositxt.Text = "0";
                eurosVtxt.Text = "0";
                eurosTtxt.Text = "0";
                pesosTtxt.Text = "0";
                pesosVtxt.Text = "0";
                pesositxt.Text = "0";
                DolarVtxt.Text = "0";
                DolarTtxt.Text = "0";
                Dolaritxt.Text = "0";
                transferenciasTtxt.Text = "0";
                transferenciasitxt.Text = "0";
                transferenciasVtxt.Text = "0";
                PAGOMovilTtxt.Text = "0";
                PAGOMovilVtxt.Text = "0";
                PAGOMovilItxt.Text = "0";

            }
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            DataSet Empresa = new DataSet();
            Empresa = Conexion.SelectEmpresaActiva();
            // Conexion.RealizarCierre( efectivoitxt.Text,efectivoVtxt.Text,efectivoTtxt.Text,POVitxt.Text,POVVtxt.Text,POVTtxt.Text,PAGOMovilTtxt.Text,PAGOMovilVtxt.Text, PAGOMovilItxt.Text, transferenciasTtxt.Text, transferenciasitxt.Text,transferenciasVtxt.Text,DolarVtxt.Text,DolarTtxt.Text,Dolaritxt.Text,pesosTtxt.Text,pesosVtxt.Text,pesositxt.Text,eurositxt.Text,eurosVtxt.Text,eurosTtxt.Text,otrosTtxt.Text,otrosVtxt.Text,otrositxt.Text,OrdenesTtxt.Text,OrdenesVtxt.Text,Ordenesitxt.Text,OingresosTtxt.Text,OingresosVtxt.Text,Oingresositxt.Text,Nombre.Text,textBox1.Text,textBox2.Text);
            try
            {
                SLDocument sl = new SLDocument();

                SLStyle style = sl.CreateStyle();
                // each indent is 3 spaces, so this is 15 spaces total
                style.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.LeftToRight;
                style.Alignment.ShrinkToFit = true;
                style.Font.FontSize = 12;
                style.Font.FontName = "Calibri";

                style.Alignment.WrapText = true;
                SLStyle style2 = sl.CreateStyle();
                style2.Alignment.ReadingOrder = SLAlignmentReadingOrderValues.LeftToRight;
                style2.Alignment.ShrinkToFit = true;
                style2.Font.FontSize = 12;
                style.Font.FontName = "Calibri";
                style2.Alignment.WrapText = true;
                style2.Font.Bold = true;

               
                    sl.SetCellValue(1, 1, "DETALLADO");
                    sl.SetCellStyle(1, 1, style2);
                    sl.SetCellValue(1, 2, "INGRESO");
                    sl.SetCellStyle(1, 2, style2);
                    sl.SetCellValue(1, 3, "VUELTOS");
                    sl.SetCellStyle(1, 3, style2);
                    sl.SetCellValue(1, 4, "TOTAL");
                    sl.SetCellStyle(1, 4, style2);
                    sl.SetCellValue(1, 5, "SEDE:");
                    sl.SetCellStyle(1, 5, style2);
                   


                sl.SetCellValue(2, 1, "EFECTIVO");
                sl.SetCellStyle(2, 1, style2);
                sl.SetCellValue(3, 1, "PUNTO DE VENTA");
                sl.SetCellStyle(3, 1, style2);
                sl.SetCellValue(4, 1, "PAGO MOVIL");
                sl.SetCellStyle(4, 1, style2);
                sl.SetCellValue(5, 1, "TRANSFERENCIAS");
                sl.SetCellStyle(5, 1, style2);
                sl.SetCellValue(6, 1, "DOLARES");
                sl.SetCellStyle(6, 1, style2);
                sl.SetCellValue(7, 1, "PESOS");
                sl.SetCellStyle(7, 1, style2);
                sl.SetCellValue(8, 1, "EUROS");
                sl.SetCellStyle(8, 1, style2);
                sl.SetCellValue(9, 1, "OTROS");
                sl.SetCellStyle(9, 1, style2);
                sl.SetCellValue(10, 1, "ORDENES");
                sl.SetCellStyle(10, 1, style2);
                sl.SetCellValue(11, 1, "OTROS INGRESOS");
                sl.SetCellStyle(11, 1, style2);

                int i = 2;
                    sl.SetCellValue(i, 2, efectivoitxt.Text);
                    sl.SetCellValue(i, 3, efectivoVtxt.Text);
                    sl.SetCellValue(i, 4, efectivoTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, POVitxt.Text);
                    sl.SetCellValue(i, 3, POVVtxt.Text);
                    sl.SetCellValue(i, 4, POVTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, PAGOMovilItxt.Text);
                    sl.SetCellValue(i, 3, PAGOMovilVtxt.Text);
                    sl.SetCellValue(i, 4, PAGOMovilTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, transferenciasitxt.Text);
                    sl.SetCellValue(i, 3, transferenciasVtxt.Text);
                    sl.SetCellValue(i, 4, transferenciasTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, Dolaritxt.Text);
                    sl.SetCellValue(i, 3, DolarVtxt.Text);
                    sl.SetCellValue(i, 4, DolarTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, pesositxt.Text);
                    sl.SetCellValue(i, 3, pesosTtxt.Text);
                    sl.SetCellValue(i, 4, pesosVtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, eurositxt.Text);
                    sl.SetCellValue(i, 3, eurosVtxt.Text);
                    sl.SetCellValue(i, 4, eurosTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, otrositxt.Text);
                    sl.SetCellValue(i, 3, otrosVtxt.Text);
                    sl.SetCellValue(i, 4, otrosTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, Ordenesitxt.Text);
                    sl.SetCellValue(i, 3, OrdenesVtxt.Text);
                    sl.SetCellValue(i, 4, OrdenesTtxt.Text);
                   i++;
                    sl.SetCellValue(i, 2, Oingresositxt.Text);
                    sl.SetCellValue(i, 3, OingresosVtxt.Text);
                    sl.SetCellValue(i, 4, OingresosTtxt.Text);
                   i++;
                sl.SetCellValue(i, 2, "TOTAL FACTURADO");
                sl.SetCellStyle(i, 2, style2);
                sl.SetCellValue(i, 3, "TOTAL COBRADO");
                sl.SetCellStyle(i, 3, style2);
                sl.SetCellValue(i, 4, "DIFERENCIA");
                sl.SetCellStyle(i, 4, style2);
                i++;
                    sl.SetCellValue(i, 2, Facturado.Text);
                    sl.SetCellValue(i, 3, cobrado.Text);
                    sl.SetCellValue(i, 4, diferencia.Text);
                i++;
                sl.SetCellValue(i, 2, Empresa.Tables[0].Rows[0]["Nombre"].ToString());
                sl.SetCellStyle(i, 2, style2);
                sl.SetCellValue(i, 3, "FECHA: ");
                sl.SetCellStyle(i, 3, style2);
                sl.SetCellValue(i, 4, DateTime.Now.ToString("dd/MM/yyyy"));
                sl.SetCellStyle(i, 4, style2);

                saveFileDialog1.Filter = "Excel|*.xlsx";


                    sl.SetColumnWidth(1, 23);
                    sl.SetColumnWidth(2, 25);
                    sl.SetColumnWidth(3, 23);
                    sl.SetColumnWidth(4, 23);
                    if (saveFileDialog1.ShowDialog() == DialogResult.OK)
                    {
                        sl.SaveAs(saveFileDialog1.FileName);
                    }
                }
            catch (Exception ex)
            {
                MessageBox.Show("Selecciono el nombre de un archivo que posiblemente este en uso, por favor cierre el archivo o cambie el nombre");
            }
    }

        private void cobrado_TextChanged(object sender, EventArgs e)
        {

        }
    }

}
