import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditProcedimientoComponent } from './edit-procedimiento.component';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditProcedimientoComponent (Fallback & General Procedure)', () => {
  let component: EditProcedimientoComponent;
  let fixture: ComponentFixture<EditProcedimientoComponent>;

  let mockCatalogService: any;
  let mockInventoryService: any;
  let mockBillingFacade: any;
  let mockMedicoService: any;

  const mockProcItem: CatalogItem = {
    id: 'proc-001',
    codigo: 'PROC-CUR-01',
    descripcion: 'Curación Mayor y Retiro de Puntos',
    precioUsd: 25,
    honorarioBase: 10,
    tipo: 'PROCEDIMIENTO',
    activo: true,
    sugerenciasIds: []
  } as unknown as CatalogItem;

  beforeEach(async () => {
    mockCatalogService = {
      getItemById: jasmine.createSpy('getItemById').and.returnValue(of(mockProcItem)),
      getUnifiedCatalog: jasmine.createSpy('getUnifiedCatalog').and.returnValue(of([mockProcItem])),
      getItems: jasmine.createSpy('getItems').and.returnValue(of([])),
      updateItem: jasmine.createSpy('updateItem').and.returnValue(of(true)),
      createItem: jasmine.createSpy('createItem').and.returnValue(of('proc-001'))
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
      imports: [EditProcedimientoComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: BillingFacadeService, useValue: mockBillingFacade },
        { provide: MedicoService, useValue: mockMedicoService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditProcedimientoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería instanciar el componente de procedimiento fallback', () => {
    expect(component).toBeTruthy();
  });

  it('debería poblar el procedimiento base correctamente', () => {
    (component as any).loadItem('proc-001');

    expect(component.nombre()).toBe('Curación Mayor y Retiro de Puntos');
    expect(component.codigo()).toBe('PROC-CUR-01');
    expect(component.precioBaseUsd()).toBe(25);
  });
});
