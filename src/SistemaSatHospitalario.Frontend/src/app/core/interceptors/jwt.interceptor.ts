import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
    const authService = inject(AuthService);
    const token = authService.getToken();

    if (token) {
        // [DEBUG 401 FIX] Verificando presencia de token en interceptor
        console.log(`[JWT DEBUG] Adjuntando token a la solicitud: ${req.url}`);
        req = req.clone({
            setHeaders: {
                Authorization: `Bearer ${token}`
            }
        });
    } else {
        console.warn(`[JWT DEBUG] No se encontró token para la solicitud: ${req.url}`);
    }

    return next(req).pipe(
        catchError((error: HttpErrorResponse) => {
            if (error.status === 401) {
                authService.logout();
            }
            return throwError(() => error);
        })
    );
};
