# Arquitectura del Módulo de Edición de Catálogo (Catalog Edit Modals)

## Contexto y Estándares de Diseño (Angular 19)

El módulo de catálogo administrativo del Sistema Sat Hospitalario gestiona 7 tipos principales de servicios clínicos y catálogo:
1. **SERVICIO** (`edit-servicio.component` - Catálogo base y fallback por defecto)
2. **MEDICAMENTO** (`edit-medicamento.component`)
3. **LABORATORIO** (`edit-laboratorio.component`)
4. **CIRUGIA** (`edit-cirugia.component`)
5. **CONSULTA** (`edit-consulta.component`)
6. **PROCEDIMIENTO** (`edit-procedimiento.component`)
7. **TOMOGRAFIA** (`edit-tomografia.component`)

### Principios SOLID & DRY Aplicados

```
src/app/features/admin/catalog/
├── models/
│   └── catalog-edit.models.ts         # Contratos e interfaces (BOMLine, HonorarioMedico, EquipoQuirurgico)
├── handlers/
│   ├── catalog-bom.handler.ts         # Encapsulamiento reactive de listas de insumos (BOM) y búsqueda
│   ├── catalog-honorarios.handler.ts  # Encapsulamiento reactive de honorarios médicos por servicio
│   ├── catalog-sugerencias.handler.ts # Encapsulamiento reactive de ítems vinculados / sugerencias
│   └── catalog-handlers.spec.ts       # Pruebas unitarias de handlers aislados
└── components/
    ├── base-catalog-edit.component.ts # Clase abstracta base con Signals (itemId, isEditing, saved, closed)
    ├── edit-servicio.component.ts / .spec.ts
    ├── edit-medicamento.component.ts / .spec.ts
    ├── edit-laboratorio.component.ts / .spec.ts
    ├── edit-cirugia.component.ts
    ├── edit-consulta.component.ts
    ├── edit-procedimiento.component.ts
    └── edit-tomografia.component.ts
```

### Principios Clave
- **Single Responsibility (SRP)**: La manipulación de datos repetitivos (BOM, Honorarios Médicos, Sugerencias) no reside dentro del componente UI, sino en handlers especializados reactivos basados en Signals de Angular 19 (`signal()`, `computed()`).
- **Open/Closed (OCP)**: La infraestructura común (efectos de reacción a `itemId`, reset de formulario, señales base `nombre`, `codigo`, `precioBaseUsd`, `activo`, `isSaving`) se encuentra en `BaseCatalogEditComponent`. Extender nuevos tipos de servicio no requiere reescribir lógica modal.
- **Iconos Limpios**: Se eliminaron importaciones inválidas de paquetes externos (e.g. `Scalpel` en `lucide-angular`).
- **Compatibilidad 100% UI**: Todos los bindings del DOM (`[(ngModel)]`, `(click)`) y selectores (`app-edit-*`) se mantienen inalterados.
