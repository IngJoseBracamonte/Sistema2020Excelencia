import { ComponentFixture, TestBed } from '@angular/core/testing';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { of } from 'rxjs';
import { ComprasComponent } from './compras.component';
import { InventoryService } from '../../../core/services/inventory.service';
import { CuentasPorPagarService } from '../../../core/services/cuentas-por-pagar.service';
import { SettingsService } from '../../../core/services/settings.service';
import { CatalogService } from '../../../core/services/catalog.service';

describe('ComprasComponent', () => {
  let component: ComprasComponent;
  let fixture: ComponentFixture<ComprasComponent>;
  let mockInventoryService: jasmine.SpyObj<InventoryService>;
  let mockCuentasService: jasmine.SpyObj<CuentasPorPagarService>;
  let mockSettingsService: jasmine.SpyObj<SettingsService>;
  let mockCatalogService: jasmine.SpyObj<CatalogService>;

  const mockInsumos = [
    {
      id: 'ins-1',
      codigo: 'MED-001',
      nombre: 'Vitamina C Ampollas',
      stockActual: 50,
      unidadMedidaBase: 'UNIDAD',
      costoUnitarioBaseUSD: 0.25,
      permiteFraccionamiento: true
    }
  ];

  beforeEach(async () => {
    mockInventoryService = jasmine.createSpyObj('InventoryService', [
      'getInsumos',
      'getPrincipiosActivos',
      'getProveedores',
      'recordPurchase'
    ]);
    mockCuentasService = jasmine.createSpyObj('CuentasPorPagarService', [
      'getOrdenes',
      'getHistorialPagos'
    ]);
    mockSettingsService = jasmine.createSpyObj('SettingsService', [], {
      tasa$: of(50.00)
    });
    mockCatalogService = jasmine.createSpyObj('CatalogService', [
      'getPaymentMethods'
    ]);

    mockInventoryService.getInsumos.and.returnValue(of(mockInsumos));
    mockInventoryService.getPrincipiosActivos.and.returnValue(of([]));
    mockInventoryService.getProveedores.and.returnValue(of([]));
    mockInventoryService.recordPurchase.and.returnValue(of({ message: 'Success' }));
    mockCuentasService.getOrdenes.and.returnValue(of([]));
    mockCuentasService.getHistorialPagos.and.returnValue(of([]));
    mockCatalogService.getPaymentMethods.and.returnValue(of([]));

    await TestBed.configureTestingModule({
      imports: [HttpClientTestingModule, FormsModule, ComprasComponent],
      providers: [
        { provide: InventoryService, useValue: mockInventoryService },
        { provide: CuentasPorPagarService, useValue: mockCuentasService },
        { provide: SettingsService, useValue: mockSettingsService },
        { provide: CatalogService, useValue: mockCatalogService }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ComprasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('debe crearse correctamente e inicializar el formulario de compras', () => {
    expect(component).toBeTruthy();
    expect(component.purchaseQty()).toBe(1);
  });

  it('debe agregar un renglon con cantidad kardex directa y costo calculado', () => {
    component.selectedInsumo.set(mockInsumos[0]);
    component.purchaseQty.set(10000); // 10000 unidades directas
    component.costMode.set('unitario');
    component.purchaseCost.set(0.25);

    component.addToCart();

    const cart = component.cart();
    expect(cart.length).toBe(1);
    expect(cart[0].cantidad).toBe(10000);
    expect(cart[0].precioCostoUSD).toBe(0.25);
    expect(cart[0].totalUSD).toBe(2500);

    // Formulario debe resetearse
    expect(component.selectedInsumo()).toBeNull();
  });

  it('debe enviar la compra al backend con los items del carrito', () => {
    component.cart.set([
      {
        insumoId: 'ins-1',
        codigo: 'MED-001',
        nombre: 'Vitamina C Ampollas',
        cantidad: 10000,
        precioCostoUSD: 0.25,
        totalUSD: 2500
      }
    ]);
    component.numeroFacturaInput.set('FAC-001');

    component.submitPurchase();

    expect(mockInventoryService.recordPurchase).toHaveBeenCalledWith(
      jasmine.objectContaining({
        numeroFactura: 'FAC-001',
        items: [
          {
            insumoId: 'ins-1',
            cantidad: 10000,
            precioCostoUSD: 0.25
          }
        ]
      })
    );
  });
});
