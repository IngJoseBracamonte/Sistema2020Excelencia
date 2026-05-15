import { test, expect } from '@playwright/test';

test('App should be accessible and show login page', async ({ page }) => {
  // We assume the app is running on localhost:4200 (dev) or localhost:80 (docker)
  const baseUrl = process.env['BASE_URL'] || 'http://localhost';
  
  await page.goto(baseUrl);
  
  // Check if we are redirected to login or see the login form
  await expect(page).toHaveTitle(/SAT Hosp/i);
  
  // Check if login button exists
  const loginBtn = page.getByRole('button', { name: /INICIAR SESIÃ“N/i });
  await expect(loginBtn).toBeVisible();
});

test('API Health check', async ({ request }) => {
  const apiUrl = process.env['API_URL'] || 'http://localhost:5000';
  const response = await request.get(`${apiUrl}/api/Tickets/report`, {
    headers: { 'X-Testing-Token': 'S4T_Hosp_Testing_2026' }
  });
  
  // Should return 400 (Bad Request) if command is empty but token is valid, 
  // or 200 if we send a dummy command. 
  // For now we just check it's not 401 or 404.
  expect(response.status()).not.toBe(401);
  expect(response.status()).not.toBe(404);
});
