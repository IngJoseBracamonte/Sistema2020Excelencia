import { request } from '@playwright/test';

/**
 * Global Setup: Garantiza datos mínimos de seed para la suite E2E.
 * Se ejecuta UNA SOLA VEZ antes de todos los tests.
 * Crea los ítems de catálogo necesarios si no existen en la BD.
 */
export default async function globalSetup() {
  const ctx = await request.newContext({
    baseURL: 'https://localhost',
    ignoreHTTPSErrors: true,
  });

  try {
    // 1. Autenticar como admin para obtener token JWT
    const authRes = await ctx.post('/api/auth/login', {
      data: { username: 'admin', password: 'Admin123*!' },
    });

    if (!authRes.ok()) {
      console.warn(`[GLOBAL SETUP] Auth failed (${authRes.status()}). Skipping seed.`);
      return;
    }

    const authData = await authRes.json();
    const token: string = authData.token ?? authData.jwtToken ?? '';
    if (!token) {
      console.warn('[GLOBAL SETUP] No token received. Skipping seed.');
      return;
    }

    const headers = { Authorization: `Bearer ${token}` };

    // 2. Obtener catálogo unificado actual
    const catalogRes = await ctx.get('/api/Catalog/unified', { headers });
    const items: Array<{ codigo: string; nombre: string }> = catalogRes.ok()
      ? await catalogRes.json()
      : [];

    // 3. Definir ítems mínimos requeridos por la suite E2E
    const requiredItems = [
      {
        nombre: 'Consulta Medica General',
        codigo: 'E2E-CON-001',
        tipoServicioNombre: 'CONSULTA',
        requiereMedico: true,
        precioBaseUsd: 40.0,
        honorarioMedicoUsd: 20.0,
      },
      {
        nombre: 'Consulta Ginecologica',
        codigo: 'E2E-CON-GIN',
        tipoServicioNombre: 'CONSULTA',
        requiereMedico: true,
        precioBaseUsd: 40.0,
        honorarioMedicoUsd: 20.0,
      },
      {
        nombre: 'Radiografía Tórax',
        codigo: 'E2E-RX-001',
        tipoServicioNombre: 'RX',
        requiereMedico: false,
        precioBaseUsd: 25.0,
        honorarioMedicoUsd: 0,
      },
      {
        nombre: 'Ibuprofeno',
        codigo: 'E2E-MED-IBU',
        tipoServicioNombre: 'MEDICAMENTO',
        requiereMedico: false,
        precioBaseUsd: 2.5,
        honorarioMedicoUsd: 0,
      },
    ];

    // 4. Crear sólo los que no existen (idempotente)
    for (const item of requiredItems) {
      const alreadyExists = items.some(
        (i) =>
          i.codigo === item.codigo ||
          (i.nombre ?? '').toLowerCase() === item.nombre.toLowerCase()
      );

      if (!alreadyExists) {
        const createRes = await ctx.post('/api/Catalog', {
          data: item,
          headers,
        });
        if (createRes.ok()) {
          console.log(`[GLOBAL SETUP] ✅ Creado: ${item.nombre} (${item.codigo})`);
        } else {
          const body = await createRes.text();
          console.warn(
            `[GLOBAL SETUP] ⚠️ No se pudo crear ${item.nombre}: HTTP ${createRes.status()} - ${body}`
          );
        }
      } else {
        console.log(`[GLOBAL SETUP] ℹ️ Ya existe: ${item.nombre}`);
      }
    }

    console.log('[GLOBAL SETUP] ✅ Seed E2E completado.');
  } catch (err) {
    console.warn('[GLOBAL SETUP] Error durante seed:', err);
  } finally {
    await ctx.dispose();
  }
}
