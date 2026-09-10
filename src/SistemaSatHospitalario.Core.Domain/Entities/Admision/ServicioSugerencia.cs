using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
   public class ServicioSugerencia
{
    public Guid Id { get; private set; }
    public Guid ServicioOrigenId { get; private set; }
    public Guid ServicioSugeridoId { get; private set; }

    public virtual ServicioClinico ServicioOrigen { get; private set; } = null!;
    public virtual ServicioClinico ServicioSugerido { get; private set; } = null!;

    private ServicioSugerencia() { }

    public ServicioSugerencia(Guid servicioOrigenId, Guid servicioSugeridoId)
    {
        if (servicioOrigenId == Guid.Empty) throw new ArgumentException("El servicio origen es obligatorio.");
        if (servicioSugeridoId == Guid.Empty) throw new ArgumentException("El servicio sugerido es obligatorio.");
        
        // ✅ Evitar auto-sugerencia
        if (servicioOrigenId == servicioSugeridoId)
            throw new InvalidOperationException("Un servicio no puede sugerirse a sí mismo.");

        Id = Guid.NewGuid();
        ServicioOrigenId = servicioOrigenId;
        ServicioSugeridoId = servicioSugeridoId;
    }
}
}