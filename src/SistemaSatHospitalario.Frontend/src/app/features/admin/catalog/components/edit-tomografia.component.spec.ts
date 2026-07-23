import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditTomografiaComponent } from './edit-tomografia.component';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditTomografiaComponent (Imaging & Contrast Flow)', () => {
  let component: EditTomografiaComponent;
  let fixture: ComponentFixture<EditTomografiaComponent>;

  let mockCatalogService: any;
  let mockInventoryService: any;
  let mockBillingFacade: any;
  let mockMedicoService: any;

  const mockTomoItem: CatalogItem = {
    id: 'tomo-001',
    codigo: 'TOMO-CRANEO-01',
    descripcion: 'Tomografía Computarizada de Cráneo Simple y Con Contraste',
    precioUsd: 120,
    honorarioBase: 40,
    tipo: 'TOMOGRAFIA',
    activo: true,
    requiereContraste: true,
    protocoloTecnico: 'Corte Helicoidal 1mm axial',
    sugerenciasIds: []
  } as unknown as CatalogItem;

  beforeEach(async () => {
    mockCatalogService = {
      getItemById: jasmine.createSpy('getItemById').and.returnValue(of(mockTomoItem)),
      getUnifiedCatalog: jasmine.createSpy('getUnifiedCatalog').and.returnValue(of([mockTomoItem])),
      getItems: jasmine.createSpy('getItems').and.returnValue(of([])),
      updateItem: jasmine.createSpy('updateItem').and.returnValue(of(true)),
      createItem: jasmine.createSpy('createItem').and.returnValue(of('tomo-001'))
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
      imports: [EditTomografiaComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: BillingFacadeService, useValue: mockBillingFacade },
        { provide: MedicoService, useValue: mockMedicoService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditTomografiaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crear el componente de tomografía correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debería cargar los datos de tomografía y activar el flag de contraste', () => {
    (component as any).loadItem('tomo-001');

    expect(component.nombre()).toBe('Tomografía Computarizada de Cráneo Simple y Con Contraste');
    expect(component.codigo()).toBe('TOMO-CRANEO-01');
    expect(component.precioBaseUsd()).toBe(120);
    expect(component.honorarioBase()).toBe(40);
    expect(component.requiereContraste()).toBeTrue();
    expect(component.protocoloTecnico()).toBe('Corte Helicoidal 1mm axial');
  });

  it('debería guardar la tomografía incluyendo requerimiento de contraste y protocolo técnico', () => {
    (component as any).loadItem('tomo-001');

    component.protocoloTecnico.set('Corte 0.5mm Reconstrucción 3D');
    component.save();

    expect(mockCatalogService.updateItem).toHaveBeenCalledWith('tomo-001', jasmine.objectContaining({
      tipo: 'TOMOGRAFIA',
      requiereContraste: true,
      protocoloTecnico: 'Corte 0.5mm Reconstrucción 3D'
    }));
  });
});
