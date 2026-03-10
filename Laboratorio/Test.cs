using Microsoft.Reporting.WinForms;
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

namespace Laboratorio
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }

        private void Test_Load(object sender, EventArgs e)
        {
            
            DataSet empresa = new DataSet();
            empresa = Conexion.SelectEmpresaActiva();
            DataSet Paciente = new DataSet();
            Paciente = Conexion.SELECTPersona("24556681");
            ReportDataSource rds1 = new ReportDataSource("Empresa", empresa.Tables[0]);
            ReportDataSource rds2 = new ReportDataSource("DataSet1", Paciente.Tables[0]);
            this.reportViewer1.ProcessingMode = ProcessingMode.Local;
            reportViewer1.LocalReport.EnableExternalImages = true;
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(rds1);
            this.reportViewer1.LocalReport.DataSources.Add(rds2);
            this.reportViewer1.RefreshReport();
        }
    }
}
