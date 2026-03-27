# ✅ Protocolo de Verificación y QA (Checks.md)

Checklist avanzado para auditoría y validación de cambios en el Sistema Sat Hospitalario.

## 🔌 Nivel 1: Infraestructura y Orquestación
- [ ] **AppHost**: ¿El proyecto levanta sin errores de puerto?
- [ ] **Service Discovery**: ¿La API es alcanzable bajo el nombre `api`?
- [ ] **Env Vars**: Confirmar que `mysql-system` y `mysql-identity` no son nulos.

## 💻 Nivel 2: Backend y Dominio
- [ ] **Compilación**: Cero (0) errores en `SistemaSatHospitalario.WebAPI`.
- [ ] **Telemetry**: ¿Se ven los Traces del `SyncCarritoCommandHandler` en el Dashboard de Aspire?
- [ ] **Queries**: ¿Se ven los comandos SQL en los Traces de Aspire?
- [ ] **Auth**: Validar que el token JWT se genera con las claims correctas.

## ⚖️ Nivel 3: Lógica de Negocio (V11.7)
- [ ] **Admission**: ¿Se crea una cuenta nueva cada vez que se envía el carrito? (MD-001)
- [ ] **Balance**: ¿Se crea una `CuentaPorCobrar` si el abono es menor al total? (MD-002)
- [ ] **Onboarding**: ¿Se migra el paciente de legacy a nativo si no existe el GUID? (MD-003)
- [ ] **Fiscal Null**: ¿El `NroControlFiscal` permite nulos en base de datos? (Indispensable para Borradores).
- [ ] **Ghost Inserts**: ¿Se usan consultas `NoTracking` para datos maestros en comandos?
- [ ] **Dual Write**: Al crear un paciente nuevo, ¿aparece en ambas bases de datos MySQL?

## 🎨 Nivel 4: Frontend y UI/UX
- [ ] **Diseño Rose**: Validar que el color Rose `--primary` solo se usa en interactivos.
- [ ] **Glassmorphism**: ¿Los paneles tienen `backdrop-filter`?
- [ ] **Nulidad**: Verificar que `?.toLowerCase()` está en el filtro de servicios de admisión.
- [ ] **Performance**: Verificar que no hay memory leaks en el `TelemetryService`.

## 📜 Nivel 5: Memoria de Arquitectura
- [ ] **StepJournal.md**: ¿Se registró el fin del ciclo de reingeniería?
- [ ] **Parameters.md**: ¿Se agregaron los nuevos campos de comando?
- [ ] **Rules.md**: ¿Se documentó el principio de atomicidad?
