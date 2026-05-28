# Instructions

- Following Playwright test failed.
- Explain why, be concise, respect Playwright best practices.
- Provide a snippet of code with the fix, if possible.

# Test info

- Name: new_features.spec.ts >> New Features Integrity Tests >> Cuentas por Auditar: Ver Detalles de Factura should toggle accordion row and display concepts/payments
- Location: e2e\new_features.spec.ts:18:7

# Error details

```
TimeoutError: page.waitForSelector: Timeout 10000ms exceeded.
Call log:
  - waiting for locator('tbody tr.group\\/row') to be visible

```

# Page snapshot

```yaml
- generic [ref=e4]:
  - generic [ref=e6]:
    - generic [ref=e7]:
      - generic [ref=e9]: S
      - generic [ref=e10]: SAT Hosp
    - navigation [ref=e11]:
      - generic [ref=e12]: Menu Principal
      - link "Panel de Control" [ref=e13] [cursor=pointer]:
        - /url: /dashboard
        - img [ref=e15]
        - generic [ref=e20]: Panel de Control
      - button "Gestión de Facturación" [ref=e22] [cursor=pointer]:
        - generic [ref=e23]:
          - img [ref=e25]
          - text: Gestión de Facturación
        - img [ref=e30]
      - link "Órdenes RX" [ref=e32] [cursor=pointer]:
        - /url: /rx-orders
        - img [ref=e34]
        - generic [ref=e36]: Órdenes RX
      - link "Órdenes Tomografía" [ref=e37] [cursor=pointer]:
        - /url: /tomo-orders
        - img [ref=e39]
        - generic [ref=e41]: Órdenes Tomografía
      - link "Monitor de Laboratorio" [ref=e42] [cursor=pointer]:
        - /url: /processing-monitor
        - img [ref=e44]
        - generic [ref=e46]: Monitor de Laboratorio
      - link "Seguros, Garantías & Compromisos" [ref=e47] [cursor=pointer]:
        - /url: /seguros
        - img [ref=e49]
        - generic [ref=e52]: Seguros, Garantías & Compromisos
      - generic [ref=e53]: Operativo / Gestión
      - button "Caja y Catálogos" [ref=e55] [cursor=pointer]:
        - generic [ref=e56]:
          - img [ref=e58]
          - text: Caja y Catálogos
        - img [ref=e62]
      - generic [ref=e64]:
        - button "Reportes Operativos" [ref=e65] [cursor=pointer]:
          - generic [ref=e66]:
            - img [ref=e68]
            - text: Reportes Operativos
          - img [ref=e71]
        - generic [ref=e73]:
          - link "Control de Citas" [ref=e74] [cursor=pointer]:
            - /url: /control-citas
          - link "Cuentas por Cobrar" [ref=e75] [cursor=pointer]:
            - /url: /cxc
          - link "Expediente de Facturación" [ref=e76] [cursor=pointer]:
            - /url: /expediente-facturacion
          - link "Cuentas por Auditar" [ref=e77] [cursor=pointer]:
            - /url: /admin/audit/cuentas
          - link "Órdenes RX" [ref=e78] [cursor=pointer]:
            - /url: /rx-orders
          - link "Órdenes Tomografía" [ref=e79] [cursor=pointer]:
            - /url: /tomo-orders
          - link "Expedientes" [ref=e80] [cursor=pointer]:
            - /url: /expedientes
          - link "Consolidado de Seguros" [ref=e81] [cursor=pointer]:
            - /url: /admin/seguros/gerencia
          - link "Consolidado de Imagenología" [ref=e82] [cursor=pointer]:
            - /url: /admin/imaging-monitor
      - button "Gestión Médica" [ref=e84] [cursor=pointer]:
        - generic [ref=e85]:
          - img [ref=e87]
          - text: Gestión Médica
        - img [ref=e92]
      - button "Configuración" [ref=e95] [cursor=pointer]:
        - generic [ref=e96]:
          - img [ref=e98]
          - text: Configuración
        - img [ref=e102]
      - link "Estado del Sistema v1.2.68" [ref=e104] [cursor=pointer]:
        - /url: /github-test
        - img [ref=e106]
        - generic [ref=e109]: Estado del Sistema v1.2.68
    - generic [ref=e111]:
      - img [ref=e114]
      - generic [ref=e119]:
        - paragraph [ref=e120]: admin
        - paragraph [ref=e121]: Admin
      - button [ref=e122] [cursor=pointer]:
        - img [ref=e124]
  - main [ref=e127]:
    - generic [ref=e129]:
      - button [ref=e132] [cursor=pointer]:
        - img [ref=e134]
      - generic [ref=e138]: JB
    - generic [ref=e142]:
      - generic [ref=e144]:
        - generic [ref=e145]:
          - img [ref=e148]
          - generic [ref=e150]:
            - heading "Panel de Auditoría" [level=1] [ref=e151]
            - paragraph [ref=e152]: Control de Integridad y Validación Operativa
        - generic [ref=e153]:
          - button "Auditoría de Cuentas" [ref=e154] [cursor=pointer]
          - button "Órdenes Directas" [ref=e155] [cursor=pointer]
      - generic [ref=e156]:
        - generic [ref=e158]:
          - generic [ref=e159]:
            - textbox "Buscar por Nombre o Cédula..." [ref=e160]
            - img [ref=e162]
          - generic [ref=e166]:
            - generic [ref=e167]:
              - generic [ref=e168]: Desde
              - textbox [ref=e169] [cursor=pointer]: 2026-05-28
              - generic [ref=e170] [cursor=pointer]:
                - text: 28/05/2026
                - img [ref=e172]
            - generic [ref=e175]:
              - generic [ref=e176]: Hasta
              - textbox [ref=e177] [cursor=pointer]: 2026-05-28
              - generic [ref=e178] [cursor=pointer]:
                - text: 28/05/2026
                - img [ref=e180]
          - generic [ref=e183]:
            - combobox [ref=e184] [cursor=pointer]:
              - option "POR PROCESAR" [selected]
              - option "PROCESADAS"
            - generic:
              - img
          - generic [ref=e186]:
            - combobox [ref=e187] [cursor=pointer]:
              - option "SÓLO CONVENIOS" [selected]
              - option "SÓLO PARTICULAR"
              - option "TODOS"
            - generic:
              - img
          - button "FILTRAR" [ref=e188] [cursor=pointer]
        - table [ref=e191]:
          - rowgroup [ref=e192]:
            - row "Fecha Paciente Seguro Monto Total Saldo Acción" [ref=e193]:
              - columnheader "Fecha" [ref=e194]
              - columnheader "Paciente" [ref=e195]
              - columnheader "Seguro" [ref=e196]
              - columnheader "Monto Total" [ref=e197]
              - columnheader "Saldo" [ref=e198]
              - columnheader "Acción" [ref=e199]
          - rowgroup [ref=e200]:
            - row "No hay cuentas por auditar que coincidan con la búsqueda" [ref=e201]:
              - cell "No hay cuentas por auditar que coincidan con la búsqueda" [ref=e202]:
                - img [ref=e204]
                - paragraph [ref=e206]: No hay cuentas por auditar que coincidan con la búsqueda
    - generic:
      - img
```

