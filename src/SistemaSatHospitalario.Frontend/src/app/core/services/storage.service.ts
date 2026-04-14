import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class StorageService {
  /**
   * Keys used in the application. Standardized here to prevent debt.
   */
  private static readonly KEYS = {
    TOKEN: 'jwt_token',
    USERNAME: 'username',
    ROLE: 'user_role',
    USER_ID: 'user_id'
  };

  /**
   * Persists a key-value pair in local storage.
   */
  public setItem(key: string, value: string): void {
    localStorage.setItem(key, value);
  }

  /**
   * Retrieves a value from local storage.
   */
  public getItem(key: string): string | null {
    return localStorage.getItem(key);
  }

  /**
   * Removes an item from local storage.
   */
  public removeItem(key: string): void {
    localStorage.removeItem(key);
  }

  /**
   * High-level Session Management (Standardized)
   */
  public saveAuthData(token: string, username: string, role: string, id: string): void {
    this.setItem(StorageService.KEYS.TOKEN, token);
    this.setItem(StorageService.KEYS.USERNAME, username);
    this.setItem(StorageService.KEYS.ROLE, role);
    this.setItem(StorageService.KEYS.USER_ID, id);
  }

  public clearAuthData(): void {
    this.removeItem(StorageService.KEYS.TOKEN);
    this.removeItem(StorageService.KEYS.USERNAME);
    this.removeItem(StorageService.KEYS.ROLE);
    this.removeItem(StorageService.KEYS.USER_ID);
  }

  public getAuthData() {
    return {
      token: this.getItem(StorageService.KEYS.TOKEN),
      username: this.getItem(StorageService.KEYS.USERNAME),
      role: this.getItem(StorageService.KEYS.ROLE),
      id: this.getItem(StorageService.KEYS.USER_ID)
    };
  }
}
