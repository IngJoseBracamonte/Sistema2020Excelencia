import { test, expect } from '@playwright/test';
import { CatalogPage } from './catalog.page';

test.describe('E2E: Maestro de Servicios - Selección Desplegable de Creación, Soft Delete y Reactivación', () => {
  let catalogPage: CatalogPage;

  test.beforeEach(async ({ page }) => {
    catalogPage = new CatalogPage(page);
    await catalogPage.gotoCatalog();
  });

  test('1. Debe desplegar el menú con los 8 procesos clínicos y cerrarlo al hacer clic en el backdrop', async ({ page }) => {
    // 1. Localizar el botón Nuevo Ítem
    const createBtn = page.locator('button').filter({ hasText: /nuevo (ítem|servicio)/i }).first();
    await expect(createBtn).toBeVisible({ timeout: 10_000 });

    // 2. Clic para abrir el dropdown
    await createBtn.click();

    // 3. Verificar encabezado del dropdown flotante reactivamente
    const dropdown = page.locator('div.absolute').filter({ hasText: /seleccionar proceso a crear/i }).first();
    await expect(dropdown).toBeVisible({ timeout: 5000 });

    // 4. Verificar que se listan los procesos clínicos esperados
    const expectedProcesses = [
      /consulta médica/i,
      /cirugía \/ pabellón/i,
      /laboratorio clínico/i,
      /tomografía \/ imágenes/i,
      /medicamento \/ fármaco/i,
      /procedimiento ambulatorio/i,
      /estancia hospitalaria/i,
      /servicio base \/ general/i
    ];

    for (const processRegex of expectedProcesses) {
      const option = dropdown.locator('button').filter({ hasText: processRegex }).first();
      await expect(option).toBeVisible();
    }

    // 5. Clic en el backdrop para cerrar el menú
    const backdrop = page.locator('div.fixed.inset-0.z-40').first();
    if (await backdrop.isVisible()) {
      await backdrop.click({ force: true });
      await expect(dropdown).not.toBeVisible();
    }
  });

  test('2. Debe abrir el CRUD especializado en modo creación con campos limpios al seleccionar desde el desplegable', async ({ page }) => {
    // Probar Consulta Médica
    const modalConsulta = await catalogPage.openCreateDropdownAndSelectType('CONSULTA');
    const headerConsulta = modalConsulta.locator('h2').first();
    await expect(headerConsulta).toContainText(/nueva consulta/i);
    
    // Cerrar modal
    const closeBtn = modalConsulta.locator('button').filter({ has: page.locator('lucide-icon') }).first();
    await closeBtn.click();
    await expect(modalConsulta).not.toBeVisible();

    // Probar Cirugía
    const modalCirugia = await catalogPage.openCreateDropdownAndSelectType('CIRUGIA');
    const headerCirugia = modalCirugia.locator('h2').first();
    await expect(headerCirugia).toContainText(/nueva cirugía/i);
    
    const closeBtnCirugia = modalCirugia.locator('button').filter({ has: page.locator('lucide-icon') }).first();
    await closeBtnCirugia.click();
    await expect(modalCirugia).not.toBeVisible();

    // Probar Tomografía
    const modalTomo = await catalogPage.openCreateDropdownAndSelectType('TOMOGRAFIA');
    const headerTomo = modalTomo.locator('h2').first();
    await expect(headerTomo).toContainText(/nueva tomografía/i);
    
    const closeBtnTomo = modalTomo.locator('button').filter({ has: page.locator('lucide-icon') }).first();
    await closeBtnTomo.click();
    await expect(modalTomo).not.toBeVisible();
  });

  test('3. Debe filtrar reactivamente por selector de Estado (TODOS, ACTIVOS, DESACTIVADOS)', async ({ page }) => {
    const estadoSelect = page.locator('select').filter({ has: page.locator('option[value="DESACTIVADOS"]') }).first();
    await expect(estadoSelect).toBeVisible({ timeout: 10_000 });

    // Filtrar Solo Activos
    await estadoSelect.selectOption('ACTIVOS');
    const deactivatedBadgesActivos = page.locator('table span').filter({ hasText: /^DESACTIVADO$/i });
    await expect(deactivatedBadgesActivos).toHaveCount(0);

    // Filtrar Solo Desactivados
    await estadoSelect.selectOption('DESACTIVADOS');
    const rowsDesactivados = page.locator('table tbody tr');
    const countDesactivados = await rowsDesactivados.count();
    if (countDesactivados > 0) {
      const firstBadge = rowsDesactivados.first().locator('span').filter({ hasText: /^DESACTIVADO$/i });
      await expect(firstBadge).toBeVisible();
    }

    // Volver a Todos
    await estadoSelect.selectOption('TODOS');
    await expect(page.locator('table tbody tr').first()).toBeVisible();
  });

  test('4. Flujo de Soft Delete y Reactivación en la grilla', async ({ page }) => {
    // 1. Crear un servicio temporal para probar el ciclo de desactivación y reactivación
    const timestamp = Date.now().toString().slice(-5);
    const nombreItem = `E2E-SOFT-DEL-${timestamp}`;
    const codigoItem = `SD-${timestamp}`;

    const modal = await catalogPage.openCreateDropdownAndSelectType('SERVICIO');
    await catalogPage.fillBasicFields(modal, {
      nombre: nombreItem,
      codigo: codigoItem,
      precioUsd: 45.0,
      honorarioBaseUsd: 10.0
    });

    const saveBtn = modal.locator('button').filter({ hasText: /guardar/i }).first();
    await saveBtn.click();
    await expect(modal).not.toBeVisible({ timeout: 10_000 });

    // 2. Buscar el servicio recién creado en la tabla
    const searchInput = page.locator('input[placeholder*="BUSCAR"]').first();
    await searchInput.fill(codigoItem);

    const fila = page.locator('table tbody tr').filter({ hasText: codigoItem }).first();
    await expect(fila).toBeVisible({ timeout: 10_000 });

    // 3. Ejecutar Soft Delete (Desactivar)
    const deleteBtn = fila.locator('button[title*="Desactivar"], button:has(lucide-icon)').filter({ has: page.locator('svg.lucide-trash-2') }).first();
    if (await deleteBtn.isVisible()) {
      await deleteBtn.click();

      // Confirmar en el modal de confirmación
      const confirmModal = page.locator('div.fixed').filter({ hasText: /¿desactivar servicio\?/i }).first();
      await expect(confirmModal).toBeVisible();
      const confirmBtn = confirmModal.locator('button').filter({ hasText: /confirmar/i }).first();
      await confirmBtn.click();
      await expect(confirmModal).not.toBeVisible();

      // 4. Verificar que permanece visible pero con badge DESACTIVADO
      const badgeDesactivado = fila.locator('span').filter({ hasText: /^DESACTIVADO$/i }).first();
      await expect(badgeDesactivado).toBeVisible({ timeout: 5000 });

      // 5. Reactivar el servicio
      const reactivateBtn = fila.locator('button').filter({ hasText: /reactivar/i }).first();
      await expect(reactivateBtn).toBeVisible({ timeout: 5000 });
      await reactivateBtn.click();

      // 6. Verificar que el badge DESACTIVADO desapareció
      await expect(badgeDesactivado).not.toBeVisible({ timeout: 5000 });
    }
  });
});
