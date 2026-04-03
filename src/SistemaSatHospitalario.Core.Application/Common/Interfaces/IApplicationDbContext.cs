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
        DbSet<ReservaTemporal> ReservasTemporales { get; }
        DbSet<BloqueoHorario> BloqueosHorarios { get; }
        DbSet<PrecioServicioConvenio> PreciosServicioConvenio { get; }
        DbSet<CuentaPorCobrar> CuentasPorCobrar { get; }
        DbSet<ErrorTicket> ErrorTickets { get; }
        DbSet<Especialidad> Especialidades { get; }
        DbSet<ConfiguracionGeneral> ConfiguracionGeneral { get; }
        DbSet<ConvenioPerfilPrecio> ConvenioPerfilPrecios { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker ChangeTracker { get; }
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}
