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

        /// <summary>
        /// LEGACY (3FN): unidad de medida como enum persistido en varchar(20).
        /// Fuente de verdad: <see cref="UnidadMedidaId"/> (FK a UnidadesMedida).
        /// Alias de compatibilidad hasta el DROP de columna.
        /// </summary>
        [Obsolete("Usar UnidadMedidaId / UnidadMedidaNav. Columna legacy pendiente de DROP.")]
        public UnidadMedida UnidadMedidaBase { get; private set; }

        /// <summary>FK al catálogo UnidadesMedida (3FN).</summary>
        public int UnidadMedidaId { get; private set; }

        /// <summary>Navegación al catálogo de unidades de medida.</summary>
        public virtual UnidadMedidaCatalogo UnidadMedidaNav { get; private set; } = null!;
        public decimal CostoUnitarioBaseUSD { get; private set; }
        public bool PermiteFraccionamiento { get; private set; }

        /// <summary>
        /// LEGACY (3FN): texto libre de categoría. Fuente de verdad es <see cref="CategoriaInsumoId"/>.
        /// Se mantiene mapeado solo como alias de compatibilidad hasta el DROP de columna
        /// (delta posterior a validación en producción). No escribir desde código nuevo.
        /// </summary>
        [Obsolete("Usar CategoriaInsumoId / CategoriaInsumo. Columna legacy pendiente de DROP.")]
        public string Categoria { get; private set; }
        public Guid? CategoriaInsumoId { get; private set; }
        public virtual CategoriaInsumo? CategoriaInsumo { get; private set; }

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
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UnidadMedidaBase = unidadMedidaBase;
#pragma warning restore CS0618
            UnidadMedidaId = Constants.UnidadMedidaConstants.FromEnum(unidadMedidaBase);
            CostoUnitarioBaseUSD = costoUnitarioBaseUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
#pragma warning disable CS0618 // alias legacy; preferir AsignarCategoria(CategoriaInsumo)
            Categoria = categoria;
#pragma warning restore CS0618
            OcultoEnTraslados = false;
            IsDeleted = false;
            FechaInactivacion = null;

            var stockInicialValido = stockActual < 0 ? 0 : stockActual;
            StocksPorSede.Add(new StockSede(Id, SistemaSatHospitalario.Core.Domain.Constants.SeedConstants.SedeId_Principal, stockInicialValido));
        }

        public void RegistrarMovimientoStock(decimal cantidadBase)
        {
             // La gestión de existencias se realiza por sede en StockSede.
        }

        public void EstablecerStockCierre(decimal stockFisicoReal)
        {
             // La gestión de existencias se realiza por sede en StockSede.
        }

        public void ActualizarDetalles(string nombre, decimal costoUSD)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            CostoUnitarioBaseUSD = costoUSD;
        }

        public void ActualizarDetalles(string nombre, UnidadMedida unidadMedidaBase, decimal costoUSD, bool permiteFraccionamiento, string categoria)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            UnidadMedidaBase = unidadMedidaBase;
#pragma warning restore CS0618
            UnidadMedidaId = Constants.UnidadMedidaConstants.FromEnum(unidadMedidaBase);
            CostoUnitarioBaseUSD = costoUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
#pragma warning disable CS0618 // alias legacy; preferir AsignarCategoria(CategoriaInsumo)
            Categoria = categoria;
#pragma warning restore CS0618
        }

        public void AsignarCategoria(CategoriaInsumo categoria)
        {
            ArgumentNullException.ThrowIfNull(categoria);

            CategoriaInsumoId = categoria.Id;
            CategoriaInsumo = categoria;
#pragma warning disable CS0618 // alias legacy sincronizado hasta el DROP de columna
            Categoria = categoria.Nombre;
#pragma warning restore CS0618
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
