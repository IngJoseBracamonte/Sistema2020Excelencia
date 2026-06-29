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

    // 3. Fill in the Sede details
    const codeInput = page.locator('label:has-text("Código") + input');
    const nameInput = page.locator('label:has-text("Nombre") + input');
    
    await codeInput.fill('SUC_TEST');
    await nameInput.fill('Sucursal de Pruebas E2E');
    console.log('Filled Sede form.');

    // 4. Save Sede
    const saveBtn = page.locator('button:has-text("Guardar")');
    await saveBtn.click();
    console.log('Clicked Guardar Sede.');

    // 5. Verify the Sede is added and visible in the list
    const newSedeBlock = page.locator('.rounded-xl.border', { hasText: 'SUC_TEST' });
    await expect(newSedeBlock).toBeVisible({ timeout: 10000 });
    console.log('New Sede is visible in the list.');

    // 6. Click "+ Agregar Área" on the newly created Sede block
    const agregarAreaBtn = newSedeBlock.locator('button:has-text("+ Agregar Área")');
    await expect(agregarAreaBtn).toBeVisible();
    await agregarAreaBtn.click();
    console.log('Opened Agregar Área Clinica modal.');

    // 7. Fill in the Area Clinica details
    const areaCodeInput = page.locator('label:has-text("Código") + input');
    const areaNameInput = page.locator('label:has-text("Nombre del Área") + input');

    await areaCodeInput.fill('AREA_E2E');
    await areaNameInput.fill('Departamento de Pruebas');
    console.log('Filled Area form.');

    // 8. Save Area Clinica
    const agregarBtn = page.locator('button:has-text("Agregar")');
    await agregarBtn.click();
    console.log('Clicked Agregar Area Clinica.');

    // 9. Verify the Area Clinica is listed under the Sede
    const areaListItem = newSedeBlock.locator('text=[AREA_E2E] Departamento de Pruebas');
    await expect(areaListItem).toBeVisible({ timeout: 10000 });
    console.log('Area Clinica successfully associated and visible.');
  });
});
