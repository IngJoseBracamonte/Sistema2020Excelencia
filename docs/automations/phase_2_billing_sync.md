# Phase 2: Billing & Legacy Orchestration Automation (Module C)

This phase automates the mission-critical synchronization between the modern billing system and the legacy `Sistema2020`, ensuring financial and clinical data atomicity.

## 1. App Breakdown: Module C (Billing & Legacy Sync)
- **Primary Source**: [CloseAccountCommandHandler.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application/Commands/Admision/CloseAccountCommandHandler.cs)
- **Infrastructure**: [LegacyLabRepository.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Infrastructure/Persistence/Legacy/LegacyLabRepository.cs)
- **The "Real Information" Gap**: Failures in the legacy handshake are currently difficult to reconcile after a rollback. We lack a "Sync Audit Trail" that lives outside the main transaction.

### Proposed Automation Tasks:
- [ ] **Automated Mapping Registry**: (Formalization) Transition from description-parsing to an explicit `LegacyMappingId` manager in the catalog.
- [ ] **Sync Log Diagnostic Engine**: Background worker that parses `legacy_sync_log.txt` and automatically creates `DiagnosticsTicket` records for recurring cloud connectivity issues (e.g., Aiven SSL drops).
- [ ] **Proactive Database Pong**: Expand the monitoring worker to detect schema mismatches (missing tables) in the legacy system before billing fails.

## 2. Process Optimization: Account Closure
- **Key Flow**: Final Receipt -> Legacy Order -> Caja Entry.
- **The "Real Information" Gap**: Discrepancies between "Billed Amount" and "Legacy Order Amount" (often due to different currency/rounding logic) are not auto-flagged.

### Proposed Automation Tasks:
- [ ] **Financial Reconciliation Bot**: Automated comparison at the end of every sync cycle to verify that `ReciboFactura.TotalUsd` matches the sum of the generated `OrdenLegacy` items.
- [ ] **Partial Recovery Pipeline**: Implement an automated "retry" mechanism for non-critical legacy sync steps that don't violate transactional integrity.

## Data Integrity Objective
By the end of Phase 2, legacy synchronization will be "self-aware," reporting its own connectivity and data-match health before any user intervention is required.
