import { test, expect } from '@playwright/test';

test.describe('Inventory Multi-Sede E2E Tests', () => {
  test.beforeEach(async ({ page }) => {
    // 1. Authenticate as Admin
    await page.goto('/');
    await page.waitForLoadState('networkidle');

    await page.fill('input#username', 'admin');
    await page.fill('input#password', 'Admin123*!');
    await page.click('button[type="submit"]');

    // Wait for redirect to dashboard
    await page.waitForURL('**/dashboard');
    console.log('Logged in successfully, navigated to dashboard.');
  });

  test('Should create a new Sede and associate an Area Clinica to it', async ({ page }) => {
    page.on('console', msg => console.log('PAGE LOG:', msg.text()));

    // 1. Navigate to Sede management page
    await page.goto('/admin/inventory/sedes');
    await page.waitForLoadState('networkidle');

    // 2. Click "+ Nueva Sede" button
    const nuevaSedeBtn = page.locator('button:has-text("+ Nueva Sede")');
    await expect(nuevaSedeBtn).toBeVisible();
    await nuevaSedeBtn.click();
    console.log('Opened Nueva Sede modal.');

    // 3. Fill in the Sede details (dentro del modal de Sede)
    //    Usamos el contenedor del modal para aislar los inputs del formulario de Sede
    //    vs. los inputs del formulario de Área Clínica (mismo label 'Código')
    const sedeModal = page.locator('[role="dialog"], .modal-sede, form').first();
    const codeInput = sedeModal.locator('input').first();
    const nameInput = sedeModal.locator('input').nth(1);

    await codeInput.fill('SUC_TEST');
    await nameInput.fill('Sucursal de Pruebas E2E');
    console.log('Filled Sede form.');

    // 4. Save Sede y esperar a que el DOM se estabilice (networkidle)
    //    CRITICAL: sin este wait, el componente Angular re-renderiza la lista
    //    inmediatamente y el locator del bloque de Sede queda detached.
    const saveBtn = page.locator('button:has-text("Guardar")');
    await saveBtn.click();
    console.log('Clicked Guardar Sede.');
    await page.waitForLoadState('networkidle');

    // 5. Verify the Sede is added and visible in the list
    //    Re-localizamos tras el re-render para obtener la referencia fresca del nodo
    const newSedeBlock = page.locator('.rounded-xl.border', { hasText: 'SUC_TEST' });
    await expect(newSedeBlock).toBeVisible({ timeout: 10000 });
    console.log('New Sede is visible in the list.');

    // 6. Click "+ Agregar Área" en el bloque de la Sede recién creada
    //    Usamos la referencia fresca del bloque para evitar el detach
    const agregarAreaBtn = newSedeBlock.locator('button:has-text("+ Agregar Área")');
    await expect(agregarAreaBtn).toBeVisible();
    await agregarAreaBtn.click();
    console.log('Opened Agregar Área Clinica modal.');

    // 7. Esperar a que el formulario de Área aparezca en el DOM
    //    El modal se renderiza async — necesitamos que los inputs estén presentes
    await page.waitForSelector('label:has-text("Nombre del Área") + input', { timeout: 8000 });

    // 8. Fill in the Area Clinica details
    //    Estos selectores son diferentes a los del modal de Sede para evitar colisión
    const areaCodeInput = page.locator('label:has-text("Código del Área") + input,
                                         label:has-text("Código") + input').last();
    const areaNameInput = page.locator('label:has-text("Nombre del Área") + input');

    await areaCodeInput.fill('AREA_E2E');
    await areaNameInput.fill('Departamento de Pruebas');
    console.log('Filled Area form.');

    // 9. Save Area Clinica y esperar re-render de la lista
    const agregarBtn = page.locator('button:has-text("Agregar")');
    await agregarBtn.click();
    await page.waitForLoadState('networkidle');
    console.log('Clicked Agregar Area Clinica.');

    // 10. Verify the Area Clinica is listed under the Sede
    //     Re-localizamos newSedeBlock de nuevo (fresco tras segundo re-render)
    const updatedSedeBlock = page.locator('.rounded-xl.border', { hasText: 'SUC_TEST' });
    const areaListItem = updatedSedeBlock.locator('text=[AREA_E2E] Departamento de Pruebas');
    await expect(areaListItem).toBeVisible({ timeout: 10000 });
    console.log('Area Clinica successfully associated and visible.');
  });
});
