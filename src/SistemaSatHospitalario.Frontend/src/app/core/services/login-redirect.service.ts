import { Injectable, inject } from '@angular/core';
import { AuthService } from './auth.service';

export interface RedirectStrategy {
  applies(requirePasswordReset: boolean): boolean;
  getRoute(): string[];
}

@Injectable({
  providedIn: 'root'
})
export class LoginRedirectService {
  private authService = inject(AuthService);

  private readonly strategies: RedirectStrategy[] = [
    {
      applies: (requirePasswordReset) => requirePasswordReset,
      getRoute: () => ['/reset-password']
    },
    {
      applies: () => this.authService.isAdmin(),
      getRoute: () => ['/dashboard']
    },
    {
      applies: () => this.authService.isRxAssistant(),
      getRoute: () => ['/rx-orders']
    },
    {
      applies: () => this.authService.isEmergencyAssistant(),
      getRoute: () => ['/enfermeria']
    },
    {
      applies: () => this.authService.isHospitalAssistant(),
      getRoute: () => ['/cierre-cuenta/Hospitalizacion']
    },
    {
      applies: () => true, // Fallback/Default
      getRoute: () => ['/dashboard']
    }
  ];

  public redirectRoute(requirePasswordReset: boolean): string[] {
    const matchedStrategy = this.strategies.find(s => s.applies(requirePasswordReset));
    return matchedStrategy ? matchedStrategy.getRoute() : ['/dashboard'];
  }
}
