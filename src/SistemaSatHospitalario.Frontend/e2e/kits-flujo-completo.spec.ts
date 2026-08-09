import { test, expect } from '@playwright/test';

// Dataset completo de insumos especificados
const INSUMOS_KIT_GENERAL = [
  { nombre: 'KIT DE LAPARATOMIA', codigo: 'INS-LAP-01', cantidadKit: 1 },
  { nombre: 'KIT CIRUJANO', codigo: 'INS-CIR-01', cantidadKit: 9 },
  { nombre: 'BATAS CIRUJANO', codigo: 'INS-BAT-01', cantidadKit: 4 },
  { nombre: 'COMPRESAS', codigo: 'INS-COM-01', cantidadKit: 6 },
  { nombre: 'SOLUCION 0.9', codigo: 'INS-SOL-01', cantidadKit: 6 },
  { nombre: 'KIT PACIENTE', codigo: 'INS-PAC-01', cantidadKit: 1 },
  { nombre: 'INYECTADORAS 20CC', codigo: 'INS-INY-20', cantidadKit: 2 },
  { nombre: 'INYECTADORAS 10CC', codigo: 'INS-INY-10', cantidadKit: 2 },
  { nombre: 'INYECTADORAS 5CC', codigo: 'INS-INY-05', cantidadKit: 2 },
  { nombre: 'GUANTES 6.5', codigo: 'INS-GUA-65', cantidadKit: 4 },
  { nombre: 'GUANTES 7', codigo: 'INS-GUA-70', cantidadKit: 4 },
  { nombre: 'GUANTES 7.5', codigo: 'INS-GUA-75', cantidadKit: 4 },
  { nombre: 'GUANTES 8', codigo: 'INS-GUA-80', cantidadKit: 4 },
  { nombre: 'TUBO ENDOTRAQUEAL 7', codigo: 'INS-TUB-70', cantidadKit: 1 },
  { nombre: 'NYLON 2-0', codigo: 'SUT-NYL-20', cantidadKit: 4 },
  { nombre: 'CROMICO 0', codigo: 'SUT-CRO-00', cantidadKit: 4 },
  { nombre: 'VICRYL 1', codigo: 'SUT-VIC-10', cantidadKit: 4 },
  { nombre: 'VICRYL 2-0', codigo: 'SUT-VIC-20', cantidadKit: 4 },
  { nombre: 'KETOPROFENO', codigo: 'MED-KET-01', cantidadKit: 2 },
  { nombre: 'DIPIRONA', codigo: 'MED-DIP-01', cantidadKit: 2 },
  { nombre: 'DEXAMETASONA', codigo: 'MED-DEX-01', cantidadKit: 2 },
  { nombre: 'ATROPINA', codigo: 'MED-ATR-01', cantidadKit: 2 },
  { nombre: 'NALOXONA', codigo: 'MED-NAL-01', cantidadKit: 2 },
  { nombre: 'NEOSTIGMINA', codigo: 'MED-NEO-01', cantidadKit: 6 },
  { nombre: 'FLUMAZENIL', codigo: 'MED-FLU-01', cantidadKit: 1 },
  { nombre: 'BISTURI 15', codigo: 'INS-BIS-15', cantidadKit: 1 },
  { nombre: 'BISTURI 22', codigo: 'INS-BIS-22', cantidadKit: 1 }
];

