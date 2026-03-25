export interface ConfiguracionGeneral {
  nombreEmpresa: string;
  rif: string;
  iva: number;
}

export interface UserDto {
  id: string;
  username: string;
  email: string;
  roles: string[];
}
