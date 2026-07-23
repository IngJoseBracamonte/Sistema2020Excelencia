import { test, expect } from '@playwright/test';
import { CatalogPage, CatalogType } from './catalog.page';

test.describe('Suite E2E Integrada - Registro y Validación de Catálogo por Tipo (8 Tipos)', () => {
  let catalogPage: CatalogPage;

  test.beforeEach(async ({ page }) => {
    catalogPage = new CatalogPage(page);
    await catalogPage.gotoCatalog();
  });

  const catalogTypes: Array<{
    type: CatalogType;
    categoryDescription: string;
    codePrefix: string;
    precioBase: number;
    honorarioBase: number;
  }> = [
    {
      type: 'SERVICIO',
      categoryDescription: 'Servicios Generales, Traslados y Logística',
      codePrefix: 'SERV',
      precioBase: 120.00,
      honorarioBase: 30.00
    },
    {
      type: 'CONSULTA',
      categoryDescription: 'Consultas Médicas Generales y Especializadas',
      codePrefix: 'CON',
      precioBase: 80.00,
      honorarioBase: 50.00
    },
    {
      type: 'CIRUGIA',
      categoryDescription: 'Procedimientos Quirúrgicos de Alta Complejidad',
      codePrefix: 'CIR',
      precioBase: 450.00,
      honorarioBase: 150.00
    },
    {
      type: 'TOMOGRAFIA',
      categoryDescription: 'Estudios Tomográficos Especializados',
      codePrefix: 'TOMO',
      precioBase: 210.00,
      honorarioBase: 45.00
    },
    {
      type: 'HOSPITALARIO',
      categoryDescription: 'Cargos por Estancia, UCI y Emergencia',
      codePrefix: 'HOSP',
      precioBase: 350.00,
      honorarioBase: 60.00
    },
    {
      type: 'LABORATORIO',
      categoryDescription: 'Análisis Clínicos y Perfiles Biológicos',
      codePrefix: 'LAB',
      precioBase: 65.00,
      honorarioBase: 15.00
    },
    {
      type: 'RX',
      categoryDescription: 'Estudios Radiológicos y Rayos X',
      codePrefix: 'RX',
      precioBase: 95.00,
      honorarioBase: 25.00
    },
    {
      type: 'PROCEDIMIENTO',
      categoryDescription: 'Procedimientos Ambulatorios y Curaciones de Enfermería',
      codePrefix: 'PROC',
      precioBase: 110.00,
      honorarioBase: 35.00
    }
  ];

  for (const itemSpec of catalogTypes) {
    test(`Flujo Completo E2E para Tipo ${itemSpec.type} (${itemSpec.categoryDescription})`, async ({ page }) => {
      const timestamp = Date.now().toString().slice(-6);
      const nombreItem = `E2E-TEST-${itemSpec.type}-${timestamp}`;
      const codigoItem = `E2E-${itemSpec.codePrefix}-${timestamp}`;

      console.log(`\n[E2E TEST RUNNING] Tipo: ${itemSpec.type} | Código: ${codigoItem} | Nombre: ${nombreItem}`);

      // 1. Apertura del Modal para el tipo actual
      const modal = await catalogPage.openCreateModalForType(itemSpec.type);
      await expect(modal).toBeVisible();

      // 2. Diligenciar Campos Básicos Requeridos
      await catalogPage.fillBasicFields(modal, {
        nombre: nombreItem,
        codigo: codigoItem,
        precioUsd: itemSpec.precioBase,
        honorarioBaseUsd: itemSpec.honorarioBase
      });

      // 3. Vincular Múltiples Sugerencias (Mínimo 2)
      await catalogPage.addSugerencias(modal, ['Atención', 'Perfil']);

      // 4. Asignar Múltiples Honorarios Médicos Específicos (Mínimo 2)
      await catalogPage.addHonorariosMedicos(modal, [
        { medicoQuery: 'Dr. Pérez', honorarioUsd: 50.00 },
        { medicoQuery: 'Dr. Gómez', honorarioUsd: 35.00 }
      ]);

      // 5. Asignar Múltiples Insumos en Receta / BOM (Mínimo 2)
      await catalogPage.addInsumosRecetaBOM(modal, [
        { insumoQuery: 'Jeringa', cantidad: 2.5 },
        { insumoQuery: 'Gasa', cantidad: 4.0 }
      ]);

      // 6. Guardado e Intercepción de Petición HTTP POST
      const { payload, status, createdId } = await catalogPage.saveAndInterceptPayload(modal);

      // Aserciones de Respuesta HTTP
      expect([200, 201]).toContain(status);
      console.log(`[API RESPONSE] HTTP Status: ${status} | Created ID: ${createdId}`);

      // 7. Validación Estricta de Payload contra CreateCatalogItemCommand (.NET)
      // Garantizar que no se envíen campos no mapeados (como notasClinicas, complejidad, etc.)
      catalogPage.validateStrictPayload(payload);
      expect(payload.descripcion).toBe(nombreItem);
      expect(payload.codigo).toBe(codigoItem);
      expect(payload.precioUsd).toBe(itemSpec.precioBase);
      expect(payload.activo).toBe(true);

      // 8. Verificación de Persistencia real en la Base de Datos vía API GET
      if (createdId && createdId.length >= 10) {
        const persistedData = await catalogPage.verifyPersistenceViaApi(createdId);
        expect(persistedData).toBeDefined();
        console.log(`[DB VERIFICATION] Ítem ${createdId} verificado exitosamente en la BD.`);

        // 9. Teardown / Clean-Up: Eliminar registro de prueba de la BD local
        await catalogPage.cleanupItemViaApi(createdId);
        console.log(`[TEARDOWN COMPLETED] Ítem ${createdId} eliminado limpiamente de la BD.`);
      } else {
        console.warn(`[TEARDOWN SKIPPED] No se obtuvo un ID válido de respuesta para teardown.`);
      }
    });
  }
});
