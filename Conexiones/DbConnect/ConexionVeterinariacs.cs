using Conexiones.Dto;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Wordprocessing;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Conexiones.DbConnect
{
    public class ConexionVeterinaria
    {
        private static string connection = "Server =localhost;Database=Veterinaria; Port =3306; Uid=root; Pwd =Labordono1818; Connection Timeout = 120; Allow User Variables=True";
        private static MySqlDataAdapter adapter = new MySqlDataAdapter();
        private static MySqlCommand command = new MySqlCommand();

        public bool activo, proceso1 = false, proceso2 = false, proceso3 = false;
        public static void Connection(string cmd, string Nombre)
        {
            string password = "Labordono1818";
            string uid = "root";
            string server = "localhost";
            string database = "Veterinaria";
            MySqlConnection conn = new MySqlConnection();
            server = ConfigurationManager.ConnectionStrings[Nombre].ConnectionString;
            string cmd2 = string.Format("Puerto{0}", Nombre);
            string puerto = ConfigurationManager.ConnectionStrings[cmd2].ConnectionString;
            connection = string.Format("Server={0}; Database={1}; Port={4}; Uid={2}; Pwd={3};Connection Timeout=120;", server, database, uid, password, puerto);
            conn = new MySqlConnection(connection);
        }
        public static string TestConnection()
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {

                con.Open();
                return "Conectado";
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return "Desconectado";
            }
            finally
            {
                con.Close();
            }

        }
        public static async Task<string> AsyncTestConnection(string cmd)
        {

            Connection(cmd, cmd);
            MySqlConnection con = new MySqlConnection(connection);
            string estado = string.Empty;
            try
            {
                await con.OpenAsync();
                return "Conectado";
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return "Desconectado";
            }
            finally
            {
                con.Close();
            }

        }

        public static DataTable AnalisisAignados(int idSesion)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select IdUsuario,NombreUsuario FROM usuarios Where contraseña = '{0}'", idSesion), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public static Ordenes selectOrden(int idOrden)
        {
            Ordenes Orden = new Ordenes();
            Orden = selectOrdenPorId(idOrden);
            Orden.datosRepresentante = selectDatosRepresentantePorId(Orden.IdDatosRepresentante);
            Orden.datosPacienteVet = datosPacientePorId(Orden.IdDatosPaciente);
            Orden.convenio = selectConvenioPorID(Orden.IdConvenio);
            Orden.ResultadosAnalisis = selectResultadosAnalisisPorOrden(Orden.idOrden);
            Orden.usuarios = selectUsuarioPorId(Orden.IdUsuario);
            Orden.datosPacienteVet.especies.hemoParasitos = selectHemoparasitosPorOrden(Orden.idOrden);
            return Orden;
        }
        public static Ordenes selectOrden(int idOrden, int IdAnalisis, int IdAgrupador)
        {
            Ordenes Orden = new Ordenes();
            Orden = selectOrdenPorId(idOrden);
            Orden.datosRepresentante = selectDatosRepresentantePorId(Orden.IdDatosRepresentante);
            Orden.datosPacienteVet = datosPacientePorId(Orden.IdDatosPaciente);
            Orden.convenio = selectConvenioPorID(Orden.IdConvenio);
            Orden.ResultadosAnalisis = selectResultadosAnalisisPorOrden(Orden.idOrden, IdAnalisis, IdAgrupador);
            Orden.usuarios = selectUsuarioPorId(Orden.IdUsuario);
            Orden.datosPacienteVet.especies.hemoParasitos = selectHemoparasitosPorOrden(Orden.idOrden);
            return Orden;
        }
        public static List<Ordenes> selectOrdenesPorIdSesion(int IdSesion)
        {
            List<Ordenes> ordenes = new List<Ordenes>();
            ordenes = selectOrdenPorIdSesion(IdSesion);
            foreach (Ordenes orden in ordenes)
            {
                orden.datosRepresentante = selectDatosRepresentantePorId(orden.IdDatosRepresentante);
                orden.datosPacienteVet = datosPacientePorId(orden.IdDatosPaciente);
                orden.convenio = selectConvenioPorID(orden.IdConvenio);
                orden.ResultadosAnalisis = selectResultadosAnalisisPorOrden(orden.idOrden);
                orden.usuarios = selectUsuarioPorId(orden.IdUsuario);
                orden.datosPacienteVet.especies.hemoParasitos = selectHemoparasitosPorOrden(orden.idOrden);
            }


            return ordenes;
        }
        public static Ordenes selectOrdenPorId(int idOrden)
        {

            Ordenes Orden = new Ordenes();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.ordenes where IdOrden = {0};", idOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        Orden.idOrden = (int)ds.Tables[0].Rows[0]["idOrden"];
                        Orden.IdDatosPaciente = (int)ds.Tables[0].Rows[0]["IdDatosPaciente"];
                        Orden.IdDatosRepresentante = (int)ds.Tables[0].Rows[0]["IdDatosRepresentante"];
                        Orden.IdConvenio = (int)ds.Tables[0].Rows[0]["IdConvenio"];
                        Orden.IdTasa = (int)ds.Tables[0].Rows[0]["idTasa"];
                        Orden.IdUsuario = (int)ds.Tables[0].Rows[0]["IdUsuario"];
                        Orden.IDEstadoDeOrden = (int)ds.Tables[0].Rows[0]["IDEstadoDeOrden"];
                        Orden.PrecioF = ds.Tables[0].Rows[0]["PrecioF"].ToString();
                        Orden.NumeroDia = (int)ds.Tables[0].Rows[0]["NumeroDia"];
                        Orden.Fecha = (DateTime)ds.Tables[0].Rows[0]["Fecha"];
                        Orden.Hora = ds.Tables[0].Rows[0]["Hora"].ToString();
                        Orden.IDSesion = (int)ds.Tables[0].Rows[0]["IDSesion"];
                    }
                }
                return Orden;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Orden;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Ordenes> selectOrdenPorIdSesion(int IdSesion)
        {
            List<Ordenes> Ordenes = new List<Ordenes>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.ordenes where IdSesion = {0};", IdSesion), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow R in ds.Tables[0].Rows)
                        {
                            Ordenes Orden = new Ordenes();
                            Orden.idOrden = (int)R["idOrden"];
                            Orden.IdDatosPaciente = (int)R["IdDatosPaciente"];
                            Orden.IdDatosRepresentante = (int)R["IdDatosRepresentante"];
                            Orden.IdConvenio = (int)R["IdConvenio"];
                            Orden.IdTasa = (int)R["idTasa"];
                            Orden.IdUsuario = (int)R["IdUsuario"];
                            Orden.IDEstadoDeOrden = (int)R["IDEstadoDeOrden"];
                            Orden.PrecioF = R["PrecioF"].ToString();
                            Orden.NumeroDia = (int)R["NumeroDia"];
                            Orden.Fecha = (DateTime)R["Fecha"];
                            Orden.Hora = R["Hora"].ToString();
                            Orden.IDSesion = (int)R["IDSesion"];
                            Ordenes.Add(Orden);
                        }

                    }
                }
                return Ordenes;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Ordenes;
            }
            finally
            {
                con.Close();
            }
        }
        public static Facturas selectFacturaPorId(int IdSesion)
        {
            int idConvenio = 0;
            int finca = 0;
            Facturas factura = new Facturas();
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {
                //LLENADO DE DATOS DE FACTURA
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.factura Where IdSesion = {0} and IdEstadoDeFactura < 3;", IdSesion), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
            }
            catch (Exception ex)
            {
                CrearEvento("HAS SELECCIONADO LA FACTURA Con Factura " + ds.Rows.Count);
                CrearEvento(ex.ToString());
                return factura;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

            }


            // LLENADO DE MODELOS QUE VAN CON LA FACTURA
            if (ds.Rows.Count > 0)
            {
                if (ds.Rows.Count > 0)
                {
                    factura.Id = (int)ds.Rows[0]["idFactura"];
                    factura.IdSesion = (int)ds.Rows[0]["IdSesion"];
                    int.TryParse(ds.Rows[0]["IdConvenio"].ToString(), out idConvenio);
                    int.TryParse(ds.Rows[0]["IdFinca"].ToString(), out finca);
                    factura.Fecha = ds.Rows[0]["Fecha"].ToString();
                    factura.IdConvenio = idConvenio;
                    factura.IdRepresentante = (int)ds.Rows[0]["IdRepresentante"];
                    CrearEvento("Empieza el select de Representantes");
                    factura.datosRepresentante = selectDatosRepresentantePorId(factura.IdRepresentante);
                    CrearEvento("Empieza el select de Representantes");
                    factura.datoveterinario = selectVeterinario((int)ds.Rows[0]["IdVeterinario"]);
                    factura.finca = selectFinca(finca);
                    factura.NumeroDia = (int)ds.Rows[0]["NumeroDia"];
                    CrearEvento("Empieza el select de Ordenes");
                    factura.Ordenes = selectOrdenesPorIdSesion(IdSesion);
                }
            }
            return factura;


        }
        public static mayoromenorreferencial selectValoresPorId(int IDAnalisis, int IdEspecie)
        {
            mayoromenorreferencial Valores = new mayoromenorreferencial();
            MySqlConnection con = new MySqlConnection(connection);
            int idEspecie = 0, idAnalisis = 0, tipoAnalisis = 0;
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM veterinaria.mayoromenorreferencial Where IdEspecie = {IdEspecie}  and IDAnalisis = {IDAnalisis};", con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int.TryParse(ds.Tables[0].Rows[0]["IdEspecie"].ToString(), out idEspecie);
                        int.TryParse(ds.Tables[0].Rows[0]["IdAnalisis"].ToString(), out idAnalisis);
                        int.TryParse(ds.Tables[0].Rows[0]["TipoAnalisis"].ToString(), out tipoAnalisis);
                        Valores.IdEspecie = idEspecie;
                        Valores.IdAnalisis = idAnalisis;
                        Valores.ValorMenor = ds.Tables[0].Rows[0]["ValorMenor"].ToString();
                        Valores.ValorMayor = ds.Tables[0].Rows[0]["ValorMayor"].ToString();
                        Valores.Unidad = ds.Tables[0].Rows[0]["Unidad"].ToString();
                        Valores.MultiplesValores = ds.Tables[0].Rows[0]["MultiplesValores"].ToString();
                        Valores.TipoAnalisis = tipoAnalisis;
                    }
                }
                return Valores;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Valores;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }

            }
        }
        public static List<int> EspeciesPorSesion(int IdSesion)
        {
            List<int> Especies = new List<int>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT IdEspecie FROM veterinaria.ordenes left join DatosPaciente on DatosPaciente.IdDatosPaciente = Ordenes.IdDatosPaciente Where IdSesion = {IdSesion} group by IdSesion,IdEspecie order by IdEspecie asc", con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Especies.Add((int)r["IdEspecie"]);
                        }
                    }
                }
                return Especies;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Especies;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static List<Especies> EspeciesModelPorSesion(int IdSesion)
        {
            List<Especies> Especies = new List<Especies>();
            Especies Especie = new Especies();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT Especies.IdEspecie,especies.Descripcion FROM veterinaria.ordenes join DatosPaciente on DatosPaciente.IdDatosPaciente = Ordenes.IdDatosPaciente join especies on DatosPaciente.IdEspecie =especies.IdEspecie  Where IdSesion = {IdSesion} group by IdSesion,IdEspecie order by IdEspecie asc", con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Especie.IdEspecie = (int)r["IdEspecie"];
                            Especie.Descripcion = r["Descripcion"].ToString();
                            Especies.Add(Especie);
                        }
                    }
                }
                return Especies;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Especies;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static List<AnalisisLaboratorio> SelectListaDeAnalisis()
        {
            List<AnalisisLaboratorio> analisisLaboratorios = new List<AnalisisLaboratorio>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM Veterinaria.AnalisisLaboratorio", con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            analisisLaboratorios.Add(new AnalisisLaboratorio
                            {
                                IdAnalisis = (int)r["IdAnalisis"],
                                NombreAnalisis = r["NombreAnalisis"].ToString(),
                                Modificable = (int)r["Modificable"]
                            });
                        }
                    }
                }
                return analisisLaboratorios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static List<AnalisisLaboratorio> SelectListaDeAnalisisGrupales(int IdSesion, int idImpresionAgrupada)
        {
            List<AnalisisLaboratorio> analisisLaboratorios = new List<AnalisisLaboratorio>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($@"
                SELECT 
                    PerfilesAnalisis.IdAnalisis
                FROM
                    `Veterinaria`.`PerfilesFacturados`
                        INNER JOIN
                    perfilesanalisis ON perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil
                        LEFT JOIN
                    perfil ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil
                WHERE
                     PerfilesFacturados.IdSesion = {IdSesion}
                    and
                    idImpresionAgrupada = {idImpresionAgrupada}
                GROUP BY idAnalisis", con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            analisisLaboratorios.Add(new AnalisisLaboratorio
                            {
                                IdAnalisis = (int)r["IdAnalisis"]
                            });
                        }
                    }
                }
                return analisisLaboratorios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static List<AnalisisLaboratorio> SelectListaAnalisisPorPerfil(int IdPerfil)
        {
            List<AnalisisLaboratorio> analisisLaboratorios = new List<AnalisisLaboratorio>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM veterinaria.selectanalisisporperfil Where IdPerfil = {IdPerfil}", con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            analisisLaboratorios.Add(new AnalisisLaboratorio
                            {
                                IdAnalisis = (int)r["IdAnalisis"],
                                NombreAnalisis = r["NombreAnalisis"].ToString(),
                                TipoAnalisis = (int)r["TipoAnalisis"],
                                Visible = (bool)r["Visible"],
                                Etiqueta = r["Etiqueta"].ToString(),
                                IdSeccion = (int)r["IdSeccion"],
                                Especiales = (int)r["Especiales"],
                            });
                        }
                    }
                }
                return analisisLaboratorios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static Usuarios selectUsuarioPorId(int idUsuario)
        {
            Usuarios usuario = new Usuarios();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Usuarios where IdUsuario = {0};", idUsuario), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        usuario.IdUsuario = (int)ds.Tables[0].Rows[0]["IdUsuario"];
                        usuario.NombreUsuario = ds.Tables[0].Rows[0]["NombreUsuario"].ToString();
                        usuario.CB = ds.Tables[0].Rows[0]["CB"].ToString();
                        usuario.MPPS = ds.Tables[0].Rows[0]["MPPS"].ToString();
                        usuario.SIGLAS = ds.Tables[0].Rows[0]["SIGLAS"].ToString();
                        int.TryParse(ds.Tables[0].Rows[0]["Cargo"].ToString(), out int Cargo);
                        usuario.Cargo = Cargo;
                        usuario.Firma = ds.Tables[0].Rows[0]["Firma"].ToString();
                        usuario.Activo = bool.Parse(ds.Tables[0].Rows[0]["Activo"].ToString());
                    }
                }
                return usuario;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return usuario;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static DataSet selectUsuarios()
        {
            Usuarios usuario = new Usuarios();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Usuarios "), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static List<Usuarios> seleccionarUsuarios()
        {
            List<Usuarios> usuarios = new List<Usuarios>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Usuarios"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();

                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach(DataRow r in ds.Tables[0].Rows)
                        {
                            Usuarios usuario = new Usuarios();
                            usuario.IdUsuario = (int)r["IdUsuario"];
                            usuario.NombreUsuario = r["NombreUsuario"].ToString();
                            usuario.CB = r["CB"].ToString();
                            usuario.MPPS = r["MPPS"].ToString();
                            usuario.SIGLAS = r["SIGLAS"].ToString();
                            int.TryParse(r["Cargo"].ToString(), out int Cargo);
                            usuario.Cargo = Cargo;
                            usuario.Firma = r["Firma"].ToString();
                            usuario.Activo = bool.Parse(r["Activo"].ToString());
                            usuarios.Add(usuario);
                        }


                    }
                }
                return usuarios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return usuarios;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }



        public static List<Hemo> selectHemoparasitosPorOrden(int IdOrden)
        {
            List<Hemo> hemo = new List<Hemo>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.resultadohemoparasito join hemoparasitos on hemoparasitos.IdHemoparasitos = resultadohemoparasito.IdHemoparasitos Where IdOrden = {0} group by hemoparasitos.IdHemoparasitos", IdOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            hemo.Add(new Hemo { Id = Convert.ToInt32(r["IdHemoparasitos"].ToString()), Resultado = r["Resultado"].ToString(), Descripcion = r["Descripcion"].ToString() });
                        }

                    }
                }
                return hemo;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return hemo;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static List<Hemo> selectHemoparasitosPorÉspecie(int IdEspecie)
        {
            List<Hemo> hemo = new List<Hemo>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.hemoparasitos where IdEspecie = {0};", IdEspecie), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            hemo.Add(new Hemo { Id = Convert.ToInt32(r["IdHemoparasitos"].ToString()), Descripcion = r["Descripcion"].ToString() });
                        }

                    }
                }
                return hemo;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return hemo;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }

        }
        public static List<Hemo> selectCoproPorÉspecie(int IdEspecie)
        {
            List<Hemo> hemo = new List<Hemo>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.hemoparasitos where IdEspecie = {0};", IdEspecie), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            hemo.Add(new Hemo { Id = Convert.ToInt32(r["IdHemoparasitos"].ToString()), Descripcion = r["Descripcion"].ToString() });
                        }

                    }
                }
                return hemo;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return hemo;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }

        public static List<Perfil> selectListaDePerfiles()
        {
            List<Perfil> perfiles = new List<Perfil>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM `Veterinaria`.`Perfil` WHERE `Activo` = 1; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                conn.Close();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Rows)
                        perfiles.Add(new Perfil
                        {
                            IdPerfil = (int)row["IdPerfil"],
                            NombrePerfil = row["NombrePerfil"].ToString(),
                            Precio = row["Precio"].ToString(),
                            PrecioDolar = Convert.ToDouble(row["PrecioDolar"].ToString()),
                            Activo = Convert.ToInt32(row["Activo"])
                        });
                }
                return perfiles;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return perfiles;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public static Perfil selectPerfil(int IdPerfil)
        {
            int IdAgrupado = 0;
            Perfil perfil = new Perfil();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM `Veterinaria`.`Perfil` WHERE `IdPerfil` = {IdPerfil}; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                conn.Close();

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return perfil;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            if (ds.Rows.Count > 0)
            {
                perfil.IdPerfil = (int)ds.Rows[0]["IdPerfil"];
                int.TryParse(ds.Rows[0]["idImpresionAgrupada"].ToString(), out IdAgrupado);
                perfil.IdImpresionAgrupado = IdAgrupado;
                perfil.NombrePerfil = ds.Rows[0]["NombrePerfil"].ToString();
                perfil.Precio = ds.Rows[0]["Precio"].ToString();
                perfil.PrecioDolar = Convert.ToDouble(ds.Rows[0]["PrecioDolar"].ToString());
                perfil.Activo = Convert.ToInt32(ds.Rows[0]["Activo"].ToString());
                perfil.analisisLaboratorios = selectAnalisisAgrupadosPorPerfil(perfil.IdPerfil);
            }
            return perfil;

        }
        public static List<Perfil> selectPerfilesPorSesion(int IdSesion)
        {
            int IdAgrupado = 0;
            List<Perfil> perfiles = new List<Perfil>();
          
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM veterinaria.perfilesafacturar join Perfil on Perfil.IdPerfil = perfilesafacturar.IdPerfil WHERE `IdSesion` = {IdSesion}; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                conn.Close();

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

            if (ds.Rows.Count > 0)
            {
                foreach (DataRow r in ds.Rows)
                {
                    Perfil perfil = new Perfil();
                    int.TryParse(r["IdPerfil"].ToString(), out int idPerfil);
                    perfil.IdPerfil = idPerfil;

                    int.TryParse(r["idImpresionAgrupada"].ToString(), out IdAgrupado);
                    perfil.IdImpresionAgrupado = IdAgrupado;
                    perfil.NombrePerfil = r["NombrePerfil"].ToString();
                    perfil.Precio = r["Precio"].ToString();

                    Double.TryParse(r["IdPerfil"].ToString(), out Double PrecioDolar);
                    perfil.PrecioDolar = PrecioDolar;

                    int.TryParse(r["Activo"].ToString(), out int Activo);
                    perfil.Activo = Activo;
                    perfil.analisisLaboratorios = selectAnalisisAgrupadosPorPerfil(perfil.IdPerfil);
                    perfiles.Add(perfil);
                }
              
            }
            return perfiles;

        }
        public static List<AnalisisLaboratorio> selectAnalisisAgrupadosPorPerfil(int idPerfil)
        {
            List<AnalisisLaboratorio> analisisLaboratorios = new List<AnalisisLaboratorio>();

            MySqlConnection conn = new MySqlConnection(connection);
            int IdAnalisis = 0, TipoAnalisis = 0, idOrganizador = 0, IdSeccion = 0, Especiales = 0, Titulo = 0, IdAgrupador = 0, FinalTitulo = 0;
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * from perfilesAnalisis inner join analisislaboratorio on PerfilesAnalisis.IdAnalisis = AnalisisLaboratorio.idAnalisis Where IdPerfil = {idPerfil} group by analisislaboratorio.IdAnalisis", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Rows)
                    {
                        AnalisisLaboratorio analisis = new AnalisisLaboratorio();
                        int.TryParse(row["IdAnalisis"].ToString(), out IdAnalisis);
                        int.TryParse(row["TipoAnalisis"].ToString(), out TipoAnalisis);
                        int.TryParse(row["IdSeccion"].ToString(), out IdSeccion);
                        int.TryParse(row["Especiales"].ToString(), out Especiales);
                        int.TryParse(row["Titulo"].ToString(), out Titulo);
                        int.TryParse(row["IdAgrupador"].ToString(), out IdAgrupador);
                        int.TryParse(row["FinalTitulo"].ToString(), out FinalTitulo);
                        int.TryParse(row["idOrganizador"].ToString(), out idOrganizador);
                        analisis.IdAnalisis = IdAnalisis;
                        analisis.NombreAnalisis = row["NombreAnalisis"].ToString();
                        analisis.TipoAnalisis = TipoAnalisis;
                        analisis.Visible = (bool)row["Visible"];
                        analisis.Etiqueta = row["Etiqueta"].ToString();
                        analisis.IdSeccion = IdSeccion;
                        analisis.Especiales = Especiales;
                        analisis.Titulo = Titulo;
                        analisis.IdAgrupador = IdAgrupador;
                        analisis.FinalTitulo = FinalTitulo;
                        analisis.idOrganizador = idOrganizador;
                        analisisLaboratorios.Add(analisis);
                    }
                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<ResultadosPorAnalisisVet> selectAnalisisAgrupadosPorPerfilySesion(int idPerfil, int IdSesion)
        {
            List<ResultadosPorAnalisisVet> analisisLaboratorios = new List<ResultadosPorAnalisisVet>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`,`resultadospaciente`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`resultadospaciente`.`IdUsuario`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`,`resultadospaciente`.`HoraIngreso`,`resultadospaciente`.`IDorganizador`,`resultadospaciente`.`Lineas` FROM `Veterinaria`.`PerfilesFacturados` inner join Ordenes on ordenes.IdSesion = perfilesfacturados.IdSesion left join perfilesanalisis on perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil left join resultadospaciente on resultadospaciente.IdAnalisis = perfilesanalisis.idAnalisis and Ordenes.IdOrden = resultadospaciente.IdOrden WHERE `PerfilesFacturados`.`IdPerfil` = {idPerfil} and `PerfilesFacturados`.`IdSesion` = {IdSesion} group by IdOrden,IdAnalisis order by idOrden; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Rows)
                    {
                        ResultadosPorAnalisisVet resultados = new ResultadosPorAnalisisVet();
                        resultados.IdOrden = (int)row["IdOrden"];
                        resultados.IdAnalisis = (int)row["IdAnalisis"];
                        resultados.ValorResultado = row["IdAnalisis"].ToString();
                        resultados.ValorMayor = (double)row["ValorMayor"];
                        resultados.ValorMenor = (double)row["ValorMenor"];
                        resultados.MultiplesValores = row["MultiplesValores"].ToString();
                        resultados.IdEstadoDeResultado = (int)row["IdEstadoDeResultado"];
                        resultados.IdOrganizador = (int)row["IdOrganizador"];
                        resultados.analisisLaboratorio = selectAnalisisPorID(resultados.IdAnalisis);
                        analisisLaboratorios.Add(resultados);

                    }

                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }
        public static Empresa selectEmpresaActiva()
        {
            Empresa empresa = new Empresa();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM `Veterinaria`.`empresas` WHERE `Empresas`.`Activa` = 1; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    empresa.IdEmpresa = (int)ds.Rows[0]["IdEmpresa"];
                    empresa.Nombre = ds.Rows[0]["Nombre"].ToString();
                    empresa.Rif = ds.Rows[0]["Rif"].ToString();
                    empresa.Ruta = ds.Rows[0]["Ruta"].ToString();
                    empresa.Sede = ds.Rows[0]["Sede"].ToString();
                    empresa.Correo = ds.Rows[0]["Correo"].ToString();
                    empresa.Clave = ds.Rows[0]["Clave"].ToString();
                    empresa.Puerto = ds.Rows[0]["Puerto"].ToString();
                    empresa.Direccion = ds.Rows[0]["Direccion"].ToString();
                    empresa.Telefono = ds.Rows[0]["Telefono"].ToString();
                }

                return empresa;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return empresa;
            }
            finally
            {
                conn.Close();
            }
        }
        public static Empresa selectEmpresaActiva(int IdConvenio)
        {
            Empresa empresa = new Empresa();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM `Veterinaria`.`empresas` left join ConveniosEmpresas on ConveniosEmpresas.IdEmpresa = Empresas.IdEmpresa WHERE `ConveniosEmpresas`.`IdConvenio` = {IdConvenio}; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    empresa.IdEmpresa = (int)ds.Rows[0]["IdEmpresa"];
                    empresa.Nombre = ds.Rows[0]["Nombre"].ToString();
                    empresa.Rif = ds.Rows[0]["Rif"].ToString();
                    empresa.Ruta = ds.Rows[0]["Ruta"].ToString();
                    empresa.Sede = ds.Rows[0]["Sede"].ToString();
                    empresa.Correo = ds.Rows[0]["Correo"].ToString();
                    empresa.Clave = ds.Rows[0]["Clave"].ToString();
                    empresa.Puerto = ds.Rows[0]["Puerto"].ToString();
                    empresa.Direccion = ds.Rows[0]["Direccion"].ToString();
                    empresa.Telefono = ds.Rows[0]["Telefono"].ToString();
                }

                return empresa;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return empresa;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<ResultadosPorAnalisisVet> selectResultadosAnalisisPorOrden(int idOrden)
        {
            List<ResultadosPorAnalisisVet> ResultadosPorAnalisisVet = new List<ResultadosPorAnalisisVet>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.resultadospaciente Where IdOrden = {0} order by IdAnalisis", idOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                            int LineasR = 1;
                            double VaLorMayorR = 0;
                            double ValorMenorR = 0;
                            int IdEstadoDeResultado = 0;
                            int IdUsuario = 0;
                            double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                            double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                            int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                            int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                            int.TryParse(r["Lineas"].ToString(), out LineasR);
                            Resultado.IdAnalisis = (int)r["IdAnalisis"];
                            Resultado.IdOrden = (int)r["IdOrden"];
                            Resultado.ValorResultado = r["ValorResultado"].ToString();
                            Resultado.unidad = r["unidad"].ToString();
                            Resultado.ValorMenor = ValorMenorR;
                            Resultado.ValorMayor = VaLorMayorR;
                            Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                            Resultado.Comentario = r["Comentario"].ToString();
                            Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                            Resultado.IdUsuario = IdUsuario;
                            Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                            Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                            Resultado.IdOrganizador = (int)r["IdOrganizador"];
                            if (IdUsuario > 0)
                            {
                                Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                            }

                            Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                            Resultado.Lineas = LineasR;
                            ResultadosPorAnalisisVet.Add(Resultado);
                        }
                    }
                }
                return ResultadosPorAnalisisVet;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ResultadosPorAnalisisVet;
            }
            finally
            {
                con.Close();
            }

        }
        public static List<ResultadosPorAnalisisVet> selectResultadosAnalisisPorOrden(int idOrden, int IdAnalisis, int IdAgrupador)
        {
            List<ResultadosPorAnalisisVet> ResultadosPorAnalisisVet = new List<ResultadosPorAnalisisVet>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                string comando = $"SELECT * FROM Veterinaria.resultadospaciente left join Analisislaboratorio on AnalisisLaboratorio.IdAnalisis = resultadospaciente.IdAnalisis Where IdOrden = {idOrden} ";
                if (IdAgrupador == 0)
                {
                    comando += $" IdAnalisis ={IdAnalisis}";
                }
                else
                {
                    comando += $" and idAgrupador ={IdAgrupador}";
                }


                con.Open();
                adapter.SelectCommand = new MySqlCommand(comando, con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                            int LineasR = 1;
                            double VaLorMayorR = 0;
                            double ValorMenorR = 0;
                            int IdEstadoDeResultado = 0;
                            int IdUsuario = 0;
                            double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                            double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                            int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                            int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                            int.TryParse(r["Lineas"].ToString(), out LineasR);
                            Resultado.IdAnalisis = (int)r["IdAnalisis"];
                            Resultado.IdOrden = (int)r["IdOrden"];
                            Resultado.ValorResultado = r["ValorResultado"].ToString();
                            Resultado.unidad = r["unidad"].ToString();
                            Resultado.ValorMenor = ValorMenorR;
                            Resultado.ValorMayor = VaLorMayorR;
                            Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                            Resultado.Comentario = r["Comentario"].ToString();
                            Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                            Resultado.IdUsuario = IdUsuario;
                            Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                            Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                            Resultado.IdOrganizador = (int)r["IdOrganizador"];
                            if (IdUsuario > 0)
                            {
                                Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                            }

                            Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                            Resultado.Lineas = LineasR;
                            ResultadosPorAnalisisVet.Add(Resultado);
                        }
                    }
                }
                return ResultadosPorAnalisisVet;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ResultadosPorAnalisisVet;
            }
            finally
            {
                con.Close();
            }

        }
        public static ResultadosPorAnalisisVet selectResultadoAnalisisPorOrden(int idOrden, int IdAnalisis, int IdAgrupador)
        {

            MySqlConnection con = new MySqlConnection(connection);
            ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
            DataSet ds = new DataSet();
            try
            {
                string comando = $"SELECT * FROM Veterinaria.resultadospaciente left join Analisislaboratorio on AnalisisLaboratorio.IdAnalisis = resultadospaciente.IdAnalisis Where IdOrden = {idOrden} ";
                if (IdAgrupador == 0)
                {
                    comando += $" and resultadospaciente.IdAnalisis ={IdAnalisis}";
                }
                else
                {
                    comando += $" and idAgrupador ={IdAgrupador}";
                }


                con.Open();
                adapter.SelectCommand = new MySqlCommand(comando, con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {


                        int LineasR = 1;
                        double VaLorMayorR = 0;
                        double ValorMenorR = 0;
                        int IdEstadoDeResultado = 0;
                        int IdUsuario = 0;
                        double.TryParse(ds.Tables[0].Rows[0]["VaLorMayor"].ToString(), out VaLorMayorR);
                        double.TryParse(ds.Tables[0].Rows[0]["ValorMenor"].ToString(), out ValorMenorR);
                        int.TryParse(ds.Tables[0].Rows[0]["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                        int.TryParse(ds.Tables[0].Rows[0]["IdUsuario"].ToString(), out IdUsuario);

                        int.TryParse(ds.Tables[0].Rows[0]["Lineas"].ToString(), out LineasR);
                        Resultado.IdAnalisis = (int)ds.Tables[0].Rows[0]["IdAnalisis"];
                        Resultado.IdOrden = (int)ds.Tables[0].Rows[0]["IdOrden"];
                        Resultado.ValorResultado = ds.Tables[0].Rows[0]["ValorResultado"].ToString();
                        Resultado.unidad = ds.Tables[0].Rows[0]["unidad"].ToString();
                        Resultado.ValorMenor = ValorMenorR;
                        Resultado.ValorMayor = VaLorMayorR;
                        Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                        Resultado.Comentario = ds.Tables[0].Rows[0]["Comentario"].ToString();
                        Resultado.MultiplesValores = ds.Tables[0].Rows[0]["MultiplesValores"].ToString();
                        Resultado.IdUsuario = IdUsuario;
                        Resultado.FechaIngreso = Convert.ToDateTime(ds.Tables[0].Rows[0]["FechaIngreso"] is DBNull ? DateTime.Now : ds.Tables[0].Rows[0]["FechaIngreso"]);
                        Resultado.HoraIngreso = ds.Tables[0].Rows[0]["HoraIngreso"].ToString();
                        Resultado.IdOrganizador = (int)ds.Tables[0].Rows[0]["IdOrganizador"];
                        if (IdUsuario > 0)
                        {
                            Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                        }

                        Resultado.analisisLaboratorio = selectAnalisisPorID((int)ds.Tables[0].Rows[0]["IdAnalisis"]);
                        Resultado.Lineas = LineasR;

                    }
                }
                return Resultado;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Resultado;
            }
            finally
            {
                con.Close();
            }

        }
        public static string ActualizarEstadoDeResultado(int EstadoDeResultado, int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET idEstadoDeResultado={0} Where IdOrden = {1} And IDAnalisis  = {2} ", EstadoDeResultado, IDOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<Perfil> selectExamenesGrupales(int IdSesion)
        {
            CrearEvento("Numero de Sesion de Factura " + IdSesion);
            List<Perfil> Perfiles = new List<Perfil>();
            Perfil perfil = new Perfil();
            DataTable dt = new DataTable();
            MySqlConnection conn = new MySqlConnection(connection);
            CrearEvento(connection);
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT IdPerfil FROM veterinaria.perfilesfacturados where IdSesion = {0};", IdSesion), conn);
                adapter.Fill(dt);
                adapter.Dispose();
                conn.Dispose();
            }
            catch (Exception ex)
            {
                CrearEvento(connection);
                CrearEvento(ex.ToString());
                return Perfiles;
            }
            finally
            {
                conn.Close();
            }



            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    perfil = selectPerfil((int)dr["Idperfil"]);
                    Perfiles.Add(perfil);
                }

            }
            return Perfiles;

        }
        public static List<Perfil> selectExamenesGrupales(List<Perfil> perfilesABuscar)
        {
            var Perfiles = new List<Perfil>();
            foreach (var perfil in perfilesABuscar)
            {
                Perfil perfilDb = new Perfil();
                perfilDb = selectPerfil(perfil.IdPerfil);
                Perfiles.Add(perfil);
            }
            return Perfiles;

        }
        public static Convenios selectConvenioPorID(int IdConvenio)
        {
            Convenios convenios = new Convenios();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.convenios where IdConvenio = {0};", IdConvenio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        convenios.IdConvenio = (int)ds.Tables[0].Rows[0]["IdConvenio"];
                        convenios.Nombre = ds.Tables[0].Rows[0]["Nombre"].ToString();
                        convenios.Direccion = ds.Tables[0].Rows[0]["Direccion"].ToString();
                        convenios.Correo = ds.Tables[0].Rows[0]["Correo"].ToString();
                        convenios.Ruta = ds.Tables[0].Rows[0]["Ruta"].ToString();
                        convenios.Activos = (bool)ds.Tables[0].Rows[0]["Activos"];
                        convenios.Descuento = ds.Tables[0].Rows[0]["Descuento"].ToString();
                    }
                }
                return convenios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return convenios;
            }
            finally
            {
                con.Close();
            }

        }
        public static List<Convenios> selectConvenios()
        {
            List<Convenios> Lista = new List<Convenios>();
          
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.convenios"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Convenios convenios = new Convenios();
                            convenios.IdConvenio = (int)r["IdConvenio"];
                            convenios.Nombre = r["Nombre"].ToString();
                            convenios.Direccion = r["Direccion"].ToString();
                            convenios.Correo = r["Correo"].ToString();
                            convenios.Ruta = r["Ruta"].ToString();
                            convenios.Activos = (bool)r["Activos"];
                            convenios.Descuento = r["Descuento"].ToString();
                            Lista.Add(convenios);                        
                        }
                       
                    }
                }
                return Lista;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Lista;
            }
            finally
            {
                con.Close();
            }

        }
        public static List<Convenios> selectConvenioPorUsuario(int IdConvenio)
        {
            List<Convenios> Lista = new List<Convenios>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.conveniosporusuario join Convenios on Convenios.IdConvenio = conveniosporusuario.IdConvenio where IdUsuario = {0};", IdConvenio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Convenios convenios = new Convenios();
                            convenios.IdConvenio = (int)r["IdConvenio"];
                            convenios.Nombre = r["Nombre"].ToString();
                            convenios.Direccion = r["Direccion"].ToString();
                            convenios.Correo = r["Correo"].ToString();
                            convenios.Ruta = r["Ruta"].ToString();
                            convenios.Activos = (bool)r["Activos"];
                            convenios.Descuento = r["Descuento"].ToString();
                            Lista.Add(convenios);
                        }

                    }
                }
                return Lista;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Lista;
            }
            finally
            {
                con.Close();
            }

        }
        public static AnalisisLaboratorio selectAnalisisPorID(int idAnalisis)
        {
            AnalisisLaboratorio analisisLaboratorio = new AnalisisLaboratorio();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM `veterinaria`.`analisislaboratorio` left join mayoromenorreferencial on mayoromenorreferencial.idMayoryMenorReferencial = analisislaboratorio.IdAnalisis where analisislaboratorio.IdAnalisis = {0};", idAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int.TryParse(ds.Tables[0].Rows[0]["IdAnalisis"].ToString(), out int IdAnalisis);
                        int.TryParse(ds.Tables[0].Rows[0]["TipoAnalisis"].ToString(), out int TipoAnalisis);
                        int.TryParse(ds.Tables[0].Rows[0]["IdSeccion"].ToString(), out int IdSeccion);
                        int.TryParse(ds.Tables[0].Rows[0]["Especiales"].ToString(), out int Especiales);
                        int.TryParse(ds.Tables[0].Rows[0]["Titulo"].ToString(), out int Titulo);
                        int.TryParse(ds.Tables[0].Rows[0]["IdAgrupador"].ToString(), out int IdAgrupador);
                        int.TryParse(ds.Tables[0].Rows[0]["FinalTitulo"].ToString(), out int FinalTitulo);

                        analisisLaboratorio.IdAnalisis = IdAnalisis;
                        analisisLaboratorio.NombreAnalisis = ds.Tables[0].Rows[0]["NombreAnalisis"].ToString();
                        analisisLaboratorio.TipoAnalisis =TipoAnalisis;
                        analisisLaboratorio.Visible = Convert.ToBoolean(ds.Tables[0].Rows[0]["Visible"]);
                        analisisLaboratorio.IdSeccion = IdSeccion;
                        analisisLaboratorio.Especiales = Especiales;
                        analisisLaboratorio.Titulo = Titulo;
                        analisisLaboratorio.IdAgrupador = IdAgrupador;
                        analisisLaboratorio.FinalTitulo = FinalTitulo;

                    }
                }
                return analisisLaboratorio;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorio;
            }
            finally
            {
                con.Close();
            }

        }

        public static DatosPacienteVet datosPacientePorId(int IdPaciente)
        {
            DatosPacienteVet datosPaciente = new DatosPacienteVet();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.datospaciente where IdDatosPaciente = {0};", IdPaciente), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        datosPaciente.IdDatosPaciente = (int)ds.Tables[0].Rows[0]["IdDatosPaciente"];
                        datosPaciente.NombrePaciente = ds.Tables[0].Rows[0]["NombrePaciente"].ToString();
                        datosPaciente.IdEspecie = (int)ds.Tables[0].Rows[0]["IdEspecie"];
                        datosPaciente.Raza = ds.Tables[0].Rows[0]["Raza"].ToString();
                        datosPaciente.idDatosRepresentante = (int)ds.Tables[0].Rows[0]["idDatosRepresentante"];
                        datosPaciente.especies = selectEspeciePorId(datosPaciente.IdEspecie);
                    }
                }

                return datosPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return datosPaciente;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static DatosPacienteVet datosPacientePorOrden(int IdOrden)
        {
            DatosPacienteVet datosPaciente = new DatosPacienteVet();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.datospaciente join Ordenes on datospaciente.IdDatosPaciente = ordenes.IdDatosPaciente where ordenes.IdOrden = {0};", IdOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        datosPaciente.IdDatosPaciente = (int)ds.Tables[0].Rows[0]["IdDatosPaciente"];
                        datosPaciente.NombrePaciente = ds.Tables[0].Rows[0]["NombrePaciente"].ToString();
                        datosPaciente.IdEspecie = (int)ds.Tables[0].Rows[0]["IdEspecie"];
                        datosPaciente.Raza = ds.Tables[0].Rows[0]["Raza"].ToString();
                        datosPaciente.idDatosRepresentante = (int)ds.Tables[0].Rows[0]["idDatosRepresentante"];
                        datosPaciente.especies = selectEspeciePorId(datosPaciente.IdEspecie);
                    }
                }

                return datosPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return datosPaciente;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static DatosRepresentante selectDatosRepresentantePorId(int idRepresentante)
        {
            DatosRepresentante datosRepresentante = new DatosRepresentante();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.datosrepresentante where idDatosRepresentante = {0};", idRepresentante), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        datosRepresentante.idDatosRepresentante = (int)ds.Tables[0].Rows[0]["idDatosRepresentante"];
                        datosRepresentante.RIF = ds.Tables[0].Rows[0]["RIF"].ToString();
                        datosRepresentante.TipoRepresentante = ds.Tables[0].Rows[0]["TipoRepresentante"].ToString();
                        datosRepresentante.NombreRepresentante = ds.Tables[0].Rows[0]["NombreRepresentante"].ToString();
                        datosRepresentante.ApellidoRepresentante = ds.Tables[0].Rows[0]["ApellidoRepresentante"].ToString();
                        datosRepresentante.TipoCelular = ds.Tables[0].Rows[0]["TipoCelular"].ToString();
                        datosRepresentante.Celular = ds.Tables[0].Rows[0]["Celular"].ToString();
                        datosRepresentante.TipoTelefono = ds.Tables[0].Rows[0]["TipoTelefono"].ToString();
                        datosRepresentante.Telefono = ds.Tables[0].Rows[0]["Telefono"].ToString();
                        datosRepresentante.TipoCorreo = ds.Tables[0].Rows[0]["TipoCorreo"].ToString();
                        datosRepresentante.Correo = ds.Tables[0].Rows[0]["Correo"].ToString();
                    }
                }
                return datosRepresentante;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return datosRepresentante;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static PersonaOrden selectDatosRepresentantePorIdSesion(int IdSesion)
        {
            PersonaOrden datosRepresentante = new PersonaOrden();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.factura join datosrepresentante on perfilesfacturados.idDatosRepresentante = datosrepresentante.idDatosRepresentante where IdSesion = {0};", IdSesion), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        datosRepresentante.Mapear(ds.Tables[0].Rows[0]);
                    }
                }
           
                return datosRepresentante;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return datosRepresentante;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static List<PerfilesAFacturar> selectPerfilesAFacturar(int idSesion)
        {
            List<PerfilesAFacturar> perfilesAFacturar = new List<PerfilesAFacturar>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.perfilesafacturar where IdSesion = {0};", idSesion), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            perfilesAFacturar.Add(new PerfilesAFacturar
                            {
                                IdSesion = idSesion,
                                idPerfil = (int)r["idPerfil"],
                                Cantidad = r["Cantidad"].ToString(),
                                Precio = r["PrecioPerfil"].ToString(),
                                idRepresentante = (int)r["ID"],
                                perfil = selectPerfil((int)r["idPerfil"]),
                                datosRepresentante = selectDatosRepresentantePorId((int)r["ID"])
                            });
                        }
                    }
                }
                return perfilesAFacturar;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return perfilesAFacturar;
            }
            finally
            {
                con.Close();
            }
        }
        public static void CrearEvento(string cmd)
        {
            string MyLogName = DateTime.Now.ToString("ddMMyyyHHmmssfff");
            string sourceName = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string subPath = @"Errores\";
            bool exists = Directory.Exists(subPath);
            if (!exists)
            {
                Directory.CreateDirectory(subPath);
            }

            if (!exists)
            {
                using (StreamWriter outputFile = new StreamWriter(Path.Combine(subPath, sourceName)))
                {
                    outputFile.WriteLine(cmd, Environment.NewLine);
                }
            }
            else
            {
                using (StreamWriter outputFile = File.AppendText(Path.Combine(subPath, sourceName)))
                {
                    outputFile.WriteLine(cmd, Environment.NewLine);
                }
            }
        }
        public static int VerificarCorreo(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.resultadospaciente Where IdOrden = '{0}' and IdEstadoDeResultado > 1;", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables[0].Rows.Count == 0)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return 0;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Login(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select IdUsuario,NombreUsuario FROM usuarios Where contraseña = '{0}'", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet LoginCaptahuellas(int IdUsuario)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select IdUsuario,NombreUsuario FROM usuarios Where IdUsuario = {0}", IdUsuario), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int InsertarHoraDeLlegada(int cmd)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO `veterinaria`.`captahuellas` (`IdUsuario`) VALUES ({0});", cmd), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }

        public static DataSet SelectCropros(int IdEspecie, int IdOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `coproanalisisespecies`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario` FROM `Veterinaria`.`resultadospaciente` join coproanalisisespecies on coproanalisisespecies.IdAnalisis = resultadospaciente.IdAnalisis where IdEspecie = {0} and IdOrden ={1} group by IdAnalisis", IdEspecie, IdOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string Telefono(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            string Retorno = "0";
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select CONCAT('+58',TipoCelular,celular) as Telefono FROM Ordenes join datosrepresentante on Ordenes.idDatosRepresentante = datosrepresentante.idDatosRepresentante Where IdOrden = {0}", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count != 0)
                {
                    if (ds.Tables[0].Rows.Count != 0)
                    {
                        Retorno = ds.Tables[0].Rows[0]["Telefono"].ToString();
                    }

                }

                return Retorno;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Retorno;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet TiposdePago()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM Veterinaria.tiposdepago", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Privilegios(int UserID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Usuarios WHERE(((Usuarios.IdUsuario) = {0}));", UserID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet OrdenesPorCobrar()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Factura.IdFactura,CONCAT(NombreRepresentante,' ',ApellidoRepresentante) as Nombre,FORMAT(precioF,2) AS Facturado, FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)),2) as Entradas,FORMAT(SUM(if(Clasificacion=2,ValorResultado,0)),2) as Salidas,FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2) AS Pago,( -PrecioF + SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0))) as Total FROM Veterinaria.Factura left join pagos on Factura.idFactura = Pagos.IDOrden left join datosrepresentante on Factura.IdRepresentante = datosrepresentante.IdDatosRepresentante Where Factura.Fecha = CURDATE() and IdEstadoDeFactura  < 3 GROUP BY IdFactura"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet OrdenesPendientes()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `cobrodiario`.`IdFactura`,`cobrodiario`.`Fecha`, `cobrodiario`.`Nombre`, `cobrodiario`.`Facturado`, `cobrodiario`.`Total` FROM `Veterinaria`.`cobrodiario` where Total <> 0 and Fecha <> CURDATE();"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet OrdenesPorcruzar(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.cobrodiario  Where IdFactura = {0}", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet OrdenesPorcruzar(int cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Factura.NumeroDia as '#',Factura.IdFactura,Factura.Idsesion,CONCAT(NombreRepresentante,' ',ApellidoRepresentante) as Nombre,FORMAT(Factura.precioF,2) AS Facturado, FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)),2) as Entradas,FORMAT(SUM(if(Clasificacion=2,ValorResultado,0)),2) as Salidas,FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2) AS Pago,(- Factura.precioF + SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0))) As Total FROM Veterinaria.Factura left join Ordenes on Ordenes.IdSesion =Factura.IDsesion left join pagos on Factura.IdFactura = Pagos.IDOrden left join datosrepresentante on Factura.IdRepresentante = datosrepresentante.idDatosRepresentante Where Ordenes.idOrden = {0} and IdEstadoDeFactura  < 3 GROUP BY Idfactura", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet DatosAgrupados(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT IdDatosPaciente,Especies.IdEspecie,NombrePaciente,Descripcion,Raza FROM veterinaria.perfilesfacturados join datospaciente on datospaciente.IdDatosRepresentante = perfilesfacturados.IdDatosRepresentante join Especies on especies.IDEspecie = datospaciente.IDEspecie Where IdSesion = 1;", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ExamenesFacturados(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT perfilesafacturar.IdPerfil,NombrePerfil,Cantidad FROM veterinaria.perfilesafacturar join Perfil on Perfil.idPerfil = perfilesafacturar.IdPerfil where iDSesion = {0};", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectAgrupador(string IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select IdAgrupador from Analisislaboratorio Where IdAnalisis  = {0};", IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static Task<DataSet> SecuenciaDeTrabajoAsync()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();

            return Task.Run(() =>
            {
                try
                {

                    con.OpenAsync();
                    adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden, Ordenes.NumeroDia, Ordenes.Fecha,Ordenes.HoraIngreso,CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante,datosrepresentante.ApellidoRepresentante,  Convenios.Nombre FROM(datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio WHERE Ordenes.Fecha = '{0}' AND Ordenes.IdEstadoDeOrden < 3 ORDER BY Ordenes.IdOrden;", DateTime.Now.ToString("yyyy-MM-dd")), con);
                    adapter.Fill(ds);
                    adapter.Dispose();
                    return ds;
                }
                catch (Exception ex)
                {
                    return ds;
                }
                finally
                {
                    con.CloseAsync();
                }
            });
        }

        public static DataTable SecuenciaDeTrabajo()
        {
            DataTable facturas = new DataTable();
            MySqlConnection con = new MySqlConnection(connection);
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `secuenciadetrabajoview`.`NumeroDia` as 'N',`secuenciadetrabajoview`.`idFactura`,`secuenciadetrabajoview`.`Fecha`,`secuenciadetrabajoview`.`Hora`,`secuenciadetrabajoview`.`IdSesion`,`secuenciadetrabajoview`.`TipoRepresentante` as 'T',`secuenciadetrabajoview`.`RIF`,CONCAT(`secuenciadetrabajoview`.`NombreRepresentante`,' ',`secuenciadetrabajoview`.`ApellidoRepresentante`) as Nombre,`secuenciadetrabajoview`.`Convenio`,`secuenciadetrabajoview`.`Usuario`,`secuenciadetrabajoview`.`PrecioF`,`secuenciadetrabajoview`.`Veterinario`,`secuenciadetrabajoview`.`NombreFinca`  FROM veterinaria.secuenciadetrabajoview where Fecha = Curdate()", DateTime.Now.ToString("yyyy-MM-dd")), con);
                adapter.Fill(facturas);
                adapter.Dispose();


                return facturas;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return facturas;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable SecuenciaDeTrabajo(DateTime desde, DateTime Hasta)
        {
            DataTable facturas = new DataTable();
            MySqlConnection con = new MySqlConnection(connection);
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `secuenciadetrabajoview`.`NumeroDia` as 'N',`secuenciadetrabajoview`.`idFactura`,`secuenciadetrabajoview`.`Fecha`,`secuenciadetrabajoview`.`Hora`,`secuenciadetrabajoview`.`IdSesion`,`secuenciadetrabajoview`.`TipoRepresentante`  as 'T',`secuenciadetrabajoview`.`RIF`,CONCAT(`secuenciadetrabajoview`.`NombreRepresentante`,' ',`secuenciadetrabajoview`.`ApellidoRepresentante`) as Nombre,`secuenciadetrabajoview`.`Convenio`,`secuenciadetrabajoview`.`Usuario`,`secuenciadetrabajoview`.`PrecioF`,`secuenciadetrabajoview`.`Veterinario`,`secuenciadetrabajoview`.`NombreFinca` FROM veterinaria.secuenciadetrabajoview where Fecha >= '{0}' and Fecha <= '{1}'", desde.ToString("yyyy/MM/dd"), Hasta.ToString("yyyy/MM/dd")), con);
                adapter.Fill(facturas);
                adapter.Dispose();
                return facturas;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return facturas;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable SecuenciaDeTrabajo(int ID)
        {
            DataTable facturas = new DataTable();
            MySqlConnection con = new MySqlConnection(connection);
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(" SELECT `secuenciadetrabajoview`.`NumeroDia` as 'N',`secuenciadetrabajoview`.`idFactura`,`secuenciadetrabajoview`.`Fecha`,`secuenciadetrabajoview`.`Hora`,`secuenciadetrabajoview`.`IdSesion`,`secuenciadetrabajoview`.`TipoRepresentante` as 'T',`secuenciadetrabajoview`.`RIF`,CONCAT(`secuenciadetrabajoview`.`NombreRepresentante`,' ',`secuenciadetrabajoview`.`ApellidoRepresentante`) as Nombre,`secuenciadetrabajoview`.`Convenio`,`secuenciadetrabajoview`.`Usuario`,`secuenciadetrabajoview`.`PrecioF`,`secuenciadetrabajoview`.`Veterinario`,`secuenciadetrabajoview`.`NombreFinca` FROM veterinaria.secuenciadetrabajoview where IdFactura = {0}", ID), con);
                adapter.Fill(facturas);
                adapter.Dispose();
                return facturas;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return facturas;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable SecuenciaDeTrabajo(string Nombre, string Apellido)
        {
            DataTable facturas = new DataTable();
            MySqlConnection con = new MySqlConnection(connection);
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `secuenciadetrabajoview`.`NumeroDia` as 'N',`secuenciadetrabajoview`.`idFactura`,`secuenciadetrabajoview`.`Fecha`,`secuenciadetrabajoview`.`Hora`,`secuenciadetrabajoview`.`IdSesion`,`secuenciadetrabajoview`.`TipoRepresentante` as 'T',`secuenciadetrabajoview`.`RIF`,CONCAT(`secuenciadetrabajoview`.`NombreRepresentante`,' ',`secuenciadetrabajoview`.`ApellidoRepresentante`) as Nombre,`secuenciadetrabajoview`.`Convenio`,`secuenciadetrabajoview`.`Usuario`,`secuenciadetrabajoview`.`PrecioF`,`secuenciadetrabajoview`.`Veterinario`,`secuenciadetrabajoview`.`NombreFinca` FROM veterinaria.secuenciadetrabajoview where NombreRepresentante like '%{0}%' and ApellidoRepresentante like '%{1}%'", Nombre, Apellido), con);
                adapter.Fill(facturas);
                adapter.Dispose();
                return facturas;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return facturas;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable SecuenciaDeTrabajo(string Cedula)
        {
            DataTable facturas = new DataTable();
            MySqlConnection con = new MySqlConnection(connection);
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `secuenciadetrabajoview`.`NumeroDia` as 'N',`secuenciadetrabajoview`.`idFactura`,`secuenciadetrabajoview`.`Fecha`,`secuenciadetrabajoview`.`Hora`,`secuenciadetrabajoview`.`IdSesion`,`secuenciadetrabajoview`.`TipoRepresentante` as 'T',`secuenciadetrabajoview`.`RIF`,CONCAT(`secuenciadetrabajoview`.`NombreRepresentante`,' ',`secuenciadetrabajoview`.`ApellidoRepresentante`) as Nombre,`secuenciadetrabajoview`.`Convenio`,`secuenciadetrabajoview`.`Usuario`,`secuenciadetrabajoview`.`PrecioF`,`secuenciadetrabajoview`.`Veterinario`,`secuenciadetrabajoview`.`NombreFinca` FROM veterinaria.factura where RIF like '%{0}%'", Cedula), con);
                adapter.Fill(facturas);
                adapter.Dispose();
                return facturas;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return facturas;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet PersonaCobro(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT factura.Fecha,factura.Hora,CONCAT(datosrepresentante.TipoRepresentante,'-',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante as Nombre, datosrepresentante.ApellidoRepresentante as Apellidos,factura.PrecioF,factura.IdTasa,  Convenios.Nombre FROM(datosrepresentante INNER JOIN factura ON datosrepresentante.idDatosRepresentante = factura.IdRepresentante) INNER JOIN Convenios ON factura.IDConvenio = Convenios.IdConvenio WHERE factura.IdFactura = {0}", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ListadoDePacientes(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(@"SELECT 
                    DATE_FORMAT(`factura`.`Fecha`, '%d-%m-%Y') AS Fecha,
                    `factura`.`idFactura` AS Orden,
                    `factura`.`NumeroDia`  AS Nro,
                    CONCAT(datosrepresentante.TipoRepresentante,
                            ' ',
                            datosrepresentante.RIF) AS Cedula,
                    CONCAT(datosrepresentante.NombreRepresentante,
                            ' ',
                            datosrepresentante.ApellidoRepresentante) AS Nombre,
                    CONCAT(datosrepresentante.tipoCelular,
                            '-',
                            datosrepresentante.Celular) AS Celular,
                    CONCAT(datosrepresentante.tipoTelefono,
                            '-',
                            datosrepresentante.Telefono) AS Telefono
                    FROM `veterinaria`.`factura` 
                    INNER JOIN
                    datosrepresentante ON datosrepresentante.idDatosRepresentante = factura.IdRepresentante
                    WHERE
                    factura.fecha BETWEEN '{0}' AND '{1}'
                        AND factura.IdEstadoDeFactura < 3;
                    ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SecuenciaDeLEAN(string Fecha1, string Fecha2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden AS NO, Ordenes.NumeroDia AS N, Ordenes.Fecha,Ordenes.hora as Hora,CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula,CONCAT(datosrepresentante.NombreRepresentante,' ', datosrepresentante.ApellidoRepresentante) AS Nombre, NombreUsuario FROM(datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio Inner join Usuarios on Usuarios.IdUsuario = Ordenes.IdUsuario WHERE Ordenes.Fecha Between '{0}' And '{1}' AND Ordenes.IdEstadoDeOrden < 3 AND Ordenes.IdConvenio <= 3 ORDER BY Ordenes.IdOrden;", Fecha1, Fecha2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SecuenciaDeLEANPorSede(string Fecha1, string Fecha2, string IDConvenio)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden AS NO, Ordenes.NumeroDia AS N, Ordenes.Fecha,Ordenes.HoraIngreso as Hora,CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula,CONCAT(datosrepresentante.NombreRepresentante,' ', datosrepresentante.ApellidoRepresentante) AS Nombre, NombreUsuario FROM(datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio Inner join Usuarios on Usuarios.IdUsuario = Ordenes.Usuario WHERE (((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}') AND Ordenes.IdEstadoDeOrden < 3) AND Ordenes.IdConvenio = {2} ORDER BY Ordenes.IdOrden;", Fecha1, Fecha2, IDConvenio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ConteoSecuenciaDeLEAN(string Fecha1, string Fecha2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(NombreUsuario) as Cant,NombreUsuario FROM(datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio Inner join Usuarios on Usuarios.IdUsuario = Ordenes.IdUsuario WHERE Ordenes.Fecha between '{0}' AND '{1}' AND Ordenes.IdEstadoDeOrden < 3 Group by NombreUsuario ORDER BY Ordenes.IdOrden;", Fecha1, Fecha2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ListaPrecios()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT IdPerfil as ID,NombrePerfil,Precio,PrecioDolar as Dolares FROM Perfil", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ListaPreciosAImprimir()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT `perfil`.`IdPerfil`,`perfil`.`NombrePerfil`, `perfil`.`Precio`,  `perfil`.`Precio` * 1000000 as Bs FROM `Veterinaria`.`perfil` Where Activo = 1 Order By `perfil`.`NombrePerfil` asc;", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Deuda(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `factura`.`Fecha` AS `Fecha`,`factura`.`IdSesion` AS `IdOrden`,CONCAT(`datosrepresentante`.`NombreRepresentante`,' ',`datosrepresentante`.`ApellidoRepresentante`) AS `Nombre`,`factura`.`PrecioF` AS `Facturado`,SUM(IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0)) AS `Entradas`,SUM(IF((`pagos`.`Clasificacion` = 2),`pagos`.`ValorResultado`,0)) AS `Salidas`,SUM((IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0) - IF((`pagos`.`Clasificacion` = 2),`pagos`.`ValorResultado`,0))) AS `Pago`,(-(`factura`.`PrecioF`) + SUM((IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0) - IF((`pagos`.`Clasificacion` = 2),`pagos`.`ValorResultado`, 0)))) AS `Total`  FROM (((`veterinaria`.`factura` LEFT JOIN `pagos` ON ((`factura`.`IdSesion` = `pagos`.`IdOrden`)))  LEFT JOIN `datosrepresentante` ON ((`factura`.`IdRepresentante` = `datosrepresentante`.`idDatosRepresentante`)))  LEFT JOIN `tasadia` ON ((`factura`.`IDTasa` = `tasadia`.`idTasaDia`)))  WHERE ((`factura`.`IdEstadoDeFactura` < 3) and `factura`.`IdSesion` = {0}) and Pagos.Fecha < Curdate() GROUP BY `factura`.`IdFactura`", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SecuenciaPorValidar()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden, Ordenes.NumeroDia, Ordenes.Fecha,CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante as Nombre,datosrepresentante.ApellidoRepresentante as Apellidos,  Convenios.Nombre FROM(datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio WHERE Ordenes.Fecha = '{0}' AND Ordenes.VALIDADA = 0  AND Ordenes.IdEstadoDeOrden < 3 AND IdEstadoDeOrden = 1 ORDER BY Ordenes.IdOrden;", DateTime.Now.ToString("yyyy-MM-dd")), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet DatosUsuario(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select NombreUsuario,empresas.Sede FROM Usuarios join Empresas Where IdUsuario = {0} and Activa = 1", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTPersona(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `datosrepresentante`.`idDatosRepresentante`,`datosrepresentante`.`RIF`,`datosrepresentante`.`TipoRepresentante`,`datosrepresentante`.`NombreRepresentante`,`datosrepresentante`.`ApellidoRepresentante`,`datosrepresentante`.`TipoCelular`,`datosrepresentante`.`Celular`,`datosrepresentante`.`TipoTelefono`,`datosrepresentante`.`Telefono`,`datosrepresentante`.`TipoCorreo`, `datosrepresentante`.`Correo` FROM `veterinaria`.`datosrepresentante` WHERE `datosrepresentante`.`TipoRepresentante` = '{0}' AND `datosrepresentante`.`RIF` = '{1}'", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static int SelectEstadoOrden(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int Estado = 0;
            DataSet ds = new DataSet();
            try
            {
                string Query = "SELECT IdEstadoDeFactura From Factura Where idFactura = @idFactura";
                con.Open();
                MySqlCommand cmd2 = new MySqlCommand(Query, con);
                cmd2.Parameters.AddWithValue("@idFactura", cmd);
                Estado = (int)cmd2.ExecuteScalar();
                return Estado;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Estado;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTPacientePorOrden(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `datospaciente`.`IdDatosPaciente`,`datospaciente`.`NombrePaciente`,`Especies`.`IdEspecie`,`Especies`.`Descripcion` as Especie,`datospaciente`.`Raza`,Concat(`datosrepresentante`.`NombreRepresentante`,' ',`datosrepresentante`.`ApellidoRepresentante`) as NombreRepresentante,`Ordenes`.`NumeroDia` FROM `veterinaria`.`datospaciente` left join `Ordenes` on `datospaciente`.`IdDatosPaciente` = `Ordenes`.`IdDatosPaciente` inner join Especies on Especies.IdEspecie = Datospaciente.IdEspecie inner join datosrepresentante on DatosRepresentante.IdDatosRepresentante = datospaciente.IdDatosRepresentante Where IdOrden = {0}", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Ordenes> SELECTPacientePorSesion(int IdSesion)
        {

            MySqlConnection con = new MySqlConnection(connection);
            List<Ordenes> Ordenes = new List<Ordenes>();
            try
            {
                Ordenes = selectOrdenesPorIdSesion(IdSesion);
                return Ordenes;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Ordenes;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectRepresentantePorSesion(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(@"SELECT 
                `factura`.`IdRepresentante`,
                `factura`.`IdConvenio`,
                `factura`.`IdEstadoDeFactura`
                   FROM
                `veterinaria`.`factura` Where IdSesion = {0}; ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTPaciente(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `datospaciente`.`IdDatosPaciente`,`datospaciente`.`NombrePaciente`, `datospaciente`.`IdEspecie`, `datospaciente`.`Raza`,   `datospaciente`.`IdDatosRepresentante` FROM `veterinaria`.`datospaciente` Where `datospaciente`.`IdDatosPaciente` = {0}; ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTPacientePorRepresentante(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `datospaciente`.`IdDatosPaciente`,`datospaciente`.`IdEspecie`,`datospaciente`.`IdDatosRepresentante`,`datospaciente`.`NombrePaciente`, `Especies`. `Descripcion`,`datospaciente`.`Raza` FROM `veterinaria`.`datospaciente` inner join `veterinaria`.`Especies` on `datospaciente`.`IdEspecie` = `Especies`.`IdEspecie` where IdDatosRepresentante = {0} and Visible = 1;", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTPersonaOrden(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT PrecioF,IDTasa,datosrepresentante.IdDatosRepresentante, concat(datosrepresentante.TipoRepresentante,'-' ,datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante, datosrepresentante.Celular, datosrepresentante.Telefono, datosrepresentante.Correo, datosrepresentante.TipoCorreo, datosrepresentante.TipoCelular as TipoCelular, datosrepresentante.TipoTelefono as TipoTelefono FROM datosrepresentante INNER JOIN factura ON datosrepresentante.iddatosrepresentante = Factura.idRepresentante WHERE(((factura.IdFactura) = {0}))", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTPersonaEmail(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT datosrepresentante.Correo, datosrepresentante.TipoCorreo FROM datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante WHERE(((Ordenes.IdOrden) = {0}))", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTConvenioEmail(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `convenios`.`CorreoSede` FROM `Veterinaria`.`convenios` Where IdConvenio = {0};", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisis(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT  `perfil`.`IdPerfil` AS `IdPerfil`, `perfil`.`NombrePerfil` AS `NombrePerfil`, `perfil`.`Precio` AS `Precio` FROM `perfil` WHERE (`perfil`.`Activo` = 1) AND `perfil`.`IdPerfil` = {0} ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisisResultado(int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ValorResultado,Comentario FROM veterinaria.resultadospaciente Where IdOrden = {0} and IdAnalisis = {1}; ", IdOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisisComentario(int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Comentario FROM veterinaria.resultadospaciente Where IdOrden = {0} and IdAnalisis = {1}; ", IdOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable SELECTAnalisisComentarioHemotripico(int IdOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Comentario FROM veterinaria.resultadospaciente Where IdOrden = {0} and IdAnalisis = {1}; ", IdOrden, 357), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisis3(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.IdPerfil, Perfil.NombrePerfil, PerfilSeleccion.IdAnalisis, PerfilesASeleccionar.NombrePerfil, PerfilesASeleccionar.Precio FROM(Perfil INNER JOIN PerfilSeleccion ON Perfil.IdPerfil = PerfilSeleccion.IdPerfil) INNER JOIN PerfilesASeleccionar ON PerfilSeleccion.IdAnalisis = PerfilesASeleccionar.IdPerfilSeleccion WHERE(((Perfil.IdPerfil) = {0})); ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable SelectAnalisisConValores(string IdAnalisis, string IdEspecie, string IdOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Resultadospaciente.ValorResultado,`analisislaboratorio`.`IdAnalisis`,`MayoroMenorReferencial`.`ValorMenor`,`MayoroMenorReferencial`.`ValorMayor`,`MayoroMenorReferencial`.`Unidad`,`MayoroMenorReferencial`.`MultiplesValores`FROM `veterinaria`.`analisislaboratorio` inner join`veterinaria`.`MayoroMenorReferencial` on `MayoroMenorReferencial`.`IdAnalisis` = `analisislaboratorio`.`idAnalisis` right join `veterinaria`.`ResultadosPaciente` on `ResultadosPaciente`.`IdAnalisis` = `analisislaboratorio`.`idAnalisis`Where `resultadospaciente`.`idAnalisis` = {0} and `MayoroMenorReferencial`.`IdEspecie` = {1} and `resultadospaciente`.`IdOrden` = {2}", IdAnalisis, IdEspecie, IdOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectAnalisisParasitos(string IdEspecie, string IdOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Hemoparasitos.IdHemoparasitos,Descripcion,Resultado FROM veterinaria.resultadohemoparasito inner join Hemoparasitos on Hemoparasitos.IdHemoparasitos = resultadohemoparasito.IdHemoparasitos Where IdOrden = {0} group by Hemoparasitos.IdHemoparasitos;;", IdOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();

                if (ds.Tables[0].Rows.Count == 0)
                {
                    ds.Tables.Clear();
                    command = new MySqlCommand(string.Format("INSERT INTO `veterinaria`.`resultadohemoparasito`(IDhemoparasitos,IdOrden,Resultado) SELECT idHemoparasitos,{0},'NEGATIVO' as Resultado FROM veterinaria.hemoparasitos where IdEspecie = {1};", IdOrden, IdEspecie), con);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                    adapter.SelectCommand = new MySqlCommand(string.Format("SELECT idHemoparasitos,Descripcion,'NEGATIVO' as Resultado FROM veterinaria.hemoparasitos where IdEspecie = {0};", IdEspecie), con);
                    adapter.Fill(ds);
                    adapter.Dispose();
                }
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Hemo> SelectAnalisisParasitos(int IdEspecie, int IdOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            List<Hemo> Hemos = new List<Hemo>();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Hemoparasitos.IdHemoparasitos,Descripcion,Resultado FROM veterinaria.resultadohemoparasito inner join Hemoparasitos on Hemoparasitos.IdHemoparasitos = resultadohemoparasito.IdHemoparasitos Where IdOrden = {0} group by Hemoparasitos.IdHemoparasitos;;", IdOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            Hemo hemo = new Hemo();
                            int IdHemo = 0;
                            int.TryParse(dr["IdHemoparasitos"].ToString(), out IdHemo);
                            hemo.Id = IdHemo;
                            hemo.Descripcion = dr["Descripcion"].ToString();
                            hemo.Resultado = dr["Resultado"].ToString();
                            Hemos.Add(hemo);
                        }
                    }
                }
                return Hemos;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Hemos;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTTasaDia()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT TasaDia.Dolar,TasaDia.Pesos,TasaDia.Euros,TasaDia.Fecha FROM TasaDia ORDER BY TasaDia.idTasaDia DESC LIMIT 1; ", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTTasaPorId(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `tasadia`.`idTasaDia` as ID,   `tasadia`.`Dolar`,   `tasadia`.`Pesos`,    `tasadia`.`Euros`,    `tasadia`.`Fecha` FROM TasaDia Where TasaDia.idTasaDia = {0}", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet CambiarListaPrecio()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT ListaDePrecio,`tasadia`.`idTasaDia` as ID FROM TasaDia ORDER BY TasaDia.idTasaDia DESC LIMIT 1; ", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static Task<DataSet> CambiarListaPrecioAsync()
        {
            DataSet ds = new DataSet();
            return Task.Run(() =>
            {
                MySqlConnection con = new MySqlConnection(connection);
                try
                {

                    con.OpenAsync();
                    adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ListaDePrecio,`tasadia`.`idTasaDia` as ID FROM TasaDia ORDER BY TasaDia.idTasaDia DESC LIMIT 1;"), con);
                    adapter.Fill(ds);
                    adapter.Dispose();
                    return ds;
                }
                catch (Exception ex)
                {
                    return ds;
                }
                finally
                {
                    con.CloseAsync();
                }
            });
        }

        public static DataSet ListadeTrabajo(string fecha1, string fecha2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("select resultadospaciente.IdAnalisis,NombreAnalisis From  resultadospaciente join Analisislaboratorio on  resultadospaciente.IdAnalisis = Analisislaboratorio.IdAnalisis join Ordenes on Ordenes.IdOrden = resultadospaciente.IdOrden Where (IdEstadoDeResultado < 2 or IdEstadoDeResultado is null) And IdEstadoDeOrden < 3 and FechaIngreso = '{0}' group by IdAnalisis order by IdOrganizador asc;", fecha1), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet AnalisisListadeTrabajo(string IdAnalisis, string Fecha)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("select NombrePaciente From resultadospaciente join Ordenes on Ordenes.IdOrden = resultadospaciente.IdOrden join DatosPaciente on Datospaciente.IdDatosPaciente = Ordenes.IdDatosPaciente Where (IdEstadoDeResultado < 2 or IdEstadoDeResultado is null) And IdEstadoDeOrden < 3 and FechaIngreso = '{1}' and IdAnalisis= {0} order by NumeroDia asc", IdAnalisis, Fecha), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectEmpresaActiva()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT `empresas`.`idEmpresa`,`empresas`.`Nombre`,`empresas`.`Rif`,`empresas`.`Ruta`,`empresas`.`Activa`,`empresas`.`Sede`,`empresas`.`Correo`,`empresas`.`Clave`,`empresas`.`Puerto`,`empresas`.`Direccion`,`empresas`.`Telefono` FROM `veterinaria`.`empresas` Where Activa = 1 ", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectEmpresaActivaEstadisticas()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT `convenios`.`IdConvenio`, `convenios`.`Nombre` FROM Veterinaria.convenioempresas join Convenios on convenioempresas.IdConvenio = Convenios.IdConvenio  join Empresas on convenioempresas.IdEmpresas = Empresas.IDEmpresa Where Empresas.Activa != 1;", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectEmpresaConvenio(string IdConvenio)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `empresas`.`IdEmpresa`,`empresas`.`Nombre`,`empresas`.`Rif`,`empresas`.`Ruta`,`empresas`.`Activa`,`empresas`.`Sede`,`empresas`.`Correo`,`empresas`.`Clave`,`empresas`.`Puerto`,`empresas`.`CorreoSistema`,`empresas`.`ClaveSistema`,`empresas`.`Direccion`,`empresas`.`Telefono` FROM `Veterinaria`.`empresas` join  `convenioempresas` on  `convenioempresas`.`IdEmpresas` = `empresas`.`IdEmpresa` Where `convenioempresas`.`IdConvenio` = {0}", IdConvenio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTLipidoGrama(int cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT *  From Veterinaria.perfilesAnalisis Left join mayoromenorreferencial on mayoromenorreferencial.IdAnalisis = PerfilesAnalisis.IdAnalisis  left join resultadospaciente on resultadospaciente.IdAnalisis = PerfilesAnalisis.IdAnalisis Where IdPerfil = 1391 and IdOrden = {0}", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisisText(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.IdPerfil, Perfil.NombrePerfil, Perfil.Precio FROM Perfil WHERE(((Perfil.NombrePerfil)Like '%{0}%') AND((Perfil.Activo) = 1)); ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisisAFacturar(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(" SELECT    `perfilesafacturar`.`IdPerfil` AS `IdPerfil`,  `perfil`.`NombrePerfil` AS `NombrePerfil`,  `perfilesafacturar`.`PrecioPerfil` AS `PrecioPerfil`,   `perfilesafacturar`.`IdSesion` AS `IdSesion`   FROM    (`perfilesafacturar`   JOIN `perfil` ON((`perfilesafacturar`.`IdPerfil` = `perfil`.`IdPerfil`))) Where IDSesion = {0} ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static int Insertar(string RIF, string tipo, string Nombre, string Apellido, string tipoC, string Ncelular, string TipoF, string NTelefono, string TipoCO, string CorreoTxt)
        {
            MySqlConnection con = new MySqlConnection(connection);

            int idPaciente = 0;
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarRepresentante", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("RIF", MySqlDbType.VarChar).Value = RIF;
                cmd2.Parameters.Add("TipoRepresentante", MySqlDbType.VarChar).Value = tipo;
                cmd2.Parameters.Add("NombreRepresentante", MySqlDbType.VarChar).Value = Nombre;
                cmd2.Parameters.Add("ApellidoRepresentante", MySqlDbType.VarChar).Value = Apellido;
                cmd2.Parameters.Add("TipoCelular", MySqlDbType.VarChar).Value = tipoC;
                cmd2.Parameters.Add("Celular", MySqlDbType.VarChar).Value = Ncelular;
                cmd2.Parameters.Add("TipoTelefono", MySqlDbType.VarChar).Value = TipoF;
                cmd2.Parameters.Add("Telefono", MySqlDbType.VarChar).Value = NTelefono;
                cmd2.Parameters.Add("TipoCorreo", MySqlDbType.VarChar).Value = TipoCO;
                cmd2.Parameters.Add("Correo", MySqlDbType.VarChar).Value = CorreoTxt;
                cmd2.Parameters.Add("IDRepresentante", MySqlDbType.Int32);
                cmd2.Parameters["IDRepresentante"].Direction = ParameterDirection.ReturnValue;
                con.Open();
                cmd2.ExecuteNonQuery();
                idPaciente = Convert.ToInt32(cmd2.Parameters["IDRepresentante"].Value);
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return idPaciente;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataTable selectValues(int IdAnalisis, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.mayoromenorreferencial Where IdEspecie = {1}  and IDAnalisis = {0};", IdAnalisis, IdEspecie), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertValoresDeReferencia(AnalisisLaboratorio analisis, int IdEspecie, string ValorMenor, string ValorMayor, string MultiplesValores, string Unidad, int Lineas, int TipoAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection); ;
            string respuesta = "Agregado Satisfactoriamente";
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarValores", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdAnalisis", MySqlDbType.Int32).Value = analisis.IdAnalisis;
                cmd2.Parameters.Add("IdEspecie", MySqlDbType.Int32).Value = IdEspecie;
                cmd2.Parameters.Add("ValorMenor", MySqlDbType.Double).Value = ValorMenor.ToString().Replace(",", ".");
                cmd2.Parameters.Add("ValorMayor", MySqlDbType.Double).Value = ValorMayor.ToString().Replace(",", ".");
                cmd2.Parameters.Add("MultiplesValores", MySqlDbType.LongText).Value = MultiplesValores;
                cmd2.Parameters.Add("Unidad", MySqlDbType.VarChar).Value = Unidad;
                cmd2.Parameters.Add("Lineas", MySqlDbType.Int32).Value = Lineas;
                cmd2.Parameters.Add("TipoAnalisis", MySqlDbType.Int32).Value = TipoAnalisis;
                con.Open();
                cmd2.ExecuteNonQuery();
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta = "Ha ocurrido un error, por favor comuniquese con sistema";
                CrearEvento(ex.ToString());
                return respuesta;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarValoresDeReferencia(AnalisisLaboratorio analisis, int IdEspecie, string ValorMenor, string ValorMayor, string MultiplesValores, string Unidad, int Lineas, int TipoAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection); ;
            string respuesta = "Agregado Satisfactoriamente";
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ActualizarValores", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = analisis.IdAnalisis;
                cmd2.Parameters.Add("IdEspecie1", MySqlDbType.Int32).Value = IdEspecie;
                cmd2.Parameters.Add("ValorMenor", MySqlDbType.Double).Value = ValorMenor.ToString().Replace(",", ".");
                cmd2.Parameters.Add("ValorMayor", MySqlDbType.Double).Value = ValorMayor.ToString().Replace(",", ".");
                cmd2.Parameters.Add("MultiplesValores", MySqlDbType.LongText).Value = MultiplesValores;
                cmd2.Parameters.Add("Unidad", MySqlDbType.VarChar).Value = Unidad;
                cmd2.Parameters.Add("Lineas", MySqlDbType.Int32).Value = Lineas;
                cmd2.Parameters.Add("TipoAnalisis1", MySqlDbType.Int32).Value = TipoAnalisis;
                cmd2.Parameters.Add("NombreAnalisis1", MySqlDbType.VarChar).Value = analisis.NombreAnalisis;
                con.Open();
                cmd2.ExecuteNonQuery();
                return respuesta;
            }
            catch (Exception ex)
            {
                respuesta = "Ha ocurrido un error, por favor comuniquese con sistema";
                CrearEvento(ex.ToString());
                return respuesta;
            }
            finally
            {
                con.Close();
            }
        }
        public static int InsertarOrina(UroAnalisis orina, int IdEstadoDeResultado, int IdOrden, int IdUsuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int idPaciente = 0;
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarOrina", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("Comentario", MySqlDbType.VarChar).Value = orina.Comentario;
                cmd2.Parameters.Add("Color", MySqlDbType.VarChar).Value = orina.Color;
                cmd2.Parameters.Add("Aspecto", MySqlDbType.VarChar).Value = orina.Aspecto;
                cmd2.Parameters.Add("TiraReactiva", MySqlDbType.VarChar).Value = orina.TiraReactiva;
                cmd2.Parameters.Add("Densidad", MySqlDbType.VarChar).Value = orina.Densidad;
                cmd2.Parameters.Add("Glucosa", MySqlDbType.VarChar).Value = orina.Glucosa;
                cmd2.Parameters.Add("Bilirrubina", MySqlDbType.VarChar).Value = orina.Bilirrubina;
                cmd2.Parameters.Add("Nitritos", MySqlDbType.VarChar).Value = orina.Nitritos;
                cmd2.Parameters.Add("Leucocitos", MySqlDbType.VarChar).Value = orina.Leucocitos;
                cmd2.Parameters.Add("Cetonas", MySqlDbType.VarChar).Value = orina.Cetonas;
                cmd2.Parameters.Add("Olor", MySqlDbType.VarChar).Value = orina.Olor;
                cmd2.Parameters.Add("Hemoglobina", MySqlDbType.VarChar).Value = orina.Hemoglobina;
                cmd2.Parameters.Add("Urobilinogeno", MySqlDbType.VarChar).Value = orina.Urobilinogeno;
                cmd2.Parameters.Add("Benedict", MySqlDbType.VarChar).Value = orina.Benedict;
                cmd2.Parameters.Add("Proteinas", MySqlDbType.VarChar).Value = orina.Proteinas;
                cmd2.Parameters.Add("Robert", MySqlDbType.VarChar).Value = orina.Robert;
                cmd2.Parameters.Add("Bacterias", MySqlDbType.VarChar).Value = orina.Bacterias;
                cmd2.Parameters.Add("LeucocitosMicro", MySqlDbType.VarChar).Value = orina.LeucocitosMicro;
                cmd2.Parameters.Add("Hematies", MySqlDbType.VarChar).Value = orina.Hematies;
                cmd2.Parameters.Add("Mucina", MySqlDbType.VarChar).Value = orina.Mucina;
                cmd2.Parameters.Add("Ceplanas", MySqlDbType.VarChar).Value = orina.Ceplanas;
                cmd2.Parameters.Add("Cetransicion", MySqlDbType.VarChar).Value = orina.Cetransicion;
                cmd2.Parameters.Add("Credondas", MySqlDbType.VarChar).Value = orina.Credondas;
                cmd2.Parameters.Add("Blastoconidias", MySqlDbType.VarChar).Value = orina.Blastoconidias;
                cmd2.Parameters.Add("Cilindros", MySqlDbType.VarChar).Value = orina.Cilindros;
                cmd2.Parameters.Add("Cristales", MySqlDbType.VarChar).Value = orina.Cristales;
                cmd2.Parameters.Add("ph", MySqlDbType.VarChar).Value = orina.Ph;
                cmd2.Parameters.Add("IdOrden", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("IdEstadoDeResultado", MySqlDbType.Int32).Value = IdEstadoDeResultado;
                con.Open();
                cmd2.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return idPaciente;
            }
            finally
            {
                con.Close();
            }
        }
        public static int ActualizarRepresentante(string RIF, string tipo, string Nombre, string Apellido, string tipoC, string Ncelular, string TipoF, string NTelefono, string TipoCO, string CorreoTxt, int IdRepresentante)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int idPaciente = 0;
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ActualizarRepresentante", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("RIF1", MySqlDbType.VarChar).Value = RIF;
                cmd2.Parameters.Add("TipoRepresentante1", MySqlDbType.VarChar).Value = tipo;
                cmd2.Parameters.Add("NombreRepresentante", MySqlDbType.VarChar).Value = Nombre;
                cmd2.Parameters.Add("ApellidoRepresentante", MySqlDbType.VarChar).Value = Apellido;
                cmd2.Parameters.Add("TipoCelular", MySqlDbType.VarChar).Value = tipoC;
                cmd2.Parameters.Add("Celular", MySqlDbType.VarChar).Value = Ncelular;
                cmd2.Parameters.Add("TipoTelefono", MySqlDbType.VarChar).Value = TipoF;
                cmd2.Parameters.Add("Telefono", MySqlDbType.VarChar).Value = NTelefono;
                cmd2.Parameters.Add("TipoCorreo", MySqlDbType.VarChar).Value = TipoCO;
                cmd2.Parameters.Add("Correo", MySqlDbType.VarChar).Value = CorreoTxt;
                cmd2.Parameters.Add("IdRepresentate", MySqlDbType.Int32).Value = IdRepresentante;
                con.Open();
                cmd2.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return idPaciente;
            }
            finally
            {
                con.Close();
            }
        }
        public static int BorrarPaciente(int IdDatosPaciente)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int idPaciente = 0;
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("BorrarPaciente", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdDatosPaciente1", MySqlDbType.Int32).Value = IdDatosPaciente;
                cmd2.ExecuteNonQuery();
                idPaciente = Convert.ToInt32(cmd2.Parameters["IdPaciente"].Value);
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return idPaciente;
            }
            finally
            {
                con.Close();
            }
        }
        public static int InsertarPaciente(string NombrePaciente, string IdEspecie, string Raza, string IdDatosRepresentante, int Visible)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int idPaciente = 0;
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarPaciente", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("NombrePaciente", MySqlDbType.VarChar).Value = NombrePaciente;
                cmd2.Parameters.Add("IdEspecie", MySqlDbType.Int32).Value = IdEspecie;
                cmd2.Parameters.Add("Raza", MySqlDbType.VarChar).Value = Raza;
                cmd2.Parameters.Add("IdDatosRepresentante", MySqlDbType.Int32).Value = IdDatosRepresentante;
                cmd2.Parameters.Add("IdPaciente", MySqlDbType.Int32);
                cmd2.Parameters.Add("Visible1", MySqlDbType.Int32).Value = Visible;
                cmd2.Parameters["IdPaciente"].Direction = ParameterDirection.ReturnValue;
                con.Open();
                cmd2.ExecuteNonQuery();
                idPaciente = Convert.ToInt32(cmd2.Parameters["IdPaciente"].Value);
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return idPaciente;
            }
            finally
            {
                con.Close();
            }
        }
        public static int InsertarPerfil(Perfil perfil)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int idPaciente = 0;
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarPerfil", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("NombrePerfil", MySqlDbType.VarChar).Value = perfil.NombrePerfil;
                cmd2.Parameters.Add("Precio", MySqlDbType.Decimal).Value = perfil.Precio;
                cmd2.Parameters.Add("PrecioDolar", MySqlDbType.Decimal).Value = perfil.PrecioDolar;
                cmd2.Parameters.Add("Activo", MySqlDbType.Int32).Value = perfil.Activo;
                con.Open();
                cmd2.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return idPaciente;
            }
            finally
            {
                con.Close();
            }
        }
        public static int FlushHost()
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                command = new MySqlCommand(string.Format("FLUSH HOSTS"), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                int MS = 1;
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static int ActualizarPerfil(Perfil perfil)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                command = new MySqlCommand($"UPDATE `veterinaria`.`perfil` SET `NombrePerfil` = '{perfil.NombrePerfil}', `Precio` = '{perfil.Precio}', `PrecioDolar` = {perfil.PrecioDolar}, `Activo` = {perfil.Activo} WHERE `idPerfil` = {perfil.IdPerfil};", con);
                adapter.UpdateCommand = command;
                adapter.UpdateCommand.ExecuteNonQuery();
                int MS = 1;
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static int ActualizarHemoparasito(string IdOrden, string IdHemoparasito, string Resultado)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            int Contador = 0;
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT COUNT(`resultadohemoparasito`.`IdOrden`) as Contador  FROM `veterinaria`.`resultadohemoparasito` Where IdOrden = {IdOrden} and `IdHemoparasitos` = {IdHemoparasito}", con);
                adapter.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        int.TryParse(ds.Tables[0].Rows[0]["Contador"].ToString(), out Contador);
                    }
                }
                if (Contador > 0)
                {
                    adapter.Dispose();
                    command = new MySqlCommand(string.Format("UPDATE `veterinaria`.`resultadohemoparasito` SET `Resultado` = '{1}' WHERE `IdHemoparasitos` = {2} and `IdOrden` = {0}", IdOrden, Resultado, IdHemoparasito), con);
                    adapter.UpdateCommand = command;
                    adapter.UpdateCommand.ExecuteNonQuery();
                }
                else
                {
                    command = new MySqlCommand($"INSERT INTO `veterinaria`.`resultadohemoparasito`(,`IdHemoparasitos`,`IdOrden`,`Resultado`) VALUES ({IdHemoparasito},{IdOrden},'{Resultado}')", con);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                }

                int MS = 1;
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarPrueba(string IdPaciente, string IdPerfil1, string PrecioPerfil1, string IdSesion1, int Cantidad)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarPerfilTemporal", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdPaciente", MySqlDbType.Int32).Value = IdPaciente;
                cmd2.Parameters.Add("IdPerfil1", MySqlDbType.Int32).Value = IdPerfil1;
                cmd2.Parameters.Add("PrecioPerfil1", MySqlDbType.VarChar).Value = PrecioPerfil1;
                cmd2.Parameters.Add("IdSesion1", MySqlDbType.Int32).Value = IdSesion1;
                cmd2.Parameters.Add("Cantidad", MySqlDbType.Int32).Value = Cantidad;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SeleccionarPagos(string cmd, string fecha1, string fecha2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `pagos`.`IdOrden`,`tiposdepago`.`Descripcion` as Tipodepago,`pagos`.`Cantidad`, `pagos`.`ValorResultado`, `pagos`.`Serial`,`pagos`.`Moneda`,`pagos`.`Clasificacion`,`pagos`.`IdTasa`,`pagos`.`Fecha`, `pagos`.`Hora` FROM `veterinaria`.`pagos` inner join tiposdepago on `tiposdepago`.`idTipoDepago` = `pagos`.`IDTipodePago`  where IdOrden = {0} and ( Fecha >= '{1}' and Fecha <= '{2}')order by Clasificacion asc;", cmd, fecha1, fecha2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }

        }
        public static DataSet SiHayResultadosReportados(int idFactura)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(*) as Contador FROM veterinaria.factura left join Ordenes on Ordenes.IDSesion = Factura.IDSesion left join resultadospaciente on resultadospaciente.IdOrden = Ordenes.IdOrden Where IdFactura = {0} and IdEstadoDeResultado is not null;", idFactura), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }

        }
        public static DataSet SELECTFechaPago(string cmd, string cantidad, string Moneda)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT FECHA FROM `Veterinaria`.`pagos` WHERE IdOrden = {0} and Cantidad = '{1}' And Moneda = {2} ;", cmd, cantidad, Moneda), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }

        }
        public static DataSet SeleccionarPagos2(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `pagos`.`idPagos`,`pagos`.`IdOrden`,`tiposdepago`.`descripcion` as tipodepago,`pagos`.`Cantidad`,`pagos`.`ValorResultado`,`pagos`.`Serial`,`pagos`.`Moneda`,`pagos`.`Clasificacion`,`pagos`.`IdTasa`,`pagos`.`Fecha`,`pagos`.`Hora`  FROM Veterinaria.pagos join tiposdepago on tiposdepago.idTipoDepago = pagos.IDTipodePago where IdOrden = {0} order by Clasificacion asc;", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }

        }
        public static void BorrarPagos(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                command = new MySqlCommand(string.Format("DELETE FROM `Veterinaria`.`pagos`WHERE IdOrden = {0}; ", cmd), con);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                con.Close();
            }

        }
        public static void BorrarPagoSeleccionado(string cmd, string cmd2, string cmd3)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                command = new MySqlCommand(string.Format("DELETE FROM `Veterinaria`.`pagos`WHERE IdOrden = {0} and Cantidad = '{1}' And Moneda = {2} ;", cmd, cmd2, cmd3), con);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                con.Close();
            }

        }


        public static string InsertarPagos(string IdOrden, string TipodePago, string Cantidad, string ValorResultado, string DSerial, string Moneda, string Clasificacion, string IdTasa)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("StoredPagos", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("Cantidad", MySqlDbType.VarChar).Value = Cantidad;
                cmd2.Parameters.Add("TipodePago", MySqlDbType.VarChar).Value = TipodePago;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.Double).Value = Convert.ToDouble(ValorResultado);
                cmd2.Parameters.Add("DSerial", MySqlDbType.VarChar).Value = DSerial;
                cmd2.Parameters.Add("Moneda", MySqlDbType.Int32).Value = Moneda;
                cmd2.Parameters.Add("Clasificacion", MySqlDbType.Int32).Value = Clasificacion;
                cmd2.Parameters.Add("IdTasa", MySqlDbType.Int32).Value = IdTasa;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string BorrarPago(string IDOrden, string IdSerial)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                MySqlCommand cmd2 = new MySqlCommand("BorrarCruce", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IDOrden;
                cmd2.Parameters.Add("IdSerial1", MySqlDbType.VarChar).Value = IdSerial;
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string BorrarAFacturar(string IDSesion, string IdPerfil1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("BorrarAFacturar", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IDSesion", MySqlDbType.Int32).Value = IDSesion;
                cmd2.Parameters.Add("IdPerfil", MySqlDbType.Int32).Value = IdPerfil1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarFactura(string IdRepresentante, string IdPaciente1, string IdUsuario, string PrecioF1, string IdSesion1, string IDConvenio1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarFactura", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdPaciente", MySqlDbType.Int32).Value = IdPaciente1;
                cmd2.Parameters.Add("IdRepresentante", MySqlDbType.Int32).Value = IdRepresentante;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                decimal.TryParse(PrecioF1, out decimal precioF);
                cmd2.Parameters.Add("PrecioF1", MySqlDbType.VarChar).Value = precioF;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                cmd2.Parameters.Add("IDConvenio1", MySqlDbType.Int32).Value = IDConvenio1;
                con.Open();
                string MS = cmd2.ExecuteScalar().ToString();
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarAnalisisAgrupados(string IdRepresentante, string IdPaciente1, string IdUsuario, string PrecioF1, string IdSesion1, string IDConvenio1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarAgrupado", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdPaciente", MySqlDbType.Int32).Value = IdPaciente1;
                cmd2.Parameters.Add("IdRepresentante", MySqlDbType.Int32).Value = IdRepresentante;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("PrecioF1", MySqlDbType.VarChar).Value = PrecioF1.Replace(",", ".");
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                cmd2.Parameters.Add("IDConvenio1", MySqlDbType.Int32).Value = IDConvenio1;
                con.Open();
                string MS = cmd2.ExecuteScalar().ToString();
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarFacturaAgrupada(string IdRepresentante, string IdUsuario, string PrecioF1, string IdSesion1, string IDConvenio1, int IdVeterinario, int IdFinca)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("PerfilesFacturados", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdRepresentante", MySqlDbType.Int32).Value = IdRepresentante;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("PrecioF1", MySqlDbType.VarChar).Value = PrecioF1.Replace(",", ".");
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                cmd2.Parameters.Add("IDConvenio1", MySqlDbType.Int32).Value = IDConvenio1;
                cmd2.Parameters.Add("idFinca", MySqlDbType.Int32).Value = IdFinca;
                cmd2.Parameters.Add("idVeterinario", MySqlDbType.Int32).Value = IdVeterinario;
                con.Open();
                 string MS = cmd2.ExecuteScalar().ToString();
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ResultadosAgrupados(string IdPerfil1, string IdOrden1, string IdSesionLocal)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ResultadosAgrupados", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdPerfil1", MySqlDbType.Int32).Value = IdPerfil1;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden1;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesionLocal;
                con.Open();

                cmd2.ExecuteNonQuery();
                string MS = "Ingresado Satisfactoriamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string CantidaddeHojas(int Hojas)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("CantidadDeHojas", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("Hojas", MySqlDbType.Int32).Value = Hojas;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Insertar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarFacturaRef(string fecha, string IdPaciente1, string Hora, string IdUsuario, string PrecioF1, string IdSesion1, string IDConvenio1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarReferido", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("fecha1", MySqlDbType.DateTime).Value = fecha;
                cmd2.Parameters.Add("idDatosRepresentante1", MySqlDbType.Int32).Value = IdPaciente1;
                cmd2.Parameters.Add("Hora", MySqlDbType.VarChar).Value = Hora;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("PrecioF1", MySqlDbType.VarChar).Value = PrecioF1;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                cmd2.Parameters.Add("IDConvenio1", MySqlDbType.Int32).Value = IDConvenio1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string Query2 = "SELECT LAST_INSERT_ID()";
                cmd2 = new MySqlCommand(Query2, con);
                string MS = cmd2.ExecuteScalar().ToString();
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "0";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarPrueba(string IdPaciente, string IdSesion1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ActualizarPrueba", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdPaciente", MySqlDbType.Int32).Value = IdPaciente;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Actualizado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Actualizar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string BorrarPrueba(string IdPerfil1, string IdSesion1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("DeleteAFacturar", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IDAnalisis", MySqlDbType.Int32).Value = IdPerfil1;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Eliminado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Borrar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string AnularOrden(string IdOrden, string IdUsuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.PrecioF,datosrepresentante.NombreRepresentante,datosrepresentante.ApellidoRepresentante FROM Ordenes join datosrepresentante on Ordenes.idDatosRepresentante = datosrepresentante.idDatosRepresentante Where ordenes.IdOrden = {0} ", IdOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                MySqlCommand cmd2 = new MySqlCommand("AnularOrden", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("Accion", MySqlDbType.VarChar).Value = string.Format("Elimino la Orden: {0} del Paciente {1} {2} , Monto: {3} ", IdOrden, ds.Tables[0].Rows[0][1].ToString(), ds.Tables[0].Rows[0][2].ToString(), ds.Tables[0].Rows[0][0].ToString());
                cmd2.ExecuteNonQuery();
                string MS = "Eliminado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Borrar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string AnularFactura(string IdFactura, string IdUsuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Factura.PrecioF,datosrepresentante.NombreRepresentante,datosrepresentante.ApellidoRepresentante FROM Factura join datosrepresentante on Factura.idRepresentante = datosrepresentante.idDatosRepresentante Where Factura.IdFactura = {0} ", IdFactura), con);
                adapter.Fill(ds);
                adapter.Dispose();
                MySqlCommand cmd2 = new MySqlCommand("AnularFactura", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdFactura", MySqlDbType.Int32).Value = IdFactura;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("Accion", MySqlDbType.VarChar).Value = string.Format("Elimino la Orden: {0} del Paciente {1} {2} , Monto: {3} ", IdFactura, ds.Tables[0].Rows[0][1].ToString(), ds.Tables[0].Rows[0][2].ToString(), ds.Tables[0].Rows[0][0].ToString());
                cmd2.ExecuteNonQuery();
                string MS = "Eliminado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Borrar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarReporte(string Accion, string IdUsuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                MySqlCommand cmd2 = new MySqlCommand("InsertarReporte", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("Accion1", MySqlDbType.LongText).Value = Accion;
                cmd2.ExecuteNonQuery();
                string MS = "Eliminado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error Al Borrar";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string EmpresaLogo(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            string MS;
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                command = new MySqlCommand(string.Format("INSERT INTO Empresas (Nombre, Rif,Ruta) Values ({0})", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                MS = "Agregado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                MS = "";
                return MS;
            }
            finally
            {
                con.Close();
            }
        }

        public static DataSet Convenios(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Convenios.IdConvenio, Convenios.Nombre, Convenios.Descuento FROM Usuarios INNER JOIN(Convenios INNER JOIN ConveniosPorUsuario ON Convenios.IdConvenio = ConveniosPorUsuario.IdConvenio) ON Usuarios.IdUsuario = ConveniosPorUsuario.IdUsuario WHERE(((ConveniosPorUsuario.IdUsuario) = {0}) AND((Convenios.Activos) = '1'));", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Especies()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM ESPECIES ORDER BY IdEspecie Asc", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Especies> OEspecies()
        {
            List<Especies> especies = new List<Especies>();
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM ESPECIES ORDER BY IdEspecie Asc", con);
                adapter.Fill(ds);
                adapter.Dispose();
                foreach (DataRow r in ds.Rows)
                {
                    especies.Add(new Especies
                    {
                        IdEspecie = (int)r["IdEspecie"],
                        Descripcion = r["Descripcion"].ToString()
                    });
                }
                return especies;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return especies;
            }
            finally
            {
                con.Close();
            }
        }
        public static int InsertarResultado(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int NumeroDia = 1;
            try
            {

                int idOrden = -1;
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, con);
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden, Ordenes.idDatosRepresentante, Ordenes.Fecha, Ordenes.HoraIngreso, Ordenes.Usuario, Ordenes.NumeroDia FROM Ordenes WHERE Ordenes.Fecha = Date('yyyy / MM / dd') Order By Ordenes.NumeroDia desc LIMIT 1"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    string Numero = ds.Tables[0].Rows[0]["NumeroDia"].ToString();                                                                        //"2,'07/06/2020','11:28:11 a. m.',1,1"
                    NumeroDia = Convert.ToInt32(Numero) + 1;
                }                                                                                                                                       //Ejemplo {0}=(2,'7/3/2020','2:17',1,1,NumeroDia) 
                command = new MySqlCommand(string.Format("INSERT INTO Ordenes (idDatosRepresentante, Fecha, HoraIngreso, Usuario, IdEstadoDeOrden,PrecioF,IdConvenio,NumeroDia,Validada) Values ({0},{1},1)", cmd, NumeroDia), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                idOrden = Convert.ToInt32(cmd2.ExecuteScalar());
                return idOrden;
            }

            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return 0;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarPerfilesFactura(string cmd2, string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                command = new MySqlCommand(string.Format("INSERT INTO PerfilesFacturados ( IdOrden, idDatosRepresentante, IdPerfil, PrecioPerfil ) SELECT Ordenes.IdOrden, Ordenes.idDatosRepresentante, PerfilesAFacturar.IdPerfil, PerfilesAFacturar.PrecioPerfil FROM Ordenes INNER JOIN PerfilesAFacturar ON Ordenes.IdOrden = PerfilesAFacturar.IDOrden WHERE(((PerfilesAFacturar.IdSesion) = {0}));", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                string query = string.Format("INSERT INTO ResultadosPaciente ( IdOrden, IdPaciente, FechaIngreso, HoraIngreso, IDConvenio, IdAnalisis, IdOrganizador ) SELECT Ordenes.IdOrden, Ordenes.idDatosRepresentante, Ordenes.Fecha, Ordenes.HoraIngreso, Ordenes.IDConvenio, PerfilesAnalisis.IdAnalisis, PerfilesAnalisis.IdOrganizador FROM(Ordenes INNER JOIN PerfilesAFacturar ON Ordenes.IdOrden = PerfilesAFacturar.IDOrden) INNER JOIN PerfilesAnalisis ON PerfilesAFacturar.IdPerfil = PerfilesAnalisis.IdPerfil WHERE(((Ordenes.IdOrden) = {0}))", cmd2);
                command = new MySqlCommand(query, con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string AnalisisEnviados(string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                string query = string.Format("INSERT INTO ResultadosPaciente ( IdOrden, IdPaciente, FechaIngreso, HoraIngreso, IDConvenio, IdAnalisis, IdOrganizador,Recibido )  Values ({0})", cmd2);
                command = new MySqlCommand(query, con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisisPrueba(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();

                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden,AnalisisLaboratorio.IdAnalisis, AnalisisLaboratorio.NombreAnalisis,ResultadosPaciente.ValorResultado,ResultadosPaciente.Unidad,ResultadosPaciente.ValorMenor,ResultadosPaciente.ValorMayor,AnalisisLaboratorio.TipoAnalisis,ResultadosPaciente.IdOrganizador,ResultadosPaciente.idEstadoDeResultado,analisislaboratorio.IdAgrupador FROM veterinaria.resultadospaciente inner join AnalisisLaboratorio on AnalisisLaboratorio.IdAnalisis = resultadospaciente.IdAnalisis where IdOrden = {0} ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisisPruebaSinAgrupar(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden,AnalisisLaboratorio.IdAnalisis,AnalisisLaboratorio.NombreAnalisis,ResultadosPaciente.ValorResultado,MayoroMenorReferencial.Unidad,MayoroMenorReferencial.ValorMenor,MayoroMenorReferencial.ValorMayor,AnalisisLaboratorio.TipoAnalisis,ResultadosPaciente.IdOrganizador,ResultadosPaciente.idEstadoDeResultado,analisislaboratorio.IdAgrupador FROM Veterinaria.resultadospaciente LEFT JOIN Analisislaboratorio ON Analisislaboratorio.IdAnalisis = Resultadospaciente.IDAnalisis        LEFT JOIN    mayoromenorreferencial ON mayoromenorreferencial.IdAnalisis = AnalisisLaboratorio.IdAnalisis WHERE IdOrden = {0}", cmd), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int SelectCantidadCorreo(string cmd)
        {
            int Correo = 0;
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT if(Count(IdOrden) > 0,Count(IdOrden),0) as Enviado FROM Veterinaria.envioporcorreo where IdOrden = {0};", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    int.TryParse(ds.Rows[0]["Enviado"].ToString(), out Correo);
                }
                return Correo;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Correo;
            }
            finally
            {
                con.Close();
            }
        }
        public static int SELECTPruebasReportadas(string IdSesion)
        {
            int Contador = 0;
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format($"SELECT COUNT(ResultadosPaciente.IdOrden) as Conteo FROM ResultadosPaciente join Ordenes on Ordenes.Idorden = ResultadosPaciente.idSesion Where IdEstadoDeResultado > 0 And IDOrden = {IdSesion}; ", IdSesion), con);
                adapter.Fill(ds);
                adapter.Dispose();
                int.TryParse(ds.Tables[0].Rows[0]["Conteo"].ToString(), out Contador);
                return Contador;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Contador;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAnalisisFinal(int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, Ordenes.NumeroDia, AnalisisLaboratorio.NombreAnalisis, ResultadosPaciente.IdPaciente, ResultadosPaciente.IdAnalisis, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante as Nombre, datosrepresentante.ApellidoRepresentante as Apellidos , .Fecha FROM(datosrepresentante INNER JOIN(AnalisisLaboratorio INNER JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente) INNER JOIN Ordenes ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1})); ", IDOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string SelectComentario(int IDOrden, int IdAnalisis)
        {
            DataTable ds = new DataTable();
            MySqlConnection con = new MySqlConnection(connection);
            string Estado;
            try
            {

                string Query = $"SELECT ResultadosPaciente.Comentario FROM ResultadosPaciente  WHERE(((ResultadosPaciente.IdOrden) = {IDOrden}) AND((ResultadosPaciente.IdAnalisis) = {IdAnalisis})); ";
                con.Open();
                adapter.SelectCommand = new MySqlCommand(Query, con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    Estado = ds.Rows[0]["Comentario"].ToString();
                }
                else
                {
                    Estado = "";
                }

                return Estado;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Estado = "";
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarFinal(string ValorResultado, string Comentario, int IdUsuario, int IdOrden, int IdAnalisis, int IdEspecie, int IdEstadoDeResultado, int Lineas)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ActualizarResultado", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.VarChar).Value = ValorResultado;
                cmd2.Parameters.Add("Comentario", MySqlDbType.LongText).Value = Comentario;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis;
                cmd2.Parameters.Add("IdEspecie1", MySqlDbType.Int32).Value = IdEspecie;
                cmd2.Parameters.Add("EstadoDeResultado", MySqlDbType.Int32).Value = IdEstadoDeResultado;
                cmd2.Parameters.Add("Lineas", MySqlDbType.Int32).Value = Lineas;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarSinValidar(string ValorResultado, string Comentario, int IdUsuario, int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ActualizarResultadoSinValidar", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.VarChar).Value = ValorResultado;
                cmd2.Parameters.Add("Comentario", MySqlDbType.LongText).Value = Comentario;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }

        public static DataSet SELECTAnalisisFinal1(int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, AnalisisLaboratorio.NombreAnalisis, ResultadosPaciente.ValorResultado, Ordenes.NumeroDia, AnalisisLaboratorio.TipoAnalisis, MayoroMenorReferencial.ValorMenor, MayoroMenorReferencial.ValorMayor, MayoroMenorReferencial.Unidad, datosrepresentante.NombreRepresentante as Nombre,datosrepresentante.ApellidoRepresentante as Apellidos, ResultadosPaciente.Comentario FROM(datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN((AnalisisLaboratorio LEFT JOIN MayoroMenorReferencial ON AnalisisLaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente) WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1})); ", IDOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet FechaDeOrden(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `ordenes`.`Fecha`FROM `Veterinaria`.`ordenes` Where IdOrden ={0}; ", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet PacienteAImprimir(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(@"SELECT `ordenes`.`idOrden`,
                CONCAT(`datosrepresentante`.`TipoRepresentante`,
                        ' ', +
                    `datosrepresentante`.`RIF`) AS `Cedula`,
                `datosrepresentante`.`NombreRepresentante`,
                `datosrepresentante`.`ApellidoRepresentante`,
                `datospaciente`.`NombrePaciente`,
                `ordenes`.`NumeroDia`,
                `ordenes`.`Fecha`,
                `especies`.`Descripcion`,
                `datosrepresentante`.`TipoCelular`,
                `datosrepresentante`.`Celular`
                FROM `veterinaria`.`ordenes`
                JOIN `datosrepresentante` ON `datosrepresentante`.`idDatosRepresentante` = `Ordenes`.`idDatosRepresentante`
                JOIN `datospaciente` ON `datospaciente`.`IdDatosPaciente` = `Ordenes`.`IdDatosPaciente`
                JOIN  `Convenios` ON `Convenios`.`IdConvenio` = `ordenes`.`IdConvenio`
                JOIN `Especies` ON `Especies`.`IdEspecie` =  `datospaciente`.`IdEspecie`
                WHERE `ordenes`.`IdOrden` = {0}",
                IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet PacienteAImprimirPorSesion(int IdSesion)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(@"SELECT `ordenes`.`idOrden`,
                CONCAT(`datosrepresentante`.`TipoRepresentante`,
                        ' ', +
                    `datosrepresentante`.`RIF`) AS `Cedula`,
                `datosrepresentante`.`NombreRepresentante`,
                `datosrepresentante`.`ApellidoRepresentante`,
                `datospaciente`.`NombrePaciente`,
                `ordenes`.`NumeroDia`,
                `ordenes`.`Fecha`,
                `especies`.`Descripcion`,
                `datosrepresentante`.`TipoCelular`,
                `datosrepresentante`.`Celular`
                FROM `veterinaria`.`ordenes`
                JOIN `datosrepresentante` ON `datosrepresentante`.`idDatosRepresentante` = `Ordenes`.`idDatosRepresentante`
                JOIN `datospaciente` ON `datospaciente`.`IdDatosPaciente` = `Ordenes`.`IdDatosPaciente`
                JOIN  `Convenios` ON `Convenios`.`IdConvenio` = `ordenes`.`IdConvenio`
                JOIN `Especies` ON `Especies`.`IdEspecie` =  `datospaciente`.`IdEspecie`
                WHERE `ordenes`.`IdSesion` = {0}",
                IdSesion), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string EnviadoPorCorreo(int IDOrden, int User)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                command = new MySqlCommand(string.Format("INSERT INTO `Veterinaria`.`envioporcorreo` (`IdOrden`,`IdUsuario`) VALUES ({0},{1}); ", IDOrden, User), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string NombreAnalisis(int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            string Nombre = "";
            try
            {
                con.Open();
                command = new MySqlCommand(string.Format("SELECT NombreAnalisis FROM veterinaria.analisislaboratorio Where IdAnalisis = {0}; ", IdAnalisis), con);
                adapter.SelectCommand = command;
                Nombre = adapter.SelectCommand.ExecuteScalar().ToString();
                return Nombre;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Nombre;
            }
            finally
            {
                con.Close();
            }

        }
        public static DataSet SELECTAnalisisFinal2(int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, AnalisisLaboratorio.NombreAnalisis, ResultadosPaciente.ValorResultado, Ordenes.NumeroDia, AnalisisLaboratorio.TipoAnalisis, MayoroMenorReferencial.MultiplesValores, MayoroMenorReferencial.Unidad, datosrepresentante.NombreRepresentante as Nombre, datosrepresentante.ApellidoRepresentante as Apellidos, ResultadosPaciente.Comentario FROM(datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN((AnalisisLaboratorio LEFT JOIN MayoroMenorReferencial ON AnalisisLaboratorio.IdAnalisis = MayoroMenorReferencial.idMayoryMenorReferencial) RIGHT JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datosrepresentante.idDatosRepresentante = Ordenes.IdDatosRepresentante) WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1})); ", IDOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectValoresDeReferencia(int IdAnalisis, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ValorMenor,ValorMayor,Unidad,MultiplesValores,Lineas FROM veterinaria.mayoromenorreferencial Where IdAnalisis = {0} and IdEspecie = {1}; ", IdAnalisis, IdEspecie), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<mayoromenorreferencial> SelectValoresDeReferencia(int IdAnalisis)
        {
            List<mayoromenorreferencial> mayoroMenorreferencial = new List<mayoromenorreferencial>();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ValorMenor,ValorMayor,Unidad,MultiplesValores,If(Lineas is null,0,Lineas) FROM veterinaria.mayoromenorreferencial Where IdAnalisis = {0}; ", IdAnalisis), con);
                adapter.Fill(ds);
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            mayoroMenorreferencial.Add
                                (
                             new mayoromenorreferencial
                             {
                                 ValorMenor = ds.Tables[0].Rows[0]["ValorMenor"].ToString(),
                                 ValorMayor = ds.Tables[0].Rows[0]["ValorMayor"].ToString(),
                                 MultiplesValores = ds.Tables[0].Rows[0]["MultiplesValores"].ToString(),
                                 IdEspecie = (int)ds.Tables[0].Rows[0]["IdEspecie"],
                                 lineas = (int)ds.Tables[0].Rows[0]["lineas"],
                                 IdAnalisis = IdAnalisis
                             }
                             );
                        }

                    }


                }
                adapter.Dispose();
                return mayoroMenorreferencial;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return mayoroMenorreferencial;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectListaDeRespuestas(int IdAnalisis, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Respuesta FROM veterinaria.listaderespuestas Where IdAnalisis = {0} and IdEspecie = {1}; ", IdAnalisis, IdEspecie), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public static string Actualizardatosrepresentante(string cmd, string Cedula)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE datosrepresentante SET  {0}  Where Cedula = '{1}' ", cmd, Cedula);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ListaDePrecioImpresa(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE TasaDia SET ListaDePrecio = 1 Where Tasadia.idTasaDia = {0}", cmd);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Hematologia(int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("", IDOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet CantidadesDeExamenes(int IdAnalisis, string date)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select Ordenes.IdOrden,NumeroDia from resultadospaciente join Ordenes on Ordenes.IdOrden = resultadospaciente.IDOrden Where IDAnalisis = {0} and Fecha >= '{1}'", IdAnalisis, date), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet VerificarHematologia(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Hematologias.IdOrden FROM Hematologias WHERE IDORDEN = {0} ", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarHematologia(string cmd, string cmd2, int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                command = new MySqlCommand(string.Format("INSERT INTO `Veterinaria`.`hematologias` (IdOrden, Neutrofilos, linfocitos,Monocitos,Eosinofilos, Basofilos, Hematies, Hemoglobina,Hematocritos,VCM,HCM,CHCM,Plaquetas, Neutrofilos2, Linfocitos2,Monocitos2,Eosinofilos2,Basofilos2,leucocitos) Values ({0})", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis={2} ", cmd2, IDOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarHematologias(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2}", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = con.CreateCommand();
                string Query2 = string.Format("UPDATE Hematologias SET  {0}  Where IdOrden = {1} ", cmd2, IdOrden);
                adapter.UpdateCommand.CommandText = Query2;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Orina(int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, Ordenes.NumeroDia, Ordenes.Fecha AS FechaImp, ResultadosPaciente.Comentario, ResultadosPaciente.IdAnalisis, datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante as Apellidos, Orinas.Color, Orinas.Aspecto, Orinas.Densidad, Orinas.ph, Orinas.Glucosa, Orinas.Bilirrubina, Orinas.Nitritos, Orinas.Leucocitos, Orinas.cetonas, Orinas.Olor, Orinas.Hemoglobina, Orinas.Urobilinogeno, Orinas.Benedict, Orinas.Proteinas, Orinas.Robert, Orinas.Bacterias, Orinas.LeucocitosMicro, Orinas.Hematies, Orinas.Mucina, Orinas.CEPLANAS, Orinas.CETRANSICION, Orinas.CREDONDAS, Orinas.BLASTOCONIDAS, Orinas.Cristales, Orinas.Cilindros, Orinas.TiraReactiva FROM((datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN ResultadosPaciente ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente)) LEFT JOIN Orinas ON Ordenes.IdOrden = Orinas.IdOrden WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1}));", IDOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static UroAnalisis OrinaVeterinaria(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            UroAnalisis Orina = new UroAnalisis();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM veterinaria.Resultadospaciente Where (IdAnalisis > 274 and IdAnalisis < 301 or IdAnalisis = 42) and IdOrden = {IDOrden}", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            int idAnalisis;
                            idAnalisis = (int)r["IdAnalisis"];
                            switch (idAnalisis)
                            {
                                case 42:
                                    Orina.Comentario = r["Comentario"].ToString();
                                    break;
                                case 275:
                                    Orina.Color = r["ValorResultado"].ToString();
                                    break;
                                case 276:
                                    Orina.Aspecto = r["ValorResultado"].ToString();
                                    break;
                                case 277:
                                    Orina.TiraReactiva = r["ValorResultado"].ToString();
                                    break;
                                case 278:
                                    Orina.Glucosa = r["ValorResultado"].ToString();
                                    break;
                                case 279:
                                    Orina.Bilirrubina = r["ValorResultado"].ToString();
                                    break;
                                case 280:
                                    Orina.Nitritos = r["ValorResultado"].ToString();
                                    break;
                                case 281:
                                    Orina.Leucocitos = r["ValorResultado"].ToString();
                                    break;
                                case 282:
                                    Orina.Cetonas = r["ValorResultado"].ToString();
                                    break;
                                case 283:
                                    Orina.Olor = r["ValorResultado"].ToString();
                                    break;
                                case 284:
                                    Orina.Hemoglobina = r["ValorResultado"].ToString();
                                    break;
                                case 285:
                                    Orina.Urobilinogeno = r["ValorResultado"].ToString();
                                    break;
                                case 286:
                                    Orina.Benedict = r["ValorResultado"].ToString();
                                    break;
                                case 287:
                                    Orina.Proteinas = r["ValorResultado"].ToString();
                                    break;
                                case 288:
                                    Orina.Robert = r["ValorResultado"].ToString();
                                    break;
                                case 289:
                                    Orina.Bacterias = r["ValorResultado"].ToString();
                                    break;
                                case 290:
                                    Orina.LeucocitosMicro = r["ValorResultado"].ToString();
                                    break;
                                case 291:
                                    Orina.Hematies = r["ValorResultado"].ToString();
                                    break;
                                case 292:
                                    Orina.Mucina = r["ValorResultado"].ToString();
                                    break;
                                case 293:
                                    Orina.Ceplanas = r["ValorResultado"].ToString();
                                    break;
                                case 294:
                                    Orina.Cetransicion = r["ValorResultado"].ToString();
                                    break;
                                case 295:
                                    Orina.Credondas = r["ValorResultado"].ToString();
                                    break;
                                case 296:
                                    Orina.Blastoconidias = r["ValorResultado"].ToString();
                                    break;
                                case 297:
                                    Orina.Cristales = r["ValorResultado"].ToString();
                                    break;
                                case 298:
                                    Orina.Cilindros = r["ValorResultado"].ToString();
                                    break;
                                case 299:
                                    Orina.Ph = r["ValorResultado"].ToString();
                                    break;
                                case 300:
                                    Orina.Densidad = r["ValorResultado"].ToString();
                                    break;
                            }
                        }
                    }
                }
                return Orina;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Orina;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarOrinas(string IdOrden, string IdAnalisis, string Usuario1, string Comentario1, string Color, string Aspecto, string Densidad,
        string ph, string Glucosa, string Bilirrubina, string Nitritos, string Leucocitos, string cetonas, string Olor,
        string Hemoglobina, string Urobilinogeno, string Benedict, string Proteinas,
        string Robert, string Bacterias, string LeucocitosMicro, string Hematies,
        string Mucina, string CEPLANAS, string CETRANSICION,
        string CREDONDAS, string BLASTOCONIDAS, string Cristales,
        string Cilindros, string TiraReactiva, string EstadoResultado)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarOrina", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("Color", MySqlDbType.VarChar).Value = Color;
                cmd2.Parameters.Add("Aspecto", MySqlDbType.VarChar).Value = Aspecto;
                cmd2.Parameters.Add("Densidad", MySqlDbType.VarChar).Value = Densidad;
                cmd2.Parameters.Add("ph", MySqlDbType.VarChar).Value = ph;
                cmd2.Parameters.Add("Glucosa", MySqlDbType.VarChar).Value = Glucosa;
                cmd2.Parameters.Add("Bilirrubina", MySqlDbType.VarChar).Value = Bilirrubina;
                cmd2.Parameters.Add("Nitritos", MySqlDbType.VarChar).Value = Nitritos;
                cmd2.Parameters.Add("Leucocitos", MySqlDbType.VarChar).Value = Leucocitos;
                cmd2.Parameters.Add("cetonas", MySqlDbType.VarChar).Value = cetonas;
                cmd2.Parameters.Add("Olor", MySqlDbType.VarChar).Value = Olor;
                cmd2.Parameters.Add("Hemoglobina", MySqlDbType.VarChar).Value = Hemoglobina;
                cmd2.Parameters.Add("Urobilinogeno", MySqlDbType.VarChar).Value = Urobilinogeno;
                cmd2.Parameters.Add("Benedict", MySqlDbType.VarChar).Value = Benedict;
                cmd2.Parameters.Add("Proteinas", MySqlDbType.VarChar).Value = Proteinas;
                cmd2.Parameters.Add("Robert", MySqlDbType.VarChar).Value = Robert;
                cmd2.Parameters.Add("Bacterias", MySqlDbType.VarChar).Value = Bacterias;
                cmd2.Parameters.Add("LeucocitosMicro", MySqlDbType.VarChar).Value = LeucocitosMicro;
                cmd2.Parameters.Add("Hematies", MySqlDbType.VarChar).Value = Hematies;
                cmd2.Parameters.Add("Mucina", MySqlDbType.VarChar).Value = Mucina;
                cmd2.Parameters.Add("CEPLANAS", MySqlDbType.VarChar).Value = CEPLANAS;
                cmd2.Parameters.Add("CETRANSICION", MySqlDbType.VarChar).Value = CETRANSICION;
                cmd2.Parameters.Add("CREDONDAS", MySqlDbType.VarChar).Value = CREDONDAS;
                cmd2.Parameters.Add("BLASTOCONIDAS", MySqlDbType.VarChar).Value = BLASTOCONIDAS;
                cmd2.Parameters.Add("Cristales", MySqlDbType.VarChar).Value = Cristales;
                cmd2.Parameters.Add("Cilindros", MySqlDbType.VarChar).Value = Cilindros;
                cmd2.Parameters.Add("TiraReactiva", MySqlDbType.VarChar).Value = TiraReactiva;
                cmd2.Parameters.Add("IdAnalisis", MySqlDbType.Int32).Value = IdAnalisis;
                cmd2.Parameters.Add("Usuario1", MySqlDbType.Int32).Value = Usuario1;
                cmd2.Parameters.Add("Comentario1", MySqlDbType.LongText).Value = Comentario1;
                cmd2.Parameters.Add("EstadoResultado", MySqlDbType.Int32).Value = EstadoResultado;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarOrinasSinVerificar(string IdOrden, string IdAnalisis, string Usuario1, string Comentario1, string Color, string Aspecto, string Densidad,
        string ph, string Glucosa, string Bilirrubina, string Nitritos, string Leucocitos, string cetonas, string Olor,
        string Hemoglobina, string Urobilinogeno, string Benedict, string Proteinas,
        string Robert, string Bacterias, string LeucocitosMicro, string Hematies,
        string Mucina, string CEPLANAS, string CETRANSICION,
        string CREDONDAS, string BLASTOCONIDAS, string Cristales,
        string Cilindros, string TiraReactiva, string EstadoResultado)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarOrinasSinVerificar", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("Color", MySqlDbType.VarChar).Value = Color;
                cmd2.Parameters.Add("Aspecto", MySqlDbType.VarChar).Value = Aspecto;
                cmd2.Parameters.Add("Densidad", MySqlDbType.VarChar).Value = Densidad;
                cmd2.Parameters.Add("ph", MySqlDbType.VarChar).Value = ph;
                cmd2.Parameters.Add("Glucosa", MySqlDbType.VarChar).Value = Glucosa;
                cmd2.Parameters.Add("Bilirrubina", MySqlDbType.VarChar).Value = Bilirrubina;
                cmd2.Parameters.Add("Nitritos", MySqlDbType.VarChar).Value = Nitritos;
                cmd2.Parameters.Add("Leucocitos", MySqlDbType.VarChar).Value = Leucocitos;
                cmd2.Parameters.Add("cetonas", MySqlDbType.VarChar).Value = cetonas;
                cmd2.Parameters.Add("Olor", MySqlDbType.VarChar).Value = Olor;
                cmd2.Parameters.Add("Hemoglobina", MySqlDbType.VarChar).Value = Hemoglobina;
                cmd2.Parameters.Add("Urobilinogeno", MySqlDbType.VarChar).Value = Urobilinogeno;
                cmd2.Parameters.Add("Benedict", MySqlDbType.VarChar).Value = Benedict;
                cmd2.Parameters.Add("Proteinas", MySqlDbType.VarChar).Value = Proteinas;
                cmd2.Parameters.Add("Robert", MySqlDbType.VarChar).Value = Robert;
                cmd2.Parameters.Add("Bacterias", MySqlDbType.VarChar).Value = Bacterias;
                cmd2.Parameters.Add("LeucocitosMicro", MySqlDbType.VarChar).Value = LeucocitosMicro;
                cmd2.Parameters.Add("Hematies", MySqlDbType.VarChar).Value = Hematies;
                cmd2.Parameters.Add("Mucina", MySqlDbType.VarChar).Value = Mucina;
                cmd2.Parameters.Add("CEPLANAS", MySqlDbType.VarChar).Value = CEPLANAS;
                cmd2.Parameters.Add("CETRANSICION", MySqlDbType.VarChar).Value = CETRANSICION;
                cmd2.Parameters.Add("CREDONDAS", MySqlDbType.VarChar).Value = CREDONDAS;
                cmd2.Parameters.Add("BLASTOCONIDAS", MySqlDbType.VarChar).Value = BLASTOCONIDAS;
                cmd2.Parameters.Add("Cristales", MySqlDbType.VarChar).Value = Cristales;
                cmd2.Parameters.Add("Cilindros", MySqlDbType.VarChar).Value = Cilindros;
                cmd2.Parameters.Add("TiraReactiva", MySqlDbType.VarChar).Value = TiraReactiva;
                cmd2.Parameters.Add("IdAnalisis", MySqlDbType.Int32).Value = IdAnalisis;
                cmd2.Parameters.Add("Usuario1", MySqlDbType.Int32).Value = Usuario1;
                cmd2.Parameters.Add("Comentario1", MySqlDbType.LongText).Value = Comentario1;
                cmd2.Parameters.Add("EstadoResultado", MySqlDbType.Int32).Value = EstadoResultado;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }

        public static string ActualizarOrinas(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2} ", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = con.CreateCommand();
                string Query2 = string.Format("UPDATE Orinas SET  {0}  Where IdOrden = {1} ", cmd2, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query2;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet VerificarOrina(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Orinas.IdOrden FROM Orinas WHERE IDORDEN = {0} ", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Heces(int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, Ordenes.Fecha AS FechaImp, datosrepresentante.NombreRepresentante as Nombre, ResultadosPaciente.Comentario, Ordenes.NumeroDia, datosrepresentante.ApellidoRepresentante as Apellidos, Heces.Color, Heces.Moco, Heces.Reaccion, Heces.Aspecto, Heces.Sangre, Heces.Ph, Heces.Consistencia, Heces.RestosAlimenticios, Heces.Hematies, Heces.Leucocitos, Heces.Parasitos FROM((datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN(AnalisisLaboratorio INNER JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente)) LEFT JOIN Heces ON Ordenes.IdOrden = Heces.IdOrden WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1}));", IDOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet VerificarHeces(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Heces.IdOrden FROM Heces WHERE IDORDEN = {0} ", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarHeces(string cmd, string cmd2, int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                string cmd4 = string.Format("INSERT INTO Heces (IdOrden, Color, Moco,Reaccion,Aspecto, Sangre, Ph,Consistencia,RestosAlimenticios,Hematies,Leucocitos,Parasitos) Values ({0})", cmd);
                command = new MySqlCommand(cmd4, con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis ", cmd2, IDOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarHeces(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2} ", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = con.CreateCommand();
                string Query2 = string.Format("UPDATE Heces SET  {0}  Where IdOrden = {1} ", cmd2, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query2;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectPT(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdPaciente, ResultadosPaciente.IdAnalisis, ResultadosPaciente.ValorResultado FROM ResultadosPaciente WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = 121 Or(ResultadosPaciente.IdAnalisis) = 122 Or(ResultadosPaciente.IdAnalisis) = 123 Or(ResultadosPaciente.IdAnalisis) = 124 Or(ResultadosPaciente.IdAnalisis) = 125 Or(ResultadosPaciente.IdAnalisis) = 126)); ", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public static List<Ordenes> SelectHematologiasOrdenPorSesion(int IdSesion, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            List<Ordenes> ordenes = new List<Ordenes>();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT Ordenes.IdORden,Ordenes.IdDatosPaciente FROM veterinaria.perfilesfacturados left join PerfilesAnalisis on Perfilesfacturados.IdPerfil = perfilesanalisis.Idperfil  join Ordenes on Ordenes.IdSesion = perfilesfacturados.IdSesion join resultadospaciente on ordenes.IdOrden = resultadospaciente.IDOrden and perfilesAnalisis.idAnalisis = resultadospaciente.IdAnalisis left join datospaciente on datospaciente.IdDatosPaciente = ordenes.IdDatosPaciente  Where PerfilesFacturados.IdSesion = {IdSesion} and PerfilesFacturados.IdPerfil = 1 And ValorMenor is not null and datospaciente.IdEspecie = {IdEspecie}  group by IdOrden", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        Ordenes orden = new Ordenes();
                        orden.idOrden = (int)r["idOrden"];
                        orden.IdDatosPaciente = (int)r["IdDatosPaciente"];
                        orden.ResultadosAnalisis = selectResultadosAnalisisPorOrden(orden.idOrden);
                        orden.datosPacienteVet = datosPacientePorId(orden.IdDatosPaciente);
                        orden.datosPacienteVet.especies.hemoParasitos = selectHemoparasitosPorOrden(orden.idOrden);
                        orden.usuarios = selectUsuarioPorId(orden.ResultadosAnalisis.Where(x => x.IdEstadoDeResultado > 0).First().IdUsuario);
                        ordenes.Add(orden);
                    }
                }
                return ordenes;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ordenes;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Ordenes> SelectOrdenesPorPerfil(int IdSesion,int IdPerfil, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            List<Ordenes> ordenes = new List<Ordenes>();
            try
            {

                con.Open(); 
                adapter.SelectCommand = new MySqlCommand($"SELECT Ordenes.IdORden,Ordenes.IdDatosPaciente FROM veterinaria.perfilesfacturados left join PerfilesAnalisis on Perfilesfacturados.IdPerfil = perfilesanalisis.Idperfil  join Ordenes on Ordenes.IdSesion = perfilesfacturados.IdSesion join resultadospaciente on ordenes.IdOrden = resultadospaciente.IDOrden and perfilesAnalisis.idAnalisis = resultadospaciente.IdAnalisis left join datospaciente on datospaciente.IdDatosPaciente = ordenes.IdDatosPaciente  Where PerfilesFacturados.IdSesion = {IdSesion} and PerfilesFacturados.IdPerfil = {IdPerfil} and datospaciente.IdEspecie = {IdEspecie}  group by IdOrden", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        Ordenes orden = new Ordenes();
                        orden.idOrden = (int)r["idOrden"];
                        orden.IdDatosPaciente = (int)r["IdDatosPaciente"];
                        orden.ResultadosAnalisis = selectResultadosAnalisisPorOrden(orden.idOrden);
                        orden.datosPacienteVet = datosPacientePorId(orden.IdDatosPaciente);
                        orden.datosPacienteVet.especies.hemoParasitos = selectHemoparasitosPorOrden(orden.idOrden);
                        orden.usuarios = selectUsuarioPorId(orden.ResultadosAnalisis.Where(x => x.IdEstadoDeResultado > 0).First().IdUsuario);
                        ordenes.Add(orden);
                    }
                }
                return ordenes;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ordenes;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Ordenes> SelectHemotropicosOrdenPorSesion(int IdSesion, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            List<Ordenes> ordenes = new List<Ordenes>();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT Ordenes.IdORden,Ordenes.IdDatosPaciente FROM veterinaria.perfilesfacturados left join PerfilesAnalisis on Perfilesfacturados.IdPerfil = perfilesanalisis.Idperfil  join Ordenes on Ordenes.IdSesion = perfilesfacturados.IdSesion join resultadospaciente on ordenes.IdOrden = resultadospaciente.IDOrden and perfilesAnalisis.idAnalisis = resultadospaciente.IdAnalisis left join datospaciente on datospaciente.IdDatosPaciente = ordenes.IdDatosPaciente  Where PerfilesFacturados.IdSesion = {IdSesion} and PerfilesFacturados.IdPerfil = 1539 And ValorMenor is not null and datospaciente.IdEspecie = {IdEspecie}  group by IdOrden", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        Ordenes orden = new Ordenes();
                        orden.idOrden = (int)r["idOrden"];
                        orden.IdDatosPaciente = (int)r["IdDatosPaciente"];
                        orden.ResultadosAnalisis = selectResultadosAnalisisPorOrden(orden.idOrden);
                        orden.datosPacienteVet = datosPacientePorId(orden.IdDatosPaciente);
                        orden.datosPacienteVet.especies.hemoParasitos = selectHemoparasitosPorOrden(orden.idOrden);
                        orden.usuarios = selectUsuarioPorId(orden.ResultadosAnalisis.Where(x => x.IdEstadoDeResultado > 0).First().IdUsuario);
                        ordenes.Add(orden);
                    }
                }
                return ordenes;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ordenes;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<ResultadosPorAnalisisVet> SelectExamenesSinValoresDeRef(int IdSesion)
        {
            List<ResultadosPorAnalisisVet> analisisLaboratorios = new List<ResultadosPorAnalisisVet>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`,`resultadospaciente`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`resultadospaciente`.`IdUsuario`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`,`resultadospaciente`.`HoraIngreso`,`resultadospaciente`.`IDorganizador`,`resultadospaciente`.`Lineas` FROM `Veterinaria`.`PerfilesFacturados` inner join Ordenes on ordenes.IdSesion = perfilesfacturados.IdSesion left join perfilesanalisis on perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil left join perfil on Perfil.IdPerfil = PerfilesFacturados.IdPerfil left join resultadospaciente on resultadospaciente.IdAnalisis = perfilesanalisis.idAnalisis and Ordenes.IdOrden = resultadospaciente.IdOrden WHERE `PerfilesFacturados`.`IdSesion` = {IdSesion} and idImpresionAgrupada = 5 and IdEstadoDeResultado > 1 group by IdOrden,IdAnalisis order by idOrden; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                        int LineasR = 1;
                        double VaLorMayorR = 0;
                        double ValorMenorR = 0;
                        int IdEstadoDeResultado = 0;
                        int IdUsuario = 0;
                        double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                        double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                        int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                        int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                        int.TryParse(r["Lineas"].ToString(), out LineasR);
                        Resultado.IdAnalisis = (int)r["IdAnalisis"];
                        Resultado.IdOrden = (int)r["IdOrden"];
                        Resultado.ValorResultado = r["ValorResultado"].ToString();
                        Resultado.unidad = r["unidad"].ToString();
                        Resultado.ValorMenor = ValorMenorR;
                        Resultado.ValorMayor = VaLorMayorR;
                        Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                        Resultado.Comentario = r["Comentario"].ToString();
                        Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                        Resultado.IdUsuario = IdUsuario;
                        Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                        Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                        Resultado.IdOrganizador = (int)r["IdOrganizador"];
                        if (IdUsuario > 0)
                        {
                            Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                        }

                        Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                        Resultado.Lineas = LineasR;
                        analisisLaboratorios.Add(Resultado);

                    }

                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<ResultadosPorAnalisisVet> IBRGRUPAL(int IdSesion)
        {
            List<ResultadosPorAnalisisVet> analisisLaboratorios = new List<ResultadosPorAnalisisVet>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`,`resultadospaciente`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`resultadospaciente`.`IdUsuario`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`,`resultadospaciente`.`HoraIngreso`,`resultadospaciente`.`IDorganizador`,`resultadospaciente`.`Lineas` FROM `Veterinaria`.`PerfilesFacturados` inner join Ordenes on ordenes.IdSesion = perfilesfacturados.IdSesion left join perfilesanalisis on perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil left join perfil on Perfil.IdPerfil = PerfilesFacturados.IdPerfil inner join resultadospaciente on resultadospaciente.IdAnalisis = perfilesanalisis.idAnalisis and Ordenes.IdOrden = resultadospaciente.IdOrden WHERE `PerfilesFacturados`.`IdSesion` = {IdSesion} and idImpresionAgrupada = 7  group by IdOrden,IdAnalisis order by idOrden; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                        int LineasR = 1;
                        double VaLorMayorR = 0;
                        double ValorMenorR = 0;
                        int IdEstadoDeResultado = 0;
                        int IdUsuario = 0;
                        double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                        double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                        int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                        int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                        int.TryParse(r["Lineas"].ToString(), out LineasR);
                        Resultado.IdAnalisis = (int)r["IdAnalisis"];
                        Resultado.IdOrden = (int)r["IdOrden"];
                        Resultado.ValorResultado = r["ValorResultado"].ToString();
                        Resultado.unidad = r["unidad"].ToString();
                        Resultado.ValorMenor = ValorMenorR;
                        Resultado.ValorMayor = VaLorMayorR;
                        Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                        Resultado.Comentario = r["Comentario"].ToString();
                        Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                        Resultado.IdUsuario = IdUsuario;
                        Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                        Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                        Resultado.IdOrganizador = (int)r["IdOrganizador"];
                        if (IdUsuario > 0)
                        {
                            Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                        }

                        Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                        Resultado.Lineas = LineasR;
                        analisisLaboratorios.Add(Resultado);

                    }

                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<ResultadosPorAnalisisVet> Elisas(int IdSesion, int IdPerfil, int idEspecie)
        {
            List<ResultadosPorAnalisisVet> analisisLaboratorios = new List<ResultadosPorAnalisisVet>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`,`resultadospaciente`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`resultadospaciente`.`IdUsuario`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`,`resultadospaciente`.`HoraIngreso`,`resultadospaciente`.`IDorganizador`,`resultadospaciente`.`Lineas` FROM `Veterinaria`.`PerfilesFacturados` inner join Ordenes on ordenes.IdSesion = perfilesfacturados.IdSesion left join perfilesanalisis on perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil left join perfil on Perfil.IdPerfil = PerfilesFacturados.IdPerfil inner join resultadospaciente on resultadospaciente.IdAnalisis = perfilesanalisis.idAnalisis and Ordenes.IdOrden = resultadospaciente.IdOrden WHERE `PerfilesFacturados`.`IdSesion` = {IdSesion} and perfilesanalisis.IdPerfil = {IdPerfil} and IdEstadoDeResultado > 1 group by IdOrden,IdAnalisis order by idOrden; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                        int LineasR = 1;
                        double VaLorMayorR = 0;
                        double ValorMenorR = 0;
                        int IdEstadoDeResultado = 0;
                        int IdUsuario = 0;
                        double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                        double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                        int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                        int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                        int.TryParse(r["Lineas"].ToString(), out LineasR);
                        Resultado.IdAnalisis = (int)r["IdAnalisis"];
                        Resultado.IdOrden = (int)r["IdOrden"];
                        Resultado.ValorResultado = r["ValorResultado"].ToString();
                        Resultado.unidad = r["unidad"].ToString();
                        Resultado.ValorMenor = ValorMenorR;
                        Resultado.ValorMayor = VaLorMayorR;
                        Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                        Resultado.Comentario = r["Comentario"].ToString();
                        Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                        Resultado.IdUsuario = IdUsuario;
                        Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                        Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                        Resultado.IdOrganizador = (int)r["IdOrganizador"];
                        if (IdUsuario > 0)
                        {
                            Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                        }

                        Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                        Resultado.Lineas = LineasR;
                        analisisLaboratorios.Add(Resultado);

                    }

                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<ResultadosPorAnalisisVet> Grupales(int IdSesion, int IdPerfil, int idEspecie)
        {
            List<ResultadosPorAnalisisVet> analisisLaboratorios = new List<ResultadosPorAnalisisVet>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`,`resultadospaciente`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`resultadospaciente`.`IdUsuario`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`,`resultadospaciente`.`HoraIngreso`,`resultadospaciente`.`IDorganizador`,`resultadospaciente`.`Lineas` FROM `Veterinaria`.`PerfilesFacturados` inner join Ordenes on ordenes.IdSesion = perfilesfacturados.IdSesion left join perfilesanalisis on perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil left join perfil on Perfil.IdPerfil = PerfilesFacturados.IdPerfil inner join resultadospaciente on resultadospaciente.IdAnalisis = perfilesanalisis.idAnalisis and Ordenes.IdOrden = resultadospaciente.IdOrden WHERE `PerfilesFacturados`.`IdSesion` = {IdSesion} and perfilesanalisis.IdPerfil = {IdPerfil} and IdEstadoDeResultado > 1 and ValorMayor is not null  group by IdAnalisis order by idOrden  ;", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                        int LineasR = 1;
                        double VaLorMayorR = 0;
                        double ValorMenorR = 0;
                        int IdEstadoDeResultado = 0;
                        int IdUsuario = 0;
                        double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                        double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                        int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                        int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                        int.TryParse(r["Lineas"].ToString(), out LineasR);
                        Resultado.IdAnalisis = (int)r["IdAnalisis"];
                        Resultado.IdOrden = (int)r["IdOrden"];
                        Resultado.ValorResultado = r["ValorResultado"].ToString();
                        Resultado.unidad = r["unidad"].ToString();
                        Resultado.ValorMenor = ValorMenorR;
                        Resultado.ValorMayor = VaLorMayorR;
                        Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                        Resultado.Comentario = r["Comentario"].ToString();
                        Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                        Resultado.IdUsuario = IdUsuario;
                        Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                        Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                        Resultado.IdOrganizador = (int)r["IdOrganizador"];
                        if (IdUsuario > 0)
                        {
                            Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                        }

                        Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                        Resultado.Lineas = LineasR;
                        analisisLaboratorios.Add(Resultado);

                    }

                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<ResultadosPorAnalisisVet> DVBGRUPAL(int IdSesion)
        {
            List<ResultadosPorAnalisisVet> analisisLaboratorios = new List<ResultadosPorAnalisisVet>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`,`resultadospaciente`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`resultadospaciente`.`IdUsuario`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`,`resultadospaciente`.`HoraIngreso`,`resultadospaciente`.`IDorganizador`,`resultadospaciente`.`Lineas` FROM `Veterinaria`.`PerfilesFacturados` inner join Ordenes on ordenes.IdSesion = perfilesfacturados.IdSesion left join perfilesanalisis on perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil left join perfil on Perfil.IdPerfil = PerfilesFacturados.IdPerfil inner join resultadospaciente on resultadospaciente.IdAnalisis = perfilesanalisis.idAnalisis and Ordenes.IdOrden = resultadospaciente.IdOrden WHERE `PerfilesFacturados`.`IdSesion` = {IdSesion} and idImpresionAgrupada =8  group by IdOrden,IdAnalisis order by idOrden; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                        int LineasR = 1;
                        double VaLorMayorR = 0;
                        double ValorMenorR = 0;
                        int IdEstadoDeResultado = 0;
                        int IdUsuario = 0;
                        double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                        double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                        int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                        int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                        int.TryParse(r["Lineas"].ToString(), out LineasR);
                        Resultado.IdAnalisis = (int)r["IdAnalisis"];
                        Resultado.IdOrden = (int)r["IdOrden"];
                        Resultado.ValorResultado = r["ValorResultado"].ToString();
                        Resultado.unidad = r["unidad"].ToString();
                        Resultado.ValorMenor = ValorMenorR;
                        Resultado.ValorMayor = VaLorMayorR;
                        Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                        Resultado.Comentario = r["Comentario"].ToString();
                        Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                        Resultado.IdUsuario = IdUsuario;
                        Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                        Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                        Resultado.IdOrganizador = (int)r["IdOrganizador"];
                        if (IdUsuario > 0)
                        {
                            Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                        }

                        Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                        Resultado.Lineas = LineasR;
                        analisisLaboratorios.Add(Resultado);

                    }

                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<ResultadosPorAnalisisVet> NEOSPORAGRUPAL(int IdSesion)
        {
            List<ResultadosPorAnalisisVet> analisisLaboratorios = new List<ResultadosPorAnalisisVet>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`,`resultadospaciente`.`IdAnalisis`,`resultadospaciente`.`ValorResultado`,`resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`resultadospaciente`.`IdUsuario`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`,`resultadospaciente`.`HoraIngreso`,`resultadospaciente`.`IDorganizador`,`resultadospaciente`.`Lineas` FROM `Veterinaria`.`PerfilesFacturados` inner join Ordenes on ordenes.IdSesion = perfilesfacturados.IdSesion left join perfilesanalisis on perfilesanalisis.IdPerfil = perfilesfacturados.Idperfil left join perfil on Perfil.IdPerfil = PerfilesFacturados.IdPerfil inner join resultadospaciente on resultadospaciente.IdAnalisis = perfilesanalisis.idAnalisis and Ordenes.IdOrden = resultadospaciente.IdOrden WHERE `PerfilesFacturados`.`IdSesion` = {IdSesion} and idImpresionAgrupada =9  group by IdOrden,IdAnalisis order by idOrden; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Rows)
                    {
                        ResultadosPorAnalisisVet Resultado = new ResultadosPorAnalisisVet();
                        int LineasR = 1;
                        double VaLorMayorR = 0;
                        double ValorMenorR = 0;
                        int IdEstadoDeResultado = 0;
                        int IdUsuario = 0;
                        double.TryParse(r["VaLorMayor"].ToString(), out VaLorMayorR);
                        double.TryParse(r["ValorMenor"].ToString(), out ValorMenorR);
                        int.TryParse(r["IdEstadoDeResultado"].ToString(), out IdEstadoDeResultado);
                        int.TryParse(r["IdUsuario"].ToString(), out IdUsuario);

                        int.TryParse(r["Lineas"].ToString(), out LineasR);
                        Resultado.IdAnalisis = (int)r["IdAnalisis"];
                        Resultado.IdOrden = (int)r["IdOrden"];
                        Resultado.ValorResultado = r["ValorResultado"].ToString();
                        Resultado.unidad = r["unidad"].ToString();
                        Resultado.ValorMenor = ValorMenorR;
                        Resultado.ValorMayor = VaLorMayorR;
                        Resultado.IdEstadoDeResultado = IdEstadoDeResultado;
                        Resultado.Comentario = r["Comentario"].ToString();
                        Resultado.MultiplesValores = r["MultiplesValores"].ToString();
                        Resultado.IdUsuario = IdUsuario;
                        Resultado.FechaIngreso = Convert.ToDateTime(r["FechaIngreso"] is DBNull ? DateTime.Now : r["FechaIngreso"]);
                        Resultado.HoraIngreso = r["HoraIngreso"].ToString();
                        Resultado.IdOrganizador = (int)r["IdOrganizador"];
                        if (IdUsuario > 0)
                        {
                            Resultado.bioanalista = selectUsuarioPorId(IdUsuario);
                        }

                        Resultado.analisisLaboratorio = selectAnalisisPorID((int)r["IdAnalisis"]);
                        Resultado.Lineas = LineasR;
                        analisisLaboratorios.Add(Resultado);

                    }

                }
                return analisisLaboratorios;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return analisisLaboratorios;
            }
            finally
            {
                conn.Close();
            }
        }
        public static List<CroprologiasGrupal> SelectCoprosWillysYDirecta(int IdSesion, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            List<CroprologiasGrupal> cropologiasGrupal = new List<CroprologiasGrupal>();
            try
            {
                int IdOrden = 0;
                int IdOrdenAnterior = 0;
                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`, `resultadospaciente`.`IdUsuario`, `resultadospaciente`.`IdAnalisis`, `resultadospaciente`.`ValorResultado`, `resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`ordenes`.`IdDatosPaciente`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`, `resultadospaciente`.`HoraIngreso`, `resultadospaciente`.`IDorganizador`, `resultadospaciente`.`Lineas` FROM veterinaria.coproanalisisespecies join resultadospaciente on resultadospaciente.IdAnalisis = coproanalisisespecies.IdAnalisis join Ordenes on Ordenes.IdOrden = resultadospaciente.IdOrden Where IdEspecie = {IdEspecie} and IdSesion = {IdSesion} and (`resultadospaciente`.IdAnalisis = 303 or `resultadospaciente`.IdAnalisis = 304);", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    CroprologiasGrupal coproGrupal = new CroprologiasGrupal();
                    int Contador = 1;
                    foreach (DataRow r in ds.Rows)
                    {
                  
                        int idAnalisis = 0;
                        int IdDatosPaciente = 0;
                        int idUsuario = 0;
                        int.TryParse(r["idOrden"].ToString(), out IdOrden);
                        int.TryParse(r["IdAnalisis"].ToString(), out idAnalisis);
                        int.TryParse(r["IdDatosPaciente"].ToString(), out IdDatosPaciente);
                        int.TryParse(r["idUsuario"].ToString(), out idUsuario);
                        switch (idAnalisis)
                        {
                            case 303:
                                coproGrupal.TECNICAWILLYS = r["ValorResultado"].ToString() + "";
                                coproGrupal.Comentarios += r["Comentario"].ToString() + " ";
                                break;
                            case 304:
                                coproGrupal.TECNICADIRECTA = r["ValorResultado"].ToString() + "";
                                coproGrupal.Comentarios += r["Comentario"].ToString() + " ";
                                break;
                        }
                        coproGrupal.Bioanalista = selectUsuarioPorId(idUsuario);
                        coproGrupal.datosPaciente = datosPacientePorId(IdDatosPaciente);
                        if (Contador % 2 == 0)
                        {
                            cropologiasGrupal.Add(coproGrupal);
                            coproGrupal = new CroprologiasGrupal();
                        }
                        Contador++;
                    }

                }
                return cropologiasGrupal;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return cropologiasGrupal;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<CroprologiasGrupal> SelectCoprosMacYTamisado(int IdSesion, int IdEspecie)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            List<CroprologiasGrupal> cropologiasGrupal = new List<CroprologiasGrupal>();
            try
            {
                
                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT `resultadospaciente`.`IdOrden`, `resultadospaciente`.`IdUsuario`, `resultadospaciente`.`IdAnalisis`, `resultadospaciente`.`ValorResultado`, `resultadospaciente`.`Comentario`, `resultadospaciente`.`Unidad`,`resultadospaciente`.`ValorMenor`,`resultadospaciente`.`ValorMayor`,`resultadospaciente`.`MultiplesValores`,`ordenes`.`IdDatosPaciente`,`resultadospaciente`.`IdEstadoDeResultado`,`resultadospaciente`.`Enviado`,`resultadospaciente`.`Recibido`,`resultadospaciente`.`FechaIngreso`, `resultadospaciente`.`HoraIngreso`, `resultadospaciente`.`IDorganizador`, `resultadospaciente`.`Lineas` FROM veterinaria.coproanalisisespecies join resultadospaciente on resultadospaciente.IdAnalisis = coproanalisisespecies.IdAnalisis join Ordenes on Ordenes.IdOrden = resultadospaciente.IdOrden Where IdEspecie = {IdEspecie} and IdSesion = {IdSesion} and (`resultadospaciente`.IdAnalisis = 305 or `resultadospaciente`.IdAnalisis = 306);", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    CroprologiasGrupal coproGrupal = new CroprologiasGrupal();
                    int Contador = 1;
                    foreach (DataRow r in ds.Rows)
                    {

                        int.TryParse(r["idUsuario"].ToString(), out int idUsuario);
                        int.TryParse(r["idOrden"].ToString(), out int IdOrden);
                        int.TryParse(r["IdAnalisis"].ToString(), out int  idAnalisis);
                        int.TryParse(r["IdDatosPaciente"].ToString(), out int IdDatosPaciente);
                        coproGrupal.IdOrden = IdOrden;
                        switch (idAnalisis)
                        {
                            case 305:
                                coproGrupal.TECNICAMAC = r["ValorResultado"].ToString() + "";
                                coproGrupal.Comentarios += r["Comentario"].ToString() + " ";
                                break;
                            case 306:
                                coproGrupal.SEDIMENTACION = r["ValorResultado"].ToString() + "";
                                coproGrupal.Comentarios += r["Comentario"].ToString() + " ";
                                break;
                            default:
                                break;
                        }
                        coproGrupal.Bioanalista = selectUsuarioPorId(idUsuario);
                        coproGrupal.datosPaciente = datosPacientePorId(IdDatosPaciente);

                        if (Contador % 2 == 0)
                        {
                            cropologiasGrupal.Add(coproGrupal);
                            coproGrupal = new CroprologiasGrupal();
                        }
                        Contador++;



                    }
                }
                return cropologiasGrupal;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return cropologiasGrupal;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectPersonaOrden(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden, Ordenes.NumeroDia,  datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante as Apellidos,  FROM datosrepresentante INNER JOIN Ordenes ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante WHERE(((Ordenes.IdOrden) = {0}));", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarPT(string ValorResultado, string Comentario, int IdUsuario, int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ActualizarResultado", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.VarChar).Value = ValorResultado;
                cmd2.Parameters.Add("Comentario", MySqlDbType.VarChar).Value = Comentario;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static int ResultadosSinReportarPorSesion(int idSesion)
        {
            int Contador = 0;
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format($@"SELECT 
                    COUNT(`resultadospaciente`.`IdOrden`) as Contador
                FROM
                    `Veterinaria`.`Ordenes`
                    join resultadospaciente on Ordenes.IdOrden = resultadospaciente.IdOrden
                    Where (IdEstadoDeResultado < 2 or IdEstadoDeResultado is Null)
                    and IDSesion = {idSesion}"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    Contador = int.Parse(ds.Rows[0]["Contador"].ToString());
                }
                return Contador;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Contador;
            }
            finally
            {
                con.Close();
            }
        }
        public static int ResultadosSinReportarPorOrden(int idOrden)
        {
            int Contador = 0;
            MySqlConnection con = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format($@"SELECT 
                    COUNT(`resultadospaciente`.`IdOrden`) as Contador
                FROM
                    `Veterinaria`.`resultadospaciente`
                    Where (IdEstadoDeResultado < 2 or IdEstadoDeResultado is Null)
                    and IdOrden = {idOrden}"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    Contador = int.Parse(ds.Rows[0]["Contador"].ToString());
                }
                return Contador;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Contador;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectPTT(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.resultadospaciente Where IDOrden = {0} and (IdAnalisis = 129 Or IdAnalisis =128 Or IdAnalisis =127);", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarPTT(string cmd, int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE `Veterinaria`.`resultadospaciente` SET {0}  Where `IdOrden` = {1} And `IdAnalisis` = {2}", cmd, IDOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTAGRUPADOR(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                string query = string.Format("SELECT AnalisisLaboratorio.IdAgrupador FROM AnalisisLaboratorio join ResultadosPaciente WHERE(((AnalisisLaboratorio.IdAnalisis) = {1}) AND((ResultadosPaciente.IdOrden) = {0}));", cmd, cmd2);
                adapter.SelectCommand = new MySqlCommand(query, con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTIMPRIMIRSECCION(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                string query = string.Format("SELECT AnalisisLaboratorio.NombreAnalisis,AnalisisLaboratorio.IdAgrupador, AnalisisLaboratorio.IdAnalisis,AnalisisLaboratorio.Titulo,AnalisisLaboratorio.FinalTitulo, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, ResultadosPaciente.IdOrganizador, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante as Nombre,datosrepresentante.ApellidoRepresentante as Apellidos,  .TipoCorreo, datosrepresentante.Correo, ResultadosPaciente.ValorMenor, ResultadosPaciente.ValorMayor, ResultadosPaciente.MultiplesValores, AnalisisLaboratorio.TipoAnalisis, ResultadosPaciente.IdEstadoDeResultado, MayoroMenorReferencial.Lineas, ResultadosPaciente.IdOrden, Usuarios.SIGLAS, ResultadosPaciente.FechaIngreso as FechaImp FROM datosrepresentante RIGHT JOIN((AnalisisLaboratorio LEFT JOIN MayoroMenorReferencial ON AnalisisLaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN(Usuarios RIGHT JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente WHERE(((AnalisisLaboratorio.IdAnalisis) = {1}) AND((ResultadosPaciente.IdOrden) = {0}));", cmd, cmd2);
                adapter.SelectCommand = new MySqlCommand(query, con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTIMPRIMIRSECCIONAGRUPADOR(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                string query = string.Format("SELECT AnalisisLaboratorio.NombreAnalisis,AnalisisLaboratorio.IdAgrupador, AnalisisLaboratorio.IdAnalisis,AnalisisLaboratorio.Titulo,AnalisisLaboratorio.FinalTitulo, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, ResultadosPaciente.IdOrganizador, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante as Apellidos,  .TipoCorreo, datosrepresentante.Correo, ResultadosPaciente.ValorMenor, ResultadosPaciente.ValorMayor, ResultadosPaciente.MultiplesValores, AnalisisLaboratorio.TipoAnalisis, ResultadosPaciente.IdEstadoDeResultado, MayoroMenorReferencial.Lineas, ResultadosPaciente.IdOrden, Usuarios.SIGLAS, ResultadosPaciente.FechaIngreso as FechaImp FROM datosrepresentante RIGHT JOIN((AnalisisLaboratorio LEFT JOIN MayoroMenorReferencial ON AnalisisLaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN(Usuarios RIGHT JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente WHERE(((AnalisisLaboratorio.IdAgrupador) = {1}) AND((ResultadosPaciente.IdOrden) = {0})) Order By IdOrganizador asc;", cmd, cmd2);
                adapter.SelectCommand = new MySqlCommand(query, con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTIMPRIMIRTOTAL(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                string query = string.Format("SELECT AnalisisLaboratorio.NombreAnalisis, AnalisisLaboratorio.IdAnalisis,AnalisisLaboratorio.Titulo,AnalisisLaboratorio.FinalTitulo, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, ResultadosPaciente.IdOrganizador, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante as Apellidos,  .TipoCorreo, datosrepresentante.Correo, ResultadosPaciente.ValorMenor, ResultadosPaciente.ValorMayor, ResultadosPaciente.MultiplesValores, AnalisisLaboratorio.TipoAnalisis, ResultadosPaciente.IdEstadoDeResultado, MayoroMenorReferencial.Lineas, ResultadosPaciente.IdOrden, Usuarios.SIGLAS, ResultadosPaciente.FechaIngreso as FechaImp FROM datosrepresentante RIGHT JOIN((AnalisisLaboratorio LEFT JOIN MayoroMenorReferencial ON AnalisisLaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN(Usuarios RIGHT JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente WHERE(((ResultadosPaciente.IdOrden) = {0}) And IdEstadoDeResultado > 1) ORDER BY ResultadosPaciente.IdOrganizador;", cmd);
                adapter.SelectCommand = new MySqlCommand(query, con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarLipidograma(string cmd, int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2} ", cmd, IDOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ReenviarReferido(string IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  Enviado='0'  Where IdOrden = {0}", IDOrden);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ReenviarReferidoEspeciales(string IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  Enviado='0' , IdEstadoDeResultado = 2 Where IdOrden = {0}", IDOrden);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ReimprimirEtiquetas(string IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE Ordenes SET  Muestra='0' Where IdOrden = {0}", IDOrden);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTTotalFacturado()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT FORMAT(sum(preciof),2) AS SumaDePrecioF,FORMAT(sum(pago),2) AS Pago,FORMAT(sum(total),2) AS total FROM Veterinaria.cobrodiario where Fecha = Curdate();"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTTotalFacturadoCierre(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT FORMAT(sum(preciof),2) AS SumaDePrecioF,FORMAT(sum(pago),2) AS Pago,FORMAT(sum(total),2) AS total FROM Veterinaria.`auditoriacierre` where Fecha = '{0}'", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet TotalDetallado()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Descripcion, Round(SUM(If(`pagos`.Moneda=1 or `pagos`.Moneda=2,if(Clasificacion=1,ROUND(ValorResultado,2),0),if(Clasificacion=1,Cantidad,0))),2)  as Entradas,Round(SUM(If(`pagos`.Moneda=1 or `pagos`.Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Salidas, ROUND(SUM(If(`pagos`.Moneda=1 or `pagos`.Moneda=2,if(Clasificacion=1,ValorResultado,0),if(Clasificacion=1,Cantidad,0))) - SUM(If(`pagos`.Moneda=1 or `pagos`.Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Total FROM `Veterinaria`.`pagos` left join Factura on Factura.IdFactura = Pagos.IdOrden inner join tiposdepago on tiposdepago.idTipoDepago = Pagos.idTipoDepago  Where Pagos.Fecha = CURDATE() and  IdEstadoDeFactura < 3 Group by Descripcion", DateTime.Now.ToString("yyyy/MM/dd")), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet TotalDetalladoPorFecha(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT descripcion, Round(SUM(If(Pagos.Moneda=1 or Pagos.Moneda=2,if(Clasificacion=1,ROUND(ValorResultado,2),0),if(Clasificacion=1,Cantidad,0))),2)  as Entradas,Round(SUM(If(Pagos.Moneda=1 or Pagos.Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Salidas, ROUND(SUM(If(Pagos.Moneda=1 or Pagos.Moneda=2,if(Clasificacion=1,ValorResultado,0),if(Clasificacion=1,Cantidad,0))) - SUM(If(Pagos.Moneda=1 or Pagos.Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Total FROM `Veterinaria`.`pagos` left join Factura on Factura.IdFactura = Pagos.IdOrden inner join tiposdepago on tiposdepago.IDTipodePago = pagos.IdtipodePago Where Pagos.Fecha >= '{0}' and Pagos.Fecha <= '{1}' and IdEstadoDeFactura < 3 Group by pagos.idTipoDepago", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarPrecios(string cmd, string cmd3, string cmd4)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("ActualizarPrecios", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("tasadia", MySqlDbType.VarChar).Value = cmd.Replace(",", ".");
                cmd2.Parameters.Add("Pesos", MySqlDbType.VarChar).Value = cmd3.Replace(",", "."); ;
                cmd2.Parameters.Add("Euros", MySqlDbType.VarChar).Value = cmd4.Replace(",", "."); ;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string RealizarCierre(
        string EfectivoI, string EfectivoV, string EfectivoT,
        string PuntoI, string PuntoV, string PuntoT,
        string PagoMovilI, string PagoMovilV, string PagoMovilT,
        string TransferenciasI, string TransferenciasT, string TransferenciasV,
        string DolarI, string DolarV, string DolarT,
        string PesosI, string PesosV, string PesosT,
        string EurosI, string EurosE, string EurosT,
        string OtrosI, string OtrosE, string OtrosT,
        string OrdenesI, string OrdenesE, string OrdenesT,
        string OtrosIngresosI, string OtrosIngresosV, string OtrosIngresosT,
        string TotalFacturado, string TotalCobrado, string Diferencia
        )
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarCierre", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("EfectivoI", MySqlDbType.VarChar).Value = EfectivoI;
                cmd2.Parameters.Add("EfectivoV", MySqlDbType.VarChar).Value = EfectivoV;
                cmd2.Parameters.Add("EfectivoT", MySqlDbType.VarChar).Value = EfectivoT;
                cmd2.Parameters.Add("PuntoI", MySqlDbType.VarChar).Value = PuntoI;
                cmd2.Parameters.Add("PuntoV", MySqlDbType.VarChar).Value = PuntoV;
                cmd2.Parameters.Add("PuntoT", MySqlDbType.VarChar).Value = PuntoT;
                cmd2.Parameters.Add("PagoMovilI", MySqlDbType.VarChar).Value = PagoMovilI;
                cmd2.Parameters.Add("PagoMovilV", MySqlDbType.VarChar).Value = PagoMovilV;
                cmd2.Parameters.Add("PagoMovilT", MySqlDbType.VarChar).Value = PagoMovilT;
                cmd2.Parameters.Add("TransferenciasI", MySqlDbType.VarChar).Value = TransferenciasI;
                cmd2.Parameters.Add("TransferenciasT", MySqlDbType.VarChar).Value = TransferenciasT;
                cmd2.Parameters.Add("TransferenciasV", MySqlDbType.VarChar).Value = TransferenciasV;
                cmd2.Parameters.Add("DolarI", MySqlDbType.VarChar).Value = DolarI;
                cmd2.Parameters.Add("DolarV", MySqlDbType.VarChar).Value = DolarV;
                cmd2.Parameters.Add("DolarT", MySqlDbType.VarChar).Value = DolarT;
                cmd2.Parameters.Add("PesosI", MySqlDbType.VarChar).Value = PesosI;
                cmd2.Parameters.Add("PesosV", MySqlDbType.VarChar).Value = PesosV;
                cmd2.Parameters.Add("PesosT", MySqlDbType.VarChar).Value = PesosT;
                cmd2.Parameters.Add("EurosI", MySqlDbType.VarChar).Value = EurosI;
                cmd2.Parameters.Add("EurosE", MySqlDbType.VarChar).Value = EurosE;
                cmd2.Parameters.Add("EurosT", MySqlDbType.VarChar).Value = EurosT;
                cmd2.Parameters.Add("OtrosI", MySqlDbType.VarChar).Value = OtrosI;
                cmd2.Parameters.Add("OtrosE", MySqlDbType.VarChar).Value = OtrosE;
                cmd2.Parameters.Add("OtrosT", MySqlDbType.VarChar).Value = OtrosT;
                cmd2.Parameters.Add("OrdenesI", MySqlDbType.VarChar).Value = OrdenesI;
                cmd2.Parameters.Add("OrdenesE", MySqlDbType.VarChar).Value = OrdenesE;
                cmd2.Parameters.Add("OrdenesT", MySqlDbType.VarChar).Value = OrdenesT;
                cmd2.Parameters.Add("OtrosIngresosI", MySqlDbType.VarChar).Value = OtrosIngresosI;
                cmd2.Parameters.Add("OtrosIngresosV", MySqlDbType.VarChar).Value = OtrosIngresosV;
                cmd2.Parameters.Add("OtrosIngresosT", MySqlDbType.VarChar).Value = OtrosIngresosT;
                cmd2.Parameters.Add("TotalFacturado", MySqlDbType.VarChar).Value = TotalFacturado;
                cmd2.Parameters.Add("TotalCobrado", MySqlDbType.VarChar).Value = TotalCobrado;
                cmd2.Parameters.Add("Diferencia", MySqlDbType.VarChar).Value = Diferencia;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Ingresado";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTOrdenes(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.Fecha, Ordenes.IdOrden AS IdOrden, Ordenes.NumeroDia AS NumeroDia,CONCAT(DatosRepresentante.TipoRepresentante, '', datosrepresentante.RIF) as Cedula,CONCAT(DatosRepresentante.NombreRepresentante, '', datosrepresentante.ApellidoRepresentante) as Nombre,DatosPaciente.NombrePaciente,Perfil.IdPerfil AS IdAnalisis, Perfil.NombrePerfil AS NombreAnalisis FROM PerfilesFacturados INNER JOIN Ordenes ON  Ordenes.IdSesion = PerfilesFacturados.IdSesion INNER JOIN datosrepresentante ON datosrepresentante.idDatosRepresentante = Ordenes.IdDatosRepresentante left join Perfil ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil left JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio left join tasadia on TasaDia.idTasaDia = Ordenes.Idtasa left join datospaciente on Ordenes.IdDatosPaciente = datospaciente.IdDatosPaciente WHERE(((Ordenes.Fecha) >= '{0}' And(Ordenes.Fecha) <= '{1}')) group by idOrden,Perfil.IdPerfil ORDER BY Ordenes.IdOrden;", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        } //

        public static DataSet SELECTReportados(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(`resultadospaciente`.`IdOrden`) as Reportados FROM `Veterinaria`.`resultadospaciente` join analisislaboratorio on analisislaboratorio.IdAnalisis = Resultadospaciente.IdAnalisis WHERE(((`resultadospaciente`.`FechaIngreso`) >= '{0}' And(`resultadospaciente`.`FechaIngreso`) <= '{1}')) and IdEstadoDeResultado > 0 and `Analisislaboratorio`.`Visible` = 1 ", cmd, cmd2), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }
        public static DataSet SELECTPorReportar(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(`resultadospaciente`.`IdOrden`) as PorReportar FROM `Veterinaria`.`resultadospaciente` join analisislaboratorio on analisislaboratorio.IdAnalisis = Resultadospaciente.IdAnalisis WHERE(((`resultadospaciente`.`FechaIngreso`) >= '{0}' And(`resultadospaciente`.`FechaIngreso`) <= '{1}')) and IdEstadoDeResultado is null and `Analisislaboratorio`.`Visible` = 1", cmd, cmd2), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }//
        public static DataSet SELECTTotalAnaliis(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(`resultadospaciente`.`IdOrden`) as Total FROM `Veterinaria`.`resultadospaciente` join analisislaboratorio on analisislaboratorio.IdAnalisis = Resultadospaciente.IdAnalisis WHERE(((`resultadospaciente`.`FechaIngreso`) >= '{0}' And(`resultadospaciente`.`FechaIngreso`) <= '{1}')) and `Analisislaboratorio`.`Visible` = 1", cmd, cmd2), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                conn.Close();
            }
        }//
        public static DataSet TotalFacturadoPoPerfil(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.Fecha, PerfilesFacturados.IdOrden AS IdOrden, Ordenes.NumeroDia AS NumeroDia, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula,CONCAT(datosrepresentante.NombreRepresentante,' ', datosrepresentante.ApellidoRepresentante) as Nombre, Perfil.NombrePerfil AS NombreAnalisis,PrecioPerfil as Bolivares,ROUND(PrecioPerfil/Dolar,2) as Dolares FROM(Perfil INNER JOIN((datosrepresentante INNER JOIN PerfilesFacturados ON datosrepresentante.idDatosRepresentante = PerfilesFacturados.idDatosRepresentante) INNER JOIN Ordenes ON(Ordenes.IdOrden = PerfilesFacturados.IdOrden) AND(datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante)) ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio left join tasadia on TasaDia.idTasaDia = Ordenes.Idtasa WHERE(((Ordenes.Fecha) >= '{0}' And(Ordenes.Fecha) <= '{1}')) ORDER BY PerfilesFacturados.IdOrden;  ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        } //
        public static DataSet TotalFacturadoPorOrden(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.Fecha, Ordenes.IdOrden AS IdOrden, Ordenes.NumeroDia AS NumeroDia, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula,CONCAT(datosrepresentante.NombreRepresentante,' ', datosrepresentante.ApellidoRepresentante) as Nombre,ROUND(PrecioF,2) as Bolivares,ROUND(PrecioF/Dolar,2) as Dolares FROM Ordenes left join tasadia on TasaDia.idTasaDia = Ordenes.IdTasa left join datosrepresentante on datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante WHERE(((Ordenes.Fecha) >= '{0}' And(Ordenes.Fecha) <= '{1}')) ORDER BY Ordenes.IdOrden;  ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        } //
        public static DataSet SELECTConteoReferidos(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(Especiales) From ResultadosPaciente inner join analisislaboratorio on analisislaboratorio.IDAnalisis = resultadospaciente.IdAnalisis Where (analisislaboratorio.Especiales = 1) And (FechaIngreso >= '{0}'and FechaIngreso <= '{1}') and analisislaboratorio.Visible = 1;", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTTotalFacturadoFecha(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ROUND(SUM(Factura.preciof),2) AS SumaDePrecioF,ROUND(SUM(preciof/dolar),2) as Dolares FROM Veterinaria.Factura join Tasadia on Factura.IDTasa = TasaDia.idTasaDia Where Factura.Fecha between '{0}' and'{1}' and IdEstadoDeFactura  < 3", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTTotalFacturadoDia(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(@" SELECT 
                    SUM(IF((`pagos`.`Clasificacion` = 1),
                                `pagos`.`ValorResultado`,
                                0))
                     - 
                    SUM(IF((`pagos`.`Clasificacion` = 2),
                        `pagos`.`ValorResultado`,
                        0)) AS `Total`
                FROM
                    Veterinaria.pagos
                        JOIN
                    Factura ON Factura.IdFactura = PAgos.IdOrden
                WHERE
                    Pagos.Fecha >= '{0}'
                        AND Pagos.Fecha <= '{1}'
                        AND IdEstadoDeFactura < 3", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTTotalPagoFecha(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT SUM(IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0)) - SUM(IF((`pagos`.`Clasificacion` = 2), `pagos`.`ValorResultado`,0)) AS `Pago` FROM Veterinaria.pagos join Factura on Factura.IdSesion = PAgos.IdOrden where Pagos.Fecha >= '{0}' and Pagos.Fecha <= '{1}' and IDEstadoDeFactura < 3 ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SecuenciaDeTrabajoPorFecha(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden, Ordenes.NumeroDia, Ordenes.Fecha,Ordenes.Hora,datospaciente.NombrePaciente,Especies.Descripcion,CONCAT(DatosRepresentante.TipoRepresentante,' - ',DatosRepresentante.RIF) as Cedula, DatosRepresentante.NombreRepresentante, DatosRepresentante.ApellidoRepresentante,Convenios.Nombre FROM(DatosRepresentante INNER JOIN Ordenes ON DatosRepresentante.idDatosRepresentante = Ordenes.IdDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio inner join DatosPaciente on DatosPaciente.IdDatosPaciente = Ordenes.IdDatosPaciente join Especies on Especies.IdEspecie = DatosPaciente.IdEspecie WHERE (((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}' AND Ordenes.Idestadodeorden < 3 )) ORDER BY Ordenes.IdOrden", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SecuenciaDeTrabajoPorID(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden, Ordenes.NumeroDia, Ordenes.Fecha,Ordenes.Hora,datospaciente.NombrePaciente,Especie.Descripcion,CONCAT(DatosRepresentante.TipoRepresentante,' - ',DatosRepresentante.RIF) as Cedula, DatosRepresentante.NombreRepresentante, DatosRepresentante.ApellidoRepresentante,Convenios.Nombre FROM(DatosRepresentante INNER JOIN Ordenes ON DatosRepresentante.idDatosRepresentante = Ordenes.IdDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio inner join DatosPaciente on DatosPaciente.IdDatosPaciente = Ordenes.IdDatosPaciente WHERE  WHERE(((Ordenes.IdOrden) = {0})) ORDER BY Ordenes.IdOrden; ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }

        }
        public static DataSet SecuenciaDeTrabajoNombreApellido(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `secuenciadetrabajo`.`IdOrden`,`secuenciadetrabajo`.`NumeroDia`,`secuenciadetrabajo`.`Fecha`, `secuenciadetrabajo`.`Hora`,`secuenciadetrabajo`.`Descripcion`,`secuenciadetrabajo`.`NombrePaciente`,`secuenciadetrabajo`.`NombreRepresentante`,`secuenciadetrabajo`.`Nombre` FROM `veterinaria`.`secuenciadetrabajo` WHERE NombrePaciente like '{0}%' AND  NombreRepresentante like '{1}%' AND IDEstadoDeOrden < 3 ORDER BY Ordenes.IdOrden;", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SecuenciaDeTrabajoPorCedula(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.IdOrden, Ordenes.NumeroDia, Ordenes.Fecha,Ordenes.Hora,datospaciente.NombrePaciente,Especie.Descripcion,CONCAT(DatosRepresentante.TipoRepresentante,' - ',DatosRepresentante.RIF) as Cedula, DatosRepresentante.NombreRepresentante, DatosRepresentante.ApellidoRepresentante,Convenios.Nombre FROM(DatosRepresentante INNER JOIN Ordenes ON DatosRepresentante.idDatosRepresentante = Ordenes.IdDatosRepresentante) INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio inner join DatosPaciente on DatosPaciente.IdDatosPaciente = Ordenes.IdDatosPaciente HAVING(((CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula) = '{0}%'));", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }

        }
        public static string ActualizarOrden(string cmd3, int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2} ", cmd3, IDOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarPorEnviar(string IDOrden, string IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET PorEnviar = 1  Where IdOrden = {0} And IDAnalisis  = {1} ", IDOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTReferidos(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden,resultadospaciente.IdAnalisis," +
                    "CONCAT(datosrepresentante.TipoRepresentante, ' ', datosrepresentante.RIF) as Cedula,"
                + "CONCAT(datosrepresentante.NombreRepresentante, ' ', datosrepresentante.ApellidoRepresentante) as Nombre,"
                + "Datospaciente.NombrePaciente,"
                + "AnalisisLaboratorio.NombreAnalisis, AnalisisLaboratorio.IdSeccion,"
                + "ResultadosPaciente.FechaIngreso, Ordenes.NumeroDia"
                + "FROM datosrepresentante INNER JOIN"
                + "AnalisisLaboratorio INNER JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis"
                + "INNER JOIN Ordenes ON Ordenes.IdOrden = ResultadosPaciente.IdOrden"
                + "AND datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante"
                + "inner join DatosPaciente on Ordenes.IdDatosPaciente = datospaciente.idDatosPaciente"
               + "WHERE AnalisisLaboratorio.Especiales = 1 And PorEnviar = 0 AND"
                + "((ResultadosPaciente.FechaIngreso) >= CURDATE() And(ResultadosPaciente.FechaIngreso) <= CURDATE())"
                + "and IdEstadoDeOrden < 3 ORDER BY fechaIngreso asc, Ordenes.NumeroDia asc, IdOrganizador asc; ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTOrdenesPacientes(string FechaDesde, string FechaHasta)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Factura.NumeroDia as '#',Factura.IdFactura,CONCAT(datosrepresentante.TipoRepresentante,' ',RIF) as Cedula,CONCAT(datosrepresentante.NombreRepresentante,' ',datosrepresentante.ApellidoRepresentante) as Nombre,precioF AS Facturado,IdSesion FROM Veterinaria.Factura left join pagos on Factura.IdFactura = Pagos.IDOrden left join datosrepresentante on Factura.idRepresentante = datosrepresentante.idDatosRepresentante  Where (((Pagos.Fecha) >= '{0}'And (Pagos.Fecha)<= '{1}') Or (Factura.Fecha >= '{0}' And (Factura.Fecha)<= '{1}')) and IdEstadoDeFactura  < 3 GROUP BY IdFactura Order By IdFactura asc;", FechaDesde, FechaHasta), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTPagosORdenesPacientes(string IdOrden, string FechaDesde, string FechaHasta)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT round(SUM((if(Clasificacion=1,ValorResultado,0))),2) as Entradas,round(SUM(if(Clasificacion=2,ValorResultado,0)),2) as Salidas,ROUND(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2) AS Pago,IF(Factura.Fecha != Pagos.Fecha,ROUND(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2),ROUND(- Factura.PrecioF + SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2)) As Total FROM Veterinaria.Factura left join pagos on Factura.IdSesion = Pagos.IDOrden left join datosrepresentante on Factura.IdRepresentante = datosrepresentante.idDatosRepresentante   Where Factura.IdFactura = {0} and IdEstadoDeFactura  < 3 and Pagos.fecha >= '{1}' and Pagos.fecha <= '{2}' GROUP BY Factura.IdSesion Order By Factura.IdSesion asc;", IdOrden, FechaDesde, FechaHasta), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Bioanalista(int OrdenID, int AnalisisID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Usuarios.IdUsuario,Usuarios.NombreUsuario,Usuarios.CB,Usuarios.MPPS,Usuarios.FIRMA From Usuarios Right join Resultadospaciente on resultadospaciente.IdUsuario = Usuarios.IdUsuario Where IdOrden = {0} And IdAnalisis = {1}", OrdenID, AnalisisID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet PorReportar(int OrdenID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(IdEstadoDeResultado) FROM resultadospaciente Inner join AnalisisLaboratorio on ResultadosPaciente.IdAnalisis = AnalisisLaboratorio.IdAnalisis Where IdOrden = {0} AND IdEstadoDeResultado < 2 and AnalisisLaboratorio.tipoanalisis <> 15", OrdenID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECPrivilegios(int ID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Usuarios.Privilegios FROM Usuarios WHERE(((Usuarios.IdUsuario) = {0}));", ID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet CantidadDeHojasFecha(string FechaDesde, string FechaHasta)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT SUM(Cantidad) AS Total From Impresiones Where Fecha >= '{0}' and  Fecha <= '{1}'", FechaDesde, FechaHasta), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Estadisticas(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($@"SELECT 
                Perfil.NombrePerfil, SUM(PerfilesFacturados.Cantidad) as CuentaDeIdPerfil
                FROM
                PerfilesFacturados
                INNER JOIN perfil on perfilesfacturados.idPerfil = perfil.IDPerfil
                inner join Factura on Factura.IdSesion = perfilesfacturados.IDSesion
                where
                PerfilesFacturados.Fecha BETWEEN '{cmd}' AND '{cmd2}'
                and Factura.IdEstadoDeFactura < 3
                GROUP BY perfilesfacturados.IdPErfil", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet EstadisticasAlmacen(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.NombrePerfil, Count(PerfilesFacturados.IdPerfil) AS CuentaDeIdPerfil FROM Perfil INNER JOIN(Ordenes INNER JOIN PerfilesFacturados ON Ordenes.IdOrden = PerfilesFacturados.IdOrden) ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil WHERE(((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}'))  AND Ordenes.IdConvenio <= 3  GROUP BY Perfil.NombrePerfil Order by NombrePerfil asc", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet EstadisticasPorSede(string cmd, string cmd2, string IdConvenio)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.NombrePerfil, Count(PerfilesFacturados.IdPerfil) AS CuentaDeIdPerfil FROM Perfil INNER JOIN(Ordenes INNER JOIN PerfilesFacturados ON Ordenes.IdOrden = PerfilesFacturados.IdOrden) ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil WHERE(((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}'))  AND Ordenes.IdConvenio = {2}  GROUP BY Perfil.NombrePerfil Order by NombrePerfil asc", cmd, cmd2, IdConvenio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet EstadisticasEspeciales(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT resultadospaciente.IDAnalisis,NombreAnalisis,Count(resultadospaciente.IDAnalisis) AS Cantidad FROM Veterinaria.resultadospaciente inner join analisislaboratorio on resultadospaciente.IdAnalisis = analisislaboratorio.IdAnalisis WHERE(((resultadospaciente.FechaIngreso) >='{0}' And (resultadospaciente.FechaIngreso)<='{1}')) And `analisislaboratorio`.`Visible` = 1 And IdEstadoDeResultado > 1 Group by IDAnalisis,IdAgrupador Order by IdOrganizador asc;", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet EstadisticasPorConvenio(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT IdConvenio,Count(resultadospaciente.IDAnalisis) AS Cantidad FROM Veterinaria.resultadospaciente inner join analisislaboratorio on resultadospaciente.IdAnalisis = analisislaboratorio.IdAnalisis WHERE(((resultadospaciente.FechaIngreso) >='{0}' And (resultadospaciente.FechaIngreso)<='{1}')) And `analisislaboratorio`.`Visible` = 1 Group by IdConvenio Order by IdConvenio asc;", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet TotalPersonasFacturadas(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(Ordenes.IdOrden) AS CuentaDeIdOrden FROM Ordenes WHERE Ordenes.Fecha between '{0}' And '{1}'  AND Ordenes.IdConvenio <= 3; ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet TotalPersonasFacturadasPorSede(string cmd, string cmd2, string IdConvenio)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(Ordenes.IdOrden) AS CuentaDeIdOrden FROM Ordenes WHERE(((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}'))  AND Ordenes.IdConvenio = {2}; ", cmd, cmd2, IdConvenio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet AnalisisRealizados(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(Ordenes.IdOrden) AS CuentaDeIdOrden FROM Ordenes WHERE(((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{0}'));", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet EstadisticasAnalisis(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT NombreAnalisis,Count(resultadospaciente.IdAnalisis) as Cantidad FROM Veterinaria.resultadospaciente join analisislaboratorio on analisislaboratorio.IdAnalisis = resultadospaciente.IdAnalisis join Ordenes on Ordenes.IdOrden = Resultadospaciente.IdOrden where ((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}') group by NombreAnalisis;", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisis(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT  CONCAT(datosrepresentante.NombreRepresentante,' ',datosrepresentante.ApellidoRepresentante) as Paciente,DATE_FORMAT(Ordenes.Fecha, '%Y-%m-%d') AS Fecha, ResultadosPaciente.`HoraIngreso`,AnalisisLaboratorio.NombreAnalisis,ResultadosPaciente.FechaValidacion,ResultadosPaciente.`HoraValidacion`,Usuarios.NombreUsuario FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden join datosrepresentante on datosrepresentante.idDatosRepresentante = resultadospaciente.IdPaciente WHERE(((AnalisisLaboratorio.Visible) = 1)) And (((ResultadosPaciente.FechaValidacion) >= '{0}' And (ResultadosPaciente.FechaValidacion)<='{1}')) Order by Fecha,HoraValidacion; ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet MonedaExtranjera(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.reportemonedaextranjera Where Fecha >=  '{0}' and Fecha <=  '{1}' and (Euros <> 0 or Pesos <> 0 or `$ Recibido`<> 0 ); ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisisHora(string cmd, string cmd2, string cmd3, string cmd4)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Ordenes.Fecha, Usuarios.NombreUsuario, AnalisisLaboratorio.NombreAnalisis, ResultadosPaciente.`HoraValidacion` FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((AnalisisLaboratorio.Visible) = 1)) And (((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}') And (HoraValidacion >= '{2}' And HoraValidacion <= '{3}')) Order by Fecha,HoraValidacion; ", cmd, cmd2, cmd3, cmd4), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisisHoraAgrupado(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, Usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((AnalisisLaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) Group by NombreUsuario,Hour(HoraValidacion),Fecha Order by Fecha,HoraValidacion; ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisisConteoPorNombre(string cmd, string cmd2, string cmd3)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, Usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((AnalisisLaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) and NombreUsuario = '{2}' Group by NombreUsuario,Hour(HoraValidacion),Fecha Order by Fecha,HoraValidacion; ", cmd, cmd2, cmd3), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisisHoraAgrupadosPorPersona(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, Usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((AnalisisLaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) and analisislaboratorio.Especiales <> 1 Group by NombreUsuario", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisisHoraAgrupadosPorPersonaConEspeciales(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, Usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((AnalisisLaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) Group by NombreUsuario", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisisContador(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Usuarios.NombreUsuario, Count(Usuarios.NombreUsuario) AS CuentaDeNombreUsuario FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((ResultadosPaciente.FechaValidacion) >='{0}' And (ResultadosPaciente.FechaValidacion)<='{1}')) GROUP BY Usuarios.NombreUsuario, AnalisisLaboratorio.Visible HAVING(((AnalisisLaboratorio.Visible) = 1)); ", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasAnalisisContadorHora(string cmd, string cmd2, string cmd3, string cmd4)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Usuarios.NombreUsuario, Count(Usuarios.NombreUsuario) AS CuentaDeNombreUsuario FROM Ordenes INNER JOIN(AnalisisLaboratorio INNER JOIN(Usuarios INNER JOIN ResultadosPaciente ON Usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON Ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((Ordenes.Fecha) >='{0}' And (Ordenes.Fecha)<='{1}') And (HoraValidacion >= '{2}' And HoraValidacion <= '{3}')) GROUP BY Usuarios.NombreUsuario, AnalisisLaboratorio.Visible HAVING(((AnalisisLaboratorio.Visible) = 1)); ", cmd, cmd2, cmd3, cmd4), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet TeclasYPrivilegios(int Usuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Usuarios.IdUsuario, Usuarios.NombreUsuario, TeclasPorUsuario.Leucocitos, TeclasPorUsuario.Neutrofilos, TeclasPorUsuario.Linfocitos, TeclasPorUsuario.Monocitos, TeclasPorUsuario.Eosinofilos, TeclasPorUsuario.Basofilos, TeclasPorUsuario.Plaquetas FROM Usuarios LEFT JOIN TeclasPorUsuario ON Usuarios.IdUsuario = TeclasPorUsuario.IdUsuario WHERE(((Usuarios.IdUsuario) = {0}));", Usuario), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }


        }
        public static string EstadoOrden(int IdOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE Ordenes SET Ordenes.VALIDADA = 2 WHERE(((Ordenes.IdOrden) = {0})); ", IdOrden);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet HematologiaEspecial(int IDOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, Ordenes.Fecha as FechaImp, Ordenes.NumeroDia, ResultadosPaciente.IdUsuario, ResultadosPaciente.IdAnalisis, datosrepresentante.NombreRepresentante as Nombre, datosrepresentante.ApellidoRepresentante as Apellidos, ResultadosPaciente.Comentario, HemaEspecial.Neutrofilos, HemaEspecial.linfocitos, HemaEspecial.Monocitos, HemaEspecial.Eosinofilos, HemaEspecial.Basofilos, HemaEspecial.Hematies, HemaEspecial.Hemoglobina, HemaEspecial.Hematocritos, HemaEspecial.VCM, HemaEspecial.HCM, HemaEspecial.CHCM, HemaEspecial.Plaquetas, HemaEspecial.Neutrofilos2, HemaEspecial.Linfocitos2, HemaEspecial.Monocitos2, HemaEspecial.Eosinofilos2, HemaEspecial.Basofilos2, HemaEspecial.leucocitos, HemaEspecial.Frotis FROM(datosrepresentante INNER JOIN(HemaEspecial RIGHT JOIN Ordenes ON HemaEspecial.IdOrden = Ordenes.IdOrden) ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) INNER JOIN ResultadosPaciente ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente) WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1})); ", IDOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet VerificarHematologiaEspecial(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT HemaEspecial.IdOrden FROM HemaEspecial WHERE(((HemaEspecial.[IDORDEN]) = {0})); ", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarHematologia(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
        string Comentario1,
        string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
        string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
        string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
        string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologia", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden1;
                cmd2.Parameters.Add("HoraValidacion1", MySqlDbType.VarChar).Value = HoraValidacion1;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis1;
                cmd2.Parameters.Add("Comentario1", MySqlDbType.LongText).Value = Comentario1;
                cmd2.Parameters.Add("Neutrofilos1", MySqlDbType.VarChar).Value = Neutrofilos1;
                cmd2.Parameters.Add("linfocitos1", MySqlDbType.VarChar).Value = linfocitos1;
                cmd2.Parameters.Add("Monocitos1", MySqlDbType.VarChar).Value = Monocitos1;
                cmd2.Parameters.Add("Eosinofilo1", MySqlDbType.VarChar).Value = Eosinofilo1;
                cmd2.Parameters.Add("Basofilos1", MySqlDbType.VarChar).Value = Basofilos1;
                cmd2.Parameters.Add("Hematies1", MySqlDbType.VarChar).Value = Hematies1;
                cmd2.Parameters.Add("Hemoglobina1", MySqlDbType.VarChar).Value = Hemoglobina1;
                cmd2.Parameters.Add("Hematocritos1", MySqlDbType.VarChar).Value = Hematocritos1;
                cmd2.Parameters.Add("VCM1", MySqlDbType.VarChar).Value = VCM1;
                cmd2.Parameters.Add("HCM1", MySqlDbType.VarChar).Value = HCM1;
                cmd2.Parameters.Add("CHCM1", MySqlDbType.VarChar).Value = CHCM1;
                cmd2.Parameters.Add("Plaquetas1", MySqlDbType.VarChar).Value = Plaquetas1;
                cmd2.Parameters.Add("Neutrofilos22", MySqlDbType.VarChar).Value = Neutrofilos22;
                cmd2.Parameters.Add("Linfocitos22", MySqlDbType.VarChar).Value = Linfocitos22;
                cmd2.Parameters.Add("Monocitos22", MySqlDbType.VarChar).Value = Monocitos22;
                cmd2.Parameters.Add("Usuario1", MySqlDbType.VarChar).Value = Usuario1;
                cmd2.Parameters.Add("Eosinofilos22", MySqlDbType.VarChar).Value = Eosinofilos22;
                cmd2.Parameters.Add("Basofilos22", MySqlDbType.VarChar).Value = Basofilos22;
                cmd2.Parameters.Add("leucocitos1", MySqlDbType.VarChar).Value = leucocitos1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarHematologiaSinValidar(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
        string Comentario1,
        string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
        string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
        string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
        string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologiaSSinValidar", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden1;
                cmd2.Parameters.Add("HoraValidacion1", MySqlDbType.VarChar).Value = HoraValidacion1;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis1;
                cmd2.Parameters.Add("Comentario1", MySqlDbType.LongText).Value = Comentario1;
                cmd2.Parameters.Add("Neutrofilos1", MySqlDbType.VarChar).Value = Neutrofilos1;
                cmd2.Parameters.Add("linfocitos1", MySqlDbType.VarChar).Value = linfocitos1;
                cmd2.Parameters.Add("Monocitos1", MySqlDbType.VarChar).Value = Monocitos1;
                cmd2.Parameters.Add("Eosinofilo1", MySqlDbType.VarChar).Value = Eosinofilo1;
                cmd2.Parameters.Add("Basofilos1", MySqlDbType.VarChar).Value = Basofilos1;
                cmd2.Parameters.Add("Hematies1", MySqlDbType.VarChar).Value = Hematies1;
                cmd2.Parameters.Add("Hemoglobina1", MySqlDbType.VarChar).Value = Hemoglobina1;
                cmd2.Parameters.Add("Hematocritos1", MySqlDbType.VarChar).Value = Hematocritos1;
                cmd2.Parameters.Add("VCM1", MySqlDbType.VarChar).Value = VCM1;
                cmd2.Parameters.Add("HCM1", MySqlDbType.VarChar).Value = HCM1;
                cmd2.Parameters.Add("CHCM1", MySqlDbType.VarChar).Value = CHCM1;
                cmd2.Parameters.Add("Plaquetas1", MySqlDbType.VarChar).Value = Plaquetas1;
                cmd2.Parameters.Add("Neutrofilos22", MySqlDbType.VarChar).Value = Neutrofilos22;
                cmd2.Parameters.Add("Linfocitos22", MySqlDbType.VarChar).Value = Linfocitos22;
                cmd2.Parameters.Add("Monocitos22", MySqlDbType.VarChar).Value = Monocitos22;
                cmd2.Parameters.Add("Usuario1", MySqlDbType.VarChar).Value = Usuario1;
                cmd2.Parameters.Add("Eosinofilos22", MySqlDbType.VarChar).Value = Eosinofilos22;
                cmd2.Parameters.Add("Basofilos22", MySqlDbType.VarChar).Value = Basofilos22;
                cmd2.Parameters.Add("leucocitos1", MySqlDbType.VarChar).Value = leucocitos1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarHematologiaEspecial(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
       string Comentario1,
       string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
       string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
       string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
       string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologiaEspecial", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden1;
                cmd2.Parameters.Add("HoraValidacion1", MySqlDbType.VarChar).Value = HoraValidacion1;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis1;
                cmd2.Parameters.Add("Comentario1", MySqlDbType.LongText).Value = Comentario1;
                cmd2.Parameters.Add("Neutrofilos1", MySqlDbType.VarChar).Value = Neutrofilos1;
                cmd2.Parameters.Add("linfocitos1", MySqlDbType.VarChar).Value = linfocitos1;
                cmd2.Parameters.Add("Monocitos1", MySqlDbType.VarChar).Value = Monocitos1;
                cmd2.Parameters.Add("Eosinofilo1", MySqlDbType.VarChar).Value = Eosinofilo1;
                cmd2.Parameters.Add("Basofilos1", MySqlDbType.VarChar).Value = Basofilos1;
                cmd2.Parameters.Add("Hematies1", MySqlDbType.VarChar).Value = Hematies1;
                cmd2.Parameters.Add("Hemoglobina1", MySqlDbType.VarChar).Value = Hemoglobina1;
                cmd2.Parameters.Add("Hematocritos1", MySqlDbType.VarChar).Value = Hematocritos1;
                cmd2.Parameters.Add("VCM1", MySqlDbType.VarChar).Value = VCM1;
                cmd2.Parameters.Add("HCM1", MySqlDbType.VarChar).Value = HCM1;
                cmd2.Parameters.Add("CHCM1", MySqlDbType.VarChar).Value = CHCM1;
                cmd2.Parameters.Add("Plaquetas1", MySqlDbType.VarChar).Value = Plaquetas1;
                cmd2.Parameters.Add("Neutrofilos22", MySqlDbType.VarChar).Value = Neutrofilos22;
                cmd2.Parameters.Add("Linfocitos22", MySqlDbType.VarChar).Value = Linfocitos22;
                cmd2.Parameters.Add("Monocitos22", MySqlDbType.VarChar).Value = Monocitos22;
                cmd2.Parameters.Add("Usuario1", MySqlDbType.VarChar).Value = Usuario1;
                cmd2.Parameters.Add("Eosinofilos22", MySqlDbType.VarChar).Value = Eosinofilos22;
                cmd2.Parameters.Add("Basofilos22", MySqlDbType.VarChar).Value = Basofilos22;
                cmd2.Parameters.Add("leucocitos1", MySqlDbType.VarChar).Value = leucocitos1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarHematologiaEspecialSinValidar(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
       string Comentario1,
       string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
       string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
       string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
       string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologiaEspecialSinValidar", con);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden1;
                cmd2.Parameters.Add("HoraValidacion1", MySqlDbType.VarChar).Value = HoraValidacion1;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis1;
                cmd2.Parameters.Add("Comentario1", MySqlDbType.LongText).Value = Comentario1;
                cmd2.Parameters.Add("Neutrofilos1", MySqlDbType.VarChar).Value = Neutrofilos1;
                cmd2.Parameters.Add("linfocitos1", MySqlDbType.VarChar).Value = linfocitos1;
                cmd2.Parameters.Add("Monocitos1", MySqlDbType.VarChar).Value = Monocitos1;
                cmd2.Parameters.Add("Eosinofilo1", MySqlDbType.VarChar).Value = Eosinofilo1;
                cmd2.Parameters.Add("Basofilos1", MySqlDbType.VarChar).Value = Basofilos1;
                cmd2.Parameters.Add("Hematies1", MySqlDbType.VarChar).Value = Hematies1;
                cmd2.Parameters.Add("Hemoglobina1", MySqlDbType.VarChar).Value = Hemoglobina1;
                cmd2.Parameters.Add("Hematocritos1", MySqlDbType.VarChar).Value = Hematocritos1;
                cmd2.Parameters.Add("VCM1", MySqlDbType.VarChar).Value = VCM1;
                cmd2.Parameters.Add("HCM1", MySqlDbType.VarChar).Value = HCM1;
                cmd2.Parameters.Add("CHCM1", MySqlDbType.VarChar).Value = CHCM1;
                cmd2.Parameters.Add("Plaquetas1", MySqlDbType.VarChar).Value = Plaquetas1;
                cmd2.Parameters.Add("Neutrofilos22", MySqlDbType.VarChar).Value = Neutrofilos22;
                cmd2.Parameters.Add("Linfocitos22", MySqlDbType.VarChar).Value = Linfocitos22;
                cmd2.Parameters.Add("Monocitos22", MySqlDbType.VarChar).Value = Monocitos22;
                cmd2.Parameters.Add("Usuario1", MySqlDbType.VarChar).Value = Usuario1;
                cmd2.Parameters.Add("Eosinofilos22", MySqlDbType.VarChar).Value = Eosinofilos22;
                cmd2.Parameters.Add("Basofilos22", MySqlDbType.VarChar).Value = Basofilos22;
                cmd2.Parameters.Add("leucocitos1", MySqlDbType.VarChar).Value = leucocitos1;
                con.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Exitoso";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un error" + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarHematologiasEspecial(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2}", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = con.CreateCommand();
                string Query2 = string.Format("UPDATE HemaEspecial SET  {0}  Where IdOrden = {1} ", cmd2, IdOrden);
                adapter.UpdateCommand.CommandText = Query2;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Etiquetas()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT Ordenes.Fecha,Ordenes.IdOrden,Ordenes.NumeroDia As N°,datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante as Apellidos, Convenios.Nombre AS NombreConvenio FROM datosrepresentante RIGHT JOIN(Convenios RIGHT JOIN Ordenes ON Convenios.IdConvenio = Ordenes.IDConvenio) ON datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante WHERE(((Ordenes.Muestra) = 0));", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ImprimirEtiquetas(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `Ordenes`.`Fecha`, `Ordenes`.`IdOrden`, `Ordenes`.`NumeroDia` AS N°, `datosrepresentante`.`Nombre`, `datosrepresentante`.`Apellidos`, `Convenios`.`Nombre` AS NombreConvenio, `AnalisisLaboratorio`.`Etiquetas`, `AnalisisLaboratorio`.`IdSeccion`, `AnalisisLaboratorio`.`Especiales` FROM `analisislaboratorio` join `resultadospaciente` on `resultadospaciente`.`IdAnalisis` = `analisislaboratorio`.`IdAnalisis` join `Ordenes` on `resultadospaciente`.`IdOrden` = `Ordenes`.`IdOrden` join convenios on `Ordenes`.`IdConvenio` = `Convenios`.`IdConvenio` join `datosrepresentante` on `datosrepresentante`.`idDatosRepresentante` = `resultadospaciente`.`IdPaciente` Where `resultadospaciente`.`IdOrden`= {0} and `AnalisisLaboratorio`.`Etiquetas` != '' order by IdSeccion asc;", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ContadorDeJeringas(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.totaljeringas Where fecha >= '{0}' and fecha <= '{1}';", cmd, cmd2), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarEtiqueta(int IdOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE Ordenes SET Muestra = 1  Where IdOrden = {0}", IdOrden);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Impreso Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Sede()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Empresas.Sede, Empresas.Activa FROM Empresas WHERE(((Empresas.Activa) = 1));"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ListaDeRespuesta(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                string query = string.Format("SELECT AnalisisLaboratorio.IdAnalisis, listaderespuestas.Respuesta FROM AnalisisLaboratorio INNER JOIN listaderespuestas ON AnalisisLaboratorio.IdAnalisis = listaderespuestas.IdAnalisis WHERE(((AnalisisLaboratorio.IdAnalisis) = {0}));", cmd);
                adapter.SelectCommand = new MySqlCommand(query, con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectProteinasTotalesYFraccionadas(int IDOrden)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis,AnalisisLaboratorio.NombreAnalisis,mayoromenorreferencial.ValorMenor,mayoromenorreferencial.ValorMayor,mayoromenorreferencial.Unidad,mayoromenorreferencial.IdEspecie,ValorResultado FROM Veterinaria.Resultadospaciente RIGHT join Ordenes on Ordenes.IdOrden = ResultadosPaciente.IdOrden LEFT join AnalisisLaboratorio on Resultadospaciente.IDAnalisis = analisislaboratorio.IDAnalisis RIGHT join mayoromenorreferencial on ResultadosPaciente.IDAnalisis = mayoromenorreferencial.IdAnalisis  RIGHT join DatosPaciente on DatosPaciente.IdDatosPaciente = Ordenes.IDdatosPaciente WHERE Resultadospaciente.IDOrden= {0} and (Resultadospaciente.IdAnalisis = 178 OR Resultadospaciente.IDAnalisis = 179 OR Resultadospaciente.IdAnalisis = 186 OR Resultadospaciente.IdAnalisis = 187) And mayoromenorreferencial.IdEspecie = DatosPaciente.IdEspecie   ", IDOrden), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet BioanalistasOrden(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, Usuarios.NombreUsuario, Usuarios.CB, Usuarios.MPPS, Usuarios.Cargo, Usuarios.FIRMA, Usuarios.SIGLAS FROM ResultadosPaciente INNER JOIN Usuarios ON ResultadosPaciente.IdUsuario = Usuarios.IdUsuario GROUP BY ResultadosPaciente.IdOrden, Usuarios.NombreUsuario, Usuarios.CB, Usuarios.MPPS, Usuarios.Cargo, Usuarios.FIRMA, Usuarios.IdUsuario, Usuarios.SIGLAS HAVING(((ResultadosPaciente.IdOrden) = {0}) AND((Usuarios.Cargo) = 3));", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectOrinas24(string Ordencmd, string Analisis1, string Analisis2, string Analisis3, string Analisis4)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, ResultadosPaciente.IdOrganizador, AnalisisLaboratorio.NombreAnalisis, ResultadosPaciente.ValorResultado, MayoroMenorReferencial.ValorMenor, MayoroMenorReferencial.ValorMayor, MayoroMenorReferencial.Unidad FROM(AnalisisLaboratorio LEFT JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) LEFT JOIN MayoroMenorReferencial ON AnalisisLaboratorio.IdAnalisis = MayoroMenorReferencial.IdAnalisis Left join Ordenes on ResultadosPaciente.IdOrden = ordenes.IdOrden left join DatosPaciente on Datospaciente.IDDatosPaciente = Ordenes.IDDatosPaciente WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1} Or(ResultadosPaciente.IdAnalisis) = {2} Or(ResultadosPaciente.IdAnalisis) = {3} Or(ResultadosPaciente.IdAnalisis) = {4})) and datospaciente.IDEspecie = mayoromenorreferencial.IdEspecie GROUP by IDAnalisis ORDER BY ResultadosPaciente.IdOrganizador;", Ordencmd, Analisis1, Analisis2, Analisis3, Analisis4), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string Fecha(DateTime FechaNacimiento)
        {
            string Fecha;
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();
            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int year;
            int month;
            int day;
            string Syear;
            string Smonth;
            string Sday;
            fromDate = FechaNacimiento;
            toDate = DateTime.Now;
            int increment = 0;

            increment = 0;
            if (fromDate.Day > toDate.Day)
            {
                increment = monthDay[fromDate.Month - 1];
            }
            if (increment == -1)
            {
                if (DateTime.IsLeapYear(fromDate.Year))
                {
                    increment = 29;
                }
                else
                {
                    increment = 28;
                }
            }
            if (increment != 0)
            {
                day = toDate.Day + increment - fromDate.Day;
                increment = 1;
            }
            else
            {
                day = toDate.Day - fromDate.Day;
            }
            if (fromDate.Month + increment > toDate.Month)
            {
                month = toDate.Month + 12 - (fromDate.Month + increment);
                increment = 1;
            }
            else
            {
                month = toDate.Month - (fromDate.Month + increment);
                increment = 0;
            }
            year = toDate.Year - (fromDate.Year + increment);

            if (day == 1)
            {
                Sday = string.Format("{0} Dia", day);
            }
            else
            {
                Sday = string.Format("{0} Dias", day);
            }
            if (month == 1)
            {
                Smonth = string.Format("{0} Mes", month);
            }
            else
            {
                Smonth = string.Format("{0} Meses", month);
            }
            if (year == 1)
            {
                Syear = string.Format("{0} Año", year);
            }
            else
            {
                Syear = string.Format("{0} Años", year);
            }
            if (year > 0)
            {
                Fecha = string.Format("{0}", Syear, Smonth, Sday);
            }
            else
            {
                Fecha = string.Format("{1}, {2}", Syear, Smonth, Sday);
            }



            return Fecha;
        }
        public static DataSet Usuarios()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("Select IdUsuario,NombreUsuario FROM Usuarios", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ConveniosUser()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT Convenios.IdConvenio, Convenios.Nombre,Convenios.Descuento FROM Convenios ", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet ConvenioActivo(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Convenios.Activos FROM Convenios WHERE Convenios.IdConvenio ={0}", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet PrivilegiosCargar(string UserID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM veterinaria.privilegios inner join usuarios on Usuarios.IdUsuario = privilegios.IdUsuario Where Usuarios.IdUsuario = {0};", UserID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarConvenio(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            string MS;
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                command = new MySqlCommand(string.Format("INSERT INTO Convenios (Nombre,Descuento,RIF,Empresa,Direccion,Telefono,Correo,Activos) Values ({0})", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                MS = "Agregado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                MS = ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
         public static string InsertarConvenio(Convenios convenio)
        {
            MySqlConnection con = new MySqlConnection(connection);
            string MS;
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                command = new MySqlCommand($"INSERT INTO Convenios (Nombre,Descuento,Activos) Values ('{convenio.Nombre}','{convenio.Descuento}',{convenio.Activos})", con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                MS = "Agregado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                MS = ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarConvenio(Convenios convenio)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = $@"UPDATE `veterinaria`.`convenios`
                SET
                `Nombre` = '{convenio.Nombre}',
                `Activos` = {convenio.Activos},
                `Descuento` = '{convenio.Descuento}'
                WHERE `IdConvenio` = {convenio.IdConvenio};";
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;

            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectConvenio(string UserID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Convenios.IdConvenio, Convenios.Nombre, Convenios.RIF, Convenios.Empresa, Convenios.Direccion, Convenios.Telefono, Convenios.Correo, Convenios.Descuento, Convenios.Activos FROM Convenios Where IdConvenio = {0}", UserID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                EventLog myLog = new EventLog();
                myLog.WriteEntry(ex.ToString(), EventLogEntryType.Error);
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarConvenio(int IDConvenio, string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE Convenios SET {1} WHERE(((Convenios.IdConvenio) = {0})); ", IDConvenio, cmd);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;

            }
            finally
            {
                con.Close();
            }
        }
        public static string BorrarConveniosDeUsuario(int IdUsuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.DeleteCommand = con.CreateCommand();
                string Query = string.Format("DELETE FROM `Veterinaria`.`conveniosporusuario` WHERE ConveniosPorUsuario.IdUsuario = {0}; ", IdUsuario);
                adapter.DeleteCommand.CommandText = Query;
                adapter.DeleteCommand.ExecuteNonQuery();
                string MS = "Eliminado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string BorrarConvenios(string IDConvenio, string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.DeleteCommand = con.CreateCommand();
                string Query = string.Format("DELETE ConveniosPorUsuario.IdConvenio, ConveniosPorUsuario.IdUsuario FROM ConveniosPorUsuario WHERE(((ConveniosPorUsuario.IdConvenio) = {0}) AND((ConveniosPorUsuario.IdUsuario) = {1})); ", IDConvenio, cmd);
                adapter.DeleteCommand.CommandText = Query;
                adapter.DeleteCommand.ExecuteNonQuery();
                string MS = "Eliminado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarConvenioUsuario(int IdUsuario,int IdConvenio)
        {
            MySqlConnection con = new MySqlConnection(connection);
            string MS;
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                command = new MySqlCommand($"INSERT INTO ConveniosPorUsuario ( IdConvenio, IdUsuario ) Values ({IdConvenio},{IdUsuario})", con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                MS = "Agregado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                MS = ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarPrivilegios(string cmd, string IdUsuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE Privilegios SET  {0}  Where IdUsuario = {1} ", cmd, IdUsuario);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string InsertarUsuario(string cmd, Bitmap image, string cargo, bool Guardar)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int Usuario;
            try
            {
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, con);
                con.Open();
                command = new MySqlCommand(string.Format("INSERT INTO Usuarios (NombreUsuario, Contraseña, Cargo, CB, MPPS, SIGLAS, Activo ) Values ({0})", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                Usuario = Convert.ToInt32(cmd2.ExecuteScalar());
                if (Guardar == true)
                {
                    image.Save(string.Format("Firma\\{0}.jpg", Usuario));
                    adapter.UpdateCommand = con.CreateCommand();
                    string Query = string.Format("UPDATE Usuarios SET Usuarios.FIRMA = '{0}.jpg' WHERE(((Usuarios.IdUsuario) = {0})); ", Usuario);
                    adapter.UpdateCommand.CommandText = Query;
                    adapter.UpdateCommand.ExecuteNonQuery();
                    command = new MySqlCommand(string.Format("INSERT INTO TeclasPorUsuario (IdUsuario,Leucocitos,Neutrofilos,Linfocitos, Monocitos, Eosinofilos, Basofilos, Plaquetas) VALUES ({0},'l','n','m','v','b','c','p')", Usuario), con);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
                command = new MySqlCommand(string.Format("INSERT INTO ConveniosPorUsuario ( IdConvenio, IdUsuario ) Values (1,{0})", Usuario), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();

                string MS = "Agregado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectCargo()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT Cargos.IDCargo, Cargos.NombreCargo, Cargos.Descripcion FROM Cargos WHERE(((Cargos.Visible) = '1'));", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet CargarUsuario(string UserID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Usuarios.IdUsuario, Usuarios.FIRMA, Usuarios.NombreUsuario, Usuarios.Contraseña, Usuarios.Cargo, Usuarios.CB, Usuarios.MPPS, Usuarios.SIGLAS, Usuarios.UsarFirma, Usuarios.Activo FROM Usuarios WHERE(((Usuarios.IdUsuario) = {0})); ", UserID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet Contrasena(string UserID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Usuarios.Contraseña FROM Usuarios WHERE(((Usuarios.Contraseña) = '{0}')); ", UserID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static int SesionTemporal()
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {

                int idOrden = -1;
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID();";
                MySqlCommand cmd2 = new MySqlCommand(Query2, con);
                con.Open();
                //Ejemplo {0}=(2,'7/3/2020','2:17',1,1,NumeroDia) 
                string Time = DateTime.Now.ToString("yyyy/MM/dd");
                command = new MySqlCommand(string.Format("INSERT INTO TempOrdenes (Fecha) Values ('{0}')", Time), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                idOrden = Convert.ToInt32(cmd2.ExecuteScalar());
                return idOrden;
            }

            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return 0;
            }
            finally
            {
                con.Close();
            }
        }
        public static string PerfilTemporal(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                string MS = "";
                con.Open();
                DataSet ds = new DataSet();
                command = new MySqlCommand(string.Format("INSERT INTO PerfilesAFacturar (IdSesion,IdPerfil,PrecioPerfil,ID) Values ({0})", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();//Ejemplo {0}=(2,'7/3/2020','2:17',1,1,NumeroDia) 
                return MS;
            }

            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Error: " + ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string BorrarTemporal(string cmd, string cmd2)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.DeleteCommand = con.CreateCommand();
                string Query = string.Format("DELETE IDPerfilFacturado,IdSesion,IdPerfil,PrecioPerfil FROM PerfilesAfacturar WHERE(((IdSesion) = {0}) AND((IdPerfil) = {1})); ", cmd, cmd2);
                adapter.DeleteCommand.CommandText = Query;
                adapter.DeleteCommand.ExecuteNonQuery();
                string MS = "Eliminado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarTemporal(string IdOrden, string Sesion)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE PerfilesAFacturar SET {0} WHERE (((PerfilesAFacturar.IdSesion)={1})); ", IdOrden, Sesion);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarIDTemporal(string IdOrden, string Sesion)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE PerfilesAFacturar SET {0} WHERE (((PerfilesAFacturar.IdSesion)={1})); ", IdOrden, Sesion);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarTeclas(string cmd, int IdUsuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE TeclasPorUsuario SET {0} WHERE (((TeclasPorUsuario.IdUsuario)={1})); ", cmd, IdUsuario);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet AnalisisTemporal(int UserID)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT AnalisisLaboratorio.NombreAnalisis FROM AnalisisLaboratorio INNER JOIN(PerfilesAFacturar INNER JOIN PerfilesAnalisis ON PerfilesAFacturar.IdPerfil = PerfilesAnalisis.IdPerfil) ON AnalisisLaboratorio.IdAnalisis = PerfilesAnalisis.IdAnalisis WHERE(((PerfilesAFacturar.IdSesion) = {0})) order by IdOrganizador;", UserID), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }

        public static DataSet BuscarAnalisisTemporal(string Sesion, string Analisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.buscaranalisistemporal WHERE(((AnalisisLaboratorio.NombreAnalisis) = '{1}') AND((PerfilesAFacturar.IdSesion) = {0}));", Sesion, Analisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string AgregarAnalisis(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int Analisis;
            try
            {
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, con);
                con.Open();
                command = new MySqlCommand(string.Format("INSERT INTO Usuarios (NombreUsuario, Contraseña, Cargo, CB, MPPS, SIGLAS, Activo ) Values ({0})", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                Analisis = Convert.ToInt32(cmd2.ExecuteScalar());
                command = new MySqlCommand(string.Format("INSERT INTO ConveniosPorUsuario ( IdConvenio, IdUsuario ) Values (1,{0})", Analisis), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                Analisis = Convert.ToInt32(cmd2.ExecuteScalar());
                command = new MySqlCommand(string.Format("INSERT INTO Privilegios ( ImprimirResultado, ReimprimirResultado, Validar, Modificar, AgregarConvenio, QuitarConvenio, VerLibroVenta, VerCierreCaja, VerOrdenes, VerEstadisticas, VerReporteBioanalista, VerReferidos, ImprimirFactura, ReImprimirFactura, AgregarUsuario, ModificarUsuario, CambioDePrecios, ImprimirEtiqueta,TeclasHematologia, IdUsuario ) SELECT PrivilegiosCargo.ImprimirResultado, PrivilegiosCargo.ReimprimirResultado, PrivilegiosCargo.Validar, PrivilegiosCargo.Modificar, PrivilegiosCargo.AgregarConvenio, PrivilegiosCargo.QuitarConvenio, PrivilegiosCargo.VerLibroVenta, PrivilegiosCargo.VerCierreCaja, PrivilegiosCargo.VerOrdenes, PrivilegiosCargo.VerEstadisticas, PrivilegiosCargo.VerReporteBioanalista, PrivilegiosCargo.VerReferidos, PrivilegiosCargo.ImprimirFactura, PrivilegiosCargo.ReImprimirFactura, PrivilegiosCargo.AgregarUsuario, PrivilegiosCargo.ModificarUsuario, PrivilegiosCargo.CambioDePrecios, PrivilegiosCargo.ImprimirEtiqueta,PrivilegiosCargo.TeclasHematologia, {0} FROM PrivilegiosCargo WHERE(((PrivilegiosCargo.IDCargo) = {0}))"), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                command = new MySqlCommand(string.Format("INSERT INTO TeclasPorUsuario (IdUsuario,Leucocitos,Neutrofilos,Linfocitos, Monocitos, Eosinofilos, Basofilos, Plaquetas) VALUES ({0},'l','n','m','v','b','c','p')"), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                string MS = "Agregado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = ex.ToString();
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static int InsertarReferido(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                command = new MySqlCommand(string.Format("Insert INTO Referidos (IDOrden,Sede) Values ({0})", cmd), con);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                int MS = 1;
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }

        public static DataSet EnviarReferidosAutomatico()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Convenios.Sede, ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante as  as Apellidos, ResultadosPaciente.IdConvenio, ResultadosPaciente.ValorResultado, Ordenes.Usuario FROM datosrepresentante INNER JOIN((Ordenes INNER JOIN Convenios ON Ordenes.IDConvenio = Convenios.IdConvenio) INNER JOIN ResultadosPaciente ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(Convenios.IdConvenio = ResultadosPaciente.IdConvenio)) ON(datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente) AND(datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) WHERE(((Convenios.Sede) = '1')); "), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectTemp(int IdOrden, int IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Resultadospaciente.IdOrden,Ordenes.NumeroDia,datosrepresentante.NombreRepresentante,Apellidos,Sexo,Convenios.Nombre,Ordenes.Fecha,Ordenes.HoraIngreso,NombreUsuario,HoraValidacion,PrecioF FROM Veterinaria.Ordenes inner join resultadospaciente on Ordenes.IdOrden = resultadospaciente.IdOrden inner join datosrepresentante on resultadospaciente.IdPaciente = datosrepresentante.idDatosRepresentante inner join convenios on Convenios.IDConvenio = resultadospaciente.IdConvenio inner join Usuarios on Ordenes.Usuario = Usuarios.IdUsuario where resultadospaciente.IdOrden = {0} and IdAnalisis = {1};", IdOrden, IdAnalisis), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet InformacionReferidoEnviada(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, CONCAT(datosrepresentante.TipoRepresentante,' ',datosrepresentante.RIF) as Cedula, datosrepresentante.NombreRepresentante, datosrepresentante.ApellidoRepresentante as  as Apellidos, ResultadosPaciente.IdConvenio, ResultadosPaciente.ValorResultado, Ordenes.Usuario FROM(datosrepresentante INNER JOIN ResultadosPaciente ON datosrepresentante.idDatosRepresentante = ResultadosPaciente.IdPaciente) INNER JOIN Ordenes ON(Ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datosrepresentante.idDatosRepresentante = Ordenes.idDatosRepresentante) WHERE(((ResultadosPaciente.IdOrden) = {0}));", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet CorreoEmpresa()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Empresas.Nombre,Empresas.Telefono, Empresas.Correo, Empresas.Clave, Empresas.Puerto FROM Empresas WHERE(((Empresas.Activa) = 1))"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet CorreoConvenio(string Correo)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Convenios.CorreoSede, Convenios.Sede FROM Convenios WHERE(((Convenios.IdConvenio) = {0}) AND((Convenios.Sede) = 1));", Correo), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SELECTMAC(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Equipos.MAC FROM Equipos WHERE(((Equipos.MAC) = '{0}'));", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SelectOrdenesEnviar()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado FROM AnalisisLaboratorio LEFT JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis WHERE(((AnalisisLaboratorio.Especiales) = '1')) and IdOrden is not Null GROUP BY ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado HAVING (ResultadosPaciente.Recibido is Null or ResultadosPaciente.Recibido = '0') AND (ResultadosPaciente.Enviado is Null or ResultadosPaciente.Enviado = '0')", con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;


            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet OrdenesSeleccionadas(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `resultadospaciente`.`IdOrden`, `Ordenes`.`Usuario` as IdUsuario, `Ordenes`.`Fecha` AS FechaOrden,`resultadospaciente`.`IdAnalisis`, `resultadospaciente`.`IdOrganizador`,  `datosrepresentante`.`Cedula`, `datosrepresentante`.`Nombre`, `datosrepresentante`.`Apellidos`, `datosrepresentante`.`Celular`, `datosrepresentante`.`Telefono`,  `datosrepresentante`.`Sexo`,`datosrepresentante`.`Fecha`, `datosrepresentante`.`Correo`, `datosrepresentante`.`TipoCorreo`, `datosrepresentante`.`TipoCelular`, `datosrepresentante`.`TipoTelefono`, `AnalisisLaboratorio`.`Especiales`, `ResultadosPaciente`.`Enviado`, `ResultadosPaciente`.`Recibido` FROM(`datosrepresentante` JOIN `Ordenes` ON `datosrepresentante`.`idDatosRepresentante` = `Ordenes`.`idDatosRepresentante`) JOIN(`AnalisisLaboratorio` JOIN `ResultadosPaciente` ON `AnalisisLaboratorio`.`IdAnalisis` = `ResultadosPaciente`.`IdAnalisis`) ON(`Ordenes`.`IdOrden` = `ResultadosPaciente`.`IdOrden`) AND(`datosrepresentante`.`idDatosRepresentante` = `ResultadosPaciente`.`IdPaciente`) Where `ResultadosPaciente`.`IdOrden` = {0} AND `AnalisisLaboratorio`.`Especiales` = '1' AND (`ResultadosPaciente`.`Enviado` is Null OR `ResultadosPaciente`.`Enviado` = 0) AND(`ResultadosPaciente`.`Recibido` is Null OR `ResultadosPaciente`.`Recibido` = 0)GROUP BY `ResultadosPaciente`.`IdOrden`, `Ordenes`.`Usuario`, `Ordenes`.`Fecha`,  `ResultadosPaciente`.`IdAnalisis`,`ResultadosPaciente`.`IdOrganizador`, `datosrepresentante`.`Cedula`, `datosrepresentante`.`Nombre`, `datosrepresentante`.`Apellidos`, `datosrepresentante`.`Celular`, `datosrepresentante`.`Telefono`, `datosrepresentante`.`Sexo`,`datosrepresentante`.`Fecha`, `datosrepresentante`.`Correo`, `datosrepresentante`.`TipoCorreo`, `datosrepresentante`.`TipoCelular`, `datosrepresentante`.`TipoTelefono`, `AnalisisLaboratorio`.`Especiales`, `ResultadosPaciente`.`Enviado`, `ResultadosPaciente`.`Recibido` ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarEnviado(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE AnalisisLaboratorio INNER JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis SET ResultadosPaciente.Enviado = 1 WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((AnalisisLaboratorio.Especiales) = '1')); ", cmd);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static int ConvenioPorCorreo(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int IDConvenio = 0;
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Convenios.IdConvenio FROM Convenios WHERE(((Convenios.CorreoSede) = '{0}')); ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();

                IDConvenio = Convert.ToInt32(ds.Tables[0].Rows[0]["IdConvenio"].ToString());
                return IDConvenio;


            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return IDConvenio;
            }
            finally
            {
                con.Close();
            }
        }
        public static int BuscarNumeroOrden(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int IDConvenio = 0;
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(IdOrden) as Contador FROM Ordenes WHERE(((Ordenes.IdOrden) =  {0})); ", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                IDConvenio = Convert.ToInt32(ds.Tables[0].Rows[0]["Contador"].ToString());
                return IDConvenio;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return IDConvenio;
            }
            finally
            {
                con.Close();
            }
        }
        public static int BurcarOrdenCruzada(string IDOrden, string Serial)
        {
            MySqlConnection con = new MySqlConnection(connection);
            int IDConvenio = 0;
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(IdOrden) as Contador FROM Veterinaria.pagos Where IdOrden ={0} and IdTipoDePago = 9 and Serial = '{1}';", IDOrden, Serial), con);
                adapter.Fill(ds);
                adapter.Dispose();
                IDConvenio = Convert.ToInt32(ds.Tables[0].Rows[0]["Contador"].ToString());
                return IDConvenio;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return IDConvenio;
            }
            finally
            {
                con.Close();
            }
        }
        public static string BuscarFechaOrden(string IDOrden, string Serial)
        {
            MySqlConnection con = new MySqlConnection(connection);
            string IDConvenio;
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha FROM Veterinaria.pagos Where IdOrden = {0}  and Serial = '{1}';  ", IDOrden, Serial), con);
                adapter.Fill(ds);
                adapter.Dispose();
                IDConvenio = ds.Tables[0].Rows[0]["Contador"].ToString();
                return IDConvenio;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return IDConvenio = "";
            }
            finally
            {
                con.Close();
            }
        }
        public static string ActualizarOrdenReferido(string IdOrden, string IdAnalisis)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                con.Open();
                adapter.UpdateCommand = con.CreateCommand();
                string Query = string.Format("UPDATE Referidos SET Referidos.IdOrdenSede = {0} WHERE(((Referidos.IdOrden) = '{1}')); ", IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                string MS = "Guardado Exitosamente";
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                string MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SeleccionOrdenesAEnviar()
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado FROM AnalisisLaboratorio LEFT JOIN ResultadosPaciente ON AnalisisLaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis WHERE(((AnalisisLaboratorio.Especiales) = '1')) and IdOrden is not Null GROUP BY ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado HAVING(((ResultadosPaciente.Recibido) is null  Or ResultadosPaciente.Recibido = 0) AND((ResultadosPaciente.Enviado) is null  Or ResultadosPaciente.Enviado = 0))"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SeleccionResultadosAEnviar(string cmd)
        {
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `referidos`.`IdOrden`,`referidos`.`IdOrdenSede`, `resultadospaciente`.`IdAnalisis`, `resultadospaciente`.`ValorResultado`, `resultadospaciente`.`Comentario`,`resultadospaciente`.`HoraValidacion`, `resultadospaciente`.`IdUsuario` FROM `Veterinaria`.`referidos` join `resultadosPaciente` on `referidos`.`IdOrdenSede` = `resultadosPaciente`.`IdOrden` Where `resultadospaciente`.`IdConvenio` = {0} And `resultadospaciente`.`IdEstadoDeResultado` >= 2 and `resultadospaciente`.`Enviado` Is null Group by IdOrden,IdAnalisis", cmd), con);
                adapter.Fill(ds);
                adapter.Dispose();
                return ds;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ds;
            }
            finally
            {
                con.Close();
            }
        }






        // Fincas
        public static int InsertarFinca(Finca finca)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("INSERT INTO `veterinaria`.`fincas` (`RIF`,`NombreFinca`,`idEstado`,`idCiudad`,`IdMunicipio`,`IdParroquia`)" +
                    $" VALUES ('{finca.RIF}','{finca.NombreFinca}',{finca.estado.id_estado},{finca.ciudad.id_ciudad},{finca.municipio.id_municipio},{finca.parroquia.id_parroquia})"
                    , conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int ActualizarFinca(Finca finca)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("UPDATE `veterinaria`.`fincas`"
                    + " SET "
                    + $" `RIF` =  '{finca.RIF}' ,"
                    + $" `NombreFinca` = '{finca.NombreFinca}' ,"
                    + $" `idEstado` = {finca.estado.id_estado} , "
                    + $" `idCiudad` = {finca.ciudad.id_ciudad} , "
                    + $" `IdMunicipio` = {finca.municipio.id_municipio} , "
                    + $" `IdParroquia` = {finca.parroquia.id_parroquia} "
                    + $" WHERE `IdFincas` = {finca.IdFinca}; "
                    , conn);
                adapter.UpdateCommand = command;
                adapter.UpdateCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }

        public static List<Finca> selectFincas()
        {
            List<Finca> fincas = new List<Finca>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.fincas"), con);
                adapter.Fill(ds);
                adapter.Dispose();

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return fincas;
            }
            finally
            {
                con.Close();
            }

            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        Finca finca = new Finca();
                        finca.IdFinca = (int)r["idFincas"];
                        finca.RIF = r["RIF"].ToString();
                        finca.NombreFinca = r["NombreFinca"].ToString();
                        finca.estado = selectEstado((int)r["idEstado"]);
                        finca.ciudad = selectCiudad((int)r["idCiudad"]);
                        finca.municipio = selectMunicipio((int)r["IdMunicipio"]);
                        finca.parroquia = selectParroquia((int)r["IdParroquia"]);
                        fincas.Add(finca);
                    }
                }
            }
            return fincas;
        }
        public static Finca selectFinca(int idFinca)
        {
            int _IdFinca = 0;
            Finca Finca = new Finca();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Fincas where idFincas = {0};", idFinca), con);
                adapter.Fill(ds);
                adapter.Dispose();
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Finca;
            }
            finally
            {
                con.Close();
            }
            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int.TryParse(ds.Tables[0].Rows[0]["idFincas"].ToString(), out _IdFinca);
                    Finca.IdFinca = _IdFinca;
                    Finca.NombreFinca = ds.Tables[0].Rows[0]["NombreFinca"].ToString();
                    Finca.estado = selectEstado((int)ds.Tables[0].Rows[0]["idEstado"]);
                    Finca.ciudad = selectCiudad((int)ds.Tables[0].Rows[0]["idCiudad"]);
                    Finca.municipio = selectMunicipio((int)ds.Tables[0].Rows[0]["IdMunicipio"]);
                    Finca.parroquia = selectParroquia((int)ds.Tables[0].Rows[0]["IdParroquia"]);
                }
            }
            return Finca;

        }

        // Estados
        public static List<Estado> selectEstados()
        {

            List<Estado> estados = new List<Estado>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Estados"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Estado estado = new Estado();
                            estado.id_estado = (int)r["id_estado"];
                            estado.estado = r["estado"].ToString();
                            estados.Add(estado);

                        }
                    }
                }
                return estados;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return estados;
            }
            finally
            {
                con.Close();
            }
        }
        public static Estado selectEstado(int idEstado)
        {

            Estado estado = new Estado();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.estados where id_estado = {0};", idEstado), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {

                        estado.id_estado = (int)ds.Tables[0].Rows[0]["id_estado"];
                        estado.estado = ds.Tables[0].Rows[0]["estado"].ToString();
                    }
                }
                return estado;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return estado;
            }
            finally
            {
                con.Close();
            }
        }

        // Ciudades
        public static List<Ciudades> selectCiudades()
        {

            List<Ciudades> ciudades = new List<Ciudades>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.ciudades"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Ciudades ciudad = new Ciudades();
                            ciudad.id_ciudad = (int)r["id_ciudad"];
                            ciudad.ciudad = r["ciudad"].ToString();
                            ciudades.Add(ciudad);

                        }

                    }
                }
                return ciudades;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ciudades;
            }
            finally
            {
                con.Close();
            }
        }
        public static Ciudades selectCiudad(int idCiudad)
        {

            Ciudades ciudad = new Ciudades();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.ciudades where id_ciudad = {0};", idCiudad), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        ciudad.id_ciudad = (int)ds.Tables[0].Rows[0]["id_ciudad"];
                        ciudad.ciudad = ds.Tables[0].Rows[0]["ciudad"].ToString();
                    }
                }
                return ciudad;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ciudad;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Ciudades> selectCiudadesPorEstado(int idEstado)
        {

            List<Ciudades> ciudades = new List<Ciudades>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM Veterinaria.ciudades where id_estado ={idEstado}", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Ciudades ciudad = new Ciudades();
                            ciudad.id_ciudad = (int)r["id_ciudad"];
                            ciudad.ciudad = r["ciudad"].ToString();
                            ciudades.Add(ciudad);

                        }

                    }
                }
                return ciudades;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return ciudades;
            }
            finally
            {
                con.Close();
            }
        }


        // Municipios
        public static List<Municipio> selectMunicipios()
        {
            List<Municipio> municipios = new List<Municipio>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Municipios"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Municipio municipio = new Municipio();
                            municipio.id_municipio = (int)r["id_municipio"];
                            municipio.municipio = r["municipio"].ToString();
                            municipios.Add(municipio);

                        }

                    }
                }
                return municipios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return municipios;
            }
            finally
            {
                con.Close();
            }
        }
        public static Municipio selectMunicipio(int idMunicipio)
        {

            Municipio municipio = new Municipio();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.municipios where id_municipio = {0};", idMunicipio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        municipio.id_municipio = (int)ds.Tables[0].Rows[0]["id_municipio"];
                        municipio.municipio = ds.Tables[0].Rows[0]["municipio"].ToString();
                    }
                }
                return municipio;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return municipio;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Municipio> selectMunicipiosPorEstado(int idEstado)
        {
            List<Municipio> municipios = new List<Municipio>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM Veterinaria.Municipios where id_estado ={idEstado}", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Municipio municipio = new Municipio();
                            municipio.id_municipio = (int)r["id_municipio"];
                            municipio.municipio = r["municipio"].ToString();
                            municipios.Add(municipio);

                        }

                    }
                }
                return municipios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return municipios;
            }
            finally
            {
                con.Close();
            }
        }

        // Parroquias
        public static List<Parroquia> selectParroquias()
        {
            List<Parroquia> parroquias = new List<Parroquia>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.parroquias"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Parroquia parroquia = new Parroquia();
                            parroquia.id_parroquia = (int)r["id_parroquia"];
                            parroquia.parroquia = r["parroquia"].ToString();
                            parroquias.Add(parroquia);

                        }
                    }
                }
                return parroquias;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return parroquias;
            }
            finally
            {
                con.Close();
            }
        }
        public static Parroquia selectParroquia(int idParroquia)
        {

            Parroquia parroquia = new Parroquia();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.parroquias where id_parroquia = {0};", idParroquia), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        parroquia.id_parroquia = (int)ds.Tables[0].Rows[0]["id_parroquia"];
                        parroquia.parroquia = ds.Tables[0].Rows[0]["parroquia"].ToString();
                    }
                }
                return parroquia;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return parroquia;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<Parroquia> selectParroquiasPorMunicipio(int idMunicipio)
        {
            List<Parroquia> parroquias = new List<Parroquia>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM Veterinaria.parroquias where id_municipio = {idMunicipio}", con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Parroquia parroquia = new Parroquia();
                            parroquia.id_parroquia = (int)r["id_parroquia"];
                            parroquia.parroquia = r["parroquia"].ToString();
                            parroquias.Add(parroquia);

                        }
                    }
                }
                return parroquias;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return parroquias;
            }
            finally
            {
                con.Close();
            }
        }


        /// Veterinario

        public static List<Veterinario> selectVeterinarios()
        {
            List<Veterinario> veterinarios = new List<Veterinario>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Veterinario"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Veterinario veterinario = new Veterinario();
                            veterinario.IdVeterinario = (int)r["IdVeterinario"];
                            veterinario.NombreVeterinario = r["NombreVeterinario"].ToString();
                            veterinario.RIF = r["RIF"].ToString();
                            veterinarios.Add(veterinario);

                        }
                    }
                }
                return veterinarios;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return veterinarios;
            }
            finally
            {
                con.Close();
            }
        }
        public static Veterinario selectVeterinario(int idVeterinario)
        {

            Veterinario veterinario = new Veterinario();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Veterinario where idVeterinario = {0};", idVeterinario), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        veterinario.NombreVeterinario = ds.Tables[0].Rows[0]["NombreVeterinario"].ToString();
                        veterinario.RIF = ds.Tables[0].Rows[0]["RIF"].ToString();
                    }
                }
                return veterinario;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return veterinario;
            }
            finally
            {
                con.Close();
            }
        }
        public static int InsertarVeterinario(Veterinario veterinario)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("INSERT INTO `veterinaria`.`Veterinario` (`RIF`,`NombreVeterinario`)" +
                    $" VALUES ('{veterinario.RIF}','{veterinario.NombreVeterinario}')"
                    , conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int ActualizarVeterinario(Veterinario veterinario)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("UPDATE `veterinaria`.`fincas`"
                    + " SET "
                    + $" `RIF` =  '{veterinario.RIF}' ,"
                    + $" `NombreVeterinario` = '{veterinario.NombreVeterinario}'"
                    + $" WHERE `idVeterinario` = {veterinario.IdVeterinario}; "
                    , conn);
                adapter.UpdateCommand = command;
                adapter.UpdateCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }


        //Especies
        public static List<Especies> selectEspecies()
        {
            List<Especies> especies = new List<Especies>();

            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.Especies"), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow r in ds.Tables[0].Rows)
                        {
                            Especies especie = new Especies();
                            especie.IdEspecie = (int)r["IdEspecie"];
                            especie.Descripcion = r["Descripcion"].ToString();
                            especie.hemoParasitos = selectHemoparasitosPorÉspecie(especie.IdEspecie);
                            especies.Add(especie);

                        }
                    }
                }
                return especies;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return especies;
            }
            finally
            {
                con.Close();
            }
        }
        public static Especies selectEspeciePorId(int IdEspecie)
        {
            Especies especie = new Especies();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Veterinaria.especies where IdEspecie = {0};", IdEspecie), con);
                adapter.Fill(ds);
                adapter.Dispose();
                con.Close();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        especie.IdEspecie = (int)ds.Tables[0].Rows[0]["IdEspecie"];
                        especie.Descripcion = ds.Tables[0].Rows[0]["Descripcion"].ToString();
                    }
                }
                return especie;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return especie;
            }
            finally
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
            }
        }
        public static int InsertarEspecie(Especies especie)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("INSERT INTO `veterinaria`.`especies` (`Descripcion`)" +
                    $" VALUES ('{especie.Descripcion}')"
                    , conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int ActualizarEspecie(Especies especie)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("UPDATE `veterinaria`.`especies`"
                    + " SET "
                    + $" `Descripcion` =  '{especie.Descripcion}' ,"
                    + $" WHERE `IdEspecie` = {especie.IdEspecie}; "
                    , conn);
                adapter.UpdateCommand = command;
                adapter.UpdateCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }


        //Hemoparasitos
        public static int InsertarHemoparasito(Hemo hemo, int IdEspecie)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("INSERT INTO `veterinaria`.`hemoparasitos` (`Descripcion`,`IDEspecie`)" +
                    $" VALUES ('{hemo.Descripcion}','{IdEspecie}')"
                    , conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                command = new MySqlCommand("SELECT LAST_INSERT_ID();", conn);
                int.TryParse(command.ExecuteScalar().ToString(), out idPaciente);
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int ActualizarHemoparasito(Hemo hemo, int IdEspecie)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("UPDATE `veterinaria`.`hemoparasitos`"
                    + " SET "
                    + $" `Descripcion` =  '{hemo.Descripcion}' "
                    + $" WHERE `idHemoparasitos` = {hemo.Id} "
                     + $" and `IdEspecie` = {IdEspecie}; "
                    , conn);
                adapter.UpdateCommand = command;
                adapter.UpdateCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }
        public static int BorrarHemoparasito(Hemo hemo)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand("DELETE FROM `veterinaria`.`hemoparasitos`"
                    + $" WHERE `idHemoparasitos` = {hemo.Id} "
                    , conn);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
                idPaciente = 1;
                return idPaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }

        public static int ActualizarUsuario(Usuarios usuario)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                command = new MySqlCommand($"UPDATE `veterinaria`.`usuarios` SET `FIRMA` = '{usuario.Firma}',`NombreUsuario` = '{usuario.NombreUsuario}',`Contraseña` = '{usuario.Contraseña}',`Cargo` = {usuario.Cargo}, `CB` ='{usuario.CB}' , `MPPS` = '{usuario.MPPS}', `SIGLAS` =  '{usuario.SIGLAS}', `Activo` = {usuario.Activo} WHERE `IdUsuario` =  {usuario.IdUsuario}", con);
                adapter.UpdateCommand = command;
                adapter.UpdateCommand.ExecuteNonQuery();
                int MS = 1;
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return MS;
            }
            finally
            {
                con.Close();
            }
        }
    }
}

