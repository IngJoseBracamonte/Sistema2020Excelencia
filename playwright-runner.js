const { exec } = require('child_process');

const API_URL = process.env.API_URL;
const TESTING_TOKEN = process.env.TESTING_TOKEN;
const INTERVAL = 15 * 60 * 1000; // 15 minutes

async function runTests() {
    console.log(`[${new Date().toISOString()}] Iniciando pruebas de integridad Playwright...`);
    
    exec('npx playwright test', async (error, stdout, stderr) => {
        if (error) {
            console.error(`[${new Date().toISOString()}] â Œ Fallo en las pruebas detectado!`);
            console.error(stderr);

            try {
                    if (!API_URL || !TESTING_TOKEN) {
                        throw new Error('API_URL y TESTING_TOKEN deben configurarse para reportar fallos de pruebas.');
                    }

                    const response = await fetch(API_URL, {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'X-Testing-Token': TESTING_TOKEN
                        },
                        body: JSON.stringify({
                    requestPath: 'E2E/Playwright/Docker',
                    metodoHTTP: 'TEST',
                    mensajeExcepcion: 'Fallo CrÃtico en Pruebas de Integridad E2E (Playwright)',
                    stackTrace: stdout + '\n' + stderr,
                    usuarioAsociado: 'Playwright_Bot'
                });

                    if (!response.ok) {
                        throw new Error(`La API devolvió el estado ${response.status}.`);
                    }
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
