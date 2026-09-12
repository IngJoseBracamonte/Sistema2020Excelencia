import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  // Seed mínimo de datos antes de toda la suite
  globalSetup: require.resolve('./e2e/global-setup'),
  // Timeout global por test: 60s
  timeout: 60_000,
  // Los escenarios E2E comparten pacientes, cuentas, camas y catálogo.
  fullyParallel: false,
  forbidOnly: !!process.env['CI'],
  retries: process.env['CI'] ? 2 : 1, // 1 retry en local
  workers: 1,
  reporter: [['html'], ['list']],
  use: {
    baseURL: 'https://localhost',
    ignoreHTTPSErrors: true,
    serviceWorkers: 'block',
    trace: 'on-first-retry',
    actionTimeout: 15_000,
    navigationTimeout: 30_000,
  },
  expect: {
    // Timeout para aserciones expect()
    timeout: 15_000,
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
