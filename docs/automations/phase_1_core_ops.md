# Phase 1: Core Operations & Verification Automation (Module A & B)

This phase establishes the foundational automated checks for patient identity and technical service validation, ensuring "Real Information" flows correctly between Admission and Clinical departments.

## 1. App Breakdown: Module A (Admission)
- **Primary Source**: [PacienteAdmision.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Domain/Entities/Admision/PacienteAdmision.cs)
- **Key Logic**: [CreatePatientCommandHandler.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application/Commands/Admision/CreatePatientCommandHandler.cs)
- **The "Real Information" Gap**: Currently, the system lacks automated duplicate detection for Cedula/Pasaporte, leading to fragmented medical histories.

### Proposed Automation Tasks:
- [ ] **Identity Integrity Check**: Implement a pre-registration logic to auto-detect duplicate JSON/Entity entries before persisting a new patient.
- [ ] **JIT Legacy Sync Verification**: Automated check to ensure that every patient with an active account has a valid `IdPacienteLegacy`.

## 2. App Breakdown: Module B (Clinical Operations - "Verificar")
- **Primary Source**: [ValidateTechnicalServiceCommand.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application/Commands/Admision/ValidateTechnicalServiceCommand.cs)
- **Key Controller**: [ValidationController.cs](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.WebAPI/Controllers/Admision/ValidationController.cs)
- **The "Real Information" Gap**: Clinical validation is reactive. There's no automated alert for "Stale Pending Studies" that missed their TAT (Turnaround Time).

### Proposed Automation Tasks:
- [ ] **Smart TAT Monitoring**: Automated background worker that flags studies not marked as `REALIZADO` within 4 hours of account closure.
- [ ] **Real-time Push Automation**: Enhance the `INotificationService` to alert technical leads when a high-priority profile (e.g., Emergencia) is pending.

## Data Integrity Objective
By the end of Phase 1, the system will provide real-time visibility into technical throughput and eliminate duplicate patient profiles.
