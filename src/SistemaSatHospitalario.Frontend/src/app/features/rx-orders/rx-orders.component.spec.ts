import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RxOrdersComponent } from './rx-orders.component';

describe('RxOrdersComponent', () => {
  let component: RxOrdersComponent;
  let fixture: ComponentFixture<RxOrdersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RxOrdersComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(RxOrdersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
