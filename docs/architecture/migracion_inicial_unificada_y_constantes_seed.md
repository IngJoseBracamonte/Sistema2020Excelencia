# Migración Inicial Unificada y Constantes de Semilla (SeedConstants)

## 1. Contexto y Justificación Arquitectónica
Para consolidar la base de datos moderna del **Sistema Sat Hospitalario (v4.0.9)** y eliminar la fragmentación producida por múltiples migraciones incrementales obsoletas, se realizó un proceso de saneamiento estructural integral:
1. **Limpieza de Migraciones**: Eliminación de todas las migraciones intermedias en `Persistence/Migrations`, `Identity/Migrations` y `Persistence/Legacy/Migrations`.
2. **Migración Inicial Limpia**: Generación de una única migración canónica (`InitialApplication`, `InitialIdentity` e `InitialLegacy`) mediante Entity Framework Core 10 / .NET 9.
3. **Persistencia Directa de Constantes de Dominio (`HasData`)**: Inclusión de los catálogos y constantes maestras directamente en el modelo de base de datos para garantizar coherencia en despliegues nuevos y entornos de prueba.

---

## 2. Mapa de Constantes Inmutables de Dominio (`SeedConstants`)

### A. Sedes Hospitalarias (`Sedes`)
| Guid / Id | Código | Nombre | EsPrincipal |
| :--- | :--- | :--- | :--- |
| `10000000-0000-0000-0000-000000000001` | `SEDE-PRINCIPAL` | Almacén Principal / Farmacia Central | `true` |
| `10000000-0000-0000-0000-000000000002` | `SEDE-EMG` | Depósito Emergencia | `false` |
| `10000000-0000-0000-0000-000000000003` | `SEDE-HOSP` | Depósito Hospitalización | `false` |
| `10000000-0000-0000-0000-000000000004` | `SEDE-UCI` | Depósito UCI | `false` |
| `10000000-0000-0000-0000-000000000005` | `SEDE-CIRUGIA` | Quirófano / Pabellón Central | `false` |

### B. Áreas Clínicas Base (`AreasClinicas`)
| Guid / Id | Sede Padre | Código | Nombre | EsAreaAdmision |
| :--- | :--- | :--- | :--- | :--- |
| `30000000-0000-0000-0000-000000000001` | Emergencia | `BOX-1` | Box Emergencia 1 | `true` |
| `30000000-0000-0000-0000-000000000002` | Hospitalización | `HAB-101` | Habitación 101 | `false` |
| `30000000-0000-0000-0000-000000000003` | UCI | `UCI-1` | Cama UCI 1 | `false` |
| `30000000-0000-0000-0000-000000000004` | Sede Principal | `FARMACIA` | Farmacia Central | `false` |
| `30000000-0000-0000-0000-000000000005` | Sede Principal | `LABORATORIO` | Laboratorio Central | `false` |
| `30000000-0000-0000-0000-000000000006` | Sede Cirugía | `QX-1` | Quirófano 1 (Cirugía Mayor) | `false` |

### C. Categorías de Insumos (`CategoriasInsumo`)
| Guid / Id | Nombre | Código |
| :--- | :--- | :--- |
| `50000000-0000-0000-0000-000000000001` | Medicamento | `MED` |
| `50000000-0000-0000-0000-000000000002` | Descartable | `DESC` |
| `50000000-0000-0000-0000-000000000003` | Material Médico | `MAT-MED` |
| `50000000-0000-0000-0000-000000000004` | Reactivo | `REACT` |
| `50000000-0000-0000-0000-000000000005` | Material Quirúrgico | `MAT-QX` |
| `50000000-0000-0000-0000-000000000006` | Otro | `OTRO` |

### D. Requisitos Quirúrgicos de Checklist (`RequisitosCirugia`)
| Guid / Id | Nombre | Descripción |
| :--- | :--- | :--- |
| `40000000-0000-0000-0000-000000000001` | Evaluación Cardiovascular / Riesgo Quirúrgico | Informe de cardiología y electrocardiograma vigente. |
| `40000000-0000-0000-0000-000000000002` | Exámenes Preoperatorios (Laboratorio) | Hematología completa, TP, TPT, Glucemia, Urea, Creatinina y VIH/VDRL. |
| `40000000-0000-0000-0000-000000000003` | Consentimiento Informado Firmado | Firma del paciente o familiar responsable para procedimiento quirúrgico y anestesia. |
| `40000000-0000-0000-0000-000000000004` | Ayuno Verificado (Mínimo 8 Horas) | Verificación por enfermería de ayuno estricto. |
| `40000000-0000-0000-0000-000000000005` | Valoración Anestésica | Aprobación formal firmada por el médico anestesiólogo. |
| `40000000-0000-0000-0000-000000000006` | Reserva de Sangre / Hemoderivados | Disponibilidad confirmada con Banco de Sangre (cuando aplique). |
| `40000000-0000-0000-0000-000000000007` | Disponibilidad de Cama Postoperatoria (UCI / Hosp) | Cama confirmada para el traslado post-quirúrgico. |

### E. Tipos de Servicio (`TiposServicio`)
| Id (INT) | Nombre | Código |
| :--- | :--- | :--- |
| 1 | Servicio Médico / Consulta | `MEDICO` |
| 2 | Examen de Laboratorio | `LAB` |
| 3 | Rayos X / Imagenología | `RX` |
| 4 | Tomografía Axial | `TOMO` |
| 5 | Insumo / Medicamento | `INSUMO` |
| 6 | Informe / Lectura Médica | `INFORME` |

### F. Monedas Oficiales (`Monedas`)
| Id (INT) | Código | Nombre | Símbolo | EsPrincipal |
| :--- | :--- | :--- | :--- | :--- |
| 1 | USD | Dólar | `$` | `true` |
| 2 | VES | Bolívar | `Bs.` | `false` |
| 3 | EUR | Euro | `€` | `false` |
| 4 | COP | Peso Colombiano | `COP$` | `false` |
| 5 | ARS | Peso Argentino | `ARS$` | `false` |

---

## 3. Garantía de Integridad y Auto-Sanación
- **EF Core HasData**: Las constantes quedan registradas en el snapshot y en los `migrationBuilder.InsertData(...)` de `InitialApplication`.
- **SystemDbInitializer**: Durante el arranque del backend, los métodos `SeedCategoriasInsumoAsync()`, `SeedRequisitosCirugiaAsync()`, `SeedInventorySedesAndMigrateStockAsync()`, `SeedAreasClinicasAsync()`, etc., garantizan la inserción y mantenimiento seguro de estos registros sin generar duplicados.
- **TDD / Pruebas Unitarias**: La clase de prueba `SeedConstantsTests` valida la inmutabilidad y presencia de todos los GUIDs de constantes.
