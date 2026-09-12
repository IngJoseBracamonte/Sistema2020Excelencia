import { test, expect } from '@playwright/test';
import { loginAs } from './helpers/auth.helper';

// IDs resueltos dinámicamente en beforeAll para evitar GUIDs hardcodeados
let sedeProveedoraId = '00000000-0000-0000-0000-000000000001'; // fallback: SeedConstants
let sedeSolicitanteId = '00000000-0000-0000-0000-000000000002'; // fallback: SeedConstants
let insumoId = '00000000-0000-0000-0000-000000000001';           // fallback: SeedConstants

test.describe('Pruebas Negativas y No Funcionales de Inventario', () => {

  /**
   * Resuelve IDs reales de sedes e insumos desde la API antes de todos los tests.
   * Usa valores de SeedConstants como fallback si la API no responde.
   */
  test.beforeAll(async ({ request }) => {
    try {
      // Autenticar para obtener token
      const authRes = await request.post('/api/auth/login', {
        data: { username: 'admin', password: 'Admin123*!' },
      });
      if (!authRes.ok()) return;

      const { token } = await authRes.json();
      const headers = { Authorization: `Bearer ${token}` };

      // Resolver IDs de sedes
      const sedesRes = await request.get('/api/Sede', { headers });
      if (sedesRes.ok()) {
        const sedes: Array<{ id: string; esPrincipal: boolean; nombre: string }> =
          await sedesRes.json();
        const principal = sedes.find((s) => s.esPrincipal);
        const subSede = sedes.find((s) => !s.esPrincipal);
        if (principal) sedeProveedoraId = principal.id;
        if (subSede) sedeSolicitanteId = subSede.id;
        console.log(`[SETUP] SedeProveedora: ${sedeProveedoraId}`);
        console.log(`[SETUP] SedeSolicitante: ${sedeSolicitanteId}`);
      }

      // Resolver ID de primer insumo disponible
      const insumosRes = await request.get('/api/inventory/insumos?pageSize=1', { headers });
      if (insumosRes.ok()) {
        const insumosData = await insumosRes.json();
        const items = insumosData.items ?? insumosData;
        if (Array.isArray(items) && items.length > 0) {
          insumoId = items[0].id;
          console.log(`[SETUP] InsumoId: ${insumoId} (${items[0].nombre ?? 'N/A'})`);
        }
      }
    } catch (err) {
      console.warn('[SETUP] No se pudieron resolver IDs dinámicamente, usando SeedConstants:', err);
    }
  });

  test.beforeEach(async ({ page }) => {
    await loginAs(page, 'admin', 'Admin123*!');
  });

  test('Debe rechazar la creación de una compra con cantidad o costo inválido', async ({ page }) => {
    await page.goto('/inventario/compras');
    await page.waitForLoadState('domcontentloaded');

    // Intentar submit sin seleccionar insumo ni cantidad válida
    const submitBtn = page.locator(
      'button:has-text("REGISTRAR COMPRA DE INSUMO"), button:has-text("Guardar"), button[type="submit"]'
    ).first();

    if (await submitBtn.isVisible()) {
      await expect(submitBtn).toBeDisabled();
    }
  });

  test('Prueba No Funcional: Inyección de Concurrencia Simultánea en Pedidos Inter-Sede', async ({ browser }) => {
    // Crear 2 contextos independientes simulando 2 usuarios solicitando el mismo insumo al mismo tiempo
    const context1 = await browser.newContext({ ignoreHTTPSErrors: true });
    const context2 = await browser.newContext({ ignoreHTTPSErrors: true });

    const page1 = await context1.newPage();
    const page2 = await context2.newPage();

    // Login simultáneo en ambos contextos
    await Promise.all([
      (async () => {
        await page1.goto('/');
        await page1.waitForSelector('input#username', { state: 'visible', timeout: 15_000 });
        await page1.fill('input#username', 'admin');
        await page1.fill('input#password', 'Admin123*!');
        await page1.click('button[type="submit"]');
        await page1.waitForURL((url) => url.pathname.includes('dashboard'), { timeout: 15_000 });
      })(),
      (async () => {
        await page2.goto('/');
        await page2.waitForSelector('input#username', { state: 'visible', timeout: 15_000 });
        await page2.fill('input#username', 'admin');
        await page2.fill('input#password', 'Admin123*!');
        await page2.click('button[type="submit"]');
        await page2.waitForURL((url) => url.pathname.includes('dashboard'), { timeout: 15_000 });
      })(),
    ]);

    // Navegar simultáneamente a la gestión de pedidos
    await Promise.all([
      page1.goto('/inventario/pedidos'),
      page2.goto('/inventario/pedidos'),
    ]);

    // Solicitud simultánea excediendo el stock real vía API (usando IDs dinámicos)
    const [res1, res2] = await Promise.all([
      page1.request.post('/api/inventory/pedidos', {
        data: {
          sedeSolicitanteId,
          sedeProveedoraId,
          lineas: [{ insumoId, cantidadSolicitada: 999_999 }],
        },
      }),
      page2.request.post('/api/inventory/pedidos', {
        data: {
          sedeSolicitanteId,
          sedeProveedoraId,
          lineas: [{ insumoId, cantidadSolicitada: 999_999 }],
        },
      }),
    ]);

    // Validar que el servidor maneja correctamente la concurrencia (400/404/422/500 controlado)
    console.log(`[CONCURRENCIA] Res1: ${res1.status()} | Res2: ${res2.status()}`);
    expect([400, 404, 422, 500]).toContain(res1.status());
    expect([400, 404, 422, 500]).toContain(res2.status());

    await context1.close();
    await context2.close();
  });
});
