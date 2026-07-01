# Sistema Sat Hospitalario & Legacy 2020

Este repositorio contiene la solución unificada para el **Sistema Sat Hospitalario** (el nuevo núcleo administrativo y de facturación web) y el sistema de escritorio heredado **Sistema 2020** (enfocado en operaciones físicas de bioanálisis en laboratorios y sincronización de asistencia biométrica).

---

## 🗺️ Guías Interactivas de la Arquitectura

Hemos diseñado cuatro exploradores interactivos en HTML que detallan el proyecto y sus componentes con enlaces directos a tu entorno local:

1.  👉 **[Guía del Proyecto y Arquitectura General](file:///c:/Src/src/Sistema2020Excelencia/docs/guia_proyecto.html)**: Explica las partes globales, la sincronización de base de datos, el flujo de datos y los módulos de negocio (Admisión, Facturación, Honorarios Pro, Tesorería, etc.).
2.  👉 **[Catálogo Completo y Detallado de SRC](file:///c:/Src/src/Sistema2020Excelencia/docs/guia_src.html)**: Contiene un visor en árbol de los **698 archivos** dentro de la carpeta `src/` explicando el nombre, tipo, tamaño y la función exacta de cada archivo (controladores, entidades, queries, comandos, componentes Angular, servicios, etc.).
3.  👉 **[Detalle de Archivos Claves del Catálogo](file:///c:/Src/src/Sistema2020Excelencia/docs/detalle_catalogo.html)**: Documentación específica para auditar la entidad base (`ServicioClinico.cs`), el endpoint controlador (`CatalogController.cs`), el comando mutador MediatR (`UpdateCatalogItemCommand.cs`) y la interfaz visual del maestro / recetas BOM en Angular (`catalog-management.component.ts/.html`).
4.  👉 **[Prompts de Extensión de Receta BOM](file:///c:/Src/src/Sistema2020Excelencia/docs/prompts_receta.html)**: Contiene los prompts de instrucciones exactas preparados para inyectar en subagentes o nuevas sesiones de IA para automatizar la extensión de recetas BOM en Backend (.NET 10) y Frontend (Angular 19), con botones de copia al portapapeles.

---

## 📂 Estructura General del Proyecto

*   **[`src/`](file:///c:/Src/src/Sistema2020Excelencia/src)**: Contiene el nuevo sistema web.
    *   **[`SistemaSatHospitalario.Frontend/`](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Frontend)**: SPA desarrollada con Angular 18 (Standalone Components, signals, TailwindCSS).
    *   **[`SistemaSatHospitalario.WebAPI/`](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.WebAPI)**: API RESTful en .NET 10 bajo el patrón Clean Architecture.
    *   **[`SistemaSatHospitalario.Core.Application/`](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Application)**: Casos de uso implementados con MediatR.
    *   **[`SistemaSatHospitalario.Core.Domain/`](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Core.Domain)**: Entidades puras y constantes del dominio hospitalario.
    *   **[`SistemaSatHospitalario.Infrastructure/`](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.Infrastructure)**: Capa de persistencia (EF Core, Pomelo MySQL, Dapper).
    *   **[`SistemaSatHospitalario.AppHost/`](file:///c:/Src/src/Sistema2020Excelencia/src/SistemaSatHospitalario.AppHost)**: Proyecto de orquestación y resiliencia de contenedores con .NET Aspire.
*   **[`Laboratorio/`](file:///c:/Src/src/Sistema2020Excelencia/Laboratorio)**: Aplicación legacy de escritorio escrita en C# Windows Forms para la gestión interna de laboratorios médicos.
*   **[`Anviz/`](file:///c:/Src/src/Sistema2020Excelencia/Anviz)**: Módulo de integración biométrica y SDK para la sincronización física de asistencia del personal clínico.
*   **[`Conexiones/`](file:///c:/Src/src/Sistema2020Excelencia/Conexiones)**: Biblioteca compartida C# para acceso de bases de datos heredadas.
*   **[`instalacion/`](file:///c:/Src/src/Sistema2020Excelencia/instalacion)**: Herramientas de despliegue local automatizadas, scripts PowerShell y el panel del administrador (`MENU_ADMINISTRADOR.bat`).

---

## 🚀 Despliegue Rápido (Servidor Windows)

El sistema corre en contenedores Docker y se comunica con un servidor MySQL local que debe estar instalado directamente sobre el host de Windows en el puerto `3306`.

1.  Entra en la carpeta **[`instalacion/`](file:///c:/Src/src/Sistema2020Excelencia/instalacion)**.
2.  Configura tus credenciales de MySQL en el archivo `.env`.
3.  Ejecuta **`MENU_ADMINISTRADOR.bat`** con privilegios administrativos para instalar, actualizar, depurar o iniciar los servicios Docker automáticamente.

Para mayor información técnica, abre el documento interactivo **[`docs/guia_proyecto.html`](file:///c:/Src/src/Sistema2020Excelencia/docs/guia_proyecto.html)**.