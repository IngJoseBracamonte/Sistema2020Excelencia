import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SidebarComponent } from './sidebar.component';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthService } from '../../../core/services/auth.service';
import { signal } from '@angular/core';

describe('SidebarComponent', () => {
    let component: SidebarComponent;
    let fixture: ComponentFixture<SidebarComponent>;
    let router: Router;
    let authServiceMock: any;

    beforeEach(async () => {
        authServiceMock = {
            currentUser: signal({
                token: 'token',
                username: 'admin',
                role: 'Administrador',
                id: '1',
                permissions: [],
                requirePasswordReset: false
            }),
            isAdministrador: jasmine.createSpy('isAdministrador').and.returnValue(true),
            isParticularAssistant: jasmine.createSpy('isParticularAssistant').and.returnValue(false),
            isInsuranceAssistant: jasmine.createSpy('isInsuranceAssistant').and.returnValue(false),
            isRxAssistant: jasmine.createSpy('isRxAssistant').and.returnValue(false),
            isTomographyAssistant: jasmine.createSpy('isTomographyAssistant').and.returnValue(false),
            isSupervisor: jasmine.createSpy('isSupervisor').and.returnValue(false)
        };


        await TestBed.configureTestingModule({
            imports: [SidebarComponent, RouterTestingModule, HttpClientTestingModule],
            providers: [
                { provide: AuthService, useValue: authServiceMock }
            ]
        }).compileComponents();

        router = TestBed.inject(Router);
    });

    it('debe crearse correctamente', () => {
        fixture = TestBed.createComponent(SidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
        expect(component).toBeTruthy();
    });

    it('debe alternar el estado del dropdown al llamar a toggleDropdown', () => {
        fixture = TestBed.createComponent(SidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();

        expect(component.dropdownsOpen()['caja']).toBeFalse();
        
        component.toggleDropdown('caja');
        expect(component.dropdownsOpen()['caja']).toBeTrue();
        
        component.toggleDropdown('caja');
        expect(component.dropdownsOpen()['caja']).toBeFalse();
    });

    it('debe mantener otros dropdowns abiertos al abrir uno nuevo', () => {
        fixture = TestBed.createComponent(SidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();

        component.toggleDropdown('caja');
        expect(component.dropdownsOpen()['caja']).toBeTrue();
        
        component.toggleDropdown('medica');
        expect(component.dropdownsOpen()['medica']).toBeTrue();
        expect(component.dropdownsOpen()['caja']).toBeTrue();
    });

    it('debe expandir automáticamente el dropdown "caja" si la ruta es /cajas', () => {
        spyOnProperty(router, 'url', 'get').and.returnValue('/cajas');
        
        fixture = TestBed.createComponent(SidebarComponent);
        component = fixture.componentInstance;
        fixture.detectChanges();
        
        expect(component.dropdownsOpen()['caja']).toBeTrue();
    });
});

