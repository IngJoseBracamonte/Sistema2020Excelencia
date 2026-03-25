import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { ReceivablesService, PendingAR, SettleARRequest } from './receivables.service';
import { environment } from '../../../environments/environment';

describe('ReceivablesService', () => {
    let service: ReceivablesService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [ReceivablesService]
        });

        service = TestBed.inject(ReceivablesService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('debe obtener las cuentas por cobrar pendientes con filtros', () => {
        const mockARs: PendingAR[] = [
            { id: '1', cuentaId: 'C-01', pacienteNombre: 'JUAN PEREZ', pacienteCedula: '123', tipoIngreso: 'Seguro', seguroNombre: 'SEGUROS CARACAS', montoTotal: 100, saldoPendiente: 100, fechaEmision: '2026-01-01', estado: 'Pendiente' }
        ];

        service.getPending('JUAN', 'Pendiente').subscribe(ars => {
            expect(ars.length).toBe(1);
            expect(ars[0].pacienteNombre).toBe('JUAN PEREZ');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/Receivables/Pending?searchTerm=JUAN&estado=Pendiente`);
        expect(req.request.method).toBe('GET');
        req.flush(mockARs);
    });

    it('debe enviar la liquidación de una cuenta correctamente', () => {
        const payload: SettleARRequest = {
            arId: 'ar-123',
            referenciaPago: 'REF-001',
            observaciones: 'Pago total'
        };

        service.settle(payload).subscribe(response => {
            expect(response.success).toBeTrue();
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/Receivables/Settle`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(payload);
        req.flush({ success: true });
    });
});
