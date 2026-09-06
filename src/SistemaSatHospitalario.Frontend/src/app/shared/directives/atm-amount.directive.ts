import { Directive, EventEmitter, HostListener, Input, Output, ElementRef } from '@angular/core';

/**
 * Directiva ATM Amount (Pachón Pro V9.1 - Buffer Fix)
 * Mantiene un buffer interno de dígitos para evitar re-parseo del valor visual.
 * Ejemplo: Escribir "6" → "0,06"; luego "0" → "0,60"; luego "5" → "6,05".
 */
@Directive({
  selector: '[appAtmAmount]',
  standalone: true
})
export class AtmAmountDirective {
  @Input('appAtmAmount') enabled: boolean = true;
  @Output() amountChanged = new EventEmitter<number>();

  // Buffer interno de dígitos (evita re-parseo del valor formateado)
  private digitBuffer = '';

  constructor(private el: ElementRef<HTMLInputElement>) {}

  @HostListener('focus')
  onFocus() {
    if (!this.enabled) return;
    // Al enfocar, si el valor es 0, limpiamos el buffer
    if (this.el.nativeElement.value === '0,00' || this.el.nativeElement.value === '0.00') {
      this.digitBuffer = '';
    }
  }

  @HostListener('keydown', ['$event'])
  onKeydown(event: KeyboardEvent) {
    if (!this.enabled) return;

    const key = event.key;

    // Permitir dígitos 0-9
    if (/^\d$/.test(key)) {
      event.preventDefault();
      // Limitar buffer a 10 dígitos para evitar overflow
      if (this.digitBuffer.length >= 10) return;
      this.digitBuffer += key;
      this.render();
      return;
    }

    // Backspace: eliminar último dígito
    if (key === 'Backspace') {
      event.preventDefault();
      this.digitBuffer = this.digitBuffer.slice(0, -1);
      this.render();
      return;
    }

    // Bloquear cualquier otro carácter (letras, puntos, comas, etc.)
    if (!['Tab', 'Enter', 'ArrowLeft', 'ArrowRight', 'Delete'].includes(key)) {
      event.preventDefault();
    }
  }

  private render() {
    const digits = this.digitBuffer || '0';
    const numericValue = parseInt(digits, 10) / 100;

    // toFixed(2) garantiza SIEMPRE 2 decimales (4.40, no 4.4)
    // Luego sustituimos el punto por coma para el formato Venezuela
    const formatted = numericValue.toFixed(2).replace('.', ',');
    this.el.nativeElement.value = formatted;

    this.amountChanged.emit(numericValue);
  }

  // Resetear buffer cuando el componente padre setea el valor a 0
  @HostListener('blur')
  onBlur() {
    if (!this.enabled) return;
    const current = parseFloat(this.el.nativeElement.value.replace(',', '.'));
    if (current === 0) this.digitBuffer = '';
  }
}
