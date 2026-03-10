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
            { path: '', redirectTo: 'dashboard', pathMatch: 'full' }
        ]
    },
    { path: '**', redirectTo: 'dashboard' }
];
