export interface BOMLine {
  insumoId: string;
  insumoCodigo: string;
  insumoNombre: string;
  cantidad: number;
  unidadMedida: string;
}

export interface HonorarioMedico {
  medicoId: string;
  medicoNombre: string;
  honorarioUsd: number;
}

export interface EquipoQuirurgico {
  id: string;
  codigo: string;
  nombre: string;
  rol: string;
  honorarioUsd: number;
}

export interface MedicoOption {
  id: string;
  nombre: string;
  especialidad: string;
}

export interface FormOption {
  value: string;
  label: string;
}

export function getTipoColor(tipo?: string | null): string {
  switch (tipo?.toUpperCase()) {
    case 'CONSULTA':
      return 'bg-rose-500/10 text-rose-500 border-rose-500/20';
    case 'MEDICINA':
    case 'MEDICAMENTO':
      return 'bg-violet-500/10 text-violet-400 border-violet-500/20';
    case 'LABORATORIO':
      return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/20';
    case 'RX':
      return 'bg-sky-500/10 text-sky-400 border-sky-500/20';
    case 'TOMO':
    case 'TOMOGRAFIA':
      return 'bg-indigo-500/10 text-indigo-400 border-indigo-500/20';
    case 'PROCEDIMIENTO':
      return 'bg-amber-500/10 text-amber-400 border-amber-500/20';
    case 'CIRUGIA':
      return 'bg-red-500/10 text-red-400 border-red-500/20';
    default:
      return 'bg-slate-500/10 text-slate-400 border-slate-500/20';
  }
}

export function getTipoBadgeStyle(tipo?: string | null): string {
  switch (tipo?.toUpperCase()?.trim()) {
    case 'CONSULTA':
      return 'bg-rose-500/10 text-rose-400 border-rose-500/30 shadow-rose-500/5';
    case 'MEDICINA':
    case 'MEDICAMENTO':
      return 'bg-violet-500/10 text-violet-400 border-violet-500/30 shadow-violet-500/5';
    case 'LABORATORIO':
      return 'bg-emerald-500/10 text-emerald-400 border-emerald-500/30 shadow-emerald-500/5';
    case 'RX':
      return 'bg-sky-500/10 text-sky-400 border-sky-500/30 shadow-sky-500/5';
    case 'TOMO':
    case 'TOMOGRAFIA':
      return 'bg-indigo-500/10 text-indigo-400 border-indigo-500/30 shadow-indigo-500/5';
    case 'PROCEDIMIENTO':
      return 'bg-amber-500/10 text-amber-400 border-amber-500/30 shadow-amber-500/5';
    case 'CIRUGIA':
      return 'bg-red-500/10 text-red-400 border-red-500/30 shadow-red-500/5';
    default:
      return 'bg-slate-500/10 text-slate-400 border-slate-500/30 shadow-slate-500/5';
  }
}

