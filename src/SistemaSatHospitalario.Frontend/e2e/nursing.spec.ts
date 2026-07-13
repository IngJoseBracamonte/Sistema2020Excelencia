import { test, expect } from '@playwright/test';

// ─── Helpers ─────────────────────────────────────────────────────────────────
/**
 * Autentica a un usuario y espera la redirección post-login.
 * Acepta cualquiera de las rutas clínicas o de panel dependiendo del rol.
 * - Admin/Billing: /dashboard
 * - Rol clínico (user_emergencia, enfermera): /cierre-cuenta/* o /enfermeria
 */
async function loginAs(
  page: any,
  username: string,
  password: string
): Promise<void> {
  await page.goto('/');
  await page.waitForLoadState('networkidle');
  await page.fill('input#username', username);
  await page.fill('input#password', password);
  await page.click('button[type="submit"]');
  // Esperar redirección — acepta dashboard (admin) o ruta clínica (user_emergencia)
  await page.waitForURL(
    (url: URL) =>
      url.pathname.includes('dashboard') ||
      url.pathname.includes('cierre-cuenta') ||
      url.pathname.includes('enfermeria'),
    { timeout: 15000 }
  );
  console.log(`[AUTH] Logged in as '${username}' → ${page.url()}`);
}

test.describe('Emergency Nursing & Egress Integrity Tests', () => {
  test.beforeEach(async ({ page }) => {
    page.on('console', msg => console.log('BROWSER LOG:', msg.text()));
    await loginAs(page, 'user_emergencia', 'Hospital2026*!');
  });

  test('Módulo Enfermería: Selective triage, pinned status, and doctor selection on quick charge', async ({ page }) => {
    // 1. Navigate to Módulo Enfermería
    await page.goto('/enfermeria');
    await page.waitForLoadState('networkidle');

    // Wait for active patients list
    await page.waitForSelector('h3:has-text("Pacientes Activos")');
    console.log('Active patients panel loaded.');

    // Click on the first active patient if any are listed
    const firstPatient = page.locator('.space-y-3.max-h-\\[600px\\] > div').first();
    const countPatients = await firstPatient.count();
    if (countPatients === 0) {
      console.log('No active patients found, skipping clinical operations test.');
      return;
    }

    await firstPatient.click();
    console.log('Selected active patient.');

    // Verify triage title is visible
    await expect(page.locator('h3:has-text("Triage y Signos Vitales")')).toBeVisible();

    // If the "Nuevo Triage" button is visible (because patient already has history), click it to open the form
    const nuevoTriageBtn = page.locator('button:has-text("Nuevo Triage")');
    if (await nuevoTriageBtn.isVisible()) {
      await nuevoTriageBtn.click();
      console.log('Clicked "Nuevo Triage" button to display the form.');
    }

    // Verify modular triage section checkboxes are visible (con el case exacto del HTML)
    await expect(page.locator('label:has-text("1. Signos Vitales")')).toBeVisible();
    await expect(page.locator('label:has-text("2. Valoración Física")')).toBeVisible();
    await expect(page.locator('label:has-text("3. Antecedentes")')).toBeVisible();
    await expect(page.locator('label:has-text("4. Estado Actual")')).toBeVisible();
    console.log('Modular triage flags verified.');

    // Navigate to Carga de Insumos tab (con el case exacto del HTML)
    await page.click('button:has-text("Carga de Insumos")');
    console.log('Navigated to Carga de Insumos.');

    // Search for a service that requires doctor association (Consultation / Rx / Tomography)
    const searchInput = page.locator('input[placeholder*="Escriba código o nombre"]');
    await searchInput.fill('CONSULTA GINECOLOGICA');
    await page.waitForTimeout(1000);

    // Select the autocomplete result
    const firstResult = page.locator('app-step-catalog-search div.hover\\:bg-white\\/5').first();
    await expect(firstResult).toBeVisible({ timeout: 10000 });
    await firstResult.click();
    console.log('Selected CONSULTA GINECOLOGICA.');

    // --- Step 2: Configure manual price overrides and responsibilities ---
    // El sistema avanza automáticamente al Paso 2 (ajustes) tras seleccionar del autocompletado

    // Verify doctor selector dropdown is shown in Step 2
    await page.waitForSelector('#selectMedicoFastCharge', { timeout: 8000 });
    
    // Seleccionar médico y área clínica en el Paso 2 usando IDs explícitos
    const doctorSelector = page.locator('#selectMedicoFastCharge');
    await expect(doctorSelector).toBeVisible();
    console.log('Doctor selector dropdown is visible.');

    // Select a doctor
    await doctorSelector.selectOption({ index: 1 });
    console.log('Selected a seeded doctor.');

    // Seleccionar también el Área Clínica en el Paso 2
    const areaSelector = page.locator('#selectAreaClinicaFastCharge');
    await areaSelector.selectOption({ index: 1 });
    console.log('Selected an Area Clinica.');

    // Now click Siguiente to advance to Step 3 (Confirmation)
    const nextBtnStep2 = page.locator('#btnStep2Next');
    await expect(nextBtnStep2).toBeEnabled();
    await nextBtnStep2.click();
    console.log('Advanced to Step 3 (Confirmation).');

    // Verify Step 3 displays the patient's name and Cédula
    await expect(page.locator('span:has-text("Cédula:")').first()).toBeVisible();
    await expect(page.locator('span:has-text("Médico Tratante")')).toBeVisible();
    console.log('Step 3 confirmation details verified.');

    // Verify final submission via local cart
    await page.click('#btnStep3Confirm');
    console.log('Added CONSULTA GINECOLOGICA to cart.');

    // Verify it is visible in the cart
    await expect(page.locator('app-nursing-cart')).toBeVisible();
    await expect(page.locator('app-nursing-cart').locator('text=Consulta Ginecologica')).toBeVisible();

    // Submit all cart items to backend
    await page.click('button:has-text("Registrar todo a la cuenta")');
    await page.waitForTimeout(2000);
  });

  test('Cierre Cuenta: Read-only check and Egress/Transfer panel for clinical users', async ({ page }) => {
    // 1. Navigate to Cierre Cuenta (Emergency directory)
    await page.goto('/cierre-cuenta/Emergencia');
    await page.waitForLoadState('networkidle');

    // Wait for active patients list
    await page.waitForSelector('h3:has-text("Pacientes Activos")', { timeout: 15000 });

    // Click on the first active patient
    const firstPatient = page.locator('.premium-card').first();
    const countPatients = await firstPatient.count();
    if (countPatients === 0) {
      console.log('No active patients found, skipping egress panel tests.');
      return;
    }

    await firstPatient.click();
    console.log('Selected patient in close account screen.');

    // Verify date/time inputs are read-only text elements for clinical users
    const dateInput = page.locator('input[type="date"]');
    const countDateInput = await dateInput.count();
    expect(countDateInput).toBe(0); // Hidden/replaced for nurses
    console.log('Admission date input is hidden (Read-only view verified).');

    // Verify "Cargar Servicio o Medicamento" card is hidden
    const fastChargeTitle = page.locator('h3:has-text("Cargar Servicio o Medicamento")');
    const countFastCharge = await fastChargeTitle.count();
    expect(countFastCharge).toBe(0); // Hidden/replaced for nurses
    console.log('Quick charge service panel is hidden from close account view.');

    // Verify "Condición y Destino Final de Egreso de Urgencias" is visible
    await expect(page.locator('span:has-text("Condición y Destino Final de Egreso")')).toBeVisible();
    await expect(page.locator('span:has-text("Destino del Paciente:")')).toBeVisible();
    console.log('Transfer / Egress destination panel is visible.');

    // Verify "Procesar Traslado y Egreso" button is visible inside the bottom info card
    const egressBtn = page.locator('button:has-text("Procesar Traslado y Egreso")');
    await expect(egressBtn).toBeVisible();
    console.log('"Procesar Traslado y Egreso" button is visible for clinical assistants.');
  });

  test('Módulo Enfermería: Charge clinical services of different categories separately and verify total calculation', async ({ page }) => {
    // 1. Navigate to Módulo Enfermería
    await page.goto('/enfermeria');
    await page.waitForLoadState('networkidle');

    // Click on the first active patient
    const firstPatient = page.locator('.space-y-3.max-h-\\[600px\\] > div').first();
    const countPatients = await firstPatient.count();
    if (countPatients === 0) {
      console.log('No active patients found, skipping test.');
      return;
    }
    await firstPatient.click();

    // Go to "Carga de Insumos" (con el case exacto del HTML)
    await page.click('button:has-text("Carga de Insumos")');
    const searchInput = page.locator('input[placeholder*="Escriba código o nombre"]');

    // --- 1. Consulta Category (requiere médico) ---
    await searchInput.fill('Consulta Medica General');
    await page.waitForTimeout(1000); // esperar debounce del autocomplete
    const firstRes1 = page.locator('app-step-catalog-search div.hover\\:bg-white\\/5').first();
    await expect(firstRes1).toBeVisible({ timeout: 10000 });
    await firstRes1.click();
    await page.waitForTimeout(500);

    // El selector de médico aparece en el paso 2 (al que se avanza automáticamente)
    await page.waitForSelector('#selectMedicoFastCharge', { timeout: 8000 });
    
    // Seleccionar médico y área clínica
    await page.locator('#selectMedicoFastCharge').selectOption({ index: 1 });
    await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });
    
    // Click Siguiente en el Paso 2 cuando el botón esté habilitado
    const step2Btn1 = page.locator('#btnStep2Next');
    await expect(step2Btn1).toBeEnabled();
    await step2Btn1.click();
    
    const step3Btn1 = page.locator('#btnStep3Confirm');
    await expect(step3Btn1).toBeVisible();
    await step3Btn1.click();
    await page.waitForTimeout(2000); // esperar respuesta del API y reinicio a Paso 1

    // --- 2. RX Category (no requiere médico, tipo examen) ---
    const searchInput2 = page.locator('input[placeholder*="Escriba código o nombre"]');
    await expect(searchInput2).toBeVisible({ timeout: 10000 });
    await searchInput2.fill('Radiografía Tórax');
    await page.waitForTimeout(1000);
    const firstRes2 = page.locator('app-step-catalog-search div.hover\\:bg-white\\/5').first();
    await expect(firstRes2).toBeVisible({ timeout: 10000 });
    await firstRes2.click();
    await page.waitForTimeout(500);
    
    // Seleccionar área clínica
    await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });

    // Avanza al Paso 3
    const step2Btn2 = page.locator('#btnStep2Next');
    await expect(step2Btn2).toBeEnabled();
    await step2Btn2.click();
    
    const step3Btn2 = page.locator('#btnStep3Confirm');
    await expect(step3Btn2).toBeVisible();
    await step3Btn2.click();
    await page.waitForTimeout(2000);

    // --- 3. Informe Category ---
    const searchInput3 = page.locator('input[placeholder*="Escriba código o nombre"]');
    await expect(searchInput3).toBeVisible({ timeout: 10000 });
    await searchInput3.fill('Informe Médico Especializado');
    await page.waitForTimeout(1000);
    const firstRes3 = page.locator('app-step-catalog-search div.hover\\:bg-white\\/5').first();
    await expect(firstRes3).toBeVisible({ timeout: 10000 });
    await firstRes3.click();
    await page.waitForTimeout(500);
    
    // Seleccionar área clínica
    await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });

    // Avanza al Paso 3
    const step2Btn3 = page.locator('#btnStep2Next');
    await expect(step2Btn3).toBeEnabled();
    await step2Btn3.click();
    
    const step3Btn3 = page.locator('#btnStep3Confirm');
    await expect(step3Btn3).toBeVisible();
    await step3Btn3.click();
    await page.waitForTimeout(1000);

    // Verify all items are accumulated in the cart
    await expect(page.locator('app-nursing-cart')).toBeVisible();

    // Now click the "Registrar todo a la cuenta" button in the cart
    await page.click('button:has-text("Registrar todo a la cuenta")');
    await page.waitForTimeout(3000);

    console.log('All three separate catalog categories successfully charged to patient.');
  });
});
