# Arquitectura: Módulo de Traslado Inteligente, Altas Médicas y Control de Solvencia v2.0

## Contexto y Visión General
El **Sistema Sat Hospitalario v2.0** requiere la estandarización del control de traslados intrahospitalarios (cambio de cama en misma área y traslado inter-área), altas médicas clasificadas (Normal, Voluntaria, Defunción), y la validación estricta de solvencia financiera por parte del personal de enfermería.

---

## 1. Reglas de Dominio y Solvencia Financiera
- **Cálculo de Solvencia**:
  - `totalCuenta = cuenta.CalcularTotal()`
  - `totalPagado = SUM(recibo.MontoTotalUSD)` para recibos activos (no anulados).
  - `saldoPendiente = totalCuenta - totalPagado`.
- **Alertas y Control de Enfermería**:
  - Si `saldoPendiente <= 0`: Paciente **SOLVENTE**. Se autorizan traslados y altas de inmediato.
  - Si `saldoPendiente > 0`: Paciente **INSOLVENTE**. Intentar un traslado o alta médica debe activar un modal de confirmación en el frontend de Enfermería.
  - Si la confirmación de enfermería no es otorgada (`ConfirmadoPorEnfermeriaSinSolvencia == false`), el backend rebotará el comando disparando una `InvalidOperationException` de dominio.

---

## 2. Clasificación de Altas Médicas (`TipoAltaEnum`)
- `Normal` (0): Alta médica regular autorizada por el médico tratante.
- `Voluntaria` (1): Alta solicitada formalmente por el paciente o familiar responsable.
- `Defuncion` (2): Egreso del paciente por fallecimiento.

---

## 3. Auditoría Inmutable (Norma Tributaria SENIAT SNAT/2024/000102)
Para garantizar trazabilidad e inmutabilidad en la gestión de cuentas y traslados con saldos pendientes o ajustes tarifarios:
- Entidad `AuditLog` en la base de datos moderna (`SatHospitalarioDbContext`).
- Campos auditados: `UserId`, `ActionType`, `OldValue`, `NewValue`, `IpAddress`, `Timestamp`.
- Las acciones registradas incluyen `ALTA_MEDICA`, `TRASLADO_AREA`, y `MODIFICACION_TARIFA`.

---

## 4. Estándares de UI (Angular 18+ Signals)
- Selector de modalidad de traslados: Pestaña renombrada estrictamente a `CAMBIO DE CAMA` (revolviendo cualquier indicación de precio previo como `$0 USD`).
- Cabecera de Enfermería: Botón desplegable `"🚪 DAR DE ALTA"` con 3 opciones explícitas:
  - 🟢 Alta Normal
  - 🟠 Alta Voluntaria
  - 🔴 Alta Por Defunción
- **UX & Persistencia de Desplegable**:
  - Para evitar cierres indeseados al desplazar el cursor, el contenedor implementa un puente de hover continuo (`top-full pt-2` con `before:content-[''] before:-top-2`).
  - Soporte de toggle por clic (`isAltaDropdownOpen` signal + `@HostListener` click-outside) para permitir interacción sin requerir precisión milimétrica de puntero.
- **Indicador de Solvencia en Cabecera**:
  - Ubicación: Inmediatamente después de `Seguro: PARTICULAR` con separador de viñeta (`•`).
  - Estado `Solvente` (`saldoPendiente <= 0`): Icono `CheckCircle` + texto **Solvente** en verde esmeralda (`text-emerald-400`).
  - Estado `Pendiente` (`saldoPendiente > 0`): Icono `AlertTriangle` + texto **Pendiente** en amarillo/ámbar (`text-amber-400`).
- Modal Interyector: Mensaje estandarizado: `"El paciente registra un saldo pendiente de $X USD. ¿Desea continuar de todos modos?"` con botones `[No, Cancelar]` / `[Sí, Continuar]`.
