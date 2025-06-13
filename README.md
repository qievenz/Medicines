# Sistema de Ingesta y Consulta de Medicamentos

Este proyecto implementa una API RESTful para la gestión de datos de medicamentos, incluyendo la ingesta de información desde archivos CSV y JSON, la consulta paginada de medicamentos y la trazabilidad de los cambios a través de un sistema de auditoría.

---

## 🚀 Setup y Ejecución

Para poner en marcha el sistema, solo necesitas Docker y Docker Compose.

### Requisitos Previos

* **Docker Desktop** (o Docker Engine) instalado y en ejecución.

### Pasos de Ejecución

1.  **Clona el repositorio** (si aún no lo has hecho):

    ```bash
    git clone [URL_DE_TU_REPOSITORIO]
    cd [nombre_de_tu_repositorio]
    ```

2.  **Levanta la aplicación con Docker Compose**:

    Este comando construirá las imágenes de Docker, iniciará los servicios (API y base de datos SQL Server) en segundo plano y los mantendrá en ejecución.

    ```bash
    docker-compose -p medicines up --build -d
    ```

    * `-p medicines`: Asigna un nombre de proyecto a tus contenedores (ej. `medicines_api_1`, `medicines_sql_1`).
    * `--build`: Fuerza la reconstrucción de las imágenes, asegurando que se utilice el código más reciente.
    * `-d`: Ejecuta los contenedores en modo "detached" (en segundo plano).

3.  **Verifica los servicios (opcional)**:

    Puedes comprobar que los contenedores se están ejecutando con:

    ```bash
    docker-compose -p medicines ps
    ```

### Acceso a la API

Una vez que los contenedores estén en marcha, la API estará disponible en:

* **Swagger UI**: `http://localhost:5010/swagger`
    Aquí podrás interactuar con todos los endpoints de la API, probar las funcionalidades de ingesta, consulta y auditoría, y ver la documentación generada automáticamente.

### Autenticación

La API utiliza autenticación **Bearer Authentication** para todos los endpoints protegidos.

* **Token**: `SecretTokenToTest123` (Configurado para desarrollo en `appsettings.Development.json`)

Para usar la API (ej. en Postman o cURL), deberás incluir el encabezado `Authorization`:

```
Authorization: Bearer SecretTokenToTest123
```

---

## 🏛️ Decisiones de Arquitectura

El proyecto sigue una arquitectura limpia (Clean Architecture) o en capas, lo que promueve la separación de intereses, la mantenibilidad y la testabilidad.

* **Medicines.Core**:
    * **Propósito**: Contiene las entidades de negocio (`Medicine`, `IngestionProcess`, `AuditEntry`) y los enumeradores centrales. Es el núcleo de la aplicación y es independiente de cualquier framework o tecnología.
    * **Decisiones Clave**:
        * **Soft Delete para `Medicine`**: En lugar de eliminar registros físicamente, se utiliza la propiedad `IsDeleted = true`. Esto permite conservar el historial y mantener la trazabilidad.
        * **Unicidad del Código de Medicamento**: Se asegura que cada medicamento tenga un código único, lo que es fundamental para la identificación y la idempotencia en la ingesta.
* **Medicines.Application**:
    * **Propósito**: Define los casos de uso (comandos y queries) y las interfaces para la interacción con el dominio y la infraestructura. Contiene la lógica de negocio principal y las validaciones.
    * **Decisiones Clave**:
        * **CQRS Simple**: Implementación de comandos (ej. `UploadDataCommand`) y queries (ej. `GetPagedMedicinesQuery`) con sus respectivos handlers para separar las responsabilidades de lectura y escritura.
        * **Inyección de Repositorios**: Utiliza interfaces de repositorio (`IMedicineRepository`, `IIngestionProcessRepository`, `IAuditRepository`) para desacoplar la lógica de negocio de los detalles de persistencia.
        * **Validación con FluentValidation**: Las reglas de validación para los DTOs de entrada están externalizadas y definidas de forma declarativa con FluentValidation, mejorando la legibilidad y reusabilidad de la validación.
        * **Servicio de Validación/Mapeo**: Se creó un servicio `IMedicineValidationService` para encapsular la lógica de validación y mapeo de datos de ingesta a entidades de dominio, especialmente útil para manejar múltiples formatos (CSV/JSON) y variaciones en los nombres de campo.
* **Medicines.Infrastructure**:
    * **Propósito**: Implementa las interfaces definidas en la capa de Aplicación, gestionando los detalles de la base de datos y otras dependencias externas.
    * **Decisiones Clave**:
        * **Entity Framework Core**: Se utiliza como ORM para interactuar con la base de datos SQL Server, facilitando el mapeo objeto-relacional y la gestión de transacciones.
        * **Migraciones de EF Core**: El esquema de la base de datos se gestiona mediante migraciones, permitiendo evoluciones controladas del modelo de datos.
        * **Filtros de Consulta Global (Global Query Filters)**: Se configuró un filtro global en el `DbContext` para que las consultas a `Medicine` automáticamente excluyan los registros marcados como `IsDeleted = true`, simplificando las operaciones de lectura.
