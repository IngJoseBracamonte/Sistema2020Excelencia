using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Common;

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
        DbSet<LogAuditoriaPrecio> AuditLogsPrecios { get; }
        DbSet<HorarioAtencionMedico> HorariosAtencionMedicos { get; }
        DbSet<ServicioSugerencia> ServiciosSugerencias { get; }
        DbSet<OrdenImagen> OrdenesImagenes { get; set; }
        DbSet<CatalogoMetodoPago> CatalogoMetodosPago { get; }
        DbSet<DocumentLog> DocumentLogs { get; }
        DbSet<Notification> Notifications { get; }
        DbSet<HonorarioConfig> HonorariosConfig { get; }
        DbSet<LogAsignacionHonorario> LogsAsignacionHonorario { get; }
        DbSet<HonorariumMappingRule> HonorariumMappingRules { get; }


        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        Microsoft.EntityFrameworkCore.ChangeTracking.ChangeTracker ChangeTracker { get; }
        Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        Task<Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
        Microsoft.EntityFrameworkCore.Infrastructure.DatabaseFacade Database { get; }
    }
}
