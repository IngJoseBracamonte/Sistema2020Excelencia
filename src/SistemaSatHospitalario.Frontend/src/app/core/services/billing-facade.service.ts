import { Injectable, inject, signal, computed } from '@angular/core';
import { FacturacionService, CargarServicioACuentaRequest, DetallePagoDto, SyncCarritoMasivoRequest } from './facturacion.service';
import { CatalogItem } from '../models/priced-item.model';
import { SpecialtyService } from './specialty.service';
import { AppointmentsService, Doctor } from './appointments.service';
import { AuthService } from './auth.service';
import { BehaviorSubject, Observable, from, of } from 'rxjs';
import { concatMap, tap, catchError, finalize, switchMap } from 'rxjs/operators';
import { takeUntilDestroyed, toSignal, toObservable } from '@angular/core/rxjs-interop';
import { SettingsService } from './settings.service';

/**
 * BillingFacadeService (Pachón Pro V7.0)
 * Implementa el patrón Facade y State Management con Signals.
 * Centraliza el estado de la facturación actual, desacoplando la lógica del componente.
 */
@Injectable({
  providedIn: 'root'
})
export class BillingFacadeService {
  private facturacionService = inject(FacturacionService);
  private authService = inject(AuthService);
  private settingsService = inject(SettingsService);
  private specialtyService = inject(SpecialtyService);
  private appointmentsService = inject(AppointmentsService);

  // --- Estado Requerido (Signals) ---
  public carritoLocal = signal<any[]>([]);
  public serviciosEnBackend = signal<any[]>([]);
  public cuentaId = signal<string | null>(null);
  public pagos = signal<DetallePagoDto[]>([]);
  public tasaCambioDia = signal<number>(0);

  // --- Búsqueda y Catálogo ---
  public searchTermServicio = signal<string>('');
  public selectedEspecialidad = signal<string | null>(null);
  public selectedMedicoId = signal<string | null>(null);
  public servicesCatalog = signal<CatalogItem[]>([]);
  public medicos = signal<any[]>([]);
  public especialidades = signal<string[]>([]);

  // --- Selectores (Computed Signals) ---
  public serviciosCargados = computed(() => [...this.serviciosEnBackend(), ...this.carritoLocal()]);
  
  public totalCargadoUSD = computed(() => {
    return this.serviciosCargados().reduce((acc: number, curr: any) => {
      return acc + (curr.precioUsd ?? curr.PrecioUsd ?? 0);
    }, 0);
  });

  public totalCargadoBS = computed(() => {
    return this.totalCargadoUSD() * this.tasaCambioDia();
  });

  public totalFacturadoUSD = computed(() => 
    this.pagos().reduce((acc, curr) => acc + curr.equivalenteAbonadoBase, 0)
  );

  public medicosFiltrados = toSignal(
    toObservable(this.selectedEspecialidad).pipe(
      switchMap((esp: string | null) => esp ? this.appointmentsService.getDoctorsBySpecialty(esp) : of([] as Doctor[]))
    ), { initialValue: [] as Doctor[] }
  );

  public serviciosFiltrados = computed(() => {
    let filtered = this.servicesCatalog();
    const term = (this.searchTermServicio() || '').toLowerCase().trim();
    const esp = this.selectedEspecialidad();

    if (!term && esp) {
      const searchKey = this.getSearchKey(esp);
      filtered = filtered.filter(s =>
        s.descripcion.toUpperCase().includes(searchKey) ||
        s.tipo.toUpperCase().includes(searchKey)
      );
    }

    if (term) {
      filtered = filtered.filter(s =>
        s.descripcion.toLowerCase().includes(term) ||
        s.tipo.toLowerCase().includes(term) ||
        s.id.toLowerCase().includes(term)
      );
    }

    return filtered;
  });

  private getSearchKey(esp: string): string {
    const map: any = { 'Ginecologia': 'GINE', 'Cardiologia': 'CARD', 'Traumatologia': 'TRAU' };
    return (map[esp] || esp.toUpperCase().substring(0, 4));
  }

  public saldoPendienteUSD = computed(() => 
    Math.max(0, this.totalCargadoUSD() - this.totalFacturadoUSD())
  );

  constructor() {
    // Sincronizar tasa de cambio global
    this.settingsService.tasa$.subscribe(tasa => this.tasaCambioDia.set(tasa));
  }

  // --- Acciones de Negocio ---

  public addServiceToLocalCart(service: CatalogItem, extraData: any = {}) {
    this.carritoLocal.update(prev => [...prev, { ...service, ...extraData }]);
  }

  public removeService(index: number, isBackend: boolean) {
    if (isBackend) {
      const item = this.serviciosEnBackend()[index];
      if (this.cuentaId() && item.detalleId) {
        return this.facturacionService.quitarServicio(
          this.cuentaId()!, 
          item.detalleId, 
          item.medicoId, 
          item.hora
        ).pipe(
          tap(() => {
            this.serviciosEnBackend.update(prev => prev.filter((_, i) => i !== index));
          })
        );
      }
    } else {
      this.carritoLocal.update(prev => prev.filter((_, i) => i !== index));
    }
    return of(null);
  }

  public addPago(pago: DetallePagoDto) {
    this.pagos.update(prev => [...prev, { ...pago }]);
  }

  public removePago(index: number) {
    this.pagos.update(prev => prev.filter((_, i) => i !== index));
  }

  public clearAll() {
    this.carritoLocal.set([]);
    this.serviciosEnBackend.set([]);
    this.pagos.set([]);
    this.cuentaId.set(null);
  }

   /**
   * Sincroniza el carrito local con el backend mediante una única transacción atómica (V11.1 Guid)
   */
  public syncCartWithBackend(pacienteId: string, tipoIngreso: string, usuarioCarga?: string, convenioId?: number | null): Observable<any> {
    const items: any[] = [...this.carritoLocal()];
    if (items.length === 0) return of(null);

    const user = usuarioCarga || this.authService.currentUser()?.username || '';

    const payload: SyncCarritoMasivoRequest = {
      pacienteId,
      tipoIngreso,
      usuarioCarga: user,
      convenioId: convenioId || undefined,
      items: items.map(s => ({
        servicioId: s.id,
        descripcion: s.descripcion,
        precio: s.precio,
        cantidad: 1,
        tipoServicio: s.tipo,
        medicoId: s.medicoId || undefined,
        horaCita: (s.horaCita || s.hora) || undefined,
        comentario: s.comentario || undefined
      }))
    };

    return this.facturacionService.syncBulk(payload).pipe(
      tap((res: any) => {
        this.cuentaId.set(res.cuentaId);
        
        // Mover items al estado persistido con sus respectivos DetalleId (Mapping V10.0 Pro)
        this.serviciosEnBackend.update(prev => [
          ...prev, 
          ...items.map(s => {
            const syncInfo = res.detalles?.find((d: any) => d.servicioId === s.id);
            return {
              ...s,
              detalleId: syncInfo?.detalleId,
              precioBs: s.precioBs || s.PrecioBs,
              precioUsd: s.precioUsd || s.PrecioUsd
            };
          })
        ]);
        
        this.carritoLocal.set([]);
      }),
      catchError(err => {
        console.error('Error en sincronización masiva:', err);
        throw err;
      })
    );
  }

}
