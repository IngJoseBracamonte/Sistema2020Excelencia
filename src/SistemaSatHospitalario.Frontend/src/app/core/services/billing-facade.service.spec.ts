import { TestBed } from '@angular/core/testing';
import { BillingFacadeService } from './billing-facade.service';
import { FacturacionService } from './facturacion.service';
import { AuthService } from './auth.service';
import { SettingsService } from './settings.service';
import { SpecialtyService } from './specialty.service';
import { AppointmentsService } from './appointments.service';
import { CatalogService } from './catalog.service';
import { of } from 'rxjs';
import { CatalogItem } from '../models/priced-item.model';

describe('BillingFacadeService - getSearchKey and Filter Tests', () => {
  let service: BillingFacadeService;
  let facturacionServiceSpy: jasmine.SpyObj<FacturacionService>;
  let authServiceSpy: jasmine.SpyObj<AuthService>;
  let settingsServiceSpy: jasmine.SpyObj<SettingsService>;
  let specialtyServiceSpy: jasmine.SpyObj<SpecialtyService>;
  let appointmentsServiceSpy: jasmine.SpyObj<AppointmentsService>;
  let catalogServiceSpy: jasmine.SpyObj<CatalogService>;

  beforeEach(() => {
    facturacionServiceSpy = jasmine.createSpyObj('FacturacionService', ['someMethod']);
    authServiceSpy = jasmine.createSpyObj('AuthService', ['currentUser', 'isAdmin']);
    settingsServiceSpy = jasmine.createSpyObj('SettingsService', ['getTasa'], { tasa$: of(36.5) });
    specialtyServiceSpy = jasmine.createSpyObj('SpecialtyService', ['getAll']);
    appointmentsServiceSpy = jasmine.createSpyObj('AppointmentsService', ['getAll']);
    catalogServiceSpy = jasmine.createSpyObj('CatalogService', ['getPaymentMethods']);

    settingsServiceSpy.getTasa.and.returnValue(of({ monto: 36.5, fechaActualizacion: '' }));
    catalogServiceSpy.getPaymentMethods.and.returnValue(of([]));

    TestBed.configureTestingModule({
      providers: [
        BillingFacadeService,
        { provide: FacturacionService, useValue: facturacionServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: SettingsService, useValue: settingsServiceSpy },
        { provide: SpecialtyService, useValue: specialtyServiceSpy },
        { provide: AppointmentsService, useValue: appointmentsServiceSpy },
        { provide: CatalogService, useValue: catalogServiceSpy }
      ]
    });

    service = TestBed.inject(BillingFacadeService);
  });

  it('debe filtrar servicios correctamente aplicando la regla de truncar 5 letras para Ginecologia (GINECO)', () => {
    // Mock catalog data
    const catalog = [
      new CatalogItem({ id: '1', descripcion: 'CONSULTA GINECOLOGICA', tipo: 'CONSULTA', precioUsd: 50 }),
      new CatalogItem({ id: '2', descripcion: 'CONSULTA CARDIOLOGICA', tipo: 'CONSULTA', precioUsd: 60 }),
      new CatalogItem({ id: '3', descripcion: 'ECOGRAFIA GENERAL', tipo: 'ECO', precioUsd: 40 })
    ];
    service.servicesCatalog.set(catalog);

    // Act: set specialty to 'Ginecologia' (ends in logia, 11 - 5 = 6 chars -> 'GINECO')
    service.selectedEspecialidad.set('Ginecologia');

    const result = service.serviciosFiltrados();

    // Assert: should match 'GINECO'
    expect(result.length).toBe(1);
    expect(result[0].id).toBe('1');
  });

  it('debe filtrar servicios correctamente aplicando la regla de truncar 5 letras para Pediatria (PEDI)', () => {
    const catalog = [
      new CatalogItem({ id: '1', descripcion: 'CONSULTA PEDIATRICA', tipo: 'CONSULTA', precioUsd: 50 }),
      new CatalogItem({ id: '2', descripcion: 'CONSULTA GENERAL', tipo: 'CONSULTA', precioUsd: 30 })
    ];
    service.servicesCatalog.set(catalog);

    // Act: set specialty to 'Pediatria' (9 - 5 = 4 chars -> 'PEDI')
    service.selectedEspecialidad.set('Pediatria');

    const result = service.serviciosFiltrados();

    // Assert: should match 'PEDI'
    expect(result.length).toBe(1);
    expect(result[0].id).toBe('1');
  });

  it('debe mantener la especialidad intacta si tiene 5 o menos letras como RX (RX)', () => {
    const catalog = [
      new CatalogItem({ id: '1', descripcion: 'RADIOGRAFIA DE TORAX (RX)', tipo: 'RX', precioUsd: 20 }),
      new CatalogItem({ id: '2', descripcion: 'ECOGRAFIA GENERAL', tipo: 'ECO', precioUsd: 40 })
    ];
    service.servicesCatalog.set(catalog);

    // Act: set specialty to 'RX' (<= 5 chars -> stays 'RX')
    service.selectedEspecialidad.set('RX');

    const result = service.serviciosFiltrados();

    // Assert: should match 'RX'
    expect(result.length).toBe(1);
    expect(result[0].id).toBe('1');
  });
});
