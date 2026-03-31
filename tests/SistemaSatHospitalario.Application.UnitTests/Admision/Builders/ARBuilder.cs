using System;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using System.Reflection;

namespace SistemaSatHospitalario.Application.UnitTests.Admision.Builders
{
    public class ARBuilder
    {
        private Guid _id = Guid.NewGuid();
        private Guid _cuentaId = Guid.NewGuid();
        private Guid _pacienteId = Guid.NewGuid();
        private decimal _total = 100.00m;
        private decimal _pagado = 0m;
        private string _estado = EstadoConstants.Pendiente;

        public ARBuilder WithId(Guid id) { _id = id; return this; }
        public ARBuilder WithTotal(decimal total) { _total = total; return this; }
        public ARBuilder WithPagado(decimal pagado) { _pagado = pagado; return this; }
        public ARBuilder WithEstado(string estado) { _estado = estado; return this; }

        public CuentaPorCobrar Build()
        {
            var ar = new CuentaPorCobrar(_cuentaId, _pacienteId, _total, _pagado);
            
            // Usamos reflexión para setear el ID privado si es necesario en las pruebas
            typeof(CuentaPorCobrar).GetProperty("Id")?.SetValue(ar, _id);
            typeof(CuentaPorCobrar).GetProperty("Estado")?.SetValue(ar, _estado);
            
            return ar;
        }
    }
}
