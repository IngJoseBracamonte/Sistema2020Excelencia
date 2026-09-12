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

describe('EnfermeriaComponent - Médico Tratante en Ingreso', () => {
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
      getMedicos: jasmine.createSpy('getMedicos').and.returnValue(of([])),
      getAll: jasmine.createSpy('getAll').and.returnValue(of([]))
    };

    mockMultiSedeService = {
      activeSede: signal({ id: 'sede-1', nombre: 'Sede Principal' }),
      areas: signal([])
    };

    mockPatientService = {
      searchPatients: jasmine.createSpy('searchPatients').and.returnValue(of([]))
    };

    mockFacturacionService = {
      getCuentaPorId: jasmine.createSpy('getCuentaPorId').and.returnValue(of(null)),
      abrirCuenta: jasmine.createSpy('abrirCuenta').and.returnValue(of({ cuentaId: 'acc-test' }))
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

  it('debe inicializar medicoTratanteIngresoId como null', () => {
    expect(component.medicoTratanteIngresoId()).toBeNull();
  });

  it('debe resetear medicoTratanteIngresoId al abrir el modal de ingreso', () => {
    component.medicoTratanteIngresoId.set('some-medico-id');
    component.abrirModalIngreso();
    expect(component.medicoTratanteIngresoId()).toBeNull();
  });

  it('esMedicoTratanteRequerido debe ser false cuando el tipo es Emergencia', () => {
    component.nursingAreaFilter.set('Emergencia');
    expect(component.esMedicoTratanteRequerido()).toBeFalse();
  });

  it('esMedicoTratanteRequerido debe ser true cuando el tipo es Hospitalizacion', () => {
    component.nursingAreaFilter.set('Hospitalizacion');
    expect(component.esMedicoTratanteRequerido()).toBeTrue();
  });

  it('esMedicoTratanteRequerido debe ser true cuando el tipo es UCI', () => {
    component.nursingAreaFilter.set('UCI');
    expect(component.esMedicoTratanteRequerido()).toBeTrue();
  });

  it('no debe bloquear procesarIngreso en Emergencia sin médico tratante', () => {
    component.nursingAreaFilter.set('Emergencia');
    component.medicoTratanteIngresoId.set(null);
    component.selectedPatientForIngreso.set({
      id: 'pac-1',
      nombre: 'Juan',
      apellidos: 'Perez',
      cedula: 'V-12345678'
    } as any);
    component.showNewPatientForm.set(false);

    component.procesarIngreso();

    // En emergencia avanza al step 2 (triage) sin error de médico
    expect(component.ingresoStep()).toBe(2);
    expect(component.errorMessage()).toBeNull();
  });

  it('debe bloquear procesarIngreso en Hospitalizacion sin médico tratante', () => {
    component.nursingAreaFilter.set('Hospitalizacion');
    component.medicoTratanteIngresoId.set(null);
    component.selectedPatientForIngreso.set({
      id: 'pac-1',
      nombre: 'Juan',
      apellidos: 'Perez',
      cedula: 'V-12345678'
    } as any);
    component.showNewPatientForm.set(false);

    component.procesarIngreso();

    expect(component.errorMessage()).toBeTruthy();
    expect(component.errorMessage()!.toLowerCase()).toContain('médico tratante');
  });

  it('debe bloquear procesarIngreso en UCI sin médico tratante', () => {
    component.nursingAreaFilter.set('UCI');
    component.medicoTratanteIngresoId.set(null);
    component.selectedPatientForIngreso.set({
      id: 'pac-1',
      nombre: 'Juan',
      apellidos: 'Perez',
      cedula: 'V-12345678'
    } as any);
    component.showNewPatientForm.set(false);

    component.procesarIngreso();

    expect(component.errorMessage()).toBeTruthy();
    expect(component.errorMessage()!.toLowerCase()).toContain('médico tratante');
  });

  it('debe permitir procesarIngreso en Hospitalizacion con médico tratante seleccionado', () => {
    component.nursingAreaFilter.set('Hospitalizacion');
    component.medicoTratanteIngresoId.set('medico-guid-hosp');
    component.selectedPatientForIngreso.set({
      id: 'pac-1',
      nombre: 'Juan',
      apellidos: 'Perez',
      cedula: 'V-12345678'
    } as any);
    component.showNewPatientForm.set(false);

    component.procesarIngreso();

    // No debe haber error de médico (puede haber un loading state)
    const err = component.errorMessage();
    if (err) {
      expect(err.toLowerCase()).not.toContain('médico tratante');
    }
  });
});

