using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Core.Application.Common.Services
{
    /// <summary>
    /// Servicio para validar que los IDs de áreas clínicas existen en la base de datos
    /// </summary>
    public interface IAreaClinicaValidationService
    {
        /// <summary>
        /// Valida que un AreaClinicaId existe en la base de datos
        /// </summary>
        /// <param name="areaClinicaId">ID del área clínica a validar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>True si el área existe, False en caso contrario</returns>
        Task<bool> ValidateAreaClinicaExistsAsync(Guid? areaClinicaId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida que un AreaClinicaId existe y lanza una excepción si no existe
        /// </summary>
        /// <param name="areaClinicaId">ID del área clínica a validar</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <exception cref="ArgumentException">Se lanza si el área no existe</exception>
        Task ValidateAreaClinicaExistsOrThrowAsync(Guid? areaClinicaId, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Implementación del servicio de validación de áreas clínicas
    /// </summary>
    public class AreaClinicaValidationService : IAreaClinicaValidationService
    {
        private readonly IApplicationDbContext _context;

        public AreaClinicaValidationService(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateAreaClinicaExistsAsync(Guid? areaClinicaId, CancellationToken cancellationToken = default)
        {
            if (!areaClinicaId.HasValue || areaClinicaId.Value == Guid.Empty)
                return true; // null o empty es válido (no se asigna área)

            if (await _context.AreasClinicas.AsNoTracking().AnyAsync(a => a.Id == areaClinicaId.Value, cancellationToken))
            {
                return true;
            }

            return await _context.Sedes.AsNoTracking().AnyAsync(s => s.Id == areaClinicaId.Value, cancellationToken);
        }

        public async Task ValidateAreaClinicaExistsOrThrowAsync(Guid? areaClinicaId, CancellationToken cancellationToken = default)
        {
            if (!areaClinicaId.HasValue || areaClinicaId.Value == Guid.Empty)
                return; // null o empty es válido (no se asigna área)

            var exists = await ValidateAreaClinicaExistsAsync(areaClinicaId, cancellationToken);
            if (!exists)
            {
                throw new ArgumentException($"El área clínica con ID '{areaClinicaId}' no existe en la base de datos.", nameof(areaClinicaId));
            }
        }
    }
}
