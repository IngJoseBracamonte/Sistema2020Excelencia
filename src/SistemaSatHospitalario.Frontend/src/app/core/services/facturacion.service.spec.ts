import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { FacturacionService, RegistrarReciboFacturaRequest, CargarServicioACuentaRequest } from './facturacion.service';
import { environment } from '../../../environments/environment';

describe('FacturacionService', () => {
    let service: FacturacionService;
    let httpMock: HttpTestingController;

    beforeEach(() => {
        TestBed.configureTestingModule({
            imports: [HttpClientTestingModule],
            providers: [FacturacionService]
        });

        service = TestBed.inject(FacturacionService);
        httpMock = TestBed.inject(HttpTestingController);
    });

    afterEach(() => {
        httpMock.verify();
    });

    it('debe enviar la carga de servicio correctamente', () => {
        const payload: CargarServicioACuentaRequest = {
            pacienteId: 1,
            tipoIngreso: 'Particular',
            servicioId: 'id-serv',
            descripcion: 'Consulta',
            precio: 50,
            cantidad: 1,
            tipoServicio: 'CONSULTA',
            usuarioCarga: 'admin'
        };

        service.cargarServicio(payload).subscribe(response => {
            expect(response.success).toBeTrue();
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Billing/CargarServicio`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(payload);
        req.flush({ success: true });
    });

    it('debe registrar el pago multidivisa', () => {
        const payload: RegistrarReciboFacturaRequest = {
            cuentaServicioId: 'guid-cuenta',
            pacienteId: 1,
            cajeroUserId: 'admin',
            tasaCambioDia: 45.5,
            pagosMultidivisa: []
        };

        service.registrarPago(payload).subscribe();

        const req = httpMock.expectOne(`${environment.apiUrl}/api/ReciboFactura/RegistrarPagoMultidivisa`);
        expect(req.request.method).toBe('POST');
        req.flush({ id: 'recibo-123' });
    });

    it('debe obtener datos de impresión correctamente', () => {
        service.getReceiptPrintData('recibo-id').subscribe(data => {
            expect(data.numeroRecibo).toBe('REC-001');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/ReciboFactura/recibo-id/Print`);
        expect(req.request.method).toBe('GET');
        req.flush({ numeroRecibo: 'REC-001' });
    });
});
