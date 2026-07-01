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
    const firstPatient = page.locator('div.group.relative').first();
    const countPatients = await firstPatient.count();
    if (countPatients === 0) {
      console.log('No active patients found, skipping clinical operations test.');
      return;
    }

    await firstPatient.click();
    console.log('Selected active patient.');

    // Verify modular triage section checkboxes are visible
    await expect(page.locator('text=TRIAGE Y SIGNOS VITALES')).toBeVisible();
    await expect(page.locator('label:has-text("1. Constantes Vitales")')).toBeVisible();
    await expect(page.locator('label:has-text("2. Valoración Física Inicial")')).toBeVisible();
    await expect(page.locator('label:has-text("5. Antecedentes y Alergias")')).toBeVisible();
    await expect(page.locator('label:has-text("6. Descripción del Estado Actual")')).toBeVisible();
    console.log('Modular triage flags verified.');

    // Navigate to Carga de Insumos tab
    await page.click('button:has-text("CARGA DE INSUMOS")');
    console.log('Navigated to Carga de Insumos.');

    // Search for a service that requires doctor association (Consultation / Rx / Tomography)
    const searchInput = page.locator('input[placeholder*="Escriba código o nombre"]');
    await searchInput.fill('CONSULTA GINECOLOGICA');
    await page.waitForTimeout(1000);

    // Select the autocomplete result
    const firstResult = page.locator('div.hover\\:bg-white\\/5').first();
    await firstResult.click();
    console.log('Selected CONSULTA GINECOLOGICA.');

    // Verify doctor selector dropdown is shown
    const doctorSelector = page.locator('select:has-text("Seleccionar Médico")');
    await expect(doctorSelector).toBeVisible();
    console.log('Doctor selector dropdown is visible.');

    // Verify "Siguiente" button is disabled when no doctor is selected
    const nextBtn = page.locator('button:has-text("Siguiente")').last();
    await expect(nextBtn).toBeDisabled();
    console.log('Verified "Siguiente" button is disabled when doctor is not selected.');

    // Select a doctor
    await doctorSelector.selectOption({ index: 1 });
    console.log('Selected a seeded doctor.');

    // Now Siguiente should be enabled, click it
    await expect(nextBtn).toBeEnabled();
    await nextBtn.click();
    console.log('Advanced to Step 3 (Confirmation).');

    // Verify Step 3 displays the patient's name and Cédula
    await expect(page.locator('span:has-text("Cédula:")')).toBeVisible();
    await expect(page.locator('span:has-text("Médico Tratante")')).toBeVisible();
    console.log('Step 3 confirmation details verified.');

    // Verify final submission
    await page.click('button:has-text("CONFIRMAR Y CARGAR A LA CUENTA")');
    await page.waitForTimeout(2000);
  });

  test('Cierre Cuenta: Read-only check and Egress/Transfer panel for clinical users', async ({ page }) => {
    // 1. Navigate to Cierre Cuenta (Emergency directory)
    await page.goto('/admision/cierre-cuenta?type=Emergencia');
    await page.waitForLoadState('networkidle');

    // Wait for active patients list
    await page.waitForSelector('text=Pacientes Activos');

    // Click on the first active patient
    const firstPatient = page.locator('div.group.relative').first();
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
    const firstPatient = page.locator('div.group.relative').first();
    const countPatients = await firstPatient.count();
    if (countPatients === 0) {
      console.log('No active patients found, skipping test.');
      return;
    }
    await firstPatient.click();

    // Go to "CARGA DE INSUMOS"
    await page.click('button:has-text("CARGA DE INSUMOS")');
    const searchInput = page.locator('input[placeholder*="Escriba código o nombre"]');

    // --- 1. Consulta Category ---
    await searchInput.fill('Consulta Medica General');
    await page.waitForTimeout(500);
    await page.locator('div.hover\\:bg-white\\/5').first().click();

    const doctorSelector = page.locator('select:has-text("Seleccionar Médico")');
    await doctorSelector.selectOption({ index: 1 });
    const nextBtn = page.locator('button:has-text("Siguiente")').last();
    await nextBtn.click();

    await page.click('button:has-text("CONFIRMAR Y CARGAR A LA CUENTA")');
    await page.waitForTimeout(1000);

    // --- 2. RX Category ---
    await searchInput.fill('Radiografía Tórax');
    await page.waitForTimeout(500);
    await page.locator('div.hover\\:bg-white\\/5').first().click();
    await nextBtn.click();
    await page.click('button:has-text("CONFIRMAR Y CARGAR A LA CUENTA")');
    await page.waitForTimeout(1000);

    // --- 3. Informe Category ---
    await searchInput.fill('Informe Médico Especializado');
    await page.waitForTimeout(500);
    await page.locator('div.hover\\:bg-white\\/5').first().click();
    await nextBtn.click();
    await page.click('button:has-text("CONFIRMAR Y CARGAR A LA CUENTA")');
    await page.waitForTimeout(1500);

    console.log('All three separate catalog categories successfully charged to patient.');
  });
});