describe('EnfermeriaComponent - Ordenamiento de Pacientes Activos por fecha más reciente primero', () => {
  let component: EnfermeriaComponent;
  let fixture: ComponentFixture<EnfermeriaComponent>;

  beforeEach(async () => {
    const mockAuthService = {
      currentUser: signal({ id: 'user-1', username: 'enfermero1', role: 'Enfermero' })
    };

    const mockMedicoService = {
      getMedicos: jasmine.createSpy('getMedicos').and.returnValue(of([])),
      getAll: jasmine.createSpy('getAll').and.returnValue(of([]))
    };

    const mockMultiSedeService = {
      activeSede: signal({ id: 'sede-1', nombre: 'Sede Principal' }),
      areas: signal([]),
      getAreasClinicas: jasmine.createSpy('getAreasClinicas').and.returnValue(of([]))
    };

    const mockPatientService = {
      searchPatients: jasmine.createSpy('searchPatients').and.returnValue(of([]))
    };

    const mockFacturacionService = {
      getCuentaPorId: jasmine.createSpy('getCuentaPorId').and.returnValue(of(null)),
      abrirCuenta: jasmine.createSpy('abrirCuenta').and.returnValue(of({ cuentaId: 'acc-test' }))
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

  it('debe ordenar filteredAccounts de más reciente a más antiguo por fechaCarga', () => {
    component.nursingAreaFilter.set('Emergencia');
    component.dateFilter.set('todos');
    component.searchTerm.set('');

    const cuentaAntigua: any = {
      cuentaId: 'acc-antigua',
      pacienteId: 'pac-1',
      pacienteNombre: 'Paciente Antiguo',
      pacienteCedula: 'V-11111111',
      tipoIngreso: 'Emergencia',
      fechaCarga: '2026-09-12T08:00:00.000Z'
    };

    const cuentaReciente: any = {
      cuentaId: 'acc-reciente',
      pacienteId: 'pac-2',
      pacienteNombre: 'Paciente Reciente',
      pacienteCedula: 'V-22222222',
      tipoIngreso: 'Emergencia',
      fechaCarga: '2026-09-12T13:39:00.000Z'
    };

    component.activeAccounts.set([cuentaAntigua, cuentaReciente]);

    const resultado = component.filteredAccounts();
    expect(resultado.length).toBe(2);
    expect(resultado[0].cuentaId).toBe('acc-reciente');
    expect(resultado[1].cuentaId).toBe('acc-antigua');
  });

  it('debe considerar fechaIngreso o fechaApertura cuando fechaCarga no esté definida', () => {
    component.nursingAreaFilter.set('Emergencia');
    component.dateFilter.set('todos');

    const cuenta1: any = {
      cuentaId: 'acc-1',
      pacienteId: 'p-1',
      pacienteNombre: 'Paciente 1',
      pacienteCedula: 'V-101',
      tipoIngreso: 'Emergencia',
      fechaIngreso: '2026-09-10T10:00:00.000Z'
    };

    const cuenta2: any = {
      cuentaId: 'acc-2',
      pacienteId: 'p-2',
      pacienteNombre: 'Paciente 2',
      pacienteCedula: 'V-102',
      tipoIngreso: 'Emergencia',
      fechaApertura: '2026-09-12T15:00:00.000Z'
    };

    component.activeAccounts.set([cuenta1, cuenta2]);

    const resultado = component.filteredAccounts();
    expect(resultado[0].cuentaId).toBe('acc-2');
    expect(resultado[1].cuentaId).toBe('acc-1');
  });

  it('debe preservar el orden descendente de fechas al aplicar búsqueda por texto', () => {
    component.nursingAreaFilter.set('Emergencia');
    component.dateFilter.set('todos');
    component.searchTerm.set('JOSE');

    const cuentaJoseAntiguo: any = {
      cuentaId: 'acc-jose-1',
      pacienteId: 'p-1',
      pacienteNombre: 'Jose Perez',
      pacienteCedula: 'V-101',
      tipoIngreso: 'Emergencia',
      fechaCarga: '2026-09-11T10:00:00.000Z'
    };

    const cuentaJoseNuevo: any = {
      cuentaId: 'acc-jose-2',
      pacienteId: 'p-2',
      pacienteNombre: 'Jose Bracamonte',
      pacienteCedula: 'V-102',
      tipoIngreso: 'Emergencia',
      fechaCarga: '2026-09-12T13:39:00.000Z'
    };

    const cuentaOtro: any = {
      cuentaId: 'acc-otro',
      pacienteId: 'p-3',
      pacienteNombre: 'Carlos Gomez',
      pacienteCedula: 'V-103',
      tipoIngreso: 'Emergencia',
      fechaCarga: '2026-09-12T14:00:00.000Z'
    };

    component.activeAccounts.set([cuentaJoseAntiguo, cuentaOtro, cuentaJoseNuevo]);

    const resultado = component.filteredAccounts();
    expect(resultado.length).toBe(2);
    expect(resultado[0].cuentaId).toBe('acc-jose-2');
    expect(resultado[1].cuentaId).toBe('acc-jose-1');
  });
});

