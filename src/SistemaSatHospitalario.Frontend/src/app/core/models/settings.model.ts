export interface ConfiguracionGeneral {
  nombreEmpresa: string;
  rif: string;
  iva: number;
  facturarLaboratorio?: boolean;
  mostrarDetalleFacturacion?: boolean;
  claveSupervisor?: string;
  logoBase64?: string;
}

export interface UserDto {
  id: string;
  username: string;
  email: string;
  esActivo: boolean;
  roles: string[];
  permissions: string[];
}
