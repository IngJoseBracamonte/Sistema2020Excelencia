using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using System.Threading;
using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<CajaDiaria> CajasDiarias { get; }
        DbSet<ReciboFactura> RecibosFactura { get; }
        DbSet<DetallePago> DetallesPago { get; }
        DbSet<SeguroConvenio> SegurosConvenios { get; }
        DbSet<PacienteAdmision> PacientesAdmision { get; }
        DbSet<OrdenDeServicio> OrdenesDeServicio { get; }
        DbSet<OrdenRX> OrdenesRX { get; }
        DbSet<TurnoMedico> TurnosMedicos { get; }
        DbSet<IncidenciaHorario> IncidenciasHorario { get; }
        DbSet<RegistroAuditoriaIncidencia> RegistrosAuditoriaIncidencia { get; }
        DbSet<CuentaServicios> CuentasServicios { get; }
        DbSet<DetalleServicioCuenta> DetallesServicioCuenta { get; }
        DbSet<CitaMedica> CitasMedicas { get; }
        DbSet<Medico> Medicos { get; }
        DbSet<TasaCambio> TasaCambio { get; }
        DbSet<ServicioClinico> ServiciosClinicos { get; }
        DbSet<PrecioServicioConvenio> PreciosServicioConvenio { get; }
        DbSet<CuentaPorCobrar> CuentasPorCobrar { get; }
        DbSet<ErrorTicket> ErrorTickets { get; }
        DbSet<Especialidad> Especialidades { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
