using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Interfaces;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;

namespace SistemaSatHospitalario.Infrastructure.Persistence.Repositories
{
    public class BillingRepository : IBillingRepository
    {
        private readonly SatHospitalarioDbContext _context;

        public BillingRepository(SatHospitalarioDbContext context)
        {
            _context = context;
        }

        public async Task<CuentaServicios?> ObtenerCuentaAbiertaPorPacienteAsync(int pacienteId, CancellationToken cancellationToken)
        {
            return await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.PacienteId == pacienteId && c.Estado == "Abierta", cancellationToken);
        }

        public async Task<CuentaServicios?> ObtenerCuentaPorIdAsync(Guid cuentaId, CancellationToken cancellationToken)
        {
            return await _context.CuentasServicios
                .Include(c => c.Detalles)
                .FirstOrDefaultAsync(c => c.Id == cuentaId, cancellationToken);
        }

        public async Task<List<CuentaServicios>> ObtenerCuentasPorPacienteAsync(int pacienteId, CancellationToken cancellationToken)
        {
            return await _context.CuentasServicios
                .Include(c => c.Detalles)
                .Where(c => c.PacienteId == pacienteId)
                .OrderByDescending(c => c.FechaCreacion)
                .ToListAsync(cancellationToken);
        }

        public async Task AgregarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken)
        {
            await _context.CuentasServicios.AddAsync(cuenta, cancellationToken);
        }

        public Task ActualizarCuentaAsync(CuentaServicios cuenta, CancellationToken cancellationToken)
        {
            _context.CuentasServicios.Update(cuenta);
            return Task.CompletedTask;
        }

        public async Task AgregarCitaMedicaAsync(CitaMedica cita, CancellationToken cancellationToken)
        {
            await _context.CitasMedicas.AddAsync(cita, cancellationToken);
        }

        public async Task<bool> ExisteCitaSimultaneaAsync(Guid medicoId, DateTime hora, CancellationToken cancellationToken)
        {
            return await _context.CitasMedicas.AnyAsync(c => c.MedicoId == medicoId && c.HoraPautada == hora && c.EstadoAtencion != "Cancelado", cancellationToken);
        }

        public async Task GuardarCambiosAsync(CancellationToken cancellationToken)
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
