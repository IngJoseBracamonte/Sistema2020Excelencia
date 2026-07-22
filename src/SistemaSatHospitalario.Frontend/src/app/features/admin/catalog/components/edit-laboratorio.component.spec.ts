import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditLaboratorioComponent } from './edit-laboratorio.component';
import { CatalogService } from '../../../../core/services/catalog.service';
import { InventoryService } from '../../../../core/services/inventory.service';
import { BillingFacadeService } from '../../../../core/services/billing-facade.service';
import { MedicoService } from '../../../../core/services/medico.service';
import { of } from 'rxjs';

describe('EditLaboratorioComponent', () => {
  let component: EditLaboratorioComponent;
  let fixture: ComponentFixture<EditLaboratorioComponent>;

  let mockCatalogService: any;
  let mockInventoryService: any;
  let mockBillingFacade: any;
  let mockMedicoService: any;

  beforeEach(async () => {
    mockCatalogService = {
      getItemById: jasmine.createSpy('getItemById').and.returnValue(of({})),
      getItems: jasmine.createSpy('getItems').and.returnValue(of([])),
      saveCatalogItem: jasmine.createSpy('saveCatalogItem').and.returnValue(of({ success: true }))
    };

    mockInventoryService = {
      getInsumos: jasmine.createSpy('getInsumos').and.returnValue(of([]))
    };

    mockBillingFacade = {
      saveCatalogServiceWithBOMAndFees: jasmine.createSpy('saveCatalogServiceWithBOMAndFees').and.returnValue(of({ success: true }))
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

  it('should create component', () => {
    expect(component).toBeTruthy();
  });
});
