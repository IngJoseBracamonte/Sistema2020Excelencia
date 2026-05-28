import { test, expect } from '@playwright/test';

test.describe('New Features Integrity Tests', () => {
  test.beforeEach(async ({ page }) => {
    // 1. Authenticate as Admin
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await page.fill('input#username', 'admin');
    await page.fill('input#password', 'Admin123*!');
    await page.click('button[type="submit"]');

    // Wait for redirect to dashboard
    await page.waitForURL('**/dashboard');
    console.log('Logged in successfully, navigated to dashboard.');
  });

  test('Cuentas por Auditar: Ver Detalles de Factura should toggle accordion row and display concepts/payments', async ({ page }) => {
    // 1. Navigate to Auditing panel
    await page.goto('/admin/audit/cuentas');
    await page.waitForLoadState('networkidle');

    // Set the start date filter back to include seeded records
    const desdeInput = page.locator('input[type="date"]').first();
    await desdeInput.fill('2026-05-25');
    
    // Click the FILTRAR button
    await page.click('button:has-text("FILTRAR")');
    console.log('Filled date filter and clicked FILTRAR.');

    // Wait for the table/loading to finish
    await page.waitForSelector('table');

    // Wait for at least one receivable row to be loaded
    const rowSelector = 'tbody tr.group\\/row';
    await page.waitForSelector(rowSelector, { timeout: 10000 });
    console.log('Receivables rows are visible.');

    // Click "Ver Detalles de Factura" (first button in Actions cell)
    const detailBtn = page.locator('button[title="Ver Detalles de Factura"]').first();
    await expect(detailBtn).toBeVisible();
    await detailBtn.click();
    console.log('Clicked "Ver Detalles de Factura" button.');

    // Assert that the accordion detail row becomes visible
    const detailRow = page.locator('text=Conceptos Facturados').first();
    await expect(detailRow).toBeVisible({ timeout: 10000 });
    console.log('Accordion expanded successfully, "Conceptos Facturados" section is visible.');

    const paymentsRow = page.locator('text=Historial de Abonos Registrados').first();
    await expect(paymentsRow).toBeVisible();
    console.log('"Historial de Abonos Registrados" section is visible.');
  });

  test('Expediente de Facturacion: Filter by "Con Compromiso" and reprint action buttons', async ({ page }) => {
    // 1. Navigate to Expediente de Facturación
    await page.goto('/expediente-facturacion');
    await page.waitForLoadState('networkidle');

    // Set start date to include seeded records
    const desdeInput = page.locator('input[type="date"]').first();
    await desdeInput.fill('2026-05-25');

    // Click "Filtrar ahora" button
    await page.click('button:has-text("Filtrar ahora")');
    console.log('Filled date filter and clicked Filtrar ahora.');

    // Wait for table
    await page.waitForSelector('table');

    // Verify "Con Compromiso" checkbox is visible
    const soloCompromisoCheckbox = page.locator('input#soloCompromiso');
    await expect(soloCompromisoCheckbox).toBeVisible();
    console.log('"Con Compromiso" checkbox filter is visible.');

    // Toggle the checkbox
    await soloCompromisoCheckbox.check();
    console.log('Checked "Con Compromiso" checkbox.');

    // Wait for table to refresh
    await page.waitForTimeout(2000);

    // If there are records in the list, verify reprint buttons are present
    const reprintCompromisoBtn = page.locator('button[title="Reimprimir Compromiso de Pago"]');
    const countCompromiso = await reprintCompromisoBtn.count();
    console.log(`Found ${countCompromiso} "Reimprimir Compromiso de Pago" buttons.`);

    const reprintGarantiaBtn = page.locator('button[title="Reimprimir Garantía de Pago"]');
    const countGarantia = await reprintGarantiaBtn.count();
    console.log(`Found ${countGarantia} "Reimprimir Garantía de Pago" buttons.`);

    if (countCompromiso > 0) {
      await expect(reprintCompromisoBtn.first()).toBeEnabled();
      console.log('First "Reimprimir Compromiso de Pago" button is enabled.');
    }
    if (countGarantia > 0) {
      await expect(reprintGarantiaBtn.first()).toBeEnabled();
      console.log('First "Reimprimir Garantía de Pago" button is enabled.');
    }

    // Uncheck "Con Compromiso" checkbox
    await soloCompromisoCheckbox.uncheck();
    console.log('Unchecked "Con Compromiso" checkbox.');
    await page.waitForTimeout(1000);
  });
});
