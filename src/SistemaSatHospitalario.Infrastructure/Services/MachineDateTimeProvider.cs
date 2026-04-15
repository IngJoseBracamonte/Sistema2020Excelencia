using System;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;

namespace SistemaSatHospitalario.Infrastructure.Services
{
    /// <summary>
    /// [PHASE-6] Machine-based implementation of IDateTimeProvider.
    /// Encapsulates the hospital's operational timezone (UTC-4).
    /// </summary>
    public class MachineDateTimeProvider : IDateTimeProvider
    {
        private const int HospitalOffsetHours = -4;

        public DateTime UtcNow => DateTime.UtcNow;

        public DateTime HospitalNow => DateTime.UtcNow.AddHours(HospitalOffsetHours);

        public DateTime TodayUtc => HospitalNow.Date.AddHours(-HospitalOffsetHours);

        public DateTime TomorrowUtc => TodayUtc.AddDays(1);
    }
}
