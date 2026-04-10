using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using SistemaSatHospitalario.Core.Application.Common.Interfaces;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;

namespace SistemaSatHospitalario.Core.Application.Commands.Admision
{
    // CREATE
    public class CreateMedicoCommand : IRequest<Guid>
    {
        public string Nombre { get; set; }
        public Guid EspecialidadId { get; set; }
        public decimal HonorarioBase { get; set; }
    }

    public class CreateMedicoCommandHandler : IRequestHandler<CreateMedicoCommand, Guid>
    {
        private readonly IApplicationDbContext _context;

        public CreateMedicoCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> Handle(CreateMedicoCommand request, CancellationToken cancellationToken)
        {
            if (request.EspecialidadId == Guid.Empty) throw new Exception("La especialidad es obligatoria");
            var entity = new Medico(request.Nombre, request.EspecialidadId, request.HonorarioBase);
            _context.Medicos.Add(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return entity.Id;
        }
    }

    // UPDATE
    public class UpdateMedicoCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public Guid EspecialidadId { get; set; }
        public decimal HonorarioBase { get; set; }
        public bool Activo { get; set; }
    }

    public class UpdateMedicoCommandHandler : IRequestHandler<UpdateMedicoCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public UpdateMedicoCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(UpdateMedicoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Medicos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity == null) return false;
            
            if (request.EspecialidadId == Guid.Empty) throw new Exception("La especialidad es obligatoria para médicos activos");
            entity.Update(request.Nombre, request.EspecialidadId, request.HonorarioBase);
            entity.SetEstado(request.Activo);

            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }

    // DELETE (Physical or Logical, here using logical/state)
    public class DeleteMedicoCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }

    public class DeleteMedicoCommandHandler : IRequestHandler<DeleteMedicoCommand, bool>
    {
        private readonly IApplicationDbContext _context;

        public DeleteMedicoCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteMedicoCommand request, CancellationToken cancellationToken)
        {
            var entity = await _context.Medicos.FindAsync(new object[] { request.Id }, cancellationToken);
            if (entity == null) return false;

            _context.Medicos.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
