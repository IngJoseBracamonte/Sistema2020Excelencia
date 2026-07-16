# Fase 3: Desarrollo del Backend (.NET)

En esta fase se implementa la lógica real en C# y EF Core para resolver el modelo de datos, persistencia en base de datos, endpoints del controlador, y las reglas de descuento de inventario, logrando que todas las pruebas unitarias creadas en la Fase 1 pasen a estado exitoso (*green stage*).

---

## Modificaciones de Dominio e Infraestructura

### 1. Entidad Insumo (`Insumo.cs`)
- **Modificación**: Agregar propiedades y métodos de cambio de estado.
- **Campos**:
  - `public string? ReactivosCombinados { get; private set; }`
  - `public string? Indicaciones { get; private set; }`
  - `public DateTime? FechaVencimiento { get; private set; }`
  - `public bool OcultoEnTraslados { get; private set; }`
- **Métodos**:
  - `ActualizarDetalles(..., string? reactivos, string? indicaciones, DateTime? vencimiento)`: Para guardar los nuevos campos.
  - `AlternarOcultoEnTraslados(bool ocultar)`: Modifica el flag `OcultoEnTraslados`.

### 2. Configuración de Base de Datos (`SatHospitalarioDbContext.cs` & SQL Script)
- Configurar en `OnModelCreating`:
  ```csharp
  builder.Entity<Insumo>(entity => {
      entity.Property(e => e.OcultoEnTraslados).HasDefaultValue(false);
  });
  ```
- Crear e invocar script `add_pharmacy_fields.sql`:
  ```sql
  ALTER TABLE SatHospitalario.Insumos 
  ADD COLUMN ReactivosCombinados VARCHAR(500) NULL,
  ADD COLUMN Indicaciones TEXT NULL,
  ADD COLUMN FechaVencimiento DATETIME NULL,
  ADD COLUMN OcultoEnTraslados TINYINT(1) NOT NULL DEFAULT 0;
  ```

---

## Controladores y Casos de Uso (API Endpoints)

### 1. Actualización de Catálogo (`GetInsumos`)
- Modificar el endpoint de consulta en `InventoryController.cs`:
  - Recibe `bool? excludeHidden` y `string? search`.
  - Filtro por borrado lógico: si `excludeHidden == true`, aplica `.Where(i => !i.OcultoEnTraslados)`.
  - Filtro por búsqueda: si `search` no está vacío, busca coincidencias parciales (`EF.Functions.Like` o `.Contains`) en:
    - `i.Nombre`
    - `i.Codigo`
    - `i.ReactivosCombinados`

### 2. Registro de Compras (`RecordPurchase`)
- **Endpoint**: `[HttpPost("compras")]`
- **DTO de Entrada**:
  ```csharp
  public class RecordPurchaseDto {
      public Guid SedeId { get; set; }
      public List<PurchaseItemDto> Items { get; set; } = new();
  }
  public class PurchaseItemDto {
      public Guid InsumoId { get; set; }
      public decimal Cantidad { get; set; }
      public decimal PrecioCostoUSD { get; set; }
      public DateTime? FechaVencimiento { get; set; }
  }
  ```
- **Lógica**:
  1. Para cada ítem, obtener la entidad `Insumo` de la base de datos.
  2. Actualizar el costo base `CostoUnitarioBaseUSD = PrecioCostoUSD` y la fecha de vencimiento.
  3. Incrementar el stock en `StockSedes` para la sede seleccionada (usando `RegistrarMovimientoStock`).
  4. Registrar un `MovimientoInsumo` de tipo `"Ingreso"` y motivo `"Compra registrada en Farmacia"`.

### 3. Borrado Lógico y Restauración
- **Borrado Lógico**: `[HttpDelete("insumos/{id}")]` -> Llama a `AlternarOcultoEnTraslados(true)`.
- **Restaurar Insumo**: `[HttpPost("insumos/{id}/restaurar")]` -> Llama a `AlternarOcultoEnTraslados(false)`.

---

## Lógica de Descuento Multisede (`InventoryService.cs`)
- **Modificación**: En `DeductInventoryForServiceDetailAsync`, flexibilizar la validación de sede.
- **Lógica actual**: Limitada estrictamente a 4 constantes de sede.
- **Ajuste**: Utilizar la sede activa resuelta a partir de la cuenta administrativa o área clínica:
  ```csharp
  Guid stockDeductionSedeId = targetSedeId ?? SeedConstants.SedeId_Principal;
  ```
  Esto permite que si el paciente es atendido en una nueva "Sede Hospitalaria", el descuento de stock se aplique de forma exacta y directa a esa sede física.

---

## Entregables de la Fase
- Código fuente del backend (.NET Core) modificado y compilado.
- Scripts de actualización de base de datos aplicados en la instancia local de MySQL.
- Ejecución exitosa de `dotnet test` para asegurar el cumplimiento del 100% de los tests unitarios.
