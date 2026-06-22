import { Component, inject, signal, OnInit, computed, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { 
  LucideAngularModule, 
  DollarSign, 
  CreditCard, 
  RefreshCcw, 
  AlertCircle, 
  Check, 
  LayoutDashboard, 
  Users, 
  Lock,
  TrendingUp,
  TrendingDown,
  MoreVertical,
  Eye,
  Settings,
  HelpCircle,
  FileSpreadsheet
} from 'lucide-angular';
import { CajaService, ResumenCajaGlobalDto, CajaSummaryDto, DailyClosingReport } from '../../../core/services/caja.service';
import { AuthService } from '../../../core/services/auth.service';
import { CatalogService } from '../../../core/services/catalog.service';
import { SettingsService } from '../../../core/services/settings.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-cajas',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule, RouterLink, LucideAngularModule],
  templateUrl: './cajas.component.html',
  styleUrl: './cajas.component.css'
})
export class CajasComponent implements OnInit, OnDestroy {
  readonly icons = {
    DollarSign,
    CreditCard,
    RefreshCcw,
    AlertCircle,
    Check,
    LayoutDashboard,
    Users,
    Lock,
    TrendingUp,
    TrendingDown,
    MoreVertical,
    Eye,
    Settings,
    HelpCircle,
    FileSpreadsheet
  };

  private cajaService = inject(CajaService);
  public authService = inject(AuthService);
  private catalogService = inject(CatalogService);
  private settingsService = inject(SettingsService);
  private tasaSubscription?: Subscription;

  // Estados Reactivos
  public isLoading = signal<boolean>(false);
  public actionMessage = signal<string | null>(null);
  public errorMessage = signal<string | null>(null);
  
  // Data para el Admin
  public resumenCaja = signal<ResumenCajaGlobalDto | null>(null);
  public historialAdmin = signal<CajaSummaryDto | null>(null);
  
  // Data para el Asistente/Cajero
  public personalReport = signal<DailyClosingReport | null>(null);
  public isMiCajaAbierta = signal<boolean>(false);
  
  public isAdministrador = this.authService.isAdmin;
  public tasaCambio = signal<number>(1);

  // Estados de Declaración (Fase 1)
  public paymentMethods = signal<any[]>([]);
  public filterState = signal<string>('TODAS');
  public selectedCajaId = signal<string | null>(null);

  // Cierre de caja en progreso (modal/auditoría)
  public selectedCajaParaAuditar = signal<any | null>(null);

  // Cómputo de la tabla de cierre (En caliente)
  public tableRows = computed(() => {
    const methods = this.paymentMethods();
    const report = this.personalReport();
    const tasa = this.tasaCambio();

    return methods.map(m => {
      // Encontrar el esperado en el reporte personal
      const esperadoIngreso = report?.desgloseMetodos?.find(d => d.metodo === m.value)?.montoMonedaOriginal || 0;
      
      // Encontrar el vuelto asociado
      let vueltoMetodo = '';
      if (m.value === 'Dolar Efectivo') vueltoMetodo = 'Vuelto Efectivo USD';
      else if (m.value === 'Efectivo BS') vueltoMetodo = 'Vuelto Efectivo BS';
      else if (m.value === 'Pago Movil') vueltoMetodo = 'Vuelto Pago Movil';
      
      const esperadoVuelto = report?.desgloseMetodos?.find(d => d.metodo === vueltoMetodo)?.montoMonedaOriginal || 0;
      const esperadoVueltoAbs = Math.abs(esperadoVuelto);

      const totalEsperado = esperadoIngreso - esperadoVueltoAbs;

      return {
        value: m.value,
        name: m.name,
        isUSD: m.isUSD,
        montoIngreso: esperadoIngreso,
        montoVueltos: esperadoVueltoAbs,
        totalDeclarado: totalEsperado,
        esperadoIngreso,
        esperadoVuelto: esperadoVueltoAbs,
        totalEsperado,
        diferencia: 0
      };
    });
  });

  // Filtro de cierres reactivo
  public cierresFiltrados = computed(() => {
    const list = this.historialAdmin()?.cierres || [];
    const filter = this.filterState();
    if (filter === 'TODAS') return list;
    if (filter === 'ABIERTAS') return list.filter(c => c.estado === 'Abierta');
    if (filter === 'PENDIENTES') return list.filter(c => c.estado === 'CerradaPorAsistente');
    if (filter === 'CONSOLIDADAS') return list.filter(c => c.estado === 'Cerrada');
    return list;
  });

  public totalIngresadoUSD = computed(() => {
    const rows = this.tableRows();
    const tasa = this.tasaCambio();
    return rows.reduce((acc, row) => {
      const val = row.isUSD ? row.totalDeclarado : (tasa > 0 ? row.totalDeclarado / tasa : 0);
      return acc + val;
    }, 0);
  });

  public totalCobradoUSD = computed(() => {
    const rows = this.tableRows();
    const tasa = this.tasaCambio();
    return rows.reduce((acc, row) => {
      const val = row.isUSD ? row.totalEsperado : (tasa > 0 ? row.totalEsperado / tasa : 0);
      return acc + val;
    }, 0);
  });

  public diferenciaUSD = computed(() => {
    return this.totalIngresadoUSD() - this.totalCobradoUSD();
  });

