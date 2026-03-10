using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Conexiones;
using Conexiones.DbConnect;

namespace Laboratorio
{
    public partial class Usuario : Form
    {
        Usuarios usuario = new Usuarios();
        List<Servidores> Server = new List<Servidores>();
        List<Task> Tareas = new List<Task>();
        Bitmap destImage;
        string STRImprimirResultado, STRReimprimirResultado, STRValidar, STRModificar;
        string STRAgregarConvenio, STRQuitarConvenio, STRVerLibroVenta, STRVerCierreCaja;
        string STRVerOrdenes, STRVerEstadisticas, STRVerReporteBioanalista;
        string STRVerReferidos, STRImprimirFactura, STRReImprimirFactura, STRAgregarUsuario;
        string STRModificarUsuario, STRCambioDePrecios, STRImprimirEtiqueta, STRTeclasHematologia;
        string STRAgregarAnalisis, STRModificarAnalisis, STREliminarAnalisis;
        string STRModificarOrden, STRAnularOrden;

        int IdUser, IdUsuario;


        private void ValidarResultado_CheckedChanged(object sender, EventArgs e)
        {
            if (ValidarResultado.Checked == true)
            {
                STRValidar = "1";
            }
            else
            {
                STRValidar = "0";
            }
        }

        private void ModificarResultado_CheckedChanged(object sender, EventArgs e)
        {
            if (ModificarResultado.Checked == true)
            {
                STRModificar = "1";
            }
            else
            {
                STRModificar = "0";
            }
        }

        private void AgregarConvenio_CheckedChanged(object sender, EventArgs e)
        {
            if (AgregarConvenio.Checked == true)
            {
                STRAgregarConvenio = "1";
            }
            else
            {
                STRAgregarConvenio = "0";
            }
        }

        private void QuitarConvenio_CheckedChanged(object sender, EventArgs e)
        {
            if (QuitarConvenio.Checked == true)
            {
                STRQuitarConvenio = "1";
            }
            else
            {
                STRQuitarConvenio = "0";
            }
        }

        private void VerLibroVenta_CheckedChanged(object sender, EventArgs e)
        {
            if (VerLibroVenta.Checked == true)
            {
                STRVerLibroVenta = "1";
            }
            else
            {
                STRVerLibroVenta = "0";
            }
        }

        private void VerCierredeCaja_CheckedChanged(object sender, EventArgs e)
        {
            if (VerCierredeCaja.Checked == true)
            {
                STRVerCierreCaja = "1";
            }
            else
            {
                STRVerCierreCaja = "0";
            }

        }

        private void VerOrdenes_CheckedChanged(object sender, EventArgs e)
        {
            if (VerOrdenes.Checked == true)
            {
                STRVerOrdenes = "1";
            }
            else
            {
                STRVerOrdenes = "0";
            }
        }

        private void VerEstadisticas_CheckedChanged(object sender, EventArgs e)
        {
            if (VerEstadisticas.Checked == true)
            {
                STRVerEstadisticas = "1";
            }
            else
            {
                STRVerEstadisticas = "0";
            }
        }

        private void ReImprimirFactura_CheckedChanged(object sender, EventArgs e)
        {
            if (ImprimirFactura.Checked == true)
            {
                STRReImprimirFactura = "1";
            }
            else
            {
                STRReImprimirFactura = "0";
            }
        }

        private void AgregarUsuario_CheckedChanged(object sender, EventArgs e)
        {
            if (AgregarUsuario.Checked == true)
            {
                STRAgregarUsuario = "1";
            }
            else
            {
                STRAgregarUsuario = "0";
            }
        }

        private void ModificarUsuario_CheckedChanged(object sender, EventArgs e)
        {
            if (ModificarUsuario.Checked == true)
            {
                STRModificarUsuario = "1";
            }
            else
            {
                STRModificarUsuario = "0";
            }
        }

