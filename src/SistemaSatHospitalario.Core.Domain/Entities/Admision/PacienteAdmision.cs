using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class PacienteAdmision
    {
        // Se revirtió a Guid para identidad interna del sistema nuevo (V11.0 Sync Pro)
        public Guid Id { get; protected set; }
        
        // Campo de enlace con el Sistema 2020 Legacy (IDPersona Autoincrementable)
        public int? IdPacienteLegacy { get; protected set; }
        
        public string CedulaPasaporte { get; protected set; }
        public string NombreCorto { get; protected set; }
        public string TelefonoContact { get; protected set; }
        public DateTime? FechaNacimiento { get; protected set; }
        public string NombreCompleto => NombreCorto; // Alias para compatibilidad con reportes avanzados

        // Historial de ordenes
        private readonly List<OrdenDeServicio> _ordenes = new();
        public IReadOnlyCollection<OrdenDeServicio> Ordenes => _ordenes.AsReadOnly();

        protected PacienteAdmision() { }

        public PacienteAdmision(string cedulaPasaporte, string nombreCorto, string telefonoContact, int? idLegacy = null, DateTime? fechaNacimiento = null)
        {
            Id = Guid.NewGuid();
            CedulaPasaporte = cedulaPasaporte ?? throw new ArgumentNullException(nameof(cedulaPasaporte));
            NombreCorto = nombreCorto ?? throw new ArgumentNullException(nameof(nombreCorto));
            TelefonoContact = telefonoContact;
            IdPacienteLegacy = idLegacy;
            FechaNacimiento = fechaNacimiento;
        }

        public void VincularLegacy(int legacyId)
        {
            IdPacienteLegacy = legacyId;
        }

        public void ActualizarDatos(string nombreCorto, string telefonoContact, DateTime? fechaNacimiento = null)
        {
            NombreCorto = nombreCorto;
            TelefonoContact = telefonoContact;
            FechaNacimiento = fechaNacimiento;
        }
    }
}
