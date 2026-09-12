import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);

    return next(req).pipe(
        catchError((err: HttpErrorResponse) => {
            // No redirigir en endpoints de autenticación ni en errores 401/404 manejados por componentes
            if (!req.url.includes('/Auth/') && (err.status === 502 || err.status === 503)) {
                router.navigate(['/error', err.status]);
            }

            return throwError(() => err);
        })
    );
};
