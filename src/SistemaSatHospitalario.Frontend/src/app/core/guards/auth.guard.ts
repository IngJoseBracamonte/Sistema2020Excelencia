import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    const user = authService.currentUser();
    
    // Si requiere reset de password, forzar navegación a reset-password
    if (user?.requirePasswordReset) {
      return state.url === '/reset-password' ? true : router.createUrlTree(['/reset-password']);
    }

    // Si NO requiere reset, no permitir entrar a esa página
    if (state.url === '/reset-password') {
      return router.createUrlTree(['/dashboard']);
    }

    return true;
  }

  // Redirigir al inicio de sesión y pasar la ruta que querían acceder
  return router.createUrlTree(['/login'], { queryParams: { returnUrl: state.url } });
};

