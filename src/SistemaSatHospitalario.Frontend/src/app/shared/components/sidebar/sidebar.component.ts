import { Component, inject, signal, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { PermissionService } from '../../../core/services/permission.service';
import {
    LucideAngularModule,
    LayoutDashboard,
    Files,
    Box,
    Users,
    LogOut,
    Settings,
    ClipboardList,
    CreditCard,
    FileText,
    Package,
    Stethoscope,
    Activity,
    Bookmark,
    ChevronDown,
    ShieldCheck,
    BarChart3
} from 'lucide-angular';

@Component({
    selector: 'app-sidebar',
    standalone: true,
    imports: [CommonModule, RouterLink, RouterLinkActive, LucideAngularModule],
    templateUrl: './sidebar.component.html'
})
export class SidebarComponent implements OnInit {
    auth = inject(AuthService);
    public permissionService = inject(PermissionService);
    private router = inject(Router);

    public dropdownsOpen = signal({
        caja: false,
        reportes: false,
        medica: false,
        settings: false
    });

    ngOnInit() {
        const url = this.router.url;
        if (url.includes('/cajas') || url.includes('/catalog')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), caja: true });
        }
        if (url.includes('/admin/audit') || url.includes('/cxc') || url.includes('/rx-orders') || url.includes('/expedientes') || url.includes('/expediente-facturacion') || url.includes('/control-citas')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), reportes: true });
        }
        if (url.includes('/medicos') || url.includes('/especialidades')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), medica: true });
        }
        if (url.includes('/settings')) {
            this.dropdownsOpen.set({ ...this.dropdownsOpen(), settings: true });
        }
    }

    readonly icons = {
        Dashboard: LayoutDashboard,
        Billing: Files,
        Cajas: Box,
        Orders: ClipboardList,
        Settings: Settings,
        User: Users,
        Logout: LogOut,
        AR: CreditCard,
        History: FileText,
        Catalog: Package,
        Doctor: Stethoscope,
        RX: Activity,
        Category: Bookmark,
        ChevronDown: ChevronDown,
        Audit: ShieldCheck,
        Reportes: BarChart3
    };

    toggleDropdown(key: 'caja' | 'medica' | 'settings' | 'reportes') {
        this.dropdownsOpen.update(prev => ({
            ...prev,
            [key]: !prev[key as keyof typeof prev]
        }));
    }

    isAdmin(): boolean {
        return this.auth.isAdministrador();
    }
}
