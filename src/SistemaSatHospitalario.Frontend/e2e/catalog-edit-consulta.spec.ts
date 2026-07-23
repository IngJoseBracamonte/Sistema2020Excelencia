import { test, expect } from '@playwright/test';

test.describe('Catálogo - Edición de Consulta con Honorarios de Médicos y Sugerencias', () => {
  test('Flujo E2E: Filtrar por Consulta, Abrir Editor, Asignar Honorarios de Médico y Sugerencias', async ({ page }) => {
    page.on('console', msg => console.log('PAGE LOG:', msg.text()));

    // 1. Iniciar sesión en el sistema como Administrador
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await page.fill('input#username', 'admin');
    await page.fill('input#password', 'Admin123*!');
    await page.click('button[type="submit"]');

    // Esperar redirección al dashboard
    await page.waitForURL('**/dashboard');
    console.log('Autenticación E2E exitosa, navegando a Maestro de Servicios.');

    // 2. Navegar a Maestro de Servicios
    await page.goto('/catalog');
    await page.waitForLoadState('networkidle');

    // Verificar título del módulo
    await expect(page.locator('h1, h2, div').filter({ hasText: /Maestro de/i }).first()).toBeVisible();

    // 3. Filtrar por tipo 'CONSULTA'
    const consultaFilterChip = page.locator('button', { hasText: 'CONSULTA' }).first();
    await expect(consultaFilterChip).toBeVisible();
    await consultaFilterChip.click();
    await page.waitForTimeout(500);

    // 4. Hacer clic en "Editar" en el primer elemento de la tabla o en "Nuevo Servicio"
    const editButton = page.locator('button:has-text("EDITAR")').first();
    if (await editButton.isVisible()) {
      await editButton.click();
      console.log('Se hizo clic en EDITAR en una consulta existente.');
    } else {
      const newServiceBtn = page.locator('button:has-text("NUEVO SERVICIO")').first();
      await newServiceBtn.click();
      console.log('Se hizo clic en NUEVO SERVICIO para consulta.');
    }

    // 5. Verificar que abre el modal de edición de consulta app-edit-consulta
    const modalEditor = page.locator('app-edit-consulta');
    await expect(modalEditor.locator('h2').first()).toBeVisible({ timeout: 10000 });
    console.log('Modal app-edit-consulta desplegado correctamente.');

    // 6. Verificar y llenar datos de consulta (Nombre, Código, Precio USD)
    const nombreInput = modalEditor.locator('input[placeholder*="Consulta"]');
    const precioUsdInput = modalEditor.locator('input[placeholder="50.00"]');
    const honorarioBaseInput = modalEditor.locator('input[placeholder="0.00"]');

    if (await nombreInput.isVisible()) {
      await nombreInput.fill('Consulta Cardiología Especializada E2E');
    }
    if (await precioUsdInput.isVisible()) {
      await precioUsdInput.fill('60.00');
    }
    if (await honorarioBaseInput.isVisible()) {
      await honorarioBaseInput.fill('35.00');
    }

    // 7. Sección de Honorarios Médicos Específicos: Buscar y agregar médico
    const medicoSearchInput = modalEditor.locator('input[placeholder*="Buscar médico"]');
    if (await medicoSearchInput.isVisible()) {
      await medicoSearchInput.focus();
      await medicoSearchInput.fill('Pérez');
      await page.waitForTimeout(500);

      const firstMedicoOption = modalEditor.locator('div button:has-text("Dr.")').first();
      if (await firstMedicoOption.isVisible()) {
        await firstMedicoOption.click();
        console.log('Médico asignado a la consulta.');
      }
    }

    // 8. Sección de Sugerencias Vinculadas: Buscar y seleccionar sugerencias
    const sugerenciasSearchInput = modalEditor.locator('aside input[placeholder*="Buscar servicio"]');
    if (await sugerenciasSearchInput.isVisible()) {
      await sugerenciasSearchInput.fill('Electrocardiograma');
      await page.waitForTimeout(300);

      const sugerenciaItem = modalEditor.locator('aside button').first();
      if (await sugerenciaItem.isVisible()) {
        await sugerenciaItem.click();
        console.log('Sugerencia vinculada a la consulta.');
      }
    }

    // 9. Hacer clic en Cancelar o Guardar para verificar cierre limpio del modal
    const closeButton = modalEditor.locator('button:has-text("Cancelar")').first();
    await expect(closeButton).toBeVisible();
    await closeButton.click();

    // Confirmar que el modal se cierra
    await expect(modalEditor).toBeHidden();
    console.log('Flujo E2E de Edición de Consulta validado con éxito.');
  });
});
