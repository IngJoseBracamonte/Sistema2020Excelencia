import { defineConfig, devices } from '@playwright/test';

export default defineConfig({
  testDir: './e2e',
  // Timeout global por test: 60s (era 30s — insuficiente en Docker con red interna)
  timeout: 60_000,
  fullyParallel: true,
  forbidOnly: !!process.env['CI'],
  retries: process.env['CI'] ? 2 : 1, // 1 retry en local para flakiness de timing
  workers: process.env['CI'] ? 1 : undefined,
  reporter: [['html'], ['list']],
  use: {
    baseURL: process.env.BASE_URL || 'http://localhost',
    trace: 'on-first-retry',
    // Timeout por acción individual (click, fill, selectOption, etc.)
    actionTimeout: 15_000,
    // Timeout para navegación (goto, waitForURL)
    navigationTimeout: 20_000,
  },
  projects: [
    {
      name: 'chromium',
      use: { ...devices['Desktop Chrome'] },
    },
  ],
});
