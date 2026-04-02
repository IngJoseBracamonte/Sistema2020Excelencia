const { env } = require('process');

// Obtenemos la URL de la API inyectada por Aspire
// El nombre de la variable de entorno suele ser services__[resource]__[endpoint]__[index]
const target = env['services__api__endpoint-api__0'] || 
               env['services__api__http__0'] || 
               'http://localhost:8080';

console.log(`[Frontend Proxy] Reenviando peticiones de /api hacia: ${target}`);

module.exports = {
  "/api": {
    "target": target,
    "secure": false,
    "changeOrigin": true,
    "logLevel": "debug"
  },
  "/hub": {
    "target": target,
    "secure": false,
    "ws": true,
    "changeOrigin": true,
    "logLevel": "debug"
  }
};
