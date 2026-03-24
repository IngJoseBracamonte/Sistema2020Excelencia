import { HttpInterceptorFn } from '@angular/common/http';
import { tap } from 'rxjs/operators';

export const loggingInterceptor: HttpInterceptorFn = (req, next) => {
    console.log(`[HTTP DEBUG] Sending request to: ${req.url}`);
    console.log(`[HTTP DEBUG] Headers:`, req.headers.keys());
    console.log(`[HTTP DEBUG] Auth Header:`, req.headers.get('Authorization'));
    
    return next(req).pipe(
        tap({
            next: (event) => console.log(`[HTTP DEBUG] Response from ${req.url}:`, event),
            error: (err) => console.error(`[HTTP DEBUG] Error from ${req.url}:`, err)
        })
    );
};
