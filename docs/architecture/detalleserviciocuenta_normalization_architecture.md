# Arquitectura: Normalización (1FN, 2FN, 3FN) y Refactorización SOLID de `DetalleServicioCuenta`

**Fecha**: 2026-08-12  
**Sistema**: Sistema Sat Hospitalario v4.0.7  
**Estándar Aplicado**: Experto en Desarrollo Profesional + Normalización Estricta DB (1FN, 2FN, 3FN) + SOLID

---

## 1. Análisis y Decisiones de Arquitectura

* **Tercera Forma Normal (3FN):**
  - Reemplazo de `TipoServicio` (string plano libre) por la llave foránea `TipoServicioId` vinculada a `TipoServicioCatalog`.
  - Reemplazo de `UsuarioCarga` (nombre de usuario string plano) por la FK `UsuarioId` vinculada a la tabla maestra de usuarios (`AspNetUsers`), asegurando la integridad referencial y eliminando dependencias transitivas.
* **Principio de Responsabilidad Única (SRP):**
  - El manejador de comandos `CargarServicioACuentaCommandHandler` coordina la validación, descuento transaccional de inventarios y creación de la entidad normalizada.
* **Inversión de Dependencias (DIP):**
  - Desacoplamiento total del DbContext a través de la interfaz `IApplicationDbContext` para la ejecución aislada de pruebas unitarias mediante *mocks* (Moq / NSubstitute).

---

## 2. Definición del Modelo de Dominio Normalizado

```csharp
public class DetalleServicioCuenta
{
    public Guid Id { get; private set; }
    public Guid CuentaServiciosId { get; private set; }
    public Guid ServicioId { get; private set; }
    
    // Normalización 3FN: Catalog FK y User FK
    public int TipoServicioId { get; private set; }
    public TipoServicioCatalog TipoServicio { get; private set; } = null!;

    public string UsuarioId { get; private set; } = string.Empty;

    public decimal PrecioUnitario { get; private set; }
    public decimal Cantidad { get; private set; }
    public decimal Total => PrecioUnitario * Cantidad;
    public DateTime FechaCarga { get; private set; }

    public static DetalleServicioCuenta Crear(
        Guid cuentaId, 
        Guid servicioId, 
        int tipoServicioId, 
        string usuarioId, 
        decimal precio, 
        decimal cantidad)
    {
        if (cantidad <= 0) throw new ArgumentException("La cantidad debe ser mayor a cero.");

        return new DetalleServicioCuenta
        {
            Id = Guid.NewGuid(),
            CuentaServiciosId = cuentaId,
            ServicioId = servicioId,
            TipoServicioId = tipoServicioId,
            UsuarioId = usuarioId,
            PrecioUnitario = precio,
            Cantidad = cantidad,
            FechaCarga = DateTime.UtcNow
        };
    }
}
```

---

## 3. Patrón CQRS & Manejo de Comandos

- **Command**: `CargarServicioACuentaCommand` encapsula únicamente los identificadores tipados y cantidades necesarias.
- **Handler**: `CargarServicioACuentaCommandHandler` ejecuta el flujo completo de validación de la cuenta clínica, descuento transaccional de stock mediante `IInventoryService`, la creación de `DetalleServicioCuenta` y la persistencia en `IApplicationDbContext`.

---

## 4. Estrategia de Pruebas Unitarias (TDD / AAA)

- Patrón **Arrange - Act - Assert**.
- Mocks para `IApplicationDbContext` y `IInventoryService`.
- Cobertura de casos positivos (carga válida de servicio) y casos limite (descuento insuficiente, cantidades <= 0, cuenta no encontrada).

---

## 5. Recomendaciones Finales de Producción

1. **Transaccionalidad (Unit of Work):** Ejecutar la actualización de inventario y la inserción del detalle en una transacción explícita (`IDbContextTransaction`) para prevenir inconsistencias en escenarios de fallo intermedio.
2. **Caché en Memoria:** Utilizar `IMemoryCache` para `TipoServicioCatalog` a fin de optimizar las lecturas frecuentes sin penalizar el rendimiento del motor MySQL.
