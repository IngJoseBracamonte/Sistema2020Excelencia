# Manual del Proceso de Facturación, Carga de Servicios y Admisión

Este documento detalla el flujo de trabajo completo del módulo de Facturación y Admisión de la plataforma, sirviendo como guía de referencia técnica y funcional.

---

## 1. Selección y Configuración del Paciente
Antes de iniciar cualquier acción de cobro o carga de servicios, se debe identificar al paciente:
*   **Búsqueda e Identificación**: Se ingresa la cédula o nombre en el buscador. El sistema consulta tanto la base de datos local (nativa) como el repositorio de legado JIT (Just-In-Time).
*   **Registro Inmediato (Onboarding)**: Si el paciente no existe, se abre el modal de registro rápido.
    *   **Formato de Fecha de Nacimiento**: La fecha de nacimiento se ingresa y visualiza estrictamente en formato `DD-MM-YYYY`. Internamente, el sistema realiza la conversión bidireccional a formato ISO `YYYY-MM-DD` para persistencia.
    *   **Tipos de Identidad**: Soporta Cédula de Identidad y Pasaporte.

---

## 2. Flujo de Seguros y Convenios (Particular vs. Asegurado)
El comportamiento del catálogo de precios y la cobertura de los servicios depende del tipo de ingreso seleccionado:
*   **Tipo de Ingreso**:
    *   **Particular**: El paciente asume el 100% de la tarifa del catálogo.
    *   **Seguro / Convenio**: Permite seleccionar una aseguradora registrada (ej: PDVSA, Mercantil, etc.).
*   **Ajuste del Catálogo por Convenio**:
    *   Al cambiar el seguro/convenio, se consulta el endpoint `/api/Catalog/unified?convenioId={id}`.
    *   Los precios base y coberturas del catálogo unificado se recalculan dinámicamente con base en los baremos contratados con dicha aseguradora.

---

## 3. El Catálogo Unificado y Clasificación de Servicios
El sistema unifica todos los conceptos facturables en un catálogo común estructurado por categorías:
*   **Tipos de Servicios**:
    *   **Consultas Médicas (MEDICO)**: Consultas generales y especializadas.
    *   **Laboratorios (LABORATORIO / LAB)**: Exámenes rutinarios y perfiles complejos.
    *   **Imagenología (RX / TOMO)**: Rayos X, tomografías y resonancias.
    *   **Insumos y Medicamentos (INSUMOS)**: Materiales gastables y fármacos.

---

## 4. Consultas Médicas: Selección de Doctores, Horario, Honorario y Citas
Las consultas médicas requieren información profesional y de agenda para ser validadas por el backend:
1.  **Selección del Médico Tratante**: Se despliega la lista de doctores activos con sus respectivas especialidades.
2.  **Cálculo Automático de Honorarios**:
    *   Al seleccionar el doctor, el sistema suma su **Honorario Base** al **Precio Base** del catálogo.
    *   Si no se modifican las tarifas manuales en pantalla, el payload se envía limpio, permitiendo al backend resolver los precios oficiales de forma segura.
3.  **Gestión de Horarios y Reservas**:
    *   Se selecciona la hora de la cita.
    *   **Evitar Solapamientos**: El backend valida en tiempo real si el médico tiene citas simultáneas a esa hora normalizada. Si existe coincidencia, desplaza el cupo automáticamente por incrementos de 1 minuto para asegurar la cita.
    *   **Creación de Cita**: Al cargar el servicio de consulta, se registra una entidad `CitaMedica` vinculada a la cuenta de servicio y área clínica del paciente.

---

## 5. Perfiles, Laboratorio e Imagenología (RX/Tomo)
*   **Laboratorios y Perfiles**:
    *   Se consultan directamente con códigos del catálogo de legado.
    *   Requieren obligatoriamente la presencia de un `LegacyMappingId` (ID de perfil de laboratorio) para asegurar la sincronización JIT con el sistema del laboratorio.
*   **Imagenología (RX / Tomografías)**:
    *   Se cargan de manera unitaria a la cuenta del paciente.
    *   **Regla de Negocio (V16.2)**: Para evitar órdenes huérfanas o estudios realizados sin garantía de cobro, el disparo del servicio de imágenes al sistema externo (PACS/RIS) se ejecuta de forma diferida al momento del **Cierre y Pago de la Cuenta**.

---

## 6. Proceso de Pago, Registro de Divisas y Cálculo de Cambio (Devuelto)
Una vez estructurada la cuenta del paciente con todos los cargos, se procede a la liquidación:
*   **USD-First (Moneda Base)**: Todos los saldos internos se calculan y consolidan en **Dólares Americanos ($)** para evitar distorsiones inflacionarias.
*   **Ingreso de Pagos Multi-moneda**:
    *   **Efectivo / Transferencia USD ($)**.
    *   **Bolívares (Bs.)**: Se ingresa el monto en bolívares y se divide de inmediato por la tasa oficial del día (`tasaCambioDia`) para integrarlo al saldo en dólares.
*   **Cálculo de Cambio (Vuelto)**:
    *   El sistema calcula el excedente pagado en dólares.
    *   Permite seleccionar la denominación y moneda de devolución (ej: devolver el cambio en Bolívares o Dólares en efectivo) calculando automáticamente la conversión a entregar al cliente.
*   **Cierre Fiscal**: Al confirmarse la transacción, se emite el recibo/factura fiscal, se cierra la cuenta administrativa y se notifican los sistemas externos.
