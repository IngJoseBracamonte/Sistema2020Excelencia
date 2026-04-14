export interface ConfiguracionGeneral {
  nombreEmpresa: string;
  rif: string;
  iva: number;
  facturarLaboratorio?: boolean;
  mostrarDetalleFacturacion?: boolean;
  claveSupervisor?: string;
}

export interface UserDto {
  id: string;
  username: string;
  email: string;
  roles: string[];
}
