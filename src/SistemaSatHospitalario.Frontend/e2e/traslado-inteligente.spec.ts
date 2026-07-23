import { test, expect } from '@playwright/test';
import { TrasladoPage } from './traslado.page';

test.describe('Suite E2E - Módulo de Traslado Inteligente de Pacientes', () => {
  let trasladoPage: TrasladoPage;

  test.beforeEach(async ({ page }) => {
    trasladoPage = new TrasladoPage(page);
    await trasladoPage.login('user_emergencia', 'Hospital2026*!');
  });

  test('E2E Modo 1: Cambio de Cama (Misma Área) - Verifica costo $0 USD', async ({ page }) => {
    await trasladoPage.gotoEnfermeria();
    const hasPatient = await trasladoPage.selectFirstPatient();
    if (!hasPatient) {
      console.log('No active patients found, skipping test.');
      return;
    }

    await trasladoPage.openTransferTab();
    await trasladoPage.selectCambioCamaMode();

    // Verify mode button active styles & text
    await expect(page.locator('button:has-text("Cambio de Cama ($0 USD)")')).toBeVisible();

    // Intercept CambioCama API Request
    const requestPromise = page.waitForRequest(
      request => request.url().includes('/api/Enfermeria/CambioCama') && request.method() === 'POST'
    );

    // Select destination bed if options exist
    const camaSelect = page.locator('select').filter({ hasText: 'SELECCIONAR CAMA' });
    const optionCount = await camaSelect.locator('option').count();

    if (optionCount > 1) {
      await camaSelect.selectOption({ index: 1 });
      await page.locator('button:has-text("Confirmar Cambio de Cama")').click();

      const request = await requestPromise;
      const payload = JSON.parse(request.postData() || '{}');
      console.log('[E2E HTTP PAYLOAD CambioCama]:', payload);

      expect(payload).toHaveProperty('cuentaId');
      expect(payload).toHaveProperty('camaDestinoId');
    }
  });

  test('E2E Modo 2: Traslado de Área (Inter-Área) - Selecciona UCI, modifica tarifa a $550 y aserta payload estricto', async ({ page }) => {
    await trasladoPage.gotoEnfermeria();
    const hasPatient = await trasladoPage.selectFirstPatient();
    if (!hasPatient) {
      console.log('No active patients found, skipping test.');
      return;
    }

    await trasladoPage.openTransferTab();
    await trasladoPage.selectTrasladoAreaMode();

    // 1. Seleccionar Área Destino UCI
    const areaSelect = page.locator('select').filter({ hasText: 'Observación de Emergencia' });
    await areaSelect.selectOption('UCI');

    // 2. Verificar que la tarifa base por defecto asignó $600
    const montoInput = page.locator('input[type="number"][placeholder*="600.00"]');
    await expect(montoInput).toHaveValue('600');

    // 3. Sobreescribir manualmente el monto de $600 a $550 USD
    await montoInput.fill('550');
    await expect(montoInput).toHaveValue('550');

    // 4. Llenar observaciones y horas
    const horasInput = page.locator('input[type="number"][placeholder*="24"]');
    await horasInput.fill('12');

    const obsTextarea = page.locator('textarea[placeholder*="Motivo clínico"]');
    await obsTextarea.fill('Traslado a UCI por monitoreo hemodinámico intensivo.');

    // Interceptar la petición HTTP POST a /api/Enfermeria/TrasladoArea
    const requestPromise = page.waitForRequest(
      request => request.url().includes('/api/Enfermeria/TrasladoArea') && request.method() === 'POST'
    );

    // Seleccionar cama disponible en área destino si existe
    const camaSelect = page.locator('select').filter({ hasText: 'SELECCIONAR CAMA' }).last();
    const optionCount = await camaSelect.locator('option').count();

    if (optionCount > 1) {
      await camaSelect.selectOption({ index: 1 });
      await page.locator('button:has-text("Confirmar Traslado de Área")').click();

      const request = await requestPromise;
      const payload = JSON.parse(request.postData() || '{}');
      console.log('[E2E HTTP PAYLOAD TrasladoArea]:', payload);

      // Asertar que el payload JSON contenga exactamente los campos estrictos
      expect(payload).toHaveProperty('cuentaId');
      expect(payload).toHaveProperty('areaDestino', 'UCI');
      expect(payload).toHaveProperty('camaDestinoId');
      expect(payload).toHaveProperty('cantidadHoras', 12);
      expect(payload).toHaveProperty('cambiaMedicoTratante');
      expect(payload).toHaveProperty('observacion', 'Traslado a UCI por monitoreo hemodinámico intensivo.');
      expect(payload).toHaveProperty('montoACobrarUsd', 550);
    }
  });
});
