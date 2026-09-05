/**
 * Constantes estandarizadas para Sedes / Almacenes hospitalarios.
 * Alineadas 1:1 con SeedConstants.cs (SistemaSatHospitalario.Core.Domain.Constants).
 */
export const SEDE_IDS = {
  PRINCIPAL: '10000000-0000-0000-0000-000000000001',
  EMERGENCIA: '10000000-0000-0000-0000-000000000002',
  HOSPITALIZACION: '10000000-0000-0000-0000-000000000003',
  UCI: '10000000-0000-0000-0000-000000000004',
  CIRUGIA: '10000000-0000-0000-0000-000000000005',
} as const;

export type SedeIdType = typeof SEDE_IDS[keyof typeof SEDE_IDS];
