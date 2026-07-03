# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: nursing.spec.ts >> Emergency Nursing & Egress Integrity Tests >> Módulo Enfermería: Charge clinical services of different categories separately and verify total calculation
- Location: e2e\nursing.spec.ts:164:7

# Error details

```
Error: expect(locator).toBeVisible() failed

Locator: locator('input[placeholder*="Escriba código o nombre"]')
Expected: visible
Timeout: 10000ms
Error: element(s) not found

Call log:
  - Expect "toBeVisible" with timeout 10000ms
  - waiting for locator('input[placeholder*="Escriba código o nombre"]')

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
  - text: "Paciente Adriana paez Cédula: 27383766 Ítem a Cargar Consulta Medica General ESPECIALIDAD: DIAGNÓSTICO DIFERENCIAL Cantidad 1 UD Médico Tratante GREGORY HOUSE Precio Base ($) $20,00 USD Honorario Médico ($) $0,00 USD Total Cargo Registrado ($) $20,00 USD"
  - paragraph: "* Este cargo se registrará a crédito del paciente para su posterior reconciliación con el seguro."
  - button "Atrás"
  - text: Paso 3 de 3
  - button "CONFIRMAR Y CARGAR A LA CUENTA"
  - img
```

# Test source

```ts
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
> 207 |     await expect(searchInput2).toBeVisible({ timeout: 10000 });
      |                                ^ Error: expect(locator).toBeVisible() failed
  208 |     await searchInput2.fill('Radiografía Tórax');
  209 |     await page.waitForTimeout(1000);
  210 |     await page.locator('div.hover\\:bg-white\\/5').first().click();
  211 |     await page.waitForTimeout(500);
  212 |     
  213 |     // Seleccionar área clínica
  214 |     await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });
  215 | 
  216 |     // Avanza al Paso 3
  217 |     const step2Btn2 = page.locator('#btnStep2Next');
  218 |     await expect(step2Btn2).toBeEnabled();
  219 |     await step2Btn2.click();
  220 |     
  221 |     const step3Btn2 = page.locator('#btnStep3Confirm');
  222 |     await expect(step3Btn2).toBeVisible();
  223 |     await step3Btn2.click();
  224 |     await page.waitForTimeout(2000);
  225 | 
  226 |     // --- 3. Informe Category ---
  227 |     const searchInput3 = page.locator('input[placeholder*="Escriba código o nombre"]');
  228 |     await expect(searchInput3).toBeVisible({ timeout: 10000 });
  229 |     await searchInput3.fill('Informe Médico Especializado');
  230 |     await page.waitForTimeout(1000);
  231 |     await page.locator('div.hover\\:bg-white\\/5').first().click();
  232 |     await page.waitForTimeout(500);
  233 |     
  234 |     // Seleccionar área clínica
  235 |     await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });
  236 | 
  237 |     // Avanza al Paso 3
  238 |     const step2Btn3 = page.locator('#btnStep2Next');
  239 |     await expect(step2Btn3).toBeEnabled();
  240 |     await step2Btn3.click();
  241 |     
  242 |     const step3Btn3 = page.locator('#btnStep3Confirm');
  243 |     await expect(step3Btn3).toBeVisible();
  244 |     await step3Btn3.click();
  245 |     await page.waitForTimeout(1000);
  246 | 
  247 |     // Verify all items are accumulated in the cart
  248 |     await expect(page.locator('app-nursing-cart')).toBeVisible();
  249 | 
  250 |     // Now click the "Registrar todo a la cuenta" button in the cart
  251 |     await page.click('button:has-text("Registrar todo a la cuenta")');
  252 |     await page.waitForTimeout(3000);
  253 | 
  254 |     console.log('All three separate catalog categories successfully charged to patient.');
  255 |   });
  256 | });
  257 | 
```