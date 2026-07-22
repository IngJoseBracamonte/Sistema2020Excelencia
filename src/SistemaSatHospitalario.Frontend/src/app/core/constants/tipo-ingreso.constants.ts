/**
 * Constantes estandarizadas de Tipos de Ingreso Clínico
 * Alineadas 1:1 con EstadoConstants de .NET (SistemaSatHospitalario.Core.Domain)
 */
export const TIPO_INGRESO = {
  EMERGENCIA: 'Emergencia',
  HOSPITALIZACION: 'Hospitalizacion',
  UCI: 'UCI',
  ENFERMERIA: 'Enfermeria',
  PARTICULAR: 'Particular',
  SEGURO: 'Seguro'
} as const;

export type TipoIngresoType = typeof TIPO_INGRESO[keyof typeof TIPO_INGRESO];

/**
 * Función utilitaria pura para normalización defensiva de TipoIngreso (ignora mayúsculas, minúsculas y tildes).
 */
export function normalizeTipoIngreso(tipo: string | null | undefined): string {
  if (!tipo) return '';
  return tipo
    .normalize('NFD')
    .replace(/[\u0300-\u036f]/g, '')
    .trim()
    .toLowerCase();
}

/**
 * Verifica si dos tipos de ingreso son equivalentes en Enfermería / Hospitalización.
 */
export function matchTipoIngreso(tipo: string | null | undefined, target: string | null | undefined): boolean {
  const normTipo = normalizeTipoIngreso(tipo);
  const normTarget = normalizeTipoIngreso(target);
  if (!normTipo || !normTarget) return false;
  if (normTarget === normalizeTipoIngreso(TIPO_INGRESO.HOSPITALIZACION)) {
    return normTipo === normalizeTipoIngreso(TIPO_INGRESO.HOSPITALIZACION) || normTipo === normalizeTipoIngreso(TIPO_INGRESO.ENFERMERIA);
  }
  return normTipo === normTarget;
}
