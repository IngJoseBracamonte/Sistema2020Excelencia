import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { CajaService, AbrirCajaRequest } from './caja.service';
import { environment } from '../../../environments/environment';

describe('CajaService', () => {
    let service: CajaService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [CajaService]
        });

        service = TestBed.inject(CajaService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('debe actualizar la señal al abrir caja', () => {
        const payload: AbrirCajaRequest = { montoInicialDivisa: 100, montoInicialBs: 4500 };

        service.abrirCaja(payload).subscribe();

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Caja/Abrir`);
        expect(req.request.method).toBe('POST');
        req.flush({ success: true });

        expect(service.isCajaAbierta()).toBeTrue();
    });

    it('debe actualizar la señal al cerrar caja', () => {
        service.isCajaAbierta.set(true);

        service.cerrarCaja().subscribe();

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Caja/Cerrar`);
        expect(req.request.method).toBe('POST');
        req.flush({ success: true });

        expect(service.isCajaAbierta()).toBeFalse();
    });

    it('debe construir correctamente la URL de historial con parámetros', () => {
        service.obtenerHistorial('2026-01-01', '2026-01-31', 'admin').subscribe();

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Caja/Historial?desde=2026-01-01&hasta=2026-01-31&usuarioId=admin`);
        expect(req.request.method).toBe('GET');
        req.flush({ granTotalDivisa: 0, granTotalBs: 0, cierres: [] });
    });

    it('debe obtener el reporte personal correctamente', () => {
        service.getPersonalReport('user1').subscribe(report => {
            expect(report.usuario).toBe('user1');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Caja/PersonalReport?userId=user1`);
        expect(req.request.method).toBe('GET');
        req.flush({ usuario: 'user1', totalOrdenes: 5 });
    });
});
