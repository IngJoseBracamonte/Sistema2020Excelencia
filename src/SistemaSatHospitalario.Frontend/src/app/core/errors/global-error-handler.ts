import { ErrorHandler, Injectable } from '@angular/core';

@Injectable()
export class GlobalErrorHandler implements ErrorHandler {
  handleError(error: any): void {
    const timestamp = new Date().toISOString();
    const message = error.message ? error.message : error.toString();
    const stack = error.stack ? error.stack : 'No stack trace available';

    // Logueamos de forma que sea visible en la consola capturada por Aspire
    console.group(`[FRONTEND ERROR] ${timestamp}`);
    console.error(`Message: ${message}`);
    console.error(`Stack: ${stack}`);
    console.groupEnd();

    // También podemos enviar esto a un servicio de telemetría si se desea
    // O llamar al ErrorHandler por defecto si queremos que siga apareciendo en la consola local
    // super.handleError(error); // ErrorHandler no es una clase base con super.handleError en este contexto si no heredamos
  }
}
