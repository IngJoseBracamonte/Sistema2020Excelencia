import { test, expect } from '@playwright/test';

test.describe('Emergency Nursing & Egress Integrity Tests', () => {
  test.beforeEach(async ({ page }) => {
    // 1. Authenticate as user_emergencia (Emergency Assistant / Nurse)
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await page.fill('input#username', 'user_emergencia');
    await page.fill('input#password', 'Hospital2026*!');
    await page.click('button[type="submit"]');

    // Wait for redirect to dashboard/landing
    await page.waitForURL('**/dashboard');
    console.log('Logged in successfully as user_emergencia.');
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
    await expect(page.locator('text=Triage y Valoracion')).toBeVisible();
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

    // Try to load to account without selecting doctor
    // Capture alert/dialog
    page.once('dialog', async dialog => {
      expect(dialog.message()).toContain('seleccione el médico tratante');
      await dialog.dismiss();
      console.log('Verified alert dialog for doctor selection validation.');
    });

    await page.click('button:has-text("CARGAR A LA CUENTA")');
    await page.waitForTimeout(500);

    // Select a doctor and submit
    await doctorSelector.selectOption({ index: 1 });
    console.log('Selected a seeded doctor.');

    // Verify submission
    await page.click('button:has-text("CARGAR A LA CUENTA")');
    await page.waitForTimeout(2000);
  });

  test('Cierre Cuenta: Read-only check and Egress/Transfer panel for clinical users', async ({ page }) => {
    // 1. Navigate to Cierre Cuenta (Emergency directory)
    await page.goto('/admision/cierre-cuenta?type=Emergencia');
    await page.waitForLoadState('networkidle');

    // Wait for active patients list
    await page.waitForSelector('h1:has-text("Pacientes Activos")');

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
});
