# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: nursing.spec.ts >> Emergency Nursing & Egress Integrity Tests >> Módulo Enfermería: Selective triage, pinned status, and doctor selection on quick charge
- Location: e2e\nursing.spec.ts:36:7

# Error details

```
TimeoutError: page.waitForSelector: Timeout 8000ms exceeded.
Call log:
  - waiting for locator('#selectMedicoFastCharge') to be visible

```

# Page snapshot

```yaml
- generic [ref=e4]:
  - generic [ref=e6]:
    - generic [ref=e7]:
      - generic [ref=e9]: S
      - generic [ref=e10]: SAT Hosp
    - navigation [ref=e11]:
      - generic [ref=e12]: Menu Principal
      - link "Panel de Control" [ref=e13] [cursor=pointer]:
        - /url: /dashboard
        - img [ref=e15]
        - generic [ref=e20]: Panel de Control
      - link "Módulo Enfermería" [ref=e21] [cursor=pointer]:
        - /url: /enfermeria
        - img [ref=e23]
        - generic [ref=e25]: Módulo Enfermería
      - button "Gestión de Órdenes" [ref=e27] [cursor=pointer]:
        - generic [ref=e28]:
          - img [ref=e30]
          - text: Gestión de Órdenes
        - img [ref=e35]
      - generic [ref=e37]: Operativo / Gestión
      - link "Estado del Sistema v1.2.83" [ref=e38] [cursor=pointer]:
        - /url: /github-test
        - img [ref=e40]
        - generic [ref=e43]: Estado del Sistema v1.2.83
    - generic [ref=e45]:
      - img [ref=e48]
      - generic [ref=e53]:
        - paragraph [ref=e54]: user_emergencia
        - paragraph [ref=e55]: Asistente Hospitalario
      - button [ref=e56] [cursor=pointer]:
        - img [ref=e58]
  - main [ref=e61]:
    - generic [ref=e62]:
      - generic [ref=e65]:
        - generic [ref=e66]: "Sede Activa:"
        - combobox [ref=e67] [cursor=pointer]:
          - option "Área de Emergencia"
          - option "Sucursal de Pruebas E2E"
          - option "Sede Principal Hospitalaria (Principal)" [selected]
          - option "Área de Hospitalización"
      - generic [ref=e68]:
        - button [ref=e71] [cursor=pointer]:
          - img [ref=e73]
        - generic [ref=e77]: JB
    - generic [ref=e81]:
      - generic [ref=e83]:
        - generic [ref=e84]:
          - img [ref=e87]
          - generic [ref=e89]:
            - heading "Módulo de Enfermería" [level=1] [ref=e90]
            - generic [ref=e91]:
              - generic [ref=e92]: Tablet Active View
              - paragraph [ref=e93]: Triage, Dosificación e Historial
        - generic [ref=e94]:
          - button "REFRESCAR PACIENTES" [ref=e95] [cursor=pointer]:
            - img [ref=e97]
            - text: REFRESCAR PACIENTES
          - button "CERRAR SESIÓN" [ref=e102] [cursor=pointer]
      - generic [ref=e103]:
        - generic [ref=e105]:
          - heading "Pacientes Activos (10)" [level=3] [ref=e106]:
            - img [ref=e108]
            - text: Pacientes Activos (10)
          - generic [ref=e111]:
            - textbox "Buscar por paciente, cedula o area..." [ref=e112]
            - img [ref=e114]
          - generic [ref=e117]:
            - generic [ref=e118] [cursor=pointer]:
              - generic [ref=e119]:
                - generic [ref=e120]:
                  - heading "Adriana paez" [level=4] [ref=e121]
                  - paragraph [ref=e122]: "27383766"
                - generic [ref=e123]:
                  - generic [ref=e124]: Emergencia
                  - generic [ref=e125]: PARTICULAR
              - generic [ref=e126]:
                - generic [ref=e127]: $0,00
                - generic [ref=e128]: TOTAL CARGADO
            - generic [ref=e130] [cursor=pointer]:
              - generic [ref=e131]:
                - generic [ref=e132]:
                  - heading "MARÍA_E2E SÁNCHEZ_DB" [level=4] [ref=e133]
                  - paragraph [ref=e134]: V-05441321
                - generic [ref=e135]:
                  - generic [ref=e136]: Seguro
                  - generic [ref=e137]: pdvsa
              - generic [ref=e138]:
                - generic [ref=e139]: $45,00
                - generic [ref=e140]: TOTAL CARGADO
            - generic [ref=e141] [cursor=pointer]:
              - generic [ref=e142]:
                - generic [ref=e143]:
                  - heading "JOSE GREGORIO BRACAMONTE GIL" [level=4] [ref=e144]
                  - paragraph [ref=e145]: "24556681"
                - generic [ref=e146]:
                  - generic [ref=e147]: Emergencia
                  - generic [ref=e148]: PARTICULAR
              - generic [ref=e149]:
                - generic [ref=e150]: $0,00
                - generic [ref=e151]: TOTAL CARGADO
            - generic [ref=e152] [cursor=pointer]:
              - generic [ref=e153]:
                - generic [ref=e154]:
                  - heading "MARIA ALEJANDRA JIMENEZ DE NIEVES" [level=4] [ref=e155]
                  - paragraph [ref=e156]: "10732123"
                - generic [ref=e157]:
                  - generic [ref=e158]: Emergencia
                  - generic [ref=e159]: pdvsa
              - generic [ref=e160]:
                - generic [ref=e161]: $0,00
                - generic [ref=e162]: TOTAL CARGADO
            - generic [ref=e163] [cursor=pointer]:
              - generic [ref=e164]:
                - generic [ref=e165]:
                  - heading "LUIS CANCINES" [level=4] [ref=e166]
                  - paragraph [ref=e167]: V33785674
                - generic [ref=e168]:
                  - generic [ref=e169]: Seguro
                  - generic [ref=e170]: pdvsa
              - generic [ref=e171]:
                - generic [ref=e172]: $50,00
                - generic [ref=e173]: TOTAL CARGADO
            - generic [ref=e174] [cursor=pointer]:
              - generic [ref=e175]:
                - generic [ref=e176]:
                  - heading "JULIO GONZALEZ" [level=4] [ref=e177]
                  - paragraph [ref=e178]: V27655085
                - generic [ref=e179]:
                  - generic [ref=e180]: Particular
                  - generic [ref=e181]: PARTICULAR
              - generic [ref=e182]:
                - generic [ref=e183]: $60,00
                - generic [ref=e184]: TOTAL CARGADO
            - generic [ref=e185] [cursor=pointer]:
              - generic [ref=e186]:
                - generic [ref=e187]:
                  - heading "JULIO GONZALEZ" [level=4] [ref=e188]
                  - paragraph [ref=e189]: V27655085
                - generic [ref=e190]:
                  - generic [ref=e191]: Particular
                  - generic [ref=e192]: PARTICULAR
              - generic [ref=e193]:
                - generic [ref=e194]: $60,00
                - generic [ref=e195]: TOTAL CARGADO
            - generic [ref=e196] [cursor=pointer]:
              - generic [ref=e197]:
                - generic [ref=e198]:
                  - heading "JOSE GREGORIO BRACAMONTE GIL" [level=4] [ref=e199]
                  - paragraph [ref=e200]: "24556681"
                - generic [ref=e201]:
                  - generic [ref=e202]: Particular
                  - generic [ref=e203]: PARTICULAR
              - generic [ref=e204]:
                - generic [ref=e205]: $265,00
                - generic [ref=e206]: TOTAL CARGADO
            - generic [ref=e207] [cursor=pointer]:
              - generic [ref=e208]:
                - generic [ref=e209]:
                  - heading "JOSE GREGORIO BRACAMONTE GIL" [level=4] [ref=e210]
                  - paragraph [ref=e211]: "24556681"
                - generic [ref=e212]:
                  - generic [ref=e213]: Particular
                  - generic [ref=e214]: PARTICULAR
              - generic [ref=e215]:
                - generic [ref=e216]: $215,00
                - generic [ref=e217]: TOTAL CARGADO
            - generic [ref=e218] [cursor=pointer]:
              - generic [ref=e219]:
                - generic [ref=e220]:
                  - heading "JOSE GREGORIO BRACAMONTE GIL" [level=4] [ref=e221]
                  - paragraph [ref=e222]: "24556681"
                - generic [ref=e223]:
                  - generic [ref=e224]: Particular
                  - generic [ref=e225]: PARTICULAR
              - generic [ref=e226]:
                - generic [ref=e227]: $70,00
                - generic [ref=e228]: TOTAL CARGADO
        - generic [ref=e230]:
          - generic [ref=e231]:
            - generic [ref=e232]:
              - img [ref=e235]
              - generic [ref=e238]:
                - heading "Adriana paez" [level=2] [ref=e239]
                - generic [ref=e240]:
                  - generic [ref=e241]: "27383766"
                  - generic [ref=e242]: •
                  - generic [ref=e243]: Emergencia
                  - generic [ref=e244]: •
                  - generic [ref=e245]: PARTICULAR
            - generic [ref=e246]:
              - generic [ref=e247]: Total Acumulado
              - text: $0,00
          - generic [ref=e248]:
            - generic [ref=e249]:
              - generic [ref=e250]: ESTADO ACTUAL (PINNED)
              - generic [ref=e251]: "Última actualización: 02/07/2026 17:30 por user_emergencia"
            - generic [ref=e252]:
              - generic [ref=e253]: paciente ingresa con dolor de cabeza,diarrea y dolor de estomago
              - paragraph [ref=e254]: "Ingreso inicial por: paciente ingresa con dolor de cabeza,diarrea y dolor de estomago. Clasificación: Nivel I (Rojo)"
          - generic [ref=e255]:
            - button "TRIAGE Y SIGNOS VITALES" [ref=e256] [cursor=pointer]:
              - img [ref=e258]
              - text: TRIAGE Y SIGNOS VITALES
            - button "CARGA DE INSUMOS" [ref=e260] [cursor=pointer]:
              - img [ref=e262]
              - text: CARGA DE INSUMOS
          - generic [ref=e264]:
            - generic [ref=e265]:
              - img [ref=e268]
              - generic [ref=e270]:
                - heading "Carga Rápida de Insumos y Medicamentos" [level=3] [ref=e271]
                - paragraph [ref=e272]: Añade consumos directos a la cuenta del paciente
            - generic [ref=e273]:
              - generic [ref=e274]:
                - generic [ref=e275]: Seleccionar Medicamento o Insumo
                - generic [ref=e276]:
                  - 'textbox "Escriba código o nombre del insumo (ej: Morfina, Jeringas...)" [ref=e277]': Consulta Ginecologica
                  - img [ref=e279]
              - generic [ref=e281]:
                - generic [ref=e282]:
                  - generic [ref=e283]: Insumo Seleccionado
                  - generic [ref=e284]: Consulta Ginecologica
                  - generic [ref=e285]: "Precio Base: $10 USD"
                - generic [ref=e286]:
                  - generic [ref=e287]: Médico Tratante / Responsable *
                  - combobox [ref=e288]:
                    - option "--- Seleccionar Médico ---" [selected]
                    - option "GREGORY HOUSE"
                    - option "DR. JAIRO MANAREZ"
                    - option "STEPHEN STRANGE"
                    - option "DRA. LISBETH HERRERA"
                    - option "JOHN WATSON"
                    - option "RAUL HERRERA"
                    - option "JAMES WILSON"
                    - option "MARÍA GUTIÉRREZ"
                    - option "LISA CUDDY"
                    - option "DR. ALIRIO TORRES"
                    - option "DR. IBRAHIM ORELLANA"
                    - option "DRA. LIZMAR GARCIA"
                    - option "DRA. KAREN PEREZ"
                    - option "JOSÉ BRACAMONTE"
                    - option "PATCH ADAMS"
                - generic [ref=e289]:
                  - generic [ref=e290]: "Cantidad a Cargar (Unidad: Unidad(es))"
                  - generic [ref=e291]:
                    - spinbutton [ref=e292]: "1"
                    - generic [ref=e293]: UD
            - generic [ref=e294]:
              - button "CANCELAR" [ref=e295] [cursor=pointer]
              - button "CARGAR A LA CUENTA" [ref=e296] [cursor=pointer]
    - generic:
      - img
```

