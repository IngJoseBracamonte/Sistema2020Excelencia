import { ApplicationConfig, provideZoneChangeDetection, isDevMode, ErrorHandler, importProvidersFrom, LOCALE_ID } from '@angular/core';
import { provideRouter } from '@angular/router';
import { provideHttpClient, withInterceptors } from '@angular/common/http';
import { registerLocaleData } from '@angular/common';
import localeEs from '@angular/common/locales/es';

import { routes } from './app.routes';
import { provideServiceWorker } from '@angular/service-worker';
import { jwtInterceptor } from './core/interceptors/jwt.interceptor';
import { loggingInterceptor } from './core/interceptors/logging.interceptor';
import { TelemetryService } from './core/services/telemetry.service';
import { GlobalErrorHandler } from './core/errors/global-error-handler';
import { 
  LucideAngularModule, 
  Stethoscope, Activity, DollarSign, Star,
  LayoutDashboard, Files, Box, ClipboardList, Settings, Users, LogOut, FileText, Bookmark, ChevronDown, ShieldCheck, BarChart3, Github
} from 'lucide-angular';

registerLocaleData(localeEs);

export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }),
    provideRouter(routes),
    provideHttpClient(withInterceptors([jwtInterceptor, loggingInterceptor])),
    provideServiceWorker('ngsw-worker.js', {
      enabled: !isDevMode(),
      registrationStrategy: 'registerWhenStable:30000'
    }),
    TelemetryService,
    { provide: ErrorHandler, useClass: GlobalErrorHandler },
    { provide: LOCALE_ID, useValue: 'es' },
    importProvidersFrom(LucideAngularModule.pick({
      Stethoscope, Activity, DollarSign, Star,
      LayoutDashboard, Files, Box, ClipboardList, Settings, Users, LogOut, FileText, Bookmark, ChevronDown, ShieldCheck, BarChart3, Github
    }))
  ]
};
