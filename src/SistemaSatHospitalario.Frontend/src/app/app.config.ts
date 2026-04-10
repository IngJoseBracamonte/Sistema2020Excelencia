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
  CreditCard, RefreshCcw, Check, Plus, User, Calendar, Search, Package, 
  Clock, SearchX, Info, ChevronRight, Trash2, X, Lock, UserPlus, Phone, 
  Mail, Layout, ShieldAlert, CalendarCheck, Edit3,
  Stethoscope, Activity, DollarSign
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
      CreditCard, RefreshCcw, Check, Plus, User, Calendar, Search, Package, 
      Clock, SearchX, Info, ChevronRight, Trash2, X, Lock, UserPlus, Phone, 
      Mail, Layout, ShieldAlert, CalendarCheck, Edit3,
      Stethoscope, Activity, DollarSign
    }))
  ]
};
