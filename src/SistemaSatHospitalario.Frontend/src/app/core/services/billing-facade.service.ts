import { Injectable, inject, signal, computed } from '@angular/core';
import { FacturacionService, CargarServicioACuentaRequest, DetallePagoDto, SyncCarritoMasivoRequest } from './facturacion.service';
import { CatalogItem } from '../models/priced-item.model';
import { CurrencyIds } from '../models/currency.model';
import { SpecialtyService } from './specialty.service';
import { AppointmentsService, Doctor } from './appointments.service';
import { AuthService } from './auth.service';
import { CatalogService } from './catalog.service';
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
  private catalogService = inject(CatalogService);

  // --- Estado Requerido (Signals) ---
  public carritoLocal = signal<any[]>([]);
  public serviciosEnBackend = signal<any[]>([]);
  public cuentaId = signal<string | null>(null);
  public pagos = signal<DetallePagoDto[]>([]);
  public tasaCambioDia = signal<number>(0);
  public currentSupervisorKey = signal<string | null>(null); // V1.0 Security Matrix

  // --- Búsqueda y Catálogo ---
  public searchTermServicio = signal<string>('');
  public selectedEspecialidad = signal<string | null>(null);
  public selectedMedicoId = signal<string | null>(null);
  public servicesCatalog = signal<CatalogItem[]>([]);
  public medicos = signal<any[]>([]);
  public especialidades = signal<string[]>([]);
  public catalogMetodosPago = signal<any[]>([]);

  // --- Selectores (Computed Signals) ---
  public serviciosCargados = computed(() => {
    const mapItem = (s: any, i: number, isBackend: boolean) => {
      if (!s) return s;
      const proto = Object.getPrototypeOf(s) || Object.prototype;
      const copy = Object.create(proto);
      return Object.assign(copy, s, { _index: i, _isBackend: isBackend });
    };
    return [
      ...this.serviciosEnBackend().map((s, i) => mapItem(s, i, true)),
      ...this.carritoLocal().map((s, i) => mapItem(s, i, false))
    ];
  });
  
  public totalCargadoUSD = computed(() => {
    return this.serviciosCargados().reduce((acc: number, curr: any) => {
      // Normalización en tiempo real (Pachón Pro V11.2)
      const precioUsd = curr.precioUsd ?? curr.PrecioUsd;
      const normalized = precioUsd ?? (curr.esLegacy ? (curr.precio / this.tasaCambioDia()) : 0);
      return acc + normalized;
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
    const cleanEsp = esp.trim().toUpperCase();
    return cleanEsp.length > 5 ? cleanEsp.substring(0, cleanEsp.length - 5) : cleanEsp;
  }

  public saldoPendienteUSD = computed(() => 
    Math.max(0, this.totalCargadoUSD() - this.totalFacturadoUSD())
  );

  constructor() {
    // Sincronizar tasa de cambio global
    this.settingsService.tasa$.subscribe(tasa => this.tasaCambioDia.set(tasa));
    this.loadPaymentCatalog();
  }

  public loadPaymentCatalog() {
    this.catalogService.getPaymentMethods().subscribe({
      next: (res) => {
        const mapped = res.map(x => ({
          id: x.id,
          name: x.nombre || x.name,
          value: x.valor || x.value,
          grupoMoneda: x.grupoMoneda,
          isUSD: x.grupoMoneda === CurrencyIds.USD || x.isUSD,
          isVuelto: x.esVuelto || x.isVuelto,
          orden: x.orden,
          activo: x.activo
        }));
        this.catalogMetodosPago.set(mapped.filter(x => !x.isVuelto));
      },
      error: (err) => {
        console.error('[BillingFacade] Error al cargar métodos de pago:', err?.status, err?.message);
      }
    });
  }

  /**
   * Recarga el catálogo de métodos de pago si está vacío.
   * Útil para componentes que necesitan asegurar que los métodos estén disponibles.
   */
  public reloadPaymentCatalogIfEmpty() {
    if (this.catalogMetodosPago().length === 0) {
      this.loadPaymentCatalog();
    }
  }

  public isMethodBs(methodName: string): boolean {
    const method = this.catalogMetodosPago().find(m => m.value === methodName || m.name === methodName);
    if (method) return method.grupoMoneda === CurrencyIds.VES;
    
    // Fallback logic for legacy strings
    const m = (methodName || '').toLowerCase();
    return m.includes('bs') || m.includes('móvil') || m.includes('punto');
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
      const item = this.carritoLocal()[index];
      // Si es una consulta local con reserva, liberarla en el backend
      if (item.medicoId && item.horaCita) {
        this.facturacionService.liberarTurno(item.medicoId, item.horaCita).subscribe();
      }
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
    this.currentSupervisorKey.set(null);
  }

  public loadOpenAccountForPatient(pacienteId: string, tipoIngreso: string): Observable<any> {
    return this.facturacionService.getOpenAccount(pacienteId, tipoIngreso).pipe(
      tap((cuenta: any) => {
        if (cuenta) {
          this.cuentaId.set(cuenta.id);
          const mappedItems = (cuenta.detalles || []).map((d: any) => {
            return new CatalogItem({
              id: d.servicioId,
              codigo: '',
              descripcion: d.descripcion,
              precio: d.precio,
              precioUsd: d.precio,
              precioBs: d.precio * this.tasaCambioDia(),
              honorarioUsd: d.honorario,
              honorarioBase: d.honorario,
              tipo: d.tipoServicio,
              medicoId: d.medicoResponsableId || undefined,
              medicoNombre: d.medicoNombre || undefined,
              detalleId: d.id,
              hora: d.fechaCarga,
              esLegacy: false
            });
          });
          this.serviciosEnBackend.set(mappedItems);
        } else {
          this.cuentaId.set(null);
          this.serviciosEnBackend.set([]);
        }
      }),
      catchError(err => {
        console.error('[BillingFacade] Error loading open account:', err);
        this.cuentaId.set(null);
        this.serviciosEnBackend.set([]);
        throw err;
      })
    );
  }

  public resetCart() {
    this.clearAll();
    this.searchTermServicio.set('');
    this.selectedEspecialidad.set(null);
    this.selectedMedicoId.set(null);
  }

   /**
   * Sincroniza el carrito local con el backend mediante una única transacción atómica (V11.1 Guid)
   */
  public syncCartWithBackend(pacienteId: string, tipoIngreso: string, usuarioCarga?: string, convenioId?: number | null, idPacienteLegacy?: number): Observable<any> {
    const items: any[] = [...this.carritoLocal()];
    if (items.length === 0) return of(null);

    const user = usuarioCarga || this.authService.currentUser()?.username || '';

    const payload: SyncCarritoMasivoRequest = {
      pacienteId,
      cuentaId: this.cuentaId() || undefined,
      idPacienteLegacy,
      tipoIngreso,
      usuarioCarga: user,
      convenioId: convenioId || undefined,
      supervisorKey: this.currentSupervisorKey() || undefined,
      items: items.map(s => {
        // Motor de Normalización USD-First (V11.2)
        // Si el item viene del legado y solo tiene precio en Bs, se divide por la tasa
        const precioBaseUsd = s.precioUsd || s.PrecioUsd;
        const normalizedPrice = precioBaseUsd || (s.esLegacy ? (s.precio / this.tasaCambioDia()) : s.precio);

        return {
          servicioId: s.id,
          descripcion: s.descripcion,
          precio: normalizedPrice,
          honorario: s.honorarioUsd || s.HonorarioUsd || 0,
          cantidad: 1,
          tipoServicio: s.tipo,
          medicoId: s.medicoId || undefined,
          horaCita: (s.horaCita || s.hora) || undefined,
          comentario: s.comentario || undefined
        };
      })
    };
    
    // Generación de Clave de Idempotencia (V12.0 Robustness - Safe for HTTP non-secure contexts)
    const idempotencyKey = (typeof crypto !== 'undefined' && crypto.randomUUID)
      ? crypto.randomUUID()
      : 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
          const r = Math.random() * 16 | 0;
          return (c === 'x' ? r : (r & 0x3 | 0x8)).toString(16);
        });

    return this.facturacionService.syncBulk(payload, idempotencyKey).pipe(
      tap((res: any) => {
        this.cuentaId.set(res.cuentaId);
        
        // Mover items al estado persistido con sus respectivos DetalleId (Mapping V10.0 Pro)
        this.serviciosEnBackend.update(prev => [
          ...prev, 
          ...items.map(s => {
            const syncInfo = res.detalles?.find((d: any) => d.servicioId === s.id);
            return new CatalogItem({
              ...s,
              detalleId: syncInfo?.detalleId,
              precioBs: s.precioBs || s.PrecioBs,
              precioUsd: s.precioUsd || s.PrecioUsd
            });
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