        private void CambioDePrecios_CheckedChanged(object sender, EventArgs e)
        {
            if (CambioDePrecios.Checked == true)
            {
                STRCambioDePrecios = "1";
            }
            else
            {
                STRCambioDePrecios = "0";
            }
        }

        private void ImprimirEtiqueta_CheckedChanged(object sender, EventArgs e)
        {
            if (ImprimirEtiqueta.Checked == true)
            {
                STRImprimirEtiqueta = "1";
            }
            else
            {
                STRImprimirEtiqueta = "0";
            }
        }

        private void iconButton2_Click(object sender, EventArgs e)
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
        private void BtnGuardar()
        {
         
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["ModificarUsuario"].ToString() == "1")
            {
                string cmd = string.Format("Privilegios.ImprimirResultado = '{0}', Privilegios.ReimprimirResultado = '{1}', Privilegios.Validar = '{2}', Privilegios.Modificar = '{3}', Privilegios.AgregarConvenio = '{4}', Privilegios.QuitarConvenio = '{5}', Privilegios.VerLibroVenta = '{6}', Privilegios.VerCierreCaja = '{7}', Privilegios.VerOrdenes = '{8}', Privilegios.VerEstadisticas = '{9}', Privilegios.VerReporteBioanalista = '{10}', Privilegios.VerReferidos = '{11}', Privilegios.ImprimirFactura = '{12}', Privilegios.ReImprimirFactura = '{13}', Privilegios.AgregarUsuario = '{14}', Privilegios.ModificarUsuario = '{15}', Privilegios.CambioDePrecios = '{16}', Privilegios.ImprimirEtiqueta = '{17}',Privilegios.TeclasHematologia = '{18}',Privilegios.AgregarAnalisis = '{19}',Privilegios.ModificarAnalisis = '{20}',Privilegios.EliminarAnalisis = '{21}',Privilegios.ModificarOrden = '{22}',Privilegios.AnularOrden = '{23}'", STRImprimirResultado, STRReimprimirResultado, STRValidar, STRModificar, STRAgregarConvenio, STRQuitarConvenio, STRVerLibroVenta, STRVerCierreCaja, STRVerOrdenes, STRVerEstadisticas, STRVerReporteBioanalista, STRVerReferidos, STRImprimirFactura, STRReImprimirFactura, STRAgregarUsuario, STRModificarUsuario, STRCambioDePrecios, STRImprimirEtiqueta, STRTeclasHematologia, STRAgregarAnalisis, STRModificarAnalisis, STREliminarAnalisis, STRModificarOrden, STRAnularOrden);
                string MS = Conexion.ActualizarPrivilegios(cmd, IdUsuario.ToString());
                MessageBox.Show(MS);
                int ValorActivo = 0;
                if (Activo.Checked)
                {
                    ValorActivo = 1;
                }
                cmd = string.Format("Activo = {0}, NombreUsuario = '{1}', Contraseña = '{2}', CB = '{3}',MPPS = '{4}',SIGLAS = '{4}'", ValorActivo,NombreUser.Text,Password.Text,CB.Text,MPPS.Text,SIGLAS.Text);
                MS = Conexion.ActualizarUsuario(cmd, IdUsuario.ToString());
                Actualizar();
            }
        }

