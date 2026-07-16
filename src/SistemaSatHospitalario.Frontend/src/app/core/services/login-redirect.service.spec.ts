import { TestBed } from '@angular/core/testing';
import { LoginRedirectService } from './login-redirect.service';
import { AuthService } from './auth.service';

describe('LoginRedirectService', () => {
  let service: LoginRedirectService;
  let authServiceSpy: jasmine.SpyObj<AuthService>;

  beforeEach(() => {
    const authSpy = jasmine.createSpyObj('AuthService', [
      'isAdmin',
      'isRxAssistant',
      'isEmergencyAssistant',
      'isHospitalAssistant'
    ]);

    TestBed.configureTestingModule({
      providers: [
        LoginRedirectService,
        { provide: AuthService, useValue: authSpy }
      ]
    });

    service = TestBed.inject(LoginRedirectService);
    authServiceSpy = TestBed.inject(AuthService) as jasmine.SpyObj<AuthService>;
  });

  it('debe crearse correctamente', () => {
    expect(service).toBeTruthy();
  });

  it('debe redirigir a reset-password si requirePasswordReset es true', () => {
    const route = service.redirectRoute(true);
    expect(route).toEqual(['/reset-password']);
  });

  it('debe redirigir a dashboard si el usuario es Admin', () => {
    authServiceSpy.isAdmin.and.returnValue(true);
    authServiceSpy.isRxAssistant.and.returnValue(false);
    authServiceSpy.isEmergencyAssistant.and.returnValue(true);
    authServiceSpy.isHospitalAssistant.and.returnValue(true);

    const route = service.redirectRoute(false);
    expect(route).toEqual(['/dashboard']);
  });

  it('debe redirigir a rx-orders si el usuario es RxAssistant', () => {
    authServiceSpy.isAdmin.and.returnValue(false);
    authServiceSpy.isRxAssistant.and.returnValue(true);
    authServiceSpy.isEmergencyAssistant.and.returnValue(false);
    authServiceSpy.isHospitalAssistant.and.returnValue(false);

    const route = service.redirectRoute(false);
    expect(route).toEqual(['/rx-orders']);
  });

  it('debe redirigir a /enfermeria si el usuario es EmergencyAssistant', () => {
    authServiceSpy.isAdmin.and.returnValue(false);
    authServiceSpy.isRxAssistant.and.returnValue(false);
    authServiceSpy.isEmergencyAssistant.and.returnValue(true);
    authServiceSpy.isHospitalAssistant.and.returnValue(false);

    const route = service.redirectRoute(false);
    expect(route).toEqual(['/enfermeria']);
  });

  it('debe redirigir a /cierre-cuenta/Hospitalizacion si el usuario es HospitalAssistant', () => {
    authServiceSpy.isAdmin.and.returnValue(false);
    authServiceSpy.isRxAssistant.and.returnValue(false);
    authServiceSpy.isEmergencyAssistant.and.returnValue(false);
    authServiceSpy.isHospitalAssistant.and.returnValue(true);

    const route = service.redirectRoute(false);
    expect(route).toEqual(['/cierre-cuenta/Hospitalizacion']);
  });

  it('debe redirigir a dashboard por defecto', () => {
    authServiceSpy.isAdmin.and.returnValue(false);
    authServiceSpy.isRxAssistant.and.returnValue(false);
    authServiceSpy.isEmergencyAssistant.and.returnValue(false);
    authServiceSpy.isHospitalAssistant.and.returnValue(false);

    const route = service.redirectRoute(false);
    expect(route).toEqual(['/dashboard']);
  });
});
