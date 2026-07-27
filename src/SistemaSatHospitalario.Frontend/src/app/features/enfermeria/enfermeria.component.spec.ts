import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EnfermeriaComponent } from './enfermeria.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthService } from '../../core/services/auth.service';
import { MedicoService } from '../../core/services/medico.service';
import { MultiSedeService } from '../../core/services/multi-sede.service';
import { PatientService } from '../../core/services/patient.service';
import { FacturacionService } from '../../core/services/facturacion.service';
import { signal } from '@angular/core';
import { of } from 'rxjs';

describe('EnfermeriaComponent - Dropdown DAR DE ALTA Tests', () => {
  let component: EnfermeriaComponent;
  let fixture: ComponentFixture<EnfermeriaComponent>;

  let mockAuthService: any;
  let mockMedicoService: any;
  let mockMultiSedeService: any;
  let mockPatientService: any;
  let mockFacturacionService: any;

  beforeEach(async () => {
    mockAuthService = {
      currentUser: signal({ id: 'user-1', username: 'enfermero1', role: 'Enfermero' })
    };

    mockMedicoService = {
      getMedicos: jasmine.createSpy('getMedicos').and.returnValue(of([]))
    };

    mockMultiSedeService = {
      activeSede: signal({ id: 'sede-1', nombre: 'Sede Principal' }),
      areas: signal([])
    };

    mockPatientService = {
      searchPatients: jasmine.createSpy('searchPatients').and.returnValue(of([]))
    };

    mockFacturacionService = {
      getCuentaPorId: jasmine.createSpy('getCuentaPorId').and.returnValue(of(null))
    };

    await TestBed.configureTestingModule({
      imports: [EnfermeriaComponent, HttpClientTestingModule],
      providers: [
        { provide: AuthService, useValue: mockAuthService },
        { provide: MedicoService, useValue: mockMedicoService },
        { provide: MultiSedeService, useValue: mockMultiSedeService },
        { provide: PatientService, useValue: mockPatientService },
        { provide: FacturacionService, useValue: mockFacturacionService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EnfermeriaComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe iniciar con el dropdown de DAR DE ALTA cerrado', () => {
    expect(component.isAltaDropdownOpen()).toBeFalse();
  });

  it('debe alternar el estado del dropdown con toggleAltaDropdown', () => {
    const mockEvent = new MouseEvent('click');
    spyOn(mockEvent, 'stopPropagation');

    component.toggleAltaDropdown(mockEvent);
    expect(component.isAltaDropdownOpen()).toBeTrue();
    expect(mockEvent.stopPropagation).toHaveBeenCalled();

    component.toggleAltaDropdown();
    expect(component.isAltaDropdownOpen()).toBeFalse();
  });

  it('debe cerrar el dropdown con closeAltaDropdown', () => {
    component.isAltaDropdownOpen.set(true);
    expect(component.isAltaDropdownOpen()).toBeTrue();

    component.closeAltaDropdown();
    expect(component.isAltaDropdownOpen()).toBeFalse();
  });

  it('debe cerrar el dropdown de DAR DE ALTA al iniciar un proceso de alta medica', () => {
    component.isAltaDropdownOpen.set(true);
    component.selectedAccount.set({
      cuentaId: 'acc-1',
      pacienteId: 'pac-1',
      pacienteNombre: 'Juan Perez',
      pacienteCedula: 'V-12345678',
      tipoIngreso: 'Hospitalizacion',
      convenioId: null,
      total: 0
    });

    component.iniciarAltaMedica(0); // Alta Normal
    expect(component.isAltaDropdownOpen()).toBeFalse();
  });

  it('debe calcular isAccountSolvent como true cuando la cuenta no tiene saldo pendiente', () => {
    component.selectedAccount.set({
      cuentaId: 'acc-1',
      pacienteId: 'pac-1',
      pacienteNombre: 'Juan Perez',
      pacienteCedula: 'V-12345678',
      tipoIngreso: 'Hospitalizacion',
      convenioId: null,
      total: 100,
      totalPagado: 100
    });
    expect(component.isAccountSolvent()).toBeTrue();
  });

  it('debe calcular isAccountSolvent como false cuando la cuenta tiene saldo pendiente', () => {
    component.selectedAccount.set({
      cuentaId: 'acc-2',
      pacienteId: 'pac-2',
      pacienteNombre: 'Maria Lopez',
      pacienteCedula: 'V-87654321',
      tipoIngreso: 'Hospitalizacion',
      convenioId: null,
      total: 250,
      totalPagado: 100
    });
    expect(component.isAccountSolvent()).toBeFalse();
  });
});
