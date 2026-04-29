export enum UserRole {
  Admin = 'Admin',
  Administrador = 'Administrador',
  AsistenteParticular = 'Asistente Particular',
  AsistenteSeguro = 'Asistente Seguro',
  AsistenteSeguros = 'Asistente de Seguros', // Variación en el sistema
  AsistenteRX = 'Asistente RX',
  AsistenteTomografia = 'Asistente de Tomografía',
  Medico = 'Médico',
  Cajero = 'Cajero',
  Supervisor = 'Supervisor',
  Farmacia = 'Farmacia',
  RX = 'RX',
  Hospitalario = 'Hospitalario',
  Emergencia = 'Emergencia'
}

export const RoleKeywords = {
  RX: 'rx',
  Farmacia: 'farmacia',
  Hospitalario: 'hospitalario',
  Emergencia: 'emergencia',
  AsistenteRX: 'asistente rx'
};

export const RoleGroups = {
  Administrative: [UserRole.Admin, UserRole.Administrador, UserRole.Supervisor],
  Support: [UserRole.AsistenteParticular, UserRole.AsistenteSeguro, UserRole.AsistenteSeguros, UserRole.AsistenteRX, UserRole.AsistenteTomografia],
  Clinical: [UserRole.Medico]
};
