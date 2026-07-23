import { Page, expect } from '@playwright/test';

export class TrasladoPage {
  readonly page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  async login(user = 'user_emergencia', pass = 'Hospital2026*!'): Promise<void> {
    await this.page.goto('/login');
    await this.page.fill('input[type="text"]', user);
    await this.page.fill('input[type="password"]', pass);
    await this.page.click('button[type="submit"]');
    await this.page.waitForURL(url => !url.href.includes('/login'), { timeout: 15000 });
  }

  async gotoEnfermeria(): Promise<void> {
    if (!this.page.url().includes('/enfermeria')) {
      await this.page.goto('/enfermeria');
    }
    await this.page.waitForTimeout(1000);
  }

  async selectFirstPatient(): Promise<boolean> {
    const patientCards = this.page.locator('.space-y-3.max-h-\\[600px\\] div[class*="cursor-pointer"]');
    const count = await patientCards.count();

    if (count > 0) {
      console.log(`[E2E TRASLADO] Selected active patient card (${count} found).`);
      await patientCards.first().click();
      await this.page.waitForTimeout(1000);
      return true;
    }

    console.log('[E2E TRASLADO] No active patient cards found.');
    return false;
  }

  async openTransferTab(): Promise<void> {
    await this.page.click('button:has-text("Traslados y Destino")');
    await this.page.waitForTimeout(1000);
  }

  async selectCambioCamaMode(): Promise<void> {
    const btn = this.page.locator('button').filter({ hasText: 'Cambio de Cama' }).first();
    if (await btn.isVisible()) {
      await btn.click();
    }
  }

  async selectTrasladoAreaMode(): Promise<void> {
    const btn = this.page.locator('button').filter({ hasText: 'Traslado de Área' }).first();
    if (await btn.isVisible()) {
      await btn.click();
    }
  }
}
