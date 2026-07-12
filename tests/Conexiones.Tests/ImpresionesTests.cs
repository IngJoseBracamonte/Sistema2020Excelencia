using System;
using System.Data;
using System.IO;
using Xunit;
using Conexiones;
using Conexiones.DbConnect;
using Conexiones.Impresion;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Conexiones.Tests
{
    public class ImpresionesTests : IDisposable
    {
        private readonly string _logosDir;
        private readonly string _firmaDir;
        private readonly string _logoPath;
        private readonly string _firmaPath;

        public ImpresionesTests()
        {
            // Setup dummy images to avoid FileNotFoundException during PDF generation
            _logosDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logos");
            _firmaDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Firma");
            _logoPath = Path.Combine(_logosDir, "logo.png");
            _firmaPath = Path.Combine(_firmaDir, "firma.png");

            Directory.CreateDirectory(_logosDir);
            Directory.CreateDirectory(_firmaDir);

            // 1x1 transparent PNG bytes
            byte[] pngBytes = {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x48, 0x44, 0x52,
                0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00, 0x01, 0x08, 0x06, 0x00, 0x00, 0x00, 0x1F, 0x15, 0xC4,
                0x89, 0x00, 0x00, 0x00, 0x0D, 0x49, 0x44, 0x41, 0x54, 0x78, 0xDA, 0x63, 0x64, 0xF8, 0xFF, 0x3F,
                0x00, 0x05, 0x00, 0x02, 0xFE, 0x02, 0x1E, 0x50, 0x44, 0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E,
                0x44, 0xAE, 0x42, 0x60, 0x82
            };

            if (!File.Exists(_logoPath))
            {
                File.WriteAllBytes(_logoPath, pngBytes);
            }
            if (!File.Exists(_firmaPath))
            {
                File.WriteAllBytes(_firmaPath, pngBytes);
            }

            // Set current directory to base directory so relative paths in Impresiones.cs resolve correctly
            Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
        }

        public void Dispose()
        {
            // Clear overrides after each test
            Conexion.SelectEmpresaActivaOverride = null;
            Conexion.SelectEmpresaConvenioOverride = null;
            Conexion.PacienteAImprimirOverride = null;
            Conexion.SELECTIMPRIMIRTOTALOverride = null;
            Conexion.HematologiaOverride = null;
            Conexion.HematologiaEspecialOverride = null;
            Conexion.BioanalistaOverride = null;
            Conexion.ActualizarOrdenOverride = null;

            // Clean up dummy directories
            try
            {
                if (File.Exists(_logoPath)) File.Delete(_logoPath);
                if (File.Exists(_firmaPath)) File.Delete(_firmaPath);
                if (Directory.Exists(_logosDir)) Directory.Delete(_logosDir);
                if (Directory.Exists(_firmaDir)) Directory.Delete(_firmaDir);
            }
            catch { /* ignore */ }
        }

        [Fact]
        public void Documento_HematologiaCompleta_Id55_GeneratesPdfSuccessfully()
        {
            // Arrange
            int idOrden = 12345;
            int idAnalisis = 55; // Hematología Completa

            // Mock Paciente data (Caratula)
            Conexion.PacienteAImprimirOverride = (orden) =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdPersona");
                dt.Columns.Add("Cedula");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Apellidos");
                dt.Columns.Add("Celular");
                dt.Columns.Add("Telefono");
                dt.Columns.Add("Sexo");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Correo");
                dt.Columns.Add("TipoCorreo");
                dt.Columns.Add("CodigoCelular");
                dt.Columns.Add("CodigoTelefono");
                dt.Columns.Add("IdConvenio");

                dt.Rows.Add("1", "V-12345678", "Juan", "Perez", "1112222", "3334444", "M", "1990-05-15", "juanperez", "@gmail.com", "414", "212", "1");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock Empresa data
            Conexion.SelectEmpresaActivaOverride = () =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdEmpresa");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Rif");
                dt.Columns.Add("Ruta");
                dt.Columns.Add("Activa");
                dt.Columns.Add("Sede");
                dt.Columns.Add("Correo");
                dt.Columns.Add("Clave");
                dt.Columns.Add("Puerto");
                dt.Columns.Add("CorreoSistema");
                dt.Columns.Add("ClaveSistema");
                dt.Columns.Add("Direccion");
                dt.Columns.Add("Telefono");
                dt.Columns.Add("Telefono2");

                dt.Rows.Add("1", "Laboratorio Clinico Excelencia", "J-99999999-9", "logo.png", "1", "Sede Principal", "info@excelencia.com", "pass", "587", "sys@excelencia.com", "pass", "Av. Principal", "0212-1111111", "0414-2222222");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock dsPrint (SelectImprimirTotal)
            Conexion.SELECTIMPRIMIRTOTALOverride = (ordenStr) =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdAnalisis");
                dt.Columns.Add("EstadoDeResultado");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("FechaImp");
                dt.Columns.Add("NombreAnalisis");
                dt.Columns.Add("Titulo");
                dt.Columns.Add("FinalTitulo");
                dt.Columns.Add("MultiplesValores");
                dt.Columns.Add("ValorMenor");
                dt.Columns.Add("ValorMayor");
                dt.Columns.Add("UNIDAD");
                dt.Columns.Add("Comentario");
                dt.Columns.Add("TipoAnalisis");
                dt.Columns.Add("Lineas");
                dt.Columns.Add("IdOrden");
                dt.Columns.Add("ValorResultado");

                dt.Rows.Add("55", "2", "1990-05-15", "2026-07-09 10:00:00", "Hematología Completa", "0", "0", "", "", "", "", "Observacion hematologia", "1", "1", idOrden.ToString(), "");
                dt.Rows.Add("204", "2", "1990-05-15", "2026-07-09 10:00:00", "ADE", "0", "0", "", "", "", "%", "", "1", "1", idOrden.ToString(), "12.4");
                dt.Rows.Add("205", "2", "1990-05-15", "2026-07-09 10:00:00", "Volumen Plaquentario Medio", "0", "0", "", "", "", "fL", "", "1", "1", idOrden.ToString(), "7.7");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock Hematologia results
            Conexion.HematologiaOverride = (orden, analisis) =>
            {
                return new Hematologia
                {
                    leucocitos = "7.5",
                    Neutrofilos = "60.0",
                    linfocitos = "30.0",
                    Monocitos = "8.0",
                    Eosinofilos = "2.0",
                    Basofilos = "0.0",
                    Hematies = "4.8",
                    Hemoglobina = "14.2",
                    Hematocritos = "42.5",
                    VCM = "88.0",
                    HCM = "29.5",
                    CHCM = "33.5",
                    Plaquetas = "250",
                    Neutrofilos2 = "4.5",
                    Linfocitos2 = "2.25",
                    Monocitos2 = "0.6",
                    Eosinofilos2 = "0.15",
                    Basofilos2 = "0.0",
                    Comentario = "Valores de hematologia normales."
                };
            };

            // Mock Bioanalista data (Footer)
            Conexion.BioanalistaOverride = (orden, analisis) =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdUsuario");
                dt.Columns.Add("NombreUsuario");
                dt.Columns.Add("CB");
                dt.Columns.Add("MPPS");
                dt.Columns.Add("FIRMA");

                dt.Rows.Add("1", "Dra. Ana Rodriguez", "7890", "1234", "firma.png");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock ActualizarOrden (evita escrituras a BD real)
            Conexion.ActualizarOrdenOverride = (cmd, orden, analisis) => "Guardado Exitosamente";

            // Act
            PdfSharp.Pdf.PdfDocument doc = Impresiones.DocumentoHematologiaCompleta(idOrden, idAnalisis, "Guardar");

            // Save PDF to C:\TemporalesPDF
            string outputDir = @"C:\TemporalesPDF";
            Directory.CreateDirectory(outputDir);
            string outputPath = Path.Combine(outputDir, $"HematologiaCompleta_{idAnalisis}_{idOrden}.pdf");
            doc.Save(outputPath);

            // Assert
            Assert.NotNull(doc);
            // Debe tener al menos 2 páginas (pág 1: carátula, pág 2: resultados de hematología)
            Assert.True(doc.PageCount >= 2);
        }

        [Fact]
        public void Documento_HematologiaEspecial_Id203_GeneratesPdfSuccessfully()
        {
            // Arrange
            int idOrden = 67890;
            int idAnalisis = 203; // Hematología Especial / Descripción de Frotis

            // Mock Paciente data (Caratula)
            Conexion.PacienteAImprimirOverride = (orden) =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdPersona");
                dt.Columns.Add("Cedula");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Apellidos");
                dt.Columns.Add("Celular");
                dt.Columns.Add("Telefono");
                dt.Columns.Add("Sexo");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Correo");
                dt.Columns.Add("TipoCorreo");
                dt.Columns.Add("CodigoCelular");
                dt.Columns.Add("CodigoTelefono");
                dt.Columns.Add("IdConvenio");

                dt.Rows.Add("2", "V-87654321", "Maria", "Gomez", "9998888", "7776666", "F", "1985-11-20", "mariagomez", "@gmail.com", "412", "212", "1");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock Empresa data
            Conexion.SelectEmpresaActivaOverride = () =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdEmpresa");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Rif");
                dt.Columns.Add("Ruta");
                dt.Columns.Add("Activa");
                dt.Columns.Add("Sede");
                dt.Columns.Add("Correo");
                dt.Columns.Add("Clave");
                dt.Columns.Add("Puerto");
                dt.Columns.Add("CorreoSistema");
                dt.Columns.Add("ClaveSistema");
                dt.Columns.Add("Direccion");
                dt.Columns.Add("Telefono");
                dt.Columns.Add("Telefono2");

                dt.Rows.Add("1", "Laboratorio Clinico Excelencia", "J-99999999-9", "logo.png", "1", "Sede Principal", "info@excelencia.com", "pass", "587", "sys@excelencia.com", "pass", "Av. Principal", "0212-1111111", "0414-2222222");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock dsPrint (SelectImprimirTotal)
            Conexion.SELECTIMPRIMIRTOTALOverride = (ordenStr) =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdAnalisis");
                dt.Columns.Add("EstadoDeResultado");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("FechaImp");
                dt.Columns.Add("NombreAnalisis");
                dt.Columns.Add("Titulo");
                dt.Columns.Add("FinalTitulo");
                dt.Columns.Add("MultiplesValores");
                dt.Columns.Add("ValorMenor");
                dt.Columns.Add("ValorMayor");
                dt.Columns.Add("UNIDAD");
                dt.Columns.Add("Comentario");
                dt.Columns.Add("TipoAnalisis");
                dt.Columns.Add("Lineas");
                dt.Columns.Add("IdOrden");
                dt.Columns.Add("ValorResultado");

                dt.Rows.Add("203", "2", "1985-11-20", "2026-07-09 11:00:00", "Hematología Especial", "0", "0", "", "", "", "", "Ver descripcion en frotis", "1", "1", idOrden.ToString(), "");
                dt.Rows.Add("204", "2", "1985-11-20", "2026-07-09 11:00:00", "ADE", "0", "0", "", "", "", "%", "", "1", "1", idOrden.ToString(), "12.4");
                dt.Rows.Add("205", "2", "1985-11-20", "2026-07-09 11:00:00", "Volumen Plaquentario Medio", "0", "0", "", "", "", "fL", "", "1", "1", idOrden.ToString(), "7.7");
                dt.Rows.Add("206", "2", "1985-11-20", "2026-07-09 11:00:00", "ADP", "0", "0", "", "", "", "", "", "1", "1", idOrden.ToString(), "16.5");
                dt.Rows.Add("207", "2", "1985-11-20", "2026-07-09 11:00:00", "PCT", "0", "0", "", "", "", "%", "", "1", "1", idOrden.ToString(), "0.16");
                dt.Rows.Add("208", "2", "1985-11-20", "2026-07-09 11:00:00", "Reticulocitos", "0", "0", "", "", "", "%", "", "1", "1", idOrden.ToString(), "1.50");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock HematologiaEspecial results (DataSet con frotis)
            Conexion.HematologiaEspecialOverride = (orden, analisis) =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdOrden");
                dt.Columns.Add("FechaImp");
                dt.Columns.Add("NumeroDia");
                dt.Columns.Add("IdUsuario");
                dt.Columns.Add("IdAnalisis");
                dt.Columns.Add("Nombre");
                dt.Columns.Add("Apellidos");
                dt.Columns.Add("Sexo");
                dt.Columns.Add("Fecha");
                dt.Columns.Add("Comentario");
                dt.Columns.Add("Neutrofilos");
                dt.Columns.Add("linfocitos");
                dt.Columns.Add("Monocitos");
                dt.Columns.Add("Eosinofilos");
                dt.Columns.Add("Basofilos");
                dt.Columns.Add("Hematies");
                dt.Columns.Add("Hemoglobina");
                dt.Columns.Add("Hematocritos");
                dt.Columns.Add("VCM");
                dt.Columns.Add("HCM");
                dt.Columns.Add("CHCM");
                dt.Columns.Add("Plaquetas");
                dt.Columns.Add("Neutrofilos2");
                dt.Columns.Add("Linfocitos2");
                dt.Columns.Add("Monocitos2");
                dt.Columns.Add("Eosinofilos2");
                dt.Columns.Add("Basofilos2");
                dt.Columns.Add("leucocitos");
                dt.Columns.Add("Frotis");
                dt.Columns.Add("ADE");
                dt.Columns.Add("VPM");
                dt.Columns.Add("ADP");
                dt.Columns.Add("PCT");
                dt.Columns.Add("Reticulocitos");

                dt.Rows.Add(
                    idOrden.ToString(), "2026-07-09", "2", "1", "203", "Maria", "Gomez", "F", "1985-11-20",
                    "Observaciones de hematologia especial.", "65.0", "25.0", "7.0", "3.0", "0.0", "4.2", "12.8",
                    "38.5", "91.0", "30.5", "33.2", "220", "4.8", "1.8", "0.5", "0.2", "0.0", "7.3",
                    "Serie Roja: Anisocitosis discreta con presencia de microcitos. Serie Blanca: Sin alteraciones. Serie Plaquetaria: Adecuada en numero.",
                    "13.0", "7.5", "16.5", "0.16", "1.50"
                );

                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock Bioanalista data (Footer)
            Conexion.BioanalistaOverride = (orden, analisis) =>
            {
                var dt = new DataTable();
                dt.Columns.Add("IdUsuario");
                dt.Columns.Add("NombreUsuario");
                dt.Columns.Add("CB");
                dt.Columns.Add("MPPS");
                dt.Columns.Add("FIRMA");

                dt.Rows.Add("1", "Dra. Ana Rodriguez", "7890", "1234", "firma.png");
                var ds = new DataSet();
                ds.Tables.Add(dt);
                return ds;
            };

            // Mock ActualizarOrden
            Conexion.ActualizarOrdenOverride = (cmd, orden, analisis) => "Guardado Exitosamente";

            // Act
            PdfSharp.Pdf.PdfDocument doc = Impresiones.DocumentoHematologiaEspecial(idOrden, idAnalisis, "Guardar");

            // Save PDF to C:\TemporalesPDF
            string outputDir = @"C:\TemporalesPDF";
            Directory.CreateDirectory(outputDir);
            string outputPath = Path.Combine(outputDir, $"HematologiaEspecial_{idAnalisis}_{idOrden}.pdf");
            doc.Save(outputPath);

            // Assert
            Assert.NotNull(doc);
            // Hematología Especial agrega la carátula, luego los resultados, y luego la página del frotis.
            // Debe tener al menos 3 páginas.
            Assert.True(doc.PageCount >= 3);
        }
    }
}