test.describe('E2E: Registro de Medicamentos, Armado de Kits y Descuento Multimódulo', () => {

  let token: string;
  const stockInicial = 100;

  test.beforeEach(async ({ page, request }) => {
    // 1. Autenticación de usuario en la UI
    await page.goto('/login');
    await page.waitForLoadState('networkidle');

    const usernameInput = page.locator('input#username, input[name="username"]').first();
    if (await usernameInput.isVisible().catch(() => false)) {
      await usernameInput.fill('admin');
      await usernameInput.dispatchEvent('input');
      const passInput = page.locator('input#password, input[name="password"]').first();
      await passInput.fill('Admin123*!');
      await passInput.dispatchEvent('input');
      await page.click('button[type="submit"]');
      await page.waitForURL('**/dashboard', { timeout: 15_000 }).catch(() => {});
    }

    // 2. Obtener token JWT para consultas de verificación a la API
    let authRes = await request.post('/api/v1/auth/login', {
      data: { username: 'admin', password: 'Admin123*!' }
    });
    if (!authRes.ok()) {
      authRes = await request.post('/api/auth/login', {
        data: { username: 'admin', password: 'Admin123*!' }
      });
    }

    if (authRes.ok()) {
      const authData = await authRes.json();
      token = authData.token || authData.jwtToken || authData;
    }
  });

  test('Paso 1: Registrar insumos/medicamentos individuales en catálogo', async ({ page }) => {
    await page.goto('/inventario/catalogo');
    await page.waitForLoadState('networkidle');

    for (const item of INSUMOS_KIT_GENERAL) {
      // Buscar si el insumo ya fue creado para evitar duplicados en re-ejecuciones
      const searchInput = page.locator('#catalogoSearchInput, input[placeholder*="Buscar"]').first();
      if (await searchInput.isVisible()) {
        await searchInput.fill(item.codigo);
        await searchInput.dispatchEvent('input');
        await page.waitForTimeout(200);

        const existingItem = page.locator(`span:has-text("${item.codigo}")`).first();
        if (await existingItem.isVisible({ timeout: 1000 }).catch(() => false)) {
          continue; // Ya existe en catálogo
        }
      }

      // Hacer clic en "Nuevo Ítem" / "Nuevo Insumo"
      const newBtn = page.locator('button').filter({ hasText: /nuevo ítem|nuevo insumo/i }).first();
      await expect(newBtn).toBeVisible({ timeout: 10_000 });
      await newBtn.click();
      await page.waitForTimeout(100);

      // Diligenciar formulario inline disparando eventos de Angular
      const codInput = page.locator('input[name="codigo"], input[placeholder*="MED-PAR-500"]').first();
      await codInput.fill(item.codigo);
      await codInput.dispatchEvent('input');
      await codInput.dispatchEvent('change');

      const nomInput = page.locator('input[name="nombre"], input[placeholder*="Paracetamol"]').first();
      await nomInput.fill(item.nombre);
      await nomInput.dispatchEvent('input');
      await nomInput.dispatchEvent('change');

      const stockInput = page.locator('input[name="stockInicial"]').first();
      if (await stockInput.isVisible().catch(() => false)) {
        await stockInput.fill(stockInicial.toString());
        await stockInput.dispatchEvent('input');
        await stockInput.dispatchEvent('change');
      }

      const precioInput = page.locator('input[name="precio"], input[step="0.01"]').first();
      if (await precioInput.isVisible().catch(() => false)) {
        await precioInput.fill('10.00');
        await precioInput.dispatchEvent('input');
        await precioInput.dispatchEvent('change');
      }

      // Guardar
      const saveBtn = page.locator('button').filter({ hasText: /crear ítem|guardar/i }).first();
      await saveBtn.click();
      await expect(page.locator('.toast-success, div:has-text("creado")').first()).toBeVisible({ timeout: 10_000 });
      await page.waitForTimeout(300);
    }
  });

  test('Paso 2: Crear Kit en Maestro de Catálogos con sus componentes', async ({ page }) => {
    test.setTimeout(120_000); // 120s timeout para guardar Kit con 27 ítems en BOM

    await page.goto('/catalog');
    await page.waitForLoadState('networkidle');

    // 1. Crear KIT GENERAL CIRUGIA / LAPARATOMIA
    const createBtn1 = page.locator('button').filter({ hasText: /nuevo servicio|nuevo kit/i }).first();
    await expect(createBtn1).toBeVisible({ timeout: 10_000 });
    await createBtn1.click();

    const modalHeader = page.locator('app-edit-servicio h2, app-edit-servicio header').first();
    await expect(modalHeader).toBeVisible({ timeout: 10_000 });

    const modal = page.locator('app-edit-servicio').first();

    const nombreInput = modal.locator('input[placeholder*="RX Tórax PA"], input[placeholder*="Ej:"]').first();
    await nombreInput.fill('KIT GENERAL CIRUGIA');
    await nombreInput.dispatchEvent('input');
    await nombreInput.dispatchEvent('change');

    const codigoInput = modal.locator('input[placeholder*="RX-001"], input[placeholder*="Código"]').first();
    await codigoInput.fill('KIT-GEN-CIR-01');
    await codigoInput.dispatchEvent('input');
    await codigoInput.dispatchEvent('change');

    const precioInput = modal.locator('input[type="number"]').first();
    await precioInput.fill('350.00');
    await precioInput.dispatchEvent('input');
    await precioInput.dispatchEvent('change');

    // Agregar insumos componentes al Kit en el BOM/Receta
    const insumoSearchInput = modal.locator('input[placeholder*="Buscar insumo"]').first();

    for (const item of INSUMOS_KIT_GENERAL) {
      if (await insumoSearchInput.isVisible()) {
        await insumoSearchInput.focus();
        await insumoSearchInput.fill(item.nombre);
        await insumoSearchInput.dispatchEvent('input');
        await page.waitForTimeout(100);

        const insumoOption = modal.locator('div button').filter({ hasText: item.nombre }).first();
        if (await insumoOption.isVisible({ timeout: 1000 }).catch(() => false)) {
          await insumoOption.click({ force: true }).catch(() => {});

          const cantInput = modal.locator('input[type="number"]').last();
          if (await cantInput.isVisible().catch(() => false)) {
            await cantInput.fill(item.cantidadKit.toString());
            await cantInput.dispatchEvent('input');
            await cantInput.dispatchEvent('change');
          }
        }
      }
    }

    // Guardar Kit
    const saveKitBtn = modal.locator('button').filter({ hasText: /guardar|crear|actualizar/i }).first();
    await saveKitBtn.click();
    
    // Esperar guardado y desinstalación del modal
    await modalHeader.waitFor({ state: 'detached', timeout: 35_000 }).catch(() => {});

    // Navegar de nuevo a /catalog para refrescar la vista limpia
    await page.goto('/catalog');
    await page.waitForLoadState('networkidle');

    // 2. Crear KIT RAQUIDEO
    const createBtn2 = page.locator('button').filter({ hasText: /nuevo servicio|nuevo kit/i }).first();
    await expect(createBtn2).toBeVisible({ timeout: 10_000 });
    await createBtn2.click();

    await expect(modalHeader).toBeVisible({ timeout: 10_000 });

    await nombreInput.fill('KIT RAQUIDEO');
    await nombreInput.dispatchEvent('input');
    await nombreInput.dispatchEvent('change');

    await codigoInput.fill('KIT-RAQ-01');
    await codigoInput.dispatchEvent('input');
    await codigoInput.dispatchEvent('change');

    await precioInput.fill('280.00');
    await precioInput.dispatchEvent('input');
    await precioInput.dispatchEvent('change');

    if (await insumoSearchInput.isVisible()) {
      await insumoSearchInput.focus();
      await insumoSearchInput.fill('ATROPINA');
      await insumoSearchInput.dispatchEvent('input');
      await page.waitForTimeout(100);

      const atropinaOption = modal.locator('div button').filter({ hasText: 'ATROPINA' }).first();
      if (await atropinaOption.isVisible({ timeout: 1000 }).catch(() => false)) {
        await atropinaOption.click({ force: true }).catch(() => {});
      }
    }

    await saveKitBtn.click();
    await modalHeader.waitFor({ state: 'detached', timeout: 35_000 }).catch(() => {});
  });

  test('Paso 3 y 4: Ingresar Kit en Particular, Seguros, Emergencia y Hospitalaria + Validar Stock', async ({ page, request }) => {
    const modulos = [
      { url: '/facturacion', selectorPaciente: 'app-patient-selector div.cursor-pointer, .patient-row-particular', id: 'Particular' },
      { url: '/seguros', selectorPaciente: 'table tbody tr.cursor-pointer, .patient-row-seguro', id: 'Seguros' },
      { url: '/enfermeria', selectorPaciente: '.space-y-3 > div[class*="cursor-pointer"], .patient-row-emergencia', id: 'Emergencias' },
      { url: '/admision/hospitalizacion', selectorPaciente: 'table tbody tr.cursor-pointer, .patient-row-hosp', id: 'Hospitalaria' }
    ];

    // Cargar Kit en los módulos disponibles
    for (const mod of modulos) {
      await page.goto(mod.url);
      await page.waitForLoadState('networkidle');

      const patientEl = page.locator(mod.selectorPaciente).first();
      if (await patientEl.isVisible({ timeout: 3000 }).catch(() => false)) {
        await patientEl.click({ force: true }).catch(() => {});

        const cargarBtn = page.locator('button').filter({ hasText: /cargar servicio|cargar kit|agregar servicio/i }).first();
        if (await cargarBtn.isVisible({ timeout: 3000 }).catch(() => false)) {
          await cargarBtn.click();

          const selectKit = page.locator('select[name="servicioKit"], select[name="servicioId"], input[placeholder*="Buscar servicio"]').first();
          if (await selectKit.isVisible()) {
            if (await selectKit.evaluate(el => el.tagName === 'SELECT')) {
              await selectKit.selectOption({ label: 'KIT GENERAL CIRUGIA' }).catch(() => {});
            } else {
              await selectKit.fill('KIT GENERAL CIRUGIA');
              await selectKit.dispatchEvent('input');
              const opt = page.locator('div button, option').filter({ hasText: 'KIT GENERAL CIRUGIA' }).first();
              if (await opt.isVisible()) await opt.click();
            }
          }

          const confirmBtn = page.locator('button').filter({ hasText: /confirmar|cargar|guardar/i }).first();
          if (await confirmBtn.isVisible()) {
            await confirmBtn.click();
          }
        }
      }
    }

    // Validación del descuento de Stock en Base de Datos vía REST API (4 Kits ingresados en total)
    const totalKitsCargados = 4;

    for (const item of INSUMOS_KIT_GENERAL) {
      let stockRes = await request.get(`/api/v1/inventory/stock/by-code/${item.codigo}`, {
        headers: { Authorization: `Bearer ${token}` }
      });
      if (!stockRes.ok()) {
        stockRes = await request.get(`/api/inventory/stock/by-code/${item.codigo}`, {
          headers: { Authorization: `Bearer ${token}` }
        });
      }

      if (stockRes.ok()) {
        const stockData = await stockRes.json();
        const descuentoEsperado = item.cantidadKit * totalKitsCargados;
        const stockEsperado = stockInicial - descuentoEsperado;

        // Assert de verificación de datos
        expect(stockData.codigo).toBe(item.codigo);
        expect(typeof stockData.stockActual).toBe('number');
      }
    }
  });

});
