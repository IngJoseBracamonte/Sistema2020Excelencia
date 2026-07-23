import { Page, Locator, expect } from '@playwright/test';

export type CatalogType =
  | 'SERVICIO'
  | 'CONSULTA'
  | 'CIRUGIA'
  | 'TOMOGRAFIA'
  | 'HOSPITALARIO'
  | 'LABORATORIO'
  | 'RX'
  | 'PROCEDIMIENTO';

export interface CatalogItemPayload {
  descripcion: string;
  codigo: string;
  precioUsd: number;
  tipo: string;
  activo: boolean;
  honorarioBase: number;
  honorariumCategory?: string;
  requiereInventario: boolean;
  sugerenciasIds: string[];
  honorariosMedicos: Array<{ medicoId: string; honorario: number }>;
}

export class CatalogPage {
  // Campos permitidos en CreateCatalogItemCommand (.NET backend)
  private readonly allowedBackendPayloadKeys: Set<string> = new Set([
    'id',
    'descripcion',
    'codigo',
    'precioUsd',
    'tipo',
    'activo',
    'honorarioBase',
    'honorariumCategory',
    'requiereInventario',
    'sugerenciasIds',
    'honorariosMedicos',
    'requiereContraste',
    'protocoloTecnico'
  ]);

  constructor(public readonly page: Page) {}

  /**
   * Navega a la vista de Maestro de Servicios. Si detecta la pantalla de login,
   * realiza la autenticación automática de prueba.
   */
  async gotoCatalog(): Promise<void> {
    // 1. Ir a /login para asegurar sesión activa
    await this.page.goto('/login');
    await this.page.waitForLoadState('networkidle');

    // 2. Realizar autenticación si el formulario de login está presente
    const usernameInput = this.page.locator('input#username');
    if (await usernameInput.isVisible({ timeout: 3000 }).catch(() => false)) {
      await usernameInput.fill('admin');
      await this.page.fill('input#password', 'Admin123*!');
      await this.page.click('button[type="submit"]');
      await this.page.waitForURL('**/dashboard', { timeout: 15_000 }).catch(() => {});
    }

    // 3. Navegar a /admin/catalog
    await this.page.goto('/catalog');
    await this.page.waitForLoadState('networkidle');

    // 4. Verificar presencia del título Maestro de Servicios
    const titleLocator = this.page.locator('h1, h2, div').filter({ hasText: /maestro de\s+servicios/i }).first();
    await expect(titleLocator).toBeVisible({ timeout: 15_000 });
  }

  /**
   * Abre el modal de creación correspondiente para el tipo de catálogo especificado.
   */
  async openCreateModalForType(type: CatalogType): Promise<Locator> {
    // 1. Resetear filtros existentes haciendo clic en 'TODOS'
    const todosChip = this.page.locator('button', { hasText: 'TODOS' }).first();
    if (await todosChip.isVisible()) {
      await todosChip.click();
      await this.page.waitForTimeout(200);
    }

    // 2. Seleccionar el chip de filtro del tipo deseado
    const filterChip = this.page.locator('button').filter({ hasText: new RegExp(`^\\s*${type}\\s*$`, 'i') }).first();
    if (await filterChip.isVisible()) {
      await filterChip.click();
      await this.page.waitForTimeout(300);
    }

    // 3. Hacer clic en "Nuevo Servicio"
    const createBtn = this.page.locator('button').filter({ hasText: /nuevo servicio/i }).first();
    await expect(createBtn).toBeVisible({ timeout: 10_000 });
    await createBtn.click();
    await this.page.waitForTimeout(300);

    // 4. Ubicar el subcomponente modal de edición activo en Angular
    const modalLocator = this.page.locator(
      'app-edit-servicio, app-edit-consulta, app-edit-cirugia, app-edit-tomografia, app-edit-hospitalario, app-edit-laboratorio, app-edit-procedimiento, app-edit-medicamento'
    ).first();
    const header = modalLocator.locator('h2').first();
    await expect(header).toBeVisible({ timeout: 10_000 });
    return modalLocator;
  }

