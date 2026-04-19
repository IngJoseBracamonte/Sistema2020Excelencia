# Phase 4: Treasury & Observability Automation (Module E)

This phase completes the roadmap by automating cash control processes and establishing higher-level observability for the entire system's operational health.

## 1. App Breakdown: Module E (Treasury & Cash Control)
- **Primary Source**: [CerrarCajaCommandHandler.cs](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application/Commands/Admision/CerrarCajaCommand.cs)
- **Key Interface**: `ICajaAdministrativaRepository`
- **The "Real Information" Gap**: "Cierre de Caja" is currently a simple status toggle. We lack automated generation of the "Z-Report" (final daily balance) and audit-trail verification of all receipts vs. cash on hand.

### Proposed Automation Tasks:
- [ ] **Automated Z-Report Engine**: (Cierre de Caja) Implement an automated summary generation that calculates total USD vs total Bs. per operator at the moment of closure.
- [ ] **Suspicious Receipt Detector**: Background audit that flags any receipts (ReciboFactura) that were cancelled or modified more than 1 hour after their creation.
- [ ] **Currency Drift Monitor**: Automation to track the "Tasa Aplicada" across all daily receipts and flag anomalous deviations that could indicate user error or fraud.

## 2. Advanced Observability & Self-Diagnostics
- **Primary Source**: [DiagnosticsController.cs](file:///C:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.WebAPI/Controllers/DiagnosticsController.cs)
- **Insight Layer**: [BusinessInsightsQuery](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application/Queries/Admision/GetBusinessInsightsQuery.cs)
- **The "Real Information" Gap**: System health is checked manually (Diagnostics endpoint). We lack a "Pulse Monitoring" system that alerts the Admin if core metrics (e.g. Server Response Time, Legacy Latency) drop below optimal levels.

### Proposed Automation Tasks:
- [ ] **Technical Pulse Monitor**: Proactive diagnostic worker that heartbeats all core DBs (Identity, System, Legacy) every 5 minutes and identifies "Ghost Connections" that slow down the API.
- [ ] **Insight Automation**: Automated daily e-mail of the "Health Insight Report" to administrators, including recent system anomalies.

## Data Integrity Objective
By the end of Phase 4, the App will be a "Self-Reporting System" where treasury gaps are identified instantly and technical infrastructure is monitored 24/7 without human intervention.
