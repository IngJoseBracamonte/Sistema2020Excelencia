using System;
using System.Collections.Generic;
using System.Linq;
using SistemaSatHospitalario.Core.Domain.Constants;
using SistemaSatHospitalario.Core.Domain.Enums;

namespace SistemaSatHospitalario.Core.Domain.Entities.Admision
{
    public class Insumo
    {
        public Guid Id { get; private set; }
        public string Codigo { get; private set; } = string.Empty;
        public string Nombre { get; private set; } = string.Empty;

        // Gestión de Existencias por Sede (1:N)
        public virtual ICollection<StockSede> StocksPorSede { get; private set; } = new List<StockSede>();
        
        // ✅ Propiedad calculada dinámica (Ignorada en persistencia de EF Core)
        public decimal StockActual => StocksPorSede?.Sum(s => s.StockActual) ?? 0m;

        // Catálogo de Unidades de Medida (3FN)
        public int UnidadMedidaId { get; private set; }
        public virtual UnidadMedidaCatalogo UnidadMedidaNav { get; private set; } = null!;
        public string? UnidadMedidaBase { get; private set; } // Alias legacy opcional

        public decimal CostoUnitarioBaseUSD { get; private set; }
        public bool PermiteFraccionamiento { get; private set; }

        public Guid? CategoriaInsumoId { get; private set; }
        public virtual CategoriaInsumo? CategoriaInsumo { get; private set; }

        // Borrado Lógico (Soft Delete) y Visibilidad
        public bool IsDeleted { get; private set; }
        public DateTime? FechaInactivacion { get; private set; }
        public bool OcultoEnTraslados { get; private set; }

        // Relación N:M con Principios Activos
        public virtual ICollection<InsumoPrincipioActivo> PrincipiosActivos { get; private set; } = new List<InsumoPrincipioActivo>();

        // Datos complementarios
        public string? ReactivosCombinados { get; private set; }
        public string? Indicaciones { get; private set; }
        public DateTime? FechaVencimiento { get; private set; }

        protected Insumo() { }

        public Insumo(
            string codigo, 
            string nombre, 
            decimal stockActual, 
            UnidadMedidaEnum unidadMedidaBase, 
            decimal costoUnitarioBaseUSD, 
            bool permiteFraccionamiento = true)
        {
            Id = Guid.NewGuid();
            Codigo = codigo ?? throw new ArgumentNullException(nameof(codigo));
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            UnidadMedidaId = UnidadMedidaConstants.FromEnum(unidadMedidaBase);
            UnidadMedidaBase = unidadMedidaBase.ToString();
            CostoUnitarioBaseUSD = costoUnitarioBaseUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
            OcultoEnTraslados = false;
            IsDeleted = false;
            FechaInactivacion = null;

            var stockInicialValido = stockActual < 0 ? 0 : stockActual;
            StocksPorSede.Add(new StockSede(Id, SeedConstants.SedeId_Principal, stockInicialValido));
        }

        public void ActualizarDetalles(string nombre, decimal costoUSD)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            CostoUnitarioBaseUSD = costoUSD;
        }

        public void ActualizarDetalles(string nombre, UnidadMedidaEnum unidadMedidaBase, decimal costoUSD, bool permiteFraccionamiento)
        {
            Nombre = nombre ?? throw new ArgumentNullException(nameof(nombre));
            UnidadMedidaId = UnidadMedidaConstants.FromEnum(unidadMedidaBase);
            UnidadMedidaBase = unidadMedidaBase.ToString();
            CostoUnitarioBaseUSD = costoUSD;
            PermiteFraccionamiento = permiteFraccionamiento;
        }

        public void AsignarCategoria(CategoriaInsumo categoria)
        {
            ArgumentNullException.ThrowIfNull(categoria);
            CategoriaInsumoId = categoria.Id;
            CategoriaInsumo = categoria;
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
            if (ocultar) SoftDelete();
            else Restaurar();
        }

        public void AgregarPrincipioActivo(PrincipioActivo principioActivo, string concentracion)
        {
            ArgumentNullException.ThrowIfNull(principioActivo);

            if (PrincipiosActivos.Any(p => p.PrincipioActivoId == principioActivo.Id))
            {
                throw new InvalidOperationException($"El principio activo '{principioActivo.Nombre}' ya está asociado a este insumo.");
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