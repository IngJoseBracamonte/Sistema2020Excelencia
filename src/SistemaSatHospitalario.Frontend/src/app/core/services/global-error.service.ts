import { Injectable, signal } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class GlobalErrorService {
  public isErrorActive = signal<boolean>(false);
  public errorCode = signal<string | number>('500');
  public errorMessage = signal<string>('');

  showError(code: string | number, message: string = '') {
    this.errorCode.set(code);
    this.errorMessage.set(message);
    this.isErrorActive.set(true);
  }

  clearError() {
    this.isErrorActive.set(false);
  }
}