        private void iconButton8_Click(object sender, EventArgs e)
        {
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["QuitarConvenio"].ToString() == "1")
            {
                string IdConvenio = dataGridView2.Rows[dataGridView2.CurrentCell.RowIndex].Cells["IdConvenio"].Value.ToString();
                string MS = Conexion.Borrarconvenios(IdConvenio, IdUsuario.ToString());
                MessageBox.Show(MS);
                Actualizar();
            } else
            {
                MessageBox.Show("No tienes permisos para realizar esta accion");
            }
        }

        private void VerReporte_CheckedChanged(object sender, EventArgs e)
        {
            if (VerReporte.Checked == true)
            {
                STRVerReporteBioanalista = "1";
            }
            else
            {
                STRVerReporteBioanalista = "0";
            }
        }

        private void TeclasHematologia_CheckedChanged(object sender, EventArgs e)
        {
            if (TeclasHematologia.Checked == true)
            {
                STRTeclasHematologia = "1";
            }
            else
            {
                STRTeclasHematologia = "0";
            }
        }

        private void AgregarAnalisis_CheckedChanged(object sender, EventArgs e)
        {
            if (AgregarAnalisis.Checked == true)
            {
                STRAgregarAnalisis = "1";
            }
            else
            {
                STRAgregarAnalisis = "0";
            }
        }

        private void ModificarAnalisis_CheckedChanged(object sender, EventArgs e)
        {
            if (ModificarAnalisis.Checked == true)
            {
                STRModificarAnalisis = "1";
            }
            else
            {
                STRModificarAnalisis = "0";
            }
        }

        private void ModificarOrden_CheckedChanged(object sender, EventArgs e)
        {
            if (ModificarOrden.Checked == true)
            {
                STRModificarOrden = "1";
            }
            else
            {
                STRModificarOrden = "0";
            }
        }
        public class Servidores
        {
            public string iPServer { get; set; }
            public int idServer { get; set; }
            public string estado { get; set; }
        }
        private void SeleccionarServer(int Server)
        {

            if (Server == 0)
            {
                Conexion.Connection(ConfigurationManager.ConnectionStrings["RIVANA"].ConnectionString, "Rivana");
            } //RIVANA
            else if (Server == 1) //ArcosParada
            {
                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARCOS PARADA"].ConnectionString, "ARCOS PARADA");

            }//ArcosParada
            else if (Server == 2)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["HOSPITALAB"].ConnectionString, "HOSPITALAB");

            }//Hospitalab
            else if (Server == 3)
            {
                Conexion.Connection(ConfigurationManager.ConnectionStrings["NARDO"].ConnectionString, "NARDO");
            }//Nardo
            else if (Server == 4)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARO"].ConnectionString, "ARO");


            }//Aro
            else if (Server == 5)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["ESPECIALES"].ConnectionString, "ESPECIALES");

            }
            else if (Server == 6)
            {

                Conexion.Connection(ConfigurationManager.ConnectionStrings["ARCOS PARADA 2"].ConnectionString, "ARCOS PARADA 2");

            }

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
        private async Task<string> ConexionAlServer(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                BtnGuardar();
            }
            return results;
        }
        private async Task<string> AgregRConvenios(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                AgregarConvenios();
            }
            return results;
        }
        private async Task<string> BorrarConvenios(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                borrarConveniso();
            }
            return results;
        }

        private void TrashConvenio_Click(object sender, EventArgs e)
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
                            Tareas.Add(BorrarConvenios(items.iPServer));
                        }
                        Task t = Task.WhenAll(Tareas);
                    }
                    else
                    {
                        borrarConveniso();
                    }
                }
            }
        }
        private void borrarConveniso()
        {
            if (dataGridView2.SelectedRows.Count > 0)
            {
                foreach (DataGridViewRow rows in dataGridView2.SelectedRows)
                {
                    Conexion.BorrarConveniosPorUsuario((int)rows.Cells["IdConvenio"].Value, IdUsuario);
                }
            }
            MessageBox.Show("Eliminado Satisfactoriamente");
        }

        private void AnularOrden_CheckedChanged(object sender, EventArgs e)
        {
            if (AnularOrden.Checked == true)
            {
                STRAnularOrden = "1";
            }
            else
            {
                STRAnularOrden = "0";
            }
        }

        private void dataGridView1_CellContentClick_1(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void EliminarAnalisis_CheckedChanged(object sender, EventArgs e)
        {
            if (EliminarAnalisis.Checked == true)
            {
                STREliminarAnalisis = "1";
            }
            else
            {
                STREliminarAnalisis = "0";
            }
        }

        private void iconButton3_Click(object sender, EventArgs e)
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

        private void VerReferidos_CheckedChanged(object sender, EventArgs e)
        {
            if (VerReferidos.Checked == true)
            {
                STRVerReferidos = "1";
            }
            else
            {
                STRVerReferidos = "0";
            }
        }

        private void ImprimirFactura_CheckedChanged(object sender, EventArgs e)
        {
            if (ImprimirFactura.Checked == true)
            {
                STRImprimirFactura = "1";
            }
            else
            {
                STRImprimirFactura = "0";
            }
        }

        private void ReImpResultado_CheckedChanged(object sender, EventArgs e)
        {
            if (ReImpResultado.Checked == true)
            {
                STRReimprimirResultado = "1";
            }
            else
            {
                STRReimprimirResultado = "0";
            }
        }

        public Usuario(int idUser,int idUsuario)
        {
            IdUser = idUser;
            IdUsuario = idUsuario;
            InitializeComponent();
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void Form30_Load(object sender, EventArgs e)
        {
            DatosDeServidores();
            CargarUsuario();
            AgregandoCargos();
            CargandoConvenios();
            ConveniosPorUsuario();
            CargarPrivilegiosUsuario();
        }
        private void AgregandoCargos()
        {
            DataSet ds = new DataSet();
            ds = Conexion.SelectCargo();
            foreach (DataRow r in ds.Tables[0].Rows)
            {
                comboBox2.Items.Add(r["idCargo"].ToString());
                comboBox1.Items.Add(r["NombreCargo"].ToString());
            }
        }
        private void CargandoConvenios()
        {
            DataSet ds = new DataSet();
            ds = Conexion.conveniosUser();
            if (ds.Tables.Count != 0)
            {
                dataGridView1.DataSource = ds.Tables[0];
                DataGridViewColumn column = dataGridView1.Columns[0];
                column.Width = 30;
                DataGridViewColumn column1 = dataGridView1.Columns[1];
                column1.Width = 280;
                DataGridViewColumn column2 = dataGridView1.Columns[2];
                column2.Width = 50;
            }
        }
        private void ConveniosPorUsuario()
        {
            DataSet ds1 = new DataSet();
            ds1 = Conexion.convenios(IdUsuario.ToString());
            if (ds1.Tables.Count != 0)
            {
                dataGridView2.DataSource = ds1.Tables[0];
                DataGridViewColumn column = dataGridView2.Columns[0];
                column.Width = 30;
                DataGridViewColumn column1 = dataGridView2.Columns[1];
                column1.Width = 280;
                DataGridViewColumn column2 = dataGridView2.Columns[2];
                column2.Width = 50;
            }
        }
        private void CargarPrivilegiosUsuario()
        {


            DataSet ds2 = new DataSet();
            ds2 = Conexion.PrivilegiosCargar(IdUsuario.ToString());
            try
            {
                if (ds2.Tables[0].Rows[0]["ImprimirResultado"].ToString() == "1")
                {
                    ImpResultado.Checked = true;
                    STRImprimirResultado = "1";
                }
                else
                {
                    ImpResultado.Checked = false;
                    STRImprimirResultado = "0";
                }
                if (ds2.Tables[0].Rows[0]["ReimprimirResultado"].ToString() == "1")
                {
                    STRReimprimirResultado = "1";
                    ReImpResultado.Checked = true;
                }
                else
                {
                    STRReimprimirResultado = "0";
                    ReImpResultado.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["Validar"].ToString() == "1")
                {
                    STRValidar = "1";
                    ValidarResultado.Checked = true;
                }
                else
                {
                    STRValidar = "0";
                    ValidarResultado.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["Modificar"].ToString() == "1")
                {
                    STRModificar = "1";
                    ModificarResultado.Checked = true;
                }
                else
                {
                    STRModificar = "0";
                    ModificarResultado.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["AgregarConvenio"].ToString() == "1")
                {
                    STRAgregarConvenio = "1";
                    AgregarConvenio.Checked = true;
                }
                else
                {
                    STRAgregarConvenio = "0";
                    AgregarConvenio.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["QuitarConvenio"].ToString() == "1")
                {
                    STRQuitarConvenio = "1";
                    QuitarConvenio.Checked = true;
                }
                else
                {
                    STRQuitarConvenio = "1";
                    QuitarConvenio.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["VerLibroVenta"].ToString() == "1")
                {
                    STRVerLibroVenta = "1";
                    VerLibroVenta.Checked = true;
                }
                else
                {
                    STRVerLibroVenta = "0";
                    VerLibroVenta.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["VerCierreCaja"].ToString() == "1")
                {
                    STRVerCierreCaja = "1";
                    VerCierredeCaja.Checked = true;
                }
                else
                {
                    STRVerCierreCaja = "0";
                    VerCierredeCaja.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["VerOrdenes"].ToString() == "1")
                {
                    STRVerOrdenes = "1";
                    VerOrdenes.Checked = true;
                }
                else
                {
                    STRVerOrdenes = "0";
                    VerOrdenes.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["VerEstadisticas"].ToString() == "1")
                {
                    STRVerEstadisticas = "1";
                    VerEstadisticas.Checked = true;
                }
                else
                {
                    STRVerEstadisticas = "0";
                    VerEstadisticas.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["VerReporteBioanalista"].ToString() == "1")
                {
                    STRVerReporteBioanalista = "1";
                    VerReporte.Checked = true;
                }
                else
                {
                    STRVerReporteBioanalista = "0";
                    VerReporte.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["VerReferidos"].ToString() == "1")
                {
                    STRVerReferidos = "1";
                    VerReferidos.Checked = true;
                }
                else
                {
                    STRVerReferidos = "0";
                    VerReferidos.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["ImprimirFactura"].ToString() == "1")
                {
                    STRImprimirFactura = "1";
                    ImprimirFactura.Checked = true;
                }
                else
                {
                    STRImprimirFactura = "0";
                    ImprimirFactura.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["ReImprimirFactura"].ToString() == "1")
                {
                    STRReImprimirFactura = "1";
                    ReImprimirFactura.Checked = true;
                }
                else
                {
                    STRReImprimirFactura = "0";
                    ReImprimirFactura.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["AgregarUsuario"].ToString() == "1")
                {
                    STRAgregarUsuario = "1";
                    AgregarUsuario.Checked = true;
                }
                else
                {
                    STRAgregarUsuario = "0";
                    AgregarUsuario.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["ModificarUsuario"].ToString() == "1")
                {
                    STRModificar = "1";
                    ModificarUsuario.Checked = true;
                }
                else
                {
                    STRModificar = "0";
                    ModificarUsuario.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["CambioDePrecios"].ToString() == "1")
                {
                    STRCambioDePrecios = "1";
                    CambioDePrecios.Checked = true;
                }
                else
                {
                    STRCambioDePrecios = "0";
                    CambioDePrecios.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["ImprimirEtiqueta"].ToString() == "1")
                {
                    STRImprimirEtiqueta = "1";
                    ImprimirEtiqueta.Checked = true;
                }
                else
                {
                    STRImprimirEtiqueta = "0";
                    ImprimirEtiqueta.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["TeclasHematologia"].ToString() == "1")
                {
                    STRTeclasHematologia = "1";
                    TeclasHematologia.Checked = true;
                }
                else
                {
                    STRTeclasHematologia = "0";
                    TeclasHematologia.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["AgregarAnalisis"].ToString() == "1")
                {
                    STRAgregarAnalisis = "1";
                    AgregarAnalisis.Checked = true;
                }
                else
                {
                    STRAgregarAnalisis = "0";
                    AgregarAnalisis.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["ModificarAnalisis"].ToString() == "1")
                {
                    STRModificarAnalisis = "1";
                    ModificarAnalisis.Checked = true;
                }
                else
                {
                    STRModificarAnalisis = "0";
                    ModificarAnalisis.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["EliminarAnalisis"].ToString() == "1")
                {
                    STREliminarAnalisis = "1";
                    EliminarAnalisis.Checked = true;
                }
                else
                {
                    STREliminarAnalisis = "0";
                    EliminarAnalisis.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["ModificarOrden"].ToString() == "1")
                {
                    STRModificarOrden = "1";
                    ModificarOrden.Checked = true;
                }
                else
                {
                    STRModificarOrden = "0";
                    ModificarOrden.Checked = false;
                }
                if (ds2.Tables[0].Rows[0]["AnularOrden"].ToString() == "1")
                {
                    STRAnularOrden = "1";
                    AnularOrden.Checked = true;
                }
                else
                {
                    STRAnularOrden = "0";
                    AnularOrden.Checked = false;
                }

            }
            catch (Exception)
            {

               
            }
         
          
        }

        private void CargarUsuario()
        {
            DataSet ds3 = new DataSet();
            ds3 = Conexion.CargarUsuario(IdUsuario.ToString());
            NombreUser.Text = ds3.Tables[0].Rows[0]["NombreUsuario"].ToString();
            Password.Text = ds3.Tables[0].Rows[0]["Contraseña"].ToString();
            CB.Text = ds3.Tables[0].Rows[0]["CB"].ToString();
            MPPS.Text = ds3.Tables[0].Rows[0]["MPPS"].ToString();
            SIGLAS.Text = ds3.Tables[0].Rows[0]["SIGLAS"].ToString();
            if (ds3.Tables[0].Rows[0]["Firma"].ToString() != "")
            {
                Image img = Image.FromFile(string.Format("Firma\\{0}", ds3.Tables[0].Rows[0]["Firma"].ToString()));
                CambiarTamaño(img, 80, 50);
            }
            if (string.IsNullOrEmpty(ds3.Tables[0].Rows[0]["Activo"].ToString()) || ds3.Tables[0].Rows[0]["Activo"].ToString() == "0")
            {
                Activo.Checked = false;
            }
            else
            {
                Activo.Checked = true;
            }
        }

        private void Actualizar()
        {
          

        }
        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void UsuarioLabel_Click(object sender, EventArgs e)
        {
            
        }

        private void iconButton7_Click(object sender, EventArgs e)
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
                            Tareas.Add(AgregRConvenios(items.iPServer));
                        }
                        Task t = Task.WhenAll(Tareas);
                    }
                    else
                    {
                        AgregarConvenios();
                    }
                }
            }

        }
        private void AgregarConvenios()
        {
            bool Encontrado = false;
            DataSet Permisos = new DataSet();
            Permisos = Conexion.PrivilegiosCargar(IdUser.ToString());
            if (Permisos.Tables[0].Rows[0]["AgregarConvenio"].ToString() == "1")
            {
                foreach (DataGridViewRow r in dataGridView1.Rows)
                {
                    if (r.Cells["IdConvenio"].ToString() == dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdConvenio"].Value.ToString())
                    {
                        Encontrado = true;
                    }
                }
                if (!Encontrado)
                {
                    DataSet ds = new DataSet();
                    ds = Conexion.ConvenioActivo(dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdConvenio"].Value.ToString());
                    if (ds.Tables[0].Rows[0]["Activos"].ToString() == "1")
                    {
                        string cmd = string.Format("{0},{1}", dataGridView1.Rows[dataGridView1.CurrentCell.RowIndex].Cells["IdConvenio"].Value.ToString(), IdUsuario.ToString());
                        string MS = Conexion.InsertarConvenioUsuario(cmd);
                        MessageBox.Show(MS);
                        Actualizar();
                    }
                    else
                    {
                        MessageBox.Show("Este convenio ha sido desactivado");
                    }
                }
            }
            else
            {
                MessageBox.Show("No tienes permisos para realizar esta accion");
            }
 

        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ImpResultado_CheckedChanged(object sender, EventArgs e)
        {
            if (ImpResultado.Checked == true)
            {
                STRImprimirResultado = "1";
            }
            else 
            {
                STRImprimirResultado = "0";
            }
        }
    }
}
