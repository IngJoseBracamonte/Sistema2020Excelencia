using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SistemaSatHospitalario.Core.Domain.Interfaces;

namespace SistemaSatHospitalario.Infrastructure.Integration
{
    public class OrdenExternaService : IOrdenExternaService
    {
        private readonly ILogger<OrdenExternaService> _logger;

        public OrdenExternaService(ILogger<OrdenExternaService> logger)
        {
            _logger = logger;
        }

        public Task EnviarOrdenRXAsync(string estudio, string paciente, CancellationToken cancellationToken)
        {
            _logger.LogInformation("TRIGGER RX: Enviando orden de {Estudio} para el paciente {Paciente}.", estudio, paciente);
            // Simulación de envío a sistema de RX
            return Task.CompletedTask;
        }

        public Task EnviarOrdenLegacyAsync(decimal monto, int idPersona, CancellationToken cancellationToken)
        {
            _logger.LogInformation("INTEGRACION LEGACY: Registrando orden de {Monto} para ID Persona {IdPersona}.", monto, idPersona);
            // Simulación de unión con sistema legacy
            return Task.CompletedTask;
        }
    }
}
