using Conexiones;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DocumentFormat.OpenXml.Drawing.Charts;
using Conexiones.DbConnect;
using Conexiones.Dto;

namespace Laboratorio
{
    public partial class ModificarPerfilCompuesto : Form
    {
        List<Servidores> Server = new List<Servidores>();
        List<Task> Tareas = new List<Task>();
        List<Perfil> perfiles = new List<Perfil>();
        Perfil perfil = new Perfil();

        List<AnalisisLaboratorio> ListadeAnalisis = new List<AnalisisLaboratorio>();

        List<AnalisisLaboratorio> ListadeAnalisisAModificar = new List<AnalisisLaboratorio>();
        private void DatosDeServidores()
        {
            Server.Add(new Servidores() { idServer = 0, iPServer = "RIVANA" });
            Server.Add(new Servidores() { idServer = 1, iPServer = "ARCOS PARADA" });
            Server.Add(new Servidores() { idServer = 2, iPServer = "HOSPITALAB" });
            Server.Add(new Servidores() { idServer = 3, iPServer = "NARDO" });
            Server.Add(new Servidores() { idServer = 4, iPServer = "ARO" });
            Server.Add(new Servidores() { idServer = 9, iPServer = "ARCOS PARADA 2" });
            Server.Add(new Servidores() { idServer = 5, iPServer = "ESPECIALES" });
        }
        private void insertarDatos()
        {
            if (string.IsNullOrEmpty(TNombrePerfil.Text))
            {
                MessageBox.Show("Debe Ingresar el Nombre del perfil");
                return;
            }
            if (string.IsNullOrEmpty(TPrecioDolar.Text))
            {
                MessageBox.Show("Debe Ingresar un monto");
                return;
            }
            perfil.NombrePerfil = TNombrePerfil.Text;
            perfil.Precio = PrecioBs.Text.Replace(",", ".");
            perfil.PrecioDolar = Convert.ToDouble(TPrecioDolar.Text);
            if (checkActivo.Checked == true)
            {
                perfil.Activo = 1;
            }
            else
            {
                perfil.Activo = 0;
            }

            int respuesta = Conexion.ModificarPerfilCompuesto(perfil);
            if (respuesta > 0)
            {
                MessageBox.Show("Agregado Satisfactoriamente");
            }
            else
            {
                MessageBox.Show("Ha ocurrido un Error");
            }

        }
        public ModificarPerfilCompuesto()
        {
            InitializeComponent();
            DatosDeServidores();
            
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ModificarPerfilCompuesto_Load(object sender, EventArgs e)
        {
            dgvPerfiles.DataSource = perfiles = Conexion.selectListaDePerfiles();
            dgvAnalisis.DataSource = ListadeAnalisis = Conexion.selectListaAnalisis();
        }

        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }
        private void ActualizarPerfil()
        {

            perfil.NombrePerfil = TNombrePerfil.Text;
            perfil.Precio = PrecioBs.Text.Replace(",", ".");
            perfil.PrecioDolar = Convert.ToDouble(TPrecioDolar.Text);
            if (checkActivo.Checked == true)
            {
                perfil.Activo = 1;
            }
            else
            {
                perfil.Activo = 0;
            }
           int i= Conexion.ActualizarPerfil(perfil);
            if (i == 1)
            {
                MessageBox.Show("Actualizado Satisfactoriamente");
            }
            else
            {
                MessageBox.Show("Ha ocurrido un error, por favor comuniquese con sistemas");
            }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            dgvAnalisis.DataSource = new List<AnalisisLaboratorio>();
            dgvAnalisis.DataSource = ListadeAnalisis.Where(x => x.NombreAnalisis.Contains(TBuscarAnalisis.Text) || x.IdAnalisis.Equals(TBuscarAnalisis.Text)).ToList();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            dgvPerfiles.DataSource = new List<Perfiles>();
            dgvPerfiles.DataSource = perfiles.Where(x => x.NombrePerfil.Contains(textBox1.Text) || x.IdPerfil.Equals(textBox1.Text)).ToList();
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            if (dgvPerfiles.Rows.Count > 0)
            {
                CargarPerfil();
            }
        }

