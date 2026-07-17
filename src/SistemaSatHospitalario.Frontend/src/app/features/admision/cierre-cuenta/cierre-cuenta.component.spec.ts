import { ComponentFixture, TestBed } from '@angular/core/testing';
import { CierreCuentaComponent } from './cierre-cuenta.component';
import { ActivatedRoute, Router } from '@angular/router';
import { AdminBillingService } from '../../../core/services/admin-billing.service';
import { FacturacionService } from '../../../core/services/facturacion.service';
import { PatientService } from '../../../core/services/patient.service';
import { AuthService } from '../../../core/services/auth.service';
import { PrintService } from '../../../core/services/print.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { SettingsService } from '../../../core/services/settings.service';
import { of, throwError } from 'rxjs';
import { signal } from '@angular/core';
import { HttpClientTestingModule } from '@angular/common/http/testing';

describe('CierreCuentaComponent', () => {
  let component: CierreCuentaComponent;
  let fixture: ComponentFixture<CierreCuentaComponent>;

  let mockActivatedRoute: any;
  let mockRouter: any;
  let mockAdminBillingService: any;
  let mockFacturacionService: any;
  let mockPatientService: any;
  let mockAuthService: any;
  let mockPrintService: any;
  let mockConveniosService: any;
  let mockSettingsService: any;

  const mockAccounts: any[] = [
    {
      cuentaId: 'account-1',
      pacienteId: 'patient-1',
      pacienteNombre: 'Marcus Elias',
      pacienteCedula: '884-29A-11',
      fechaCarga: '2026-06-08T08:00:00',
      estado: 'Abierta',
      tipoIngreso: 'Hospitalizacion',
      total: 1000,
      detalles: [
        {
          id: 'detail-1',
          servicioId: 'service-1',
          descripcion: 'Habitacion Estandar',
          precio: 500,
          honorario: 0,
          cantidad: 2,
          tipoServicio: 'HOSP',
          fechaCarga: '2026-06-08T08:00:00'
        }
      ]
    }
  ];

  beforeEach(async () => {
    mockActivatedRoute = {
      params: of({ type: 'Hospitalizacion' }),
      queryParams: of({})
    };

    mockRouter = jasmine.createSpyObj('Router', ['navigate']);

    mockAdminBillingService = jasmine.createSpyObj('AdminBillingService', ['getCuentasAdministrativas']);
    mockAdminBillingService.getCuentasAdministrativas.and.returnValue(of(mockAccounts));

    mockFacturacionService = jasmine.createSpyObj('FacturacionService', ['closeAccount']);
    mockFacturacionService.closeAccount.and.returnValue(of({ reciboId: 'recibo-1', id: 'recibo-1' }));

    mockPatientService = jasmine.createSpyObj('PatientService', ['searchPatients']);
    mockPatientService.searchPatients.and.returnValue(of([{
      id: 'patient-1',
      cedula: '884-29A-11',
      nombre: 'Marcus',
      apellidos: 'Elias',
      celular: '555-0192',
      fechaNacimiento: '1981-11-04'
    }]));

    mockAuthService = {
      currentUser: signal({ id: 'user-1', username: 'admin', role: 'Administrador' }),
      isAdministrador: () => true,
      isCajero: () => true,
      isAdmin: () => true,
      isParticularAssistant: () => true,
      isInsuranceAssistant: () => true,
      isHospitalAssistant: () => true,
      isEmergencyAssistant: () => true
    };

    mockPrintService = jasmine.createSpyObj('PrintService', ['generateReceiptHtml']);

    mockConveniosService = jasmine.createSpyObj('ConveniosService', ['getAll']);
    mockConveniosService.getAll.and.returnValue(of([]));

    mockSettingsService = jasmine.createSpyObj('SettingsService', ['getTasa', 'updateTasa']);
    mockSettingsService.getTasa.and.returnValue(of({ monto: 36.5 }));

    await TestBed.configureTestingModule({
      imports: [CierreCuentaComponent, HttpClientTestingModule],
      providers: [
        { provide: ActivatedRoute, useValue: mockActivatedRoute },
        { provide: Router, useValue: mockRouter },
        { provide: AdminBillingService, useValue: mockAdminBillingService },
        { provide: FacturacionService, useValue: mockFacturacionService },
        { provide: PatientService, useValue: mockPatientService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: PrintService, useValue: mockPrintService },
        { provide: ConveniosService, useValue: mockConveniosService },
        { provide: SettingsService, useValue: mockSettingsService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(CierreCuentaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente el componente', () => {
    expect(component).toBeTruthy();
  });

  it('debe cargar la lista de cuentas abiertas al inicializar', () => {
    expect(mockAdminBillingService.getCuentasAdministrativas).toHaveBeenCalledWith(undefined, 'Hospitalizacion', 'Abierta');
    expect(component.accounts().length).toBe(1);
    expect(component.accounts()[0].pacienteNombre).toBe('Marcus Elias');
  });

  it('debe filtrar pacientes por nombre o cédula en la antesala', () => {
    component.searchTerm.set('Marcus');
    expect(component.filteredAccounts().length).toBe(1);

    component.searchTerm.set('999-99');
    expect(component.filteredAccounts().length).toBe(0);

    component.searchTerm.set('884-29A-11');
    expect(component.filteredAccounts().length).toBe(1);
  });

  it('debe calcular correctamente el desglose financiero del cierre', () => {
    // Seleccionar cuenta para activar el panel
    component.selectAccount(mockAccounts[0]);

    // Subtotal = 1000
    expect(component.subtotalServicios()).toBe(1000);
    // Total General = 1000
    expect(component.totalGeneral()).toBe(1000);
    // Paciente Particular (sin seguro) -> Cobertura = 0, neto a pagar = 1000
    expect(component.totalAPagarPaciente()).toBe(1000);
  });

  it('debe calcular correctamente los dias de estancia', () => {
    component.selectAccount(mockAccounts[0]);
    // Fecha de ingreso: 2026-06-08T08:00:00Z
    // Forzamos fecha de egreso al 2026-06-11
    component.fechaEgreso.set('2026-06-11');
    component.horaEgreso.set('08:00');

    // Días de estancia = 3 días
    expect(component.diasEstancia()).toBe(3);
  });

  it('debe llamar a closeAccount con los parametros de cierre y pagos', () => {
    component.selectAccount(mockAccounts[0]);
    component.nuevoPagoMetodo.set('Particular - Tarjeta de Crédito/Débito');
    component.nuevoPagoReferencia.set('REF-111');
    component.nuevoPagoMontoMoneda.set(1000);
    component.agregarPago();

    component.procesarCierre();

    expect(mockFacturacionService.closeAccount).toHaveBeenCalledWith(jasmine.objectContaining({
      cuentaId: 'account-1',
      usuarioCajero: 'admin',
      tasaCambio: 36.5
    }));
  });

  it('debe mostrar el panel de acciones de caja cuando el tipo es Hospitalizacion', () => {
    component.type.set('Hospitalizacion');
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const actionsBox = compiled.querySelector('a[routerLink="/cxc"]');
    expect(actionsBox).toBeTruthy();
  });

  it('debe ocultar el panel de acciones de caja cuando el tipo es Emergencia', () => {
    component.type.set('Emergencia');
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    const actionsBox = compiled.querySelector('a[routerLink="/cxc"]');
    expect(actionsBox).toBeFalsy();
  });
});
