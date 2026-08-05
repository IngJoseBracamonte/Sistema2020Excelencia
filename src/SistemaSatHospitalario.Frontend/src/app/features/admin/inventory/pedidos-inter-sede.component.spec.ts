import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PedidosInterSedeComponent } from './pedidos-inter-sede.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MultiSedeService } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { AuthService } from '../../../core/services/auth.service';
import { of } from 'rxjs';
import { signal } from '@angular/core';

describe('PedidosInterSedeComponent', () => {
  let component: PedidosInterSedeComponent;
  let fixture: ComponentFixture<PedidosInterSedeComponent>;
  let multiSedeServiceMock: any;
  let inventoryServiceMock: any;
  let authServiceMock: any;

  beforeEach(async () => {
    multiSedeServiceMock = {
      getSedes: jasmine.createSpy('getSedes').and.returnValue(of([
        { id: '10000000-0000-0000-0000-000000000001', codigo: 'PRN', nombre: 'Almacén Principal', esPrincipal: true, activo: true },
        { id: '10000000-0000-0000-0000-000000000002', codigo: 'EMG', nombre: 'Área de Emergencia', esPrincipal: false, activo: true },
        { id: '10000000-0000-0000-0000-000000000003', codigo: 'HOSP', nombre: 'Área de Hospitalización', esPrincipal: false, activo: true },
        { id: '10000000-0000-0000-0000-000000000004', codigo: 'UCI', nombre: 'Unidad de Cuidados Intensivos', esPrincipal: false, activo: true }
      ])),
      getPedidosPendientes: jasmine.createSpy('getPedidosPendientes').and.returnValue(of([])),
      createPedido: jasmine.createSpy('createPedido').and.returnValue(of({}))
    };

    inventoryServiceMock = {
      getInsumos: jasmine.createSpy('getInsumos').and.returnValue(of([
        { id: 'ins-1', codigo: 'INS01', nombre: 'Omeprazol 40mg', stockActual: 10, unidadMedidaBase: 'VIAL' }
      ]))
    };

    authServiceMock = {
      currentUser: signal({ username: 'enfermera1', role: 'Enfermeria' }),
      isAdmin: jasmine.createSpy('isAdmin').and.returnValue(false),
      isSupervisor: jasmine.createSpy('isSupervisor').and.returnValue(false),
      isEmergencyAssistant: jasmine.createSpy('isEmergencyAssistant').and.returnValue(true)
    };

    await TestBed.configureTestingModule({
      imports: [PedidosInterSedeComponent, HttpClientTestingModule],
      providers: [
        { provide: MultiSedeService, useValue: multiSedeServiceMock },
        { provide: InventoryService, useValue: inventoryServiceMock },
        { provide: AuthService, useValue: authServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PedidosInterSedeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('no debe renderizar "TIPO DE DESTINO" en el DOM', () => {
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).not.toContain('TIPO DE DESTINO');
  });

  it('no debe renderizar el badge de rol en amarillo', () => {
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).not.toContain('👑 Rol: Supervisor / Admin');
  });

  it('debe sincronizar la sede solicitante al cambiar areaNombre a UCI', () => {
    component.areaNombre = 'UCI';
    fixture.detectChanges();
    expect(component.newPedido.sedeSolicitanteId).toBe('10000000-0000-0000-0000-000000000004');
  });
});
