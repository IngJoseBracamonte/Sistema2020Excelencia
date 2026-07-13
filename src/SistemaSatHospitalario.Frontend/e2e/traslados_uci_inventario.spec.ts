import { test, expect } from '@playwright/test';

// ─── Helpers ─────────────────────────────────────────────────────────────────
async function loginAs(
  page: any,
  username: string,
  password: string
): Promise<void> {
  await page.goto('/');
  await page.waitForLoadState('networkidle');
  await page.fill('input#username', username);
  await page.fill('input#password', password);
  await page.click('button[type="submit"]');
  await page.waitForURL(
    (url: URL) =>
      url.pathname.includes('dashboard') ||
      url.pathname.includes('cierre-cuenta') ||
      url.pathname.includes('enfermeria'),
    { timeout: 15000 }
  );
  console.log(`[AUTH] Logged in as '${username}' → ${page.url()}`);
}

test.describe('E2E: Traslados, UCI e Inventario Permisivo (V1.3.0)', () => {
  test.beforeEach(async ({ page }) => {
    page.on('console', msg => console.log('BROWSER LOG:', msg.text()));
    await loginAs(page, 'user_emergencia', 'Hospital2026*!');
  });

  test('Debería admitir un paciente, trasladarlo (EMG -> HOS -> UCI), verificar precios base+honorarios y deducción permisiva', async ({ page }) => {
    // Generar cédula única aleatoria para evitar colisiones
    const testCedula = 'V-' + Math.floor(10000000 + Math.random() * 90000000);
    console.log(`[TEST-PREP] Paciente de prueba con cédula: ${testCedula}`);

    // 1. Ir al Módulo de Enfermería
    await page.goto('/enfermeria');
    await page.waitForLoadState('networkidle');
    await page.waitForSelector('h3:has-text("Pacientes Activos")');

    // 2. Modal de Ingreso de Paciente
    await page.click('button:has-text("Ingresar Paciente")');
    console.log('Opened Ingresar Paciente Modal.');

    // 3. Seleccionar "Registrar Nuevo Paciente"
    await page.click('button:has-text("REGISTRAR NUEVO PACIENTE")');

    // 4. Llenar los datos del nuevo paciente
    await page.locator('label:has-text("Cédula / Pasaporte") + input').fill(testCedula);
    await page.locator('label:has-text("Nombres") + input').fill('E2E_Nombre');
    await page.locator('label:has-text("Apellidos") + input').fill('E2E_Apellido');
    await page.locator('label:has-text("Celular Principal") + div >> input').fill('7654321');
    await page.locator('label:has-text("Dirección de Habitación") + input').fill('Avenida Principal E2E');
    
    // Configurar fecha de nacimiento DD-MM-YYYY
    await page.locator('input[placeholder*="DD-MM-YYYY"]').fill('02-07-1990');
    console.log('Filled new patient personal details.');

    // 5. Avanzar al Triage (Botón Siguiente: Datos de Triage)
    const nextTriageBtn = page.locator('button:has-text("SIGUIENTE: DATOS DE TRIAGE")');
    await expect(nextTriageBtn).toBeVisible();
    await nextTriageBtn.click();
    console.log('Advanced to step 2: Triage.');

    // 6. Llenar Triage Obligatorio de Emergencia
    await page.locator('label:has-text("Motivo de Consulta") + textarea').fill('DOLOR TORACICO AGUDO DE PRUEBA E2E');
    await page.locator('label:has-text("T.A. (mmHg)") + input').fill('130/85');
    await page.locator('label:has-text("F.C. (xmin)") + input').fill('90');
    await page.locator('label:has-text("F.R. (xmin)") + input').fill('20');
    await page.locator('label:has-text("Temp (°C)") + input').fill('38.2');
    await page.locator('label:has-text("Sat O2 (%)") + input').fill('96');
    
    // Seleccionar Clasificación Nivel II (Naranja)
    await page.locator('button:has-text("Nivel II (Naranja)")').click();
    console.log('Filled vital signs and triage form.');

    // 7. Enviar formulario (Registrar Ingreso y Triage Clínico)
    const registerIngresoBtn = page.locator('button:has-text("REGISTRAR INGRESO Y TRIAGE CLÍNICO")');
    await expect(registerIngresoBtn).toBeEnabled();
    await registerIngresoBtn.click();
    console.log('Clicked register and submitted triage.');

    // 8. Esperar a que se complete y refresque la lista de activos
    await page.waitForTimeout(3000);
    
    // Buscar al paciente recién creado en la lista de activos
    const newlyCreatedPatient = page.locator(`.space-y-3.max-h-\\[600px\\] >> text=${testCedula}`).first();
    await expect(newlyCreatedPatient).toBeVisible({ timeout: 15000 });
    await newlyCreatedPatient.click();
    console.log('Selected newly admitted patient.');

    // 9. Verificar que se muestra la información clínica
    await expect(page.locator('h3:has-text("Triage y Signos Vitales")')).toBeVisible();
    await expect(page.locator('span:has-text("Emergencia")').first()).toBeVisible();

    // ─────────────────────────────────────────────────────────────────────────────
    // TRASLADO 1: Emergencia (EMG) -> Hospitalización (HOS)
    // ─────────────────────────────────────────────────────────────────────────────
    console.log('[TRANSFER 1] Starting transfer from Emergencia to Hospitalización...');
    await page.click('button:has-text("Traslados y Destino")');
    await expect(page.locator('h3:has-text("Registrar Traslado o Alta Clínica")')).toBeVisible();

    // Seleccionar Hospitalización en el dropdown
    const locationDropdown = page.locator('label:has-text("Ubicación Destino de Traslado") + select');
    await locationDropdown.selectOption({ value: 'Hospitalizacion' });

    // Hacer click en confirmar traslado
    const confirmTransferBtn = page.locator('button:has-text("Confirmar Traslado de Ubicación")');
    await confirmTransferBtn.click();
    console.log('Clicked Confirmar Traslado to Hospitalización.');

    // Esperar toast de éxito
    await page.waitForTimeout(2500);

    // Filtrar por Hospitalización en la cabecera
    await page.click('button:has-text("Hospitalización")');
    await page.waitForTimeout(1000);

    // El paciente debe aparecer bajo la sección de Hospitalización
    const patientInHos = page.locator(`.space-y-3.max-h-\\[600px\\] >> text=${testCedula}`).first();
    await expect(patientInHos).toBeVisible({ timeout: 10000 });
    await patientInHos.click();
    console.log('Verified patient transferred to Hospitalización.');

    // ─────────────────────────────────────────────────────────────────────────────
    // TRASLADO 2: Hospitalización (HOS) -> UCI (Cuidados Intensivos)
    // ─────────────────────────────────────────────────────────────────────────────
    console.log('[TRANSFER 2] Starting transfer from Hospitalización to UCI...');
    await page.click('button:has-text("Traslados y Destino")');
    
    // Seleccionar UCI en el dropdown
    const locationDropdown2 = page.locator('label:has-text("Ubicación Destino de Traslado") + select');
    await locationDropdown2.selectOption({ value: 'UCI' });

    // Confirmar traslado a UCI
    const confirmTransferBtn2 = page.locator('button:has-text("Confirmar Traslado de Ubicación")');
    await confirmTransferBtn2.click();
    console.log('Clicked Confirmar Traslado to UCI.');

    await page.waitForTimeout(2500);

    // Filtrar por UCI en la cabecera
    await page.click('button:has-text("UCI")');
    await page.waitForTimeout(1000);

    // El paciente debe aparecer bajo la sección de UCI
    const patientInUci = page.locator(`.space-y-3.max-h-\\[600px\\] >> text=${testCedula}`).first();
    await expect(patientInUci).toBeVisible({ timeout: 10000 });
    await patientInUci.click();
    console.log('Verified patient transferred to UCI.');

    // ─────────────────────────────────────────────────────────────────────────────
    // CARGA DE CONSULTA: Regla de Cálculo Base + Honorario
    // ─────────────────────────────────────────────────────────────────────────────
    console.log('[PRICING] Verifying Base + Honorario calculation for Consulta...');
    await page.click('button:has-text("Carga de Insumos")');
    
    // Buscar Consulta Ginecologica
    const searchInput = page.locator('input[placeholder*="Escriba código o nombre"]');
    await searchInput.fill('Consulta Ginecologica');
    await page.waitForTimeout(1000);

    // Seleccionar el resultado
    const catalogResult = page.locator('app-step-catalog-search div.hover\\:bg-white\\/5').first();
    await expect(catalogResult).toBeVisible();
    await catalogResult.click();

    // En el Paso 2 (médico y área)
    await page.waitForSelector('#selectMedicoFastCharge', { timeout: 8000 });

    // Seleccionar un médico de la lista
    const doctorSelector = page.locator('#selectMedicoFastCharge');
    await doctorSelector.selectOption({ index: 1 });

    // Seleccionar Área Clínica (UCI en este caso)
    const areaSelector = page.locator('#selectAreaClinicaFastCharge');
    await areaSelector.selectOption({ index: 1 });

    // Obtener valores de los inputs de precio y honorario para calcular dinámicamente
    const basePriceStr = await page.locator('label:has-text("Precio Base USD") + input').inputValue();
    const honoraryPriceStr = await page.locator('label:has-text("Honorario Médico USD") + input').inputValue();
    
    const basePriceVal = parseFloat(basePriceStr) || 0;
    const honoraryPriceVal = parseFloat(honoraryPriceStr) || 0;
    const expectedTotal = basePriceVal + honoraryPriceVal;
    
    console.log(`[PRICING-CALC] Base: $${basePriceVal}, Honorario: $${honoraryPriceVal} -> Esperado: $${expectedTotal}`);

    // Verificar que el total estimado en el paso 2 coincide con Base + Honorario
    const totalStep2Text = await page.locator('span:has-text("Total Estimado de Consulta:") + span').textContent();
    const totalStep2Val = parseFloat((totalStep2Text || '').replace('$', '').trim()) || 0;
    expect(totalStep2Val).toBeCloseTo(expectedTotal, 1);

    // Avanzar a Paso 3 (Confirmación)
    await page.click('#btnStep2Next');
    await page.waitForTimeout(500);

    // Confirmar y agregar al carro
    await page.click('#btnStep3Confirm');
    await page.waitForTimeout(1500);

    // Verificar en el carro que el precio base y honorario se muestran correctamente
    const cartItemRow = page.locator('app-nursing-cart').locator('text=Consulta Ginecologica');
    await expect(cartItemRow).toBeVisible();
    
    const cartItemTotalText = await page.locator('app-nursing-cart .font-mono.font-bold').first().textContent();
    const cartItemTotalVal = parseFloat((cartItemTotalText || '').replace('$', '').trim()) || 0;
    expect(cartItemTotalVal).toBeCloseTo(expectedTotal, 1);
    console.log('Pricing verification matches Base + Honorario.');

    // ─────────────────────────────────────────────────────────────────────────────
    // INVENTARIO: Carga de Medicamento y Deducción Permisiva (Stock Negativo)
    // ─────────────────────────────────────────────────────────────────────────────
    console.log('[INVENTORY] Verifying permissive inventory deduction with negative stock...');
    
    // Buscar Ibuprofeno 600mg (Medicamento)
    await searchInput.fill('Ibuprofeno');
    await page.waitForTimeout(1000);

    // Seleccionar del catálogo
    const catalogResultMed = page.locator('app-step-catalog-search div.hover\\:bg-white\\/5').first();
    await expect(catalogResultMed).toBeVisible();
    await catalogResultMed.click();

    // En el Paso 2 (Cantidad y Área)
    await page.waitForSelector('#fastChargeQuantityInput', { timeout: 8000 });

    // Ingresar una cantidad alta (ej: 20 unidades) que exceda el stock inicial (5.00m en UCI)
    await page.fill('#fastChargeQuantityInput', '20');

    // Seleccionar Área Clínica (UCI en este caso)
    await page.locator('#selectAreaClinicaFastCharge').selectOption({ index: 1 });

    // Avanzar a Paso 3
    await page.click('#btnStep2Next');
    await page.waitForTimeout(500);

    // Confirmar y agregar al carro
    await page.click('#btnStep3Confirm');
    await page.waitForTimeout(1500);

    // Registrar todo a la cuenta para enviar al backend
    await page.click('button:has-text("Registrar todo a la cuenta")');
    console.log('Submitted all cart items to backend.');

    // Esperar confirmación de guardado exitoso
    await page.waitForTimeout(3000);

    // El carro debe estar vacío
    await expect(page.locator('text=El carro está vacío.')).toBeVisible();
    console.log('Permissive inventory test completed successfully.');
  });
});
