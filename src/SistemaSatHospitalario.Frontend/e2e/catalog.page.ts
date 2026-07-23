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
    'honorariosMedicos'
  ]);

  constructor(public readonly page: Page) {}

  /**
   * Navega a la vista de Maestro de Servicios. Si detecta la pantalla de login,
   * realiza la autenticación automática de prueba.
   */
  async gotoCatalog(): Promise<void> {
    await this.page.goto('/admin/catalog');
    await this.page.waitForLoadState('networkidle');

    // Manejo de autenticación si fue redirigido al Login
    if (this.page.url().includes('/login') || (await this.page.locator('input#username').isVisible())) {
      await this.page.fill('input#username', 'admin');
      await this.page.fill('input#password', 'Admin123*!');
      await this.page.click('button[type="submit"]');
      await this.page.waitForURL('**/dashboard');
      await this.page.goto('/admin/catalog');
      await this.page.waitForLoadState('networkidle');
    }

    // Verificar presencia del contenedor principal
    const titleLocator = this.page.locator('h1, h2, div').filter({ hasText: 'MAESTRO DE SERVICIOS' }).first();
    await expect(titleLocator).toBeVisible({ timeout: 15_000 });
  }

  /**
   * Abre el modal de creación correspondiente para el tipo de catálogo especificado.
   */
  async openCreateModalForType(type: CatalogType): Promise<Locator> {
    // 1. Filtrar por el tipo deseado si existe el chip de filtro
    const filterChip = this.page.locator('button', { hasText: type }).first();
    if (await filterChip.isVisible()) {
      await filterChip.click();
      await this.page.waitForTimeout(300);
    }

    // 2. Hacer clic en "NUEVO SERVICIO"
    const createBtn = this.page.locator('button:has-text("NUEVO SERVICIO")').first();
    await expect(createBtn).toBeVisible({ timeout: 10_000 });
    await createBtn.click();

    // 3. Determinar el tag del selector del modal de Angular
    let modalTag = 'app-edit-servicio';
    switch (type) {
      case 'CONSULTA':
        modalTag = 'app-edit-consulta';
        break;
      case 'CIRUGIA':
        modalTag = 'app-edit-cirugia';
        break;
      case 'TOMOGRAFIA':
        modalTag = 'app-edit-tomografia';
        break;
      case 'HOSPITALARIO':
        modalTag = 'app-edit-hospitalario';
        break;
      case 'LABORATORIO':
        modalTag = 'app-edit-laboratorio';
        break;
      case 'PROCEDIMIENTO':
        modalTag = 'app-edit-procedimiento';
        break;
      case 'RX':
      case 'SERVICIO':
      default:
        modalTag = 'app-edit-servicio';
        break;
    }

    const modalLocator = this.page.locator(modalTag);
    await expect(modalLocator).toBeVisible({ timeout: 10_000 });
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

    // Campo Código
    const codigoInput = modal.locator('input[placeholder*="SERV-"], input[placeholder*="Código"], input[placeholder*="CIR-"]').first();
    if (await codigoInput.isVisible()) {
      await codigoInput.fill(data.codigo);
    }

    // Campo Precio Base USD
    const precioInput = modal.locator('input[type="number"]').first();
    if (await precioInput.isVisible()) {
      await precioInput.fill(data.precioUsd.toFixed(2));
    }

    // Campo Honorario Base USD (si está presente)
    const honorarioBaseInput = modal.locator('input[placeholder="0.00"]').first();
    if (await honorarioBaseInput.isVisible()) {
      await honorarioBaseInput.fill(data.honorarioBaseUsd.toFixed(2));
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

    // Verificar que los chips / cards de confirmación se muestren
    const chips = modal.locator('div.flex-wrap div, span.font-medium');
    await expect(chips.first()).toBeVisible({ timeout: 5000 });
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
    const saveButton = modal.locator('button:has-text("Guardar")').first();
    await expect(saveButton).toBeVisible();

    // Promesa de intercepción de red para POST /api/catalog
    const requestPromise = this.page.waitForRequest(
      request => request.url().includes('/api/catalog') && request.method() === 'POST',
      { timeout: 15_000 }
    );

    const responsePromise = this.page.waitForResponse(
      response => response.url().includes('/api/catalog') && response.request().method() === 'POST',
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
   * Consulta directa a la API GET /api/catalog/{id} para validar la persistencia real en la BD.
   */
  async verifyPersistenceViaApi(id: string): Promise<any> {
    const apiResponse = await this.page.request.get(`/api/catalog/${id}`);
    expect(apiResponse.status()).toBe(200);

    const data = await apiResponse.json();
    expect(data.id || data.codigo).toBeDefined();
    return data;
  }

  /**
   * Teardown / Clean-Up: Elimina el registro de prueba creado vía API DELETE.
   */
  async cleanupItemViaApi(id: string): Promise<void> {
    if (!id || id.length < 10) return;
    const deleteResponse = await this.page.request.delete(`/api/catalog/${id}`);
    // Acepta 200 OK o 204 No Content
    expect([200, 204]).toContain(deleteResponse.status());
  }
}
