import { Page } from '@playwright/test';

/**
 * Helper centralizado de autenticación para todos los specs E2E.
 * Usa waitForSelector en lugar de waitForLoadState('networkidle')
 * para mayor estabilidad ante SignalR y polling de notificaciones.
 */
export async function loginAs(
  page: Page,
  username: string,
  password: string
): Promise<void> {
  await page.goto('/');
  // Esperar el formulario de login — más estable que networkidle
  await page.waitForSelector('input#username', { state: 'visible', timeout: 20_000 });
  await page.fill('input#username', username);
  await page.fill('input#password', password);
  await page.click('button[type="submit"]');
  await page.waitForURL(
    (url) =>
      url.pathname.includes('dashboard') ||
      url.pathname.includes('cierre-cuenta') ||
      url.pathname.includes('enfermeria'),
    { timeout: 20_000 }
  );
  console.log(`[AUTH] Logged in as '${username}' → ${page.url()}`);
}
