using System;

namespace SistemaSatHospitalario.Core.Domain.Entities
{
    public class OrdenRX : OrdenDeServicio
    {
        public string EstudioSolicitado { get; private set; }
        public bool Procesada { get; private set; }
        public DateTime? FechaProcesada { get; private set; }
        public Guid AsistenteRxId { get; private set; }

        private OrdenRX() { }

        // Se cambió pacienteId a int y convenioId a int? para paridad con Legacy
        public OrdenRX(int numeroLlegada, int pacienteId, string nombrePaciente, string tipoIngreso, string estudioSolicitado, Guid asistenteId, int? convenioId = null) 
            : base(numeroLlegada, pacienteId, nombrePaciente, tipoIngreso, convenioId)
        {
            EstudioSolicitado = estudioSolicitado ?? throw new ArgumentNullException(nameof(estudioSolicitado));
            Procesada = false;
            AsistenteRxId = asistenteId;
        }

        public void MarcarComoProcesada()
        {
            if (Procesada) throw new InvalidOperationException("La orden ya fue procesada.");
            Procesada = true;
            FechaProcesada = DateTime.UtcNow;
        }
    }
}
