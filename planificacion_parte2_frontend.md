# PLANIFICACIÓN: Módulo Honorarios Pro — PARTE 2 (Frontend)

## PRERREQUISITO
Completar PARTE 1 (Backend) primero. Verificar que `dotnet build` pase sin errores.

---

## PASO 10: Registrar Rutas Angular

**Archivo:** `src/SistemaSatHospitalario.Frontend/src/app/app.routes.ts`

**Añadir dentro del array `children` (después de la ruta de `admin/reportes/calculo-honorarios`):**

```typescript
{ path: 'admin/honorarios/asignaciones', loadComponent: () => import('./features/admin/honorariums/honorario-asignaciones.component').then(m => m.HonorarioAsignacionesComponent) },
{ path: 'admin/honorarios/config', loadComponent: () => import('./features/admin/honorariums/honorario-config.component').then(m => m.HonorarioConfigComponent) },
```

---

## PASO 11: Añadir Links al Sidebar

**Archivo:** `src/SistemaSatHospitalario.Frontend/src/app/shared/components/sidebar/sidebar.component.html`

**Dentro del dropdown "Gestión Médica" (después de la línea del link "Cálculo de Honorarios", línea ~122), añadir:**

```html
<a *ngIf="isAdmin()" routerLink="/admin/honorarios/asignaciones" routerLinkActive="active-sublink" class="nav-subitem text-blue-500 font-black">Panel de Asignaciones</a>
<a *ngIf="isAdmin()" routerLink="/admin/honorarios/config" routerLinkActive="active-sublink" class="nav-subitem text-amber-500 font-black">Config. Honorarios</a>
```

---

## PASO 12: Componente de Configuración de Honorarios

**Archivo NUEVO:** `src/SistemaSatHospitalario.Frontend/src/app/features/admin/honorariums/honorario-config.component.ts`

Este componente muestra una tabla con 5 filas (una por categoría) y permite seleccionar el médico por defecto. Usa el patrón visual `midnight-panel` existente.

**Características:**
- GET `/api/HonorarioConfig` para cargar configuración actual.
- GET `/api/Medicos` para cargar la lista de médicos activos.
- PUT `/api/HonorarioConfig/{categoria}` al seleccionar un médico del dropdown.
- Cada fila: CATEGORÍA | DROPDOWN MEDICO | USUARIO QUE CONFIGURÓ | FECHA.
- Usar standalone component con `CommonModule, FormsModule, LucideAngularModule`.
- Iconos: `Settings, User, Save, Trash2` de lucide-angular.
- Importar `HttpClient` e `environment` como en otros componentes del proyecto.

**Template clave (tabla de configuración):**
```html
<!-- Header -->
<tr class="bg-white/5 border-b border-white/5">
    <th>CATEGORÍA</th>
    <th>MÉDICO POR DEFECTO</th>
    <th>CONFIGURADO POR</th>
    <th>FECHA</th>
    <th>ACCIONES</th>
</tr>

<!-- Fila por cada categoría -->
<tr *ngFor="let cat of categorias">
    <td>{{ cat.nombre }}</td>
    <td>
        <select [(ngModel)]="cat.medicoId" (change)="guardarConfig(cat)">
            <option [value]="null">-- Sin asignar --</option>
            <option *ngFor="let m of medicos()" [value]="m.id">{{ m.nombre }}</option>
        </select>
    </td>
    <td>{{ cat.usuarioConfiguro }}</td>
    <td>{{ cat.fechaConfiguracion | date:'dd/MM/yyyy HH:mm' }}</td>
    <td>
        <button (click)="limpiarConfig(cat)">Limpiar</button>
    </td>
</tr>
```

**Categorías fijas en el componente:**
```typescript
readonly CATEGORIAS = ['CONSULTA', 'RX', 'INFORME', 'CITOLOGIA', 'BIOPSIA'];
```

---

## PASO 13: Componente Panel de Asignaciones

**Archivo NUEVO:** `src/SistemaSatHospitalario.Frontend/src/app/features/admin/honorariums/honorario-asignaciones.component.ts`

Este es el componente principal tipo CxC. Muestra tabla de servicios filtrable.

**Características:**
- GET `/api/AsignacionHonorarios/pendientes?desde=&hasta=&estado=` para cargar datos.
- POST `/api/AsignacionHonorarios/asignar` al asignar un médico.
- Barra de filtros estilo CxC (fecha desde/hasta, selector de estado, botón filtrar).
- Tabla con columnas: PACIENTE | SERVICIO | TIPO | HONORARIO | MÉDICO | ESTADO | ACCIÓN.
- Badge de estado: PENDIENTE (rojo), ASIGNADO (verde), AUTO (índigo).
- Modal para seleccionar médico al hacer click en "Asignar".
- Standalone component, mismos imports que otros componentes.

