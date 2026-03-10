using Conexiones.Dto;
using System.Collections.Generic;

namespace Conexiones
{
    public interface IPerfil
    {
        int Activo { get; set; }
        List<AnalisisLaboratorio> analisisLaboratorios { get; set; }
        int IdImpresionAgrupado { get; set; }
        int idMetodo { get; set; }
        int IdPerfil { get; set; }
        string NombrePerfil { get; set; }
        string Precio { get; set; }
        double PrecioDolar { get; set; }
        List<ResultadosPorAnalisisVet> resultados { get; set; }
    }
}