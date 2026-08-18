import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { HospitalizacionComponent } from './hospitalizacion.component';
import { MultiSedeService } from '../../../core/services/multi-sede.service';
import { AuthService } from '../../../core/services/auth.service';
import { SignalrService } from '../../../core/services/signalr.service';

describe('HospitalizacionComponent', () => {
  let component: HospitalizacionComponent;
  let fixture: ComponentFixture<HospitalizacionComponent>;
  let mockMultiSedeService: jasmine.SpyObj<MultiSedeService>;
  let mockAuthService: jasmine.SpyObj<AuthService>;
  let mockSignalrService: jasmine.SpyObj<SignalrService>;

  const mockSedes = [
    { id: '10000000-0000-0000-0000-000000000003', codigo: 'HOSPITALIZACION', nombre: 'Área de Hospitalización', esPrincipal: false, activo: true },
    { id: '10000000-0000-0000-0000-000000000005', codigo: 'CIRUGIA', nombre: 'Área de Cirugía y Quirófano', esPrincipal: false, activo: true }
  ];

  const mockCamas = [
    { id: 'cama-1', codigo: 'HAB-101', nombre: 'Habitación 101', sedeNombre: 'Área de Hospitalización', estado: 'Disponible', versionEstado: 1 }
  ];

  const mockAreasClinicas = [
    { id: 'q-1', codigo: 'QX-1', nombre: 'Quirófano 1', sedeId: '10000000-0000-0000-0000-000000000005', sedeNombre: 'Área de Cirugía y Quirófano', activo: true },
    { id: 'c-1', codigo: 'HAB-101', nombre: 'Habitación 101', sedeId: '10000000-0000-0000-0000-000000000003', sedeNombre: 'Área de Hospitalización', activo: true }
  ];

  beforeEach(async () => {
    mockMultiSedeService = jasmine.createSpyObj('MultiSedeService', [
      'getCamasMonitoreo',
      'getSedes',
      'getAreasClinicas',
      'createAreaClinica',
      'crearCama',
      'deleteAreaClinica'
    ]);
    mockAuthService = jasmine.createSpyObj('AuthService', ['getToken', 'currentUser', 'isAdmin']);
    mockSignalrService = jasmine.createSpyObj('SignalrService', ['startConnection', 'stopConnection', 'incomingCamaUpdates']);

    mockAuthService.getToken.and.returnValue('fake-token');
    mockAuthService.currentUser.and.returnValue({ id: 'u1', username: 'admin', role: 'Administrador' } as any);
    mockAuthService.isAdmin.and.returnValue(true);

    mockSignalrService.incomingCamaUpdates.and.returnValue(null);

    mockMultiSedeService.getCamasMonitoreo.and.returnValue(of(mockCamas));
    mockMultiSedeService.getSedes.and.returnValue(of(mockSedes));
    mockMultiSedeService.getAreasClinicas.and.returnValue(of(mockAreasClinicas));
    mockMultiSedeService.createAreaClinica.and.returnValue(of('new-id'));
    mockMultiSedeService.deleteAreaClinica.and.returnValue(of(null));

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, HospitalizacionComponent],
      providers: [
        { provide: MultiSedeService, useValue: mockMultiSedeService },
        { provide: AuthService, useValue: mockAuthService },
        { provide: SignalrService, useValue: mockSignalrService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(HospitalizacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente e inicializar en la pestaña Camas', () => {
    expect(component).toBeTruthy();
    expect(component.activeTab()).toBe('camas');
    expect(component.camas().length).toBe(1);
  });

  it('debe cargar y filtrar los quirofanos correctamente en la pestaña Quirofanos', () => {
    expect(component.quirofanos().length).toBe(1);
    expect(component.quirofanos()[0].nombre).toBe('Quirófano 1');
    // Habitación 101 no debe estar en quirofanos
    expect(component.quirofanos().some(q => q.nombre === 'Habitación 101')).toBeFalse();
  });

  it('debe permitir cambiar a la pestaña Quirofanos y abrir el modal de anexar quirofano', () => {
    component.activeTab.set('quirofanos');
    expect(component.activeTab()).toBe('quirofanos');

    component.abrirModalCrearQuirofano();
    expect(component.showCreateQuirofanoModal()).toBeTrue();
  });

  it('debe anexar un nuevo quirofano llamando al servicio multiSedeService', () => {
    component.nuevoQuirofano.codigo = 'QX-02';
    component.nuevoQuirofano.nombre = 'Quirófano 2';

    component.anexarQuirofano();

    expect(mockMultiSedeService.createAreaClinica).toHaveBeenCalledWith({
      sedeId: '10000000-0000-0000-0000-000000000005',
      codigo: 'QX-02',
      nombre: 'Quirófano 2'
    });
    expect(component.showCreateQuirofanoModal()).toBeFalse();
  });
});
