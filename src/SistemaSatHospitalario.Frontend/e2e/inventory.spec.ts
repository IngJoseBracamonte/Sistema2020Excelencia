import { test, expect } from '@playwright/test';

test.describe('Inventory Multi-Sede E2E Tests', () => {

  test('Should create a new Sede and associate an Area Clinica to it', async ({ page }) => {
    const timestamp = Date.now();
    const codigoSede = `E2ES${timestamp}`;
    const nombreSede = `Sede E2E ${timestamp}`;
    const codigoArea = `E2EA${timestamp}`;
    const nombreArea = `Area E2E ${timestamp}`;

    // 1. Autenticarse y navegar al módulo de Inventario / Sedes
    await page.goto('/login');
    await page.getByLabel('Usuario').fill('admin');
    await page.getByLabel('Contraseña').fill('Admin123*!');
    await page.getByRole('button', { name: 'Ingresar al Sistema' }).click();
    await page.waitForURL('**/dashboard');

    await page.goto('/inventario/sedes-areas');

    // 2. Abrir modal/formulario para Nueva Sede
    await page.getByRole('button', { name: /nueva sede|crear sede/i }).click();

    // 3. Completa los datos de la Sede
    const sedeModal = page.locator('div.fixed.inset-0').filter({ has: page.getByRole('heading', { name: 'Nueva Sede' }) });
    const sedeInputs = sedeModal.locator('input:not([type="checkbox"])');
    await sedeInputs.nth(0).fill(codigoSede);
    await sedeInputs.nth(1).fill(nombreSede);
    await sedeModal.getByRole('button', { name: 'Guardar' }).click();

    // 4. Verificar que la Sede se creó correctamente en la tabla/lista
    await expect(page.getByText(nombreSede)).toBeVisible();

    // 5. Seleccionar la tarjeta de la Sede creada y agregar Área Clínica
    const sedeCard = page.locator('div.glass-card').filter({ hasText: nombreSede });
    await sedeCard.getByRole('button', { name: /agregar área/i }).click();

    const areaModal = page.locator('div.fixed.inset-0').filter({ has: page.getByRole('heading', { name: 'Agregar Área Clínica' }) });
    const areaInputs = areaModal.locator('input');
    await areaInputs.nth(0).fill(codigoArea);
    await areaInputs.nth(1).fill(nombreArea);
    await areaModal.getByRole('button', { name: 'Agregar' }).click();

    // 6. Afirmar que el Área Clínica fue asociada a la Sede
    await expect(page.getByText(nombreArea)).toBeVisible();
  });

});