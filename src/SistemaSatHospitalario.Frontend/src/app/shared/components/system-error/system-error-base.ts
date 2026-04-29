import { 
  Ghost, 
  ShieldAlert, 
  WifiOff, 
  AlertTriangle,
  LucideIconData 
} from 'lucide-angular';

export interface ErrorUIConfig {
  title: string;
  highlight: string;
  message: string;
  icon: any;
  glowClass: string;
  iconContainerClass: string;
  iconClass: string;
  badgeClass: string;
  gradientClass: string;
}

export abstract class BaseErrorComponent {
  // Estos iconos se pasan como referencias directas para evitar inyectarlos cada vez
  protected readonly errorIcons = {
    Ghost,
    ShieldAlert,
    WifiOff,
    AlertTriangle
  };

  protected readonly ERROR_CONFIGS: Record<string, ErrorUIConfig> = {
    '404': {
      title: 'Página',
      highlight: 'No Encontrada',
      message: 'El recurso que buscas parece haber sido movido o eliminado del servidor.',
      icon: Ghost,
      glowClass: 'bg-indigo-500/20',
      iconContainerClass: 'bg-indigo-500/10 border-indigo-500/20',
      iconClass: 'text-indigo-400',
      badgeClass: 'bg-indigo-600',
      gradientClass: 'bg-gradient-to-r from-indigo-400 to-blue-500'
    },
    '401': {
      title: 'Acceso',
      highlight: 'Restringido',
      message: 'No tienes los permisos necesarios o tu sesión ha expirado. Por favor, reidentifícate.',
      icon: ShieldAlert,
      glowClass: 'bg-amber-500/20',
      iconContainerClass: 'bg-amber-500/10 border-amber-500/20',
      iconClass: 'text-amber-400',
      badgeClass: 'bg-amber-600',
      gradientClass: 'bg-gradient-to-r from-amber-400 to-orange-500'
    },
    '403': {
      title: 'Acceso',
      highlight: 'Prohibido',
      message: 'Tu rango actual no permite acceder a este sector del sistema hospitalario.',
      icon: ShieldAlert,
      glowClass: 'bg-amber-500/20',
      iconContainerClass: 'bg-amber-500/10 border-amber-500/20',
      iconClass: 'text-amber-400',
      badgeClass: 'bg-amber-600',
      gradientClass: 'bg-gradient-to-r from-amber-400 to-orange-500'
    },
    '0': {
      title: 'Sin',
      highlight: 'Conexión',
      message: 'No logramos establecer contacto con el núcleo del sistema. Revisa tu conexión a internet.',
      icon: WifiOff,
      glowClass: 'bg-slate-500/20',
      iconContainerClass: 'bg-slate-500/10 border-slate-500/20',
      iconClass: 'text-slate-400',
      badgeClass: 'bg-slate-600',
      gradientClass: 'bg-gradient-to-r from-slate-400 to-zinc-500'
    },
    'default': {
      title: 'Fallo',
      highlight: 'Crítico',
      message: 'Se ha producido una interrupción inesperada. El equipo técnico ha sido notificado.',
      icon: AlertTriangle,
      glowClass: 'bg-rose-500/20',
      iconContainerClass: 'bg-rose-500/10 border-rose-500/20',
      iconClass: 'text-rose-400',
      badgeClass: 'bg-rose-600',
      gradientClass: 'bg-gradient-to-r from-rose-400 to-pink-600'
    }
  };

  getErrorConfig(errorCode: string | null | undefined): ErrorUIConfig {
    const code = errorCode || 'default';
    return this.ERROR_CONFIGS[code] || this.ERROR_CONFIGS['default'];
  }
}
