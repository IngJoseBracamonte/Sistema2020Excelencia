# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: health.spec.ts >> App should be accessible and show login page
- Location: e2e\health.spec.ts:3:5

# Error details

```
Error: page.goto: net::ERR_CONNECTION_REFUSED at http://localhost/
Call log:
  - navigating to "http://localhost/", waiting until "load"

```

# Test source

```ts
  1  | import { test, expect } from '@playwright/test';
  2  | 
  3  | test('App should be accessible and show login page', async ({ page }) => {
  4  |   // We assume the app is running on localhost:4200 (dev) or localhost:80 (docker)
  5  |   const baseUrl = process.env['BASE_URL'] || 'http://localhost';
  6  |   
> 7  |   await page.goto(baseUrl);
     |              ^ Error: page.goto: net::ERR_CONNECTION_REFUSED at http://localhost/
  8  |   
  9  |   // Check if we are redirected to login or see the login form
  10 |   await expect(page).toHaveTitle(/SAT Hosp/i);
  11 |   
  12 |   // Check if login button exists
  13 |   const loginBtn = page.getByRole('button', { name: /INICIAR SESIÃ“N/i });
  14 |   await expect(loginBtn).toBeVisible();
  15 | });
  16 | 
  17 | test('API Health check', async ({ request }) => {
  18 |   const apiUrl = process.env['API_URL'] || 'http://localhost:5000';
  19 |   const response = await request.get(`${apiUrl}/api/Tickets/report`, {
  20 |     headers: { 'X-Testing-Token': 'S4T_Hosp_Testing_2026' }
  21 |   });
  22 |   
  23 |   // Should return 400 (Bad Request) if command is empty but token is valid, 
  24 |   // or 200 if we send a dummy command. 
  25 |   // For now we just check it's not 401 or 404.
  26 |   expect(response.status()).not.toBe(401);
  27 |   expect(response.status()).not.toBe(404);
  28 | });
  29 | 
```