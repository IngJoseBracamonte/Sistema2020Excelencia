# Senior Design Patterns (Backend & Frontend)

---
name: Senior Design Patterns
description: Colección de patrones de diseño avanzados para arquitecturas limpias y escalables en .NET y Angular, integrando los principios SOLID.
---

## 🏛️ Principios SOLID (La Base del Diseño Senior)

1.  **S - Single Responsibility Principle (SRP)**:
    *   **Definición**: Una clase debe tener una única razón para cambiar.
    *   **Aplicación**: Separar `FacturacionComponent` (UI) de `BillingFacade` (Lógica de Estado).
2.  **O - Open/Closed Principle (OCP)**:
    *   **Definición**: Abierto para extensión, cerrado para modificación.
    *   **Aplicación**: Usar `ServiceCategory` y `Estrategias` para nuevos tipos de servicios sin tocar el core de facturación.
3.  **L - Liskov Substitution Principle (LSP)**:
    *   **Definición**: Los objetos deben ser reemplazables por instancias de sus subtipos sin alterar el funcionamiento.
    *   **Aplicación**: Todas las implementaciones de `IPricedItem` deben ser compatibles con los cálculos de totales.
4.  **I - Interface Segregation Principle (ISP)**:
    *   **Definición**: Un cliente no debe ser forzado a depender de interfaces que no usa.
    *   **Aplicación**: Dividir grandes servicios en interfaces pequeñas (`IPaymentService`, `IPatientSearchService`).
5.  **D - Dependency Inversion Principle (DIP)**:
    *   **Definición**: Depender de abstracciones, no de concreciones.
    *   **Aplicación**: Inyectar interfaces (`IApplicationDbContext`) en lugar de clases concretas de base de datos.

---

## 🏗️ Patrones en el Backend (.NET / DDD)

### 1. Repository & Unit of Work
*   **Caso de uso**: Desacoplar la lógica de persistencia del dominio y asegurar transacciones atómicas.
*   **Beneficio**: Facilita las pruebas unitarias y centraliza el acceso a datos.
*   **Ejemplo**: `IBillingRepository` para manejar persistencia de cuentas.

### 2. CQRS (Command Query Responsibility Segregation)
*   **Caso de uso**: Separar las operaciones de lectura (Query) de las de escritura (Command).
*   **Beneficio**: Optimiza el rendimiento y simplifica el escalado de reportes pesados.
*   **Ejemplo**: `GetUnifiedCatalogQuery` vs `CargarServicioCommand`.

### 3. Strategy Pattern
*   **Caso de uso**: Intercambiar algoritmos o lógicas de negocio en tiempo de ejecución.
*   **Beneficio**: Elimina bloques `if/else` gigantes y cumple con el principio Open/Closed.
*   **Ejemplo**: Diferentes estrategias de cálculo según el convenio del seguro.

### 4. Specification Pattern
*   **Caso de uso**: Encapsular reglas de validación o filtros complejos.
*   **Beneficio**: Reutilización de lógica de búsqueda en múltiples consultas.

---

## 🎨 Patrones en el Frontend (Angular / Signals)

### 1. Facade Pattern
*   **Caso de uso**: Proporcionar una interfaz simplificada a un subsistema complejo (varios servicios).
*   **Beneficio**: Reduce el acoplamiento directo entre componentes y múltiples servicios core.
*   **Ejemplo**: `BillingFacadeService` que orqueste Catálogo, Pagos y Agenda.

### 2. Container vs Presenter (Smart vs Dumb Components)
*   **Caso de uso**: Separar la gestión de datos (Smart) de la pura visualización (Dumb).
*   **Beneficio**: Facilita la reutilización de la UI y las pruebas de componentes.

### 3. State Management (Service-with-Signals)
*   **Caso de uso**: Gestionar estados reactivos globales o de feature.
*   **Beneficio**: Evita el "prop-drilling" y asegura que la UI esté sincronizada sin `manual change detection`.

### 4. Factory Pattern
*   **Caso de uso**: Crear instancias de componentes o estrategias dinámicamente.
*   **Beneficio**: Desacopla la creación del objeto de su uso.

---

## 🧠 Guía de Refactorización (Senior Level)
1. **Identificar Fat Components**: Si un componente tiene >300 líneas, necesita un Facade.
2. **Extraer Lógica a Pipes/Directivas**: Si el HTML tiene cálculos o formateos, muévelos a un Pipe.
3. **Desacoplar HTTP**: Los componentes nunca deben llamar a `HttpClient` directamente; usan Facades.