* **Medicines.Api**:
    * **Propósito**: La capa de presentación. Expone los endpoints RESTful, maneja las solicitudes HTTP, realiza la autenticación y delega las operaciones a los handlers de la capa de Aplicación.
    * **Decisiones Clave**:
        * **Controladores Minimalistas**: Los controladores son ligeros, centrados en el ruteo, el parseo de solicitudes y la invocación de handlers, manteniendo la lógica de negocio fuera de la capa de presentación.
        * **Autenticación Básica Personalizada**: Se implementó un handler de autenticación básico para la prueba y demostración, utilizando un token configurable.
        * **Swagger/OpenAPI**: Integración con Swashbuckle.AspNetCore para generar documentación interactiva de la API y facilitar su prueba. Se incluye configuración específica para la carga de archivos (`IFormFile`) y la autenticación.
        * **Inyección de Dependencias**: El contenedor de DI de .NET Core se utiliza para gestionar la creación y el ciclo de vida de los servicios, repositorios y handlers, asegurando un bajo acoplamiento.

---

## 🤔 Supuestos Realizados

* **Formato de Archivos**: Se asume que los archivos CSV y JSON seguirán los formatos esperados, aunque se ha implementado cierta tolerancia a la variación de nombres de campo en JSON y a la ausencia de campos en CSV.
* **Volumen de Datos**: La implementación actual maneja la ingesta de archivos completos en memoria antes de procesarlos. Se asume que el tamaño de los archivos no será excesivamente grande para causar problemas de memoria en el procesamiento síncrono.
* **Base de Datos SQL Server**: El sistema está configurado para utilizar SQL Server como motor de base de datos.
* **Token de Autenticación**: El token de autenticación (`SecretTokenToTest123`) es para propósitos de desarrollo y prueba. En un entorno de producción, se usaría un sistema de autenticación más robusto y una gestión de secretos segura.
* **Auditoría Básica**: La auditoría actual registra cambios en campos específicos de `Medicine`. Se asume que esta granularidad es suficiente para los requisitos actuales.

---

## 🚀 Mejoras Futuras a Implementar

1.  **Procesamiento Asíncrono de Ingesta (Colas de Mensajes)**:
    * **Problema Actual**: El endpoint `POST /api/v1/data-ingestion/upload` procesa todo el archivo de forma síncrona, lo que puede llevar a tiempos de espera prolongados y posibles timeouts para archivos grandes.
    * **Mejora Propuesta**:
        * Devolver un `202 Accepted` desde el endpoint `Upload` inmediatamente después de recibir el archivo y registrar el `IngestionProcess` en estado `Pending`.
        * Enviar los detalles del archivo a una **cola de mensajes** (ej. RabbitMQ, Azure Service Bus, Kafka).
        * Un **worker o consumidor en segundo plano** leería de esta cola y procesaría los archivos, actualizando el estado del `IngestionProcess` en la base de datos (`Processing`, `Completed`, `Failed`).
        * El endpoint `GET /api/v1/data-ingestion/{ingestionId}/status` permitiría al cliente consultar el estado del procesamiento.
    * **Beneficios**: Mejora la responsividad de la API, escalabilidad y robustez al manejar fallos transitorios.

2.  **Paralelismo en el Procesamiento de Registros**:
    * **Problema Actual**: El procesamiento de registros dentro de un archivo se realiza de forma secuencial.
    * **Mejora Propuesta**: Utilizar `Parallel.ForEach` o `Task.WhenAll` para procesar múltiples registros (líneas de CSV / objetos JSON) de un archivo en paralelo. Esto podría acelerar significativamente el tiempo total de procesamiento para archivos grandes.
    * **Consideraciones**: Requiere un manejo cuidadoso de las operaciones de base de datos (quizás usando un `DbContext` separado por hilo o lote de operaciones) y la agregación de errores/auditoría.

3.  **Manejo de Errores Más Granular y Reportes**:
    * **Mejora Propuesta**: Proporcionar informes más detallados sobre los registros fallidos durante la ingesta, incluyendo el número de línea o índice del registro y los errores específicos de validación. Esto podría almacenarse como parte del `ErrorDetails` del `IngestionProcess` o en una tabla separada de "Errores de Ingesta".

4.  **Autenticación y Autorización Robustas**:
    * **Mejora Propuesta**: Implementar un sistema de autenticación basado en **JWT (JSON Web Tokens)** con roles y permisos más finos, adecuado para entornos de producción. Esto implicaría la creación de un endpoint de login que devuelva un token, y el uso de atributos `[Authorize(Roles = "Admin")]` en los controladores/acciones.

5.  **Health Checks Detallados**:
    * **Mejora Propuesta**: Añadir Health Checks personalizados para verificar la conectividad con la base de datos, el estado de los repositorios y la disponibilidad de otros servicios críticos (como la cola de mensajes, si se implementa).

6.  **Paginación y Filtrado Avanzado**:
    * **Mejora Propuesta**: Extender las capacidades de paginación y filtrado en el endpoint de medicamentos para soportar criterios más complejos, como rangos de fechas de expiración, múltiples ingredientes activos, etc.

7.  **Monitorización**:
    * **Mejora Propuesta**: Enviar logs a sistemas de agregación (ej. ELK Stack, Azure Monitor) para una mejor monitorización y análisis de la aplicación.

8.  **Unit Tests y Integration Tests Completos**:
    * **Estado Actual**: Se han iniciado unit tests para los handlers.
    * **Mejora Propuesta**: Ampliar la cobertura de pruebas para incluir:
        * Más casos de borde y escenarios de fallo en los unit tests de los handlers y validadores.
        * Integration tests para los repositorios y los endpoints de la API, asegurando que las capas interactúen correctamente.
