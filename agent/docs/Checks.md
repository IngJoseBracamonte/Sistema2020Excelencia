# ✅ Protocolo de Verificación y QA (Checks.md)

Checklist avanzado para auditoría y validación de cambios en el Sistema Sat Hospitalario.

## 🔌 Nivel 1: Infraestructura y Orquestación
- [ ] **AppHost**: ¿El proyecto levanta sin errores de puerto?
- [ ] **Service Discovery**: ¿La API es alcanzable bajo el nombre `api`?
- [ ] **Env Vars**: Confirmar que `mysql-system` y `mysql-identity` no son nulos.

## 💻 Nivel 2: Backend y Dominio
- [ ] **Compilación**: Cero (0) errores en `SistemaSatHospitalario.WebAPI`.
- [ ] **Telemetry**: ¿Aparece el log `[OTEL DEBUG]` en la consola de la API?
- [ ] **Queries**: ¿Se ven los comandos SQL en los Traces de Aspire?
- [ ] **Auth**: Validar que el token JWT se genera con las claims correctas.

## 🎨 Nivel 3: Frontend y UI/UX
- [ ] **Diseño Rose**: Validar que el color Rose `--primary` solo se usa en interactivos.
- [ ] **Glassmorphism**: ¿Los paneles tienen `backdrop-filter`?
- [ ] **Nulidad**: Verificar que `?.toLowerCase()` está en el filtro de servicios de admisión.
- [ ] **Performance**: Verificar que no hay memory leaks en el `TelemetryService`.

## 📜 Nivel 4: Memoria de Arquitectura
- [ ] **StepJournal.md**: ¿Se registró la última acción exitosa?
- [ ] **Parameters.md**: ¿Se agregaron nuevos parámetros si los hubo?
- [ ] **Rules.md**: ¿Se documentó alguna nueva ley o patrón descubierto?
