import { ComponentFixture, TestBed } from '@angular/core/testing';
import { StockMultisedeComponent } from './stock-multisede.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { InventoryService } from '../../../core/services/inventory.service';
import { MultiSedeService } from '../../../core/services/multi-sede.service';
import { of } from 'rxjs';

describe('StockMultisedeComponent', () => {
  let component: StockMultisedeComponent;
  let fixture: ComponentFixture<StockMultisedeComponent>;
  let inventoryServiceMock: any;
  let multiSedeServiceMock: any;

  beforeEach(async () => {
    inventoryServiceMock = {
      getInsumos: jasmine.createSpy('getInsumos').and.returnValue(of([
        { id: '1', codigo: 'INS01', nombre: 'Gasas Esteriles', stockActual: 10, unidadMedidaBase: 'UNIDAD' }
      ])),
      getStockConsolidado: jasmine.createSpy('getStockConsolidado').and.returnValue(of([
        { insumoId: '1', codigo: 'INS01', nombre: 'Gasas Esteriles', stockActual: 25, unidadMedidaBase: 'UNIDAD', stockMinimo: 5 }
      ])),
      getStockSedeById: jasmine.createSpy('getStockSedeById').and.returnValue(of([])),
      getKardex: jasmine.createSpy('getKardex').and.returnValue(of({
        initialBalance: 10,
        totalEntradas: 20,
        totalSalidas: 5,
        finalBalance: 25,
        movimientos: [
          {
            id: 'm1',
            insumoId: '1',
            insumoNombre: 'Gasas Esteriles',
            insumoCodigo: 'INS01',
            sedeId: 's1',
            sedeNombre: 'Emergencia',
            tipoMovimiento: 'TransferenciaEntrada',
            cantidadBase: 20,
            unidadMedida: 'UNIDAD',
            usuario: 'Admin',
            motivo: 'Reposición',
            fecha: new Date().toISOString()
          }
        ]
      }))
    };

    multiSedeServiceMock = {
      getSedes: jasmine.createSpy('getSedes').and.returnValue(of([
        { id: 's1', codigo: 'EMG', nombre: 'Emergencia', esPrincipal: false, activo: true },
        { id: 's2', codigo: 'PRN', nombre: 'Almacén Principal', esPrincipal: true, activo: true }
      ]))
    };

    await TestBed.configureTestingModule({
      imports: [StockMultisedeComponent, HttpClientTestingModule],
      providers: [
        { provide: InventoryService, useValue: inventoryServiceMock },
        { provide: MultiSedeService, useValue: multiSedeServiceMock }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(StockMultisedeComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente', () => {
    expect(component).toBeTruthy();
  });

  it('debe cargar stock consolidado global al seleccionar TODAS', () => {
    component.onSedeChange('TODAS');
    expect(inventoryServiceMock.getStockConsolidado).toHaveBeenCalled();
    expect(component.stockItems().length).toBe(1);
    expect(component.stockItems()[0].stockActual).toBe(25);
  });

  it('debe cambiar de pestaña a Kardex y cargar los movimientos', () => {
    component.onTabChange('kardex');
    expect(component.activeTab()).toBe('kardex');
    expect(inventoryServiceMock.getKardex).toHaveBeenCalled();
    expect(component.kardexResult()?.finalBalance).toBe(25);
  });
});
