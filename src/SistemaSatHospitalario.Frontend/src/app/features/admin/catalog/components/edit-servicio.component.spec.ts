import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditServicioComponent } from './edit-servicio.component'
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditServicioComponent (Base Service Catalog Editor)', () => {
  let component: EditServicioComponent;
  let fixture: ComponentFixture<EditServicioComponent>;

  let mockCatalogService: any;
  let mockInventoryService: any;
  let mockBillingFacade: any;
  let mockMedicoService: any;

  const mockServicioItem: CatalogItem = {
    id: 'serv-001',
    codigo: 'SERV-001',
    descripcion: 'Atención Médica General',
    precioUsd: 30,
    tipo: 'SERVICIO',
    editorType: 'SERVICIO',
    activo: true,
    sugerenciasIds: ['serv-002']
  } as unknown as CatalogItem;

  beforeEach(async () => {
    mockCatalogService = {
      getItemById: jasmine.createSpy('getItemById').and.returnValue(of(mockServicioItem)),
      getUnifiedCatalog: jasmine.createSpy('getUnifiedCatalog').and.returnValue(of([mockServicioItem])),
      getItems: jasmine.createSpy('getItems').and.returnValue(of([mockServicioItem])),
      updateItem: jasmine.createSpy('updateItem').and.returnValue(of(true)),
      createItem: jasmine.createSpy('createItem').and.returnValue(of('serv-001'))
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
      imports: [EditServicioComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: BillingFacadeService, useValue: mockBillingFacade },
        { provide: MedicoService, useValue: mockMedicoService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditServicioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería instanciar correctamente el componente de edición de Servicio Base', () => {
    expect(component).toBeTruthy();
  });

  it('debería poblar la información del servicio base al cargar un ID', () => {
    (component as any).loadItem('serv-001');

    expect(component.nombre()).toBe('Atención Médica General');
    expect(component.codigo()).toBe('SERV-001');
    expect(component.precioBaseUsd()).toBe(30);
    expect(component.selectedSugerenciasIds()).toEqual(['serv-002']);
  });

  it('debería guardar un nuevo servicio base con tipo SERVICIO', () => {
    spyOn(component.saved, 'emit');
    spyOn(component.closed, 'emit');

    component.nombre.set('Servicio General Diagnóstico');
    component.codigo.set('SERV-DIAG-01');
    component.precioBaseUsd.set(50);
    component.activo.set(true);

    component.save();

    expect(mockCatalogService.createItem).toHaveBeenCalledWith(jasmine.objectContaining({
      descripcion: 'Servicio General Diagnóstico',
      codigo: 'SERV-DIAG-01',
      precioUsd: 50,
      tipo: 'SERVICIO',
      activo: true
    }));
    expect(component.saved.emit).toHaveBeenCalled();
  });
});
