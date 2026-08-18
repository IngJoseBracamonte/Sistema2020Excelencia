import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EnviosRecepcionesComponent } from './envios-recepciones.component';
import { MultiSedeService, PedidoInterSede, EstadoPedidoInterSede } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { of } from 'rxjs';

describe('EnviosRecepcionesComponent', () => {
  let component: EnviosRecepcionesComponent;
  let fixture: ComponentFixture<EnviosRecepcionesComponent>;
  let mockMultiSedeService: jasmine.SpyObj<MultiSedeService>;
  let mockInventoryService: jasmine.SpyObj<InventoryService>;

  const mockPedidos: PedidoInterSede[] = [
    {
      id: 'ped-1',
      correlativo: 'PED-2026-0001',
      sedeSolicitanteId: 'sede-2',
      sedeSolicitanteNombre: 'Emergencia',
      sedeProveedoraId: '10000000-0000-0000-0000-000000000001',
      sedeProveedoraNombre: 'Almacén Principal',
      estado: EstadoPedidoInterSede.Solicitado, // Estado real retornado por la API .NET (1: Solicitado)
      fechaCreacion: '2026-08-10T10:00:00.000Z',
      usuarioCreador: 'enfermero',
      observaciones: 'Urgente reposición',
      detalles: [
        {
          id: 'det-1',
          insumoId: 'ins-1',
          insumoNombre: 'Jeringa 5ml',
          insumoCodigo: 'INS-001',
          cantidadSolicitada: 10,
          cantidadDespachada: 0,
          cantidadRecibida: 0,
          stockDisponibleSedeProveedora: 50
        }
      ]
    },
    {
      id: 'ped-2',
      correlativo: 'PED-2026-0002',
      sedeSolicitanteId: 'sede-3',
      sedeSolicitanteNombre: 'Hospitalización',
      sedeProveedoraId: '10000000-0000-0000-0000-000000000001',
      sedeProveedoraNombre: 'Almacén Principal',
      estado: EstadoPedidoInterSede.Pendiente, // Compatibilidad con alias
      fechaCreacion: '2026-08-10T11:00:00.000Z',
      usuarioCreador: 'asistente',
      observaciones: 'Stock de turno',
      detalles: []
    },
    {
      id: 'ped-3',
      correlativo: 'PED-2026-0003',
      sedeSolicitanteId: 'sede-2',
      sedeSolicitanteNombre: 'Emergencia',
      sedeProveedoraId: '10000000-0000-0000-0000-000000000001',
      sedeProveedoraNombre: 'Almacén Principal',
      estado: EstadoPedidoInterSede.Despachado, // Ya procesado, no debe mostrarse
      fechaCreacion: '2026-08-09T08:00:00.000Z',
      usuarioCreador: 'enfermero',
      observaciones: '',
      detalles: []
    }
  ];

  beforeEach(async () => {
    mockMultiSedeService = jasmine.createSpyObj('MultiSedeService', [
      'getSedes',
      'getAreasClinicas',
      'getPedidosRecibidos',
      'aprobarPedido',
      'rechazarPedido'
    ]);
    mockInventoryService = jasmine.createSpyObj('InventoryService', [
      'getInsumos',
      'getHistorialMovimientos',
      'enviarASubArea'
    ]);

    mockMultiSedeService.getSedes.and.returnValue(of([]));
    mockMultiSedeService.getAreasClinicas.and.returnValue(of([]));
    mockMultiSedeService.getPedidosRecibidos.and.returnValue(of(mockPedidos));
    mockInventoryService.getInsumos.and.returnValue(of([]));
    mockInventoryService.getHistorialMovimientos.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [EnviosRecepcionesComponent],
      providers: [
        { provide: MultiSedeService, useValue: mockMultiSedeService },
        { provide: InventoryService, useValue: mockInventoryService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(EnviosRecepcionesComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debe incluir los pedidos en estado "Solicitado" y "Pendiente" en las requisiciones pendientes', () => {
    const pendientes = component.filteredPedidosPendientes();
    expect(pendientes.length).toBe(2);
    expect(pendientes.some(p => p.id === 'ped-1')).toBeTrue();
    expect(pendientes.some(p => p.id === 'ped-2')).toBeTrue();
    expect(pendientes.some(p => p.id === 'ped-3')).toBeFalse();
  });

  it('debe incluir únicamente la sede Hospitalización y sub-áreas en subAreasDisponibles excluyendo otras sedes', () => {
    const mockSedes = [
      { id: 'sede-1', codigo: 'PRINCIPAL', nombre: 'Almacén Principal', esPrincipal: true, activo: true },
      { id: '10000000-0000-0000-0000-000000000002', codigo: 'EMERGENCIA', nombre: 'Área de Emergencia', esPrincipal: false, activo: true },
      { id: '10000000-0000-0000-0000-000000000003', codigo: 'HOSPITALIZACION', nombre: 'Área de Hospitalización', esPrincipal: false, activo: true },
      { id: '10000000-0000-0000-0000-000000000005', codigo: 'CIRUGIA', nombre: 'Área de Cirugía', esPrincipal: false, activo: true }
    ];
    const mockAreas = [
      { id: 'area-farm', codigo: 'FARMACIA', nombre: 'Farmacia', activo: true }
    ];

    mockMultiSedeService.getSedes.and.returnValue(of(mockSedes));
    mockMultiSedeService.getAreasClinicas.and.returnValue(of(mockAreas));

    component.loadData();

    const disponibles = component.subAreasDisponibles();
    expect(disponibles.length).toBe(2);
    expect(disponibles.some(d => d.nombre === 'Área de Hospitalización')).toBeTrue();
    expect(disponibles.some(d => d.nombre === 'Farmacia')).toBeTrue();
    expect(disponibles.some(d => d.nombre === 'Área de Emergencia')).toBeFalse();
    expect(disponibles.some(d => d.nombre === 'Área de Cirugía')).toBeFalse();
  });
});

