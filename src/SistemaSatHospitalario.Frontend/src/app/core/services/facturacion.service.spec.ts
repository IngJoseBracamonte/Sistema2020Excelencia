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
            pacienteId: '1',
            tipoIngreso: 'Particular',
            servicioId: 'id-serv',
            descripcion: 'Consulta',
            precio: 50,
            honorario: 10,
            cantidad: 1,
            tipoServicio: 'CONSULTA',
            usuarioCarga: 'admin'
        };

        service.cargarServicio(payload).subscribe(response => {
            expect(response.cuentaId).toBe('c1');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Billing/cargar-servicio`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(payload);
        req.flush({ cuentaId: 'c1', detalleId: 'd1' });
    });

    it('debe registrar el recibo factura correctamente', () => {
        const payload: RegistrarReciboFacturaRequest = {
            cuentaServicioId: 'guid-cuenta',
            pacienteId: '1',
            cajeroUserId: 'admin',
            tasaCambioDia: 45.5,
            pagosMultidivisa: []
        };

        service.registrarReciboFactura(payload).subscribe(res => {
            expect(res.facturaId).toBe('fac-123');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Billing/recibo-factura`);
        expect(req.request.method).toBe('POST');
        req.flush({ facturaId: 'fac-123', numeroFactura: 'REC-001' });
    });

    it('debe obtener datos de impresión correctamente', () => {
        service.getReciboPrintData('recibo-id').subscribe((data: any) => {
            expect(data.numeroFactura).toBe('REC-001');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Billing/recibos/recibo-id/print`);
        expect(req.request.method).toBe('GET');
        req.flush({ numeroFactura: 'REC-001' });
    });

    it('debe enviar la apertura de cuenta correctamente con tipoIngreso, convenioId y camaId', () => {
        service.abrirCuenta('pac-123', 'Emergencia', 5, 'cama-12').subscribe(response => {
            expect(response.cuentaId).toBe('acc-789');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Billing/abrir-cuenta`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual({
            pacienteId: 'pac-123',
            tipoIngreso: 'Emergencia',
            convenioId: 5,
            camaId: 'cama-12'
        });
        req.flush({ cuentaId: 'acc-789', message: 'Cuenta clínica abierta exitosamente.' });
    });

    it('debe enviar el cierre de cuenta correctamente', () => {
        const payload: any = {
            cuentaId: 'acc-123',
            usuarioCajero: 'cajero1',
            usuarioId: 'usr-1',
            tasaCambio: 45.0,
            pagos: []
        };

        service.closeAccount(payload).subscribe(response => {
            expect(response.reciboId).toBe('rec-123');
        });

        const req = httpMock.expectOne(`${environment.apiUrl}/api/Billing/CloseAccount`);
        expect(req.request.method).toBe('POST');
        expect(req.request.body).toEqual(payload);
        req.flush({ reciboId: 'rec-123', id: 'rec-123' });
    });
});


