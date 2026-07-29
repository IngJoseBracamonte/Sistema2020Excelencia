using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Insumo
    {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public virtual ICollection<StockSede> StocksPorSede { get; private set; } = new List<StockSede>();
        public decimal StockActual => Enumerable.Sum(StocksPorSede, s => s.StockActual);
        public UnidadMedida UnidadMedidaBase { get; private set; }
        public decimal CostoUnitarioBaseUSD { get; private set; }
        public bool PermiteFraccionamiento { get; private set; }
        public string Categoria { get; private set; }

        // Borrado Lógico (Soft Delete)
        public bool IsDeleted { get; private set; }
        public DateTime? FechaInactivacion { get; private set; }
        public bool OcultoEnTraslados { get; private set; }

        // Relación N:M Principios Activos
        public virtual ICollection<InsumoPrincipioActivo> PrincipiosActivos { get; private set; } = new List<InsumoPrincipioActivo>();

        // Propiedades deprecadas/compatibilidad (eliminadas de la lógica de negocio activa)
        public string? ReactivosCombinados { get; private set; }
        public string? Indicaciones { get; private set; }
        public DateTime? FechaVencimiento { get; private set; }

        protected Insumo() { }

        public Insumo(string codigo, string nombre, decimal stockActual, UnidadMedida unidadMedidaBase, decimal costoUnitarioBaseUSD, bool permiteFraccionamiento = true, string categoria = "Medicamento")
        {
            Id = Guid.NewGuid();
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            UnidadMedidaBase = unidadMedidaBase;
            CostoUnitarioBaseUSD = costoUnitarioBaseUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
            Categoria = categoria;
            OcultoEnTraslados = false;
            IsDeleted = false;
            FechaInactivacion = null;

            if (stockActual > 0)
            {
                StocksPorSede.Add(new StockSede(Id, Guid.Empty, stockActual));
            }
        }

        public void RegistrarMovimientoStock(decimal cantidadBase)
        {
        }

        public void EstablecerStockCierre(decimal stockFisicoReal)
        {
        }

        public void ActualizarDetalles(string nombre, decimal costoUSD)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            CostoUnitarioBaseUSD = costoUSD;
        }

        public void ActualizarDetalles(string nombre, UnidadMedida unidadMedidaBase, decimal costoUSD, bool permiteFraccionamiento, string categoria)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            UnidadMedidaBase = unidadMedidaBase;
            CostoUnitarioBaseUSD = costoUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
            Categoria = categoria;
        }

        // Overload para compatibilidad legacy mientras se completa migración total
        public void ActualizarDetalles(string nombre, UnidadMedida unidadMedidaBase, decimal costoUSD, bool permiteFraccionamiento, string categoria, string? reactivos, string? indicaciones, DateTime? vencimiento)
        {
            ActualizarDetalles(nombre, unidadMedidaBase, costoUSD, permiteFraccionamiento, categoria);
            ReactivosCombinados = reactivos;
            Indicaciones = indicaciones;
            FechaVencimiento = vencimiento;
        }

        public void SoftDelete()
        {
            IsDeleted = true;
            FechaInactivacion = DateTime.UtcNow;
            OcultoEnTraslados = true;
        }

        public void Restaurar()
        {
            IsDeleted = false;
            FechaInactivacion = null;
            OcultoEnTraslados = false;
        }

        public void AlternarOcultoEnTraslados(bool ocultar)
        {
            if (ocultar)
            {
                SoftDelete();
            }
            else
            {
                Restaurar();
            }
        }

        public void AgregarPrincipioActivo(PrincipioActivo principioActivo, string concentracion)
        {
            if (principioActivo == null) throw new ArgumentNullException(nameof(principioActivo));

            if (PrincipiosActivos.Any(p => p.PrincipioActivoId == principioActivo.Id))
            {
                throw new InvalidOperationException($"El principio activo {principioActivo.Nombre} ya está asociado a este insumo.");
            }

            PrincipiosActivos.Add(new InsumoPrincipioActivo(this, principioActivo, concentracion));
        }

        public void RemoverPrincipioActivo(Guid principioActivoId)
        {
            var item = PrincipiosActivos.FirstOrDefault(p => p.PrincipioActivoId == principioActivoId);
            if (item != null)
            {
                PrincipiosActivos.Remove(item);
            }
        }
    }
}
