import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { AuthService, LoginRequest, AuthResponse } from './auth.service';
import { Router } from '@angular/router';
import { environment } from '../../../environments/environment';

describe('AuthService', () => {
    let service: AuthService;
    let httpMock: HttpTestingController;
    let router: jasmine.SpyObj<Router>;

    beforeEach(() => {
        const routerSpy = jasmine.createSpyObj('Router', ['navigate']);
        localStorage.clear();

        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [
                AuthService,
                { provide: Router, useValue: routerSpy }
            ]
        });

        service = TestBed.inject(AuthService);
        httpMock = TestBed.inject(HttpTestingController);
        router = TestBed.inject(Router) as jasmine.SpyObj<Router>;
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('debe crearse correctamente', () => {
        expect(service).toBeTruthy();
    });

    it('debe realizar login exitoso y persistir datos en LocalStorage', () => {
        const mockResponse = {
            token: 'mock-jwt-token',
            username: 'admin',
            role: 'Administrador',
            userId: 'user-123'
        };

        const credentials: LoginRequest = { username: 'admin', password: 'password' };

        service.login(credentials).subscribe(response => {
            expect(response.token).toBe('mock-jwt-token');
            expect(response.id).toBe('user-123');
            expect(service.currentUser()?.username).toBe('admin');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/Auth/Login`);
        expect(req.request.method).toBe('POST');
        req.flush(mockResponse);

        expect(localStorage.getItem('jwt_token')).toBe('mock-jwt-token');
        expect(localStorage.getItem('username')).toBe('admin');
        expect(localStorage.getItem('user_id')).toBe('user-123');
    });

    it('debe manejar error de login correctamente', () => {
        const credentials: LoginRequest = { username: 'wrong', password: 'wrong' };

        service.login(credentials).subscribe({
            next: () => fail('debería haber fallado'),
            error: (error) => {
                expect(error.message).toBe('Credenciales inválidas o Error en el Servidor');
            }
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/Auth/Login`);
        req.error(new ErrorEvent('Unauthorized'), { status: 401 });
    });

    it('debe limpiar LocalStorage al cerrar sesión', () => {
        localStorage.setItem('jwt_token', 'token');
        service.currentUser.set({ token: 'token', username: 'u', role: 'r', id: '1' });

        service.logout();

        expect(localStorage.getItem('jwt_token')).toBeNull();
        expect(service.currentUser()).toBeNull();
        expect(router.navigate).toHaveBeenCalledWith(['/login']);
    });

    it('debe recuperar la sesión desde LocalStorage al inicializar', () => {
        localStorage.setItem('jwt_token', 'saved-token');
        localStorage.setItem('username', 'saved-user');
        localStorage.setItem('user_role', 'Admin');
        localStorage.setItem('user_id', 'id-1');

        // Note: The signal is initialized in the constructor, so we need a new service instance
        const newService = TestBed.runInInjectionContext(() => new AuthService());
        
        expect(newService.currentUser()?.token).toBe('saved-token');
        expect(newService.currentUser()?.username).toBe('saved-user');
    });
});
