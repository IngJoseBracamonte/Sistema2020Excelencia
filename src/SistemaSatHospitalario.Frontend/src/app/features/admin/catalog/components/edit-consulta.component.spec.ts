import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditConsultaComponent } from './edit-consulta.component';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditConsultaComponent (E2E & Integration Flow)', () => {
  let component: EditConsultaComponent;
  let fixture: ComponentFixture<EditConsultaComponent>;
  let mockCatalogService: jasmine.SpyObj<CatalogService>;
  let mockInventoryService: jasmine.SpyObj<InventoryService>;
  let mockMedicoService: jasmine.SpyObj<MedicoService>;

  const mockConsultaItem: CatalogItem = {
    id: 'cons-101',
    codigo: 'CONS-CARD-001',
    descripcion: 'Consulta Cardiología Especializada',
    precioUsd: 50,
    honorarioBase: 30,
    tipo: 'CONSULTA',
    activo: true,
    sugerenciasIds: ['sug-1', 'sug-2'],
    honorariosMedicos: [
      { medicoId: 'med-1', medicoNombre: 'Dr. Pérez', honorario: 35 }
    ]
  } as unknown as CatalogItem;

  const mockMedicos = [
    { id: 'med-1', nombre: 'Dr. Pérez', apellido: 'Juan', especialidad: 'Cardiología', activo: true },
    { id: 'med-2', nombre: 'Dra. Gómez', apellido: 'María', especialidad: 'Neurología', activo: true }
  ];

  const mockSugerencias = [
    { id: 'sug-1', codigo: 'EKG-01', descripcion: 'Electrocardiograma 12 Derivaciones', tipo: 'PROCEDIMIENTO' } as CatalogItem,
    { id: 'sug-2', codigo: 'LAB-02', descripcion: 'Perfil Lipídico Completo', tipo: 'LABORATORIO' } as CatalogItem
  ];

  beforeEach(async () => {
    mockCatalogService = jasmine.createSpyObj('CatalogService', ['getItemById', 'getItems', 'createItem', 'updateItem']);
    mockCatalogService.getItemById.and.returnValue(of(mockConsultaItem));
    mockCatalogService.getItems.and.returnValue(of(mockSugerencias));
    mockCatalogService.updateItem.and.returnValue(of(true as any));
    mockCatalogService.createItem.and.returnValue(of('cons-101'));

    mockInventoryService = jasmine.createSpyObj('InventoryService', ['getInsumos', 'getRecetas', 'createOrUpdateRecipe']);
    mockInventoryService.getInsumos.and.returnValue(of([]));
    mockInventoryService.getRecetas.and.returnValue(of([]));
    mockInventoryService.createOrUpdateRecipe.and.returnValue(of(true as any));

    mockMedicoService = jasmine.createSpyObj('MedicoService', ['getAll']);
    mockMedicoService.getAll.and.returnValue(of(mockMedicos));

    await TestBed.configureTestingModule({
      imports: [EditConsultaComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: MedicoService, useValue: mockMedicoService },
        { provide: BillingFacadeService, useValue: { tasaCambioDia: signal(36.5) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditConsultaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería inicializar el formulario de edición de consulta correctamente', () => {
    expect(component).toBeTruthy();
    expect(mockMedicoService.getAll).toHaveBeenCalled();
    expect(mockCatalogService.getItems).toHaveBeenCalled();
  });

  it('debería cargar la consulta y poblar los honorarios de médicos y sugerencias asociadas', () => {
    (component as any).loadItem('cons-101');

    expect(component.nombre()).toBe('Consulta Cardiología Especializada');
    expect(component.codigo()).toBe('CONS-CARD-001');
    expect(component.precioBaseUsd()).toBe(50);
    expect(component.honorarioBase()).toBe(30);

    // Verificar honorarios específicos asignados
    expect(component.honorariosMedicos().length).toBe(1);
    expect(component.honorariosMedicos()[0].medicoId).toBe('med-1');
    expect(component.honorariosMedicos()[0].honorarioUsd).toBe(35);

    // Verificar sugerencias asociadas
    expect(component.selectedSugerenciasIds()).toEqual(['sug-1', 'sug-2']);
  });

  it('debería permitir asignar un nuevo médico con honorario específico y guardar la consulta', () => {
    (component as any).loadItem('cons-101');

    // Agregar segundo médico
    const newMedico = { id: 'med-2', nombre: 'Dra. María Gómez', especialidad: 'Neurología' };
    component.addMedicoToHonorarios(newMedico, 40);

    expect(component.honorariosMedicos().length).toBe(2);

    // Guardar cambios
    component.save();

    expect(mockCatalogService.updateItem).toHaveBeenCalledWith('cons-101', jasmine.objectContaining({
      descripcion: 'Consulta Cardiología Especializada',
      tipo: 'CONSULTA',
      sugerenciasIds: ['sug-1', 'sug-2'],
      honorariosMedicos: [
        { medicoId: 'med-1', honorario: 35 },
        { medicoId: 'med-2', honorario: 40 }
      ]
    }));
  });
});
