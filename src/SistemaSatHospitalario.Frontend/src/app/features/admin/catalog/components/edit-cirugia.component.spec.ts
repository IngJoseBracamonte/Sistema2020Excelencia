import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditCirugiaComponent } from './edit-cirugia.component';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditCirugiaComponent (Surgical & Operating Room Flow)', () => {
  let component: EditCirugiaComponent;
  let fixture: ComponentFixture<EditCirugiaComponent>;

  let mockCatalogService: any;
  let mockInventoryService: any;
  let mockBillingFacade: any;
  let mockMedicoService: any;

  const mockCirugiaItem: CatalogItem = {
    id: 'cir-001',
    codigo: 'CIR-APP-01',
    descripcion: 'Apendicectomía Laparoscópica',
    precioUsd: 450,
    precioBaseUsd: 450,
    honorarioBase: 150,
    tipo: 'CIRUGIA',
    activo: true,
    complejidad: 'ALTA',
    duracionEstimadaMinutos: 90,
    requiereAnestesia: true,
    tipoAnestesia: 'GENERAL',
    clasificacionRiesgo: 'ASA II',
    sugerenciasIds: []
  } as unknown as CatalogItem;

  beforeEach(async () => {
    mockCatalogService = {
      getItemById: jasmine.createSpy('getItemById').and.returnValue(of(mockCirugiaItem)),
      getItems: jasmine.createSpy('getItems').and.returnValue(of([])),
      updateItem: jasmine.createSpy('updateItem').and.returnValue(of(true)),
      createItem: jasmine.createSpy('createItem').and.returnValue(of('cir-001'))
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
      imports: [EditCirugiaComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: BillingFacadeService, useValue: mockBillingFacade },
        { provide: MedicoService, useValue: mockMedicoService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditCirugiaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería instanciar el componente de edición quirúrgica', () => {
    expect(component).toBeTruthy();
  });

  it('debería cargar la intervención quirúrgica y poblar complejidad y anestesia', () => {
    (component as any).loadItem('cir-001');

    expect(component.nombre()).toBe('Apendicectomía Laparoscópica');
    expect(component.codigo()).toBe('CIR-APP-01');
    expect(component.precioBaseUsd()).toBe(450);
    expect(component.complejidad()).toBe('ALTA');
    expect(component.duracionEstimadaMinutos()).toBe(90);
    expect(component.tipoAnestesia()).toBe('GENERAL');
  });

  it('debería permitir asignar honorarios por rol quirúrgico (Cirujano, Anestesiólogo)', () => {
    component.addHonorarioRol();
    component.updateHonorarioRol(0, 'rol', 'Cirujano Principal');
    component.updateHonorarioRol(0, 'honorarioUsd', 200);

    expect(component.honorariosEquipo().length).toBe(1);
    expect(component.honorariosEquipo()[0].rol).toBe('Cirujano Principal');
    expect(component.honorariosEquipo()[0].honorarioUsd).toBe(200);
  });
});
