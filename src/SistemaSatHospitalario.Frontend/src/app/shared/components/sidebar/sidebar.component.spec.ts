import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SidebarComponent } from './sidebar.component';
import { RouterTestingModule } from '@angular/router/testing';
import { Router, NavigationEnd } from '@angular/router';
import { of, Subject } from 'rxjs';

describe('SidebarComponent', () => {
    let component: SidebarComponent;
    let fixture: ComponentFixture<SidebarComponent>;
    let routerEvents: Subject<any>;

    beforeEach(async () => {
        routerEvents = new Subject<any>();

        await TestBed.configureTestingModule({
            imports: [SidebarComponent, RouterTestingModule],
            providers: [
                {
                    provide: Router,
                    useValue: {
                        events: routerEvents.asObservable(),
                        url: '/'
                    }
                }
            ]
        }).compileComponents();

        fixture = TestBed.createComponent(SidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
    });

    it('debe crearse correctamente', () => {
        expect(component).toBeTruthy();
    });

    it('debe alternar el estado del dropdown al llamar a toggleDropdown', () => {
        expect(component.dropdownsOpen()['caja']).toBeFalse();
        
        component.toggleDropdown('caja');
        expect(component.dropdownsOpen()['caja']).toBeTrue();
        
        component.toggleDropdown('caja');
        expect(component.dropdownsOpen()['caja']).toBeFalse();
    });

    it('debe cerrar otros dropdowns al abrir uno nuevo', () => {
        component.toggleDropdown('caja');
        expect(component.dropdownsOpen()['caja']).toBeTrue();
        
        component.toggleDropdown('medica');
        expect(component.dropdownsOpen()['medica']).toBeTrue();
        expect(component.dropdownsOpen()['caja']).toBeFalse();
    });

    it('debe expandir automáticamente el dropdown "caja" si la ruta es /cajas', (done) => {
        const router = TestBed.inject(Router);
        (router as any).url = '/cajas';
        
        // Simular evento de navegación
        routerEvents.next(new NavigationEnd(1, '/cajas', '/cajas'));
        
        // El sidebar escucha eventos de navegación
        setTimeout(() => {
            expect(component.dropdownsOpen()['caja']).toBeTrue();
            done();
        }, 100);
    });
});
