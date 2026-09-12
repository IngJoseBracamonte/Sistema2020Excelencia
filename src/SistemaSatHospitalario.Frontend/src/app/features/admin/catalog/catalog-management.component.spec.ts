import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CatalogManagementComponent } from './catalog-management.component';
import { CatalogService, CatalogItem } from '../../../core/services/catalog.service';
import { BillingFacadeService } from '../../../core/services/billing-facade.service';
import { ActivatedRoute } from '@angular/router';
import { of } from 'rxjs';
import { signal } from '@angular/core';
import { CatalogEditorType } from './models/catalog-edit.models';

describe('CatalogManagementComponent (Senior Refactoring - Procesos CRUD y Soft Delete)', () => {
  let component: CatalogManagementComponent;
  let fixture: ComponentFixture<CatalogManagementComponent>;
  let mockCatalogService: jasmine.SpyObj<CatalogService>;
  let mockBillingFacadeService: any;

  const mockCatalogItems: CatalogItem[] = [
    { id: '1', codigo: 'CONS-001', descripcion: 'Consulta Médica General', precioUsd: 30, tipo: 'CONSULTA', activo: true } as unknown as CatalogItem,
    { id: '2', codigo: 'LAB-001', descripcion: 'Perfil 20', precioUsd: 15, tipo: 'LABORATORIO', activo: true } as unknown as CatalogItem,
    { id: '3', codigo: 'CIR-001', descripcion: 'Apendicectomía', precioUsd: 1200, tipo: 'CIRUGIA', activo: false } as unknown as CatalogItem
  ];

  beforeEach(async () => {
    mockCatalogService = jasmine.createSpyObj('CatalogService', [
      'getUnifiedCatalog',
      'getItems',
      'createItem',
      'updateItem',
      'deleteItem',
      'reactivateItem'
    ]);
    mockCatalogService.getUnifiedCatalog.and.returnValue(of(mockCatalogItems));
    mockCatalogService.deleteItem.and.returnValue(of(true));
    mockCatalogService.reactivateItem.and.returnValue(of(true));

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

  it('debería crearse correctamente el componente', () => {
    expect(component).toBeTruthy();
  });

  describe('Control del Menú Desplegable de Creación', () => {
    it('debería iniciar con showCreateDropdown en false', () => {
      expect(component.showCreateDropdown()).toBeFalse();
    });

    it('debería alternar showCreateDropdown con toggleCreateDropdown', () => {
      component.toggleCreateDropdown();
      expect(component.showCreateDropdown()).toBeTrue();

      component.toggleCreateDropdown();
      expect(component.showCreateDropdown()).toBeFalse();
    });

    it('debería cerrar showCreateDropdown con closeCreateDropdown', () => {
      component.showCreateDropdown.set(true);
      component.closeCreateDropdown();
      expect(component.showCreateDropdown()).toBeFalse();
    });
  });

  describe('Apertura de CRUD Especializado en Modo Creación (openCreateOfType)', () => {
    const testCases: { type: CatalogEditorType; label: string }[] = [
      { type: 'CONSULTA', label: 'Consulta Médica' },
      { type: 'CIRUGIA', label: 'Cirugía' },
      { type: 'LABORATORIO', label: 'Laboratorio' },
      { type: 'TOMOGRAFIA', label: 'Tomografía' },
      { type: 'MEDICAMENTO', label: 'Medicamento' },
      { type: 'PROCEDIMIENTO', label: 'Procedimiento' },
      { type: 'HOSPITALARIO', label: 'Hospitalario' },
      { type: 'SERVICIO', label: 'Servicio Base' }
    ];

    testCases.forEach(({ type, label }) => {
      it(`debería abrir el CRUD especializado de ${label} en modo creación`, () => {
        component.showCreateDropdown.set(true);

        component.openCreateOfType(type);

        expect(component.showCreateDropdown()).toBeFalse();
        expect(component.isEditing()).toBeFalse();
        expect(component.selectedItemId()).toBeNull();
        expect(component.activeEditorType()).toBe(type);
        expect(component.showModal()).toBeTrue();
      });
    });
  });

  describe('Filtrado por Estado y Búsqueda', () => {
    it('debería mostrar todos los ítems (activos e inactivos) cuando filtroEstado es TODOS', () => {
      component.filtroEstado.set('TODOS');
      expect(component.filteredCatalog().length).toBe(3);
    });

    it('debería filtrar únicamente ítems activos cuando filtroEstado es ACTIVOS', () => {
      component.filtroEstado.set('ACTIVOS');
      const items = component.filteredCatalog();
      expect(items.length).toBe(2);
      expect(items.every(i => i.activo)).toBeTrue();
    });

    it('debería filtrar únicamente ítems desactivados cuando filtroEstado es DESACTIVADOS', () => {
      component.filtroEstado.set('DESACTIVADOS');
      const items = component.filteredCatalog();
      expect(items.length).toBe(1);
      expect(items[0].codigo).toBe('CIR-001');
      expect(items[0].activo).toBeFalse();
    });
  });

  describe('Soft Delete Persistente y Reactivación', () => {
    it('debería ejecutar executeDelete marcando activo = false sin eliminar el ítem de la grilla', () => {
      const itemToDelete = mockCatalogItems[0];
      component.confirmDelete(itemToDelete);

      expect(component.itemToDelete()).toEqual(itemToDelete);

      component.executeDelete();

      expect(mockCatalogService.deleteItem).toHaveBeenCalledWith('1');
      expect(component.itemToDelete()).toBeNull();
      
      // El ítem permanece en la lista pero con activo = false
      const updatedItem = component.catalog().find(i => i.id === '1');
      expect(updatedItem).toBeDefined();
      expect(updatedItem?.activo).toBeFalse();
    });

    it('debería ejecutar reactivateItem marcando activo = true y llamando al servicio', () => {
      const itemToReactivate = mockCatalogItems[2]; // CIR-001 está inactivo
      expect(itemToReactivate.activo).toBeFalse();

      component.reactivateItem(itemToReactivate);

      expect(mockCatalogService.reactivateItem).toHaveBeenCalledWith('3');
      const updated = component.catalog().find(i => i.id === '3');
      expect(updated?.activo).toBeTrue();
    });
  });
});
