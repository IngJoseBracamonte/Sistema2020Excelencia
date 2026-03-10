import { Component, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FacturacionService, DetallePagoDto, CargarServicioACuentaRequest } from '../../../core/services/facturacion.service';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-facturacion',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './facturacion.component.html',
  styleUrl: './facturacion.component.css'
})
export class FacturacionComponent {
  private facturacionService = inject(FacturacionService);
  private authService = inject(AuthService);

  // Estados de Usuario y Rol
  public user = this.authService.currentUser;
  public isAdmin = computed(() => this.user()?.role === 'Administrador');
  public isParticularAssistant = computed(() => this.user()?.role === 'Asistente Particular');
  public isInsuranceAssistant = computed(() => this.user()?.role === 'Asistente de Seguros');

  // Estados de Facturación
  public pacienteId = signal<string>('');
  public cuentaId = signal<string | null>(null);
  public tipoIngreso = signal<string>('Particular');
  public convenioId = signal<string | null>(null);
  public tasaCambioDia = signal<number>(45.50);

  // Catálogos (Mocks para la Demo, idealmente vendrían de servicios)
  public especialidades = ['Ginecologo', 'Pediatra', 'Traumatologo', 'Cardiologo'];
  public medicos = [
    { id: 'm1', nombre: 'Juan Pérez', especialidad: 'Traumatologo' },
    { id: 'm2', nombre: 'María García', especialidad: 'Ginecologo' },
    { id: 'm3', nombre: 'José Bracamonte', especialidad: 'Pediatra' }
  ];
  public serviciosCatalogo = [
    { id: 's1', descripcion: 'Consulta Médica', precio: 40, tipo: 'Medico' },
    { id: 's2', descripcion: 'Rayos X Torax', precio: 25, tipo: 'RX' },
    { id: 's3', descripcion: 'Perfil 20', precio: 30, tipo: 'Laboratorio' },
    { id: 's4', descripcion: 'Hospitalización Diaria', precio: 100, tipo: 'Insumo' }
  ];
  public convenios = [
    { id: 'c1', nombre: 'Caritas' },
    { id: 'c2', nombre: 'Seguros Caracas' }
  ];

  // Filtros UI
  public selectedEspecialidad = signal<string | null>(null);
  public medicosFiltrados = computed(() => 
    this.medicos.filter(m => m.especialidad === this.selectedEspecialidad())
  );
  public selectedMedicoId = signal<string | null>(null);
  public horaCita = signal<string>('08:00');

  // Carrito de Servicios (Servicios ya cargados en la cuenta)
  public serviciosCargados = signal<any[]>([]);

  // Array de Pagos
  public pagos = signal<DetallePagoDto[]>([]);

  // Feedback UI
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);

  // Nuevo Elemento Form (Pagos)
  public currentPago = {
    metodoPago: 'Punto de Venta Bs',
    referenciaBancaria: '',
    montoAbonadoMoneda: 0
  };

  public metodosDisponibles = ['Punto de Venta Bs', 'Pago Móvil', 'Efectivo Divisas', 'Zelle'];

  constructor() {
    // Ajustar tipo ingreso inicial según rol
    if (this.isParticularAssistant()) this.tipoIngreso.set('Particular');
    if (this.isInsuranceAssistant()) this.tipoIngreso.set('Seguro');
  }

  cargarServicio(servId: string) {
    const s = this.serviciosCatalogo.find(x => x.id === servId);
    if (!s || !this.pacienteId()) {
      this.errorMessage.set("Seleccione un paciente antes de cargar servicios.");
      return;
    }

    this.isLoading.set(true);
    const payload: CargarServicioACuentaRequest = {
      pacienteId: this.pacienteId(),
      tipoIngreso: this.tipoIngreso(),
      convenioId: this.convenioId() || undefined,
      servicioId: s.id,
      descripcion: s.descripcion,
      precio: s.precio,
      cantidad: 1,
      tipoServicio: s.tipo,
      usuarioCarga: this.user()?.username || 'admin',
      medicoId: s.tipo === 'Medico' ? this.selectedMedicoId() || undefined : undefined,
      horaCita: s.tipo === 'Medico' ? this.horaCita() : undefined
    };

    this.facturacionService.cargarServicio(payload).subscribe({
      next: (res) => {
        this.cuentaId.set(res.cuentaId);
        this.serviciosCargados.update(prev => [...prev, { ...s, hora: this.horaCita() }]);
        this.actionMessage.set("Servicio cargado exitosamente.");
        this.errorMessage.set(null);
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || "Error al cargar servicio.");
        this.isLoading.set(false);
      }
    });
  }

  agregarPago() {
    if (this.currentPago.montoAbonadoMoneda <= 0) return;

    let eqBase = 0;
    const bsMethods = ['Punto de Venta Bs', 'Pago Móvil', 'Transferencia Bs'];
    if (bsMethods.includes(this.currentPago.metodoPago)) {
      eqBase = this.currentPago.montoAbonadoMoneda / this.tasaCambioDia();
    } else {
      eqBase = this.currentPago.montoAbonadoMoneda;
    }

    const nuevoPago: DetallePagoDto = {
      metodoPago: this.currentPago.metodoPago,
      referenciaBancaria: this.currentPago.referenciaBancaria || 'NA',
      montoAbonadoMoneda: this.currentPago.montoAbonadoMoneda,
      equivalenteAbonadoBase: parseFloat(eqBase.toFixed(2))
    };

    this.pagos.update(ps => [...ps, nuevoPago]);
    this.currentPago = { metodoPago: 'Punto de Venta Bs', referenciaBancaria: '', montoAbonadoMoneda: 0 };
  }

  removerPago(index: number) {
    this.pagos.update(ps => ps.filter((_, i) => i !== index));
  }

  public totalCargado = computed(() => this.serviciosCargados().reduce((acc, curr) => acc + curr.precio, 0));
  public totalFacturadoBase = computed(() => this.pagos().reduce((acc, curr) => acc + curr.equivalenteAbonadoBase, 0));

  procesarCobro() {
    if (!this.cuentaId() || this.pagos().length === 0) {
      this.errorMessage.set("No hay una cuenta activa o pagos registrados.");
      return;
    }

    this.isLoading.set(true);
    this.facturacionService.registrarPagoMultidivisa({
      cuentaServicioId: this.cuentaId()!,
      cajeroUserId: this.user()?.username || 'admin',
      tasaCambioDia: this.tasaCambioDia(),
      pagosMultidivisa: this.pagos()
    }).subscribe({
      next: (res) => {
        this.actionMessage.set(`¡Facturación Exitosa! Recibo: ${res.reciboId}`);
        this.resetForm();
        this.isLoading.set(false);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.error || "Error al procesar cobro.");
        this.isLoading.set(false);
      }
    });
  }

  private resetForm() {
    this.pagos.set([]);
    this.serviciosCargados.set([]);
    this.cuentaId.set(null);
    this.pacienteId.set('');
  }
}
