import { test, expect } from '@playwright/test';
import { loginAs } from './helpers/auth.helper';

// ─── Helpers ─────────────────────────────────────────────────────────────────
/**
 * Espera a que un <select> tenga al menos `minOptions` opciones (incluyendo placeholder).
 * Más robusto que buscar por texto exacto — no depende de datos seed específicos.
 */
async function waitForSelectToLoad(page: any, cssSelector: string, minOptions = 2, timeout = 15_000) {
  await page.waitForFunction(
    ({ sel, min }: { sel: string; min: number }) => {
      const el = document.querySelector(sel) as HTMLSelectElement | null;
      return el ? el.options.length >= min : false;
    },
    { sel: cssSelector, min: minOptions },
    { timeout }
  );
}

test.describe('Billing Wizard Stabilization', () => {
  test('Auto-Add Consultation and Sequential Suggestions Flow', async ({ page }) => {
    page.on('console', msg => console.log('PAGE LOG:', msg.text()));

    // 1. Authenticate as Admin (helper centralizado — más robusto que networkidle)
    await loginAs(page, 'admin', 'Admin123*!');
    console.log('Logged in successfully, navigated to dashboard.');

    // 2. Navigate to billing page with particular type
    await page.goto('/facturacion?type=Particular');
    await page.waitForLoadState('domcontentloaded');

    // 3. Esperar carga asíncrona del dropdown de especialidades (≥2 opciones)
    await waitForSelectToLoad(page, 'app-service-catalog select', 2);
    console.log('Specialty options loaded.');

    const specialtySelect = page.locator('app-service-catalog select').first();

    // Intentar seleccionar GINECOLOGÍA; si no existe → primer ítem disponible
    const gineOption = specialtySelect.locator('option').filter({ hasText: /GINECOL/i });
    if (await gineOption.count() > 0) {
      const label = (await gineOption.first().textContent())!.trim();
      await specialtySelect.selectOption({ label });
      console.log(`Selected specialty: ${label}`);
    } else {
      await specialtySelect.selectOption({ index: 1 });
      console.log('Selected first available specialty (GINECOLOGÍA not found in seed).');
    }

    // 4. Esperar carga del dropdown de médicos tras seleccionar especialidad
    // 4. Esperar carga del dropdown de médicos (segundo <select> dentro de app-service-catalog)
    await page.waitForFunction(
      () => {
        const selects = document.querySelectorAll('app-service-catalog select');
        const docSel = selects[1] as HTMLSelectElement | undefined;
        return !!docSel && docSel.options.length >= 2;
      },
      { timeout: 15_000 }
    );
    console.log('Doctor options loaded.');

    const doctorSelect = page.locator('app-service-catalog select').nth(1);

    // Intentar seleccionar LISA CUDDY; si no existe → primer médico disponible
    const cuddyOption = doctorSelect.locator('option').filter({ hasText: /CUDDY/i });
    if (await cuddyOption.count() > 0) {
      const label = (await cuddyOption.first().textContent())!.trim();
      await doctorSelect.selectOption({ label });
      console.log(`Selected doctor: ${label}`);
    } else {
      await doctorSelect.selectOption({ index: 1 });
      console.log('Selected first available doctor (LISA CUDDY not found in seed).');
    }

    // 5. Open Appointment Scheduler
    await page.click('button:has-text("AGENDAR CITA / SOLICITAR TURNO")');
    console.log('Opened appointment scheduler modal.');

    // Wait for the modal to be visible
    const modalHeader = page.locator('text=Agenda:');
    await expect(modalHeader).toBeVisible({ timeout: 10_000 });

    // 6. Select the first available slot (AGREGAR)
    const addSlotBtn = page.locator('button:has-text("AGREGAR")').first();
    await expect(addSlotBtn).toBeVisible({ timeout: 10_000 });
    await addSlotBtn.click();
    console.log('Clicked first available slot AGREGAR button.');

    // 7. Verify the Suggestions Modal pops up
    const suggestionsHeader = page.locator('text=Servicios Referidos');
    await expect(suggestionsHeader).toBeVisible({ timeout: 15_000 });
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

    await expect(cartConsultation).toBeVisible({ timeout: 5_000 });
    await expect(cartCitologia).toBeVisible({ timeout: 5_000 });
    await expect(cartEcoGinecologico).not.toBeVisible();

    console.log('E2E validation successful: cart contains Consultation and Citologia, but not Eco Ginecologico.');
  });
});
