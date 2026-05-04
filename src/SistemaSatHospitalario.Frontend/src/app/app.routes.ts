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
            { path: 'rx-orders', loadComponent: () => import('./features/rx-orders/rx-orders.component').then(m => m.RxOrdersComponent) },
            { path: 'tomo-orders', loadComponent: () => import('./features/rx-orders/rx-orders.component').then(m => m.RxOrdersComponent) },
            { path: 'cajas', loadComponent: () => import('./features/admision/cajas/cajas.component').then(m => m.CajasComponent) },
            { path: 'facturacion', loadComponent: () => import('./features/admision/facturacion/facturacion.component').then(m => m.FacturacionComponent) },
            { path: 'processing-monitor', loadComponent: () => import('./features/admision/processing-monitor/processing-monitor.component').then(m => m.ProcessingMonitorComponent) },
            { path: 'cxc', loadComponent: () => import('./features/admision/receivables/receivables.component').then(m => m.ReceivablesComponent) },
            { path: 'expediente-facturacion', loadComponent: () => import('./features/admision/expediente/expediente-facturacion.component').then(m => m.ExpedienteFacturacionComponent) },
            { path: 'control-citas', loadComponent: () => import('./features/admision/expediente/control-citas.component').then(m => m.ControlCitasComponent) },
            { path: 'expedientes', loadComponent: () => import('./features/admision/patient-history/patient-history.component').then(m => m.PatientHistoryComponent) },
            { path: 'catalog', loadComponent: () => import('./features/admin/catalog/catalog-management.component').then(m => m.CatalogManagementComponent) },
            { path: 'medicos', loadComponent: () => import('./features/admin/medicos/medico-management.component').then(m => m.MedicoManagementComponent) },
            { path: 'admin/analytics', loadComponent: () => import('./features/admin/analytics/admin-analytics.component').then(m => m.AdminAnalyticsComponent) },
            { path: 'admin/reportes/honorarios', loadComponent: () => import('./features/admin/medicos/components/honoraria-report/honoraria-report.component').then(m => m.HonorariaReportComponent) },
            { path: 'admin/reportes/calculo-honorarios', loadComponent: () => import('./features/admin/honorariums/admin-honorariums.component').then(m => m.AdminHonorariumsComponent) },
            { path: 'admin/honorarios/asignaciones', loadComponent: () => import('./features/admin/honorariums/honorario-asignaciones.component').then(m => m.HonorarioAsignacionesComponent) },
            { path: 'admin/honorarios/config', loadComponent: () => import('./features/admin/honorariums/honorario-config.component').then(m => m.HonorarioConfigComponent) },
            { path: 'admin/audit/precios', loadComponent: () => import('./features/admin/audit/price-audit.component').then(m => m.PriceAuditComponent) },
            { path: 'admin/audit/cuentas', loadComponent: () => import('./features/admision/auditing/auditing.component').then(m => m.AuditingComponent) },
            { path: 'especialidades', loadComponent: () => import('./features/admin/especialidades/especialidad-management.component').then(m => m.EspecialidadManagementComponent) },
            { path: 'admin/reset-requests', loadComponent: () => import('./features/admin/reset-requests/password-reset-requests.component').then(m => m.PasswordResetRequestsComponent) },
            { path: 'settings', loadComponent: () => import('./features/admin/settings/system-settings.component').then(m => m.SystemSettingsComponent) },
            { path: 'tickets', loadComponent: () => import('./features/admin/tickets/tickets.component').then(m => m.AdminTicketsComponent) },
            { path: 'admin/health', loadComponent: () => import('./features/admin/sanity-check/sanity-check.component').then(m => m.SanityCheckComponent) },
            { path: 'seguros', loadComponent: () => import('./features/seguros/seguros-dashboard.component').then(m => m.SegurosDashboardComponent) },
            { path: 'admin/seguros/gerencia', loadComponent: () => import('./features/admin/seguros-gerencia/management-insurance-dashboard.component').then(m => m.ManagementInsuranceDashboardComponent) },
            { path: 'github-test', loadComponent: () => import('./features/github-test/github-test.component').then(m => m.GithubTestComponent) },

            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    },
    {
        path: 'error/:code',
        loadComponent: () => import('./shared/components/system-error/system-error.component').then(m => m.SystemErrorComponent)
    },
    { path: '**', redirectTo: 'error/404' }
];
