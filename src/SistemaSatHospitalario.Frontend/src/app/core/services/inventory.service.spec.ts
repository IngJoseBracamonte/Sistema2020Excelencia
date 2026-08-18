import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { InventoryService } from './inventory.service';
import { TransferenciaReposicionItem, ProcesarReposicionRequest } from '../models/inventory.model';
import { environment } from '../../../environments/environment';

describe('InventoryService', () => {
  let service: InventoryService;
  let httpMock: HttpTestingController;

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [HttpClientTestingModule],
      providers: [InventoryService]
    });
    service = TestBed.inject(InventoryService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('getInsumos should fetch list of inventory items', () => {
    const mockInsumos = [
      {
        id: 'ins-1',
        codigo: 'INS-01',
        nombre: 'Guantes Quirúrgicos',
        stockActual: 50,
        unidadMedidaBase: 'UNIDAD',
        costoUnitarioBaseUSD: 1.5
      }
    ];

    service.getInsumos(true, 'Guantes', 'sede-1').subscribe(res => {
      expect(res.length).toBe(1);
      expect(res[0].nombre).toBe('Guantes Quirúrgicos');
    });

    const req = httpMock.expectOne(r => r.url === `${environment.apiUrl}/api/inventory/insumos`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('search')).toBe('Guantes');
    expect(req.request.params.get('sedeId')).toBe('sede-1');
    req.flush(mockInsumos);
  });

  it('procesarReposicion should post request to /api/Inventario/Reposicion', () => {
    const requestPayload: ProcesarReposicionRequest = {
      insumoId: 'ins-1',
      sedeOrigenId: 'sede-orig-1',
      sedeDestinoId: 'sede-dest-2',
      cantidad: 10,
      motivo: 'CambioTalla',
      observaciones: 'Ajuste de stock'
    };

    service.procesarReposicion(requestPayload).subscribe(res => {
      expect(res.success).toBeTrue();
    });

    const req = httpMock.expectOne(`${environment.apiUrl}/api/Inventario/Reposicion`);
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(requestPayload);
    req.flush({ success: true });
  });

  it('getReposicionesHistorial should fetch and update reposicionesHistorial signal', () => {
    const mockHistorial: TransferenciaReposicionItem[] = [
      {
        id: 'trans-1',
        insumoId: 'ins-1',
        insumoNombre: 'Guantes',
        insumoCodigo: 'INS01',
        sedeOrigenId: 'sede-1',
        sedeOrigenNombre: 'Almacén Central',
        sedeDestinoId: 'sede-2',
        sedeDestinoNombre: 'Emergencia',
        cantidad: 10,
        motivo: 'Reposicion',
        fechaTransferencia: '2026-08-16T12:00:00Z',
        usuarioId: 'admin'
      }
    ];

    service.getReposicionesHistorial({ sedeId: 'sede-1', motivo: 'Reposicion' }).subscribe(res => {
      expect(res.length).toBe(1);
      expect(service.reposicionesHistorial().length).toBe(1);
      expect(service.reposicionesHistorial()[0].motivo).toBe('Reposicion');
    });

    const req = httpMock.expectOne(r => r.url === `${environment.apiUrl}/api/Inventario/Reposicion/Historial`);
    expect(req.request.method).toBe('GET');
    expect(req.request.params.get('sedeId')).toBe('sede-1');
    expect(req.request.params.get('motivo')).toBe('Reposicion');
    req.flush(mockHistorial);
  });
});
