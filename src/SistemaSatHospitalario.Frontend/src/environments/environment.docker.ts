export const environment = {
  production: true,
  // En Docker, Nginx hace proxy de /api/ internamente al contenedor API.
  // No se necesita URL absoluta.
  apiUrl: '',
  systemVersion: '1.2.92'
};