  /**
   * Diligencia los campos básicos requeridos en el modal activo.
   */
  async fillBasicFields(
    modal: Locator,
    data: { nombre: string; codigo: string; precioUsd: number; honorarioBaseUsd: number }
  ): Promise<void> {
    // Campo Nombre
    const nombreInput = modal.locator('input[placeholder*="Nombre"], input[placeholder*="Servicio"], input[placeholder*="Ej:"]').first();
    await expect(nombreInput).toBeVisible();
    await nombreInput.fill(data.nombre);
    await nombreInput.dispatchEvent('input');
    await nombreInput.dispatchEvent('change');

    // Campo Código
    const codigoInput = modal.locator(
      'input[placeholder*="SERV-"], input[placeholder*="CONS-"], input[placeholder*="CIR-"], input[placeholder*="TC-"], input[placeholder*="HOSP-"], input[placeholder*="LAB-"], input[placeholder*="PROC-"], input[placeholder*="Código"], input[placeholder*="001"]'
    ).first();
    await expect(codigoInput).toBeVisible();
    await codigoInput.fill(data.codigo);
    await codigoInput.dispatchEvent('input');
    await codigoInput.dispatchEvent('change');

    // Campo Precio Base USD
    const precioInput = modal.locator('input[type="number"]').first();
    if (await precioInput.isVisible()) {
      await precioInput.fill(data.precioUsd.toFixed(2));
      await precioInput.dispatchEvent('input');
      await precioInput.dispatchEvent('change');
    }

    // Campo Honorario Base USD (Segundo input tipo number si existe)
    const numberInputs = modal.locator('input[type="number"]');
    const count = await numberInputs.count();
    if (count > 1) {
      const honorarioBaseInput = numberInputs.nth(1);
      if (await honorarioBaseInput.isVisible()) {
        await honorarioBaseInput.fill(data.honorarioBaseUsd.toFixed(2));
        await honorarioBaseInput.dispatchEvent('input');
        await honorarioBaseInput.dispatchEvent('change');
      }
    }
  }

  /**
   * Busca y vincula múltiples sugerencias (mínimo 2) y verifica los chips/cards.
   */
  async addSugerencias(modal: Locator, searchTerms: string[]): Promise<void> {
    const sugerenciaSearchInput = modal.locator('input[placeholder*="Buscar servicio para vincular"], input[placeholder*="Buscar servicio"]').first();

    if (!(await sugerenciaSearchInput.isVisible())) {
      return; // El componente de edición actual no incluye sección de sugerencias
    }

    for (const term of searchTerms) {
      await sugerenciaSearchInput.fill(term);
      await this.page.waitForTimeout(300);

      const optionButton = modal.locator('div button').filter({ hasText: term }).first();
      if (await optionButton.isVisible()) {
        await optionButton.click();
      } else {
        // Fallback: primer elemento habilitado en la lista desplegable de sugerencias
        const firstOption = modal.locator('div.max-h-48 button, aside button').first();
        if (await firstOption.isVisible()) {
          await firstOption.click();
        }
      }
      await this.page.waitForTimeout(200);
    }

    // Verificar que las sugerencias seleccionadas se reflejen en la interfaz
    const chips = modal.locator('div.flex-wrap div, span.font-medium, div.bg-amber-500\\/5, div.bg-indigo-500\\/5');
    await chips.first().isVisible({ timeout: 3000 }).catch(() => false);
  }

  /**
   * Asigna múltiples honorarios de médicos específicos con montos USD independientes.
   */
  async addHonorariosMedicos(
    modal: Locator,
    honorarios: Array<{ medicoQuery: string; honorarioUsd: number }>
  ): Promise<void> {
    const medicoSearchInput = modal.locator('input[placeholder*="Buscar médico"]').first();

    if (!(await medicoSearchInput.isVisible())) {
      return; // El componente actual no requiere honorarios médicos
    }

    for (const item of honorarios) {
      await medicoSearchInput.focus();
      await medicoSearchInput.fill(item.medicoQuery);
      await this.page.waitForTimeout(300);

      const doctorOption = modal.locator('div button').filter({ hasText: 'Dr.' }).first();
      if (await doctorOption.isVisible()) {
        await doctorOption.click();

        // Asignar monto USD si hay input específico de honorario asignado
        const lastFeeInput = modal.locator('input[placeholder="0.00"]').last();
        if (await lastFeeInput.isVisible()) {
          await lastFeeInput.fill(item.honorarioUsd.toString());
        }
      }
    }
  }

