import { test, expect } from '@playwright/test';

test.describe('Billing Wizard Stabilization', () => {
  test('Auto-Add Consultation and Sequential Suggestions Flow', async ({ page }) => {
    page.on('console', msg => console.log('PAGE LOG:', msg.text()));
    // 1. Authenticate as Admin
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await page.fill('input#username', 'admin');
    await page.fill('input#password', 'Admin123*!');
    await page.click('button[type="submit"]');

    // Wait for redirect to dashboard
    await page.waitForURL('**/dashboard');
    console.log('Logged in successfully, navigated to dashboard.');

    // 2. Navigate to billing page with particular type
    await page.goto('/facturacion?type=Particular');
    await page.waitForLoadState('networkidle');

    // 3. Select Ginecología Specialty
    const specialtySelect = page.locator('select').first();
    await specialtySelect.selectOption({ label: 'GINECOLOGÍA' });
    console.log('Selected specialty: Ginecología');

    // 4. Select Doctor Lisa Cuddy
    const doctorSelect = page.locator('select').nth(1);
    await doctorSelect.selectOption({ label: 'LISA CUDDY' });
    console.log('Selected doctor: Lisa Cuddy');

    // 5. Open Appointment Scheduler
    await page.click('button:has-text("AGENDAR CITA / SOLICITAR TURNO")');
    console.log('Opened appointment scheduler modal.');

    // Wait for the modal to be visible
    const modalHeader = page.locator('text=Agenda: LISA CUDDY');
    await expect(modalHeader).toBeVisible({ timeout: 10000 });

    // 6. Select the first available slot (AGREGAR)
    const addSlotBtn = page.locator('button:has-text("AGREGAR")').first();
    await expect(addSlotBtn).toBeVisible({ timeout: 10000 });
    await addSlotBtn.click();
    console.log('Clicked first available slot AGREGAR button.');

    // 7. Verify the Suggestions Modal pops up
    const suggestionsHeader = page.locator('text=Servicios Referidos');
    await expect(suggestionsHeader).toBeVisible({ timeout: 15000 });
    console.log('Suggestions modal is visible.');

    // Verify both Citologia and Eco Ginecologico suggestions are listed
    const citologiaRow = page.locator('.fixed.z-\\[200\\]').locator('text=Citologia');
    const ecoGinecologicoRow = page.locator('.fixed.z-\\[200\\]').locator('text=Eco Ginecologico');
    await expect(citologiaRow).toBeVisible();
    await expect(ecoGinecologicoRow).toBeVisible();
    console.log('Both CITOLOGIA and ECO GINECOLOGICO suggestion rows are visible.');

    // Toggle (Uncheck) ECO GINECOLOGICO to verify checklist functionality
    await ecoGinecologicoRow.click();
    console.log('Unchecked ECO GINECOLOGICO.');

    // Click "Agregar seleccionados"
    await page.click('button:has-text("Agregar seleccionados")');
    console.log('Clicked "Agregar seleccionados".');

    // Wait for Suggestions modal to disappear
    await expect(suggestionsHeader).not.toBeVisible();

    // 8. Verify the Cart contains: CONSULTA GINECOLOGICA and CITOLOGIA, but NOT ECO GINECOLOGICO
    const cart = page.locator('app-billing-cart');
    await expect(cart).toBeVisible();

    const cartConsultation = cart.locator('text=CONSULTA GINECOLOGICA');
    const cartCitologia = cart.locator('text=CITOLOGIA');
    const cartEcoGinecologico = cart.locator('text=ECO GINECOLOGICO');

    await expect(cartConsultation).toBeVisible({ timeout: 5000 });
    await expect(cartCitologia).toBeVisible({ timeout: 5000 });
    await expect(cartEcoGinecologico).not.toBeVisible();

    console.log('E2E validation successful: cart contains Consultation and Citologia, but not Eco Ginecologico.');
  });
});
