import { HttpInterceptorFn, HttpErrorResponse } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { catchError } from 'rxjs/operators';
import { throwError } from 'rxjs';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router);

    return next(req).pipe(
        catchError((err: HttpErrorResponse) => {
            // Redirección profesional a pantalla de error en español
            if (err.status === 401 || err.status === 403 || err.status === 404 || err.status >= 500 || err.status === 0) {
                router.navigate(['/error', err.status || '0']);
            }

            return throwError(() => err);
        })
    );
};
