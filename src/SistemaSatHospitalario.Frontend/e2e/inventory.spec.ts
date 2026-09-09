
import { test, expect } from '@playwright/test';

test.describe('Inventory Multi-Sede E2E Tests', () => {

  test('Should create a new Sede and associate an Area Clinica to it', async ({ page }) => {
    const timestamp = Date.now();

    const codigoSede = `E2ES${timestamp}`;
    const nombreSede = `Sede E2E ${timestamp}`;

    const codigoArea = `E2EA${timestamp}`;
    const nombreArea = `Area E2E ${timestamp}`;

    // 1. Autenticarse
    await page.goto('/login');

    await page.getByLabel('Usuario').fill('admin');
    await page.getByLabel('Contraseña').fill('Admin123*!');

    await Promise.all([
      page.waitForURL('**/dashboard'),
      page.getByRole('button', {
        name: 'Ingresar al Sistema'
      }).click()
    ]);

    // 2. Navegar a Sedes
    await page.goto('/inventario/sedes-areas');

    // 3. Crear Sede
    await page
      .getByRole('button', {
        name: /nueva sede|crear sede/i
      })
      .click();

    const sedeModal = page
      .locator('div.fixed.inset-0')
      .filter({
        has: page.getByRole('heading', {
          name: 'Nueva Sede'
        })
      });

    const sedeInputs = sedeModal.locator(
      'input:not([type="checkbox"])'
    );

    await sedeInputs.nth(0).fill(codigoSede);
    await sedeInputs.nth(1).fill(nombreSede);

    const createSedeResponse = page.waitForResponse(
      response =>
        response.request().method() === 'POST' &&
        response.ok() &&
        (
          response.url().includes('/Sede') ||
          response.url().includes('/sede')
        )
    );

    await sedeModal
      .getByRole('button', {
        name: 'Guardar'
      })
      .click();

    await createSedeResponse;

    // Verificar Sede creada
    const sedeCard = page
      .locator('div.glass-card')
      .filter({
        hasText: nombreSede
      });

    await expect(sedeCard).toBeVisible({
      timeout: 10000
    });

    // 4. Agregar Área Clínica
    await sedeCard
      .getByRole('button', {
        name: /agregar área/i
      })
      .click();

    const areaModal = page
      .locator('div.fixed.inset-0')
      .filter({
        has: page.getByRole('heading', {
          name: 'Agregar Área Clínica'
        })
      });

    const areaInputs = areaModal.locator('input');

    await areaInputs.nth(0).fill(codigoArea);
    await areaInputs.nth(1).fill(nombreArea);

    // Capturar cualquier POST que ocurra al crear el área
    const areaResponsePromise = page.waitForResponse(
      response =>
        response.request().method() === 'POST' &&
        response.url().toLowerCase().includes('area')
    );

    await areaModal
      .getByRole('button', {
        name: 'Agregar'
      })
      .click();

    const areaResponse = await areaResponsePromise;

    // Si el backend devuelve error, mostrar información útil
    expect(
      areaResponse.ok(),
      `Error creando Área Clínica. HTTP ${areaResponse.status()} - ${areaResponse.url()}`
    ).toBeTruthy();

    // 5. Verificar Área Clínica
    await expect(
      page.getByText(nombreArea, {
        exact: true
      })
    ).toBeVisible({
      timeout: 10000
    });
  });

});
