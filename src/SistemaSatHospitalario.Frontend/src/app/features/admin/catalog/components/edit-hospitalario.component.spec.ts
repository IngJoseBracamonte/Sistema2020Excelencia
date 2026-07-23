import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditHospitalarioComponent } from './edit-hospitalario.component';
import { CatalogService, CatalogItem } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('EditHospitalarioComponent', () => {
  let component: EditHospitalarioComponent;
  let fixture: ComponentFixture<EditHospitalarioComponent>;
  let mockCatalogService: jasmine.SpyObj<CatalogService>;
  let mockInventoryService: jasmine.SpyObj<InventoryService>;
  let mockMedicoService: jasmine.SpyObj<MedicoService>;

  const mockHospitalarioItem: CatalogItem = {
    id: 'hosp-101',
    codigo: 'HOSP-UCI-001',
    descripcion: 'Cargo por Estancia / Traslado a UCI',
    precioUsd: 150,
    honorarioBase: 50,
    tipo: 'HOSPITALARIO',
    activo: true,
    sugerenciasIds: []
  } as unknown as CatalogItem;

  beforeEach(async () => {
    mockCatalogService = jasmine.createSpyObj('CatalogService', ['getItemById', 'createItem', 'updateItem', 'getItems', 'getUnifiedCatalog']);
    mockCatalogService.getItemById.and.returnValue(of(mockHospitalarioItem));
    mockCatalogService.updateItem.and.returnValue(of(true as any));
    mockCatalogService.createItem.and.returnValue(of('hosp-101'));
    mockCatalogService.getItems.and.returnValue(of([]));
    mockCatalogService.getUnifiedCatalog.and.returnValue(of([]));

    mockInventoryService = jasmine.createSpyObj('InventoryService', ['getInsumos', 'getRecetas', 'createOrUpdateRecipe']);
    mockInventoryService.getInsumos.and.returnValue(of([]));
    mockInventoryService.getRecetas.and.returnValue(of([]));

    mockMedicoService = jasmine.createSpyObj('MedicoService', ['getAll']);
    mockMedicoService.getAll.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [EditHospitalarioComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: MedicoService, useValue: mockMedicoService },
        { provide: BillingFacadeService, useValue: { tasaCambioDia: signal(36.5) } }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EditHospitalarioComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería inicializar el componente de edición hospitalaria', () => {
    expect(component).toBeTruthy();
  });

  it('debería cargar y poblar los campos del servicio hospitalario', () => {
    (component as any).loadItem('hosp-101');

    expect(component.nombre()).toBe('Cargo por Estancia / Traslado a UCI');
    expect(component.codigo()).toBe('HOSP-UCI-001');
    expect(component.precioBaseUsd()).toBe(150);
  });

  it('debería actualizar un cargo hospitalario y llamar a CatalogService.updateItem', () => {
    (component as any).loadItem('hosp-101');
    component.precioBaseUsd.set(200);

    component.save();

    expect(mockCatalogService.updateItem).toHaveBeenCalledWith('hosp-101', jasmine.objectContaining({
      descripcion: 'Cargo por Estancia / Traslado a UCI',
      tipo: 'HOSPITALARIO',
      precioUsd: 200
    }));
  });
});
