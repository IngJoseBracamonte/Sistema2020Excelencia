using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    public class CreateRequisitoCirugiaCommand : IRequest<Guid>
    {
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool EsActivo { get; set; } = true;
    }

    public class CreateRequisitoCirugiaCommandHandler : IRequestHandler<CreateRequisitoCirugiaCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateRequisitoCirugiaCommandHandler(IApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Guid> Handle(CreateRequisitoCirugiaCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Nombre))
                throw new ArgumentException("El nombre del requisito es obligatorio.");

            var existe = await _context.RequisitosCirugia
                .AnyAsync(r => r.Nombre.ToLower() == request.Nombre.Trim().ToLower(), cancellationToken);

            if (existe)
                throw new InvalidOperationException($"Ya existe un requisito quirúrgico registrado con el nombre: {request.Nombre}");

            var entidad = new RequisitoCirugia(request.Nombre, request.Descripcion, request.EsActivo);

            _context.RequisitosCirugia.Add(entidad);
            await _context.SaveChangesAsync(cancellationToken);

            return entidad.Id;
        }
    }
}
