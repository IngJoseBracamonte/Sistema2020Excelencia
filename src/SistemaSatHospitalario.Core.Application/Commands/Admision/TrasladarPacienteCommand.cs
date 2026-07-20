using System;
using MediatR;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class TrasladarPacienteCommand : IRequest<TrasladarPacienteResult>
    {
        public Guid PacienteId { get; set; }
        public string NuevoTipoIngreso { get; set; } = string.Empty;
        public int? NuevoConvenioId { get; set; }
        public string UsuarioTraslado { get; set; } = string.Empty;
        public bool EsEgreso { get; set; } // Si es true, representa el alta del paciente (no abre nueva cuenta)
        public Guid? NuevaAreaClinicaId { get; set; }
        public string? NuevaSubAreaClinica { get; set; }
        public DateTime? FechaHoraEgresoEfectiva { get; set; }
        public decimal? MontoSobrescrito { get; set; }
    }

    public class TrasladarPacienteResult
    {
        public Guid CuentaCerradaId { get; set; }
        public Guid? NuevaCuentaId { get; set; }
        public Guid? CuentaPrincipalId { get; set; }
    }
}
