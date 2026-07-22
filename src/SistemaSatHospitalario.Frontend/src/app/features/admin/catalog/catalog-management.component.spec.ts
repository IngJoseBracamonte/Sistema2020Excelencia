import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CatalogManagementComponent } from './catalog-management.component';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
import { BillingFacadeService } from '../../../core/services/billing-facade.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('CatalogManagementComponent (Senior Refactoring)', () => {
  let component: CatalogManagementComponent;
  let fixture: ComponentFixture<CatalogManagementComponent>;
  let mockCatalogService: jasmine.SpyObj<CatalogService>;
  let mockBillingFacadeService: any;

  const mockCatalogItems: CatalogItem[] = [
    { id: '1', codigo: 'CONS-001', descripcion: 'Consulta Médica General', precioUsd: 30, tipo: 'CONSULTA', activo: true } as unknown as CatalogItem,
    { id: '2', codigo: 'LAB-001', descripcion: 'Perfil 20', precioUsd: 15, tipo: 'LABORATORIO', activo: true } as unknown as CatalogItem
  ];

  beforeEach(async () => {
    mockCatalogService = jasmine.createSpyObj('CatalogService', ['getUnifiedCatalog', 'createItem', 'updateItem', 'deleteItem']);
    mockCatalogService.getUnifiedCatalog.and.returnValue(of(mockCatalogItems));
    mockCatalogService.deleteItem.and.returnValue(of(true));

    mockBillingFacadeService = {
      tasaCambioDia: signal(36.5)
    };

    await TestBed.configureTestingModule({
      imports: [CatalogManagementComponent],
      providers: [
        { provide: CatalogService, useValue: mockCatalogService },
        { provide: BillingFacadeService, useValue: mockBillingFacadeService },
        {
          provide: ActivatedRoute,
          useValue: {
            queryParams: of({})
          }
        }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CatalogManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debería crearse correctamente el componente refactorizado', () => {
    expect(component).toBeTruthy();
  });

  it('debería calcular reactivamente filteredCatalog con computed signals', () => {
    expect(component.filteredCatalog().length).toBe(2);
    
    // Filtrar por búsqueda
    component.searchQuery.set('Perfil');
    expect(component.filteredCatalog().length).toBe(1);
    expect(component.filteredCatalog()[0].codigo).toBe('LAB-001');

    // Limpiar filtro
    component.clearAllFilters();
    expect(component.filteredCatalog().length).toBe(2);
  });

  it('debería registrar y ejecutar confirmDelete y executeDelete correctamente', () => {
    const itemToDelete = mockCatalogItems[0];
    component.confirmDelete(itemToDelete);

    expect(component.itemToDelete()).toEqual(itemToDelete);

    component.executeDelete();

    expect(mockCatalogService.deleteItem).toHaveBeenCalledWith('1');
    expect(component.itemToDelete()).toBeNull();
  });

  it('debería cancelar la eliminación al llamar cancelDelete', () => {
    component.confirmDelete(mockCatalogItems[0]);
    expect(component.itemToDelete()).not.toBeNull();

    component.cancelDelete();
    expect(component.itemToDelete()).toBeNull();
  });
});
