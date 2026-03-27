import { Pipe, PipeTransform } from '@angular/core';

/**
 * Pipe CurrencyBs (Pachón Pro V7.0)
 * Formatea un valor numérico a moneda Bolívares (es-VE) con 2 decimales fijos.
 */
@Pipe({
  name: 'currencyBs',
  standalone: true
})
export class CurrencyBsPipe implements PipeTransform {
  transform(value: number | string | null | undefined): string {
    if (value === null || value === undefined || value === '') return '0,00';

    const num = typeof value === 'string' ? parseFloat(value) : value;

    if (isNaN(num)) return '0,00';

    return num.toLocaleString('es-VE', {
      minimumFractionDigits: 2,
      maximumFractionDigits: 2
    });
  }
}
