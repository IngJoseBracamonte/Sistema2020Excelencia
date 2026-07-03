# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: nursing.spec.ts >> Emergency Nursing & Egress Integrity Tests >> Módulo Enfermería: Selective triage, pinned status, and doctor selection on quick charge
- Location: e2e\nursing.spec.ts:36:7

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('app-nursing-cart')
Expected: visible
Timeout: 5000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" with timeout 5000ms
  - waiting for locator('app-nursing-cart')

```

```yaml
- text: S SAT Hosp
- navigation:
  - text: Menu Principal
  - link "Panel de Control":
    - /url: /dashboard
  - link "Módulo Enfermería":
    - /url: /enfermeria
  - button "Gestión de Órdenes"
  - text: Operativo / Gestión
  - link "Estado del Sistema v1.2.87":
    - /url: /github-test
- paragraph: user_emergencia
- paragraph: Asistente Hospitalario
- button
- main:
  - button
  - text: JB
  - heading "Módulo de Enfermería" [level=1]
  - paragraph: Registro de triage, constantes vitales y administración rápida de cargos clínicos
  - button "Emergencia"
  - button "Hospitalización"
  - button "UCI"
  - heading "Pacientes Activos" [level=3]
  - button
  - textbox "Buscar por cédula o nombre..."
  - heading "Adriana paez" [level=4]
  - paragraph: "27383766"
  - text: Emergencia PARTICULAR $0,00 TOTAL CARGADO
  - heading "JOSE GREGORIO BRACAMONTE GIL" [level=4]
  - paragraph: "24556681"
  - text: Emergencia PARTICULAR $0,00 TOTAL CARGADO
  - heading "MARIA ALEJANDRA JIMENEZ DE NIEVES" [level=4]
  - paragraph: "10732123"
  - text: Emergencia pdvsa $0,00 TOTAL CARGADO Cuenta Abierta
  - heading "Adriana paez" [level=2]
  - text: "Cédula: 27383766 Seguro: PARTICULAR Total Acumulado en Cuenta $ 0,00"
  - button "Triage y Signos Vitales"
  - button "Carga de Insumos"
  - button "Traslados y Destino"
  - text: Búsqueda Ajuste 3 Confirmación
  - heading "Confirmar Cargo" [level=3]
  - paragraph: Verifique los detalles antes de registrar a la cuenta
  - text: "Paciente Adriana paez Cédula: 27383766 Ítem a Cargar Consulta Ginecologica ESPECIALIDAD: DIAGNÓSTICO DIFERENCIAL Cantidad 1 UD Médico Tratante GREGORY HOUSE Precio Base ($) $10,00 USD Honorario Médico ($) $0,00 USD Total Cargo Registrado ($) $10,00 USD"
  - paragraph: "* Este cargo se registrará a crédito del paciente para su posterior reconciliación con el seguro."
  - button "Atrás"
  - text: Paso 3 de 3
  - button "CONFIRMAR Y CARGAR A LA CUENTA"
  - img
```

# Test source

```ts
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
  82  |     await page.waitForSelector('#selectMedicoFastCharge', { timeout: 8000 });
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
> 114 |     await expect(page.locator('app-nursing-cart')).toBeVisible();
      |                                                    ^ Error: expect(locator).toBeVisible() failed
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
  183 |     await searchInput.fill('Consulta Medica General');
  184 |     await page.waitForTimeout(1000); // esperar debounce del autocomplete
  185 |     await page.locator('div.hover\\:bg-white\\/5').first().click();
  186 |     await page.waitForTimeout(500);
  187 | 
  188 |     // El selector de médico aparece en el paso 2 (al que se avanza automáticamente)
  189 |     await page.waitForSelector('#selectMedicoFastCharge', { timeout: 8000 });
  190 |     
  191 |     // Seleccionar médico y área clínica
  192 |     await page.locator('#selectMedicoFastCharge').selectOption({ index: 1 });
  193 |     await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });
  194 |     
  195 |     // Click Siguiente en el Paso 2 cuando el botón esté habilitado
  196 |     const step2Btn1 = page.locator('#btnStep2Next');
  197 |     await expect(step2Btn1).toBeEnabled();
  198 |     await step2Btn1.click();
  199 |     
  200 |     const step3Btn1 = page.locator('#btnStep3Confirm');
  201 |     await expect(step3Btn1).toBeVisible();
  202 |     await step3Btn1.click();
  203 |     await page.waitForTimeout(2000); // esperar respuesta del API y reinicio a Paso 1
  204 | 
  205 |     // --- 2. RX Category (no requiere médico, tipo examen) ---
  206 |     const searchInput2 = page.locator('input[placeholder*="Escriba código o nombre"]');
  207 |     await expect(searchInput2).toBeVisible({ timeout: 10000 });
  208 |     await searchInput2.fill('Radiografía Tórax');
  209 |     await page.waitForTimeout(1000);
  210 |     await page.locator('div.hover\\:bg-white\\/5').first().click();
  211 |     await page.waitForTimeout(500);
  212 |     
  213 |     // Seleccionar área clínica
  214 |     await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });
```