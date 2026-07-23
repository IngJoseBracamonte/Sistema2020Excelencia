import { Page, Locator, expect } from '@playwright/test';

export class TrasladoPage {
  readonly page: Page;
  readonly transferTabButton: Locator;
  readonly cambioCamaButton: Locator;
  readonly trasladoAreaButton: Locator;
  readonly camaDestinoSelect: Locator;
  readonly areaDestinoSelect: Locator;
  readonly cantidadHorasInput: Locator;
  readonly montoACobrarInput: Locator;
  readonly cambiaMedicoCheckbox: Locator;
  readonly nuevoMedicoSelect: Locator;
  readonly observacionTextarea: Locator;
  readonly confirmCambioCamaButton: Locator;
  readonly confirmTrasladoAreaButton: Locator;

  constructor(page: Page) {
    this.page = page;
    this.transferTabButton = page.locator('button:has-text("Traslados y Destino")');
    this.cambioCamaButton = page.locator('button:has-text("Cambio de Cama")');
    this.trasladoAreaButton = page.locator('button:has-text("Traslado de Área")');
    this.camaDestinoSelect = page.locator('select').filter({ hasText: 'SELECCIONAR CAMA' });
    this.areaDestinoSelect = page.locator('select').filter({ hasText: 'Observación de Emergencia' });
    this.cantidadHorasInput = page.locator('input[type="number"][placeholder*="24"]');
    this.montoACobrarInput = page.locator('input[type="number"][placeholder*="600.00"]');
    this.cambiaMedicoCheckbox = page.locator('input[type="checkbox"]').filter({ hasText: '' }).last();
    this.nuevoMedicoSelect = page.locator('select').filter({ hasText: 'SELECCIONAR MÉDICO' });
    this.observacionTextarea = page.locator('textarea[placeholder*="Motivo clínico"]');
    this.confirmCambioCamaButton = page.locator('button:has-text("Confirmar Cambio de Cama")');
    this.confirmTrasladoAreaButton = page.locator('button:has-text("Confirmar Traslado de Área")');
  }

  async login(username = 'user_emergencia', password = 'Hospital2026*!'): Promise<void> {
    await this.page.goto('/');
    await this.page.waitForLoadState('networkidle');
    await this.page.fill('input#username', username);
    await this.page.fill('input#password', password);
    await this.page.click('button[type="submit"]');
    await this.page.waitForURL(url => url.pathname.includes('enfermeria') || url.pathname.includes('dashboard') || url.pathname.includes('cierre-cuenta'), { timeout: 15000 });
  }

  async gotoEnfermeria(): Promise<void> {
    await this.page.goto('/enfermeria');
    await this.page.waitForLoadState('networkidle');
  }

  async selectFirstPatient(): Promise<boolean> {
    await this.page.waitForSelector('h3:has-text("Pacientes Activos")');

    const areaFilters = ['Emergencia', 'Hospitalización', 'UCI'];
    for (const filterText of areaFilters) {
      const filterButton = this.page.locator('button').filter({ hasText: filterText }).first();
      if (await filterButton.isVisible()) {
        await filterButton.click();
        await this.page.waitForTimeout(500);
      }

      const patientCard = this.page.locator('.space-y-3.max-h-\\[600px\\] > div[class*="cursor-pointer"]').first();
      if ((await patientCard.count()) > 0 && (await patientCard.isVisible())) {
        console.log(`[E2E TRASLADO] Selected active patient card in section '${filterText}'.`);
        await patientCard.click();
        await this.page.waitForSelector('h3:has-text("Triage y Signos Vitales")', { timeout: 10000 });
        await this.page.waitForSelector('button:has-text("Traslados y Destino")', { timeout: 10000 });
        return true;
      }
    }

    console.log('[E2E TRASLADO] No active patient cards found in any section.');
    return false;
  }

  async openTransferTab(): Promise<void> {
    await this.transferTabButton.click();
    await this.page.waitForSelector('button:has-text("Cambio de Cama")', { timeout: 10000 });
  }

  async selectCambioCamaMode(): Promise<void> {
    await this.cambioCamaButton.click();
  }

  async selectTrasladoAreaMode(): Promise<void> {
    await this.trasladoAreaButton.click();
  }
}
