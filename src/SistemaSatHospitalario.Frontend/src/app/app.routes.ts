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
        path: '',
        component: UserLayoutComponent,
        canActivate: [authGuard],
        children: [
            { path: 'dashboard', loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent) },
            { path: 'rx-orders', loadComponent: () => import('./features/rx-orders/rx-orders.component').then(m => m.RxOrdersComponent) },
            { path: 'cajas', loadComponent: () => import('./features/admision/cajas/cajas.component').then(m => m.CajasComponent) },
            { path: 'facturacion', loadComponent: () => import('./features/admision/facturacion/facturacion.component').then(m => m.FacturacionComponent) },
            { path: 'cxc', loadComponent: () => import('./features/admision/receivables/receivables.component').then(m => m.ReceivablesComponent) },
            { path: 'expedientes', loadComponent: () => import('./features/admision/patient-history/patient-history.component').then(m => m.PatientHistoryComponent) },
            { path: 'catalog', loadComponent: () => import('./features/admin/catalog/catalog-management.component').then(m => m.CatalogManagementComponent) },
            { path: 'medicos', loadComponent: () => import('./features/admin/medicos/medico-management.component').then(m => m.MedicoManagementComponent) },
            { path: 'especialidades', loadComponent: () => import('./features/admin/especialidades/especialidad-management.component').then(m => m.EspecialidadManagementComponent) },
            { path: 'settings', loadComponent: () => import('./features/admin/settings/system-settings.component').then(m => m.SystemSettingsComponent) },
            { path: 'tickets', loadComponent: () => import('./features/admin/tickets/tickets.component').then(m => m.AdminTicketsComponent) },
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    },
    { path: '**', redirectTo: 'dashboard' }
];
