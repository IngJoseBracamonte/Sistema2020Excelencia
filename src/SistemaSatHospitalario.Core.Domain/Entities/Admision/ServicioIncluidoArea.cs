using System;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class ServicioIncluidoArea
    {
        public Guid AreaClinicaId { get; private set; }
        public Guid ServicioClinicoId { get; private set; }
        public bool Activo { get; private set; }

        public virtual AreaClinica AreaClinica { get; private set; }
        public virtual ServicioClinico ServicioClinico { get; private set; }

        protected ServicioIncluidoArea() { }

        public ServicioIncluidoArea(Guid areaClinicaId, Guid servicioClinicoId)
        {
            AreaClinicaId = areaClinicaId;
            ServicioClinicoId = servicioClinicoId;
            Activo = true;
        }

        public void Activar()
        {
            Activo = true;
        }

        public void Desactivar()
        {
            Activo = false;
        }
    }
}
