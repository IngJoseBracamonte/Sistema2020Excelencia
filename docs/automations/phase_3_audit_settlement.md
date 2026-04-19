# Phase 3: Financial Audit & Settlement Automation (Module D)

This phase automates the post-billing financial lifecycle, focusing on Accounts Receivable (AR) integrity and efficient insurance settlement (Liquidar).

## 1. App Breakdown: Module D (Financial Audit)
- **Primary Source**: [AuditARCommandHandler.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application/Commands/Admision/AuditARCommandHandler.cs)
- **Key Entity**: `CuentaPorCobrar`
- **The "Real Information" Gap**: The system tracks AR but doesn't auto-flag "Aging Anomalies" (e.g., accounts over 60 days that haven't moved to settlement).

### Proposed Automation Tasks:
- [ ] **Automated AR Aging Engine**: Background worker that calculates the age of all open `CuentaPorCobrar` and auto-flags those exceeding historical insurance payment windows.
- [ ] **Discrepancy Auto-Audit**: Implement an automated check that flags AR records where the "Billed Total" does not equal "Base + Honoraria" due to manual edits.

## 2. Process Optimization: Settlement (Liquidar)
- **Primary Source**: [SettleARCommandHandler.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application/Commands/Admision/SettleARCommandHandler.cs)
- **Key Process**: Applying insurance payments to closed accounts.
- **The "Real Information" Gap**: Settlement is a manual entry process for each payment method. There's no automated "Batch Reconciler" for insurance deposit files.

### Proposed Automation Tasks:
- [ ] **Batch Settlement Importer**: Automation to ingest standardized insurance settlement files (Excel/CSV) and auto-allocate `SettleARCommand` calls to matching `CuentaPorCobrar` records.
- [ ] **Overpayment/Underpayment Bot**: Auto-calculate and log differences between expected AR and actual settlement, creating "Adjustment Tickets" for accounting review.

## Data Integrity Objective
By the end of Phase 3, the financial department will have "Real-time AR Health" data, significantly reducing the manual labor required for insurance reconciliation.