**Estados visuales de los badges:**
```html
<!-- PENDIENTE: rojo -->
<span *ngIf="!item.medicoAsignadoId" class="px-3 py-1 bg-rose-500/5 border border-rose-500/10 text-rose-500/60 rounded-lg text-[8px] font-black uppercase">
    PENDIENTE
</span>

<!-- ASIGNADO (manual): verde -->
<span *ngIf="item.medicoAsignadoId && !item.esAutoAsignado" class="px-3 py-1 bg-emerald-500/5 border border-emerald-500/10 text-emerald-500/60 rounded-lg text-[8px] font-black uppercase">
    ASIGNADO
</span>

<!-- AUTO: índigo -->
<span *ngIf="item.medicoAsignadoId && item.esAutoAsignado" class="px-3 py-1 bg-indigo-500/5 border border-indigo-500/10 text-indigo-500/60 rounded-lg text-[8px] font-black uppercase">
    ⚡ AUTO
</span>
```

**Modal de asignación (reutilizar el patrón de modal de seguros-dashboard):**
- Overlay oscuro con `fixed inset-0`.
- Panel centrado con lista de médicos.
- Cada médico es un botón que al hacer click ejecuta la asignación.

---

## PASO 14: Modificar admin-honorariums.component.ts (Desglose)

**Archivo:** `src/SistemaSatHospitalario.Frontend/src/app/features/admin/honorariums/admin-honorariums.component.ts`

**Cambios:**
1. Añadir propiedad `expandedRow = signal<string | null>(null)`.
2. En cada `<tr>` del médico, al hacer click → `toggleExpand(row.medicoId)`.
3. Debajo de cada `<tr>`, añadir un `<tr>` condicional con el desglose:

```html
<tr *ngIf="expandedRow() === row.medicoId" class="bg-white/[0.02]">
    <td colspan="3" class="px-8 py-4">
        <div class="grid grid-cols-5 gap-3">
            <div *ngFor="let cat of row.desglose" class="midnight-panel p-4 text-center">
                <p class="text-[8px] font-black text-slate-500 uppercase tracking-widest mb-1">{{ cat.categoria }}</p>
                <p class="text-lg font-black text-white">{{ cat.cantidad }}</p>
                <p class="text-[10px] font-mono text-emerald-400">$ {{ cat.total | number:'1.2-2' }}</p>
            </div>
        </div>
    </td>
</tr>
```

4. El endpoint del backend `GetDoctorHonorariumSummaryQuery` debe ser modificado para incluir el `Desglose` (lista de `{Categoria, Cantidad, Total}`).

---

## PASO 15: Reconstruir y Verificar

```bash
# Backend
cd src/SistemaSatHospitalario.WebAPI
dotnet build

# Frontend
cd src/SistemaSatHospitalario.Frontend
npm.cmd run build -- --configuration docker

# Docker
cd ../..
docker compose --env-file ./instalacion/.env build
docker compose --env-file ./instalacion/.env up -d
```

### Verificación funcional en el navegador:
1. Login como admin (admin / Admin123*!).
2. Ir a Gestión Médica → Config. Honorarios → asignar "Dr. Jorge" como default de INFORME.
3. Ir a Facturación → cargar un servicio tipo INFORME a un paciente.
4. Ir a Panel de Asignaciones → verificar que aparece con badge "⚡ AUTO" y Dr. Jorge asignado.
5. Reasignar manualmente a otro médico → verificar badge cambia a "ASIGNADO".
6. Ir a Cálculo de Honorarios → expandir fila del médico → ver desglose por categoría.

---

## NOTAS IMPORTANTES PARA GEMINI FLASH

1. **DetalleServicioCuenta NO tiene propiedad de navegación `CuentaServicio`.** Necesitas añadirla:
   ```csharp
   public virtual CuentaServicios CuentaServicio { get; private set; }
   ```

2. **PacienteAdmision** — verifica si tiene `NombreCompleto` o si debes usar otra propiedad como `Nombre`.

3. **Los AddDbSet deben ir en AMBOS archivos:** `IApplicationDbContext.cs` Y `SatHospitalarioDbContext.cs`.

4. **OnModelCreating:** seguir el patrón existente con `entity.ToTable("NombreTabla")` y `entity.HasKey(x => x.Id)`.

5. **El sistema usa MySQL (Pomelo).** Las migraciones se generan con `--context SatHospitalarioDbContext`.

6. **Existe un segundo DbContext (IdentityDbContext).** SIEMPRE especificar `--context SatHospitalarioDbContext` en comandos EF.

7. **Frontend usa Angular 18 standalone components** con signals (`signal<T>()`) y `LucideAngularModule` para iconos.

8. **CSS usa TailwindCSS** con clases personalizadas como `midnight-panel`, `nav-item`, `nav-subitem`.

9. **PowerShell en Windows:** usar `npm.cmd` en lugar de `npm`. Usar `;` en vez de `&&`.

10. **Docker:** `docker compose --env-file ./instalacion/.env build` desde la raíz del proyecto.
