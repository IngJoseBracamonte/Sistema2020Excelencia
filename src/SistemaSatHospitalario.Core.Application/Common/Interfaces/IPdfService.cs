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
    }
}
