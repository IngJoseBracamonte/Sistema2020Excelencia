import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ComprasComponent } from './compras.component';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Package, Plus, Trash2, Edit, Check, X, Search } from 'lucide-angular';

describe('ComprasComponent', () => {
  let component: ComprasComponent;
  let fixture: ComponentFixture<ComprasComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        ComprasComponent,
        HttpClientTestingModule,
        FormsModule,
        LucideAngularModule.pick({ Package, Plus, Trash2, Edit, Check, X, Search })
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ComprasComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize with default active tab as purchase', () => {
    expect(component.activeTab()).toBe('purchase');
  });

  it('should select an item and calculate total price correctly', () => {
    const item = {
      id: '1',
      codigo: 'TEST-1',
      nombre: 'Ibuprofeno + Cafeina 500mg',
      stockActual: 10,
      unidadMedidaBase: 'UNIDAD',
      costoUnitarioBaseUSD: 1.50
    };
    component.selectItem(item);
    component.purchaseQty.set(10);
    component.purchaseCost.set(2.00);
    component.addToCart();

    expect(component.cart().length).toBe(1);
    expect(component.cart()[0].totalUSD).toBe(20.00);
  });
});
