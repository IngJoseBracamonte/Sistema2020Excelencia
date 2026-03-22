using System.Threading.Tasks;

namespace SistemaSatHospitalario.Core.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
    }
}
