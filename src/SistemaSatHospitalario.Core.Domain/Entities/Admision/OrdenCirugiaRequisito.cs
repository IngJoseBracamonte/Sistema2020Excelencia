using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    /// <summary>
    /// Entidad de checklist transaccional que vincula un RequisitoCirugia a una OrdenCirugia específica.
    /// </summary>
    public class OrdenCirugiaRequisito
    {
        public Guid Id { get; private set; }
        public Guid OrdenCirugiaId { get; private set; }
        public Guid RequisitoCirugiaId { get; private set; }
        public bool Cumplido { get; private set; }
        public DateTime? FechaVerificacion { get; private set; }
        public string? VerificadoPor { get; private set; }

        public virtual OrdenCirugia OrdenCirugia { get; private set; }
        public virtual RequisitoCirugia RequisitoCirugia { get; private set; }

        protected OrdenCirugiaRequisito() { }

        public OrdenCirugiaRequisito(Guid ordenCirugiaId, Guid requisitoCirugiaId, bool cumplido = false)
        {
            if (ordenCirugiaId == Guid.Empty)
                throw new ArgumentException("El ID de la orden de cirugía es obligatorio.", nameof(ordenCirugiaId));
            if (requisitoCirugiaId == Guid.Empty)
                throw new ArgumentException("El ID del requisito de cirugía es obligatorio.", nameof(requisitoCirugiaId));

            Id = Guid.NewGuid();
            OrdenCirugiaId = ordenCirugiaId;
            RequisitoCirugiaId = requisitoCirugiaId;
            Cumplido = cumplido;
            if (cumplido)
            {
                FechaVerificacion = DateTime.UtcNow;
            }
        }

        public void SetCumplido(bool cumplido, string verificadoPor)
        {
            Cumplido = cumplido;
            FechaVerificacion = cumplido ? DateTime.UtcNow : null;
            VerificadoPor = cumplido ? verificadoPor : null;
        }
    }
}
