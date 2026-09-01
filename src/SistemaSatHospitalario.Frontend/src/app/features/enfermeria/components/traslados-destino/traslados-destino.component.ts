import { Component, Input, Output, EventEmitter, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { LucideAngularModule, AlertCircle, CheckCircle, AlertTriangle, X, LogOut, ArrowRight } from 'lucide-angular';
import { AreaClinica } from '../../../../core/services/multi-sede.service';

export enum TipoAltaEnum {
  Normal = 0,
  Voluntaria = 1,
  Defuncion = 2
}

@Component({
  selector: 'app-traslados-destino',
  standalone: true,
  imports: [CommonModule, FormsModule, LucideAngularModule],
  templateUrl: './traslados-destino.component.html'
})
export class TrasladosDestinoComponent {
  private http = inject(HttpClient);

  readonly icons = { AlertCircle, CheckCircle, AlertTriangle, X, LogOut, ArrowRight };

  @Input() selectedAccount: any = null;
  @Input() cuentaId: string | null = null;
  @Input() pacienteId: string | null = null;
  @Input() pacienteNombre: string = '';
  @Input() totalCuenta: number = 0;
  @Input() totalPagado: number = 0;
  @Input() camasDisponibles: any[] = [];
  @Input() medicos: any[] = [];

  // DB-Driven: Arreglo de áreas clínicas cargado dinámicamente desde la BD
  @Input() areasClinicas: AreaClinica[] = [];

  @Output() transferCompleted = new EventEmitter<string>();
  @Output() dischargeCompleted = new EventEmitter<string>();
  @Output() actionErrorMessage = new EventEmitter<string>();
  @Output() trasladoCompletado = new EventEmitter<string>();
  @Output() altaCompletada = new EventEmitter<string>();
  @Output() errorOcurrido = new EventEmitter<string>();

  public getEffectiveCuentaId(): string | null {
    return this.selectedAccount?.cuentaId || this.selectedAccount?.id || this.cuentaId;
  }

  public getEffectivePacienteId(): string | null {
    return this.selectedAccount?.pacienteId || this.pacienteId;
  }

  public getEffectivePacienteNombre(): string {
    return this.selectedAccount?.pacienteNombre || this.pacienteNombre || 'Paciente';
  }

  public getEffectiveTotalCuenta(): number {
    return this.selectedAccount?.total || this.totalCuenta || 0;
  }

  public getEffectiveTotalPagado(): number {
    return this.selectedAccount?.['totalPagado'] || this.totalPagado || 0;
  }

  public saldoPendiente = computed(() => {
    const total = this.getEffectiveTotalCuenta();
    const pagado = this.getEffectiveTotalPagado();
    return Math.max(0, total - pagado);
  });

  public esSolvente = computed(() => this.saldoPendiente() <= 0);

  // Modal Solvencia Interceptor
  public showSolvenciaModal = signal<boolean>(false);
  public pendingAction = signal<'TRASLADO' | 'ALTA' | null>(null);

  // Modal Alta Médica
  public showAltaModal = signal<boolean>(false);
  public tipoAltaSeleccionada = signal<TipoAltaEnum>(TipoAltaEnum.Normal);
  public observacionesAlta = signal<string>('');
  public confirmadoEnfermeriaSinSolvencia = signal<boolean>(false);
  public isProcessingAlta = signal<boolean>(false);

  // Modalidad de Traslado (DB-Driven)
  public modoTraslado = signal<'CAMBIO_CAMA' | 'TRASLADO_AREA'>('CAMBIO_CAMA');
  public selectedCamaId = signal<string | null>(null);
  public areaDestinoId = signal<string | null>(null);
  public cantidadHoras = signal<number>(24);

  // DB-Driven: Tarifas derivadas del Maestro de Catálogos con opción de edición manual
  public montoTrasladoInput = signal<number>(0);
  public honorarioTrasladoInput = signal<number>(0);
  public modoEdicionTarifa = signal<boolean>(false);

  public cambiaMedico = signal<boolean>(false);
  public nuevoMedicoId = signal<string | null>(null);
  public observacionTraslado = signal<string>('');
  public isSavingTransfer = signal<boolean>(false);

  public filteredCamas = computed(() => {
    const allCamas = this.camasDisponibles || [];
    if (this.modoTraslado() === 'CAMBIO_CAMA') {
      const currentArea = this.selectedAccount?.subAreaClinica || this.selectedAccount?.areaClinicaNombre;
      if (!currentArea) return allCamas;
      const normCurrent = currentArea.toUpperCase();
      const filtered = allCamas.filter((c: any) =>
        (c.areaClinicaNombre && c.areaClinicaNombre.toUpperCase().includes(normCurrent)) ||
        (c.nombre && c.nombre.toUpperCase().includes(normCurrent)) ||
        (c.subAreaClinica && c.subAreaClinica.toUpperCase().includes(normCurrent))
      );
      return filtered.length > 0 ? filtered : allCamas;
    } else {
      const areaId = this.areaDestinoId();
      if (!areaId) return allCamas;
      const areaObj = (this.areasClinicas || []).find(a => a.id === areaId);
      if (!areaObj) {
        const norm = areaId.toUpperCase();
        const filtered = allCamas.filter((c: any) =>
          (c.areaClinicaNombre && c.areaClinicaNombre.toUpperCase().includes(norm)) ||
          (c.nombre && c.nombre.toUpperCase().includes(norm)) ||
          (c.subAreaClinica && c.subAreaClinica.toUpperCase().includes(norm)) ||
          (c.codigo && c.codigo.toUpperCase().includes(norm))
        );
        return filtered.length > 0 ? filtered : allCamas;
      }
      const normArea = areaObj.nombre.toUpperCase();
      const filtered = allCamas.filter((c: any) =>
        c.areaClinicaId === areaId ||
        c.areaPadreId === areaId ||
        (c.areaClinicaNombre && c.areaClinicaNombre.toUpperCase().includes(normArea)) ||
        (c.nombre && c.nombre.toUpperCase().includes(normArea))
      );
      return filtered.length > 0 ? filtered : allCamas;
    }
  });

  /**
   * Asignación dinámica de tarifa derivada del ServicioClinico base vinculado al Área
   */
  public onAreaDestinoChange(areaId: string): void {
    this.areaDestinoId.set(areaId);
    this.selectedCamaId.set(null);

    const areaSeleccionada = (this.areasClinicas || []).find(a =>
      a.id === areaId ||
      a.nombre.toUpperCase().includes((areaId || '').toUpperCase()) ||
      a.codigo?.toUpperCase() === (areaId || '').toUpperCase()
    );
    if (!areaSeleccionada) return;

    // Si no está en modo de edición manual, asigna las tarifas oficiales del catálogo maestro
    if (!this.modoEdicionTarifa()) {
      this.montoTrasladoInput.set(areaSeleccionada.tarifaBasePrecioUsd ?? 0);
      this.honorarioTrasladoInput.set(areaSeleccionada.tarifaBaseHonorarioUsd ?? 0);
    }
  }

  public toggleModoEdicion(): void {
    this.modoEdicionTarifa.update(v => !v);
  }

  public iniciarAlta(tipo: TipoAltaEnum): void {
    this.tipoAltaSeleccionada.set(tipo);
    this.observacionesAlta.set('');
    this.confirmadoEnfermeriaSinSolvencia.set(false);

    if (!this.esSolvente()) {
      this.pendingAction.set('ALTA');
      this.showSolvenciaModal.set(true);
    } else {
      this.showAltaModal.set(true);
    }
  }

  public continuarAccionInsolvente(): void {
    this.confirmadoEnfermeriaSinSolvencia.set(true);
    this.showSolvenciaModal.set(false);

    if (this.pendingAction() === 'ALTA') {
      this.showAltaModal.set(true);
    } else if (this.pendingAction() === 'TRASLADO') {
      this.ejecutarTraslado();
    }
  }

  public cancelarAccionInsolvente(): void {
    this.showSolvenciaModal.set(false);
    this.pendingAction.set(null);
  }

  public procesarAlta(): void {
    const cuentaId = this.getEffectiveCuentaId();
    const pacienteId = this.getEffectivePacienteId();

    if (!cuentaId && !pacienteId) {
      const err = 'Debe seleccionar una cuenta de paciente activa.';
      this.actionErrorMessage.emit(err);
      this.errorOcurrido.emit(err);
      return;
    }

    this.isProcessingAlta.set(true);

    const payload = {
      pacienteId: pacienteId,
      admisionId: cuentaId,
      tipoAlta: this.tipoAltaSeleccionada(),
      observaciones: this.observacionesAlta(),
      confirmadoPorEnfermeriaSinSolvencia: this.confirmadoEnfermeriaSinSolvencia()
    };

    this.http.post(`${environment.apiUrl}/api/Enfermeria/Alta`, payload).subscribe({
      next: (res: any) => {
        this.isProcessingAlta.set(false);
        this.showAltaModal.set(false);
        const msg = res.mensaje || 'Alta médica registrada exitosamente.';
        this.dischargeCompleted.emit(msg);
        this.altaCompletada.emit(msg);
      },
      error: (err: any) => {
        this.isProcessingAlta.set(false);
        const rawBody = err.error;
        const errorMsg =
          (typeof rawBody === 'string' ? rawBody : null) ||
          rawBody?.error ||
          rawBody?.Error ||
          rawBody?.message ||
          rawBody?.Message ||
          err.message ||
          'Error al registrar el alta médica.';

        if (errorMsg.toLowerCase().includes('saldo pendiente') || errorMsg.toLowerCase().includes('solvencia')) {
          this.showAltaModal.set(false);
          this.pendingAction.set('ALTA');
          this.showSolvenciaModal.set(true);
        } else {
          this.actionErrorMessage.emit(errorMsg);
          this.errorOcurrido.emit(errorMsg);
        }
      }
    });
  }

  public solicitarTraslado(): void {
    if (!this.esSolvente() && !this.confirmadoEnfermeriaSinSolvencia()) {
      this.pendingAction.set('TRASLADO');
      this.showSolvenciaModal.set(true);
      return;
    }
    this.ejecutarTraslado();
  }

  private ejecutarTraslado(): void {
    const cuentaId = this.getEffectiveCuentaId();
    if (!cuentaId) {
      const err = 'Seleccione una cuenta activa.';
      this.actionErrorMessage.emit(err);
      this.errorOcurrido.emit(err);
      return;
    }

    this.isSavingTransfer.set(true);

    if (this.modoTraslado() === 'CAMBIO_CAMA') {
      const payload = {
        cuentaId: cuentaId,
        camaDestinoId: this.selectedCamaId()
      };

      this.http.post(`${environment.apiUrl}/api/Enfermeria/CambioCama`, payload).subscribe({
        next: () => {
          this.isSavingTransfer.set(false);
          const msg = 'Cambio de cama procesado exitosamente.';
          this.transferCompleted.emit(msg);
          this.trasladoCompletado.emit(msg);
        },
        error: (err) => {
          this.isSavingTransfer.set(false);
          const msg = err.error?.Error || err.message || 'Error al cambiar de cama.';
          this.actionErrorMessage.emit(msg);
          this.errorOcurrido.emit(msg);
        }
      });
    } else {
      const medicoIdRaw = this.nuevoMedicoId();
      const nuevoMedicoIdSaneado = (medicoIdRaw && medicoIdRaw.trim() !== '') ? medicoIdRaw.trim() : null;

      const areaId = this.areaDestinoId();
      const areaObj = (this.areasClinicas || []).find(a => a.id === areaId);

      const payload = {
        cuentaId: cuentaId,
        areaDestino: areaObj?.nombre || 'ÁREA DESTINO',
        camaDestinoId: this.selectedCamaId(),
        cantidadHoras: Number(this.cantidadHoras()) || 1,
        cambiaMedicoTratante: Boolean(this.cambiaMedico()),
        nuevoMedicoId: nuevoMedicoIdSaneado,
        observacion: (this.observacionTraslado() || '').trim(),
        montoACobrarUsd: Number(this.montoTrasladoInput()) || 0
      };

      this.http.post(`${environment.apiUrl}/api/Enfermeria/TrasladoArea`, payload).subscribe({
        next: () => {
          this.isSavingTransfer.set(false);
          const msg = `Traslado a ${areaObj?.nombre || 'Área'} procesado con éxito ($${this.montoTrasladoInput()} USD).`;
          this.transferCompleted.emit(msg);
          this.trasladoCompletado.emit(msg);
        },
        error: (err) => {
          this.isSavingTransfer.set(false);
          const rawBody = err.error;
          const msg =
            (typeof rawBody === 'string' ? rawBody : null) ||
            rawBody?.error ||
            rawBody?.Error ||
            rawBody?.message ||
            rawBody?.Message ||
            err.message ||
            'Error al trasladar de área.';
          this.actionErrorMessage.emit(msg);
          this.errorOcurrido.emit(msg);
        }
      });
    }
  }
}