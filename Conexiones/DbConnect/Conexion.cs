using System;
using System.Data;
using System.Configuration;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using MySqlConnector;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Conexiones.Dto;
using Conexiones.Modelos;
using DocumentFormat.OpenXml.Office.Word;
using DocumentFormat.OpenXml.ExtendedProperties;
using System.Runtime.ConstrainedExecution;

namespace Conexiones.DbConnect
{
    public class Conexion
    {
        private static string server = "";
        private static string database = "sistema2020";
        private static string uid = "root";
        private static string password = "Labordono1818";
        private static string connection = "SERVER=" + server + ";" + "DATABASE=" +
            database + ";" + "UID=" + uid + ";" + "PASSWORD=" + password + ";";
        private static MySqlConnection con = new MySqlConnection(connection);
        private static MySqlDataAdapter adapter = new MySqlDataAdapter();
        private static MySqlCommand command = new MySqlCommand();

        public static bool activo, proceso1 = false, proceso2 = false, proceso3 = false;
        public static void Connection(string cmd, string Nombre)
        {
            server = ConfigurationManager.ConnectionStrings[Nombre].ConnectionString;
            string cmd2 = string.Format("Puerto{0}", Nombre);
            string puerto = ConfigurationManager.ConnectionStrings[cmd2].ConnectionString;
            connection = connection = string.Format("Server={0};Port={4};Database={1};Uid={2};Pwd={3};Connection Timeout=20;Allow User Variables=True", server, database, uid, password, puerto);
            con = new MySqlConnection(connection);
        }
        public static string TestConnection()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                return "Conectado";
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return "Desconectado";
            }
            finally
            {
                conn.Close();
            }

        }

        public static int ActualizarAnalisis(AnalisisLaboratorio analisis)
        {
            int MS = 0;
            int Visible = 0;
            if (analisis.Visible)
            {
                Visible = 1;
            }
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                command = new MySqlCommand($@"UPDATE `sistema2020`.`analisislaboratorio` SET
                `NombreAnalisis` = '{analisis.NombreAnalisis}',`TipoAnalisis` = {analisis.TipoAnalisis},
                `Visible` = {Visible},`Etiquetas` = '{analisis.Etiqueta}',`IdSeccion` = {analisis.IdSeccion},
                `Especiales` = {analisis.Especiales}
                WHERE `IdAnalisis` = {analisis.IdAnalisis};", con);
                adapter.UpdateCommand = command;
                adapter.UpdateCommand.ExecuteNonQuery();
                MS = 1;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                MS = 0;
            }
            finally
            {
                con.Close();
            }
            MS = ActualizarValoresDeReferencia(analisis.valoresDeReferencia);
            return MS;
        }
        public static int ActualizarValoresDeReferencia(mayoromenorreferencial ValorRef)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                command = new MySqlCommand($@"UPDATE `sistema2020`.`mayoromenorreferencial` SET
                      `ValorMenor` = {ValorRef.ValorMenor},`ValorMayor` = {ValorRef.ValorMayor},
                       `Unidad` = '{ValorRef.Unidad}',`MultiplesValores` ='{ValorRef.MultiplesValores}',`Lineas` = {ValorRef.lineas}
                WHERE `IdReferencial` = {ValorRef.IdAnalisis};", con);
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
        public static int ActualizarPerfil(Perfil perfil)
        {
            MySqlConnection con = new MySqlConnection(connection);
            try
            {
                con.Open();
                command = new MySqlCommand($"UPDATE `sistema2020`.`perfil` SET `NombrePerfil` = '{perfil.NombrePerfil}', `Precio` = '{perfil.Precio}', `PrecioDolar` = {perfil.PrecioDolar}, `Activo` = {perfil.Activo} WHERE `idPerfil` = {perfil.IdPerfil};", con);
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
        public static Perfil selectPerfil(int IdPerfil)
        {
            Perfil perfil = new Perfil();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM `Sistema2020`.`Perfil` WHERE `IdPerfil` = {IdPerfil}; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    perfil.IdPerfil = (int)ds.Rows[0]["IdPerfil"];
                    perfil.NombrePerfil = ds.Rows[0]["NombrePerfil"].ToString();
                    perfil.Precio = ds.Rows[0]["Precio"].ToString();
                    perfil.PrecioDolar = Convert.ToDouble(ds.Rows[0]["PrecioDolar"].ToString());
                    perfil.Activo = Convert.ToInt32(ds.Rows[0]["Activo"].ToString());
                };
                return perfil;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return perfil;
            }
            finally
            {
                conn.Close();
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
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM `Sistema2020`.`Perfil` ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
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
                conn.Close();
            }
        }
        //public static Privilegios selectPrivilegios(Privilegios privilegios)
        //{

        //    MySqlConnection conn = new MySqlConnection(connection);
        //    DataTable ds = new DataTable();
        //    try
        //    {

        //        conn.Open();
        //        adapter.SelectCommand = new MySqlCommand($"SELECT * FROM `Sistema2020`.`Privilegios` WHERE `IdUsuario` = {privilegios.IdUsuario}; ", conn);
        //        adapter.Fill(ds);
        //        adapter.Dispose();
        //        if (ds.Rows.Count > 0)
        //        {
        //            privilegios.IdUsuario = (int)row["IdUsuario"];
        //            privilegios.NombreUsuario = row["NombreUsuario"].ToString();
        //            privilegios.Cargo = (int)row["Cargo"];
        //            privilegios.CB = row["CB"].ToString();
        //            privilegios.MPPS = row["Activo"].ToString();
        //            privilegios.SIGLAS = row["PrecioDolar"].ToString();
        //            privilegios.Firma = row["Activo"].ToString();
        //            privilegios.Privilegios =
        //        }
        //        return privilegios;

        //    }
        //    catch (Exception ex)
        //    {
        //        CrearEvento(ex.ToString());
        //        return privilegios;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}
        //public static Usuarios selectUsuarios(Usuarios usuario)
        //{

        //    MySqlConnection conn = new MySqlConnection(connection);
        //    DataTable ds = new DataTable();
        //    try
        //    {

        //        conn.Open();
        //        adapter.SelectCommand = new MySqlCommand("SELECT * FROM `Sistema2020`.`Perfil` WHERE `Activo` = 1; ", conn);
        //        adapter.Fill(ds);
        //        adapter.Dispose();
        //        if (ds.Rows.Count > 0)
        //        {
        //            usuario.IdUsuario = (int)row["IdUsuario"];
        //            usuario.NombreUsuario = row["NombreUsuario"].ToString();
        //            usuario.Cargo = (int)row["Cargo"];
        //            usuario.CB = row["CB"].ToString();
        //            usuario.MPPS = row["Activo"].ToString();
        //            usuario.SIGLAS = row["PrecioDolar"].ToString();
        //            usuario.Firma = row["Activo"].ToString();
        //            usuario.Privilegios =
        //        }
        //        return usuario;

        //    }
        //    catch (Exception ex)
        //    {
        //        CrearEvento(ex.ToString());
        //        return usuario;
        //    }
        //    finally
        //    {
        //        conn.Close();
        //    }
        //}

        public static List<Usuarios> selectUsuarios()
        {
            List<Usuarios> usuarios = new List<Usuarios>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM `Sistema2020`.`Perfil` WHERE `Activo` = 1; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    foreach (DataRow row in ds.Rows)
                    {
                        Usuarios usuario = new Usuarios();
                        usuario.IdUsuario = (int)row["IdUsuario"];
                        usuario.NombreUsuario = row["NombreUsuario"].ToString();
                        usuario.Cargo = (int)row["Cargo"];
                        usuario.CB = row["CB"].ToString();
                        usuario.MPPS = row["Activo"].ToString();
                        usuario.SIGLAS = row["PrecioDolar"].ToString();
                        usuario.Firma = row["Activo"].ToString();
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
                conn.Close();
            }
        }
        public static async Task<string> AsyncTestConnection(string cmd)
        {

            Connection(cmd, cmd);
            MySqlConnection conn = new MySqlConnection(connection);
            string estado;
            try
            {

                conn.Open();
                return "Conectado";
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return "Desconectado";
            }
            finally
            {
                conn.Close();
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
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.resultadospaciente Where IdOrden = '{0}' and EstadoDeResultado > 1;", cmd), conn);
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
                conn.Close();
            }
        }
        public static int VerificarTelefono(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT CodigoTelefono,Telefono FROM sistema2020.datospersonales join ordenes on ordenes.IdPersona = datospersonales.IdPersona Where IdOrden = '{0}'", cmd), conn);
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
                conn.Close();
            }
        }
        public static DataSet Login(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select IdUsuario,NombreUsuario FROM usuarios Where contraseña = '{0}'", cmd), conn);
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
        public static string Telefono(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            string Retorno = "0";
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select CONCAT('+58',CodigoCelular,celular) as Telefono FROM ordenes join datospersonales on ordenes.IdPersona = datospersonales.IdPersona Where IdOrden = {0}", cmd), conn);
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
                conn.Close();
            }
        }
        public static DataSet TiposdePago()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT * FROM sistema2020.tipodepago", conn);
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
        public static DataSet Privilegios(int UserID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.IdUsuario, usuarios.NombreUsuario, usuarios.Privilegios,usuarios.Cargo FROM usuarios WHERE(((usuarios.IdUsuario) = {0}));", UserID), conn);
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
        public static DataSet ordenesPorCobrar()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.NumeroDia as '#',ordenes.IdOrden,CONCAT(Nombre,' ',Apellidos) as Nombre,FORMAT(precioF,2) AS Facturado, FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)),2) as Entradas,FORMAT(SUM(if(Clasificacion=2,ValorResultado,0)),2) as Salidas,FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2) AS Pago,convert( -PrecioF + SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),DECIMAL(10,2)) as Total FROM sistema2020.ordenes left join pagos on ordenes.IdOrden = Pagos.IDOrden left join datospersonales on ordenes.IdPersona = datospersonales.IdPersona Where ordenes.Fecha = CURDATE() and EstadoDeOrden  < 3 GROUP BY IdOrden"), conn);
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
        public static DataSet ordenesPendientes()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `cobrodiario`.`#`, `cobrodiario`.`Fecha`, `cobrodiario`.`IdOrden`, `cobrodiario`.`Nombre`, `cobrodiario`.`Facturado`, `cobrodiario`.`Total` FROM `sistema2020`.`cobrodiario` where Total <> 0 and Fecha <> CURDATE();"), conn);
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
        public static DataSet ordenesPorcruzar(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.NumeroDia as '#',ordenes.IdOrden,CONCAT(Nombre,' ',Apellidos) as Nombre,FORMAT(precioF,2) AS Facturado, FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)),2) as Entradas,FORMAT(SUM(if(Clasificacion=2,ValorResultado,0)),2) as Salidas,FORMAT(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2) AS Pago,(- PrecioF + ROUND(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2)) As Total FROM sistema2020.ordenes left join pagos on ordenes.IdOrden = Pagos.IDOrden left join datospersonales on ordenes.IdPersona = datospersonales.IdPersona Where ordenes.IdOrden = {0} and EstadoDeOrden  < 3 GROUP BY IdOrden", cmd), conn);
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
        public static DataSet SelectAgrupador(string IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select IdAgrupador from analisislaboratorio Where IdAnalisis  = {0};", IdAnalisis), conn);
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
        public static Task<DataSet> SecuenciaDeTrabajoAsync()
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            return Task.Run(() =>
            {
                try
                {

                    conn.Open();
                    adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, ordenes.Fecha,ordenes.HoraIngreso,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio WHERE ordenes.Fecha = '{0}' AND ordenes.estadodeorden < 3 ORDER BY ordenes.IdOrden;", DateTime.Now.ToString("yyyy-MM-dd")), conn);
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
                    conn.Close();
                }
            });
        }

        public static DataSet SecuenciaDeTrabajo()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, ordenes.Fecha,ordenes.HoraIngreso,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio WHERE ordenes.Fecha = CURDATE() AND ordenes.estadodeorden < 3 ORDER BY ordenes.IdOrden;", DateTime.Now.ToString("yyyy-MM-dd")), conn);
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
        public static DataSet PersonaCobro(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.NumeroDia, ordenes.Fecha,ordenes.HoraIngreso,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,ordenes.PrecioF,ordenes.IdTasa,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio WHERE ordenes.IdOrden = {0}", cmd), conn);
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
        public static DataSet ListadoDePacientes(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT DATE_FORMAT(ordenes.Fecha,'%d-%m-%Y') as Fecha,ordenes.IdOrden as Orden, ordenes.NumeroDia as Nro,datospersonales.Cedula,CONCAT(datospersonales.Nombre,' ', datospersonales.Apellidos) As Nombre, datospersonales.Fecha as 'Fecha de Nacimiento',concat(datospersonales.CodigoCelular,'-',datospersonales.Celular) as Celular,concat(datospersonales.CodigoTelefono,'-',datospersonales.Telefono) as Telefono FROM(datospersonales) INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio  WHERE(ordenes.Fecha >= '{0}'  AND ordenes.Fecha <= '{1}') AND ordenes.estadodeorden < 3  ORDER BY ordenes.IdOrden; ", cmd, cmd2), conn);
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
        public static DataSet SecuenciaDeLEAN(string Fecha1, string Fecha2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden AS NO, ordenes.NumeroDia AS N, ordenes.Fecha,ordenes.HoraIngreso as Hora,datospersonales.Cedula,CONCAT(datospersonales.Nombre,' ', datospersonales.Apellidos) AS Nombre, NombreUsuario FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio Inner join usuarios on usuarios.IdUsuario = ordenes.Usuario WHERE (((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}') AND ordenes.estadodeorden < 3) AND ordenes.IdConvenio <= 3 ORDER BY ordenes.IdOrden;", Fecha1, Fecha2), conn);
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
        public static DataSet SecuenciaDeLEANPorSede(string Fecha1, string Fecha2, string IDConvenio)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden AS NO, ordenes.NumeroDia AS N, ordenes.Fecha,ordenes.HoraIngreso as Hora,datospersonales.Cedula,CONCAT(datospersonales.Nombre,' ', datospersonales.Apellidos) AS Nombre, NombreUsuario FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio Inner join usuarios on usuarios.IdUsuario = ordenes.Usuario WHERE (((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}') AND ordenes.estadodeorden < 3) AND ordenes.IdConvenio = {2} ORDER BY ordenes.IdOrden;", Fecha1, Fecha2, IDConvenio), conn);
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
        public static DataSet ConteoSecuenciaDeLEAN(string Fecha1, string Fecha2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(NombreUsuario) as Cant,NombreUsuario FROM(datosrepresentante INNER JOIN ordenes ON datosrepresentante.IdDatosRepresentante = ordenes.IdDatosRepresentante) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio Inner join usuarios on usuarios.IdUsuario = ordenes.IdUsuario WHERE ordenes.Fecha between '{0}' And '{1}' AND ordenes.IDEstadoDeOrden < 3 Group by NombreUsuario ORDER BY ordenes.IdOrden;", Fecha1, Fecha2), conn);
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
        public static DataSet ListaPrecios()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT IdPerfil as ID,NombrePerfil,Precio,PrecioDolar as Dolares FROM Perfil", conn);
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
        public static DataSet ListaPreciosAImprimir()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT `perfil`.`IdPerfil`,`perfil`.`NombrePerfil`, `perfil`.`Precio`,  `perfil`.`Precio` * 1000000 as Bs FROM `sistema2020`.`perfil` Where Activo = 1 Order By `perfil`.`NombrePerfil` asc;", conn);
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
        public static DataSet Deuda(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `ordenes`.`NumeroDia` AS `#`,`ordenes`.`Fecha` AS `Fecha`,`ordenes`.`IdOrden` AS `IdOrden`,CONCAT(`datospersonales`.`Nombre`,' ',`datospersonales`.`Apellidos`) AS `Nombre`,`ordenes`.`PrecioF` AS `Facturado`,SUM(IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0)) AS `Entradas`,SUM(IF((`pagos`.`Clasificacion` = 2),`pagos`.`ValorResultado`,0)) AS `Salidas`,SUM((IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0) - IF((`pagos`.`Clasificacion` = 2),`pagos`.`ValorResultado`,0))) AS `Pago`,(-(`ordenes`.`PrecioF`) + SUM((IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0) - IF((`pagos`.`Clasificacion` = 2),`pagos`.`ValorResultado`, 0)))) AS `Total`FROM (((`ordenes` LEFT JOIN `pagos` ON ((`ordenes`.`IdOrden` = `pagos`.`IdOrden`))) LEFT JOIN `datospersonales` ON ((`ordenes`.`IdPersona` = `datospersonales`.`IdPersona`)))LEFT JOIN `tasadia` ON ((`ordenes`.`IDTasa` = `tasadia`.`Id`))) WHERE ((`ordenes`.`EstadoDeOrden` < 3) and `ordenes`.`IdOrden` = {0}) and Pagos.Fecha < Curdate() GROUP BY `ordenes`.`IdOrden`", cmd), conn);
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
        public static DataTable SecuenciaPorValidar()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, ordenes.Fecha,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio WHERE ordenes.Fecha = '{0}' AND ordenes.VALIDADA = 0  AND ordenes.estadodeorden < 3 AND estadodeorden = 1 ORDER BY ordenes.IdOrden;", DateTime.Now.ToString("yyyy-MM-dd")), conn);
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
        public static DataSet DatosUsuario(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select NombreUsuario,empresas.Sede FROM usuarios join empresas Where IdUsuario = {0} and Activa = 1", cmd), conn);
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
        public static DataSet SELECTPersona(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM datospersonales WHERE datospersonales.Cedula = '{0}'", cmd), conn);
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
        public static DataSet SELECTPersonaOrden(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT NumeroDia,PrecioF,IDTasa,datospersonales.IdPersona, datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Celular, datospersonales.Telefono, datospersonales.Sexo, datospersonales.Correo, datospersonales.Fecha, datospersonales.TipoCorreo, datospersonales.CodigoCelular, datospersonales.CodigoTelefono FROM datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona WHERE(((ordenes.IdOrden) = {0}))", cmd), conn);
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
        public static DataSet SELECTPersonaEmail(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT datospersonales.Correo, datospersonales.TipoCorreo FROM datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona WHERE(((ordenes.IdOrden) = {0}))", cmd), conn);
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
        public static DataSet SELECTConvenioEmail(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `convenios`.`CorreoSede` FROM `sistema2020`.`convenios` Where IdConvenio = {0};", cmd), conn);
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
        public static DataSet SELECTAnalisis(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(" SELECT  `perfil`.`IdPerfil` AS `IdPerfil`, `perfil`.`NombrePerfil` AS `NombrePerfil`, `perfil`.`Precio` AS `Precio` FROM `perfil` WHERE (`perfil`.`Activo` = 1) AND `perfil`.`IdPerfil` = {0} ", cmd), conn);
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
        public static DataSet SELECTAnalisis3(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.IdPerfil, Perfil.NombrePerfil, PerfilSeleccion.IdAnalisis, PerfilesASeleccionar.NombrePerfil, PerfilesASeleccionar.Precio FROM(Perfil INNER JOIN PerfilSeleccion ON Perfil.IdPerfil = PerfilSeleccion.IdPerfil) INNER JOIN PerfilesASeleccionar ON PerfilSeleccion.IdAnalisis = PerfilesASeleccionar.IdPerfilSeleccion WHERE(((Perfil.IdPerfil) = {0})); ", cmd), conn);
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
        public static DataSet SELECTTasaDia()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT TasaDia.Dolar,TasaDia.Pesos,TasaDia.Euros,TasaDia.Fecha FROM TasaDia ORDER BY TasaDia.Id DESC LIMIT 1; ", conn);
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
        public static DataSet SELECTTasaPorId(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM TasaDia Where TasaDia.ID = {0}", cmd), conn);
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
        public static DataSet CambiarListaPrecio()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT ListaDePrecio,ID FROM TasaDia ORDER BY TasaDia.Id DESC LIMIT 1; ", conn);
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
        public static Task<DataSet> CambiarListaPrecioAsync()
        {
            DataSet ds = new DataSet();

            return Task.Run(() =>
            {
                MySqlConnection conn = new MySqlConnection(connection);
                try
                {

                    conn.Open();
                    adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ListaDePrecio,ID FROM TasaDia ORDER BY TasaDia.Id DESC LIMIT 1;"), conn);
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
                    conn.Close();
                }
            });
        }

        public static DataSet ListadeTrabajo(string fecha1, string fecha2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("select resultadospaciente.IdAnalisis,Etiquetas From  resultadospaciente join analisislaboratorio on  resultadospaciente.IdAnalisis = analisislaboratorio.IdAnalisis join ordenes on ordenes.IdOrden = resultadospaciente.IdOrden Where (EstadodeResultado < 2 or EstadodeResultado is null) And EstadoDeOrden < 3 And Etiquetas is not Null and FechaIngreso = '{0}' group by IdAnalisis order by IdOrganizador asc;", fecha1), conn);
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
        public static DataSet AnalisisListadeTrabajo(string IdAnalisis, string Fecha)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("select ordenes.NumeroDia From resultadospaciente join ordenes on ordenes.IdOrden = resultadospaciente.IdOrden Where (EstadodeResultado < 2 or EstadodeResultado is null) And EstadoDeOrden < 3 and FechaIngreso = '{1}' and IdAnalisis= {0} order by NumeroDia asc", IdAnalisis, Fecha), conn);
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
        public static DataSet SelectEmpresaActiva()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT `empresas`.`IdEmpresa`,`empresas`.`Nombre`,`empresas`.`Rif`,`empresas`.`Ruta`,`empresas`.`Activa`,`empresas`.`Sede`,`empresas`.`Correo`,`empresas`.`Clave`,`empresas`.`Puerto`,`empresas`.`CorreoSistema`,`empresas`.`ClaveSistema`,`empresas`.`Direccion`,`empresas`.`Telefono`,`empresas`.`Telefono2` FROM`sistema2020`.`empresas` WHERE `Empresas`.`Activa` = 1; ", conn);
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
        public static Empresa selectEmpresaActiva()
        {
            Empresa empresa = new Empresa();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT `empresas`.`IdEmpresa`,`empresas`.`Nombre`,`empresas`.`Rif`,`empresas`.`Ruta`,`empresas`.`Activa`,`empresas`.`Sede`,`empresas`.`Correo`,`empresas`.`Clave`,`empresas`.`Puerto`,`empresas`.`CorreoSistema`,`empresas`.`ClaveSistema`,`empresas`.`Direccion`,`empresas`.`Telefono`,`empresas`.`Telefono2` FROM`sistema2020`.`empresas` WHERE `Empresas`.`Activa` = 1; ", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    empresa.IdEmpresa = (int)ds.Rows[0]["IdEmpresa"];
                    empresa.Nombre = ds.Rows[0]["Nombre"].ToString();
                    empresa.Rif = ds.Rows[0]["Rif"].ToString();
                    empresa.Ruta = ds.Rows[0]["Ruta"].ToString();
                    empresa.Activa = (int)ds.Rows[0]["Activa"];
                    empresa.Sede = ds.Rows[0]["Sede"].ToString();
                    empresa.Correo = ds.Rows[0]["Correo"].ToString();
                    empresa.Clave = ds.Rows[0]["Clave"].ToString();
                    empresa.Puerto = ds.Rows[0]["Puerto"].ToString();
                    empresa.CorreoSistema = ds.Rows[0]["CorreoSistema"].ToString();
                    empresa.ClaveSistema = ds.Rows[0]["ClaveSistema"].ToString();
                    empresa.Direccion = ds.Rows[0]["Direccion"].ToString();
                    empresa.Telefono = ds.Rows[0]["Telefono"].ToString();
                    empresa.Telefono2 = ds.Rows[0]["Telefono2"].ToString();
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
        public static DataSet SelectEmpresaActivaEstadisticas()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT `convenios`.`IdConvenio`, `convenios`.`Nombre` FROM sistema2020.convenioempresas join convenios on convenioempresas.IdConvenio = convenios.IdConvenio  join Empresas on convenioempresas.IdEmpresas = Empresas.IDEmpresa Where Empresas.Activa != 1;", conn);
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
        public static DataSet SelectEmpresaConvenio(string IdConvenio)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `empresas`.`IdEmpresa`,`empresas`.`Nombre`,`empresas`.`Rif`,`empresas`.`Ruta`,`empresas`.`Activa`,`empresas`.`Sede`,`empresas`.`Correo`,`empresas`.`Clave`,`empresas`.`Puerto`,`empresas`.`CorreoSistema`,`empresas`.`ClaveSistema`,`empresas`.`Direccion`,`empresas`.`Telefono` FROM `sistema2020`.`empresas` join  `convenioempresas` on  `convenioempresas`.`IdEmpresas` = `empresas`.`IdEmpresa` Where `convenioempresas`.`IdConvenio` = {0}", IdConvenio), conn);
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
        public static DataSet SELECTLipidoGrama(int cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, ResultadosPaciente.ValorResultado, ResultadosPaciente.IdOrganizador, MayoroMenorReferencial.ValorMenor, MayoroMenorReferencial.ValorMayor, PerfilesAnalisis.IdPerfil FROM((analisislaboratorio INNER JOIN MayoroMenorReferencial ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) INNER JOIN PerfilesAnalisis ON analisislaboratorio.IdAnalisis = PerfilesAnalisis.IdAnalisis WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((PerfilesAnalisis.IdPerfil) = 1391)); ", cmd), conn);
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
        public static DataSet SELECTAnalisisText(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.IdPerfil, Perfil.NombrePerfil, Perfil.Precio FROM Perfil WHERE(((Perfil.NombrePerfil)Like '%{0}%') AND((Perfil.Activo) = 1)); ", cmd), conn);
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
        public static DataSet SELECTAnalisisAFacturar(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(" SELECT    `perfilesafacturar`.`IdPerfil` AS `IdPerfil`,  `perfil`.`NombrePerfil` AS `NombrePerfil`,  `perfilesafacturar`.`PrecioPerfil` AS `PrecioPerfil`,   `perfilesafacturar`.`IdSesion` AS `IdSesion`   FROM    (`perfilesafacturar`   JOIN `perfil` ON((`perfilesafacturar`.`IdPerfil` = `perfil`.`IdPerfil`))) Where IDSesion = {0} ", cmd), conn);
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
        public static int Insertar(string cmd)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                command = new MySqlCommand(string.Format("Insert into datospersonales (Cedula,Nombre,Apellidos,Celular,Telefono,Sexo,Correo,Fecha,TipoCorreo,CodigoCelular) Values ({0})", cmd), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                idPaciente = Convert.ToInt32(cmd2.ExecuteScalar());
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
        public static int InsertarPerfilSimple(Perfil perfil, AnalisisLaboratorio analisis, mayoromenorreferencial valores)
        {
            int IdPerfil, IdAnalisis;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("InsertarPerfil", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("NombrePerfil", MySqlDbType.String).Value = perfil.NombrePerfil;
                cmd.Parameters.Add("Precio", MySqlDbType.String).Value = perfil.Precio;
                cmd.Parameters.Add("PrecioDolar", MySqlDbType.VarChar).Value = perfil.PrecioDolar;
                cmd.Parameters.Add("Activo", MySqlDbType.Int32).Value = perfil.Activo;
                cmd.ExecuteNonQuery();
                IdPerfil = Convert.ToInt32(cmd2.ExecuteScalar());


                MySqlCommand cmd3 = new MySqlCommand("InsertarAnalisis", conn);
                cmd3.CommandType = CommandType.StoredProcedure;
                int Visible = 0;
                if (analisis.Visible)
                {
                    Visible = 1;
                }
                //IdPerfil int,
                cmd3.Parameters.Add("IdPerfil", MySqlDbType.Int32).Value = IdPerfil;
                //NombreAnalisis varchar(255),
                cmd3.Parameters.Add("NombreAnalisis", MySqlDbType.VarChar).Value = analisis.NombreAnalisis;
                //TipoAnalisis int
                cmd3.Parameters.Add("TipoAnalisis", MySqlDbType.Int32).Value = analisis.TipoAnalisis;
                //, EsVisible int,
                cmd3.Parameters.Add("EsVisible", MySqlDbType.Int32).Value = Visible;
                //Etiquetas varchar(50)
                cmd3.Parameters.Add("Etiquetas", MySqlDbType.VarChar).Value = analisis.Etiqueta;
                //Seccion int
                cmd3.Parameters.Add("Seccion", MySqlDbType.Int32).Value = analisis.IdSeccion;
                //Especiales int
                cmd3.Parameters.Add("Especiales", MySqlDbType.VarChar).Value = analisis.Especiales;
                //Titulo int
                cmd3.Parameters.Add("Titulo", MySqlDbType.Int32).Value = analisis.Titulo;
                //FinalTitulo int
                cmd3.Parameters.Add("FinalTitulo", MySqlDbType.Int32).Value = analisis.FinalTitulo;
                //idAgrupador int
                cmd3.Parameters.Add("idAgrupador", MySqlDbType.Int32).Value = analisis.IdAgrupador;
                cmd3.ExecuteNonQuery();
                MySqlCommand cmd5 = new MySqlCommand(Query2, conn);
                IdAnalisis = Convert.ToInt32(cmd5.ExecuteScalar());

                MySqlCommand cmd4 = new MySqlCommand("InsertarValores", conn);
                cmd4.CommandType = CommandType.StoredProcedure;
                //IdAnalisis int
                cmd4.Parameters.Add("IdAnalisis", MySqlDbType.Int32).Value = IdAnalisis;
                //ValorMayor double
                cmd4.Parameters.Add("ValorMayor", MySqlDbType.VarChar).Value = valores.ValorMayor.Replace(",", ".");
                //ValorMenor double
                cmd4.Parameters.Add("ValorMenor", MySqlDbType.VarChar).Value = valores.ValorMenor.Replace(",", ".");
                // MultiplesValores longText
                cmd4.Parameters.Add("MultiplesValores", MySqlDbType.LongText).Value = valores.MultiplesValores;
                // lineas int
                cmd4.Parameters.Add("lineas", MySqlDbType.Int32).Value = valores.lineas;
                // Unidad varchar(50))
                cmd4.Parameters.Add("Unidad", MySqlDbType.VarChar).Value = valores.Unidad;
                cmd4.ExecuteNonQuery();




                return IdAnalisis;
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
        public static int InsertarPerfilCompuesto(Perfil perfil, List<AnalisisLaboratorio> analisis)
        {
            int IdPerfil, IdAnalisis;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("InsertarPerfil", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("NombrePerfil", MySqlDbType.String).Value = perfil.NombrePerfil;
                cmd.Parameters.Add("Precio", MySqlDbType.Decimal).Value = perfil.Precio;
                cmd.Parameters.Add("PrecioDolar", MySqlDbType.Decimal).Value = perfil.PrecioDolar;
                cmd.Parameters.Add("Activo", MySqlDbType.Int32).Value = perfil.Activo;
                cmd.ExecuteNonQuery();
                IdPerfil = Convert.ToInt32(cmd2.ExecuteScalar());

                foreach (var items in analisis)
                {
                    MySqlCommand cmd3 = new MySqlCommand("InsertarPerfilesAnalisis", conn);
                    cmd3.CommandType = CommandType.StoredProcedure;
                    //IdPerfil
                    cmd3.Parameters.Add("IdPerfil", MySqlDbType.Int32).Value = IdPerfil;
                    //IdAnalisis int
                    cmd3.Parameters.Add("IdAnalisis", MySqlDbType.Int32).Value = items.IdAnalisis;
                    cmd3.Parameters.Add("IDOrganizador", MySqlDbType.Int32).Value = items.idOrganizador;
                    cmd3.ExecuteNonQuery();
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
                conn.Close();
            }
        }
        public static int ModificarPerfilCompuesto(Perfil perfil, List<AnalisisLaboratorio> analisis)
        {
            int IdPerfil, IdAnalisis;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("ActualizarPerfil", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("NombrePerfil", MySqlDbType.String).Value = perfil.NombrePerfil;
                cmd.Parameters.Add("Precio", MySqlDbType.String).Value = perfil.Precio;
                cmd.Parameters.Add("PrecioDolar", MySqlDbType.VarChar).Value = perfil.PrecioDolar;
                cmd.Parameters.Add("Activo", MySqlDbType.Int32).Value = perfil.Activo;
                cmd.ExecuteNonQuery();
                IdPerfil = Convert.ToInt32(cmd2.ExecuteScalar());
                if (IdPerfil != 357 || IdPerfil != 1 || IdPerfil != 664 || IdPerfil != 56 || IdPerfil != 62 || IdPerfil != 151 || IdPerfil != 225 || IdPerfil != 224 || IdPerfil != 415)
                {
                    BorrarPerfil(perfil.IdPerfil);
                    foreach (var items in perfil.analisisLaboratorios)
                    {
                        MySqlCommand cmd3 = new MySqlCommand("InsertarPerfilesAnalisis", conn);
                        cmd3.CommandType = CommandType.StoredProcedure;
                        //IdPerfil
                        cmd3.Parameters.Add("IdPerfil", MySqlDbType.Int32).Value = IdPerfil;
                        //IdAnalisis int
                        cmd3.Parameters.Add("IdAnalisis", MySqlDbType.Int32).Value = items.IdAnalisis;
                        cmd3.ExecuteNonQuery();
                    }
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
                conn.Close();
            }
        }
        public static int BorrarPerfil(int IdPerfil)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("BorrarPerfil", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("IdPerfil1", MySqlDbType.Int32).Value = IdPerfil;
                cmd.ExecuteNonQuery();
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
                conn.Close();
            }

        }
        public static int ModificarPerfilCompuesto(Perfil perfil)
        {
            int IdAnalisis;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                MySqlCommand cmd = new MySqlCommand("ActualizarPerfil", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add("IdPerfil1", MySqlDbType.Int32).Value = perfil.IdPerfil;
                cmd.Parameters.Add("NombrePerfil", MySqlDbType.String).Value = perfil.NombrePerfil;
                cmd.Parameters.Add("Precio", MySqlDbType.String).Value = perfil.Precio.ToString().Replace(",",".");
                cmd.Parameters.Add("PrecioDolar", MySqlDbType.VarChar).Value = perfil.PrecioDolar;
                cmd.Parameters.Add("Activo", MySqlDbType.Int32).Value = perfil.Activo;
                cmd.ExecuteNonQuery();
                BorrarPerfil(perfil.IdPerfil);
                foreach (var items in perfil.analisisLaboratorios)
                {
                    MySqlCommand cmd3 = new MySqlCommand("InsertarPerfilesAnalisis", conn);
                    cmd3.CommandType = CommandType.StoredProcedure;
                    cmd3.Parameters.Add("IdPerfil", MySqlDbType.Int32).Value = perfil.IdPerfil;
                    cmd3.Parameters.Add("IdAnalisis", MySqlDbType.Int32).Value = items.IdAnalisis;
                    cmd3.Parameters.Add("IDOrganizador", MySqlDbType.Int32).Value = items.idOrganizador;
                    cmd3.ExecuteNonQuery();
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
                conn.Close();
            }
        }
        public static int InsertarHoraDeLlegada(string cmd)
        {
            int idPaciente;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO `sistema2020`.`captahuellas` (`IdUsuario`) VALUES ({0});", cmd), conn);
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
        public static int FlushHost()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand(string.Format("FLUSH HOSTS"), conn);
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
                conn.Close();
            }
        }
        public static async Task<string> InsertarPrueba(string IdPaciente, string IdPerfil1, string PrecioPerfil1, string IdSesion1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand("InsertarPerfilTemporal", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdPaciente", MySqlDbType.Int32).Value = IdPaciente;
                cmd2.Parameters.Add("IdPerfil1", MySqlDbType.Int32).Value = IdPerfil1;
                cmd2.Parameters.Add("PrecioPerfil1", MySqlDbType.VarChar).Value = PrecioPerfil1;
                cmd2.Parameters.Add("IdSesion1", MySqlDbType.Int32).Value = IdSesion1;

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
                conn.Close();
            }
        }
        public static DataSet SeleccionarPagos(string cmd, string fecha1, string fecha2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.pagos left join Bancos on Pagos.IdBanco = bancos.IdBancos where IdOrden = {0} and ( Fecha >= '{1}' and Fecha <= '{2}') order by Clasificacion asc;", cmd, fecha1, fecha2), conn);
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
        public static DataSet SelectPersonasEstadistica()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.vecesfacturado Where Cantidad > 1 Order By IdPersona desc"), conn);
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
        public static DataSet SelectOrdenesPorPersona(int IdPersona)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT IdOrden,Fecha,PrecioF FROM sistema2020.ordenes Where IdPersona = {IdPersona};", conn);
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
        public static DataSet SelectPerfilesPorOrden(int IdOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT NombrePerfil FROM sistema2020.perfilesfacturados left join Perfil on Perfil.IdPerfil = perfilesfacturados.IdPerfil where IdOrden = {IdOrden};", conn);
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
        public static DataSet SELECTFechaPago(string cmd, string Cantidad, string Serial)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.pagos where IdOrden = {0} and ( Cantidad = '{1}' and `Serial` <= '{2}')order by Clasificacion asc;", cmd, Cantidad, Serial), conn);
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
        public static DataSet SeleccionarPagos2(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.pagos where IdOrden = {0} order by Clasificacion asc;", cmd), conn);
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
        public static void BorrarPagos(string cmd)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                command = new MySqlCommand(string.Format("DELETE FROM `sistema2020`.`pagos`WHERE IdOrden = {0}; ", cmd), conn);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                conn.Close();
            }

        }
        public static void BorrarConveniosPorUsuario(int idConvenio, int IdUsuario)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                command = new MySqlCommand($"DELETE FROM `sistema2020`.`conveniosporusuario` WHERE IdConvenio = {idConvenio} and IdUsuario= {IdUsuario}; ", conn);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                conn.Close();
            }

        }
        public static void BorrarPagoSeleccionado(string IdOrden, string Cantidad, string Moneda, string serie)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                command = new MySqlCommand($"DELETE FROM `sistema2020`.`pagos` WHERE IdOrden = {IdOrden} and `pagos`.`Serial`= '{serie}' and Cantidad = '{Cantidad}' And Moneda = {Moneda} ;", conn);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                conn.Close();
            }

        }
        public static DataSet SelecFechaPago(string cmd, string cmd2, string cmd3)
        {
            MySqlConnection conn = new MySqlConnection(connection);

            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                command = new MySqlCommand(string.Format("SELECT FECHA FROM `sistema2020`.`pagos`WHERE IdOrden = {0} and Cantidad = '{1}' And Moneda = {2} ;", cmd, cmd2, cmd3), conn);
                adapter.SelectCommand = command;
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

        public static string InsertarPagos(string IdOrden, string TipodePago, string Cantidad, string ValorResultado, string DSerial, string Moneda, string Clasificacion, string IdTasa,int IdBanco)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("StoredPagos", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("Cantidad", MySqlDbType.VarChar).Value = Cantidad;
                cmd2.Parameters.Add("TipodePago", MySqlDbType.VarChar).Value = TipodePago;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.Decimal).Value = Convert.ToDecimal(ValorResultado);
                cmd2.Parameters.Add("DSerial", MySqlDbType.VarChar).Value = DSerial;
                cmd2.Parameters.Add("Moneda", MySqlDbType.Int32).Value = Moneda;
                cmd2.Parameters.Add("Clasificacion", MySqlDbType.Int32).Value = Clasificacion;
                cmd2.Parameters.Add("IdTasa", MySqlDbType.Int32).Value = IdTasa;
                cmd2.Parameters.Add("IdBanco", MySqlDbType.Int32).Value = IdBanco;
                conn.Open();
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
                conn.Close();
            }
        }

        public static string BorrarPago(string IDOrden, string IdSerial)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand("BorrarCruce", conn);
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
                conn.Close();
            }
        }
        public static string BorrarAFacturar(string IDSesion, string IdPerfil1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("BorrarAFacturar", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IDSesion", MySqlDbType.Int32).Value = IDSesion;
                cmd2.Parameters.Add("IdPerfil", MySqlDbType.Int32).Value = IdPerfil1;
                conn.Open();
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
                conn.Close();
            }
        }
        public static string InsertarFactura(string fecha, string IdPaciente1, string Hora, string IdUsuario, string PrecioF1, string IdSesion1, string IDConvenio1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarFactura", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("fecha1", MySqlDbType.DateTime).Value = fecha;
                cmd2.Parameters.Add("IdPersona1", MySqlDbType.Int32).Value = IdPaciente1;
                cmd2.Parameters.Add("Hora", MySqlDbType.VarChar).Value = Hora;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("PrecioF1", MySqlDbType.VarChar).Value = PrecioF1.Replace(",", ".");
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                cmd2.Parameters.Add("IDConvenio1", MySqlDbType.Int32).Value = IDConvenio1;
                conn.Open();
                cmd2.ExecuteNonQuery();
                string Query2 = "SELECT LAST_INSERT_ID()";
                cmd2 = new MySqlCommand(Query2, conn);
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
                conn.Close();
            }
        }
        public static int EOrinas(DateTime FechaInicial, DateTime FechaFinal)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            int EOrinas = 0;
            try
            {
                DataSet ds = new DataSet();
                string Query2 = $"SELECT IdPerfil,COUNT(IdPerfil) From PerfilesFacturados Join ordenes on ordenes.IdOrden = PerfilesFacturados.IdOrden WHere Fecha >= '{FechaInicial.ToString("yy-mm-dd")}' and Fecha <= '{FechaFinal.ToString("yy-mm-dd")}' and IDPerfil = 1509 Group by IdPerfil";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open(); ;
                EOrinas = Convert.ToInt32(cmd2.ExecuteScalar());
                return EOrinas;
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
        public static string CantidaddeHojas(int Hojas, bool EsResultado)
        {
            int Cantidad;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                if (EsResultado)
                {
                    if (Hojas > 2)
                    {
                        Hojas = Hojas - 1;
                        int residuo = Hojas % 2;
                        Cantidad = Hojas / 2 + residuo + 1;
                    }
                    else
                    {
                        Cantidad = Hojas;
                    }
                }
                else
                {
                    Cantidad = Hojas;
                }

                MySqlCommand cmd2 = new MySqlCommand("CantidadDeHojas", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("Hojas", MySqlDbType.Int32).Value = Cantidad;
                conn.Open();
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
                conn.Close();
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
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarReferido", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("fecha1", MySqlDbType.DateTime).Value = fecha;
                cmd2.Parameters.Add("IdPersona1", MySqlDbType.Int32).Value = IdPaciente1;
                cmd2.Parameters.Add("Hora", MySqlDbType.VarChar).Value = Hora;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("PrecioF1", MySqlDbType.VarChar).Value = PrecioF1;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                cmd2.Parameters.Add("IDConvenio1", MySqlDbType.Int32).Value = IDConvenio1;
                conn.Open();
                cmd2.ExecuteNonQuery();
                string Query2 = "SELECT LAST_INSERT_ID()";
                cmd2 = new MySqlCommand(Query2, conn);
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
                conn.Close();
            }
        }
        public static string ActualizarPrueba(string IdPaciente, string IdSesion1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("ActualizarPrueba", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdPaciente", MySqlDbType.Int32).Value = IdPaciente;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                conn.Open();
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
                conn.Close();
            }
        }
        public static string BorrarPrueba(string IdPerfil1, string IdSesion1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("DeleteAFacturar", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IDAnalisis", MySqlDbType.Int32).Value = IdPerfil1;
                cmd2.Parameters.Add("IdSesionLocal", MySqlDbType.Int32).Value = IdSesion1;
                conn.Open();
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
                conn.Close();
            }
        }
        public static string AnularOrden(string IdOrden, string IdUsuario)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.PrecioF,datospersonales.Nombre,datospersonales.Apellidos FROM ordenes join datospersonales on ordenes.IdPersona = datospersonales.IdPersona Where ordenes.IdOrden = {0} ", IdOrden), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                MySqlCommand cmd2 = new MySqlCommand("AnularOrden", conn);
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
                conn.Close();
            }
        }
        public static string InsertarReporte(string Accion, string IdUsuario)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand("InsertarReporte", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdUsuario1", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("Accion1", MySqlDbType.VarChar).Value = Accion;
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
                conn.Close();
            }
        }
        public static string EmpresaLogo(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            string MS;
            try
            {
                DataSet ds = new DataSet();
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO Empresas (Nombre, Rif,Ruta) Values ({0})", cmd), conn);
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
                conn.Close();
            }
        }

        public static DataSet convenios(string cmd)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT convenios.IdConvenio, convenios.Nombre, convenios.Descuento FROM usuarios INNER JOIN(convenios INNER JOIN conveniosPorUsuario ON convenios.IdConvenio = conveniosPorUsuario.IdConvenio) ON usuarios.IdUsuario = conveniosPorUsuario.IdUsuario WHERE(((conveniosPorUsuario.IdUsuario) = {0}) AND((convenios.Activos) = '1'));", cmd), conn);
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
        public static int InsertarResultado(string cmd)
        {
            int NumeroDia = 1;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                int idOrden = -1;
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.IdPersona, ordenes.Fecha, ordenes.HoraIngreso, ordenes.Usuario, ordenes.NumeroDia FROM ordenes WHERE ordenes.Fecha = Date('yyyy / MM / dd') Order By ordenes.NumeroDia desc LIMIT 1"), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables[0].Rows.Count != 0)
                {
                    string Numero = ds.Tables[0].Rows[0]["NumeroDia"].ToString();                                                                        //"2,'07/06/2020','11:28:11 a. m.',1,1"
                    NumeroDia = Convert.ToInt32(Numero) + 1;
                }                                                                                                                                       //Ejemplo {0}=(2,'7/3/2020','2:17',1,1,NumeroDia) 
                command = new MySqlCommand(string.Format("INSERT INTO ordenes (IdPersona, Fecha, HoraIngreso, Usuario, EstadoDeOrden,PrecioF,IdConvenio,NumeroDia,Validada) Values ({0},{1},1)", cmd, NumeroDia), conn);
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
                conn.Close();
            }
        }
        public static string InsertarPerfilesFactura(string cmd2, string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO PerfilesFacturados ( IdOrden, IdPersona, IdPerfil, PrecioPerfil ) SELECT ordenes.IdOrden, ordenes.IdPersona, PerfilesAFacturar.IdPerfil, PerfilesAFacturar.PrecioPerfil FROM ordenes INNER JOIN PerfilesAFacturar ON ordenes.IdOrden = PerfilesAFacturar.IDOrden WHERE(((PerfilesAFacturar.IdSesion) = {0}));", cmd), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                string query = string.Format("INSERT INTO ResultadosPaciente ( IdOrden, IdPaciente, FechaIngreso, HoraIngreso, IDConvenio, IdAnalisis, IdOrganizador ) SELECT ordenes.IdOrden, ordenes.IdPersona, ordenes.Fecha, ordenes.HoraIngreso, ordenes.IDConvenio, PerfilesAnalisis.IdAnalisis, PerfilesAnalisis.IdOrganizador FROM(ordenes INNER JOIN PerfilesAFacturar ON ordenes.IdOrden = PerfilesAFacturar.IDOrden) INNER JOIN PerfilesAnalisis ON PerfilesAFacturar.IdPerfil = PerfilesAnalisis.IdPerfil WHERE(((ordenes.IdOrden) = {0}))", cmd2);
                command = new MySqlCommand(query, conn);
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
                conn.Close();
            }
        }
        public static string AnalisisEnviados(string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                string query = string.Format("INSERT INTO ResultadosPaciente ( IdOrden, IdPaciente, FechaIngreso, HoraIngreso, IDConvenio, IdAnalisis, IdOrganizador,Recibido )  Values ({0})", cmd2);
                command = new MySqlCommand(query, conn);
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
                conn.Close();
            }
        }
        public static DataSet SELECTAnalisisPrueba(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden,analisislaboratorio.IdAnalisis,analisislaboratorio.NombreAnalisis,ResultadosPaciente.ValorResultado,MayoroMenorReferencial.Unidad,MayoroMenorReferencial.ValorMenor,MayoroMenorReferencial.ValorMayor,analisislaboratorio.TipoAnalisis,ResultadosPaciente.IdOrganizador,ResultadosPaciente.EstadoDeResultado,analisislaboratorio.IdAgrupador FROM Sistema2020.resultadospaciente LEFT JOIN analisislaboratorio ON analisislaboratorio.IdAnalisis = Resultadospaciente.IDAnalisis  left join mayoromenorreferencial on Resultadospaciente.IDAnalisis = mayoromenorreferencial.IdReferencial Where IdOrden = {0} Order by IdOrganizador", cmd), conn);
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
        public static int SelectEstadoOrden(string cmd)
        {
            int Estado = 0;
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                string Query = "SELECT EstadoDeOrden From ordenes Where IdOrden = @IdOrden";
                conn.Open();
                MySqlCommand cmd2 = new MySqlCommand(Query, conn);
                cmd2.Parameters.AddWithValue("@IdOrden", cmd);
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
                conn.Close();
            }
        }
        public static DataSet SelectCantidadCorreo(string cmd)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT if(Count(IdOrden) > 0,Count(IdOrden),0) as Enviado FROM sistema2020.envioporcorreo where IdOrden = {0};", cmd), conn);
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
        public static DataSet SELECTPruebasReportadas(string cmd)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(ResultadosPaciente.IdOrden) as Conteo FROM ResultadosPaciente Where EstadoDeResultado > 0 And IDOrden = {0}; ", cmd), conn);
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
        public static DataSet SELECTAnalisisFinal(int IDOrden, int IdAnalisis)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ordenes.NumeroDia, analisislaboratorio.NombreAnalisis, ResultadosPaciente.IdPaciente, ResultadosPaciente.IdAnalisis, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha FROM(datospersonales INNER JOIN(analisislaboratorio INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datospersonales.IdPersona = ResultadosPaciente.IdPaciente) INNER JOIN ordenes ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ordenes.IdPersona) WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1})); ", IDOrden, IdAnalisis), conn);
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
        public static DataSet SelectComentario(int IDOrden, int IdAnalisis)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT resultadospaciente.Comentario FROM resultadospaciente WHERE IdOrden = {0} and IdAnalisis = {1}; ", IDOrden, IdAnalisis), conn);
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
        public static string InsertarFinal(string ValorResultado, string Comentario, int IdUsuario, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("ActualizarResultado", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.VarChar).Value = ValorResultado;
                cmd2.Parameters.Add("Comentario", MySqlDbType.LongText).Value = Comentario;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis;
                conn.Open();
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
                conn.Close();
            }
        }
        public static string InsertarSinValidar(string ValorResultado, string Comentario, int IdUsuario, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("ActualizarResultadoSinValidar", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.VarChar).Value = ValorResultado;
                cmd2.Parameters.Add("Comentario", MySqlDbType.LongText).Value = Comentario;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis;
                conn.Open();
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
                conn.Close();
            }
        }

        public static DataSet SELECTAnalisisFinal1(int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, analisislaboratorio.NombreAnalisis, ResultadosPaciente.ValorResultado, ordenes.NumeroDia, analisislaboratorio.TipoAnalisis, MayoroMenorReferencial.ValorMenor, MayoroMenorReferencial.ValorMayor, MayoroMenorReferencial.Unidad, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, ResultadosPaciente.Comentario FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN((analisislaboratorio LEFT JOIN MayoroMenorReferencial ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente) WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1})); ", IDOrden, IdAnalisis), conn);
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
        public static DataSet FechaDeOrden(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `ordenes`.`Fecha`FROM `sistema2020`.`ordenes` Where IdOrden ={0} and EstadoDeOrden < 3; ", IDOrden), conn);
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
        public static DataSet PacienteAImprimir(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `datospersonales`.`IdPersona`,`datospersonales`.`Cedula`, `datospersonales`.`Nombre`,`datospersonales`.`Apellidos`,`datospersonales`.`Celular`,`datospersonales`.`Telefono`,`datospersonales`.`Sexo`,`datospersonales`.`Fecha`, `datospersonales`.`Correo`, `datospersonales`.`TipoCorreo`,`datospersonales`.`CodigoCelular`,`datospersonales`.`CodigoTelefono`,`convenios`.`IdConvenio`,`datospersonales`.`Apellidos` as Edad FROM `sistema2020`.`ordenes` join `datospersonales` on `datospersonales`.`IdPersona` = `ordenes`.`IdPersona` join `convenios` on  `convenios`.`IdConvenio` = `ordenes`.`IdConvenio` Where `ordenes`.`IdOrden` = {0}", IDOrden), conn);
                adapter.Fill(ds);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    int CodigoCelular = 0;
                    int.TryParse(ds.Tables[0].Rows[0]["CodigoCelular"].ToString(), out CodigoCelular);
                    ds.Tables[0].Rows[0]["CodigoCelular"] = CodigoCelular;
                }
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
        public static string EnviadoPorCorreo(int IDOrden, int User)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO `sistema2020`.`envioporcorreo` (`IdOrden`,`IdUsuario`) VALUES ({0},{1}); ", IDOrden, User), conn);
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
                conn.Close();
            }
        }
        public static async Task<DataSet> SELECTAnalisisFinal2(int IDOrden, int IdAnalisis)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format(@"SELECT 
                ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, ResultadosPaciente.ValorResultado,
                analisislaboratorio.NombreAnalisis, analisislaboratorio.TipoAnalisis,analisislaboratorio.TipoAnalisis,
                ordenes.NumeroDia,
                MayoroMenorReferencial.MultiplesValores, MayoroMenorReferencial.Unidad,
                datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha,
                ResultadosPaciente.Comentario
                FROM(datospersonales INNER JOIN ordenes
                ON datospersonales.IdPersona = ordenes.IdPersona) 
                INNER JOIN
                ((analisislaboratorio LEFT JOIN MayoroMenorReferencial 
                ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial)
                RIGHT JOIN ResultadosPaciente 
                ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) 
                ON(ordenes.IdOrden = ResultadosPaciente.IdOrden)
                AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente) 
                WHERE(((ResultadosPaciente.IdOrden) = {0}) AND ((ResultadosPaciente.IdAnalisis) = {1}));  ", IDOrden, IdAnalisis), conn);
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
        public static string Actualizardatospersonales(string cmd, string Cedula)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE datospersonales SET  {0}  Where Cedula = '{1}' ", cmd, Cedula);
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
                conn.Close();
            }
        }
        public static string ListaDePrecioImpresa(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE TasaDia SET ListaDePrecio = 1  Where Id = {0}", cmd);
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
                conn.Close();
            }
        }
        public static Hematologia Hematologia(int IDOrden, int IdAnalisis)
        {
            Hematologia hematologia = new Hematologia();
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ordenes.NumeroDia, ordenes.Fecha as FechaImp, ResultadosPaciente.IdUsuario, ResultadosPaciente.IdAnalisis, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, ResultadosPaciente.Comentario, Hematologias.Neutrofilos, Hematologias.linfocitos, Hematologias.Monocitos, Hematologias.Eosinofilos, Hematologias.Basofilos, Hematologias.Hematies, Hematologias.Hemoglobina, Hematologias.Hematocritos, Hematologias.VCM, Hematologias.HCM, Hematologias.CHCM, Hematologias.Plaquetas, Hematologias.Neutrofilos2, Hematologias.Linfocitos2, Hematologias.Monocitos2, Hematologias.Eosinofilos2, Hematologias.Basofilos2, Hematologias.leucocitos FROM((datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN(analisislaboratorio INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente)) LEFT JOIN Hematologias ON ordenes.IdOrden = Hematologias.IdOrden WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1}));", IDOrden, IdAnalisis), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    hematologia.leucocitos = ds.Tables[0].Rows[0]["leucocitos"].ToString();
                    hematologia.Neutrofilos = ds.Tables[0].Rows[0]["Neutrofilos"].ToString();
                    hematologia.linfocitos = ds.Tables[0].Rows[0]["linfocitos"].ToString();
                    hematologia.Monocitos = ds.Tables[0].Rows[0]["Monocitos"].ToString();
                    hematologia.Eosinofilos = ds.Tables[0].Rows[0]["Eosinofilos"].ToString();
                    hematologia.Basofilos = ds.Tables[0].Rows[0]["Basofilos"].ToString();
                    hematologia.Hematies = ds.Tables[0].Rows[0]["Hematies"].ToString();
                    hematologia.Hemoglobina = ds.Tables[0].Rows[0]["Hemoglobina"].ToString();
                    hematologia.Hematocritos = ds.Tables[0].Rows[0]["Hematocritos"].ToString();
                    hematologia.VCM = ds.Tables[0].Rows[0]["VCM"].ToString();
                    hematologia.HCM = ds.Tables[0].Rows[0]["HCM"].ToString();
                    hematologia.CHCM = ds.Tables[0].Rows[0]["CHCM"].ToString();
                    hematologia.Plaquetas = ds.Tables[0].Rows[0]["Plaquetas"].ToString();
                    hematologia.Neutrofilos2 = ds.Tables[0].Rows[0]["Neutrofilos2"].ToString();
                    hematologia.Linfocitos2 = ds.Tables[0].Rows[0]["Linfocitos2"].ToString();
                    hematologia.Monocitos2 = ds.Tables[0].Rows[0]["Monocitos2"].ToString();
                    hematologia.Eosinofilos2 = ds.Tables[0].Rows[0]["Eosinofilos2"].ToString();
                    hematologia.Basofilos2 = ds.Tables[0].Rows[0]["Basofilos2"].ToString();
                    hematologia.Comentario = ds.Tables[0].Rows[0]["Comentario"].ToString();

                }

                return hematologia;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return hematologia;
            }
            finally
            {
                conn.Close();
            }
        }
        public static DataSet Hematologia(int IDOrden)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ordenes.NumeroDia, ordenes.Fecha as FechaImp, ResultadosPaciente.IdUsuario, ResultadosPaciente.IdAnalisis, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, ResultadosPaciente.Comentario, Hematologias.Neutrofilos, Hematologias.linfocitos, Hematologias.Monocitos, Hematologias.Eosinofilos, Hematologias.Basofilos, Hematologias.Hematies, Hematologias.Hemoglobina, Hematologias.Hematocritos, Hematologias.VCM, Hematologias.HCM, Hematologias.CHCM, Hematologias.Plaquetas, Hematologias.Neutrofilos2, Hematologias.Linfocitos2, Hematologias.Monocitos2, Hematologias.Eosinofilos2, Hematologias.Basofilos2, Hematologias.leucocitos FROM((datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN(analisislaboratorio INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente)) LEFT JOIN Hematologias ON ordenes.IdOrden = Hematologias.IdOrden WHERE ResultadosPaciente.IdOrden = {0};", IDOrden), conn);
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
        public static DataSet CantidadesDeExamenes(int IdAnalisis, string date)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select ordenes.IdOrden,NumeroDia from resultadospaciente join ordenes on ordenes.IdOrden = resultadospaciente.IDOrden Where IDAnalisis = {0} and fechaingreso >= '{1}' and EstadodeOrden < 3", IdAnalisis, date), conn);
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
        public static DataSet VerificarHematologia(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Hematologias.IdOrden FROM Hematologias WHERE IDORDEN = {0} ", IDOrden), conn);
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
        public static string InsertarHematologia(string cmd, string cmd2, int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO `sistema2020`.`hematologias` (IdOrden, Neutrofilos, linfocitos,Monocitos,Eosinofilos, Basofilos, Hematies, Hemoglobina,Hematocritos,VCM,HCM,CHCM,Plaquetas, Neutrofilos2, Linfocitos2,Monocitos2,Eosinofilos2,Basofilos2,leucocitos) Values ({0})", cmd), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ActualizarHematologias(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2}", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static DataSet Orina(int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ordenes.NumeroDia, ordenes.Fecha AS FechaImp, ResultadosPaciente.Comentario, ResultadosPaciente.IdAnalisis, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, Orinas.Color, Orinas.Aspecto, Orinas.Densidad, Orinas.ph, Orinas.Glucosa, Orinas.Bilirrubina, Orinas.Nitritos, Orinas.Leucocitos, Orinas.cetonas, Orinas.Olor, Orinas.Hemoglobina, Orinas.Urobilinogeno, Orinas.Benedict, Orinas.Proteinas, Orinas.Robert, Orinas.Bacterias, Orinas.LeucocitosMicro, Orinas.Hematies, Orinas.Mucina, Orinas.CEPLANAS, Orinas.CETRANSICION, Orinas.CREDONDAS, Orinas.BLASTOCONIDAS, Orinas.Cristales, Orinas.Cilindros, Orinas.TiraReactiva FROM((datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN ResultadosPaciente ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente)) LEFT JOIN Orinas ON ordenes.IdOrden = Orinas.IdOrden WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1}));", IDOrden, IdAnalisis), conn);
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
        public static string InsertarOrinas(string IdOrden, string IdAnalisis, string Usuario1, string Comentario1, string Color, string Aspecto, string Densidad,
        string ph, string Glucosa, string Bilirrubina, string Nitritos, string Leucocitos, string cetonas, string Olor,
        string Hemoglobina, string Urobilinogeno, string Benedict, string Proteinas,
        string Robert, string Bacterias, string LeucocitosMicro, string Hematies,
        string Mucina, string CEPLANAS, string CETRANSICION,
        string CREDONDAS, string BLASTOCONIDAS, string Cristales,
        string Cilindros, string TiraReactiva, string EstadoResultado)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarOrina", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden;
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
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis;
                cmd2.Parameters.Add("Usuario1", MySqlDbType.Int32).Value = Usuario1;
                cmd2.Parameters.Add("Comentario1", MySqlDbType.LongText).Value = Comentario1;
                cmd2.Parameters.Add("EstadoResultado", MySqlDbType.Int32).Value = EstadoResultado;
                conn.Open();
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
                conn.Close();
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
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarOrinasSinVerificar", conn);
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
                conn.Open();
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
                conn.Close();
            }
        }

        public static string ActualizarOrinas(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2} ", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static DataSet VerificarOrina(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Orinas.IdOrden FROM Orinas WHERE IDORDEN = {0} ", IDOrden), conn);
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
        public static DataSet Heces(int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM((datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN(analisislaboratorio INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente)) LEFT JOIN Heces ON ordenes.IdOrden = Heces.IdOrden WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1}))", IDOrden, IdAnalisis), conn);
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
        public static DataSet SelectHecesDirecta(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, ordenes.Fecha AS FechaImp, datospersonales.Nombre, ResultadosPaciente.Comentario, ordenes.NumeroDia, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, hecesdirectas.Hematies, hecesdirectas.Leucocitos, hecesdirectas.Parasitos FROM((datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN(analisislaboratorio INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente)) LEFT JOIN hecesdirectas ON ordenes.IdOrden = hecesdirectas.IdOrden WHERE(((ResultadosPaciente.IdOrden) = {0}))", IDOrden), conn);
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
        public static DataSet HecesDirecta(int IdOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `hecesdirectas`.`idHecesDirectas`,`hecesdirectas`.`IdOrden`,`hecesdirectas`.`Leucocitos`,`hecesdirectas`.`Hematies`,`hecesdirectas`.`Parasitos` FROM `sistema2020`.`hecesdirectas` Where IdOrden = {0}", IdOrden), conn);
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
        public static DataSet VerificarHeces(int IDOrden)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Heces.IdOrden FROM Heces WHERE IDORDEN = {0} ", IDOrden), conn);
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
        public static DataSet VerificarHecesDirecta(int IDOrden)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT hecesdirectas.IdOrden FROM hecesdirectas WHERE IDORDEN = {0} ", IDOrden), conn);
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
        public static string InsertarHeces(string cmd, string cmd2, int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                string cmd4 = string.Format("INSERT INTO Heces (IdOrden, Color, Moco,Reaccion,Aspecto, Sangre, Ph,Consistencia,RestosAlimenticios,Hematies,Leucocitos,Parasitos) Values ({0})", cmd);
                command = new MySqlCommand(cmd4, conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string InsertarHeces(Heces heces)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                string cmd4 = $@"INSERT INTO `sistema2020`.`heces` 
                (`IdOrden`,`Color`,`Moco`,`Reaccion`,`Aspecto`,`Sangre`,`Ph`,`Consistencia`,`RestosAlimenticios`,`Hematies`,`Leucocitos`,`Parasitos`,`Amilorrea`,`Creatorrea`,`Polisacaridos`,`Gotas`,`Levaduras`)
                VALUES({heces.IdOrden},'{heces.Color}','{heces.Moco}','{heces.Reaccion}','{heces.Aspecto}','{heces.Sangre}','{heces.Ph}','{heces.Consistencia}','{heces.RestosAlimenticios}','{heces.Hematies}','{heces.Leucocitos}','{heces.Parasitos}',
                '{heces.Amilorrea}','{heces.Creatorrea}','{heces.Polisacaridos}','{heces.Gotas}','{heces.Levaduras}');";
                command = new MySqlCommand(cmd4, conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = $@"UPDATE `sistema2020`.`resultadospaciente`
                SET
                `Comentario` = '{heces.Comentario}',
                `HoraValidacion` = CURTIME(),
                `FechaValidacion` = CURDATE(),
                `EstadoDeResultado` = {heces.EstadoResultado},
                `IdUsuario` = {heces.IdUsuario} Where IdOrden = {heces.IdOrden} And IDAnalisis = 12";
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
                conn.Close();
            }
        }
        public static string InsertarHecesDirecta(string cmd, string cmd2, int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                string cmd4 = string.Format("INSERT INTO `sistema2020`.`hecesdirectas` (`IdOrden`, `Leucocitos`, `Hematies`, `Parasitos`) VALUES ({0});", cmd);
                command = new MySqlCommand(cmd4, conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis= {2} ", cmd2, IDOrden, IdAnalisis);
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
                conn.Close();
            }
        }
        public static string ActualizarHeces(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2} ", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ActualizarHeces(Heces heces)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = $@"UPDATE `sistema2020`.`heces`
                SET
                `Color` = '{heces.Color}',
                `Moco` = '{heces.Moco}',
                `Reaccion` = '{heces.Reaccion}',
                `Aspecto` = '{heces.Aspecto}',
                `Sangre` = '{heces.Sangre}',
                `Ph` = '{heces.Ph}',
                `Consistencia` = '{heces.Consistencia}',
                `RestosAlimenticios` = '{heces.RestosAlimenticios}',
                `Hematies` = '{heces.Hematies}',
                `Leucocitos` = '{heces.Leucocitos}',
                `Parasitos` = '{heces.Parasitos}',
                `Amilorrea` = '{heces.Amilorrea}',
                `Creatorrea` = '{heces.Creatorrea}',
                `Polisacaridos` = '{heces.Polisacaridos}',
                `Gotas` = '{heces.Gotas}',
                `Levaduras` = '{heces.Levaduras}' 
                WHERE `IdOrden` = {heces.IdOrden};";
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query2 = $@"UPDATE `sistema2020`.`resultadospaciente`
                SET
                `Comentario` = '{heces.Comentario}',
                `HoraValidacion` = CURTIME(),
                `FechaValidacion` = CURDATE(),
                `EstadoDeResultado` = {heces.EstadoResultado},
                `IdUsuario` = {heces.IdUsuario} Where IdOrden = {heces.IdOrden} And IDAnalisis = 12";
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
                conn.Close();
            }
        }
        public static string ActualizarHecesDirecta(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2} ", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query2 = string.Format("UPDATE HecesDirectas SET  {0}  Where IdOrden = {1} ", cmd2, IdOrden, IdAnalisis);
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
                conn.Close();
            }
        }
        public static DataSet SelectPT(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdPaciente, ResultadosPaciente.IdAnalisis, ResultadosPaciente.ValorResultado FROM ResultadosPaciente WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = 121 Or(ResultadosPaciente.IdAnalisis) = 122 Or(ResultadosPaciente.IdAnalisis) = 123 Or(ResultadosPaciente.IdAnalisis) = 124 Or(ResultadosPaciente.IdAnalisis) = 125 Or(ResultadosPaciente.IdAnalisis) = 126)); ", IDOrden), conn);
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
        public static DataSet SelectPersonaOrden(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, datospersonales.Sexo, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Fecha FROM datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona WHERE(((ordenes.IdOrden) = {0}));", IDOrden), conn);
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
        public static string ActualizarPT(string ValorResultado, string Comentario, int IdUsuario, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("ActualizarResultado", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("ValorResultado", MySqlDbType.VarChar).Value = ValorResultado;
                cmd2.Parameters.Add("Comentario", MySqlDbType.VarChar).Value = Comentario;
                cmd2.Parameters.Add("IdUsuario", MySqlDbType.Int32).Value = IdUsuario;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis;
                conn.Open();
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
                conn.Close();
            }
        }
        public static DataSet SelectPTT(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.resultadospaciente Where IDOrden = {0} and (IdAnalisis = 129 Or IdAnalisis =128 Or IdAnalisis =127);", IDOrden), conn);
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
        public static string ActualizarPTT(string cmd, int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE `sistema2020`.`resultadospaciente` SET {0}  Where `IdOrden` = {1} And `IdAnalisis` = {2}", cmd, IDOrden, IdAnalisis);
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
        public static DataSet SELECTAGRUPADOR(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                string query = string.Format("SELECT analisislaboratorio.IdAgrupador FROM analisislaboratorio join ResultadosPaciente WHERE(((analisislaboratorio.IdAnalisis) = {1}) AND((ResultadosPaciente.IdOrden) = {0}));", cmd, cmd2);
                adapter.SelectCommand = new MySqlCommand(query, conn);
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
        public static DataSet SELECTIMPRIMIRSECCION(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();

            try
            {

                conn.Open();
                string query = string.Format("SELECT analisislaboratorio.NombreAnalisis,analisislaboratorio.IdAgrupador, analisislaboratorio.IdAnalisis,analisislaboratorio.Titulo,analisislaboratorio.FinalTitulo, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, ResultadosPaciente.IdOrganizador, datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Fecha, datospersonales.Sexo, datospersonales.TipoCorreo, datospersonales.Correo, ResultadosPaciente.ValorMenor, ResultadosPaciente.ValorMayor, ResultadosPaciente.MultiplesValores, analisislaboratorio.TipoAnalisis, ResultadosPaciente.EstadoDeResultado, MayoroMenorReferencial.Lineas, ResultadosPaciente.IdOrden, usuarios.SIGLAS, ResultadosPaciente.FechaIngreso as FechaImp FROM datospersonales RIGHT JOIN((analisislaboratorio LEFT JOIN MayoroMenorReferencial ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN(usuarios RIGHT JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datospersonales.IdPersona = ResultadosPaciente.IdPaciente WHERE(((analisislaboratorio.IdAnalisis) = {1}) AND((ResultadosPaciente.IdOrden) = {0}));", cmd, cmd2);
                adapter.SelectCommand = new MySqlCommand(query, conn);
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
        public static DataSet SELECTIMPRIMIRSECCIONAGRUPADOR(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                string query = string.Format("SELECT analisislaboratorio.NombreAnalisis,analisislaboratorio.IdAgrupador, analisislaboratorio.IdAnalisis,analisislaboratorio.Titulo,analisislaboratorio.FinalTitulo, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, ResultadosPaciente.IdOrganizador, datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Fecha, datospersonales.Sexo, datospersonales.TipoCorreo, datospersonales.Correo, ResultadosPaciente.ValorMenor, ResultadosPaciente.ValorMayor, ResultadosPaciente.MultiplesValores, analisislaboratorio.TipoAnalisis, ResultadosPaciente.EstadoDeResultado, MayoroMenorReferencial.Lineas, ResultadosPaciente.IdOrden, usuarios.SIGLAS, ResultadosPaciente.FechaIngreso as FechaImp FROM datospersonales RIGHT JOIN((analisislaboratorio LEFT JOIN MayoroMenorReferencial ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN(usuarios RIGHT JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datospersonales.IdPersona = ResultadosPaciente.IdPaciente WHERE(((analisislaboratorio.IdAgrupador) = {1}) AND((ResultadosPaciente.IdOrden) = {0})) Order By IdOrganizador asc;", cmd, cmd2);
                adapter.SelectCommand = new MySqlCommand(query, conn);
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
        public static DataSet SELECTIMPRIMIRTOTAL(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                string query = string.Format("SELECT analisislaboratorio.NombreAnalisis, analisislaboratorio.IdAnalisis,analisislaboratorio.Titulo,analisislaboratorio.FinalTitulo, ResultadosPaciente.ValorResultado, ResultadosPaciente.Unidad, ResultadosPaciente.Comentario, ResultadosPaciente.IdOrganizador, datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Fecha, datospersonales.Sexo, datospersonales.TipoCorreo, datospersonales.Correo, ResultadosPaciente.ValorMenor, ResultadosPaciente.ValorMayor, ResultadosPaciente.MultiplesValores, analisislaboratorio.TipoAnalisis, ResultadosPaciente.EstadoDeResultado, MayoroMenorReferencial.Lineas, ResultadosPaciente.IdOrden, usuarios.SIGLAS, ResultadosPaciente.FechaIngreso as FechaImp FROM datospersonales RIGHT JOIN((analisislaboratorio LEFT JOIN MayoroMenorReferencial ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN(usuarios RIGHT JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datospersonales.IdPersona = ResultadosPaciente.IdPaciente WHERE(((ResultadosPaciente.IdOrden) = {0}) And estadoderesultado > 1) ORDER BY ResultadosPaciente.IdOrganizador;", cmd);
                adapter.SelectCommand = new MySqlCommand(query, conn);
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
        public static string ActualizarLipidograma(string cmd, int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ReenviarReferido(string IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ReenviarReferidoEspeciales(string IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  Enviado='0' , EstadoDeResultado = 2 Where IdOrden = {0}", IDOrden);
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
        public static string ReimprimirEtiquetas(string IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ordenes SET  Muestra='0' Where IdOrden = {0}", IDOrden);
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
        public static DataSet SELECTTotalFacturado()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT FORMAT(sum(preciof),2) AS SumaDePrecioF,FORMAT(sum(pago),2) AS Pago,FORMAT(sum(total),2) AS total FROM sistema2020.cobrodiario where Fecha = Curdate();"), conn);
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
        public static DataSet SELECTTotalFacturadoCierre(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT FORMAT(sum(preciof),2) AS SumaDePrecioF,FORMAT(sum(pago),2) AS Pago,FORMAT(sum(total),2) AS total FROM sistema2020.`auditoriacierre` where Fecha = '{0}'", cmd), conn);
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
        public static DataSet TotalDetallado()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT tipodepago, Round(SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=1,ROUND(ValorResultado,2),0),if(Clasificacion=1,Cantidad,0))),2)  as Entradas,Round(SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Salidas, ROUND(SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=1,ValorResultado,0),if(Clasificacion=1,Cantidad,0))) - SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Total FROM `sistema2020`.`pagos` left join ordenes on ordenes.IDOrden = Pagos.IdOrden Where Pagos.Fecha = CURDATE() and  EstadodeOrden < 3 Group by TipodePago", DateTime.Now.ToString("yyyy/MM/dd")), conn);
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
        public static DataSet TotalDetalladoPorFecha(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT tipodepago, Round(SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=1,ROUND(ValorResultado,2),0),if(Clasificacion=1,Cantidad,0))),2)  as Entradas,Round(SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Salidas, ROUND(SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=1,ValorResultado,0),if(Clasificacion=1,Cantidad,0))) - SUM(If(Moneda=1 or Moneda=2,if(Clasificacion=2,ValorResultado,0),if(Clasificacion=2,Cantidad,0))),2) as Total FROM `sistema2020`.`pagos` left join ordenes on ordenes.IDOrden = Pagos.IdOrden Where Pagos.Fecha >= '{0}' and Pagos.Fecha <= '{1}' and EstadodeOrden < 3 Group by TipodePago; ", cmd, cmd2), conn);
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
        public static string ActualizarPrecios(string cmd, string cmd3, string cmd4)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("ActualizarPrecios", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("tasadia", MySqlDbType.VarChar).Value = cmd;
                cmd2.Parameters.Add("Pesos", MySqlDbType.VarChar).Value = cmd3;
                cmd2.Parameters.Add("Euros", MySqlDbType.VarChar).Value = cmd4;
                conn.Open();
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
                conn.Close();
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
        string ordenesI, string ordenesE, string ordenesT,
        string OtrosIngresosI, string OtrosIngresosV, string OtrosIngresosT,
        string TotalFacturado, string TotalCobrado, string Diferencia
        )
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarCierre", conn);
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
                cmd2.Parameters.Add("ordenesI", MySqlDbType.VarChar).Value = ordenesI;
                cmd2.Parameters.Add("ordenesE", MySqlDbType.VarChar).Value = ordenesE;
                cmd2.Parameters.Add("ordenesT", MySqlDbType.VarChar).Value = ordenesT;
                cmd2.Parameters.Add("OtrosIngresosI", MySqlDbType.VarChar).Value = OtrosIngresosI;
                cmd2.Parameters.Add("OtrosIngresosV", MySqlDbType.VarChar).Value = OtrosIngresosV;
                cmd2.Parameters.Add("OtrosIngresosT", MySqlDbType.VarChar).Value = OtrosIngresosT;
                cmd2.Parameters.Add("TotalFacturado", MySqlDbType.VarChar).Value = TotalFacturado;
                cmd2.Parameters.Add("TotalCobrado", MySqlDbType.VarChar).Value = TotalCobrado;
                cmd2.Parameters.Add("Diferencia", MySqlDbType.VarChar).Value = Diferencia;
                conn.Open();
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
                conn.Close();
            }
        }
        public static DataSet SELECTordenes(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.vistaanalisisconfecha Where Fecha >= '{0}' and Fecha <= '{1}'; ", cmd, cmd2), conn);
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
        } //
        public static DataSet SELECTGrupoOrdenes(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT NombreAnalisis FROM sistema2020.vistaanalisisconfecha Where Fecha >= '{0}' and Fecha <= '{1}' Group By NombreAnalisis; ", cmd, cmd2), conn);
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
        } //
        public static DataSet SELECTReportados(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(`resultadospaciente`.`IdOrden`) as Reportados FROM `sistema2020`.`resultadospaciente` join analisislaboratorio on analisislaboratorio.IdAnalisis = Resultadospaciente.IdAnalisis WHERE(((`resultadospaciente`.`FechaIngreso`) >= '{0}' And(`resultadospaciente`.`FechaIngreso`) <= '{1}')) and EstadoDeResultado > 0 and `analisislaboratorio`.`Visible` = 1 ", cmd, cmd2), conn);
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
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(`resultadospaciente`.`IdOrden`) as PorReportar FROM `sistema2020`.`resultadospaciente` left join analisislaboratorio on analisislaboratorio.IdAnalisis = Resultadospaciente.IdAnalisis WHERE(((`resultadospaciente`.`FechaIngreso`) >= '{0}' And(`resultadospaciente`.`FechaIngreso`) <= '{1}')) and EstadoDeResultado is null and `analisislaboratorio`.`Visible` = 1", cmd, cmd2), conn);
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
        public static List<AnalisisLaboratorio> SelectListadeAnalisis()
        {
            List<AnalisisLaboratorio> listaDeAnalisis = new List<AnalisisLaboratorio>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                int IdAnalisis = 0, Especiales = 0;
                bool Visible = false;
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * From sistema2020.AnalisisLaboratorio"), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        int.TryParse(r["IdAnalisis"].ToString(), out IdAnalisis);
                        int.TryParse(r["Especiales"].ToString(), out Especiales);
                        if (r["Visible"].ToString() == "1")
                        {
                            Visible = true;
                        }
                        AnalisisLaboratorio analisis = new AnalisisLaboratorio();
                        analisis.IdAnalisis = IdAnalisis;
                        analisis.NombreAnalisis = r["NombreAnalisis"].ToString();
                        analisis.Etiqueta = r["Etiquetas"].ToString();
                        analisis.Especiales = Especiales;
                        analisis.Visible = Visible;
                        analisis.valoresDeReferencia = selectValorDeReferencia(analisis.IdAnalisis);
                        listaDeAnalisis.Add(analisis);
                    }
                }

                return listaDeAnalisis;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return listaDeAnalisis;
            }
            finally
            {
                conn.Close();
            }
        }//
        public static mayoromenorreferencial selectValorDeReferencia(int IdAnalisis)
        {
            mayoromenorreferencial mayoromenorreferencial = new mayoromenorreferencial();
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format($"SELECT * From sistema2020.mayoromenorreferencial Where IdReferencial = {IdAnalisis}"), conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        mayoromenorreferencial.IdAnalisis = (int)r["IdReferencial"];
                        mayoromenorreferencial.ValorMayor = r["ValorMayor"].ToString();
                        mayoromenorreferencial.ValorMenor = r["ValorMenor"].ToString();
                        mayoromenorreferencial.Unidad = r["Unidad"].ToString();
                        mayoromenorreferencial.MultiplesValores = r["MultiplesValores"].ToString();
                    }
                }

                return mayoromenorreferencial;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return mayoromenorreferencial;
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
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(`resultadospaciente`.`IdOrden`) as Total FROM `sistema2020`.`resultadospaciente` left join analisislaboratorio on analisislaboratorio.IdAnalisis = Resultadospaciente.IdAnalisis WHERE(((`resultadospaciente`.`FechaIngreso`) >= '{0}' And(`resultadospaciente`.`FechaIngreso`) <= '{1}')) and `analisislaboratorio`.`Visible` = 1", cmd, cmd2), conn);
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
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.Fecha, PerfilesFacturados.IdOrden AS IdOrden, ordenes.NumeroDia AS NumeroDia, datospersonales.Cedula,CONCAT(datospersonales.Nombre,' ', datospersonales.Apellidos) as Nombre,CONCAT(datospersonales.CodigoTelefono,'-', datospersonales.Telefono) as Telefono,CONCAT(datospersonales.CodigoCelular,'-', datospersonales.Celular) as Celular, Perfil.NombrePerfil AS NombreAnalisis,PrecioPerfil as Bolivares,ROUND(PrecioPerfil/Dolar,2) as Dolares FROM(Perfil INNER JOIN((datospersonales INNER JOIN PerfilesFacturados ON datospersonales.IdPersona = PerfilesFacturados.IdPersona) INNER JOIN ordenes ON(ordenes.IdOrden = PerfilesFacturados.IdOrden) AND(datospersonales.IdPersona = ordenes.IdPersona)) ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio left join tasadia on tasadia.Id = ordenes.Idtasa WHERE(((ordenes.Fecha) >= '{0}' And(ordenes.Fecha) <= '{1}')) ORDER BY PerfilesFacturados.IdOrden;  ", cmd, cmd2), conn);
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
        } //
        public static DataSet TotalFacturadoPorOrden(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.Fecha, ordenes.IdOrden AS IdOrden, ordenes.NumeroDia AS NumeroDia, datospersonales.Cedula,CONCAT(datospersonales.Nombre,' ', datospersonales.Apellidos) as Nombre,ROUND(PrecioF,2) as Bolivares,ROUND(PrecioF/Dolar,2) as Dolares FROM ordenes left join tasadia on Tasadia.Id = ordenes.IdTasa left join datospersonales on datospersonales.IdPersona = ordenes.IdPersona WHERE(((ordenes.Fecha) >= '{0}' And(ordenes.Fecha) <= '{1}')) ORDER BY ordenes.IdOrden;  ", cmd, cmd2), conn);
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
        } //
        public static DataSet SELECTConteoReferidos(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(Especiales) From ResultadosPaciente inner join analisislaboratorio on analisislaboratorio.IDAnalisis = resultadospaciente.IdAnalisis Where (analisislaboratorio.Especiales = 1) And (FechaIngreso >= '{0}'and FechaIngreso <= '{1}') and analisislaboratorio.Visible = 1;", cmd, cmd2), conn);
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
        public static DataSet SELECTTotalFacturadoFecha(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ROUND(SUM(CONVERT(ordenes.preciof,DECIMAL(10,2))),2) AS SumaDePrecioF,ROUND(SUM(preciof/dolar),2) as Dolares FROM sistema2020.ordenes join Tasadia on ordenes.IDTasa = Tasadia.ID Where ordenes.Fecha >= '{0}' and ordenes.Fecha  <= '{1}' and EstadoDeOrden  < 3", cmd, cmd2), conn);
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
        public static DataSet SELECTTotalFacturadoDia(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ROUND(SUM(CONVERT(IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0),DECIMAL(10,2))) - SUM(IF((`pagos`.`Clasificacion` = 2), `pagos`.`ValorResultado`,0)),2) AS `Total` FROM sistema2020.pagos join ordenes on ordenes.IdOrden = PAgos.IdOrden where Pagos.Fecha >= '{0}' and Pagos.Fecha <= '{1}' and EstadodeOrden < 3", cmd, cmd2), conn);
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
        public static DataSet SELECTTotalPagoFecha(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT SUM(IF((`pagos`.`Clasificacion` = 1),`pagos`.`ValorResultado`,0)) - SUM(IF((`pagos`.`Clasificacion` = 2), `pagos`.`ValorResultado`,0)) AS `Pago` FROM sistema2020.pagos join ordenes on ordenes.IdOrden = PAgos.IdOrden where Pagos.Fecha >= '{0}' and Pagos.Fecha <= '{1}' and EstadodeOrden < 3 ", cmd, cmd2), conn);
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
        public static DataSet SecuenciaDeTrabajoPorFecha(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, ordenes.Fecha,ordenes.HoraIngreso,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}' AND ordenes.estadodeorden < 3 )) ORDER BY ordenes.IdOrden ", cmd, cmd2), conn);
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
        public static DataSet SecuenciaDeTrabajoPorID(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, ordenes.Fecha,ordenes.HoraIngreso,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio WHERE(((ordenes.IdOrden) = {0})) ORDER BY ordenes.IdOrden; ", cmd), conn);
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
        public static DataSet SecuenciaDeTrabajoNombreApellido(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, ordenes.Fecha,ordenes.HoraIngreso,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio  Where (((datospersonales.Nombre)Like '{0}%') AND((datospersonales.Apellidos)Like '{1}%')) ORDER BY ordenes.IdOrden Limit 100; ", cmd, cmd2), conn);
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
        public static DataSet SecuenciaDeTrabajoPorCedula(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.IdOrden, ordenes.NumeroDia, ordenes.Fecha,ordenes.HoraIngreso,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos,  convenios.Nombre FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio HAVING(((datospersonales.Cedula) = '{0}'));", cmd), conn);
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
        public static string ActualizarOrden(string cmd3, int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ActualizarPorEnviar(string IDOrden, string IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static DataSet SELECTReferidos(string cmd, string cmd2)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden,resultadospaciente.IdAnalisis,datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Fecha, datospersonales.Sexo, analisislaboratorio.NombreAnalisis, analisislaboratorio.IdSeccion, ResultadosPaciente.FechaIngreso, ordenes.NumeroDia FROM(datospersonales INNER JOIN(analisislaboratorio INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON datospersonales.IdPersona = ResultadosPaciente.IdPaciente) INNER JOIN ordenes ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ordenes.IdPersona) WHERE analisislaboratorio.Especiales = 1 And PorEnviar = 0 AND((ResultadosPaciente.FechaIngreso) >='{0}' And (ResultadosPaciente.FechaIngreso)<='{1}') and EstadoDeOrden < 3 ORDER BY fechaIngreso asc, ordenes.NumeroDia asc,IdOrganizador asc; ", cmd, cmd2), conn);
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
        public static DataSet SELECTordenesPacientes(string cmd, string cmd2)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.NumeroDia as '#',ordenes.IdOrden,Cedula,CONCAT(datospersonales.Nombre,' ',Apellidos) as Nombre,precioF AS Facturado FROM sistema2020.ordenes left join pagos on ordenes.IdOrden = Pagos.IDOrden left join datospersonales on ordenes.IdPersona = datospersonales.IdPersona  Where (((Pagos.Fecha) >='{0}' And (Pagos.Fecha)<= '{1}') Or (ordenes.Fecha >= '{0}' And (ordenes.Fecha)<= '{1}')) and EstadoDeOrden  < 3  GROUP BY IdOrden Order By IdOrden asc;", cmd, cmd2), conn);
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
        public static DataSet SELECTordenesPacientesCobro(string cmd, string cmd2)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.NumeroDia as '#',ordenes.IdOrden,Cedula,CONCAT(datospersonales.Nombre,' ',Apellidos) as Nombre,precioF AS Facturado FROM sistema2020.ordenes left join pagos on ordenes.IdOrden = Pagos.IDOrden left join datospersonales on ordenes.IdPersona = datospersonales.IdPersona  Where (((Pagos.Fecha) >='{0}' And (Pagos.Fecha)<= '{1}') Or (ordenes.Fecha >= '{0}' And (ordenes.Fecha)<= '{1}')) and EstadoDeOrden  < 3  GROUP BY IdOrden Order By IdOrden asc;", cmd, cmd2), conn);
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
        public static DataSet SELECTordenesPacientesAnulados(string cmd, string cmd2)
        {
            DataSet ds = new DataSet();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.NumeroDia as '#',ordenes.IdOrden,Cedula,CONCAT(datospersonales.Nombre,' ',Apellidos) as Nombre,precioF AS Facturado FROM sistema2020.ordenes left join pagos on ordenes.IdOrden = Pagos.IDOrden left join datospersonales on ordenes.IdPersona = datospersonales.IdPersona  Where (((Pagos.Fecha) >='{0}' And (Pagos.Fecha)<= '{1}') Or (ordenes.Fecha >= '{0}' And (ordenes.Fecha)<= '{1}')) and EstadoDeOrden  = 3  GROUP BY IdOrden Order By IdOrden asc;", cmd, cmd2), conn);
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
        public static DataSet SELECTPagosordenesPacientes(string cmd, string cmd2, string cmd3)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT round((if(Clasificacion=1,ValorResultado,0)),2) as Entradas,round((if(Clasificacion=2,ValorResultado,0)),2) as Salidas,ROUND(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2) AS Pago,IF(ordenes.Fecha != Pagos.Fecha,ROUND(SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2),ROUND(- PrecioF + SUM(if(Clasificacion=1,ValorResultado,0)-if(Clasificacion=2,ValorResultado,0)),2)) As Total FROM sistema2020.ordenes left join pagos on ordenes.IdOrden = Pagos.IDOrden left join datospersonales on ordenes.IdPersona = datospersonales.IdPersona  Where ordenes.IdOrden = {0} and EstadoDeOrden  < 3 and Pagos.fecha >= '{1}' and Pagos.fecha <= '{2}' GROUP BY ordenes.IdOrden Order By ordenes.IdOrden asc;", cmd, cmd2, cmd3), conn);
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
        public static DataSet Bioanalista(int OrdenID, int AnalisisID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.IdUsuario,usuarios.NombreUsuario,usuarios.CB,usuarios.MPPS,usuarios.FIRMA From usuarios Right join Resultadospaciente on resultadospaciente.IdUsuario = usuarios.IdUsuario Where IdOrden = {0} And IdAnalisis = {1}", OrdenID, AnalisisID), conn);
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
        public static DataSet PorReportar(int OrdenID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT COUNT(EstadoDeResultado) FROM resultadospaciente Inner join analisislaboratorio on ResultadosPaciente.IdAnalisis = analisislaboratorio.IdAnalisis Where IdOrden = {0} AND EstadoDeResultado < 2 and analisislaboratorio.tipoanalisis <> 15", OrdenID), conn);
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
        public static DataSet SELECPrivilegios(int ID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.Privilegios FROM usuarios WHERE(((usuarios.IdUsuario) = {0}));", ID), conn);
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
        public static DataSet CantidadDeHojasFecha(string FechaDesde, string FechaHasta)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT SUM(Cantidad) AS Total From Impresiones Where Fecha >= '{0}' and  Fecha <= '{1}'", FechaDesde, FechaHasta), conn);
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
        public static DataSet Estadisticas(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.NombrePerfil, Count(PerfilesFacturados.IdPerfil) AS CuentaDeIdPerfil FROM Perfil INNER JOIN(ordenes INNER JOIN PerfilesFacturados ON ordenes.IdOrden = PerfilesFacturados.IdOrden) ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}'))  AND ordenes.IdConvenio <= 3 and ordenes.EstadoDeOrden < 3 GROUP BY Perfil.NombrePerfil Order by NombrePerfil asc", cmd, cmd2), conn);
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
        private void oculto()
        {

        }
        public static DataSet EstadisticasAlmacen(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT resultadospaciente.IdAnalisis,NombreAnalisis, Count(`resultadospaciente`.`IdAnalisis`) as Cantidad FROM `sistema2020`.`resultadospaciente` left join analisislaboratorio on analisislaboratorio.IdAnalisis = resultadospaciente.IDAnalisis left join ordenes on ordenes.IdOrden = ResultadosPaciente.IdOrden Where fechaIngreso >= '{0}' AND fechaIngreso <= '{1}' and ordenes.IdConvenio <= 3 and  analisislaboratorio.Visible = 1 and ordenes.EstadoDeOrden < 3 group by resultadospaciente.IdAnalisis order By NombreAnalisis asc", cmd, cmd2), conn);
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
        public static DataSet EstadisticasPorSede(string cmd, string cmd2, string IdConvenio)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Perfil.NombrePerfil, Count(PerfilesFacturados.IdPerfil) AS CuentaDeIdPerfil FROM Perfil INNER JOIN(ordenes INNER JOIN PerfilesFacturados ON ordenes.IdOrden = PerfilesFacturados.IdOrden) ON Perfil.IdPerfil = PerfilesFacturados.IdPerfil WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}'))  AND ordenes.IdConvenio = {2}  GROUP BY Perfil.NombrePerfil Order by NombrePerfil asc", cmd, cmd2, IdConvenio), conn);
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
        public static DataSet EstadisticasEspeciales(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT resultadospaciente.IDAnalisis,NombreAnalisis,Count(resultadospaciente.IDAnalisis) AS Cantidad FROM sistema2020.resultadospaciente inner join analisislaboratorio on resultadospaciente.IdAnalisis = analisislaboratorio.IdAnalisis WHERE(((resultadospaciente.FechaIngreso) >='{0}' And (resultadospaciente.FechaIngreso)<='{1}')) And `analisislaboratorio`.`Visible` = 1 And EstadoDeResultado > 1 Group by IDAnalisis,IdAgrupador Order by IdOrganizador asc;", cmd, cmd2), conn);
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
        public static DataSet EstadisticasPorConvenio(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT IdConvenio,Count(resultadospaciente.IDAnalisis) AS Cantidad FROM sistema2020.resultadospaciente inner join analisislaboratorio on resultadospaciente.IdAnalisis = analisislaboratorio.IdAnalisis WHERE(((resultadospaciente.FechaIngreso) >='{0}' And (resultadospaciente.FechaIngreso)<='{1}')) And `analisislaboratorio`.`Visible` = 1 Group by IdConvenio Order by IdConvenio asc;", cmd, cmd2), conn);
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
        public static DataSet TotalPersonasFacturadas(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(ordenes.IdOrden) AS CuentaDeIdOrden FROM ordenes WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}'))  AND ordenes.IdConvenio <= 3 and ordenes.EstadodeOrden < 3; ", cmd, cmd2), conn);
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
        public static DataSet TotalPersonasFacturadasAlmacen(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(ordenes.IdOrden) AS CuentaDeIdOrden FROM ordenes WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}'))  AND ordenes.IdConvenio <= 3 and ordenes.EstadoDeOrden < 3; ", cmd, cmd2), conn);
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
        public static DataSet TotalPersonasFacturadasPorSede(string cmd, string cmd2, string IdConvenio)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(ordenes.IdOrden) AS CuentaDeIdOrden FROM ordenes WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}'))  AND ordenes.IdConvenio = {2}; ", cmd, cmd2, IdConvenio), conn);
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
        public static DataSet AnalisisRealizados(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(ordenes.IdOrden) AS CuentaDeIdOrden FROM ordenes WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{0}'));", cmd, cmd2), conn);
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
        public static DataSet EstadisticasAnalisis(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT NombreAnalisis,Count(resultadospaciente.IdAnalisis) as Cantidad FROM sistema2020.resultadospaciente join analisislaboratorio on analisislaboratorio.IdAnalisis = resultadospaciente.IdAnalisis join ordenes on ordenes.IdOrden = Resultadospaciente.IdOrden where ((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}') group by NombreAnalisis;", cmd, cmd2), conn);
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
        public static DataSet BioanalistasAnalisis(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.Fecha, usuarios.NombreUsuario, analisislaboratorio.NombreAnalisis, ResultadosPaciente.`HoraValidacion` FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((analisislaboratorio.Visible) = 1)) And (((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}')) Order by Fecha,HoraValidacion; ", cmd, cmd2), conn);
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
        public static DataSet MonedaExtranjera(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.reportemonedaextranjera Where Fecha >=  '{0}' and Fecha <=  '{1}' and (Euros <> 0 or Pesos <> 0 or `$ Recibido`<> 0 ); ", cmd, cmd2), conn);
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
        public static DataSet ReporteCobro(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.ReporteCobro2 Where Fecha >=  '{0}' and Fecha <=  '{1}'; ", cmd, cmd2), conn);
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
        public static List<Perfil> selectPerfilesPorOrden(int idOrden)
        {
            List<Perfil> Perfiles = new List<Perfil>();
            Perfil perfil = new Perfil();
            DataTable dt = new DataTable();
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format($"SELECT IdPerfil FROM sistema2020.perfilesfacturados where idOrden = {idOrden};"), con);
                adapter.Fill(dt);
                adapter.Dispose();
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
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return Perfiles;
            }
            finally
            {
                con.Close();
            }
        }
        public static List<OrdenesEstadistica> Estadistica2(string cmd, string cmd2)
        {
            List<OrdenesEstadistica> estadisticas = new List<OrdenesEstadistica>();

            List<Perfil> Perfiles = new List<Perfil>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * From Sistema2020.Ordenes Where Ordenes.Fecha >= '{cmd}' and Ordenes.Fecha <= '{cmd2}' and EstadoDeOrden < 3", conn);
                adapter.Fill(ds);
                adapter.Dispose();
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return estadisticas;
            }
            finally
            {
                conn.Close();
            }

            if (ds.Rows.Count > 0)
            {
                foreach (DataRow r in ds.Rows)
                {
                    OrdenesEstadistica estadistica2 = new OrdenesEstadistica();
                    estadistica2.idOrden = (int)r["idOrden"];
                    estadistica2.IdDatosPaciente = (int)r["IdPersona"];
                    estadistica2.Fecha = (DateTime)r["Fecha"];
                    estadistica2.Hora = r["Fecha"].ToString();
                    estadistica2.IdUsuario = (int)r["Usuario"];
                    estadistica2.IdConvenio = (int)r["IdConvenio"];
                    estadistica2.EstadoDeOrden = (int)r["EstadoDeOrden"];
                    estadistica2.datosPaciente = selectDatosPacientePorId(estadistica2.IdDatosPaciente);
                    estadistica2.convenio = selectConvenioPorID(estadistica2.IdConvenio);
                    estadistica2.perfiles = selectPerfilesPorOrden(estadistica2.idOrden);
                    estadisticas.Add(estadistica2);

                }

            }

            return estadisticas;
        }
        public static Convenios selectConvenioPorID(int IdConvenio)
        {
            Convenios convenios = new Convenios();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.convenios where IdConvenio = {0};", IdConvenio), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        convenios.IdConvenio = (int)ds.Tables[0].Rows[0]["IdConvenio"];
                        convenios.Nombre = ds.Tables[0].Rows[0]["Nombre"].ToString();
                        convenios.Descuento = ds.Tables[0].Rows[0]["Descuento"].ToString();
                        convenios.Correo = ds.Tables[0].Rows[0]["CorreoSede"].ToString();
                        convenios.Activos = (bool)ds.Tables[0].Rows[0]["Activos"];

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
        public static List<AnalisisLaboratorio> selectAnalisisAgrupadosPorPerfil(int idPerfil)
        {
            List<AnalisisLaboratorio> analisisLaboratorios = new List<AnalisisLaboratorio>();

            MySqlConnection conn = new MySqlConnection(connection);
            bool Visible = false;
            int IdAnalisis = 0, TipoAnalisis = 0, IdSeccion = 0,
                Especiales = 0, Titulo = 0, IdAgrupador = 0, FinalTitulo = 0, idOrganizador = 0;
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * from perfilesAnalisis inner join analisislaboratorio on PerfilesAnalisis.IdAnalisis = AnalisisLaboratorio.idAnalisis Where IdPerfil = {idPerfil}", conn);
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
                        bool.TryParse(row["Visible"].ToString(), out Visible);
                        analisis.Visible = Visible;
                        analisis.Etiqueta = row["Etiquetas"].ToString();
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
        public static List<AnalisisLaboratorio> selectListaAnalisis()
        {
            List<AnalisisLaboratorio> analisisLaboratorios = new List<AnalisisLaboratorio>();

            MySqlConnection conn = new MySqlConnection(connection);
            bool Visible = false;
            int IdAnalisis = 0, TipoAnalisis = 0, IdSeccion = 0,
                Especiales = 0, Titulo = 0, IdAgrupador = 0, FinalTitulo = 0, idOrganizador = 0;
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * from analisislaboratorio left join perfilesAnalisis on perfilesAnalisis.IdAnalisis = analisislaboratorio.IdAnalisis group by analisislaboratorio.IdAnalisis", conn);
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
                        bool.TryParse(row["Visible"].ToString(), out Visible);
                        analisis.Visible = Visible;
                        analisis.Etiqueta = row["Etiquetas"].ToString();
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
        public static DatosDePaciente selectDatosPacientePorId(int idRepresentante)
        {
            DatosDePaciente datosDePaciente = new DatosDePaciente();
            MySqlConnection con = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                con.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.datospersonales where IdPersona = {0};", idRepresentante), con);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Tables.Count > 0)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        datosDePaciente.IdPersona = (int)ds.Tables[0].Rows[0]["IdPersona"];
                        datosDePaciente.Cedula = ds.Tables[0].Rows[0]["Cedula"].ToString();
                        datosDePaciente.Nombre = ds.Tables[0].Rows[0]["Nombre"].ToString();
                        datosDePaciente.Apellidos = ds.Tables[0].Rows[0]["Apellidos"].ToString();
                        datosDePaciente.Fecha = Convert.ToDateTime(ds.Tables[0].Rows[0]["Fecha"].ToString());
                        datosDePaciente.CodigoCelular = ds.Tables[0].Rows[0]["CodigoCelular"].ToString();
                        datosDePaciente.Celular = ds.Tables[0].Rows[0]["Celular"].ToString();
                        datosDePaciente.CodigoTelefono = ds.Tables[0].Rows[0]["CodigoTelefono"].ToString();
                        datosDePaciente.Telefono = ds.Tables[0].Rows[0]["Telefono"].ToString();
                        datosDePaciente.TipoCorreo = ds.Tables[0].Rows[0]["TipoCorreo"].ToString();
                        datosDePaciente.Correo = ds.Tables[0].Rows[0]["Correo"].ToString();
                        datosDePaciente.Edad = FechaSinAnio(datosDePaciente.Fecha);
                    }
                }
                return datosDePaciente;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return datosDePaciente;
            }
            finally
            {
                con.Close();
            }
        }
        public static DataSet SerialDeBillete(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("Select * From `pagos` where `pagos`.`Serial` like '%{0}%' ", cmd), conn);
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
        public static DataSet BioanalistasAnalisisHora(string cmd, string cmd2, string cmd3, string cmd4)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ordenes.Fecha, usuarios.NombreUsuario, analisislaboratorio.NombreAnalisis, ResultadosPaciente.`HoraValidacion` FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((analisislaboratorio.Visible) = 1)) And (((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}') And (HoraValidacion >= '{2}' And HoraValidacion <= '{3}')) Order by Fecha,HoraValidacion; ", cmd, cmd2, cmd3, cmd4), conn);
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
        public static DataSet BioanalistasAnalisisHoraAgrupado(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((analisislaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) Group by NombreUsuario,Hour(HoraValidacion),Fecha Order by Fecha,HoraValidacion; ", cmd, cmd2), conn);
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
        public static DataSet BioanalistasAnalisisConteoPorNombre(string cmd, string cmd2, string cmd3)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((analisislaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) and NombreUsuario = '{2}' Group by NombreUsuario,Hour(HoraValidacion),Fecha Order by Fecha,HoraValidacion; ", cmd, cmd2, cmd3), conn);
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
        public static DataSet BioanalistasAnalisisHoraAgrupadosPorPersona(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((analisislaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) and analisislaboratorio.Especiales <> 1 Group by NombreUsuario", cmd, cmd2), conn);
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
        public static DataSet BioanalistasAnalisisHoraAgrupadosPorPersonaConEspeciales(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha, Hour(ResultadosPaciente.`HoraValidacion`) AS HORA, usuarios.NombreUsuario as USUARIO, COUNT(Hour(ResultadosPaciente.`HoraValidacion`)) AS CANTIDAD FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((analisislaboratorio.Visible) = 1)) And(((ResultadosPaciente.FechaValidacion) >= '{0}' And(ResultadosPaciente.FechaValidacion) <= '{1}')) Group by NombreUsuario", cmd, cmd2), conn);
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
        public static DataSet BioanalistasAnalisisContador(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.NombreUsuario, Count(usuarios.NombreUsuario) AS CuentaDeNombreUsuario FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((ResultadosPaciente.FechaValidacion) >='{0}' And (ResultadosPaciente.FechaValidacion)<='{1}')) GROUP BY usuarios.NombreUsuario, analisislaboratorio.Visible HAVING(((analisislaboratorio.Visible) = 1)); ", cmd, cmd2), conn);
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
        public static DataSet BioanalistasAnalisisContadorHora(string cmd, string cmd2, string cmd3, string cmd4)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.NombreUsuario, Count(usuarios.NombreUsuario) AS CuentaDeNombreUsuario FROM ordenes INNER JOIN(analisislaboratorio INNER JOIN(usuarios INNER JOIN ResultadosPaciente ON usuarios.IdUsuario = ResultadosPaciente.IdUsuario) ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON ordenes.IdOrden = ResultadosPaciente.IdOrden WHERE(((ordenes.Fecha) >='{0}' And (ordenes.Fecha)<='{1}') And (HoraValidacion >= '{2}' And HoraValidacion <= '{3}')) GROUP BY usuarios.NombreUsuario, analisislaboratorio.Visible HAVING(((analisislaboratorio.Visible) = 1)); ", cmd, cmd2, cmd3, cmd4), conn);
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
        public static DataSet TeclasYPrivilegios(int Usuario)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.IdUsuario, usuarios.NombreUsuario, usuarios.Privilegios, TeclasPorUsuario.Leucocitos, TeclasPorUsuario.Neutrofilos, TeclasPorUsuario.Linfocitos, TeclasPorUsuario.Monocitos, TeclasPorUsuario.Eosinofilos, TeclasPorUsuario.Basofilos, TeclasPorUsuario.Plaquetas FROM usuarios LEFT JOIN TeclasPorUsuario ON usuarios.IdUsuario = TeclasPorUsuario.IdUsuario WHERE(((usuarios.IdUsuario) = {0}));", Usuario), conn);
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
        public static string EstadoOrden(int IdOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ordenes SET ordenes.VALIDADA = 2 WHERE(((ordenes.IdOrden) = {0})); ", IdOrden);
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
        public static DataSet HematologiaEspecial(int IDOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ordenes.Fecha as FechaImp, ordenes.NumeroDia, ResultadosPaciente.IdUsuario, ResultadosPaciente.IdAnalisis, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, ResultadosPaciente.Comentario, HemaEspecial.Neutrofilos, HemaEspecial.linfocitos, HemaEspecial.Monocitos, HemaEspecial.Eosinofilos, HemaEspecial.Basofilos, HemaEspecial.Hematies, HemaEspecial.Hemoglobina, HemaEspecial.Hematocritos, HemaEspecial.VCM, HemaEspecial.HCM, HemaEspecial.CHCM, HemaEspecial.Plaquetas, HemaEspecial.Neutrofilos2, HemaEspecial.Linfocitos2, HemaEspecial.Monocitos2, HemaEspecial.Eosinofilos2, HemaEspecial.Basofilos2, HemaEspecial.leucocitos, HemaEspecial.Frotis FROM(datospersonales INNER JOIN(HemaEspecial RIGHT JOIN ordenes ON HemaEspecial.IdOrden = ordenes.IdOrden) ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN ResultadosPaciente ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente) WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1})); ", IDOrden, IdAnalisis), conn);
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
        public static DataSet VerificarHematologiaEspecial(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT HemaEspecial.IdOrden FROM HemaEspecial WHERE HemaEspecial.IDORDEN {0}; ", IDOrden), conn);
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
        public static string InsertarHematologia(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
        string Comentario1,
        string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
        string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
        string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
        string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologia", conn);
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
                conn.Open();
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
                conn.Close();
            }
        }
        public static string InsertarHematologiaSinValidar(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
        string Comentario1,
        string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
        string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
        string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
        string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologiaSSinValidar", conn);
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
                conn.Open();
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
                conn.Close();
            }
        }
        public static string InsertarHematologiaEspecial(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
       string Comentario1,
       string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
       string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
       string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
       string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologiaEspecial", conn);
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
                conn.Open();
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
                conn.Close();
            }
        }
        public static string InsertarHematologiaEspecialSinValidar(string IdOrden1, string HoraValidacion1, string IdAnalisis1,
       string Comentario1,
       string Neutrofilos1, string linfocitos1, string Monocitos1, string Eosinofilo1, string Basofilos1,
       string Hematies1, string Hemoglobina1, string Hematocritos1, string VCM1, string HCM1, string CHCM1,
       string Plaquetas1, string Neutrofilos22, string Linfocitos22, string Monocitos22, string Usuario1,
       string Eosinofilos22, string Basofilos22, string leucocitos1)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("InsertarHematologiaEspecialSinValidar", conn);
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
                conn.Open();
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
                conn.Close();
            }
        }
        public static string ActualizarHematologiasEspecial(string cmd, string cmd2, int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ResultadosPaciente SET  {0}  Where IdOrden = {1} And IDAnalisis  = {2}", cmd, IdOrden, IdAnalisis);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static DataSet Etiquetas()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT   ordenes.Fecha, ordenes.IdOrden,  ordenes.NumeroDia As N°, datospersonales.Nombre, datospersonales.Apellidos, convenios.Nombre AS NombreConvenio FROM datospersonales RIGHT JOIN(convenios RIGHT JOIN ordenes ON convenios.IdConvenio = ordenes.IDConvenio) ON datospersonales.IdPersona = ordenes.IdPersona WHERE(((ordenes.Muestra) = 0));", conn);
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
        public static DataSet ImprimirEtiquetas(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `ordenes`.`Fecha`, `ordenes`.`IdOrden`, `ordenes`.`NumeroDia` AS N°, `datospersonales`.`Nombre`, `datospersonales`.`Apellidos`, `convenios`.`Nombre` AS NombreConvenio, `analisislaboratorio`.`Etiquetas`, `analisislaboratorio`.`IdSeccion`, `analisislaboratorio`.`Especiales` FROM `analisislaboratorio` join `resultadospaciente` on `resultadospaciente`.`IdAnalisis` = `analisislaboratorio`.`IdAnalisis` join `ordenes` on `resultadospaciente`.`IdOrden` = `ordenes`.`IdOrden` join convenios on `ordenes`.`IdConvenio` = `convenios`.`IdConvenio` join `datospersonales` on `datospersonales`.`IdPersona` = `resultadospaciente`.`IdPaciente` Where `resultadospaciente`.`IdOrden`= {0} and `analisislaboratorio`.`Etiquetas` != '' order by IdSeccion,IdOrganizador asc;", cmd), conn);
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
        public static DataSet ContadorDeJeringas(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.totaljeringas Where fecha >= '{0}' and fecha <= '{1}';", cmd, cmd2), conn);
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
        public static string ActualizarEtiqueta(int IdOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);

            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE ordenes SET Muestra = 1  Where IdOrden = {0}", IdOrden);
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
                conn.Close();
            }
        }
        public static DataSet Sede()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Empresas.Sede, Empresas.Activa FROM Empresas WHERE(((Empresas.Activa) = 1));"), conn);
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
        public static DataSet ListaDeRespuesta(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                string query = string.Format("SELECT analisislaboratorio.IdAnalisis, ListaDeRespuetas.Respuesta FROM analisislaboratorio INNER JOIN ListaDeRespuetas ON analisislaboratorio.IdAnalisis = ListaDeRespuetas.IdAnalisis WHERE(((analisislaboratorio.IdAnalisis) = {0}));", cmd);
                adapter.SelectCommand = new MySqlCommand(query, conn);
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
        public static DataSet SELECTAnalisisFinal3(int IDOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, analisislaboratorio.NombreAnalisis, ResultadosPaciente.ValorResultado, ordenes.NumeroDia, analisislaboratorio.TipoAnalisis, MayoroMenorReferencial.ValorMenor, MayoroMenorReferencial.ValorMayor, MayoroMenorReferencial.Unidad, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, ResultadosPaciente.Comentario FROM(datospersonales INNER JOIN ordenes ON datospersonales.IdPersona = ordenes.IdPersona) INNER JOIN((analisislaboratorio LEFT JOIN MayoroMenorReferencial ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial) RIGHT JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ResultadosPaciente.IdPaciente) WHERE(((ResultadosPaciente.IdOrden) = {0})); ", IDOrden), conn);
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
        public static DataSet BioanalistasOrden(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, usuarios.NombreUsuario, usuarios.CB, usuarios.MPPS, usuarios.Cargo, usuarios.FIRMA, usuarios.SIGLAS FROM ResultadosPaciente INNER JOIN usuarios ON ResultadosPaciente.IdUsuario = usuarios.IdUsuario GROUP BY ResultadosPaciente.IdOrden, usuarios.NombreUsuario, usuarios.CB, usuarios.MPPS, usuarios.Cargo, usuarios.FIRMA, usuarios.IdUsuario, usuarios.SIGLAS HAVING(((ResultadosPaciente.IdOrden) = {0}) AND((usuarios.Cargo) = 3));", cmd), conn);
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
        public static DataSet SelectOrinas24(string Ordencmd, string Analisis1, string Analisis2, string Analisis3, string Analisis4)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, ResultadosPaciente.IdOrganizador, analisislaboratorio.NombreAnalisis, ResultadosPaciente.ValorResultado, MayoroMenorReferencial.ValorMenor, MayoroMenorReferencial.ValorMayor, MayoroMenorReferencial.Unidad FROM(analisislaboratorio LEFT JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis) LEFT JOIN MayoroMenorReferencial ON analisislaboratorio.IdAnalisis = MayoroMenorReferencial.IdReferencial WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((ResultadosPaciente.IdAnalisis) = {1} Or(ResultadosPaciente.IdAnalisis) = {2} Or(ResultadosPaciente.IdAnalisis) = {3} Or(ResultadosPaciente.IdAnalisis) = {4})) ORDER BY ResultadosPaciente.IdOrganizador;", Ordencmd, Analisis1, Analisis2, Analisis3, Analisis4), conn);
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
        public static string FechaSinAnio(DateTime FechaNacimiento)
        {
            string Fecha;
            DateTime fromDate = new DateTime();
            DateTime toDate = new DateTime();
            int[] monthDay = new int[12] { 31, -1, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int year;
            int month;
            int day;
            string Syear;
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


            if (year == 1)
            {
                Syear = string.Format("{0}", year);
            }
            else
            {
                Syear = string.Format("{0}", year);
            }
            if (year > 0)
            {
                Fecha = string.Format("{0}", Syear);
            }
            else
            {
                Fecha = "0";
            }



            return Fecha;
        }
        public static DataSet usuarios()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand("Select IdUsuario,NombreUsuario FROM usuarios", conn);
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
        public static DataSet conveniosUser()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT convenios.IdConvenio, convenios.Nombre,convenios.Descuento FROM convenios ", conn);
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
        public static DataSet ConvenioActivo(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT convenios.Activos FROM convenios WHERE convenios.IdConvenio ={0}", cmd), conn);
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
        public static DataSet PrivilegiosCargar(string UserID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.NombreUsuario, usuarios.Cargo, `privilegios`.`IdUsuario`,  `privilegios`.`ImprimirResultado`, `privilegios`.`ReimprimirResultado`, `privilegios`.`Validar`, `privilegios`.`Modificar`," +
            "`privilegios`.`AgregarConvenio`," +
            "`privilegios`.`QuitarConvenio`," +
            "`privilegios`.`VerLibroVenta`," +
            " `privilegios`.`VerCierreCaja`," +
            "`privilegios`.`Verordenes`," +
            " `privilegios`.`VerEstadisticas`," +
            " `privilegios`.`VerReporteBioanalista`," +
            " `privilegios`.`VerReferidos`," +
            " `privilegios`.`ImprimirFactura`," +
            " `privilegios`.`ReImprimirFactura`," +
            " `privilegios`.`AgregarUsuario`," +
            " `privilegios`.`ModificarUsuario`," +
            " `privilegios`.`CambioDePrecios`," +
            " `privilegios`.`ImprimirEtiqueta`," +
            " `privilegios`.`TeclasHematologia`," +
            " `privilegios`.`AgregarAnalisis`," +
            " `privilegios`.`ModificarAnalisis`," +
            "  `privilegios`.`EliminarAnalisis`," +
            " `privilegios`.`ModificarOrden`," +
            " `privilegios`.`AnularOrden` FROM usuarios INNER JOIN Privilegios ON usuarios.IdUsuario = Privilegios.IdUsuario WHERE(((usuarios.IdUsuario) = {0}));", UserID), conn);
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
        public static string InsertarConvenio(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            string MS;
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO convenios (Nombre,Descuento,Telefono,Correo,Activos) Values ({0})", cmd), conn);
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
                conn.Close();
            }
        }
        public static DataSet SelectConvenio(string UserID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT convenios.IdConvenio, convenios.Nombre, convenios.RIF, convenios.Empresa, convenios.Direccion, convenios.Telefono, convenios.Correo, convenios.Descuento, convenios.Activos FROM convenios Where IdConvenio = {0}", UserID), conn);
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
                conn.Close();
            }
        }
        public static string ActualizarConvenio(int IDConvenio, string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE convenios SET {1} WHERE(((convenios.IdConvenio) = {0})); ", IDConvenio, cmd);
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
        public static string ActualizarEstadoDeOrden(int IdOrden)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE Ordenes SET EstadoDeOrden = 2 WHERE (((Ordenes.IdOrden) = {0})); ", IdOrden);
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
        public static string Borrarconvenios(string IDConvenio, string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.DeleteCommand = conn.CreateCommand();
                string Query = string.Format("DELETE conveniosPorUsuario.IdConvenio, conveniosPorUsuario.IdUsuario FROM conveniosPorUsuario WHERE(((conveniosPorUsuario.IdConvenio) = {0}) AND((conveniosPorUsuario.IdUsuario) = {1})); ", IDConvenio, cmd);
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
                conn.Close();
            }
        }
        public static string InsertarConvenioUsuario(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            string MS;
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO conveniosPorUsuario ( IdConvenio, IdUsuario ) Values ({0})", cmd), conn);
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
                conn.Close();
            }
        }
        public static string ActualizarPrivilegios(string cmd, string IdUsuario)
        {
            MySqlConnection conn = new MySqlConnection(connection);

            try
            {


                DataSet ds = new DataSet();
                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM Privilegios WHERE IdUsuario = {IdUsuario};", conn);
                adapter.Fill(ds);
                if (ds.Tables.Count == 0)
                {
                    command = new MySqlCommand(string.Format("INSERT INTO privilegios (IdUsuario) Values ({0})", IdUsuario), conn);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
                else if (ds.Tables[0].Rows.Count == 0)
                {
                    command = new MySqlCommand(string.Format("INSERT INTO privilegios (IdUsuario) Values ({0})", IdUsuario), conn);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ActualizarUsuario(string cmd, string IdUsuario)
        {
            MySqlConnection conn = new MySqlConnection(connection);

            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE Usuarios SET  {0}  Where IdUsuario = {1} ", cmd, IdUsuario);
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
        public static string InsertarUsuario(string cmd, Bitmap image, string cargo, bool Guardar)
        {
            int Usuario;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO usuarios (NombreUsuario, Contraseña, Cargo, CB, MPPS, SIGLAS, Activo ) Values ({0})", cmd), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                Usuario = Convert.ToInt32(cmd2.ExecuteScalar());
                if (Guardar == true)
                {
                    image.Save(string.Format("Firma\\{0}.jpg", Usuario));
                    adapter.UpdateCommand = conn.CreateCommand();
                    string Query = string.Format("UPDATE usuarios SET usuarios.FIRMA = '{0}.jpg' WHERE(((usuarios.IdUsuario) = {0})); ", Usuario);
                    adapter.UpdateCommand.CommandText = Query;
                    adapter.UpdateCommand.ExecuteNonQuery();
                    command = new MySqlCommand(string.Format("INSERT INTO TeclasPorUsuario (IdUsuario,Leucocitos,Neutrofilos,Linfocitos, Monocitos, Eosinofilos, Basofilos, Plaquetas) VALUES ({0},'l','n','m','v','b','c','p')", Usuario), conn);
                    adapter.InsertCommand = command;
                    adapter.InsertCommand.ExecuteNonQuery();
                }
                command = new MySqlCommand(string.Format("INSERT INTO conveniosPorUsuario ( IdConvenio, IdUsuario ) Values (1,{0})", Usuario), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                command = new MySqlCommand(string.Format("INSERT INTO Privilegios ( ImprimirResultado, ReimprimirResultado, Validar, Modificar, AgregarConvenio, QuitarConvenio, VerLibroVenta, VerCierreCaja, Verordenes, VerEstadisticas, VerReporteBioanalista, VerReferidos, ImprimirFactura, ReImprimirFactura, AgregarUsuario, ModificarUsuario, CambioDePrecios, ImprimirEtiqueta,TeclasHematologia, IdUsuario ) SELECT PrivilegiosCargo.ImprimirResultado, PrivilegiosCargo.ReimprimirResultado, PrivilegiosCargo.Validar, PrivilegiosCargo.Modificar, PrivilegiosCargo.AgregarConvenio, PrivilegiosCargo.QuitarConvenio, PrivilegiosCargo.VerLibroVenta, PrivilegiosCargo.VerCierreCaja, PrivilegiosCargo.Verordenes, PrivilegiosCargo.VerEstadisticas, PrivilegiosCargo.VerReporteBioanalista, PrivilegiosCargo.VerReferidos, PrivilegiosCargo.ImprimirFactura, PrivilegiosCargo.ReImprimirFactura, PrivilegiosCargo.AgregarUsuario, PrivilegiosCargo.ModificarUsuario, PrivilegiosCargo.CambioDePrecios, PrivilegiosCargo.ImprimirEtiqueta,PrivilegiosCargo.TeclasHematologia, {0} FROM PrivilegiosCargo WHERE(((PrivilegiosCargo.IDCargo) = {1}))", Usuario, cargo), conn);
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
                conn.Close();
            }
        }

        public static string SelectUsuarioPassword(string Password)
        {
            string password = "";
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand($"SELECT * FROM usuarios WHERE(((usuarios.Contraseña) = '{Password}'));", conn);
                adapter.Fill(ds);
                adapter.Dispose();
                if (ds.Rows.Count > 0)
                {
                    password = ds.Rows[0]["Contraseña"].ToString();
                }

                return password;

            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return password;
            }
            finally
            {
                conn.Close();
            }
        }
        public static DataSet SelectCargo()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT Cargos.IDCargo, Cargos.NombreCargo, Cargos.Descripcion FROM Cargos WHERE(((Cargos.Visible) = '1'));", conn);
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
        public static DataSet CargarUsuario(string UserID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.IdUsuario, usuarios.FIRMA, usuarios.NombreUsuario, usuarios.Contraseña, usuarios.Cargo, usuarios.CB, usuarios.MPPS, usuarios.SIGLAS, usuarios.UsarFirma, usuarios.Activo FROM usuarios WHERE(((usuarios.IdUsuario) = {0})); ", UserID), conn);
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
        public static DataSet Contrasena(string UserID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT usuarios.Contraseña FROM usuarios WHERE(((usuarios.Contraseña) = '{0}')); ", UserID), conn);
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
        public static int SesionTemporal()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                int idOrden = -1;
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID();";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                //Ejemplo {0}=(2,'7/3/2020','2:17',1,1,NumeroDia) 
                string Time = DateTime.Now.ToString("yyyy/MM/dd");
                command = new MySqlCommand(string.Format("INSERT INTO Tempordenes (Fecha) Values ('{0}')", Time), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                var Orden = cmd2.ExecuteScalar();
                idOrden = Convert.ToInt32(Orden);
                return idOrden;
            }

            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                return 0;
            }
            finally
            {
                conn.Close();
            }
        }
        public static string PerfilTemporal(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                string MS = "";
                conn.Open();
                DataSet ds = new DataSet();
                command = new MySqlCommand(string.Format("INSERT INTO PerfilesAFacturar (IdSesion,IdPerfil,PrecioPerfil,ID) Values ({0})", cmd), conn);
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
                conn.Close();
            }
        }
        public static string BorrarTemporal(string cmd, string cmd2)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.DeleteCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ActualizarTemporal(string IdOrden, string Sesion)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ActualizarIDTemporal(string IdOrden, string Sesion)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static string ActualizarTeclas(string cmd, int IdUsuario)
        {
            DataTable UsuarioTeclas = new DataTable();
            string MS = "Guardado Exitosamente";
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                UsuarioTeclas = SeleccionarUsuarioTecla(IdUsuario);

                if (UsuarioTeclas.Rows.Count == 0)
                {
                    CrearTeclas(IdUsuario);
                }
                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE TeclasPorUsuario SET {0} WHERE (((TeclasPorUsuario.IdUsuario)={1})); ", cmd, IdUsuario);
                adapter.UpdateCommand.CommandText = Query;
                adapter.UpdateCommand.ExecuteNonQuery();
              
                return MS;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                MS = "Ha Ocurrido un Error: " + ex;
                return MS;
            }
            finally
            {
                conn.Close();
            }
        }
        public static DataTable SeleccionarUsuarioTecla(int UserID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataTable ds = new DataTable();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT IdUsuario FROM teclasporusuario WHERE IdUsuario = {0}; ", UserID), conn);
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
        public static string CrearTeclas(int IdUsuario)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.InsertCommand = conn.CreateCommand();
                string Query = string.Format($@"INSERT INTO `sistema2020`.`teclasporusuario`
                    (`IdUsuario`,
                    `Leucocitos`,
                    `Neutrofilos`,
                    `Linfocitos`,
                    `Monocitos`,
                    `Eosinofilos`,
                    `Basofilos`,
                    `Plaquetas`)
                    VALUES
                    ({IdUsuario},
                    'L',
                    'N',
                    'B',
                    'M',
                    'V',
                    'C',
                    'P');
                     ", IdUsuario);
                adapter.InsertCommand.CommandText = Query;
                adapter.InsertCommand.ExecuteNonQuery();
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
        public static DataSet AnalisisTemporal(string UserID)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT analisislaboratorio.NombreAnalisis FROM analisislaboratorio INNER JOIN(PerfilesAFacturar INNER JOIN PerfilesAnalisis ON PerfilesAFacturar.IdPerfil = PerfilesAnalisis.IdPerfil) ON analisislaboratorio.IdAnalisis = PerfilesAnalisis.IdAnalisis WHERE(((PerfilesAFacturar.IdSesion) = {0})) order by IdOrganizador;", UserID), conn);
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

        public static DataSet BuscarAnalisisTemporal(string Sesion, string Analisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.buscaranalisistemporal WHERE(((analisislaboratorio.NombreAnalisis) = '{1}') AND((PerfilesAFacturar.IdSesion) = {0}));", Sesion, Analisis), conn);
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
        public static string AgregarAnalisis(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            int Analisis;
            try
            {

                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                command = new MySqlCommand(string.Format("INSERT INTO usuarios (NombreUsuario, Contraseña, Cargo, CB, MPPS, SIGLAS, Activo ) Values ({0})", cmd), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                Analisis = Convert.ToInt32(cmd2.ExecuteScalar());
                command = new MySqlCommand(string.Format("INSERT INTO conveniosPorUsuario ( IdConvenio, IdUsuario ) Values (1,{0})", Analisis), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                Analisis = Convert.ToInt32(cmd2.ExecuteScalar());
                command = new MySqlCommand(string.Format("INSERT INTO Privilegios ( ImprimirResultado, ReimprimirResultado, Validar, Modificar, AgregarConvenio, QuitarConvenio, VerLibroVenta, VerCierreCaja, Verordenes, VerEstadisticas, VerReporteBioanalista, VerReferidos, ImprimirFactura, ReImprimirFactura, AgregarUsuario, ModificarUsuario, CambioDePrecios, ImprimirEtiqueta,TeclasHematologia, IdUsuario ) SELECT PrivilegiosCargo.ImprimirResultado, PrivilegiosCargo.ReimprimirResultado, PrivilegiosCargo.Validar, PrivilegiosCargo.Modificar, PrivilegiosCargo.AgregarConvenio, PrivilegiosCargo.QuitarConvenio, PrivilegiosCargo.VerLibroVenta, PrivilegiosCargo.VerCierreCaja, PrivilegiosCargo.Verordenes, PrivilegiosCargo.VerEstadisticas, PrivilegiosCargo.VerReporteBioanalista, PrivilegiosCargo.VerReferidos, PrivilegiosCargo.ImprimirFactura, PrivilegiosCargo.ReImprimirFactura, PrivilegiosCargo.AgregarUsuario, PrivilegiosCargo.ModificarUsuario, PrivilegiosCargo.CambioDePrecios, PrivilegiosCargo.ImprimirEtiqueta,PrivilegiosCargo.TeclasHematologia, {0} FROM PrivilegiosCargo WHERE(((PrivilegiosCargo.IDCargo) = {0}))"), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                command = new MySqlCommand(string.Format("INSERT INTO TeclasPorUsuario (IdUsuario,Leucocitos,Neutrofilos,Linfocitos, Monocitos, Eosinofilos, Basofilos, Plaquetas) VALUES ({0},'l','n','m','v','b','c','p')"), conn);
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
                conn.Close();
            }
        }
        public static int InsertarReferido(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                command = new MySqlCommand(string.Format("Insert INTO Referidos (IDOrden,Sede) Values ({0})", cmd), conn);
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
                conn.Close();
            }
        }

        public static DataSet EnviarReferidosAutomatico()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT convenios.Sede, ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, ResultadosPaciente.IdConvenio, ResultadosPaciente.ValorResultado, ordenes.Usuario FROM datospersonales INNER JOIN((ordenes INNER JOIN convenios ON ordenes.IDConvenio = convenios.IdConvenio) INNER JOIN ResultadosPaciente ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(convenios.IdConvenio = ResultadosPaciente.IdConvenio)) ON(datospersonales.IdPersona = ResultadosPaciente.IdPaciente) AND(datospersonales.IdPersona = ordenes.IdPersona) WHERE(((convenios.Sede) = '1')); "), conn);
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
        public static DataSet SelectTemp(int IdOrden, int IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Resultadospaciente.IdOrden,ordenes.NumeroDia,datospersonales.Nombre,Apellidos,Sexo,convenios.Nombre,ordenes.Fecha,ordenes.HoraIngreso,NombreUsuario,HoraValidacion,PrecioF FROM sistema2020.ordenes inner join resultadospaciente on ordenes.IdOrden = resultadospaciente.IdOrden inner join datospersonales on resultadospaciente.IdPaciente = datospersonales.IdPersona inner join convenios on convenios.IDConvenio = resultadospaciente.IdConvenio inner join usuarios on ordenes.Usuario = usuarios.IdUsuario where resultadospaciente.IdOrden = {0} and IdAnalisis = {1};", IdOrden, IdAnalisis), conn);
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
        public static DataSet InformacionReferidoEnviada(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.IdAnalisis, datospersonales.Cedula, datospersonales.Nombre, datospersonales.Apellidos, datospersonales.Sexo, datospersonales.Fecha, ResultadosPaciente.IdConvenio, ResultadosPaciente.ValorResultado, ordenes.Usuario FROM(datospersonales INNER JOIN ResultadosPaciente ON datospersonales.IdPersona = ResultadosPaciente.IdPaciente) INNER JOIN ordenes ON(ordenes.IdOrden = ResultadosPaciente.IdOrden) AND(datospersonales.IdPersona = ordenes.IdPersona) WHERE(((ResultadosPaciente.IdOrden) = {0}));", cmd), conn);
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
        public static DataSet CorreoEmpresa()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {
                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM Empresas WHERE(((Empresas.Activa) = 1))"), conn);
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
        public static DataSet CorreoConvenio(string Correo)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT convenios.CorreoSede, convenios.Sede FROM convenios WHERE(((convenios.IdConvenio) = {0}) AND((convenios.Sede) = 1));", Correo), conn);
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
        public static DataSet SELECTMAC(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Equipos.MAC FROM Equipos WHERE(((Equipos.MAC) = '{0}'));", cmd), conn);
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
        public static DataSet SelectordenesEnviar()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado FROM analisislaboratorio LEFT JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis WHERE(((analisislaboratorio.Especiales) = '1')) and IdOrden is not Null GROUP BY ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado HAVING (ResultadosPaciente.Recibido is Null or ResultadosPaciente.Recibido = '0') AND (ResultadosPaciente.Enviado is Null or ResultadosPaciente.Enviado = '0')", conn);
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
        public static DataSet ordenesSeleccionadas(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `resultadospaciente`.`IdOrden`, `ordenes`.`Usuario` as IdUsuario, `ordenes`.`Fecha` AS FechaOrden,`resultadospaciente`.`IdAnalisis`, `resultadospaciente`.`IdOrganizador`,  `datospersonales`.`Cedula`, `datospersonales`.`Nombre`, `datospersonales`.`Apellidos`, `datospersonales`.`Celular`, `datospersonales`.`Telefono`,  `datospersonales`.`Sexo`,`datospersonales`.`Fecha`, `datospersonales`.`Correo`, `datospersonales`.`TipoCorreo`, `datospersonales`.`CodigoCelular`, `datospersonales`.`CodigoTelefono`, `analisislaboratorio`.`Especiales`, `ResultadosPaciente`.`Enviado`, `ResultadosPaciente`.`Recibido` FROM(`datospersonales` JOIN `ordenes` ON `datospersonales`.`IdPersona` = `ordenes`.`IdPersona`) JOIN(`analisislaboratorio` JOIN `ResultadosPaciente` ON `analisislaboratorio`.`IdAnalisis` = `ResultadosPaciente`.`IdAnalisis`) ON(`ordenes`.`IdOrden` = `ResultadosPaciente`.`IdOrden`) AND(`datospersonales`.`IdPersona` = `ResultadosPaciente`.`IdPaciente`) Where `ResultadosPaciente`.`IdOrden` = {0} AND `analisislaboratorio`.`Especiales` = '1' AND (`ResultadosPaciente`.`Enviado` is Null OR `ResultadosPaciente`.`Enviado` = 0) AND(`ResultadosPaciente`.`Recibido` is Null OR `ResultadosPaciente`.`Recibido` = 0)GROUP BY `ResultadosPaciente`.`IdOrden`, `ordenes`.`Usuario`, `ordenes`.`Fecha`,  `ResultadosPaciente`.`IdAnalisis`,`ResultadosPaciente`.`IdOrganizador`, `datospersonales`.`Cedula`, `datospersonales`.`Nombre`, `datospersonales`.`Apellidos`, `datospersonales`.`Celular`, `datospersonales`.`Telefono`, `datospersonales`.`Sexo`,`datospersonales`.`Fecha`, `datospersonales`.`Correo`, `datospersonales`.`TipoCorreo`, `datospersonales`.`CodigoCelular`, `datospersonales`.`CodigoTelefono`, `analisislaboratorio`.`Especiales`, `ResultadosPaciente`.`Enviado`, `ResultadosPaciente`.`Recibido` ", cmd), conn);
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
        public static string ActualizarEnviado(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format("UPDATE analisislaboratorio INNER JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis SET ResultadosPaciente.Enviado = 1 WHERE(((ResultadosPaciente.IdOrden) = {0}) AND((analisislaboratorio.Especiales) = '1')); ", cmd);
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
        public static int ConvenioPorCorreo(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            int IDConvenio = 0;
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT convenios.IdConvenio FROM convenios WHERE(((convenios.CorreoSede) = '{0}')); ", cmd), conn);
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
                conn.Close();
            }
        }
        public static int BuscarNumeroOrden(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            int IDConvenio = 0;
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(IdOrden) as Contador FROM ordenes WHERE(((ordenes.IdOrden) =  {0})); ", cmd), conn);
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
                conn.Close();
            }
        }
        public static int BurcarOrdenCruzada(string IDOrden, string Serial)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            int IDConvenio = 0;
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Count(IdOrden) as Contador FROM sistema2020.pagos Where IdOrden ={0} and TipoDePago = 'ordenes' and Serial = '{1}';", IDOrden, Serial), conn);
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
                conn.Close();
            }
        }
        public static string BuscarFechaOrden(string IDOrden, string Serial)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            string IDConvenio;
            DataSet ds = new DataSet();
            try
            {


                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT Fecha FROM sistema2020.pagos Where IdOrden = {0}  and Serial = '{1}';  ", IDOrden, Serial), conn);
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
                conn.Close();
            }
        }
        public static string ActualizarOrdenReferido(string IdOrden, string IdAnalisis)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
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
                conn.Close();
            }
        }
        public static DataSet SeleccionordenesAEnviar()
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado FROM analisislaboratorio LEFT JOIN ResultadosPaciente ON analisislaboratorio.IdAnalisis = ResultadosPaciente.IdAnalisis WHERE(((analisislaboratorio.Especiales) = '1')) and IdOrden is not Null GROUP BY ResultadosPaciente.IdOrden, ResultadosPaciente.Recibido, ResultadosPaciente.Enviado HAVING(((ResultadosPaciente.Recibido) is null  Or ResultadosPaciente.Recibido = 0) AND((ResultadosPaciente.Enviado) is null  Or ResultadosPaciente.Enviado = 0))"), conn);
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
        public static DataSet SeleccionResultadosAEnviar(string cmd)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT `referidos`.`IdOrden`,`referidos`.`IdOrdenSede`, `resultadospaciente`.`IdAnalisis`, `resultadospaciente`.`ValorResultado`, `resultadospaciente`.`Comentario`,`resultadospaciente`.`HoraValidacion`, `resultadospaciente`.`IdUsuario` FROM `sistema2020`.`referidos` join `resultadosPaciente` on `referidos`.`IdOrdenSede` = `resultadosPaciente`.`IdOrden` Where `resultadospaciente`.`IdConvenio` = {0} And `resultadospaciente`.`EstadoDeResultado` >= 2 and `resultadospaciente`.`Enviado` Is null Group by IdOrden,IdAnalisis", cmd), conn);
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
        public List<Bancos> ObtenerBancos()
        {

            List<Bancos> bancos = new List<Bancos>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.bancos"), conn);
                adapter.Fill(ds);
                adapter.Dispose();

   
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                conn.Close();
            }


            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    bancos = new Bancos().MapearLista(ds.Tables[0]);
                }

            }

            return bancos;
        }
        public List<Bancos> ObtenerBancosPorTipoDepago(int IdTipoPago)
        {

            List<Bancos> bancos = new List<Bancos>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format($"SELECT bancos.IdBancos,bancos.NombreBanco FROM sistema2020.bancotipopago inner join bancos on Bancos.IdBancos = bancotipopago.IdBanco where bancotipopago.IdTipoPago = {IdTipoPago}"), conn);
                adapter.Fill(ds);
                adapter.Dispose();


            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                conn.Close();
            }


            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    bancos = new Bancos().MapearLista(ds.Tables[0]);
                }

            }

            return bancos;
        }
        public List<TipoPago> ObtenerTiposDePago()
        {

            List<TipoPago> tipoPagos = new List<TipoPago>();
            MySqlConnection conn = new MySqlConnection(connection);
            DataSet ds = new DataSet();
            try
            {

                conn.Open();
                adapter.SelectCommand = new MySqlCommand(string.Format("SELECT * FROM sistema2020.tipodepago"), conn);
                adapter.Fill(ds);
                adapter.Dispose();


            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
            }
            finally
            {
                conn.Close();
            }


            if (ds.Tables.Count > 0)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    tipoPagos = new TipoPago().MapearLista(ds.Tables[0]);
                }

            }

            return tipoPagos;
        }
        public static Bancos InsertarBancos(Bancos banco)
        {
            int IdBanco;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                command = new MySqlCommand(string.Format($"INSERT INTO `sistema2020`.`bancos` ( `NombreBanco`) VALUES  ('{banco.NombreBanco}')"), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                IdBanco = Convert.ToInt32(cmd2.ExecuteScalar());
                banco.IdBancos = IdBanco;
                return banco;
            }
            catch (Exception ex)
            {
                CrearEvento(ex.ToString());
                int MS = 0;
                return banco;
            }
            finally
            {
                conn.Close();
            }
        }
        public static string ActualizarBanco(Bancos banco)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                DataSet ds = new DataSet();
                conn.Open();
                adapter.UpdateCommand = conn.CreateCommand();
                string Query = string.Format($"UPDATE `sistema2020`.`bancos` SET `NombreBanco` = '{banco.NombreBanco}' WHERE `IdBancos` = {banco.IdBancos}");
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
        public static string BorrarBanco(Bancos banco)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                command = new MySqlCommand(string.Format("DELETE FROM `sistema2020`.`Bancos` WHERE IdBancos = {0}; ", banco.IdBancos), conn);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
                string MS = "Borrado Exitosamente";
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
        public static string InsertarBancosPorTipoDePago(int IdTipoPago,int Idbanco)
        {
            int IdBanco;
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {
                DataSet ds = new DataSet();
                string Query2 = "SELECT LAST_INSERT_ID()";
                MySqlCommand cmd2 = new MySqlCommand(Query2, conn);
                conn.Open();
                command = new MySqlCommand(string.Format($"INSERT INTO `sistema2020`.`bancotipopago` ( `IdTipopago`,`idBanco`) VALUES  ({IdTipoPago},{Idbanco})"), conn);
                adapter.InsertCommand = command;
                adapter.InsertCommand.ExecuteNonQuery();
                IdBanco = Convert.ToInt32(cmd2.ExecuteScalar());
                string MS = "Agregado Exitosamente";
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
        public static string BorrarBancoEnTipoDepago(int IdtipoPago,int Idbanco)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                conn.Open();
                command = new MySqlCommand(string.Format("DELETE FROM `sistema2020`.`bancotipopago` WHERE IdTipopago = {0} and idBanco = {1}; ", IdtipoPago,Idbanco), conn);
                adapter.DeleteCommand = command;
                adapter.DeleteCommand.ExecuteNonQuery();
                string MS = "Borrado Exitosamente";
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
        public static string ActualizarResultadoEnviado(string IdOrden1, string IdAnalisis1, string IdOrdenSede)
        {
            MySqlConnection conn = new MySqlConnection(connection);
            try
            {

                MySqlCommand cmd2 = new MySqlCommand("ActualizarResultadoEnviado", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrden1;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis1;
                conn.Open();
                cmd2.ExecuteNonQuery();
                string MS = "Exitoso";
                conn.Close();
                Connection(ConfigurationManager.ConnectionStrings["server"].ConnectionString, "server");
                conn.Open();
                cmd2 = new MySqlCommand("ActualizarResultadoEnviado", conn);
                cmd2.CommandType = CommandType.StoredProcedure;
                cmd2.Parameters.Add("IdOrden1", MySqlDbType.Int32).Value = IdOrdenSede;
                cmd2.Parameters.Add("IdAnalisis1", MySqlDbType.Int32).Value = IdAnalisis1;
                cmd2.ExecuteNonQuery();
                MS = "Exitoso";
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


       
    }
}


