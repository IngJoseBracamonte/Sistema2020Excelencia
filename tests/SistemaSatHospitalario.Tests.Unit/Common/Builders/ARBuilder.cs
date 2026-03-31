using System;
using SistemaSatHospitalario.Core.Domain.Entities.Admision;
using SistemaSatHospitalario.Core.Domain.Constants;
using System.Reflection;

namespace SistemaSatHospitalario.Tests.Unit.Common.Builders
{
    /// <summary>
    /// Builder para CuentaPorCobrar (Patrón Senior Builders).
    /// Permite crear estados complejos de forma fluida y mantenible.
    /// </summary>
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
        public ARBuilder WithCuentaId(Guid cuentaId) { _cuentaId = cuentaId; return this; }
        public ARBuilder WithPacienteId(Guid pacienteId) { _pacienteId = pacienteId; return this; }

        public CuentaPorCobrar Build()
        {
            // El constructor de CuentaPorCobrar es el punto de entrada oficial para preservar invariantes
            var ar = new CuentaPorCobrar(_cuentaId, _pacienteId, _total, _pagado);
            
            // Usamos reflexión para setear propiedades privadas requeridas por los Mocks (Framework constraints)
            SetPrivateProperty(ar, "Id", _id);
            SetPrivateProperty(ar, "Estado", _estado);
            
            return ar;
        }

        private void SetPrivateProperty(object obj, string propertyName, object value)
        {
            var prop = obj.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(obj, value);
            }
            else
            {
                // Si el setter es privado (private set), necesitamos buscar el backing field o usar el FieldInfo
                var field = obj.GetType().GetField($"<{propertyName}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                {
                    field.SetValue(obj, value);
                }
            }
        }
    }
}
