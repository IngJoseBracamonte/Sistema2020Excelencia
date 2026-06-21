using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Infrastructure.Persistence.Contexts;
using SistemaSatHospitalario.Infrastructure.Services;
using SistemaSatHospitalario.Core.Application.Queries.Admision;
using System.Threading.Tasks;
using QuestPDF.Infrastructure;

class Program
{
    static async Task Main()
    {
        string connStr = "server=localhost;database=SatHospitalario;user=root;password=Labordono1818;AllowPublicKeyRetrieval=True;SslMode=None";
        var optionsBuilder = new DbContextOptionsBuilder<SatHospitalarioDbContext>();
        optionsBuilder.UseMySql(connStr, ServerVersion.AutoDetect(connStr));

        try
        {
            using var context = new SatHospitalarioDbContext(optionsBuilder.Options);
            Console.WriteLine("Context loaded successfully.");

            var queryId = Guid.Parse("51a704d8-18a1-4562-bd3b-c969bbc182da");
            var handler = new GetReciboPdfQueryHandler(context);
            var query = new GetReciboPdfQuery { ReciboId = queryId };

            Console.WriteLine("Executing GetReciboPdfQueryHandler...");
            var data = await handler.Handle(query, default);

            if (data == null)
            {
                Console.WriteLine("Error: GetReciboPdfQuery returned null.");
                return;
            }

            Console.WriteLine($"Query success! Recibo Nro: {data.NumeroRecibo}, Paciente: {data.PacienteNombre}, Total: {data.TotalUSD} USD");

            Console.WriteLine("Initializing PdfGenerationService...");
            var pdfService = new PdfGenerationService();

            Console.WriteLine("Calling GenerarReciboPdf...");
            var pdfBytes = pdfService.GenerarReciboPdf(data);
            Console.WriteLine($"PDF generated successfully! Size: {pdfBytes.Length} bytes");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception occurred:");
            Console.WriteLine(ex.ToString());
        }
    }
}
