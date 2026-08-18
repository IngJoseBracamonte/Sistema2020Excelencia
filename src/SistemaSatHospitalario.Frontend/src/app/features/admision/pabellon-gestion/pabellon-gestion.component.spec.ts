import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { PabellonGestionComponent } from './pabellon-gestion.component';
import { PabellonService, PacienteQuirurgicoItem, CirugiaCalendarioItem } from '../../../core/services/pabellon.service';
import { MedicoService } from '../../../core/services/medico.service';
import { MultiSedeService, AreaClinica } from '../../../core/services/multi-sede.service';
import { PatientService } from '../../../core/services/patient.service';

describe('PabellonGestionComponent', () => {
  let component: PabellonGestionComponent;
  let fixture: ComponentFixture<PabellonGestionComponent>;
  let mockPabellonService: jasmine.SpyObj<PabellonService>;
  let mockMedicoService: jasmine.SpyObj<MedicoService>;
  let mockMultiSedeService: jasmine.SpyObj<MultiSedeService>;
  let mockPatientService: jasmine.SpyObj<PatientService>;

  const mockPacientes: PacienteQuirurgicoItem[] = [
    {
      id: 'ord-1',
      cuentaServicioId: 'cta-1',
      pacienteId: 'pac-1',
      pacienteNombre: 'María Rodríguez',
      pacienteCedula: 'V-19876543',
      descripcionCirugia: 'Hernia Inguinal',
      salaQuirofano: 'Quirófano 1',
      modalidadAnestesia: 'General',
      esAlquilado: false,
      precioDerechoSalaUsd: 200,
      precioBaseUsd: 1500,
      medicoPrincipalId: 'med-1',
      medicoPrincipalNombre: 'Dr. A. López',
      fechaHoraProgramada: '2026-08-10T09:00:00.000Z',
      estado: 'Programada',
      fechaCreacion: '2026-08-01T08:00:00.000Z',
      usuarioCreacion: 'admin',
      requisitos: [],
      medicosHonorarios: [],
      insumosAsignados: [],
      solicitudesInsumos: [],
      logs: [],
      ubicacion: {
        areaClinicaNombre: 'Hospitalizacion',
        camaNombre: 'Cama 101',
        camaCodigo: 'C101',
        descripcionCompleta: 'Habitación 101'
      },
      ingresoCobertura: {
        tipo: 'Seguro',
        esAsegurado: true,
        convenioNombre: 'Mercantil'
      }
    }
  ];

  const mockCalendario: CirugiaCalendarioItem[] = [
    {
      id: 'ord-1',
      cuentaServicioId: 'cta-1',
      pacienteId: 'pac-1',
      descripcionCirugia: 'Hernia Inguinal',
      pacienteNombre: 'María Rodríguez',
      pacienteCedula: 'V-19876543',
      medicoId: 'med-1',
      medicoNombre: 'Dr. A. López',
      modalidadAnestesia: 'General',
      precioDerechoSalaUsd: 200,
      precioBaseUsd: 1500,
      fechaHoraProgramada: '2026-08-10T09:00:00.000Z',
      salaQuirofano: 'Quirófano 1',
      estado: 'Programada',
      esAlquilado: false,
      estaAptoParaQuirofano: true,
      requisitosCumplidos: 4,
      totalRequisitos: 4,
      ubicacion: {
        areaClinicaNombre: 'Hospitalizacion',
        camaNombre: 'Cama 101',
        camaCodigo: 'C101',
        descripcionCompleta: 'Habitación 101'
      },
      ingresoCobertura: {
        tipo: 'Seguro',
        esAsegurado: true,
        convenioNombre: 'Mercantil'
      }
    }
  ];

  const mockMedicos = [
    { id: 'med-1', nombre: 'Dr. A. López', especialidad: 'Cirugía General', activo: true },
    { id: 'med-2', nombre: 'Dr. R. Martínez', especialidad: 'Traumatología', activo: true }
  ];

  const mockAreasClinicas: AreaClinica[] = [
    { id: 'area-1', codigo: 'Q1', nombre: 'Quirófano 1', activo: true },
    { id: 'area-2', codigo: 'Q2', nombre: 'Quirófano 2', activo: true },
    { id: 'area-3', codigo: 'H101', nombre: 'Habitación 101', activo: true }
  ];

  beforeEach(async () => {
    mockPabellonService = jasmine.createSpyObj('PabellonService', [
      'getPacientesQuirurgicos',
      'getCalendario',
      'crearOrden',
      'cambiarEstado'
    ]);
    mockMedicoService = jasmine.createSpyObj('MedicoService', ['getAll']);
    mockMultiSedeService = jasmine.createSpyObj('MultiSedeService', ['getAreasClinicas']);
    mockPatientService = jasmine.createSpyObj('PatientService', ['searchPatients']);

    mockPabellonService.getPacientesQuirurgicos.and.returnValue(of(mockPacientes));
    mockPabellonService.getCalendario.and.returnValue(of(mockCalendario));
    mockMedicoService.getAll.and.returnValue(of(mockMedicos));
    mockMultiSedeService.getAreasClinicas.and.returnValue(of(mockAreasClinicas));
    mockPatientService.searchPatients.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, PabellonGestionComponent],
      providers: [
        { provide: PabellonService, useValue: mockPabellonService },
        { provide: MedicoService, useValue: mockMedicoService },
        { provide: MultiSedeService, useValue: mockMultiSedeService },
        { provide: PatientService, useValue: mockPatientService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PabellonGestionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse exitosamente', () => {
    expect(component).toBeTruthy();
  });

  it('debe cargar pacientes quirurgicos, calendario, medicos y quirofanos al inicializarse', () => {
    expect(mockPabellonService.getPacientesQuirurgicos).toHaveBeenCalled();
    expect(mockPabellonService.getCalendario).toHaveBeenCalled();
    expect(mockMedicoService.getAll).toHaveBeenCalled();
    expect(mockMultiSedeService.getAreasClinicas).toHaveBeenCalled();
    expect(component.pacientesQuirurgicos().length).toBe(1);
    expect(component.calendarioItems().length).toBe(1);
    expect(component.medicos().length).toBe(2);
    expect(component.quirofanos().length).toBe(2);
    expect(component.quirofanos().some(q => q.nombre === 'Habitación 101')).toBeFalse();
  });

  it('debe alternar los modos de vista entre tablero y calendario', () => {
    expect(component.vistaModo()).toBe('tablero');
    component.vistaModo.set('calendario');
    expect(component.vistaModo()).toBe('calendario');
  });

  it('debe abrir y cerrar el detalle del paciente seleccionado', () => {
    expect(component.pacienteSeleccionadoDetalle()).toBeNull();
    component.onSeleccionarPaciente(mockPacientes[0]);
    expect(component.pacienteSeleccionadoDetalle()?.id).toBe('ord-1');
  });
});