        private void CargarPerfil()
        {
            int IdPerfil = 0;
            int index = dgvPerfiles.CurrentCell.RowIndex;
            if (index < 0)
            {
                MessageBox.Show("Por Favor seleccione una columna en la lista de la izquierda");
                return;
            }

            //Seleccionando Perfil
            int.TryParse(dgvPerfiles.Rows[index].Cells["IdPerfil"].Value.ToString(), out IdPerfil);
            perfil = perfiles.Where(x => x.IdPerfil == IdPerfil).FirstOrDefault();
            //Asignando Variables a visualizar

            TNombrePerfil.Text = perfil.NombrePerfil;
            PrecioBs.Text = perfil.Precio;
            TPrecioDolar.Text = perfil.PrecioDolar.ToString();
            if (perfil.Activo == 1)
            {
                checkActivo.Checked = true;
            }
            else
            {
                checkActivo.Checked = false;
            }

            List<AnalisisLaboratorio> list = new List<AnalisisLaboratorio>();
            list = Conexion.selectAnalisisAgrupadosPorPerfil(perfil.IdPerfil);
            list.Except(perfil.analisisLaboratorios);
            perfil.analisisLaboratorios.AddRange(list);
            dgvAnalisisAsignados.DataSource = new List<AnalisisLaboratorio>();
            dgvAnalisisAsignados.DataSource = perfil.analisisLaboratorios.OrderBy(o => o.idOrganizador).ToList();
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            if (dgvAnalisisAsignados.Rows.Count > 0)
            {
                int index = dgvAnalisisAsignados.CurrentCell.RowIndex;
                if (index < 0)
                {
                    MessageBox.Show("Por favor seleccione un analisis");
                    return;
                }
                int IdAnalisis = 0;
                int.TryParse(dgvAnalisisAsignados.Rows[index].Cells["IdAnalisis"].Value.ToString(), out IdAnalisis);
                var AnalisisSeleccionado = perfil.analisisLaboratorios.Where(x=> x.IdAnalisis== IdAnalisis).FirstOrDefault();
                if (!(AnalisisSeleccionado is null))
                {
                    perfil.analisisLaboratorios.Remove(AnalisisSeleccionado);
                    dgvAnalisisAsignados.DataSource = new List<AnalisisLaboratorio>();
                    dgvAnalisisAsignados.DataSource = perfil.analisisLaboratorios.OrderBy(o => o.idOrganizador).ToList();
                }
            }

        }

