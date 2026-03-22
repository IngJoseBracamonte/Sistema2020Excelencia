export interface TicketError {
  id: string;
  requestPath: string;
  metodoHTTP: string;
  mensajeExcepcion: string;
  stackTrace: string;
  usuarioAsociado?: string;
  fechaCreacion: string;
  resuelto: boolean;
  comentariosResolucion?: string;
  fechaResolucion?: string;
  resueltoPor?: string;
}

export interface ResolveTicketRequest {
  comentariosResolucion?: string;
}
