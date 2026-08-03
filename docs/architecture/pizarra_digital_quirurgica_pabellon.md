# Arquitectura de la Pizarra Digital Quirúrgica (Pabellón - Frontend)

## Visión General

La **Pizarra Digital Quirúrgica** del Sistema Sat Hospitalario proporciona un panel operativo unificado para la programación, trazabilidad y control de actos quirúrgicos en Pabellón. Está inspirada en paneles quirúrgicos digitales de alta densidad informativa, ofreciendo visualización semanal/horaria, detalle dinámico de observaciones y gestión de insumos consumidos.

## Estructura de Componentes y Vistas

```
src/app/features/admision/pabellon-gestion/
├── pabellon-gestion.component.ts        # Componente principal de la Pizarra Digital Quirúrgica
├── pabellon-gestion.component.spec.ts   # Pruebas unitarias completas en TDD
└── gestion-consumo-modal.component.ts   # Modal para consumos, auditoría y devoluciones de insumos
```

### Modos de Vista (`vistaModo`)

1. **Pizarra Horaria (`'pizarra'`) [Predeterminado]**:
   - Matriz bidimensional: Días de la semana (Lunes a Domingo) en columnas vs. Franjas horarias (07:00 a 18:00) en filas.
   - Tarjetas quirúrgicas codificadas por color de estado (Programado, En Proceso, Completado, Cancelado).
   - Muestra de forma destacada el **Doctor Cirujano**, **Item/Nombre de la Cirugía**, **Especialidad** y **Paciente**.

2. **Tarjetas (`'calendario'`)**:
   - Cuadrícula adaptable de tarjetas detalladas por orden de cirugía.

3. **Lista (`'lista'`)**:
   - Tabla comparativa tabular para búsquedas de alta densidad.

### Panel Lateral de Observaciones y Detalle (`ordenSeleccionada`)

Al hacer clic sobre cualquier cirugía en la Pizarra horaria, tarjetas o lista, se activa de forma síncrona el panel lateral derecho conteniendo:
- **Item de la Cirugía**: Descripción o nombre de la cirugía agendada.
- **Cuándo fue Programada**: Fecha completa y hora programada del procedimiento.
- **Razón / Motivo de la Cirugía**: Diagnóstico clínico o indicación justificante (`razonCirugia`).
- **Médico Cirujano**: Nombre del especialista responsable (`medicoNombre`).
- **Paciente**: Nombre completo y número de cédula.
- **Requisitos Quirúrgicos & Notas**: Protocolo de preparación e instrumental.
- **Transición de Estados & Consumos**: Botones directos para Iniciar, Completar, Cancelar o Gestionar Insumos.

## Modelo de Datos Frontend (`OrdenCirugia`)

```typescript
export interface OrdenCirugia {
  id: string;
  cuentaServicioId: string;
  pacienteId: string;
  pacienteNombre: string;
  pacienteCedula: string;
  descripcionCirugia: string;
  precioBaseUsd: number;
  medicoId: string;
  medicoNombre: string;
  fechaHoraProgramada: string;
  estado: string; // Programado, PendienteEjecucion, EnProceso, Completada, Cancelada
  motivoCancelacion?: string;
  fechaCreacion: string;
  usuarioCreacion: string;
  razonCirugia?: string;
  especialidad?: string;
  notasOperatorias?: string;
  requisitosQuirurgicos?: string;
}
```

## Pruebas E2E Directas a Base de Datos (`e2e-db-tests`)

Las pruebas E2E directas a la base de datos se encuentran en la carpeta ignorada por Git (`/e2e-db-tests`):
- **Archivo**: `e2e-db-tests/pabellon-quirurgico.spec.ts`
- **Comportamiento**:
  1. Realiza consultas SQL directas a la base de datos MySQL `SatHospitalario` (`OrdenesCirugia`, `Medicos`, `Pacientes`) a través de `queryDb()`.
  2. Verifica que las cirugías reales registradas en la BD se rendericen dinámicamente en la **Pizarra Digital Quirúrgica**.
  3. No utiliza datos mockeados ni valores hardcodeados en el componente.
  4. Valida el clic en la tarjeta de la Pizarra y la renderización de la cirugía dentro del Panel Lateral derecho ("Observaciones & Detalle Quirúrgico").

## Estándares de UI/UX Aplicados

- **Lucide Icons**: Iconos `stethoscope`, `grid`, `calendar`, `search`, `clock`, `user-check`, `file-text`, `heart-pulse` alineados mediante posicionamiento absoluto (`absolute left-3 top-1/2 -translate-y-1/2`) con padding horizontal `pl-9` para evitar cualquier colisión visual.
- **Glassmorphism Dark UI**: Paleta `bg-gray-950`, `bg-gray-900/70`, bordes sutiles `border-gray-800` y acentos neón por estado (Amber, Cyan, Emerald, Red).
- **Signals y OnPush**: Reactividad pura con `signal` y `computed` sin mutación directa de estado ni BehaviorSubjects locales.
