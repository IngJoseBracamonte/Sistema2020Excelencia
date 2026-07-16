import { ComponentFixture, TestBed } from '@angular/core/testing';
import { AccountModificationComponent } from './account-modification.component';
import { AdminBillingService, CuentaAdministrativaDto } from '../../../core/services/admin-billing.service';
import { PatientService } from '../../../core/services/patient.service';
import { ConveniosService } from '../../../core/services/convenios.service';
import { of, throwError } from 'rxjs';
import { signal } from '@angular/core';

describe('AccountModificationComponent', () => {
  let component: AccountModificationComponent;
  let fixture: ComponentFixture<AccountModificationComponent>;
  
  let adminBillingServiceMock: any;
  let patientServiceMock: any;
  let conveniosServiceMock: any;

  const mockAccount: CuentaAdministrativaDto = {
    cuentaId: 'account-123',
    pacienteId: 'patient-123',
    pacienteNombre: 'JUAN PEREZ',
    pacienteCedula: '12345',
    fechaCarga: new Date().toISOString(),
    estado: 'Abierta',
    tipoIngreso: 'Particular',
    total: 100,
    detalles: [
      {
        id: 'detail-1',
        servicioId: 'serv-1',
        descripcion: 'CONSULTA MEDICA',
        precio: 100,
        honorario: 20,
        cantidad: 1,
        tipoServicio: 'Medico',
        fechaCarga: new Date().toISOString(),
        incluidoEnTarifaBase: false
      }
    ]
  };

  beforeEach(async () => {
    adminBillingServiceMock = {
      getCuentasAdministrativas: jasmine.createSpy('getCuentasAdministrativas').and.returnValue(of([mockAccount])),
      updateCuentaAdministrativa: jasmine.createSpy('updateCuentaAdministrativa').and.returnValue(of({ success: true })),
      getHistorialModificaciones: jasmine.createSpy('getHistorialModificaciones').and.returnValue(of([]))
    };

    patientServiceMock = {
      searchPatients: jasmine.createSpy('searchPatients').and.returnValue(of([]))
    };

    conveniosServiceMock = {
      getAll: jasmine.createSpy('getAll').and.returnValue(of([]))
    };

    await TestBed.configureTestingModule({
      imports: [AccountModificationComponent],
      providers: [
        { provide: AdminBillingService, useValue: adminBillingServiceMock },
        { provide: PatientService, useValue: patientServiceMock },
        { provide: ConveniosService, useValue: conveniosServiceMock }
      ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountModificationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should search and load accounts into signal', () => {
    component.searchTerm.set('JUAN');
    component.searchAccounts();

    expect(adminBillingServiceMock.getCuentasAdministrativas).toHaveBeenCalledWith('JUAN');
    expect(component.accounts()).toEqual([mockAccount]);
    expect(component.isLoading()).toBeFalse();
  });

  it('should initialize modification state on account selection', () => {
    component.selectAccount(mockAccount);

    expect(component.selectedAccount()).toEqual(mockAccount);
    expect(component.modTipoIngreso()).toBe('Particular');
    expect(component.editedPrices()['detail-1']).toEqual({ precio: 100, honorario: 20 });
    expect(adminBillingServiceMock.getHistorialModificaciones).toHaveBeenCalledWith('account-123');
  });

  it('should fail validation when saving modifications with no changes', () => {
    component.selectAccount(mockAccount);
    component.saveModifications();

    expect(component.errorMessage()).toBe('No se han realizado modificaciones.');
    expect(adminBillingServiceMock.updateCuentaAdministrativa).not.toHaveBeenCalled();
  });

  it('should call service and save when prices are modified', () => {
    component.selectAccount(mockAccount);
    
    // modify price
    component.updatePriceEdit('detail-1', 'precio', 120);
    component.saveModifications();

    expect(adminBillingServiceMock.updateCuentaAdministrativa).toHaveBeenCalledWith({
      cuentaId: 'account-123',
      nuevoPacienteId: undefined,
      nuevoTipoIngreso: undefined,
      nuevoConvenioId: null,
      correccionesPrecios: [
        { detalleId: 'detail-1', nuevoPrecio: 120, nuevoHonorario: 20 }
      ]
    });
    expect(component.actionMessage()).toBe('¡Cuenta modificada administrativamente con éxito!');
  });
});
