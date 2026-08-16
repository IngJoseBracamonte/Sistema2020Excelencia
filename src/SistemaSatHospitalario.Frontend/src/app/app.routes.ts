import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { UserLayoutComponent } from './shared/layouts/user-layout/user-layout.component';

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
    {
        path: 'login',
        loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
    },
    {
        path: 'reset-password',
        canActivate: [authGuard],
        loadComponent: () => import('./features/auth/reset-password/reset-password.component').then(m => m.ResetPasswordComponent)
    },
    {
        path: '',
        component: UserLayoutComponent,
        canActivate: [authGuard],
        children: [
            { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
            
            // --- Módulo Inventario v2 ---
            { path: 'inventario/stock', loadComponent: () => import('./features/inventario/stock/stock-multisede.component').then(m => m.StockMultisedeComponent) },
            { path: 'inventario/solicitudes', loadComponent: () => import('./features/admin/inventory/pedidos-inter-sede.component').then(m => m.PedidosInterSedeComponent) },
            { path: 'inventario/reposicion', loadComponent: () => import('./features/inventario/reposicion-inventario.component').then(m => m.ReposicionInventarioComponent) },
            { path: 'inventario/compras', loadComponent: () => import('./features/inventario/compras/compras.component').then(m => m.ComprasComponent) },
            { path: 'inventario/envios-recepciones', loadComponent: () => import('./features/inventario/envios-recepciones/envios-recepciones.component').then(m => m.EnviosRecepcionesComponent) },
            { path: 'inventario/pedidos', redirectTo: 'inventario/envios-recepciones', pathMatch: 'full' },
            { path: 'inventario/historiales', loadComponent: () => import('./features/inventario/historiales/historiales.component').then(m => m.HistorialesComponent) },
            { path: 'inventario/catalogo', loadComponent: () => import('./features/inventario/catalogo/catalogo.component').then(m => m.CatalogoComponent) },
            { path: 'inventario/descarte', loadComponent: () => import('./features/inventario/descarte/descarte.component').then(m => m.DescarteComponent) },
            { path: 'inventario/sedes-areas', loadComponent: () => import('./features/admin/inventory/sede-management.component').then(m => m.SedeManagementComponent) },
            { path: 'inventario/cuentas-por-pagar', loadComponent: () => import('./features/admin/inventory/cuentas-por-pagar.component').then(m => m.CuentasPorPagarComponent) },

            // Rutas administrativas / clínicas
            { path: 'rx-orders', loadComponent: () => import('./features/rx-orders/rx-orders.component').then(m => m.RxOrdersComponent) },
            { path: 'tomo-orders', loadComponent: () => import('./features/rx-orders/rx-orders.component').then(m => m.RxOrdersComponent) },
            { path: 'cajas', loadComponent: () => import('./features/admision/cajas/cajas.component').then(m => m.CajasComponent) },
            { path: 'facturacion', loadComponent: () => import('./features/admision/facturacion/facturacion.component').then(m => m.FacturacionComponent) },
            { path: 'cierre-cuenta/:type', loadComponent: () => import('./features/admision/cierre-cuenta/cierre-cuenta.component').then(m => m.CierreCuentaComponent) },
            { path: 'admision/hospitalizacion', loadComponent: () => import('./features/admision/hospitalizacion/hospitalizacion.component').then(m => m.HospitalizacionComponent) },
            { path: 'processing-monitor', loadComponent: () => import('./features/admision/processing-monitor/processing-monitor.component').then(m => m.ProcessingMonitorComponent) },
            { path: 'cxc', loadComponent: () => import('./features/admision/receivables/receivables.component').then(m => m.ReceivablesComponent) },
            { path: 'expediente-facturacion', loadComponent: () => import('./features/admision/expediente/expediente-facturacion.component').then(m => m.ExpedienteFacturacionComponent) },
            { path: 'control-citas', loadComponent: () => import('./features/admision/expediente/control-citas.component').then(m => m.ControlCitasComponent) },
            { path: 'expedientes', loadComponent: () => import('./features/admision/patient-history/patient-history.component').then(m => m.PatientHistoryComponent) },
            { path: 'catalog', loadComponent: () => import('./features/admin/catalog/catalog-management.component').then(m => m.CatalogManagementComponent) },
            { path: 'medicos', loadComponent: () => import('./features/admin/medicos/medico-management.component').then(m => m.MedicoManagementComponent) },
            
            // Alias/compatibilidad legacy si alguna referencia antigua navega ahí
            { path: 'admin/catalog', redirectTo: 'catalog', pathMatch: 'full' },
            { path: 'admision/facturacion', redirectTo: 'facturacion', pathMatch: 'full' },
            { path: 'admin/inventory', redirectTo: 'inventario/compras', pathMatch: 'full' },
            { path: 'admin/inventory/pedidos', redirectTo: 'inventario/pedidos', pathMatch: 'full' },
            { path: 'admin/inventory/compras', redirectTo: 'inventario/compras', pathMatch: 'full' },
            { path: 'admin/inventory/sedes-management', redirectTo: 'inventario/sedes-areas', pathMatch: 'full' },

            { path: 'admin/analytics', loadComponent: () => import('./features/admin/analytics/admin-analytics.component').then(m => m.AdminAnalyticsComponent) },
            { path: 'admin/reportes/honorarios', loadComponent: () => import('./features/admin/medicos/components/honoraria-report/honoraria-report.component').then(m => m.HonorariaReportComponent) },
            { path: 'admin/reportes/calculo-honorarios', loadComponent: () => import('./features/admin/honorariums/admin-honorariums.component').then(m => m.AdminHonorariumsComponent) },
            { path: 'admin/honorarios/asignaciones', loadComponent: () => import('./features/admin/honorariums/honorario-asignaciones.component').then(m => m.HonorarioAsignacionesComponent) },
            { path: 'admin/honorarios/config', loadComponent: () => import('./features/admin/honorariums/honorario-config.component').then(m => m.HonorarioConfigComponent) },
            { path: 'admin/audit/precios', loadComponent: () => import('./features/admin/audit/price-audit.component').then(m => m.PriceAuditComponent) },
            { path: 'admin/audit/cuentas', loadComponent: () => import('./features/admision/auditing/auditing.component').then(m => m.AuditingComponent) },
            { path: 'admin/audit/modificar-cuenta', loadComponent: () => import('./features/admin/account-modification/account-modification.component').then(m => m.AccountModificationComponent) },
            { path: 'admin/imaging-monitor', loadComponent: () => import('./features/admin/imaging-monitor/imaging-monitor.component').then(m => m.ImagingMonitorComponent) },
            { path: 'admin/reports/unprocessed-imaging', loadComponent: () => import('./features/admin/imaging-monitor/unprocessed-imaging-reports.component').then(m => m.UnprocessedImagingReportsComponent) },
            { path: 'especialidades', loadComponent: () => import('./features/admin/especialidades/especialidad-management.component').then(m => m.EspecialidadManagementComponent) },
            { path: 'admin/reset-requests', loadComponent: () => import('./features/admin/reset-requests/password-reset-requests.component').then(m => m.PasswordResetRequestsComponent) },
            { path: 'settings', loadComponent: () => import('./features/admin/settings/system-settings.component').then(m => m.SystemSettingsComponent) },
            { path: 'tickets', loadComponent: () => import('./features/admin/tickets/tickets.component').then(m => m.AdminTicketsComponent) },
            { path: 'seguros', loadComponent: () => import('./features/seguros/seguros-dashboard.component').then(m => m.SegurosDashboardComponent) },
            { path: 'admin/seguros/gerencia', loadComponent: () => import('./features/admin/seguros-gerencia/management-insurance-dashboard.component').then(m => m.ManagementInsuranceDashboardComponent) },
            { path: 'github-test', loadComponent: () => import('./features/github-test/github-test.component').then(m => m.GithubTestComponent) },
            { path: 'enfermeria', loadComponent: () => import('./features/enfermeria/enfermeria.component').then(m => m.EnfermeriaComponent) },
            { path: 'pabellon/gestion', loadComponent: () => import('./features/admision/pabellon-gestion/pabellon-gestion.component').then(m => m.PabellonGestionComponent) },

            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    },
    {
        path: 'error/:code',
        loadComponent: () => import('./shared/components/system-error/system-error.component').then(m => m.SystemErrorComponent)
    },
    { path: '**', redirectTo: 'error/404' }
];
