using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Registra transferencias directas, reposiciones y cambios físicos de insumos/tallas entre sedes y sub-áreas
    /// gestionados por el Supervisor de Inventario, garantizando consistencia atómica y trazabilidad de stock.
    /// </summary>
    public class TransferenciaReposicionStock
    {
        public Guid Id { get; private set; }
        public Guid InsumoId { get; private set; }
        public Guid SedeOrigenId { get; private set; }
        public Guid SedeDestinoId { get; private set; }
        public decimal Cantidad { get; private set; }
        public string Motivo { get; private set; }
        public string? Observaciones { get; private set; }
        public DateTime FechaTransferencia { get; private set; }
        public string UsuarioId { get; private set; }

        // Navegación
        public virtual Insumo Insumo { get; private set; }
        public virtual Sede SedeOrigen { get; private set; }
        public virtual Sede SedeDestino { get; private set; }

        protected TransferenciaReposicionStock() { }

        public TransferenciaReposicionStock(
            Guid insumoId,
            Guid sedeOrigenId,
            Guid sedeDestinoId,
            decimal cantidad,
            string motivo,
            string usuarioId,
            string? observaciones = null)
        {
            if (insumoId == Guid.Empty)
                throw new ArgumentException("El insumo es obligatorio.", nameof(insumoId));
            if (sedeOrigenId == Guid.Empty)
                throw new ArgumentException("La sede/sub-área de origen es obligatoria.", nameof(sedeOrigenId));
            if (sedeDestinoId == Guid.Empty)
                throw new ArgumentException("La sede/sub-área de destino es obligatoria.", nameof(sedeDestinoId));
            if (sedeOrigenId == sedeDestinoId)
                throw new ArgumentException("La sede de origen y destino no pueden ser iguales.", nameof(sedeDestinoId));
            if (cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero.", nameof(cantidad));
            if (string.IsNullOrWhiteSpace(usuarioId))
                throw new ArgumentException("El usuario operador es obligatorio.", nameof(usuarioId));

            Id = Guid.NewGuid();
            InsumoId = insumoId;
            SedeOrigenId = sedeOrigenId;
            SedeDestinoId = sedeDestinoId;
            Cantidad = cantidad;
            Motivo = string.IsNullOrWhiteSpace(motivo) ? "Reposicion" : motivo.Trim();
            UsuarioId = usuarioId.Trim();
            Observaciones = observaciones?.Trim();
            FechaTransferencia = DateTime.UtcNow;
        }
    }
}
