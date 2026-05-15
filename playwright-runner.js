const { exec } = require('child_process');
const axios = require('axios');

const API_URL = process.env.API_URL || 'http://api:8080/api/Tickets/report';
const TESTING_TOKEN = 'S4T_Hosp_Testing_2026';
const INTERVAL = 15 * 60 * 1000; // 15 minutes

async function runTests() {
    console.log(`[${new Date().toISOString()}] Iniciando pruebas de integridad Playwright...`);
    
    exec('npx playwright test', async (error, stdout, stderr) => {
        if (error) {
            console.error(`[${new Date().toISOString()}] â Œ Fallo en las pruebas detectado!`);
            console.error(stderr);

            try {
                await axios.post(API_URL, {
                    requestPath: 'E2E/Playwright/Docker',
                    metodoHTTP: 'TEST',
                    mensajeExcepcion: 'Fallo CrÃtico en Pruebas de Integridad E2E (Playwright)',
                    stackTrace: stdout + '\n' + stderr,
                    usuarioAsociado: 'Playwright_Bot'
                }, {
                    headers: { 'X-Testing-Token': TESTING_TOKEN }
                });
                console.log(`[${new Date().toISOString()}] âœ… Alerta enviada al sistema de tickets.`);
            } catch (apiError) {
                console.error(`[${new Date().toISOString()}] â Œ No se pudo enviar la alerta a la API:`, apiError.message);
            }
        } else {
            console.log(`[${new Date().toISOString()}] âœ… Pruebas superadas con Ã©xito.`);
        }

        // Programar la siguiente ejecuciÃ³n
        setTimeout(runTests, INTERVAL);
    });
}

// Iniciar primer ciclo
runTests();
