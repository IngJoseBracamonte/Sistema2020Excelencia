using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Application.Commands.Admision;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Entities.Legacy;
using SistemaSatHospitalario.Core.Domain.Entities;
using SistemaSatHospitalario.Core.Domain.Enums;
using SistemaSatHospitalario.Core.Domain.Interfaces.Legacy;

namespace SistemaSatHospitalario.Core.Application.Common.Strategies
{
    public class LegacyLabLoadingStrategy : IServiceLoadingStrategy
    {
        private readonly ILegacyLabRepository _legacyRepository;
        private readonly IApplicationDbContext _context;
        private readonly ILogger<LegacyLabLoadingStrategy> _logger;

        public LegacyLabLoadingStrategy(
            ILegacyLabRepository legacyRepository, 
            IApplicationDbContext context, 
            ILogger<LegacyLabLoadingStrategy> logger)
        {
            _legacyRepository = legacyRepository;
            _context = context;
            _logger = logger;
        }

        public bool CanHandle(string tipoServicio, ServicioClinico? baseService)
        {
            return EstadoConstants.EsLaboratorio(tipoServicio) || 
                   (baseService != null && baseService.Category == ServiceCategory.Laboratory);
        }

        public async Task ExecuteAsync(
            CargarServicioACuentaCommand request, 
            CuentaServicios cuenta, 
            PacienteAdmision paciente, 
            DetalleServicioCuenta detalle, 
            ServicioClinico? baseService, 
            CancellationToken cancellationToken)
        {
            // Regla de Negocio 1: Sistema Legacy - Conectarse y generar la orden en 'sistema2020'.
            // Autodetección: Se genera si OrigenCarga está definido OR si es flujo clínico (Hospitalización, Emergencia, UCI).
            bool isClinical = !string.IsNullOrEmpty(request.OrigenCarga) || 
                              cuenta.TipoIngreso == EstadoConstants.Hospitalizacion || 
                              cuenta.TipoIngreso == EstadoConstants.Emergencia || 
                              cuenta.TipoIngreso == "UCI";

            if (isClinical && int.TryParse(detalle.LegacyMappingId, out int idPerfil))
            {
                _logger.LogInformation("[LEGACY-SYNC-IMMEDIATE] Generando orden de laboratorio legacy inmediata para perfil {PerfilId}...", idPerfil);

                if (!paciente.IdPacienteLegacy.HasValue || paciente.IdPacienteLegacy.Value == 0)
                {
                    var existingLegacy = await _legacyRepository.GetPatientByCedulaAsync(paciente.CedulaPasaporte, cancellationToken);
                    if (existingLegacy != null)
                    {
                        paciente.VincularLegacy(existingLegacy.IdPersona);
                    }
                    else
                    {
                        var legacyPatient = new DatosPersonalesLegacy
                        {
                            Cedula = paciente.CedulaPasaporte,
                            Nombre = paciente.NombreCorto,
                            Apellidos = "",
                            Sexo = "M",
                            Fecha = DateTime.Now.AddYears(-20).ToString("yyyy-MM-dd"),
                            Celular = paciente.TelefonoContact ?? "",
                            Telefono = "",
                            Correo = "",
                            TipoCorreo = "@gmail.com",
                            CodigoCelular = "0414",
                            CodigoTelefono = "0212"
                        };
                        int newId = await _legacyRepository.CreatePatientLegacyAsync(legacyPatient, cancellationToken);
                        if (newId > 0)
                        {
                            paciente.VincularLegacy(newId);
                        }
                    }
                    await _context.SaveChangesAsync(cancellationToken);
                }

                if (paciente.IdPacienteLegacy.HasValue && paciente.IdPacienteLegacy.Value > 0)
                {
                    var perfilesFacturados = new List<PerfilesFacturadosLegacy>
                    {
                        new PerfilesFacturadosLegacy
                        {
                            IdOrden = 0,
                            IdPersona = paciente.IdPacienteLegacy.Value,
                            IdPerfil = idPerfil,
                            PrecioPerfil = detalle.Precio * detalle.Cantidad
                        }
                    };

                    var ordenLegacy = new OrdenLegacy
                    {
                        IdPersona = paciente.IdPacienteLegacy.Value,
                        IDConvenio = request.ConvenioId ?? cuenta.ConvenioId ?? 1,
                        Fecha = DateTime.Now,
                        HoraIngreso = DateTime.Now.ToString("HH:mm:ss"),
                        PrecioF = perfilesFacturados.Sum(p => p.PrecioPerfil)
                    };

                    var idOrdenLegacy = await _legacyRepository.GenerarOrdenLaboratorioAsync(ordenLegacy, perfilesFacturados, new List<ResultadosPacienteLegacy>(), cancellationToken);
                    _logger.LogInformation("[LEGACY-SYNC-IMMEDIATE] Orden de laboratorio legacy generada exitosamente con ID: {IdOrden}", idOrdenLegacy);
                }
            }
        }
    }
}
