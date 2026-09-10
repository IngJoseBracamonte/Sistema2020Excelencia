using System;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision;

/// <summary>
/// Entidad inmutable para el registro de historial de observaciones, motivos de reprogramación e hitos quirúrgicos.
/// </summary>
public class CirugiaObservacionHistorial
{
    public Guid Id { get; private set; }
    public Guid OrdenCirugiaId { get; private set; }
    public string Observacion { get; private set; } = string.Empty;
    public TipoObservacionCirugiaConstants Tipo { get; private set; }
    public DateTime FechaRegistro { get; private set; }

    /// <summary>
    /// FK lógica a Usuarios (Identity, PK Guid). Nullable para registros automáticos del sistema.
    /// </summary>
    public Guid? UsuarioRegistroId { get; private set; }

    // Propiedad de navegación EF Core
    public virtual OrdenCirugia OrdenCirugia { get; private set; } = null!;

    /// <summary>
    /// Constructor requerido por Entity Framework Core.
    /// </summary>
    protected CirugiaObservacionHistorial() { }

    /// <summary>
    /// Crea un nuevo registro de historial u observación asociada a una orden quirúrgica.
    /// </summary>
    public CirugiaObservacionHistorial(
        Guid ordenCirugiaId, 
        string observacion, 
        TipoObservacionCirugiaConstants tipo, 
        Guid? usuarioRegistroId = null)
    {
        if (ordenCirugiaId == Guid.Empty)
            throw new ArgumentException("El ID de la orden de cirugía es obligatorio.", nameof(ordenCirugiaId));

        ArgumentException.ThrowIfNullOrWhiteSpace(observacion, nameof(observacion));

        Id = Guid.NewGuid();
        OrdenCirugiaId = ordenCirugiaId;
        Observacion = observacion.Trim();
        Tipo = tipo;
        FechaRegistro = DateTime.UtcNow;
        UsuarioRegistroId = usuarioRegistroId;
    }
}