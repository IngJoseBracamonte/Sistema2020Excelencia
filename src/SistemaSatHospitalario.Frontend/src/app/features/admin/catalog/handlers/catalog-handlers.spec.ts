import { CatalogBOMHandler } from './catalog-bom.handler';
import { CatalogHonorariosHandler } from './catalog-honorarios.handler';
import { CatalogSugerenciasHandler } from './catalog-sugerencias.handler';
import { Insumo } from '../../../../core/models/inventory.model';
import { CatalogItem } from '../../../../core/models/priced-item.model';

describe('Catalog Handlers (Angular 19 Signals)', () => {
  describe('CatalogBOMHandler', () => {
    let bomHandler: CatalogBOMHandler;
    const mockInsumo = {
      id: 'ins-1',
      codigo: 'INS-001',
      nombre: 'JERINGA 5ML',
      unidadMedidaBase: 'UND',
      costoUnitarioBaseUSD: 0.2,
      stockActual: 100
    } as Insumo;

    beforeEach(() => {
      bomHandler = new CatalogBOMHandler();
      bomHandler.availableInsumos.set([mockInsumo]);
    });

    it('should add an insumo to BOM lines', () => {
      bomHandler.addInsumo(mockInsumo, 2);
      expect(bomHandler.bomLines().length).toBe(1);
      expect(bomHandler.bomLines()[0].insumoId).toBe('ins-1');
      expect(bomHandler.bomLines()[0].cantidad).toBe(2);
    });

    it('should update quantity if insumo already exists in BOM lines', () => {
      bomHandler.addInsumo(mockInsumo, 2);
      bomHandler.addInsumo(mockInsumo, 3);
      expect(bomHandler.bomLines().length).toBe(1);
      expect(bomHandler.bomLines()[0].cantidad).toBe(5);
    });

    it('should filter available insumos based on search query', () => {
      bomHandler.insumoSearchQuery.set('JERINGA');
      expect(bomHandler.filteredInsumos().length).toBe(1);

      bomHandler.insumoSearchQuery.set('ALGODON');
      expect(bomHandler.filteredInsumos().length).toBe(0);
    });

    it('should remove a line from BOM', () => {
      bomHandler.addInsumo(mockInsumo, 2);
      bomHandler.removeLine('ins-1');
      expect(bomHandler.bomLines().length).toBe(0);
    });
  });

  describe('CatalogHonorariosHandler', () => {
    let honorariosHandler: CatalogHonorariosHandler;
    const mockMedico = {
      id: 'med-1',
      nombre: 'DR. PEREZ',
      especialidad: 'CARDIOLOGIA'
    };

    beforeEach(() => {
      honorariosHandler = new CatalogHonorariosHandler();
      honorariosHandler.availableMedicos.set([mockMedico]);
    });

    it('should add a medical fee entry', () => {
      honorariosHandler.addHonorario(mockMedico, 50);
      expect(honorariosHandler.honorarios().length).toBe(1);
      expect(honorariosHandler.honorarios()[0].medicoId).toBe('med-1');
      expect(honorariosHandler.honorarios()[0].honorarioUsd).toBe(50);
    });

    it('should update honorario USD value', () => {
      honorariosHandler.addHonorario(mockMedico, 50);
      honorariosHandler.updateHonorarioUsd('med-1', 75);
      expect(honorariosHandler.honorarios()[0].honorarioUsd).toBe(75);
    });

    it('should filter available medicos excluding already added ones', () => {
      honorariosHandler.addHonorario(mockMedico, 50);
      expect(honorariosHandler.filteredMedicos().length).toBe(0);
    });
  });

  describe('CatalogSugerenciasHandler', () => {
    let sugerenciasHandler: CatalogSugerenciasHandler;
    const mockCatalogItem = {
      id: 'item-1',
      codigo: 'LAB-001',
      descripcion: 'HEMOGRAMA COMPLETO',
      tipo: 'LABORATORIO',
      precioUsd: 15,
      honorarioBase: 0,
      activo: true
    } as CatalogItem;

    beforeEach(() => {
      sugerenciasHandler = new CatalogSugerenciasHandler();
      sugerenciasHandler.allCatalogItems.set([mockCatalogItem]);
    });

    it('should toggle suggestion selection', () => {
      sugerenciasHandler.toggleSugerencia('item-1');
      expect(sugerenciasHandler.sugerenciasIds()).toContain('item-1');
      expect(sugerenciasHandler.selectedSugerenciasCards().length).toBe(1);

      sugerenciasHandler.toggleSugerencia('item-1');
      expect(sugerenciasHandler.sugerenciasIds()).not.toContain('item-1');
      expect(sugerenciasHandler.selectedSugerenciasCards().length).toBe(0);
    });
  });
});
