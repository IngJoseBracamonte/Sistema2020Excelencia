import { Page } from '@playwright/test';

export class TrasladoPage {
  readonly page: Page;

  constructor(page: Page) {
    this.page = page;
  }

  /** Autenticación con URL check permisivo (sale de /login sin importar la ruta destino) */
  async login(user = 'user_emergencia', pass = 'Hospital2026*!'): Promise<void> {
    await this.page.goto('/');
    await this.page.waitForSelector('input#username', { state: 'visible', timeout: 20_000 });
    await this.page.fill('input#username', user);
    await this.page.fill('input#password', pass);
    await this.page.click('button[type="submit"]');
    // Permisivo: solo verificar que se abandonó /login (user_emergencia puede ir a /monitoreo u otras rutas)
    await this.page.waitForURL((url) => !url.href.includes('/login'), { timeout: 20_000 });
    console.log(`[AUTH TrasladoPage] Logged in as '${user}' → ${this.page.url()}`);
  }

  async gotoEnfermeria(): Promise<void> {
    if (!this.page.url().includes('/enfermeria')) {
      await this.page.goto('/enfermeria');
    }
    // Esperar a que aparezca el panel de pacientes activos
    await this.page.waitForSelector('h3:has-text("Pacientes Activos")', {
      state: 'visible',
      timeout: 15_000,
    });
  }

  /**
   * Selecciona el primer paciente activo disponible.
   * Espera hasta 5 segundos antes de concluir que no hay pacientes.
   * @returns `true` si encontró y seleccionó un paciente, `false` si no hay pacientes activos.
   */
  async selectFirstPatient(): Promise<boolean> {
    const patientCards = this.page.locator(
      '.space-y-3.max-h-\\[600px\\] div[class*="cursor-pointer"]'
    );

    try {
      // Esperar hasta 5s a que aparezca al menos una tarjeta de paciente
      await patientCards.first().waitFor({ state: 'visible', timeout: 5_000 });
      const count = await patientCards.count();
      if (count > 0) {
        console.log(`[E2E TRASLADO] Selected active patient card (${count} found).`);
        await patientCards.first().click();
        // Esperar a que cargue el detalle del paciente
        await this.page.waitForTimeout(1_000);
        return true;
      }
    } catch {
      // No hay pacientes visibles en el tiempo de espera
    }

    console.log('[E2E TRASLADO] No active patient cards found.');
    return false;
  }

  async openTransferTab(): Promise<void> {
    await this.page.click('button:has-text("Traslados y Destino")');
    await this.page.waitForTimeout(800);
  }

  async selectCambioCamaMode(): Promise<void> {
    const btn = this.page.locator('button').filter({ hasText: /Cambio de Cama/i }).first();
    if (await btn.isVisible()) {
      await btn.click();
      await this.page.waitForTimeout(500);
    }
  }

  async selectTrasladoAreaMode(): Promise<void> {
    const btn = this.page.locator('button').filter({ hasText: /Traslado de Área/i }).first();
    if (await btn.isVisible()) {
      await btn.click();
      await this.page.waitForTimeout(500);
    }
  }
}
