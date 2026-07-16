import { ComponentFixture, TestBed } from '@angular/core/testing';
import { SidebarComponent } from './sidebar.component';
import { RouterTestingModule } from '@angular/router/testing';
import { Router } from '@angular/router';
import { HttpClientTestingModule } from '@angular/common/http/testing';
import { AuthService } from '../../../core/services/auth.service';
import { signal } from '@angular/core';

const IS_ADMINISTRADOR = 'isAdministrador';
const IS_PARTICULAR_ASSISTANT = 'isParticularAssistant';
const IS_INSURANCE_ASSISTANT = 'isInsuranceAssistant';
const IS_RX_ASSISTANT = 'isRxAssistant';
const IS_TOMOGRAPHY_ASSISTANT = 'isTomographyAssistant';
const IS_SUPERVISOR = 'isSupervisor';
const IS_INVENTORY_SUPERVISOR = 'isInventorySupervisor';
const IS_EMERGENCY_ASSISTANT = 'isEmergencyAssistant';
const IS_HOSPITAL_ASSISTANT = 'isHospitalAssistant';

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
            isAdministrador: jasmine.createSpy(IS_ADMINISTRADOR).and.returnValue(true),
            isParticularAssistant: jasmine.createSpy(IS_PARTICULAR_ASSISTANT).and.returnValue(false),
            isInsuranceAssistant: jasmine.createSpy(IS_INSURANCE_ASSISTANT).and.returnValue(false),
            isRxAssistant: jasmine.createSpy(IS_RX_ASSISTANT).and.returnValue(false),
            isTomographyAssistant: jasmine.createSpy(IS_TOMOGRAPHY_ASSISTANT).and.returnValue(false),
            isSupervisor: jasmine.createSpy(IS_SUPERVISOR).and.returnValue(false),
            isInventorySupervisor: jasmine.createSpy(IS_INVENTORY_SUPERVISOR).and.returnValue(false),
            isEmergencyAssistant: jasmine.createSpy(IS_EMERGENCY_ASSISTANT).and.returnValue(false),
            isHospitalAssistant: jasmine.createSpy(IS_HOSPITAL_ASSISTANT).and.returnValue(false)
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