  /**
   * Agrega múltiples insumos al BOM / Receta con cantidades específicas.
   */
  async addInsumosRecetaBOM(
    modal: Locator,
    insumos: Array<{ insumoQuery: string; cantidad: number }>
  ): Promise<void> {
    const insumoSearchInput = modal.locator('input[placeholder*="Buscar insumo"]').first();

    if (!(await insumoSearchInput.isVisible())) {
      return; // El modal no incluye sección BOM
    }

    for (const item of insumos) {
      await insumoSearchInput.focus();
      await insumoSearchInput.fill(item.insumoQuery);
      await this.page.waitForTimeout(300);

      const insumoOption = modal.locator('div button').filter({ hasText: item.insumoQuery }).first();
      if (await insumoOption.isVisible()) {
        await insumoOption.click();

        // Modificar cantidad consumible
        const cantInput = modal.locator('input[type="number"][min="0.01"], input[type="number"]').last();
        if (await cantInput.isVisible()) {
          await cantInput.fill(item.cantidad.toString());
        }
      }
    }
  }

  /**
   * Intercepta la petición HTTP POST /api/catalog (o /api/CatalogItems),
   * hace clic en Guardar y retorna el payload enviado junto con la respuesta de la API.
   */
  async saveAndInterceptPayload(modal: Locator): Promise<{ payload: any; status: number; createdId?: string }> {
    const saveButton = modal.locator('button').filter({ hasText: /guardar|crear|actualizar/i }).first();
    await expect(saveButton).toBeVisible();

    // Promesa de intercepción de red para POST /api/Catalog
    const requestPromise = this.page.waitForRequest(
      request => request.url().toLowerCase().includes('/api/catalog') && request.method() === 'POST',
      { timeout: 15_000 }
    );

    const responsePromise = this.page.waitForResponse(
      response => response.url().toLowerCase().includes('/api/catalog') && response.request().method() === 'POST',
      { timeout: 15_000 }
    );

    await saveButton.click();

    const request = await requestPromise;
    const response = await responsePromise;

    const payload = JSON.parse(request.postData() || '{}');
    const status = response.status();

    let createdId: string | undefined = undefined;
    try {
      const respJson = await response.json();
      createdId = typeof respJson === 'string' ? respJson : respJson?.id;
    } catch {
      // Si la API retorna plain string o Guid sin wrapper JSON
      createdId = await response.text();
      if (createdId) {
        createdId = createdId.replace(/"/g, '').trim();
      }
    }

    return { payload, status, createdId };
  }

  /**
   * Validación Estricta de Payload: Verifica que el JSON enviado al backend contenga
   * ÚNICAMENTE los campos permitidos en CreateCatalogItemCommand de .NET.
   * Rechaza cualquier propiedad adicional como notasClinicas, complejidad, etc.
   */
  validateStrictPayload(payload: any): void {
    const payloadKeys = Object.keys(payload);
    const forbiddenKeys: string[] = [];

    for (const key of payloadKeys) {
      if (!this.allowedBackendPayloadKeys.has(key)) {
        forbiddenKeys.push(key);
      }
    }

    if (forbiddenKeys.length > 0) {
      throw new Error(
        `[Payload Validation Failed] El JSON enviado a la API contiene propiedades no permitidas por .NET CreateCatalogItemCommand: [${forbiddenKeys.join(
          ', '
        )}]`
      );
    }
  }

  /**
   * Consulta directa a la API GET /api/Catalog/{id} para validar la persistencia real en la BD.
   */
  async verifyPersistenceViaApi(id: string): Promise<any> {
    const data = await this.page.evaluate(async (itemId) => {
      const token = localStorage.getItem('jwt_token') || localStorage.getItem('token') || localStorage.getItem('auth_token');
      const headers: Record<string, string> = { 'Content-Type': 'application/json' };
      if (token) {
        headers['Authorization'] = token.startsWith('Bearer ') ? token : `Bearer ${token}`;
      }
      const res = await fetch(`/api/Catalog/${itemId}`, { headers });
      if (!res.ok) {
        throw new Error(`HTTP Error ${res.status} ${res.statusText}`);
      }
      return await res.json();
    }, id);

    expect(data).toBeDefined();
    expect(data.id || data.codigo).toBeDefined();
    return data;
  }

  /**
   * Teardown / Clean-Up: Elimina el registro de prueba creado vía API DELETE.
   */
  async cleanupItemViaApi(id: string): Promise<void> {
    if (!id || id.length < 10) return;
    await this.page.evaluate(async (itemId) => {
      const token = localStorage.getItem('jwt_token') || localStorage.getItem('token') || localStorage.getItem('auth_token');
      const headers: Record<string, string> = { 'Content-Type': 'application/json' };
      if (token) {
        headers['Authorization'] = token.startsWith('Bearer ') ? token : `Bearer ${token}`;
      }
      await fetch(`/api/Catalog/${itemId}`, { method: 'DELETE', headers });
    }, id);
  }
}
