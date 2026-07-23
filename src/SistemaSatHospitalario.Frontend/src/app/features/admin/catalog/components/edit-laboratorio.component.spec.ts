import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditLaboratorioComponent } from './edit-laboratorio.component';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditLaboratorioComponent (Integration & Legacy Support)', () => {
  let component: EditLaboratorioComponent;
  let fixture: ComponentFixture<EditLaboratorioComponent>;

  let mockCatalogService: any;
  let mockInventoryService: any;
  let mockBillingFacade: any;
  let mockMedicoService: any;

  const mockLabItem: CatalogItem = {
    id: 'lab-001',
    codigo: 'LAB-HEM-01',
    descripcion: 'Hematología Completa',
    precioUsd: 15,
    honorarioBase: 5,
    tipo: 'LABORATORIO',
    activo: true,
    esLegacy: true,
    sugerenciasIds: []
  } as unknown as CatalogItem;

  beforeEach(async () => {
    mockCatalogService = {
      getItemById: jasmine.createSpy('getItemById').and.returnValue(of(mockLabItem)),
      getItems: jasmine.createSpy('getItems').and.returnValue(of([])),
      updateItem: jasmine.createSpy('updateItem').and.returnValue(of(true)),
      createItem: jasmine.createSpy('createItem').and.returnValue(of('lab-001'))
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
      imports: [EditLaboratorioComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: BillingFacadeService, useValue: mockBillingFacade },
        { provide: MedicoService, useValue: mockMedicoService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditLaboratorioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería instanciar el componente de laboratorio correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debería poblar los Signals del laboratorio al cargar un examen', () => {
    (component as any).loadItem('lab-001');

    expect(component.nombre()).toBe('Hematología Completa');
    expect(component.codigo()).toBe('LAB-HEM-01');
    expect(component.precioBaseUsd()).toBe(15);
    expect(component.honorarioBase()).toBe(5);
  });
});
