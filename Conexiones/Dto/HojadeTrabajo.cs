using DocumentFormat.OpenXml.Wordprocessing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Conexiones.DbConnect;

namespace Conexiones.Dto
{
    public class HojadeTrabajo
    {

        public PdfSharp.Pdf.PdfDocument HematologiaGrupal(PdfSharp.Pdf.PdfDocument document)
        {
            double PosicionP = 95;
            const string facename = "Arial Rounded MT";
            int IdEspecie = 0;
            bool HeaderImpreso = false;
            List<Ordenes> ordenes = new List<Ordenes>();
            List<Hemo> hemoparasitos = new List<Hemo>();
            List<int> Especies = new List<int>();
            PdfSharp.Pdf.PdfPage page = document.AddPage();
            //Especies = ConexionVeterinaria.EspeciesPorSesion(factura.IdSesion);
            XRect Margen = new XRect(5, PosicionP, 250, 14);
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);

            XFont fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            XFont Titulo = new XFont(facename, 20, XFontStyle.Regular);
            XSolidBrush blueBrush = new XSolidBrush(XColors.Black);
            XColor color = new XColor { R = 105, G = 105, B = 105 };
            XPen pen = new XPen(color);
            XPoint point = new XPoint(5, 70);
            XSize size = new XSize(580, 15);
            XSize Elipsesize = new XSize(5, 5);
            XRect rect = new XRect(point, size);
            int i = 0;
            int PosicionY = 75;
            string Observaciones = string.Empty;
            hemoparasitos = ConexionVeterinaria.selectHemoparasitosPorÉspecie(1);
            fontRegular2 = new XFont(facename, 8, XFontStyle.Regular);
            blueBrush = new XSolidBrush(XColors.Black);
            color = new XColor { R = 105, G = 105, B = 105 };
            pen = new XPen(color);
            point = new XPoint(20, 70);
            size = new XSize(580, 15);
            Elipsesize = new XSize(5, 5);
            rect = new XRect(point, size);
            PosicionP = 70;
            Margen = new XRect(230, PosicionP, 120, 14);
            gfx.DrawString($"Hoja de Trabajo {DateTime.Now.ToString("dd/MM/yyyy")} ", Titulo, blueBrush, Margen, XStringFormats.Center);


            int PosicionX = 120;
            PosicionP = 95;
            Margen = new XRect(230, PosicionP, 120, 14);
            gfx.DrawString($"HEMATOLOGIA COMPLETA ", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("IDENTIFICACION ANIMAL", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("HEMOGLOBINA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("HEMATOCRITO", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("CUENTA LEUCOCITARIA", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("NEUTROFILOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("LINFOCITOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("MONOCITOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("EOSINOFILOS", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            Margen = new XRect(5, PosicionP, 120, 14);
            gfx.DrawString("DET. PLAQUETAS", fontRegular2, blueBrush, Margen, XStringFormats.Center);

            for (int z = 0; z <= hemoparasitos.Count - 1; z++)
            {
                PosicionP += 15;
                Margen = new XRect(5, PosicionP, 120, 14);
                Hemo hemo = hemoparasitos.ElementAt(z);
                gfx.DrawString(hemo.Descripcion, fontRegular2, blueBrush, Margen, XStringFormats.Center);

            }
            point = new XPoint(10, 110);
            size = new XSize(565, PosicionP - 95);

            rect = new XRect(point, size);
            gfx.DrawRectangle(pen, rect);


            //gfx.DrawLine(pen, PosicionX, 110, PosicionX, 320);

            PosicionP = 110;
            for (int x = 1; x < 9 + hemoparasitos.Count; x++)
            {
                PosicionP += 15;
                gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
            }

            for (int x = 1; x < 8; x++)
            {
                gfx.DrawLine(pen, PosicionX, 110, PosicionX, PosicionP + 15);
                PosicionX += 65;
            }

            PosicionP += 15;
            Margen = new XRect(230, PosicionP, 120, 14);

            gfx.DrawString($"EJEMPLO SIGUIENTE EXAMEN ", fontRegular2, blueBrush, Margen, XStringFormats.Center);
            PosicionP += 15;
            gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);
            double takingPosicion = PosicionP;
            PosicionP -= 15;
            for (int z = 0; z <= hemoparasitos.Count - 1; z++)
            {
                PosicionP += 15;
                Margen = new XRect(5, PosicionP, 120, 14);
                Hemo hemo = hemoparasitos.ElementAt(z);
                gfx.DrawString(hemo.Descripcion, fontRegular2, blueBrush, Margen, XStringFormats.Center);
                gfx.DrawLine(pen, 10, PosicionP, 575, PosicionP);

            }
            point = new XPoint(10, 110);
            size = new XSize(565, PosicionP - 95);

            rect = new XRect(point, size);
            gfx.DrawRectangle(pen, rect);


            PosicionX = 120;
            //gfx.DrawLine(pen, PosicionX, 110, PosicionX, 320);
            for (int x = 1; x < 8; x++)
            {
                gfx.DrawLine(pen, PosicionX, takingPosicion, PosicionX, PosicionP + 15);
                PosicionX += 65;
            }

            return document;
        }
    }
}
