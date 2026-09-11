namespace SistemaSatHospitalario.Core.Domain.Constants
{
public static class AreaConstants
{
    // Constantes reales utilizables en atributos y metadatos
    public const string ClasificacionId_Cama_String = "60000000-0000-0000-0000-000000000001";
    public const string ClasificacionId_Quirofano_String = "60000000-0000-0000-0000-000000000002";
    public const string ClasificacionId_SalaParto_String = "60000000-0000-0000-0000-000000000003";

    // Guids inmutables listos para usar en tus entidades y consultas
    public static readonly Guid ClasificacionId_Cama = Guid.Parse(ClasificacionId_Cama_String);
    public static readonly Guid ClasificacionId_Quirofano = Guid.Parse(ClasificacionId_Quirofano_String);
    public static readonly Guid ClasificacionId_SalaParto = Guid.Parse(ClasificacionId_SalaParto_String);
    }   
}