# Test source

```ts
  1  | import { test, expect } from '@playwright/test';
  2  | 
  3  | test.describe('New Features Integrity Tests', () => {
  4  |   test.beforeEach(async ({ page }) => {
  5  |     // 1. Authenticate as Admin
  6  |     await page.goto('/');
  7  |     await page.waitForLoadState('networkidle');
  8  | 
  9  |     await page.fill('input#username', 'admin');
  10 |     await page.fill('input#password', 'Admin123*!');
  11 |     await page.click('button[type="submit"]');
  12 | 
  13 |     // Wait for redirect to dashboard
  14 |     await page.waitForURL('**/dashboard');
  15 |     console.log('Logged in successfully, navigated to dashboard.');
  16 |   });
  17 | 
  18 |   test('Cuentas por Auditar: Ver Detalles de Factura should toggle accordion row and display concepts/payments', async ({ page }) => {
  19 |     // 1. Navigate to Auditing panel
  20 |     await page.goto('/admin/audit/cuentas');
  21 |     await page.waitForLoadState('networkidle');
  22 | 
  23 |     // Wait for the table/loading to finish
  24 |     await page.waitForSelector('table');
  25 | 
  26 |     // Wait for at least one receivable row to be loaded
  27 |     const rowSelector = 'tbody tr.group\\/row';
> 28 |     await page.waitForSelector(rowSelector, { timeout: 10000 });
     |                ^ TimeoutError: page.waitForSelector: Timeout 10000ms exceeded.
  29 |     console.log('Receivables rows are visible.');
  30 | 
  31 |     // Click "Ver Detalles de Factura" (first button in Actions cell)
  32 |     const detailBtn = page.locator('button[title="Ver Detalles de Factura"]').first();
  33 |     await expect(detailBtn).toBeVisible();
  34 |     await detailBtn.click();
  35 |     console.log('Clicked "Ver Detalles de Factura" button.');
  36 | 
  37 |     // Assert that the accordion detail row becomes visible
  38 |     const detailRow = page.locator('text=Conceptos Facturados').first();
  39 |     await expect(detailRow).toBeVisible({ timeout: 10000 });
  40 |     console.log('Accordion expanded successfully, "Conceptos Facturados" section is visible.');
  41 | 
  42 |     const paymentsRow = page.locator('text=Historial de Abonos Registrados').first();
  43 |     await expect(paymentsRow).toBeVisible();
  44 |     console.log('"Historial de Abonos Registrados" section is visible.');
  45 |   });
  46 | 
  47 |   test('Expediente de Facturacion: Filter by "Con Compromiso" and reprint action buttons', async ({ page }) => {
  48 |     // 1. Navigate to Expediente de Facturación
  49 |     await page.goto('/expediente-facturacion');
  50 |     await page.waitForLoadState('networkidle');
  51 | 
  52 |     // Wait for table
  53 |     await page.waitForSelector('table');
  54 | 
  55 |     // Verify "Con Compromiso" checkbox is visible
  56 |     const soloCompromisoCheckbox = page.locator('input#soloCompromiso');
  57 |     await expect(soloCompromisoCheckbox).toBeVisible();
  58 |     console.log('"Con Compromiso" checkbox filter is visible.');
  59 | 
  60 |     // Toggle the checkbox
  61 |     await soloCompromisoCheckbox.check();
  62 |     console.log('Checked "Con Compromiso" checkbox.');
  63 | 
  64 |     // Wait for table to refresh (either show results or show "no hay registros")
  65 |     await page.waitForTimeout(2000);
  66 | 
  67 |     // If there are records in the list, verify reprint buttons are present
  68 |     const reprintCompromisoBtn = page.locator('button[title="Reimprimir Compromiso de Pago"]');
  69 |     const countCompromiso = await reprintCompromisoBtn.count();
  70 |     console.log(`Found ${countCompromiso} "Reimprimir Compromiso de Pago" buttons.`);
  71 | 
  72 |     const reprintGarantiaBtn = page.locator('button[title="Reimprimir Garantía de Pago"]');
  73 |     const countGarantia = await reprintGarantiaBtn.count();
  74 |     console.log(`Found ${countGarantia} "Reimprimir Garantía de Pago" buttons.`);
  75 | 
  76 |     if (countCompromiso > 0) {
  77 |       await expect(reprintCompromisoBtn.first()).toBeEnabled();
  78 |       console.log('First "Reimprimir Compromiso de Pago" button is enabled.');
  79 |     }
  80 |     if (countGarantia > 0) {
  81 |       await expect(reprintGarantiaBtn.first()).toBeEnabled();
  82 |       console.log('First "Reimprimir Garantía de Pago" button is enabled.');
  83 |     }
  84 | 
  85 |     // Uncheck "Con Compromiso" checkbox
  86 |     await soloCompromisoCheckbox.uncheck();
  87 |     console.log('Unchecked "Con Compromiso" checkbox.');
  88 |     await page.waitForTimeout(1000);
  89 |   });
  90 | });
  91 | 
```