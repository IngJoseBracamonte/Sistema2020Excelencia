import { ComponentFixture, TestBed } from '@angular/core/testing';
import { PedidosAprobacionComponent } from './pedidos-aprobacion.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { MultiSedeService } from '../../../core/services/multi-sede.service';
import { InventoryService } from '../../../core/services/inventory.service';
import { of } from 'rxjs';

describe('PedidosAprobacionComponent', () => {
  let component: PedidosAprobacionComponent;
  let fixture: ComponentFixture<PedidosAprobacionComponent>;
  let multiSedeServiceMock: any;
  let inventoryServiceMock: any;

  beforeEach(async () => {
    multiSedeServiceMock = {
      getPedidosPendientes: jasmine.createSpy('getPedidosPendientes').and.returnValue(of([
        {
          id: 'p1',
          correlativo: 'PED-2026-0001',
          sedeSolicitanteId: 's1',
          sedeSolicitanteNombre: 'Emergencia',
          estado: 'Solicitado',
          fechaCreacion: new Date().toISOString(),
          detalles: [
            { id: 'd1', insumoId: 'ins-1', insumoNombre: 'Omeprazol 40mg', cantidadSolicitada: 5, cantidadDespachada: 0 }
          ]
        }
      ])),
      getPedidosHistorial: jasmine.createSpy('getPedidosHistorial').and.returnValue(of([
        {
          id: 'p2',
          correlativo: 'PED-2026-0000',
          sedeSolicitanteId: 's1',
          sedeSolicitanteNombre: 'UCI',
          estado: 'Despachado',
          fechaCreacion: new Date().toISOString(),
          detalles: [
            { id: 'd2', insumoId: 'ins-1', insumoNombre: 'Omeprazol 40mg', cantidadSolicitada: 10, cantidadDespachada: 10 }
          ]
        }
      ])),
      despacharPedido: jasmine.createSpy('despacharPedido').and.returnValue(of({})),
      rechazarPedido: jasmine.createSpy('rechazarPedido').and.returnValue(of({}))
    };

    inventoryServiceMock = {
      getInsumos: jasmine.createSpy('getInsumos').and.returnValue(of([
        { id: 'ins-1', codigo: 'INS01', nombre: 'Omeprazol 40mg', stockActual: 20 }
      ]))
    };

    await TestBed.configureTestingModule({
      imports: [PedidosAprobacionComponent, HttpClientTestingModule],
      providers: [
        { provide: MultiSedeService, useValue: multiSedeServiceMock },
        { provide: InventoryService, useValue: inventoryServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(PedidosAprobacionComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debe cargar pedidos pendientes e historial al inicializarse', () => {
    expect(multiSedeServiceMock.getPedidosPendientes).toHaveBeenCalled();
    expect(multiSedeServiceMock.getPedidosHistorial).toHaveBeenCalled();
    expect(component.pedidos().length).toBe(1);
    expect(component.historialPedidos().length).toBe(1);
    expect(component.filteredHistorial().length).toBe(1);
  });

  it('debe cambiar de pestaña a historial al invocar activeTab.set', () => {
    component.activeTab.set('historial');
    fixture.detectChanges();
    expect(component.activeTab()).toBe('historial');
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('PED-2026-0000');
  });
});
