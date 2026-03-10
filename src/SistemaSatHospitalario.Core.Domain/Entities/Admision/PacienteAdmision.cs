using System;
using System.Collections.Generic;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PacienteAdmision
    {
        public Guid Id { get; protected set; }
        public string CedulaPasaporte { get; protected set; }
        public string NombreCorto { get; protected set; }
        public string TelefonoContact { get; protected set; }

        // Historial de ordenes
        private readonly List<OrdenDeServicio> _ordenes = new();
        public IReadOnlyCollection<OrdenDeServicio> Ordenes => _ordenes.AsReadOnly();

        protected PacienteAdmision() { }

        public PacienteAdmision(string cedulaPasaporte, string nombreCorto, string telefonoContact)
        {
            Id = Guid.NewGuid();
            CedulaPasaporte = cedulaPasaporte ?? throw new ArgumentNullException(nameof(cedulaPasaporte));
            NombreCorto = nombreCorto ?? throw new ArgumentNullException(nameof(nombreCorto));
            TelefonoContact = telefonoContact;
        }

        public void ActualizarDatos(string nombreCorto, string telefonoContact)
        {
            NombreCorto = nombreCorto;
            TelefonoContact = telefonoContact;
        }
    }
}
