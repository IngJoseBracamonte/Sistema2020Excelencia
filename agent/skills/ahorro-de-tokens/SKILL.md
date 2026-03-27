---
name: ahorro-de-tokens
description: Protocolo de eficiencia extrema para minimizar el consumo de tokens sin perder calidad. Parametriza el comportamiento del agente según el modelo activo (Claude Opus/Sonnet, Gemini Flash/Pro) y delega a skills especializados en el momento exacto.
---

# Ahorro de Tokens (v1.0)

## Cuándo usar este skill
- **SIEMPRE**. Este skill es un meta-protocolo que debe activarse al inicio de cada sesión o cambio de modelo.
- Cuando el usuario lo invoque explícitamente o mencione "ahorro de tokens", "optimizar tokens", "ser breve".
- Implícitamente al detectar que el contexto del proyecto ya existe en `agent/docs/`.

## Filosofía Central
> **El token más barato es el que no se gasta.**
> No re-explicar lo que ya está documentado. No narrar el proceso. Solo documentar el resultado.

---

## Protocolo de Arranque (Obligatorio)

### Paso 0: Identificar Modelo Activo
Detectar automáticamente el modelo y aplicar el perfil correspondiente:

| Modelo | Perfil | Archivos Prioritarios | Verbosidad |
|---|---|---|---|
| **Claude Opus 4.6** | Deep/Arquitectural | `Architecture.md`, `DataFlow.md`, `Rules.md` | Mínima en proceso, completa en entrega |
| **Claude Sonnet 4.6** | Balanced/Execution | `Rules.md`, `StepJournal.md`, `Parameters.md` | Mínima siempre |
| **Gemini Pro 3.1 High** | Deep/Arquitectural | `Architecture.md`, `DataFlow.md`, `Rules.md` | Mínima en proceso, completa en entrega |
| **Gemini Pro 3.1 Low** | Iterative/Task | `Checks.md`, `StepJournal.md`, `Parameters.md` | Cero narrativa, solo código y diff |
| **Gemini Flash** | Speed/Strategic | `Rules.md`, `Parameters.md`, `Brainstorming`, `Planificacion-Pro` | Cero. Solo output. |

### Paso 1: Memoria antes que exploración
**ANTES de usar `list_dir`, `grep_search` o `view_file` en el código fuente:**
1. Leer los archivos de `agent/docs/` del perfil asignado (máximo 2-3 archivos).
2. Si la respuesta está en la memoria → actuar directamente.
3. Si no → explorar el código con la mínima cantidad de llamadas necesarias.

### Paso 2: Zero-Narration Mode
**Durante la ejecución (mientras se trabaja):**
- ❌ NO explicar qué se va a hacer antes de hacerlo.
- ❌ NO narrar "Ahora voy a buscar...", "Voy a verificar...", "El siguiente paso es...".
- ❌ NO resumir archivos que se acaban de leer (ya los tienes en contexto).
- ❌ NO repetir el contenido de un archivo al usuario.
- ✅ Ejecutar herramientas en paralelo cuando no haya dependencias.
- ✅ Usar `view_file` con rangos precisos (no archivos completos si solo necesitas 20 líneas).

**Al entregar resultado final:**
- ✅ Explicar QUÉ se hizo y POR QUÉ (causa raíz, decisión de diseño).
- ✅ Documentar en `StepJournal.md`.
- ✅ Usar tablas y diffs, no prosa.

---

## Reglas de Delegación a Skills

### Backend (.cs, .csproj, EF Core, CQRS)
→ Usar skill: `experto-en-desarrollo-profesional`
→ Leer: `Rules.md` (Leyes del proyecto), `Architecture.md` (patrones CQRS)
→ Regla crítica: **No regenerar migraciones** (Ley #3)

### Frontend (Angular, .ts, .html, .css)
→ Usar skills: `experto-angular` + `disenador-ui-ux`
→ Leer: `Rules.md` § UI/UX ("Rose on Slate" Premium)
→ **LEY VISUAL ABSOLUTA**: No modificar tema, colores, tipografía, border-radius ni transiciones establecidas a menos que el usuario lo solicite explícitamente. Todo nuevo elemento debe copiar los patrones de clase existentes en el mismo archivo.

### Infraestructura (Aspire, Docker, OTEL)
→ Usar skill: `aspire-pro`
→ Leer: `Parameters.md` (connection strings, endpoints)

### Planificación / Tareas complejas
→ Usar skill: `planificacion-pro`
→ Solo si la tarea requiere >3 archivos y >2 capas de la arquitectura.

### Cambio de modelo
→ Usar skill: `cambio-de-agente`
→ Después de cambiar, re-ejecutar este skill (`ahorro-de-tokens`) con el nuevo perfil.

---

## Patrones de Reducción de Tokens

### 1. Lecturas Inteligentes
```
MAL  → view_file(archivo_completo_de_800_lineas)
BIEN → grep_search("patrón_exacto") → view_file(StartLine, EndLine)
```

### 2. Ediciones Quirúrgicas
```
MAL  → replace_file_content con 200 líneas cuando solo cambian 5
BIEN → replace_file_content con TargetContent exacto de las líneas afectadas
MAL  → Proponer reescribir un archivo completo
BIEN → multi_replace_file_content con chunks no contiguos
```

### 3. Herramientas en Paralelo
```
MAL  → Leer archivo A, esperar, leer archivo B, esperar, leer archivo C
BIEN → Leer A, B, C en una sola ronda (sin waitForPreviousTools)
```

### 4. Resumen de Entrega
```
MAL  → 3 párrafos explicando lo que el diff ya muestra
BIEN → Tabla: | Archivo | Cambio | Razón |
       Luego una línea de contexto si es no-obvio.
```

### 5. Journal Atómico
Actualizar `StepJournal.md` con formato compacto:
```markdown
### N. [Verbo]: [Descripción corta]
- **Archivo**: `ruta/al/archivo.ext`
- **Cambio**: Qué se hizo en 1 línea.
- **Razón**: Por qué, en 1 línea.
```

---

## Métricas de Éxito
El skill se considera exitoso cuando:
- [ ] Primera herramienta ejecutada en <3 tool calls desde el inicio.
- [ ] Zero explicaciones intermedias durante el proceso de ejecución.
- [ ] Entrega final usa tablas/diffs en lugar de prosa.
- [ ] `StepJournal.md` actualizado con formato compacto.
- [ ] No se leyó ningún archivo completo que pudiera haberse resuelto con `grep_search`.
- [ ] Se respetó el tema visual existente (si aplica frontend).

---

## Anti-Patrones (Prohibidos)

| Anti-Patrón | Costo Estimado | Alternativa |
|---|---|---|
| Leer `facturacion.component.html` completo (800 líneas) | ~2000 tokens | `grep_search` + `view_file` con rango |
| Explicar qué hará antes de hacerlo | ~300 tokens/vez | Ejecutar directamente |
| Resumir un archivo recién leído al usuario | ~500 tokens | El usuario no lo pidió; actuar |
| Re-analizar arquitectura que ya está en `Architecture.md` | ~1000 tokens | Leer el .md de 50 líneas |
| Proponer plan cuando la tarea es trivial (<3 archivos) | ~800 tokens | Ejecutar directamente |
| Explicar cada grep/view_file antes de llamarla | ~150 tokens/vez | Usar toolAction/toolSummary |
