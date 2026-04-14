import { Injectable, inject, signal, effect } from '@angular/core';
import { AuthService, AuthResponse } from './auth.service';
import { StorageService } from './storage.service';

@Injectable({
  providedIn: 'root'
})
export class PermissionService {
  private authService = inject(AuthService);
  private storageService = inject(StorageService);

  // Load permissions from storage initialy, then sync with auth
  private permissionsSignal = signal<string[]>(this.getPermissionsFromStorage());

  constructor() {
    // Keep in sync with user signal using effect
    effect(() => {
      const user = this.authService.currentUser();
      if (user) {
        const permissions = user.permissions || [];
        this.permissionsSignal.set(permissions);
        this.storageService.savePermissions(permissions);
      } else {
        this.permissionsSignal.set([]);
        this.storageService.clearPermissions();
      }
    });
  }

  public can(permission: string): boolean {
    const perms = this.permissionsSignal();
    return perms.includes(permission) || this.authService.isAdministrador();
  }

  public any(permissions: string[]): boolean {
    return permissions.some(p => this.can(p));
  }

  private getPermissionsFromStorage(): string[] {
    const data = localStorage.getItem('user_permissions');
    return data ? JSON.parse(data) : [];
  }
}
