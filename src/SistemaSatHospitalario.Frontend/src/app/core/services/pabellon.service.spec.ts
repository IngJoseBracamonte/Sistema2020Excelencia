import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { PabellonService, OrdenCirugia, OrdenCirugiaDetalle, CrearOrdenCirugiaRequest } from './pabellon.service';
import { environment } from '../../../environments/environment';

describe('PabellonService', () => {
  let service: PabellonService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [PabellonService]
    });
    service = TestBed.inject(PabellonService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getOrdenes should fetch list of surgery orders', () => {
    const mockData: OrdenCirugia[] = [
      {
        id: 'ord-1',
        cuentaServicioId: 'cta-1',
        pacienteId: 'pac-1',
        pacienteNombre: 'Juan Pérez',
        pacienteCedula: 'V-12345678',
        descripcionCirugia: 'Apendicectomía',
        precioBaseUsd: 1200,
        medicoId: 'med-1',
        medicoNombre: 'Dr. López',
        fechaHoraProgramada: '2026-07-28T09:00:00Z',
        estado: 'PendienteEjecucion',
        fechaCreacion: '2026-07-27T10:00:00Z',
        usuarioCreacion: 'admin'
      }
    ];

    service.getOrdenes().subscribe(res => {
      expect(res.length).toBe(1);
      expect(res[0].descripcionCirugia).toBe('Apendicectomía');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/Pabellon/Ordenes`);
    expect(req.request.method).toBe('GET');
    req.flush(mockData);
  });

  it('getOrdenDetalle should fetch surgery order detail with logs and insumos', () => {
    const mockDetail: OrdenCirugiaDetalle = {
      id: 'ord-1',
      cuentaServicioId: 'cta-1',
      pacienteId: 'pac-1',
      pacienteNombre: 'Juan Pérez',
      pacienteCedula: 'V-12345678',
      descripcionCirugia: 'Apendicectomía',
      precioBaseUsd: 1200,
      medicoId: 'med-1',
      medicoNombre: 'Dr. López',
      fechaHoraProgramada: '2026-07-28T09:00:00Z',
      estado: 'EnProceso',
      fechaCreacion: '2026-07-27T10:00:00Z',
      usuarioCreacion: 'admin',
      logs: [
        { id: 'l-1', ordenCirugiaId: 'ord-1', usuarioId: 'admin', evento: 'Creacion', detalle: 'Orden creada', timestamp: '2026-07-27T10:00:00Z' }
      ],
      insumosAsignados: [
        { insumoId: 'ins-1', insumoNombre: 'Bisturí #11', insumoCodigo: 'INS01', cantidadEntregada: 5, cantidadDevuelta: 1, cantidadConsumida: 4, precioUnitarioUsd: 10 }
      ]
    };

    service.getOrdenDetalle('ord-1').subscribe(res => {
      expect(res.id).toBe('ord-1');
      expect(res.logs.length).toBe(1);
      expect(res.insumosAsignados.length).toBe(1);
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/Pabellon/Ordenes/ord-1`);
    expect(req.request.method).toBe('GET');
    req.flush(mockDetail);
  });

  it('crearOrden should post new surgery order', () => {
    const reqDto: CrearOrdenCirugiaRequest = {
      cuentaServicioId: 'cta-1',
      pacienteId: 'pac-1',
      descripcionCirugia: 'Hernioplastia Inguinal',
      precioBaseUsd: 950,
      medicoId: 'med-1',
      fechaHoraProgramada: '2026-07-29T08:00:00Z'
    };

    service.crearOrden(reqDto).subscribe(res => {
      expect(res.id).toBe('new-ord-id');
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/Pabellon/Ordenes`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(reqDto);
    req.flush({ id: 'new-ord-id' });
  });

  it('devolucionMasiva should post items to return to main branch', () => {
    const reqDto = {
      ordenCirugiaId: 'ord-1',
      cuentaId: 'cta-1',
      items: [{ insumoId: 'ins-1', cantidadUsada: 3 }]
    };

    service.devolucionMasiva(reqDto).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/Pabellon/DevolucionMasiva`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(reqDto);
    req.flush({ success: true });
  });
});