  ngOnInit() {
    this.checkStatus();
    this.loadCatalogMethods();
    
    // Escuchar la tasa de cambio en vivo
    this.settingsService.refreshTasa();
    this.tasaSubscription = this.settingsService.tasa$.subscribe(tasa => {
      if (tasa > 0) this.tasaCambio.set(tasa);
    });

    if (this.isAdministrador()) {
      this.refrescarAdmin();
    }
  }

  ngOnDestroy() {
    this.tasaSubscription?.unsubscribe();
  }

  loadCatalogMethods() {
    this.catalogService.getPaymentMethods().subscribe({
      next: (res) => {
        const methods = res.filter(x => !x.isVuelto);
        this.paymentMethods.set(methods);
      }
    });
  }

  checkStatus() {
    this.cajaService.getPersonalReport().subscribe({
      next: (res) => {
        this.personalReport.set(res);
        this.isMiCajaAbierta.set(res.isCajaAbierta);
      },
      error: (err) => console.log("Usuario sin actividad hoy todavía.")
    });
  }

  cerrarMiCaja() {
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    // Mapear los inputs al DTO esperado por el backend
    const payload = this.tableRows().map(row => ({
      metodoPago: row.value,
      montoIngreso: row.montoIngreso,
      montoVueltos: row.montoVueltos
    }));

    this.cajaService.cerrarCaja(payload).subscribe({
      next: (res: any) => {
        const summary = `Cierre Exitoso. Total Recaudado: $${res.totalIngresosUSD.toFixed(2)}. Usuario: ${res.usuario}`;
        this.actionMessage.set(summary);
        
        this.isMiCajaAbierta.set(false);
        this.isLoading.set(false);
        this.checkStatus();
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al cerrar su caja.');
        this.isLoading.set(false);
      }
    });
  }

  refrescarAdmin() {
    if (!this.isAdministrador()) return;
    this.cajaService.obtenerResumenDiario().subscribe(res => this.resumenCaja.set(res));
    this.cajaService.obtenerHistorial().subscribe(res => {
      this.historialAdmin.set(res);
    });
  }

  consolidarCajas() {
    if (!confirm('¿Está seguro de que desea realizar el cierre total consolidado de todas las cajas en conjunto?')) return;
    
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    this.cajaService.consolidarTodasLasCajas().subscribe({
      next: (res: any) => {
        this.actionMessage.set(`¡Cierre Total Consolidado Ejecutado! Total Recaudado: $${res.totalRecaudado.toFixed(2)} USD.`);
        this.refrescarAdmin();
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al consolidar las cajas.');
        this.isLoading.set(false);
      }
    });
  }

  forzarCierre(caja: any) {
    if (!confirm(`¿Está seguro de que desea FORZAR el cierre de la caja del usuario "${caja.usuario}"?`)) return;
    
    this.isLoading.set(true);
    this.errorMessage.set(null);
    this.actionMessage.set(null);

    this.cajaService.forzarCierreCaja(caja.id).subscribe({
      next: (res: any) => {
        this.actionMessage.set(`¡Cierre forzado exitosamente para la caja de ${caja.usuario}!`);
        this.refrescarAdmin();
        this.isLoading.set(false);
      },
      error: (err: any) => {
        this.errorMessage.set(err.error?.Error || err.error?.error || 'Error al forzar el cierre de la caja.');
        this.isLoading.set(false);
      }
    });
  }

  auditarCaja(caja: any) {
    if (!caja.declaracionCierreJson) {
      alert("Esta caja no cuenta con una declaración detallada (es un cierre del sistema anterior o incompleto).");
      return;
    }
    try {
      const desglose = JSON.parse(caja.declaracionCierreJson);
      this.selectedCajaParaAuditar.set({
        ...caja,
        desglose
      });
    } catch (e) {
      alert("Error al deserializar la declaración de cierre.");
    }
  }

  cerrarAuditoria() {
    this.selectedCajaParaAuditar.set(null);
  }

  exportarAuditoria(userId?: string) {
    this.isLoading.set(true);
    const dateStr = new Date().toISOString().split('T')[0];
    
    this.cajaService.exportExcelCashClosing(userId, dateStr, true).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        const label = userId || 'Global';
        a.download = `Auditoria_Caja_${label}_${dateStr}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set("Error al exportar Excel de auditoría");
      }
    });
  }

  exportarMiCaja() {
    const user = this.authService.currentUser();
    if (!user) return;
    
    this.isLoading.set(true);
    const dateStr = new Date().toISOString().split('T')[0];
    
    this.cajaService.exportExcelCashClosing(user.username, dateStr, false).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = `Cierre_Caja_${user.username}_${dateStr}.xlsx`;
        a.click();
        window.URL.revokeObjectURL(url);
        this.isLoading.set(false);
      },
      error: () => {
        this.isLoading.set(false);
        this.errorMessage.set("Error al exportar su reporte de caja");
      }
    });
  }

  toggleCajaDetalle(cajaId: string) {
    if (this.selectedCajaId() === cajaId) {
      this.selectedCajaId.set(null);
    } else {
      this.selectedCajaId.set(cajaId);
    }
  }

  obtenerDesglose(caja: any): any[] {
    if (!caja.declaracionCierreJson) return [];
    try {
      return JSON.parse(caja.declaracionCierreJson);
    } catch {
      return [];
    }
  }
}