# Test source

```ts
  1   | import { test, expect } from '@playwright/test';
  2   | 
  3   | // ─── Helpers ─────────────────────────────────────────────────────────────────
  4   | /**
  5   |  * Autentica a un usuario y espera la redirección post-login.
  6   |  * Acepta cualquiera de las rutas clínicas o de panel dependiendo del rol.
  7   |  * - Admin/Billing: /dashboard
  8   |  * - Rol clínico (user_emergencia, enfermera): /cierre-cuenta/* o /enfermeria
  9   |  */
  10  | async function loginAs(
  11  |   page: any,
  12  |   username: string,
  13  |   password: string
  14  | ): Promise<void> {
  15  |   await page.goto('/');
  16  |   await page.waitForLoadState('networkidle');
  17  |   await page.fill('input#username', username);
  18  |   await page.fill('input#password', password);
  19  |   await page.click('button[type="submit"]');
  20  |   // Esperar redirección — acepta dashboard (admin) o ruta clínica (user_emergencia)
  21  |   await page.waitForURL(
  22  |     (url: URL) =>
  23  |       url.pathname.includes('dashboard') ||
  24  |       url.pathname.includes('cierre-cuenta') ||
  25  |       url.pathname.includes('enfermeria'),
  26  |     { timeout: 15000 }
  27  |   );
  28  |   console.log(`[AUTH] Logged in as '${username}' → ${page.url()}`);
  29  | }
  30  | 
  31  | test.describe('Emergency Nursing & Egress Integrity Tests', () => {
  32  |   test.beforeEach(async ({ page }) => {
  33  |     await loginAs(page, 'user_emergencia', 'Hospital2026*!');
  34  |   });
  35  | 
  36  |   test('Módulo Enfermería: Selective triage, pinned status, and doctor selection on quick charge', async ({ page }) => {
  37  |     // 1. Navigate to Módulo Enfermería
  38  |     await page.goto('/enfermeria');
  39  |     await page.waitForLoadState('networkidle');
  40  | 
  41  |     // Wait for active patients list
  42  |     await page.waitForSelector('h3:has-text("Pacientes Activos")');
  43  |     console.log('Active patients panel loaded.');
  44  | 
  45  |     // Click on the first active patient if any are listed
  46  |     const firstPatient = page.locator('.space-y-3.max-h-\\[600px\\] > div').first();
  47  |     const countPatients = await firstPatient.count();
  48  |     if (countPatients === 0) {
  49  |       console.log('No active patients found, skipping clinical operations test.');
  50  |       return;
  51  |     }
  52  | 
  53  |     await firstPatient.click();
  54  |     console.log('Selected active patient.');
  55  | 
  56  |     // Verify modular triage section checkboxes are visible (con el case exacto del HTML)
  57  |     await expect(page.locator('text=Triage y Signos Vitales')).toBeVisible();
  58  |     await expect(page.locator('label:has-text("1. Signos Vitales")')).toBeVisible();
  59  |     await expect(page.locator('label:has-text("2. Valoración Física")')).toBeVisible();
  60  |     await expect(page.locator('label:has-text("3. Antecedentes")')).toBeVisible();
  61  |     await expect(page.locator('label:has-text("4. Estado Actual")')).toBeVisible();
  62  |     console.log('Modular triage flags verified.');
  63  | 
  64  |     // Navigate to Carga de Insumos tab (con el case exacto del HTML)
  65  |     await page.click('button:has-text("Carga de Insumos")');
  66  |     console.log('Navigated to Carga de Insumos.');
  67  | 
  68  |     // Search for a service that requires doctor association (Consultation / Rx / Tomography)
  69  |     const searchInput = page.locator('input[placeholder*="Escriba código o nombre"]');
  70  |     await searchInput.fill('CONSULTA GINECOLOGICA');
  71  |     await page.waitForTimeout(1000);
  72  | 
  73  |     // Select the autocomplete result
  74  |     const firstResult = page.locator('div.hover\\:bg-white\\/5').first();
  75  |     await firstResult.click();
  76  |     console.log('Selected CONSULTA GINECOLOGICA.');
  77  | 
  78  |     // --- Step 2: Configure manual price overrides and responsibilities ---
  79  |     // El sistema avanza automáticamente al Paso 2 (ajustes) tras seleccionar del autocompletado
  80  | 
  81  |     // Verify doctor selector dropdown is shown in Step 2
> 82  |     await page.waitForSelector('#selectMedicoFastCharge', { timeout: 8000 });
      |                ^ TimeoutError: page.waitForSelector: Timeout 8000ms exceeded.
  83  |     
  84  |     // Seleccionar médico y área clínica en el Paso 2 usando IDs explícitos
  85  |     const doctorSelector = page.locator('#selectMedicoFastCharge');
  86  |     await expect(doctorSelector).toBeVisible();
  87  |     console.log('Doctor selector dropdown is visible.');
  88  | 
  89  |     // Select a doctor
  90  |     await doctorSelector.selectOption({ index: 1 });
  91  |     console.log('Selected a seeded doctor.');
  92  | 
  93  |     // Seleccionar también el Área Clínica en el Paso 2
  94  |     const areaSelector = page.locator('#selectAreaClinicaFastCharge');
  95  |     await areaSelector.selectOption({ index: 1 });
  96  |     console.log('Selected an Area Clinica.');
  97  | 
  98  |     // Now click Siguiente to advance to Step 3 (Confirmation)
  99  |     const nextBtnStep2 = page.locator('#btnStep2Next');
  100 |     await expect(nextBtnStep2).toBeEnabled();
  101 |     await nextBtnStep2.click();
  102 |     console.log('Advanced to Step 3 (Confirmation).');
  103 | 
  104 |     // Verify Step 3 displays the patient's name and Cédula
  105 |     await expect(page.locator('span:has-text("Cédula:")').first()).toBeVisible();
  106 |     await expect(page.locator('span:has-text("Médico Tratante")')).toBeVisible();
  107 |     console.log('Step 3 confirmation details verified.');
  108 | 
  109 |     // Verify final submission via local cart
  110 |     await page.click('#btnStep3Confirm');
  111 |     console.log('Added CONSULTA GINECOLOGICA to cart.');
  112 | 
  113 |     // Verify it is visible in the cart
  114 |     await expect(page.locator('app-nursing-cart')).toBeVisible();
  115 |     await expect(page.locator('app-nursing-cart :text-is("CONSULTA GINECOLOGICA")')).toBeVisible();
  116 | 
  117 |     // Submit all cart items to backend
  118 |     await page.click('button:has-text("Registrar todo a la cuenta")');
  119 |     await page.waitForTimeout(2000);
  120 |   });
  121 | 
  122 |   test('Cierre Cuenta: Read-only check and Egress/Transfer panel for clinical users', async ({ page }) => {
  123 |     // 1. Navigate to Cierre Cuenta (Emergency directory)
  124 |     await page.goto('/cierre-cuenta/Emergencia');
  125 |     await page.waitForLoadState('networkidle');
  126 | 
  127 |     // Wait for active patients list
  128 |     await page.waitForSelector('h3:has-text("Pacientes Activos")', { timeout: 15000 });
  129 | 
  130 |     // Click on the first active patient
  131 |     const firstPatient = page.locator('.premium-card').first();
  132 |     const countPatients = await firstPatient.count();
  133 |     if (countPatients === 0) {
  134 |       console.log('No active patients found, skipping egress panel tests.');
  135 |       return;
  136 |     }
  137 | 
  138 |     await firstPatient.click();
  139 |     console.log('Selected patient in close account screen.');
  140 | 
  141 |     // Verify date/time inputs are read-only text elements for clinical users
  142 |     const dateInput = page.locator('input[type="date"]');
  143 |     const countDateInput = await dateInput.count();
  144 |     expect(countDateInput).toBe(0); // Hidden/replaced for nurses
  145 |     console.log('Admission date input is hidden (Read-only view verified).');
  146 | 
  147 |     // Verify "Cargar Servicio o Medicamento" card is hidden
  148 |     const fastChargeTitle = page.locator('h3:has-text("Cargar Servicio o Medicamento")');
  149 |     const countFastCharge = await fastChargeTitle.count();
  150 |     expect(countFastCharge).toBe(0); // Hidden/replaced for nurses
  151 |     console.log('Quick charge service panel is hidden from close account view.');
  152 | 
  153 |     // Verify "Condición y Destino Final de Egreso de Urgencias" is visible
  154 |     await expect(page.locator('span:has-text("Condición y Destino Final de Egreso")')).toBeVisible();
  155 |     await expect(page.locator('span:has-text("Destino del Paciente:")')).toBeVisible();
  156 |     console.log('Transfer / Egress destination panel is visible.');
  157 | 
  158 |     // Verify "Procesar Traslado y Egreso" button is visible inside the bottom info card
  159 |     const egressBtn = page.locator('button:has-text("Procesar Traslado y Egreso")');
  160 |     await expect(egressBtn).toBeVisible();
  161 |     console.log('"Procesar Traslado y Egreso" button is visible for clinical assistants.');
  162 |   });
  163 | 
  164 |   test('Módulo Enfermería: Charge clinical services of different categories separately and verify total calculation', async ({ page }) => {
  165 |     // 1. Navigate to Módulo Enfermería
  166 |     await page.goto('/enfermeria');
  167 |     await page.waitForLoadState('networkidle');
  168 | 
  169 |     // Click on the first active patient
  170 |     const firstPatient = page.locator('.space-y-3.max-h-\\[600px\\] > div').first();
  171 |     const countPatients = await firstPatient.count();
  172 |     if (countPatients === 0) {
  173 |       console.log('No active patients found, skipping test.');
  174 |       return;
  175 |     }
  176 |     await firstPatient.click();
  177 | 
  178 |     // Go to "Carga de Insumos" (con el case exacto del HTML)
  179 |     await page.click('button:has-text("Carga de Insumos")');
  180 |     const searchInput = page.locator('input[placeholder*="Escriba código o nombre"]');
  181 | 
  182 |     // --- 1. Consulta Category (requiere médico) ---
```