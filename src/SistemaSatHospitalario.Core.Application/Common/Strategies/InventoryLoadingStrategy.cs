using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Application.Common.Services;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public class InventoryLoadingStrategy : IServiceLoadingStrategy
    {
        private readonly IInventoryService _inventoryService;
        private readonly IApplicationDbContext _context;

        public InventoryLoadingStrategy(IInventoryService inventoryService, IApplicationDbContext context)
        {
            _inventoryService = inventoryService;
            _context = context;
        }

        public bool CanHandle(string tipoServicio, ServicioClinico? baseService)
        {
            return tipoServicio.Equals("Insumo", StringComparison.OrdinalIgnoreCase) || 
                   tipoServicio.Equals("Medicamento", StringComparison.OrdinalIgnoreCase) ||
                   (baseService != null && baseService.Category == ServiceCategory.Insumo);
        }

        public Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken)
        {
            // La deducción de inventario se ejecuta de manera universal en el coordinador (CargarServicioACuentaCommandHandler)
            // para dar soporte a recetas/BOMs en todos los tipos de servicios cargados (procedimientos, laboratorios, etc.)
            return Task.CompletedTask;
        }
    }
}
