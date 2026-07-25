import { Component, Input, Output, EventEmitter, signal, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../../../environments/environment';
import { LucideAngularModule, AlertCircle, CheckCircle, AlertTriangle, X, LogOut, ArrowRight } from 'lucide-angular';

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

  // Modal Solvencia Interyector
  public showSolvenciaModal = signal<boolean>(false);
  public pendingAction = signal<'TRASLADO' | 'ALTA' | null>(null);

  // Modal Alta Médica
  public showAltaModal = signal<boolean>(false);
  public tipoAltaSeleccionada = signal<TipoAltaEnum>(TipoAltaEnum.Normal);
  public observacionesAlta = signal<string>('');
  public confirmadoEnfermeriaSinSolvencia = signal<boolean>(false);
  public isProcessingAlta = signal<boolean>(false);

  // Modalidad de Traslado
  public modoTraslado = signal<'CAMBIO_CAMA' | 'TRASLADO_AREA'>('CAMBIO_CAMA');
  public selectedCamaId = signal<string | null>(null);
  public areaDestino = signal<string>('HOSPITALIZACION');
  public cantidadHoras = signal<number>(24);
  public montoUsd = signal<number>(450);
  public cambiaMedico = signal<boolean>(false);
  public nuevoMedicoId = signal<string | null>(null);
  public observacionTraslado = signal<string>('');
  public isSavingTransfer = signal<boolean>(false);

  public onAreaDestinoChange(area: string): void {
    this.areaDestino.set(area);
    if (area === 'EMERGENCIA') this.montoUsd.set(300);
    else if (area === 'HOSPITALIZACION') this.montoUsd.set(450);
    else if (area === 'UCI') this.montoUsd.set(600);
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

      const payload = {
        cuentaId: cuentaId,
        areaDestino: this.areaDestino() || 'HOSPITALIZACION',
        camaDestinoId: this.selectedCamaId(),
        cantidadHoras: Number(this.cantidadHoras()) || 1,
        cambiaMedicoTratante: Boolean(this.cambiaMedico()),
        nuevoMedicoId: nuevoMedicoIdSaneado,
        observacion: (this.observacionTraslado() || '').trim(),
        montoACobrarUsd: Number(this.montoUsd()) || 0
      };

      this.http.post(`${environment.apiUrl}/api/Enfermeria/TrasladoArea`, payload).subscribe({
        next: () => {
          this.isSavingTransfer.set(false);
          const msg = `Traslado a ${this.areaDestino()} procesado con éxito ($${this.montoUsd()} USD).`;
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
