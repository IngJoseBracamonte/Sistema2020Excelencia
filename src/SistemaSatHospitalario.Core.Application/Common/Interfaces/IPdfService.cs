using System.Threading.Tasks;
using SistemaSatHospitalario.Core.Application.DTOs.Admision;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface IPdfService
    {
        /// <summary>
        /// Genera el comprobante de facturación en formato PDF.
        /// </summary>
        /// <param name="data">Datos del recibo normalizados.</param>
        /// <returns>Bytes del archivo PDF generado.</returns>
        byte[] GenerarReciboPdf(ReciboPdfDto data);
        
        /// <summary>
        /// Genera el documento de Compromiso de Pago en formato PDF.
        /// </summary>
        /// <param name="data">Datos del compromiso de pago y logo en base64 opcional.</param>
        /// <param name="logoBase64">Logo opcional en Base64.</param>
        /// <returns>Bytes del archivo PDF generado.</returns>
        byte[] GenerarCompromisoPagoPdf(CompromisoPagoDto data, string? logoBase64);
    }
}
