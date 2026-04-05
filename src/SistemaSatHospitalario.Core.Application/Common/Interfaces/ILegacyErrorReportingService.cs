using System;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface ILegacyErrorReportingService
    {
        void LogTrace(string message);
        void LogError(string message, Exception? ex = null);
    }
}
