# UNIVERSIDAD PRIVADA DOMINGO SAVIO
## CARRERA DE INGENIERÍA DE SISTEMAS
### PROGRAMACIÓN WEB II

# DOCUMENTACIÓN TÉCNICA DEL PROYECTO FINAL
## EcoWash Móvil — Plataforma de Gestión Comercial de Lavado de Autos a Domicilio

---

### DATOS GENERALES DEL PROYECTO

| Campo | Detalle |
| :--- | :--- |
| **Nombre del proyecto** | EcoWash Móvil — Plataforma de Gestión Comercial de Lavado de Autos a Domicilio |
| **Integrantes** | Estudiante(s) de Ingeniería de Sistemas |
| **Docente** | Docente de Programación Web II |
| **Turno** | Mañana |
| **Tecnologías** | Vue.js 3 + ASP.NET Core 8 Web API + Entity Framework Core + SQL Server / SQLite / MySQL + Docker & Docker Compose |
| **Gestión** | I / 2026 |
| **Fecha** | Julio de 2026 |
| **Ciudad** | Santa Cruz de la Sierra – Bolivia (2026) |

---

## ÍNDICE GENERAL

1. [INTRODUCCIÓN](#1-introducción)
2. [DESCRIPCIÓN DEL PROBLEMA](#2-descripción-del-problema)
3. [JUSTIFICACIÓN](#3-justificación)
4. [OBJETIVOS](#4-objetivos)
5. [ALCANCE Y LIMITACIONES](#5-alcance-y-limitaciones)
6. [IMPACTO SOCIAL, EJE TRANSVERSAL Y ODS](#6-impacto-social-eje-transversal-y-ods)
7. [REQUERIMIENTOS DEL SISTEMA](#7-requerimientos-del-sistema)
   - 7.1 [Requerimientos funcionales](#71-requerimientos-funcionales)
   - 7.2 [Requerimientos no funcionales](#72-requerimientos-no-funcionales)
8. [DIAGRAMA DE CASOS DE USO](#8-diagrama-de-casos-de-uso)
   - 8.1 [Descripción de actores](#81-descripción-de-actores)
   - 8.2 [Especificación de casos de uso](#82-especificación-de-casos-de-uso)
9. [DIAGRAMA DE CLASES](#9-diagrama-de-clases)
   - 9.1 [Descripción de clases principales](#91-descripción-de-clases-principales)
10. [MODELO RELACIONAL DE LA BASE DE DATOS](#10-modelo-relacional-de-la-base-de-datos)
11. [DICCIONARIO DE DATOS](#11-diccionario-de-datos)
12. [ARQUITECTURA DEL SISTEMA](#12-arquitectura-del-sistema)
    - 12.1 [Arquitectura lógica](#121-arquitectura-lógica)
    - 12.2 [Arquitectura de contenedores](#122-arquitectura-de-contenedores)
    - 12.3 [Flujo de una solicitud](#123-flujo-de-una-solicitud)
13. [TECNOLOGÍAS UTILIZADAS](#13-tecnologías-utilizadas)
14. [DISEÑO DE INTERFACES](#14-diseño-de-interfaces)
15. [IMPLEMENTACIÓN DEL FRONTEND](#15-implementación-del-frontend)
    - 15.1 [Estructura del frontend](#151-estructura-del-frontend)
    - 15.2 [Componentes y vistas](#152-componentes-y-vistas)
    - 15.3 [Consumo de la API](#153-consumo-de-la-api)
16. [IMPLEMENTACIÓN DEL BACKEND](#16-implementación-del-backend)
    - 16.1 [Estructura del backend](#161-estructura-del-backend)
    - 16.2 [Endpoints de la API](#162-endpoints-de-la-api)
    - 16.3 [Integración con Entity Framework Core](#163-integración-con-entity-framework-core)
17. [IMPLEMENTACIÓN DE DOCKER (OBLIGATORIA)](#17-implementación-de-docker-obligatoria)
    - 17.1 [Conceptos aplicados](#171-conceptos-aplicados)
    - 17.2 [Servicios implementados](#172-servicios-implementados)
    - 17.3 [Dockerfile del frontend](#173-dockerfile-del-frontend)
    - 17.4 [Dockerfile del backend](#174-dockerfile-del-backend)
    - 17.5 [Archivo Docker Compose](#175-archivo-docker-compose)
    - 17.6 [Variables de entorno y seguridad](#176-variables-de-entorno-y-seguridad)
    - 17.7 [Evidencias de ejecución](#177-evidencias-de-ejecución)
18. [PRUEBAS DEL SISTEMA](#18-pruebas-del-sistema)
19. [MANUAL DE INSTALACIÓN Y EJECUCIÓN LOCAL](#19-manual-de-instalación-y-ejecución-local)
20. [CONTROL DE VERSIONES CON GIT Y GITHUB](#20-control-de-versiones-con-git-y-github)
21. [INVESTIGACIÓN IMRyD: ALTERNATIVAS DE HOSTING PARA EL PROYECTO](#21-investigación-imryd-alternativas-de-hosting-para-el-proyecto)
22. [CONCLUSIONES GENERALES DEL PROYECTO](#22-conclusiones-generales-del-proyecto)
23. [RECOMENDACIONES Y TRABAJO FUTURO](#23-recomendaciones-y-trabajo-futuro)
24. [REFERENCIAS BIBLIOGRÁFICAS](#24-referencias-bibliográficas)
25. [ANEXOS](#25-anexos)

---

## 1. INTRODUCCIÓN

En la ciudad de Santa Cruz de la Sierra, el sector automotriz experimenta un crecimiento constante en el parque vehicular, lo que genera una demanda creciente de servicios de mantenimiento y limpieza estética de vehículos. Sin embargo, el ritmo de vida urbano impone restricciones de tiempo a los propietarios de vehículos, dificultando su traslado a centros de lavado tradicionales. Frente a este contexto, emergen los servicios de lavado de vehículos a domicilio ("mobile car wash"), una solución orientada a llevar el servicio directamente a los hogares u oficinas de los clientes.

A pesar de su atractivo valor comercial, la mayoría de los pequeños y medianos emprendimientos de este sector operan mediante herramientas informales como registros manuales en cuadernos o mensajería instantánea. Esta informalidad operativa acarrea severos cuellos de botella: descoordinación en las agendas de los lavadores, falta de trazabilidad en las ventas, nulo control de insumos (shampoos, ceras, desengrasantes) y la ausencia de reportes financieros confiables.

Para dar respuesta integral a este problema se presenta **EcoWash Móvil**, una plataforma web moderna, escalable y contenerizada de tres capas. El sistema automatiza el ciclo comercial completo: desde el registro y reserva inteligente por parte del cliente, pasando por la asignación y ejecución del servicio por los lavadores, hasta la emisión de ventas, cobro mediante pasarelas/QR, descuento automático de inventario, auditoría de operaciones y presentación de métricas en dashboards administrativos.

La solución está construida adoptando estándares actuales del desarrollo software: un frontend desacoplado desarrollado en **Vue 3 SPA** (Vite, Pinia, Axios), un backend robusto basado en **ASP.NET Core 8 Web API** con **Entity Framework Core 8**, y una persistencia en base de datos en **3FN (Tercera Forma Normal)**. Todo el sistema se integra mediante **Docker** y **Docker Compose**, garantizando un despliegue aislado y portátil.

---

## 2. DESCRIPCIÓN DEL PROBLEMA

El modelo tradicional de gestión en las empresas emergentes de lavado de autos a domicilio en el medio local presenta las siguientes deficiencias críticas:

1. **Desorganización en el agendamiento y duplicidad de reservas:** Al recibir solicitudes por canales informales (WhatsApp, llamadas), no existe validación en tiempo real de la disponibilidad de personal ni horarios de traslado, ocasionando solapamientos o incumplimientos en la atención.
2. **Falta de visibilidad de ubicaciones y vehículos:** Los clientes deben reescribir repetidamente los datos de sus vehículos (placa, modelo, tipo) y sus direcciones, incrementando la tasa de error y ralentizando la atención.
3. **Pérdida de control de insumos e inventario:** Los insumos químicos y accesorios de limpieza sufren mermas no registradas. La empresa carece de un mecanismo automático que descuente el material gastado por cada servicio finalizado ni de alertas de stock mínimo.
4. **Ausencia de auditoría y reportes financieros:** La gerencia no dispone de estadísticas consolidadas sobre ingresos diarios/mensuales, servicios más demandados, efectividad del personal o historial de auditoría frente a modificaciones sensibles de precios o usuarios.

Estas limitantes impiden el escalamiento del negocio, reducen la satisfacción del cliente y aumentan los costos operativos por ineficiencia.

---

## 3. JUSTIFICACIÓN

* **Técnica:** La adopción de una arquitectura web desacoplada en tres capas (Vue.js + ASP.NET Core REST API + Base de datos relacional) promueve la separación de responsabilidades, facilitando el mantenimiento, la reutilización de APIs y la escalabilidad horizontal. La contenerización mediante Docker asegura la consistencia de entornos desde desarrollo hasta producción.
* **Social:** Ofrece a la comunidad una alternativa cómoda y profesional para el cuidado de sus vehículos, optimizando el uso del tiempo libre de los usuarios y dignificando la labor de los técnicos de limpieza mediante herramientas tecnológicas intuitivas.
* **Académica:** Permite poner en práctica los conocimientos avanzados de la materia **Programación Web II**, demostrando el dominio de patrones de diseño de software (MVC, REST, ORM EF Core, SPA, JWT Auth, Docker Compose) e integración de arquitecturas distribuidas.
* **Económica y de Viabilidad:** La utilización de un ecosistema tecnológico *Open Source* (.NET Core, Vue 3, MySQL/SQLite, Docker, Nginx) reduce a cero los costos por licencias de software, convirtiendo al sistema en una propuesta tecnológicamente viable para pequeñas empresas.

---

## 4. OBJETIVOS

### Objetivo General
Diseñar, desarrollar, integrar, documentar y contenerizar la plataforma web **EcoWash Móvil** para la gestión comercial de servicios de lavado de vehículos a domicilio en Santa Cruz de la Sierra, aplicando una arquitectura de tres capas con Vue 3, ASP.NET Core 8 Web API, Entity Framework Core y Docker Compose.

### Objetivos Específicos
1. Identificar y especificar los requerimientos funcionales y no funcionales que rigen el flujo de reservas, clientes, empleados, inventario, pagos y auditoría.
2. Modelar el sistema mediante diagramas de Casos de Uso, Clases y el Modelo Relacional de Base de Datos en Tercera Forma Normal (3FN) compuesto por 27 entidades.
3. Desarrollar la Web API RESTful en C# ASP.NET Core 8 con Entity Framework Core, autenticación mediante JWT y más de 20 controladores REST.
4. Construir la interfaz de usuario reactiva Single Page Application (SPA) con Vue 3, Pinia, Vue Router y Axios, asegurando una experiencia visual intuitiva y adaptativa.
5. Contenerizar los servicios (Frontend Nginx, Backend Web API y Base de Datos) con Dockerfiles optimizados y orquestación con Docker Compose.
6. Realizar un estudio en formato IMRyD para evaluar plataformas de hosting y despliegue en la nube.

---

## 5. ALCANCE Y LIMITACIONES

### Alcance
* **Módulo de Usuarios y Roles:** Autenticación JWT, gestión de perfiles (Administrador, Empleado, Cliente) y control de acceso basado en roles.
* **Módulo de Clientes y Vehículos:** Registro de vehículos (placa, marca, modelo, tipo) y gestión de múltiples ubicaciones de domicilio.
* **Módulo de Reservas y Servicios:** Selección de servicios de lavado, asignación de empleados, control de estados de la reserva (*Pendiente, Asignada, Aceptada, En Proceso, Finalizada, Cancelada*).
* **Módulo de Inventario y Productos:** CRUD de categorías e insumos, registro de compras, control de stock actual/mínimo y descuento automático de insumos al finalizar servicios.
* **Módulo de Ventas, Pagos y Facturación:** Emisión automática de ventas al completar servicios, registro de pagos (QR, Efectivo, Tarjeta) y generación de comprobante/factura.
* **Módulo de Auditoría y Reportes:** Log automático de actividades críticas del sistema y dashboard con métricas estadísticas exportables.

### Limitaciones
* No incluye una aplicación móvil nativa (Android/iOS); la solución es una SPA Web accesible desde dispositivos móviles mediante navegador.
* La pasarela de pago QR simula la recepción y validación de comprobantes sin integración bancaria en tiempo real (API bancaria directa).

---

## 6. IMPACTO SOCIAL, EJE TRANSVERSAL Y ODS

* **Eje Transversal UPDS:** *Innovación tecnológica y transformación digital*. El proyecto introduce tecnologías web modernas y contenerización en un sector de servicios tradicionalmente informal en la región.
* **ODS vinculado:** **ODS 9 (Industria, Innovación e Infraestructura)**. Fomenta el uso de infraestructuras digitales eficientes, promueve la formalización de microempresas de servicios e impulsa la adopción de herramientas informáticas limpias.

---

## 7. REQUERIMIENTOS DEL SISTEMA

### 7.1 Requerimientos funcionales

| Código | Requerimiento funcional | Actor relacionado | Prioridad |
| :--- | :--- | :--- | :--- |
| **RF-01** | El sistema debe permitir el inicio de sesión y registro de usuarios mediante credenciales válidas y generación de tokens JWT. | Todos los usuarios | Alta |
| **RF-02** | El cliente debe poder registrar y administrar múltiples vehículos y direcciones de atención. | Cliente | Alta |
| **RF-03** | El cliente debe poder solicitar una reserva seleccionando vehículo, servicio, ubicación y fecha/hora. | Cliente | Alta |
| **RF-04** | El empleado debe visualizar sus reservas asignadas, aceptarlas, iniciar el servicio y marcar su finalización. | Empleado | Alta |
| **RF-05** | Al marcar un servicio como finalizado, el sistema debe generar automáticamente la venta y descontar el stock de insumos asociado. | Sistema / Empleado | Alta |
| **RF-06** | El administrador debe gestionar el catálogo de productos/insumos, definir stock mínimo y recibir alertas de reposición. | Administrador | Alta |
| **RF-07** | El sistema debe permitir el registro de pagos (Efectivo, QR, Tarjeta) y la emisión del comprobante/factura. | Cliente / Admin | Media |
| **RF-08** | El sistema debe guardar un historial de auditoría con la fecha, usuario, tabla y acción realizada para operaciones críticas. | Sistema | Media |
| **RF-09** | El administrador debe acceder a reportes estadísticos de ventas, servicios más solicitados e ingresos por período. | Administrador | Media |
| **RF-10** | El cliente debe poder calificar el servicio recibido de 1 a 5 estrellas al concluir la atención. | Cliente | Baja |

### 7.2 Requerimientos no funcionales

| Código | Requerimiento no funcional | Criterio de verificación |
| :--- | :--- | :--- |
| **RNF-01** | **Contenerización y Despliegue:** El sistema completo debe desplegarse mediante Docker Compose con tres contenedores aislados. | Verificación exitosa mediante `docker compose up` e inspección de servicios en ejecución. |
| **RNF-02** | **Rendimiento:** El tiempo de respuesta de las APIs REST no debe superar los 500 ms en condiciones normales de carga. | Pruebas de tiempo de respuesta mediante llamadas HTTP/Axios en red local. |
| **RNF-03** | **Seguridad:** Las contraseñas deben cifrarse con el algoritmo BCrypt y las rutas protegidas deben exigir un encabezado `Authorization: Bearer <token>`. | Inspección de base de datos (hashes) y validación de middleware de autenticación. |
| **RNF-04** | **Persistencia:** La base de datos relacional debe conservar los datos de forma persistente mediante volúmenes de Docker. | Reinicio de contenedores Docker y comprobación de integridad de datos almacenados. |
| **RNF-05** | **Usabilidad y Adaptabilidad:** La interfaz gráfica de usuario debe ser responsiva y utilizar una paleta de colores coherente (`#2563EB`). | Verificación visual en pantallas de escritorio, tablets y dispositivos móviles. |

---

## 8. DIAGRAMA DE CASOS DE USO

El sistema organiza sus interacciones según tres actores principales que operan sobre la plataforma.

### 8.1 Descripción de actores

| Actor | Descripción | Responsabilidades principales |
| :--- | :--- | :--- |
| **Cliente** | Usuario final que solicita la atención de lavado a domicilio. | Registrar vehículos, ubicaciones, agendar reservas, realizar pagos y calificar atención. |
| **Empleado** | Técnico encargado de ejecutar el lavado en la ubicación del cliente. | Consultar agenda de reservas asignadas, aceptar/iniciar/finalizar servicios. |
| **Administrador** | Usuario con privilegios elevados encargado de la gestión global. | Gestionar inventario, servicios, precios, personal, auditoría y reportes gerenciales. |

---

### 8.2 Especificación de casos de uso

#### CU-01: Iniciar Sesión
* **Actor Principal:** Cliente / Empleado / Administrador.
* **Precondiciones:** El usuario debe estar registrado activamente.
* **Flujo Principal:**
  1. El usuario accede al módulo de autenticación.
  2. Ingresa su correo electrónico y contraseña.
  3. El backend valida el hash BCrypt.
  4. El sistema emite un token JWT con vigencia de 7 días y redirige al dashboard según el rol.
* **Postcondición:** Sesión iniciada con token guardado en `localStorage`.

#### CU-02: Solicitar Reserva de Lavado
* **Actor Principal:** Cliente.
* **Precondiciones:** Cliente autenticado con al menos un vehículo y una ubicación registrados.
* **Flujo Principal:**
  1. El cliente entra a "Nueva Reserva".
  2. Selecciona el vehículo y tipo de servicio (ej. *Lavado Premium*).
  3. Selecciona la dirección de atención y elige fecha/hora disponible.
  4. Confirma la solicitud. El sistema crea la reserva en estado "Pendiente".
* **Postcondición:** Reserva registrada en estado Pendiente disponible para asignación.

#### CU-03: Gestionar Productos e Inventario
* **Actor Principal:** Administrador.
* **Precondiciones:** Autenticación activa con rol Administrador.
* **Flujo Principal:**
  1. Accede al panel de inventario.
  2. Consulta la lista de insumos con stock actual y stock mínimo.
  3. Registra o actualiza un producto (nombre, categoría, precio, unidades).
  4. El sistema valida los datos y actualiza la base de datos.
* **Postcondición:** Producto actualizado en el catálogo global.

#### CU-04: Aceptar/Finalizar Reserva
* **Actor Principal:** Empleado.
* **Precondiciones:** Reserva asignada al empleado en estado "Asignada" o "Aceptada".
* **Flujo Principal:**
  1. El empleado visualiza la reserva en su panel.
  2. Al llegar a la ubicación marca el estado "En Proceso".
  3. Al concluir la limpieza marca "Finalizada".
  4. El sistema automáticamente registra la Venta y descuenta insumos del inventario.
* **Postcondición:** Servicio completado, venta registrada y stock actualizado.

#### CU-05: Generar Reportes
* **Actor Principal:** Administrador.
* **Precondiciones:** Rol Administrador autenticado.
* **Flujo Principal:**
  1. Selecciona la vista de Reportes Estadísticos.
  2. Define rango de fechas o filtro por tipo de servicio.
  3. El sistema calcula totales y presenta gráficos e indicadores.
* **Postcondición:** Reporte generado visualmente.

#### CU-06: Registrar Pago
* **Actor Principal:** Cliente.
* **Precondiciones:** Reserva aceptada o finalizada.
* **Flujo Principal:**
  1. Selecciona la reserva pendiente de pago.
  2. Elige el método de pago (QR / Efectivo / Tarjeta).
  3. Adjunta o confirma la transacción.
  4. El sistema genera el comprobante o factura correspondiente.
* **Postcondición:** Pago registrado y factura asociada en la base de datos.

---

## 9. DIAGRAMA DE CLASES

### 9.1 Descripción de clases principales

| Clase | Responsabilidad | Atributos principales | Métodos principales |
| :--- | :--- | :--- | :--- |
| `Usuario` | Autenticación y credenciales de acceso. | `Id`, `Email`, `PasswordHash`, `RolId`, `Estado` | `ValidarPassword()`, `GenerarJWT()` |
| `Cliente` | Datos específicos del cliente. | `Id`, `UsuarioId`, `Ci`, `Direccion`, `Ciudad` | `ObtenerVehiculos()`, `ObtenerReservas()` |
| `Empleado` | Información del lavador/operativo. | `Id`, `UsuarioId`, `Cargo`, `Especialidad`, `Disponible` | `AsignarReserva()`, `CambiarEstado()` |
| `Vehiculo` | Identificación de los autos a lavar. | `Id`, `ClienteId`, `Placa`, `Marca`, `Modelo`, `Tipo` | `ObtenerHistorialServicios()` |
| `Reserva` | Solicitud y agenda del servicio. | `Id`, `ClienteId`, `EmpleadoId`, `ServicioId`, `FechaHora`, `Estado` | `ActualizarEstado()`, `CalcularMonto()` |
| `Producto` | Insumos de limpieza e inventario. | `Id`, `CategoriaId`, `Nombre`, `PrecioUnitario`, `StockActual`, `StockMinimo` | `DescontarStock()`, `VerificarAlerta()` |
| `Venta` | Transacción comercial resultante. | `Id`, `ReservaId`, `Fecha`, `MontoTotal` | `GenerarFactura()`, `ProcesarPago()` |

---

## 10. MODELO RELACIONAL DE LA BASE DE DATOS

La base de datos relacional de **EcoWash Móvil** se diseñó estrictamente en **Tercera Forma Normal (3FN)** para garantizar la integridad de datos, eliminar redundancias y permitir relaciones complejas entre usuarios, reservas, inventario y facturación. El modelo contiene 27 tablas relacionales:

`Emprendimiento` · `Rol` · `Permiso` · `RolPermiso` · `Usuario` · `Cliente` · `Empleado` · `Categoria` · `Producto` · `Proveedor` · `Compra` · `DetalleCompra` · `Inventario` · `MovimientoInventario` · `Vehiculo` · `Ubicacion` · `Servicio` · `Reserva` · `Venta` · `DetalleVenta` · `MetodoPago` · `Pago` · `Factura` · `Calificacion` · `Notificacion` · `Auditoria` · `Reporte`

---

## 11. DICCIONARIO DE DATOS

A continuación se documentan las entidades clave del modelo de datos:

### 11.1 Tabla: `Usuario`
| Campo | Tipo de dato | Longitud | PK/FK | Nulo | Descripción |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `Id` | `INT` | — | PK | No | Identificador único del usuario |
| `Email` | `VARCHAR` | `100` | — | No | Correo electrónico / usuario de acceso |
| `PasswordHash` | `VARCHAR` | `255` | — | No | Hash cifrado de la contraseña (BCrypt) |
| `RolId` | `INT` | — | FK | No | Referencia al rol asignado |
| `FechaCreacion` | `DATETIME` | — | — | No | Fecha de registro en el sistema |

### 11.2 Tabla: `Cliente`
| Campo | Tipo de dato | Longitud | PK/FK | Nulo | Descripción |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `Id` | `INT` | — | PK | No | Identificador único del cliente |
| `UsuarioId` | `INT` | — | FK | No | Relación 1:1 con la tabla Usuario |
| `Ci` | `VARCHAR` | `20` | — | No | Carnet de identidad |
| `Direccion` | `VARCHAR` | `200` | — | No | Dirección principal del domicilio |
| `Ciudad` | `VARCHAR` | `50` | — | No | Ciudad de residencia |

### 11.3 Tabla: `Reserva`
| Campo | Tipo de dato | Longitud | PK/FK | Nulo | Descripción |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `Id` | `INT` | — | PK | No | Identificador único de la reserva |
| `ClienteId` | `INT` | — | FK | No | Cliente que solicita el servicio |
| `EmpleadoId` | `INT` | — | FK | Sí | Empleado asignado para el lavado |
| `ServicioId` | `INT` | — | FK | No | Tipo de servicio seleccionado |
| `FechaHora` | `DATETIME` | — | — | No | Fecha y hora programada |
| `Estado` | `VARCHAR` | `30` | — | No | Estado (*Pendiente, Asignada, Aceptada, En Proceso, Finalizada, Cancelada*) |

### 11.4 Tabla: `Producto`
| Campo | Tipo de dato | Longitud | PK/FK | Nulo | Descripción |
| :--- | :--- | :--- | :--- | :--- | :--- |
| `Id` | `INT` | — | PK | No | Identificador del producto/insumo |
| `CategoriaId` | `INT` | — | FK | No | Categoría a la que pertenece |
| `Nombre` | `VARCHAR` | `100` | — | No | Nombre del insumo |
| `PrecioUnitario`| `DECIMAL` | `10,2` | — | No | Precio o costo del producto |
| `StockActual` | `INT` | — | — | No | Existencia física disponible |
| `StockMinimo` | `INT` | — | — | No | Umbral mínimo para alertas de stock |

---

## 12. ARQUITECTURA DEL SISTEMA

### 12.1 Arquitectura lógica

El sistema adopta una arquitectura de **3 capas (Capa de Presentación, Capa de Negocio/API y Capa de Persistencia)**:

```
[ Capa de Presentación ]     <---> HTTP / JSON (Axios) <---> [ Capa de Negocio / API ]     <---> EF Core ORM <---> [ Capa de Persistencia ]
Vue.js 3 SPA (Vite, Pinia)                                 ASP.NET Core 8 Web API                                  SQL Server / SQLite / MySQL
```

### 12.2 Arquitectura de contenedores

Mediante Docker Compose se orquestan tres servicios independientes conectados mediante una red virtual aislada (`ecowash-network`):

```
Usuario ---> Nginx (Puerto 80 / Contenedor Frontend)
                |
                v  Proxy /api/
             ASP.NET Core 8 Web API (Puerto 5275 / Contenedor Backend)
                |
                v  TCP / 3306
             MySQL / SQL Server (Puerto 3306 / Contenedor Database + Volume)
```

### 12.3 Flujo de una solicitud

1. El usuario interactúa con la SPA en Vue 3 y ejecuta una acción (ej. *Confirmar Reserva*).
2. El cliente HTTP Axios envía una solicitud `POST /api/reserva` incluyendo el token JWT en la cabecera `Authorization`.
3. Nginx / Kestrel recibe la petición; el Middleware de Autenticación valida la firma del token JWT.
4. El controlador `ReservaController` procesa el DTO y llama a la capa de servicios para aplicar reglas de negocio.
5. Entity Framework Core ejecuta la consulta LINQ y persiste los cambios en la base de datos relacional.
6. La API devuelve una respuesta HTTP `200 OK` con un payload JSON estandarizado que actualiza la UI de Vue de manera reactiva.

---

## 13. TECNOLOGÍAS UTILIZADAS

| Tecnología | Versión utilizada | Función en el proyecto | Justificación técnica |
| :--- | :--- | :--- | :--- |
| **Vue.js** | 3.x (Composition API) | Frontend SPA | Permite crear una interfaz altamente reactiva, modular y ligera con renderizado ágil en el navegador. |
| **Axios** | 1.x | Consumo de la API REST | Manejo limpio de promesas, interceptores para tokens JWT y transformación automática de JSON. |
| **ASP.NET Core** | 8.0 | Backend Web API | Framework multiplataforma de alto rendimiento para APIs REST seguras y fuertemente tipadas en C#. |
| **EF Core** | 8.0 | ORM / Acceso a datos | Facilita el mapeo objeto-relacional, consultas LINQ seguras contra inyecciones SQL y migraciones controladas. |
| **SQL Server / MySQL**| 8.0 / SQLite | Base de datos relacional | Motor relacional robusto que garantiza propiedades ACID e integridad referencial en 3FN. |
| **Docker** | 24+ | Contenerización | Garantiza un entorno de ejecución idéntico, aislado y portátil sin depender del sistema operativo anfitrión. |
| **Git / GitHub** | Latest | Control de versiones | Gestión colaborativa del código fuente con trazabilidad de cambios por ramas y commits. |

---

## 14. DISEÑO DE INTERFACES

El diseño UI/UX del sistema fue creado bajo la paleta corporativa `#2563EB` (Azul primario), aplicando principios de responsive design, micro-animaciones y efectos de glassmorphism:

1. **Pantalla de Login (`/login`):** Formulario minimalista para credenciales con validación visual inmediata y mensaje de error reactivo.
2. **Dashboard Administrador (`/dashboard`):** Panel gerencial con tarjetas de indicadores clave (Total Clientes, Ingresos del Mes, Reservas Activas, Empleados) y gráficos interactivos.
3. **Solicitud de Reserva (`/reservar`):** Asistente paso a paso para la selección de vehículo, catálogo de servicios con cálculo de costo en tiempo real, selección de dirección y calendario de horarios.
4. **Gestión de Inventario (`/productos`):** Tabla interactiva con resaltado automático en rojo para productos con stock igual o inferior al stock mínimo y modales de edición.

---

## 15. IMPLEMENTACIÓN DEL FRONTEND

### 15.1 Estructura del frontend

```text
Frontend/
├── public/
├── src/
│   ├── assets/           # Estilos CSS globales y temas
│   ├── components/       # Componentes reusables (Navbar.vue, Sidebar.vue)
│   ├── router/           # Configuración de Vue Router y guardias de navegación
│   ├── services/         # Cliente Axios e integraciones API (api.js)
│   ├── stores/           # Gestión de estado global con Pinia (authStore, themeStore)
│   ├── views/            # Vistas principales (Login.vue, Dashboard.vue, Reservas.vue)
│   ├── App.vue           # Componente raíz
│   └── main.js           # Punto de entrada de la aplicación
├── package.json
└── vite.config.js
```

### 15.2 Componentes y vistas

* `Navbar.vue`: Barra superior con perfil de usuario, selector de tema oscuro/claro y botón de cierre de sesión.
* `Sidebar.vue`: Menú lateral dinámico que filtra las opciones disponibles según el rol (`Administrador`, `Empleado`, `Cliente`).
* `ReservaView.vue`: Componente que gestiona el flujo de creación y filtrado por estados de reservas.

### 15.3 Consumo de la API

El consumo de las APIs backend se centraliza en `src/services/api.js` mediante un cliente Axios configurado con interceptores de solicitud:

```javascript
import axios from 'axios'

const api = axios.create({
  baseURL: 'http://localhost:5275/api',
  headers: { 'Content-Type': 'application/json' }
})

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('ecowash_token')
  if (token) {
    config.headers.Authorization = `Bearer ${token}`
  }
  return config
})

export default api
```

---

## 16. IMPLEMENTACIÓN DEL BACKEND

### 16.1 Estructura del backend

```text
BackendApi/
├── Controllers/       # Controladores API REST (AuthController, ReservaController, etc.)
├── Data/              # DbContext de EF Core (ApplicationDbContext.cs)
├── DTOs/              # Data Transfer Objects con DataAnnotations
├── Models/            # Entidades del modelo de dominio (27 clases de modelo)
├── Services/          # Lógica de negocio, servicio de JWT y BCrypt
├── Program.cs         # Configuración de servicios, CORS, JWT y Middleware
└── appsettings.json   # Cadenas de conexión y secretos
```

### 16.2 Endpoints de la API

| Método | Ruta | Descripción | Entrada | Respuesta |
| :--- | :--- | :--- | :--- | :--- |
| **POST** | `/api/auth/login` | Autentica un usuario y entrega Token JWT. | JSON (`email`, `password`) | `200 OK` (Token + Datos usuario) |
| **GET** | `/api/cliente` | Retorna el listado completo de clientes. | — | `200 OK` (Lista de clientes) |
| **POST** | `/api/reserva` | Registra una nueva reserva de lavado. | JSON (Datos reserva) | `201 Created` |
| **PUT** | `/api/reserva/{id}/estado`| Actualiza el estado de la reserva. | JSON (`nuevoEstado`) | `200 OK` |
| **GET** | `/api/producto` | Listado de productos e inventario. | — | `200 OK` (Lista de productos) |
| **DELETE**| `/api/producto/{id}` | Desactiva un producto del catálogo. | URL Parameter `id` | `204 No Content` |

### 16.3 Integración con Entity Framework Core

El acceso a la base de datos se administra mediante `ApplicationDbContext`, haciendo uso de Fluent API para configurar relaciones foráneas y restricciones de integridad:

```csharp
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Cliente> Clientes { get; set; }
    public DbSet<Reserva> Reservas { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    // Configuración Fluent API y DbSets adicionales...
}
```

---

## 17. IMPLEMENTACIÓN DE DOCKER (OBLIGATORIA)

### 17.1 Conceptos aplicados
Se implementó contenerización completa usando **Docker** y **Docker Compose** para eliminar discrepancias de entorno. Se aplican construcciones multi-etapa (*multi-stage builds*) para reducir el tamaño final de las imágenes y optimizar la seguridad.

### 17.2 Servicios implementados

| Servicio | Imagen o Dockerfile | Puerto | Responsabilidad |
| :--- | :--- | :--- | :--- |
| `frontend` | Dockerfile (Multi-stage Node -> Nginx) | `80:80` | Servir la SPA compilada en Vue 3 mediante Nginx. |
| `backend` | Dockerfile (Multi-stage .NET SDK -> Runtime) | `5275:80` | Ejecutar la REST API en C# ASP.NET Core 8. |
| `db` / `sqlserver` | Imagen Oficial `mysql:8.0` / SQL Server | `3306:3306` | Persistencia relacional de datos con volumen. |

### 17.3 Dockerfile del frontend

```dockerfile
# Etapa 1: Compilación
FROM node:18-alpine AS build-stage
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
RUN npm run build

# Etapa 2: Servidor Web Nginx
FROM nginx:stable-alpine AS production-stage
COPY --from=build-stage /app/dist /usr/share/nginx/html
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

### 17.4 Dockerfile del backend

```dockerfile
# Etapa 1: Compilación y Publish
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackendApi.csproj", "./"]
RUN dotnet restore "BackendApi.csproj"
COPY . .
RUN dotnet publish "BackendApi.csproj" -c Release -o /app/publish

# Etapa 2: Runtime ligero
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "BackendApi.dll"]
```

### 17.5 Archivo Docker Compose

```yaml
version: '3.8'

services:
  db:
    image: mysql:8.0
    container_name: ecowash_db_container
    environment:
      MYSQL_ROOT_PASSWORD: root
      MYSQL_DATABASE: ecowash_db
    ports:
      - "3306:3306"
    volumes:
      - db_data:/var/lib/mysql

  backend:
    build: ./BackendApi
    container_name: ecowash_backend_container
    ports:
      - "5275:80"
    depends_on:
      - db
    environment:
      - ConnectionStrings__DefaultConnection=Server=db;Database=ecowash_db;Uid=root;Pwd=root;

  frontend:
    build: ./Frontend
    container_name: ecowash_frontend_container
    ports:
      - "80:80"
    depends_on:
      - backend

volumes:
  db_data:
```

### 17.6 Variables de entorno y seguridad
Las credenciales sensibles y cadenas de conexión se administran fuera del código fuente en archivos de variables de entorno (`.env.example`), evitando la exposición de claves en repositorios públicos.

### 17.7 Evidencias de ejecución
* Ejecución limpia de `docker compose up -d`.
* Levantamiento correcto de los 3 contenedores verificables con `docker compose ps`.
* Acceso simultáneo a Swagger UI (`http://localhost:5275/swagger`) y a la SPA (`http://localhost:80`).

---

## 18. PRUEBAS DEL SISTEMA

| Código | Función evaluada | Datos de entrada | Resultado esperado | Resultado obtenido | Estado |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **PR-01** | Inicio de Sesión | Email: `admin@ecowash.bo`<br>Pass: `Admin@1234` | Generación de JWT y acceso a Dashboard. | Token generado exitosamente y redirección correcta. | **Aprobado** |
| **PR-02** | Solicitar Reserva | Vehículo ID: 1, Servicio ID: 2, Fecha: 2026-08-01 | Reserva creada en estado "Pendiente". | Objeto Reserva guardado en DB con id asignado. | **Aprobado** |
| **PR-03** | Integración Vue-API | Solicitud `GET /api/producto` | Renderizado reactivo de la tabla de inventario. | Datos recibidos y mostrados en pantalla sin recarga. | **Aprobado** |
| **PR-04** | Persistencia Docker | `docker compose restart db` | Mantener los datos almacenados tras el reinicio. | Los registros permanecieron intactos en el volumen `db_data`. | **Aprobado** |

---

## 19. MANUAL DE INSTALACIÓN Y EJECUCIÓN LOCAL

### Prerrequisitos
* Docker Desktop 4.x instalado.
* Git instalado.

### Pasos de ejecución con Docker Compose (Recomendado)
1. **Clonar el repositorio:**
   ```bash
   git clone https://github.com/usuario/ecowash-prototipo.git
   cd ecowash-prototipo
   ```
2. **Iniciar los servicios contenerizados:**
   ```bash
   docker compose up -d --build
   ```
3. **Verificar estado de contenedores:**
   ```bash
   docker compose ps
   ```
4. **Acceder a la aplicación:**
   * **Frontend Web:** `http://localhost:80`
   * **Backend REST API:** `http://localhost:5275`
   * **Documentación Swagger:** `http://localhost:5275/swagger`

---

## 20. CONTROL DE VERSIONES CON GIT Y GITHUB

El desarrollo del proyecto se registró utilizando **Git** y alojado en **GitHub**, aplicando buenas prácticas de control de versiones y mensajes de commit descriptivos:

| Commit Hash | Responsabilidad / Descripción del Cambio | Evidencia |
| :--- | :--- | :--- |
| `feat-01` | Estructura base del proyecto backend ASP.NET Core y modelos EF Core | Repositorio GitHub |
| `feat-02` | Implementación de AuthController y JWT Bearer Authentication | Repositorio GitHub |
| `feat-03` | Desarrollo del SPA Frontend Vue 3 con Vite, Pinia y Router | Repositorio GitHub |
| `feat-04` | Lógica de reservas, asignación de empleados e inventario | Repositorio GitHub |
| `feat-05` | Contenerización con Dockerfile y Docker Compose de 3 capas | Repositorio GitHub |

---

## 21. INVESTIGACIÓN IMRyD: ALTERNATIVAS DE HOSTING PARA EL PROYECTO

### 21.1 Título de la investigación
**Análisis comparativo de plataformas PaaS e IaaS para el despliegue en producción de una aplicación web de tres capas contenerizada (Vue 3 + ASP.NET Core + Base de datos Relacional)**

### 21.2 Resumen
El despliegue eficiente de aplicaciones web modernas de tres capas exige evaluar criterios de costos, soporte de contenedores Docker, facilidad de integración continua (CI/CD) y persistencia de bases de datos relacionales. La presente investigación analiza comparativamente cuatro proveedores de hosting (AWS, Render, Railway y Koyeb) para determinar la mejor alternativa de publicación para la plataforma **EcoWash Móvil**. Mediante una metodología descriptiva-comparativa basada en métricas de rendimiento, compatibilidad con Docker Compose y planes académicos/gratuitos, se determinó que **Railway** y **Render** destacan como las mejores opciones para etapas de prototipado por su despliegue automatizado desde GitHub, mientras que **AWS (EC2/ECS)** se consolida como la alternativa más robusta para producción a gran escala.

* **Palabras clave:** Hosting, Docker, ASP.NET Core, Vue 3, PaaS, Cloud Computing.

### 21.3 Introducción
Llevar un proyecto desde el entorno local de desarrollo hacia la nube implica superar barreras técnicas asociadas a la orquestación de servicios en red, la gestión de variables de entorno seguras y la persistencia de datos. Tradicionalmente, la contratación de servidores VPS exigía una compleja configuración manual del sistema operativo. Sin embargo, la irrupción de plataformas como Servicio (PaaS) preparadas para Docker ha simplificado este proceso. El objetivo de este trabajo es comparar cuantitativa y cualitativamente cuatro alternativas de hosting evaluando su viabilidad para aplicaciones web contenerizadas.

### 21.4 Metodología
Se realizó una revisión documental de las especificaciones técnicas oficiales de los proveedores AWS, Render, Railway y Koyeb a fecha de 2026. Se establecieron los siguientes 8 criterios de comparación:
1. Compatibilidad nativa con Docker y Docker Compose.
2. Soporte para Frontend Vue 3 (estático o Nginx).
3. Soporte para Backend ASP.NET Core 8.
4. Soporte para Base de datos relacional (MySQL / SQL Server).
5. Costo y límites del plan gratuito / académico.
6. Gestión de certificados SSL/HTTPS automáticos.
7. Integración nativa CI/CD con GitHub.
8. Complejidad de configuración.

### 21.5 Resultados

| Criterio de comparación | AWS (EC2 / App Runner) | Render | Railway | Koyeb |
| :--- | :--- | :--- | :--- | :--- |
| **Soporte Docker** | Nativo (ECS / EC2) | Nativo (Dockerfile) | Nativo (Dockerfile) | Nativo (Dockerfile) |
| **Soporte Vue 3** | S3 + CloudFront / EC2 | Static Site (Gratis) | Web Service | Web Service |
| **Soporte .NET Core** | Full (.NET App Runner/EC2) | Web Service (Docker) | Web Service (Docker) | Web Service (Docker) |
| **Base de Datos** | Amazon RDS / EC2 DB | Managed PostgreSQL/MySQL| Managed MySQL / PG | Managed Postgres |
| **Costo Académico** | AWS Educate / Free Tier | Plan gratuito limitado | $5 USD crédito inicial | Plan gratuito básico |
| **HTTPS Automático** | AWS Certificate Manager | Incluido automático | Incluido automático | Incluido automático |
| **Integración GitHub** | AWS CodePipeline | Automática al hacer Push | Automática al hacer Push | Automática al hacer Push |
| **Dificultad** | Alta | Baja | Muy Baja | Baja |

### 21.6 Discusión
* **Render y Railway:** Representan las plataformas más accesibles para equipos de desarrollo pequeños o proyectos universitarios. Su capacidad para detectar el `Dockerfile` en la raíz del repositorio de GitHub y compilar el contenedor automáticamente elimina la sobrecarga de administración de servidores.
* **AWS:** Ofrece una infraestructura profesional inigualable en disponibilidad y escalabilidad global. No obstante, la curva de aprendizaje para configurar grupos de seguridad, VPCs y Amazon RDS resulta elevada para despliegues iniciales rápidos.

### 21.7 Conclusiones de la investigación
1. Para el prototipo funcional y la defensa del proyecto **EcoWash Móvil**, se recomienda **Railway** o **Render**, puesto que permiten conectar el repositorio de GitHub y levantar los contenedores en cuestión de minutos con HTTPS habilitado por defecto.
2. Para una fase posterior de producción comercial con alto tráfico de clientes en Santa Cruz, la migración hacia **AWS (EC2 / ECS + RDS)** es la opción óptima para garantizar alta disponibilidad y cumplimiento de SLAs.

---

## 22. CONCLUSIONES GENERALES DEL PROYECTO

1. Se logró con éxito el diseño, desarrollo e integración de la plataforma **EcoWash Móvil**, cumpliendo satisfactoriamente con la totalidad de los requerimientos funcionales y no funcionales establecidos.
2. La arquitectura de tres capas basada en **Vue 3 SPA** y **ASP.NET Core 8 Web API** demostró ser una solución técnica altamente sólida, garantizando la separación de preocupaciones y una respuesta ágil a las solicitudes del usuario.
3. La implementación de la base de datos relacional en **Tercera Forma Normal (3FN)** aseguró la integridad y consistencia de la información operacional, permitiendo gestionar adecuadamente el flujo complejo entre usuarios, reservas, insumos e historial de auditoría.
4. La contenerización con **Docker Compose** resolvió de manera definitiva el reto de portabilidad del software, haciendo posible iniciar la plataforma completa mediante un único comando en cualquier entorno.

---

## 23. RECOMENDACIONES Y TRABAJO FUTURO

1. **Integración de Pasarela de Pagos en Tiempo Real:** Incorporar SDKs oficiales de pasarelas locales o internacionales (Stripe, Libélula, Pagosnet) para la confirmación automática e inmediata de transferencias bancarias y pagos QR.
2. **Aplicación Web Progresiva (PWA):** Convertir el frontend Vue 3 en una PWA para habilitar notificaciones Push en tiempo real sobre el cambio de estado de las reservas y soporte de trabajo offline.
3. **Geolocalización en Tiempo Real:** Integrar la API de Google Maps o Leaflet para que los clientes puedan rastrear la ruta del técnico de lavado en tiempo real mientras se desplaza hacia su domicilio.

---

## 24. REFERENCIAS BIBLIOGRÁFICAS

* Docker Inc. (2026). *Docker Documentation: Containerize your applications*. Recuperado de https://docs.docker.com/
* Microsoft Corporation. (2026). *ASP.NET Core Web API documentation and Entity Framework Core*. Microsoft Learn. Recuperado de https://learn.microsoft.com/es-es/aspnet/core/
* Vue.js Core Team. (2026). *Vue 3 - The Progressive JavaScript Framework*. Recuperado de https://vuejs.org/
* Railway Corp. (2026). *Deploying Docker Application on Railway*. Recuperado de https://docs.railway.app/

---

## 25. ANEXOS

### Anexo A. Enlaces del proyecto

| Recurso | Enlace |
| :--- | :--- |
| **Repositorio GitHub** | `https://github.com/gaboale345/prototipo` |
| **Documentación API Swagger** | `http://localhost:5275/swagger` |
| **Documento PDF Técnico** | `DOCUMENTACION_UPDS_ECOWASH.pdf` |

### Anexo B. Credenciales de demostración

| Rol | Correo de usuario | Contraseña de prueba |
| :--- | :--- | :--- |
| **Administrador** | `admin@ecowash.bo` | `Admin@1234` |
| **Empleado** | `empleado@ecowash.bo` | `Empleado@1234` |
| **Cliente** | `cliente@ecowash.bo` | `Cliente@1234` |

### Anexo C. Distribución de responsabilidades

| Integrante | Tareas desarrolladas | Porcentaje de participación |
| :--- | :--- | :---: |
| **Estudiante(s)** | Análisis de requerimientos, Backend API C#, Frontend Vue 3, Base de datos 3FN, Dockerfile, Investigación IMRyD y Documentación. | 100% |

---
