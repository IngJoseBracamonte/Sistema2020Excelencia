using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class CierreInventario
    {
        public Guid Id { get; private set; }
        public Guid SedeId { get; private set; }
        public virtual Sede Sede { get; private set; } = null!;
        public DateTime FechaCierre { get; private set; }

        // Auditoría e Identidad (3FN Limpio)
        public Guid? UsuarioId { get; private set; }
        public string? Usuario { get; private set; } // Alias legacy opcional

        public string Observaciones { get; private set; } = string.Empty;

        // Colección de ítems auditados en el cierre (1:N)
        public virtual ICollection<CierreInventarioDetalle> Detalles { get; private set; } = new List<CierreInventarioDetalle>();

        protected CierreInventario() { }

        public CierreInventario(
            Guid sedeId, 
            Guid? usuarioId = null,
            string? usuario = null, 
            string? observaciones = null)
        {
            if (sedeId == Guid.Empty) throw new ArgumentException("La sede es requerida.", nameof(sedeId));

            Id = Guid.NewGuid();
            SedeId = sedeId;
            FechaCierre = DateTime.UtcNow;
            
            // 3FN: Asignar ID directo o intentar parsear string si vino el alias
            UsuarioId = usuarioId ?? (Guid.TryParse(usuario, out var parsed) ? parsed : (Guid?)null);
            Usuario = usuario;

            Observaciones = observaciones ?? string.Empty;
        }

        public void AgregarDetalle(CierreInventarioDetalle detalle)
        {
            ArgumentNullException.ThrowIfNull(detalle);
            Detalles.Add(detalle);
        }
    }
}