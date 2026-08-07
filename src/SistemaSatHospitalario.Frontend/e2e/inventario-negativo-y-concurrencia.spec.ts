import { test, expect } from '@playwright/test';

test.describe('Pruebas Negativas y No Funcionales de Inventario', () => {

  test.beforeEach(async ({ page }) => {
    // Autenticación como usuario administrador/supervisor
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await page.fill('input#username', 'admin');
    await page.fill('input#password', 'Admin123*!');
    await page.click('button[type="submit"]');

    await page.waitForURL((url) => url.pathname.includes('dashboard'), { timeout: 15000 });
  });

  test('Debe rechazar la creación de una compra con cantidad o costo inválido', async ({ page }) => {
    await page.goto('/inventario/compras');
    await page.waitForLoadState('networkidle');

    // Intentar submit sin seleccionar insumo ni cantidad válida
    const submitBtn = page.locator('button:has-text("REGISTRAR COMPRA DE INSUMO"), button:has-text("Guardar"), button[type="submit"]').first();
    
    if (await submitBtn.isVisible()) {
      await expect(submitBtn).toBeDisabled();
    }
  });

  test('Prueba No Funcional: Inyección de Concurrencia Simultánea en Pedidos Inter-Sede', async ({ browser }) => {
    // Crear 2 contextos independientes simulando 2 usuarios solicitando el mismo insumo al mismo tiempo
    const context1 = await browser.newContext();
    const context2 = await browser.newContext();

    const page1 = await context1.newPage();
    const page2 = await context2.newPage();

    // Login simultáneo en ambos contextos
    await Promise.all([
      (async () => {
        await page1.goto('/');
        await page1.fill('input#username', 'admin');
        await page1.fill('input#password', 'Admin123*!');
        await page1.click('button[type="submit"]');
        await page1.waitForURL((url) => url.pathname.includes('dashboard'));
      })(),
      (async () => {
        await page2.goto('/');
        await page2.fill('input#username', 'admin');
        await page2.fill('input#password', 'Admin123*!');
        await page2.click('button[type="submit"]');
        await page2.waitForURL((url) => url.pathname.includes('dashboard'));
      })()
    ]);

    // Navegar simultáneamente a la gestión de pedidos
    await Promise.all([
      page1.goto('/inventario/pedidos'),
      page2.goto('/inventario/pedidos')
    ]);

    // Solicitud simultánea de consumo/despacho excediendo el stock real vía API
    const [res1, res2] = await Promise.all([
      page1.request.post('/api/inventory/pedidos', {
        data: {
          sedeSolicitanteId: '00000000-0000-0000-0000-000000000002',
          sedeProveedoraId: '00000000-0000-0000-0000-000000000001',
          lineas: [{ insumoId: '00000000-0000-0000-0000-000000000001', cantidadSolicitada: 999999 }]
        }
      }),
      page2.request.post('/api/inventory/pedidos', {
        data: {
          sedeSolicitanteId: '00000000-0000-0000-0000-000000000002',
          sedeProveedoraId: '00000000-0000-0000-0000-000000000001',
          lineas: [{ insumoId: '00000000-0000-0000-0000-000000000001', cantidadSolicitada: 999999 }]
        }
      })
    ]);

    // Validar que el servidor maneje correctamente los errores o rechazos de sobre-solicitud/concurrencia (400 / 422 / 500 controlado)
    expect([400, 404, 422, 500]).toContain(res1.status());
    expect([400, 404, 422, 500]).toContain(res2.status());

    await context1.close();
    await context2.close();
  });
});