        private void tBuscarAnalisisAsignados_TextChanged(object sender, EventArgs e)
        {
            dgvAnalisisAsignados.DataSource = new List<AnalisisLaboratorio>();
            dgvAnalisisAsignados.DataSource= perfil.analisisLaboratorios.Where(x => x.NombreAnalisis.Contains(tBuscarAnalisisAsignados.Text) || x.IdAnalisis.Equals(tBuscarAnalisisAsignados.Text)).ToList();
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            if (dgvAnalisis.Rows.Count > 0)
            {
                int index = dgvAnalisis.CurrentCell.RowIndex;
         
                if (index < 0)
                {
                    MessageBox.Show("Por favor seleccione un analisis");
                    return;
                }

                int idAnalisis = 0;
                int.TryParse(dgvAnalisis.Rows[index].Cells["idAnalisisDataGridViewTextBoxColumn"].Value.ToString(),out idAnalisis);
                var AnalisisSeleccionado = ListadeAnalisis.Where(a => a.IdAnalisis == idAnalisis).First();
                if (!perfil.analisisLaboratorios.Exists(x => x.IdAnalisis == AnalisisSeleccionado.IdAnalisis))
                {
               
                        switch (AnalisisSeleccionado.IdAnalisis)
                        {
                        case 12:
                            AnalisisSeleccionado.idOrganizador = 2;
                            break;
                        case 42:
                            AnalisisSeleccionado.idOrganizador = 3;
                            break;
                        case 55:
                                AnalisisSeleccionado.idOrganizador = 1;
                                break;
                        case 203:
                            AnalisisSeleccionado.idOrganizador = 4;
                            break;
                        default:
                                AnalisisSeleccionado.idOrganizador += 1;
                                break;
                        }
                    perfil.analisisLaboratorios.Add(AnalisisSeleccionado);
                    dgvAnalisisAsignados.DataSource = new List<AnalisisLaboratorio>();
                    dgvAnalisisAsignados.DataSource = perfil.analisisLaboratorios.OrderBy(o => o.idOrganizador).ToList();
                }
                else
                {
                    MessageBox.Show("Examen ya agregado");
                }

            }
           
        }
        private async Task<string> ConexionAlServer(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                insertarDatos();
            }
            return results;
        }
        private async Task<string> ActualizarSoloPerfil(string cmd)
        {
            string results = await Conexion.AsyncTestConnection(cmd);
            if (results == "Conectado")
            {
                ActualizarPerfil();
            }
            return results;
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
                        insertarDatos();
                    }
                }
            }
            this.Close();

        }

        private void iconButton7_Click(object sender, EventArgs e)
        {
            int Contador = dgvAnalisisAsignados.Rows.Count;
            int Index = dgvAnalisisAsignados.CurrentCell.RowIndex;
            if(Contador != 0)
            {
                if(Index < 0)
                {
                    MessageBox.Show("Por favor seleccione un analisis");
                    return;
                }
                else
                {
                    if (dgvAnalisisAsignados.Rows[Index] == dgvAnalisisAsignados.Rows[0])
                    {
                        MessageBox.Show("El Analisis seleccionado ya esta en el primer lugar");
                        return;
                    }
                    else
                    {
                        int rowS, RowTM,idAnalisisS,idAnalisisTM;
                        int.TryParse(dgvAnalisisAsignados.Rows[Index].Cells["IdOrganizador"].Value.ToString(), out rowS);
                        int.TryParse(dgvAnalisisAsignados.Rows[Index-1].Cells["IdOrganizador"].Value.ToString(), out RowTM);
                        int.TryParse(dgvAnalisisAsignados.Rows[Index].Cells["IdAnalisis"].Value.ToString(), out idAnalisisS);
                        int.TryParse(dgvAnalisisAsignados.Rows[Index - 1].Cells["IdAnalisis"].Value.ToString(), out idAnalisisTM);
                        DataGridViewRow rowSelected, rowToModified = new DataGridViewRow();
                        rowSelected = dgvAnalisisAsignados.Rows[Index];
                        rowToModified = dgvAnalisisAsignados.Rows[Index - 1];
                        perfil.analisisLaboratorios.Where(x => x.IdAnalisis == idAnalisisS).FirstOrDefault().idOrganizador = RowTM;
                        perfil.analisisLaboratorios.Where(x => x.IdAnalisis == idAnalisisTM).FirstOrDefault().idOrganizador = rowS;
                        dgvAnalisisAsignados.DataSource = new List<AnalisisLaboratorio>();
                        dgvAnalisisAsignados.DataSource = perfil.analisisLaboratorios.OrderBy(o => o.idOrganizador).ToList();
                    }
                }
            }
        }

        private void checkActivo_CheckedChanged(object sender, EventArgs e)
        {
            if(perfil.IdPerfil <= 0)
            {
                MessageBox.Show("Debe seleccionar un perfil primero");
                return;
            }

            if (checkActivo.Checked == true)
            {
                perfil.Activo = 1;
            }
            else
            {
                perfil.Activo = 0;
            }
        }

        private void iconButton6_Click(object sender, EventArgs e)
        {
            int Contador = dgvAnalisisAsignados.Rows.Count;
            int Index = dgvAnalisisAsignados.CurrentCell.RowIndex;
            if (Contador != 0)
            {
                if (Index < 0)
                {
                    MessageBox.Show("Por favor seleccione un analisis");
                    return;
                }
                else
                {
                    if (dgvAnalisisAsignados.Rows[Index] == dgvAnalisisAsignados.Rows[Contador - 1])
                    {
                        MessageBox.Show("El Analisis seleccionado ya esta en el ultimo lugar");
                        return;
                    }
                    else
                    {
                        int rowS, RowTM, idAnalisisS, idAnalisisTM;
                        int.TryParse(dgvAnalisisAsignados.Rows[Index].Cells["IdOrganizador"].Value.ToString(), out rowS);
                        int.TryParse(dgvAnalisisAsignados.Rows[Index + 1].Cells["IdOrganizador"].Value.ToString(), out RowTM);
                        int.TryParse(dgvAnalisisAsignados.Rows[Index].Cells["IdAnalisis"].Value.ToString(), out idAnalisisS);
                        int.TryParse(dgvAnalisisAsignados.Rows[Index + 1].Cells["IdAnalisis"].Value.ToString(), out idAnalisisTM);
                        DataGridViewRow rowSelected, rowToModified = new DataGridViewRow();
                        rowSelected = dgvAnalisisAsignados.Rows[Index];
                        rowToModified = dgvAnalisisAsignados.Rows[Index + 1];
                        perfil.analisisLaboratorios.Where(x => x.IdAnalisis == idAnalisisS).FirstOrDefault().idOrganizador = RowTM;
                        perfil.analisisLaboratorios.Where(x => x.IdAnalisis == idAnalisisTM).FirstOrDefault().idOrganizador = rowS;
                        dgvAnalisisAsignados.DataSource = new List<AnalisisLaboratorio>();
                        dgvAnalisisAsignados.DataSource = perfil.analisisLaboratorios.OrderBy(o => o.idOrganizador).ToList();
                    }
                }
            }
        }

        private void iconButton1_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(TNombrePerfil.Text))
            {
                MessageBox.Show("Debe Ingresar el Nombre del perfil");
                return;
            }
            if (string.IsNullOrEmpty(TPrecioDolar.Text))
            {
                MessageBox.Show("Debe Ingresar un monto");
                return;
            }
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
                            Tareas.Add(ActualizarSoloPerfil(items.iPServer));
                        }
                        Task t = Task.WhenAll(Tareas);
                    }
                    else
                    {
                        ActualizarPerfil();
                    }
                }
            }
            
        }

        private void TPrecioDolar_TextChanged(object sender, EventArgs e)
        {

        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }
    }
}
