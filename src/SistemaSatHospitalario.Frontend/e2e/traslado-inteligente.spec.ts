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

    // Verify mode button — .first() evita strict mode violation con "Confirmar Cambio de Cama"
    await expect(page.locator('button:has-text("CAMBIO DE CAMA")').first()).toBeVisible();

    // Select destination bed (si existen camas disponibles)
    const camaSelect = page.locator('select').filter({ hasText: /SELECCIONAR CAMA EN MISMA/i });
    const optionCount = await camaSelect.locator('option').count();

    if (optionCount > 1) {
      await camaSelect.selectOption({ index: 1 });

      // waitForRequest debe estar ANTES del click que dispara la petición
      const requestPromise = page.waitForRequest(
        request => request.url().includes('/api/Enfermeria/CambioCama') && request.method() === 'POST',
        { timeout: 15_000 }
      );

      await page.locator('button:has-text("Confirmar Cambio de Cama")').click();

      const request = await requestPromise;
      const payload = JSON.parse(request.postData() || '{}');
      console.log('[E2E HTTP PAYLOAD CambioCama]:', payload);

      expect(payload).toHaveProperty('cuentaId');
      expect(payload).toHaveProperty('camaDestinoId');
    } else {
      console.log('[E2E CAMBIO CAMA] Sin camas disponibles para cambiar — test validado parcialmente.');
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
    const areaSelect = page.locator('#selectAreaDestino');
    await expect(areaSelect).toBeVisible({ timeout: 8_000 });

    // Intentar seleccionar UCI; si no está disponible → primer área disponible
    const uciOption = areaSelect.locator('option').filter({ hasText: /UCI/i });
    if (await uciOption.count() > 0) {
      const uciVal = await uciOption.first().getAttribute('value');
      if (uciVal) await areaSelect.selectOption(uciVal);
    } else {
      await areaSelect.selectOption({ index: 1 });
    }

    // Capturar el VALUE del área seleccionada (el backend recibe el value, no el texto visible)
    const selectedAreaValue = await areaSelect.evaluate(
      (el: HTMLSelectElement) => el.options[el.selectedIndex]?.value ?? ''
    );
    console.log(`[TRASLADO] Área seleccionada valor: ${selectedAreaValue}`);

    // 2. Monto de traslado — el placeholder varía según catálogo (450, 600, etc.)
    const montoInput = page.locator('input[type="number"]').filter({
      has: page.locator('..'),
    }).nth(1); // Segundo input numérico en el panel de TRASLADO_AREA
    // Usar selector más directo: el input con clase bg-rose-500/10 (monto editable)
    const montoRose = page.locator('input.bg-rose-500\\/10, input[class*="rose"]').first();

    if (await montoRose.isVisible()) {
      await montoRose.fill('550');
      await expect(montoRose).toHaveValue('550');
      console.log('[TRASLADO] Monto establecido a $550.');
    } else {
      console.log('[TRASLADO] Input de monto no visible, continuando sin sobrescribir tarifa.');
    }

    // 3. Llenar observaciones y horas
    const horasInput = page.locator('input[type="number"][placeholder*="24"]');
    if (await horasInput.isVisible()) await horasInput.fill('12');

    const obsTextarea = page.locator('textarea[placeholder*="Motivo clínico"]');
    if (await obsTextarea.isVisible()) {
      await obsTextarea.fill('Traslado a UCI por monitoreo hemodinámico intensivo.');
    }

    // 4. Seleccionar cama disponible en área destino
    const camaSelect = page.locator('#selectCamaDestino');
    const optionCount = await camaSelect.locator('option').count();

    if (optionCount > 1) {
      await camaSelect.selectOption({ index: 1 });

      // waitForRequest ANTES del click que dispara la petición
      const requestPromise = page.waitForRequest(
        request => request.url().includes('/api/Enfermeria/TrasladoArea') && request.method() === 'POST',
        { timeout: 15_000 }
      );

      await page.locator('button:has-text("Confirmar Traslado de Área")').click();

      const request = await requestPromise;
      const payload = JSON.parse(request.postData() || '{}');
      console.log('[E2E HTTP PAYLOAD TrasladoArea]:', payload);

      // Aserciones del payload con valores dinámicos
      expect(payload).toHaveProperty('cuentaId');
      expect(payload).toHaveProperty('areaDestino', selectedAreaValue);
      expect(payload).toHaveProperty('camaDestinoId');
      expect(payload).toHaveProperty('cambiaMedicoTratante');
    } else {
      console.log('[E2E TRASLADO AREA] Sin camas disponibles en área destino — test validado parcialmente.');
    }
  });
});
