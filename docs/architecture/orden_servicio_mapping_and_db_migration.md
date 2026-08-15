# Memoria de Arquitectura: Corrección de Mapeo en OrdenDeServicio y Migración Delta de Base de Datos

## 1. Contexto y Diagnóstico del Problema

### 1.1 Incompatibilidad de Charset / Collation en Claves Foráneas (MySQL Error 3780)
Al intentar aplicar claves foráneas en la base de datos de producción (`sathospitalario`), MySQL rechazaba las restricciones con el código de error `3780 (Incompatible foreign key constraint)`. Esto se debió a que las tablas maestras (`areasclinicas`, `sedes`, `cuentasservicios`, `medicos`, etc.) fueron creadas con el juego de caracteres `char(36) CHARACTER SET ascii COLLATE ascii_general_ci`, mientras que nuevas columnas y tablas secundarias heredaban la collation por defecto de la base de datos (`utf8mb4 COLLATE utf8mb4_0900_ai_ci`).

### 1.2 Shadow Property en EF Core (`OrdenDeServicio.PacienteId1`)
En la configuración previa de `SatHospitalarioDbContext`, la relación entre `OrdenDeServicio` y `PacienteAdmision` se definía como:
```csharp
entity.HasOne<PacienteAdmision>()
      .WithMany(p => p.Ordenes)
      .HasForeignKey(o => o.PacienteId)
      .OnDelete(DeleteBehavior.Restrict);
```
Dado que la entidad `OrdenDeServicio` contenía la propiedad de navegación `public virtual PacienteAdmision? Paciente`, EF Core interpretaba `Paciente` como una segunda relación no mapeada y creaba una propiedad fantasma en "shadow state" denominada `PacienteId1`. Esto provocaba que en endpoints como `/api/Dashboard/Insights`, EF Core emitiera consultas SQL con `SELECT o.PacienteId1 FROM OrdenesDeServicio AS o`, fallando en MySQL con `Unknown column 'o.PacienteId1' in 'field list'`.

---

## 2. Solución Arquitectónica

### 2.1 Corrección de Mapeo Relacional en Fluent API
En `SatHospitalarioDbContext.cs`, se explicitó la propiedad de navegación `o.Paciente`:
```csharp
builder.Entity<OrdenDeServicio>(entity =>
{
    entity.ToTable("OrdenesDeServicio");
    entity.HasKey(o => o.Id);
    entity.Property(o => o.TotalCobrado).HasPrecision(18, 2);
    entity.Property(o => o.EstadoFacturacion).HasConversion<int>();

    entity.HasOne(o => o.Paciente)
          .WithMany(p => p.Ordenes)
          .HasForeignKey(o => o.PacienteId)
          .OnDelete(DeleteBehavior.Restrict);
});
```
Esto eliminó la propiedad fantasma `PacienteId1` en todo el ciclo de vida de EF Core y normalizó la proyección SQL.

### 2.2 Script de Migración Delta Idempotente
Se estructuró el script `Sat20260814_Delta_Update.sql` en 3 fases:
1. **Normalización de Tipado**: Conversión explícita de todas las columnas GUID/FK a `char(36) CHARACTER SET ascii COLLATE ascii_general_ci`.
2. **Creación de Tablas**: Creación de `auditlogs`, `cirugiasobservacioneshistorial`, `requisitoscirugia`, `ordenescirugiarequisitos` y tablas de ASP.NET Core Identity.
3. **Restricciones Relacionales (FKs)**: Creación de claves foráneas con `ON DELETE` acorde a las reglas de negocio hospitalarias.

---

## 3. Verificación y Calidad
- **Pruebas Unitarias**: 294 pruebas unitarias aprobadas (100% éxito) en la solución.
- **Validación SQL**: 0 columnas fantasma y 100% de compatibilidad en validadores automáticos.
