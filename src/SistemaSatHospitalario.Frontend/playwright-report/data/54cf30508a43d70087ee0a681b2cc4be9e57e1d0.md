# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: nursing.spec.ts >> Emergency Nursing & Egress Integrity Tests >> Módulo Enfermería: Charge clinical services of different categories separately and verify total calculation
- Location: e2e\nursing.spec.ts:164:7

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
                  - 'textbox "Escriba código o nombre del insumo (ej: Morfina, Jeringas...)" [ref=e277]': Consulta Medica General
                  - img [ref=e279]
              - generic [ref=e281]:
                - generic [ref=e282]:
                  - generic [ref=e283]: Insumo Seleccionado
                  - generic [ref=e284]: Consulta Medica General
                  - generic [ref=e285]: "Precio Base: $20 USD"
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
  183 |     await searchInput.fill('Consulta Medica General');
  184 |     await page.waitForTimeout(1000); // esperar debounce del autocomplete
  185 |     await page.locator('div.hover\\:bg-white\\/5').first().click();
  186 |     await page.waitForTimeout(500);
  187 | 
  188 |     // El selector de médico aparece en el paso 2 (al que se avanza automáticamente)
> 189 |     await page.waitForSelector('#selectMedicoFastCharge', { timeout: 8000 });
      |                ^ TimeoutError: page.waitForSelector: Timeout 8000ms exceeded.
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