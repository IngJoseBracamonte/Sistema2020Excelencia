import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditMedicamentoComponent } from './edit-medicamento.component';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditMedicamentoComponent (Pharma & Inventory Flow)', () => {
  let component: EditMedicamentoComponent;
  let fixture: ComponentFixture<EditMedicamentoComponent>;

  let mockCatalogService: any;
  let mockInventoryService: any;
  let mockBillingFacade: any;
  let mockMedicoService: any;

  const mockMedItem: CatalogItem = {
    id: 'med-001',
    codigo: 'MED-IBU-400',
    descripcion: 'Ibuprofeno 400mg Comprimidos',
    precioUsd: 2.5,
    honorarioBase: 0,
    tipo: 'MEDICAMENTO',
    activo: true,
    sugerenciasIds: []
  } as unknown as CatalogItem;

  beforeEach(async () => {
    mockCatalogService = {
      getItemById: jasmine.createSpy('getItemById').and.returnValue(of(mockMedItem)),
      getUnifiedCatalog: jasmine.createSpy('getUnifiedCatalog').and.returnValue(of([mockMedItem])),
      getItems: jasmine.createSpy('getItems').and.returnValue(of([])),
      updateItem: jasmine.createSpy('updateItem').and.returnValue(of(true)),
      createItem: jasmine.createSpy('createItem').and.returnValue(of('med-001'))
    };

    mockInventoryService = {
      getInsumos: jasmine.createSpy('getInsumos').and.returnValue(of([])),
      getRecetas: jasmine.createSpy('getRecetas').and.returnValue(of([]))
    };

    mockBillingFacade = {
      tasaCambioDia: signal(36.5)
    };

    mockMedicoService = {
      getAll: jasmine.createSpy('getAll').and.returnValue(of([]))
    };

    await TestBed.configureTestingModule({
      imports: [EditMedicamentoComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: BillingFacadeService, useValue: mockBillingFacade },
        { provide: MedicoService, useValue: mockMedicoService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditMedicamentoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería instanciar el componente de farmacia e insumos', () => {
    expect(component).toBeTruthy();
  });

  it('debería cargar el medicamento y poblar campos básicos', () => {
    (component as any).loadItem('med-001');

    expect(component.nombre()).toBe('Ibuprofeno 400mg Comprimidos');
    expect(component.codigo()).toBe('MED-IBU-400');
    expect(component.precioBaseUsd()).toBe(2.5);
  });
});
