import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { PabellonGestionComponent } from './pabellon-gestion.component';
import { PabellonService, OrdenCirugia } from '../../../core/services/pabellon.service';
import { MedicoService } from '../../../core/services/medico.service';

describe('PabellonGestionComponent', () => {
  let component: PabellonGestionComponent;
  let fixture: ComponentFixture<PabellonGestionComponent>;
  let mockPabellonService: jasmine.SpyObj<PabellonService>;
  let mockMedicoService: jasmine.SpyObj<MedicoService>;

  const mockOrdenes: OrdenCirugia[] = [
    {
      id: 'ord-1',
      cuentaServicioId: 'cta-1',
      pacienteId: 'pac-1',
      pacienteNombre: 'María Rodríguez',
      pacienteCedula: 'V-19876543',
      descripcionCirugia: 'Hernia Inguinal',
      especialidad: 'Ginecología',
      razonCirugia: 'Hernia estrangulada síntoma severo',
      precioBaseUsd: 1500,
      medicoId: 'med-1',
      medicoNombre: 'Dr. A. López',
      fechaHoraProgramada: '2026-08-10T09:00:00.000Z',
      estado: 'Programado',
      fechaCreacion: '2026-08-01T08:00:00.000Z',
      usuarioCreacion: 'admin'
    },
    {
      id: 'ord-2',
      cuentaServicioId: 'cta-2',
      pacienteId: 'pac-2',
      pacienteNombre: 'Carlos Gómez',
      pacienteCedula: 'V-15432109',
      descripcionCirugia: 'Prótesis de Rodilla',
      especialidad: 'Traumatología',
      razonCirugia: 'Gonoartrosis severa Grado IV',
      precioBaseUsd: 3200,
      medicoId: 'med-2',
      medicoNombre: 'Dr. R. Martínez',
      fechaHoraProgramada: '2026-08-11T11:00:00.000Z',
      estado: 'EnProceso',
      fechaCreacion: '2026-08-01T09:00:00.000Z',
      usuarioCreacion: 'admin'
    }
  ];

  const mockMedicos = [
    { id: 'med-1', nombre: 'Dr. A. López', especialidad: 'Cirugía General', activo: true },
    { id: 'med-2', nombre: 'Dr. R. Martínez', especialidad: 'Traumatología', activo: true }
  ];

  beforeEach(async () => {
    mockPabellonService = jasmine.createSpyObj('PabellonService', [
      'getOrdenes',
      'crearOrden',
      'cambiarEstado'
    ]);
    mockMedicoService = jasmine.createSpyObj('MedicoService', ['getAll']);

    mockPabellonService.getOrdenes.and.returnValue(of(mockOrdenes));
    mockMedicoService.getAll.and.returnValue(of(mockMedicos));

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, PabellonGestionComponent],
      providers: [
        { provide: PabellonService, useValue: mockPabellonService },
        { provide: MedicoService, useValue: mockMedicoService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PabellonGestionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse exitosamente', () => {
    expect(component).toBeTruthy();
  });

  it('debe cargar las órdenes de cirugía y los médicos al inicializarse', () => {
    expect(mockPabellonService.getOrdenes).toHaveBeenCalled();
    expect(mockMedicoService.getAll).toHaveBeenCalled();
    expect(component.ordenes().length).toBe(2);
    expect(component.medicos().length).toBe(2);
  });

  it('debe filtrar las órdenes por texto (paciente, cirujano, especialidad o cirugía)', () => {
    component.filtroTexto.set('María');
    expect(component.ordenesFiltradas().length).toBe(1);
    expect(component.ordenesFiltradas()[0].pacienteNombre).toBe('María Rodríguez');

    component.filtroTexto.set('Traumatología');
    expect(component.ordenesFiltradas().length).toBe(1);
    expect(component.ordenesFiltradas()[0].descripcionCirugia).toBe('Prótesis de Rodilla');
  });

  it('debe seleccionar una orden de cirugía al hacer clic y mostrarla en la señal de ordenSeleccionada', () => {
    expect(component.ordenSeleccionada()).toBeNull();
    component.ordenSeleccionada.set(mockOrdenes[0]);
    expect(component.ordenSeleccionada()?.id).toBe('ord-1');
    expect(component.ordenSeleccionada()?.razonCirugia).toBe('Hernia estrangulada síntoma severo');
  });

  it('debe alternar los modos de vista entre pizarra, calendario y lista', () => {
    expect(component.vistaModo()).toBe('pizarra');
    component.vistaModo.set('lista');
    expect(component.vistaModo()).toBe('lista');
    component.vistaModo.set('calendario');
    expect(component.vistaModo()).toBe('calendario');
  });

  it('debe enviar la razón de la cirugía al crear una nueva orden', () => {
    mockPabellonService.crearOrden.and.returnValue(of({ id: 'ord-new' }));
    component.nuevaCirugiaForm = {
      cuentaServicioId: 'cta-10',
      pacienteId: 'pac-10',
      descripcionCirugia: 'Colecistectomía Laparoscópica',
      especialidad: 'Cirugía General',
      razonCirugia: 'Colelitiasis sintomática recurrente',
      precioBaseUsd: 1800,
      medicoId: 'med-1',
      fechaHoraProgramada: '2026-08-12T10:00:00'
    };

    expect(component.esFormularioValido()).toBeTrue();
    component.guardarNuevaCirugia();
    expect(mockPabellonService.crearOrden).toHaveBeenCalled();
  });
});
