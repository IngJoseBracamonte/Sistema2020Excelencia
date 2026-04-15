using System;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    /// <summary>
    /// [PHASE-6] Standardized Provider for Date/Time operations.
    /// Prevents "Manual Offset Bugs" by centralizing Hospital Local Time logic.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Precise UTC Now.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Local Time of the Hospital (Fixed at UTC-4 for current deployment).
        /// </summary>
        DateTime HospitalNow { get; }

        /// <summary>
        /// Start of the current operational day in UTC.
        /// </summary>
        DateTime TodayUtc { get; }

        /// <summary>
        /// Start of the next operational day in UTC.
        /// </summary>
        DateTime TomorrowUtc { get; }
    }
}
