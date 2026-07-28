# 🐳 Fundamentos de Docker y Contenerización de Proyecto — EcoWash Móvil

**Universidad Privada Domingo Savio (UPDS)**  
**Materia:** Programación Web II — Santa Cruz de la Sierra, Bolivia

---

## 📌 1. El Problema del "En mi computadora funciona"

Al desarrollar aplicaciones web en equipo o al desplegar en producción, surgen discrepancias entre entornos:

| Entorno | Computadora A (Desarrollador 1) | Computadora B (Desarrollador 2 / Servidor) | Resultado |
|---------|--------------------------------|-------------------------------------------|-----------|
| **Node.js** | v18.3 | v14.1 | ❌ Conflicto de versión |
| **Base de Datos** | MySQL 8.0 | MySQL 5.7 | ❌ Incompatibilidad de SQL |
| **Extensiones / Libs** | Correctamente instaladas | Incompletas / Faltantes | ❌ Error de ejecución |
| **Resultado** | **PROYECTO FUNCIONA** | **PROYECTO FALLA** | **Inconsistencia** |

### Causas comunes de diferencias entre entornos:
- Diferentes versiones de lenguajes o entornos (PHP, Node.js, .NET SDK).
- Extensiones o módulos de dependencias faltantes.
- Configuración diferente en el gestor de base de datos.
- Archivos de variables de entorno `.env` desactualizados.
- Problemas con servidores locales tradicionales (XAMPP, WAMP).
- Incompatibilidades por diferencias en el Sistema Operativo (Windows, Linux, macOS).

> 💡 **La solución de Docker:** Reduce este tipo de diferencias creando un entorno controlado, aislado y reproducible.

---

## 🏗️ 2. ¿Qué es Docker y cómo funciona?

**Docker** es una plataforma que permite empaquetar una aplicación junto con todas sus dependencias y configuraciones necesarias para ejecutarla de manera uniforme en diferentes computadoras o servidores.

```
┌─────────────────────────────────────────────────────────────────┐
│                          DOCKER CLIENT                          │
│               docker build  │  docker pull  │  docker run        │
└────────────────────────────────┬────────────────────────────────┘
                                 │
┌────────────────────────────────▼────────────────────────────────┐
│                       DOCKER DAEMON (HOST)                      │
│   ┌───────────────────────────┐   ┌───────────────────────────┐ │
│   │        Contenedores       │   │          Imágenes         │ │
│   │ [Vue.js] [API] [Database] │   │ [nginx] [.NET] [mysql:8]  │ │
│   └───────────────────────────┘   └───────────────────────────┘ │
└────────────────────────────────┬────────────────────────────────┘
                                 │
┌────────────────────────────────▼────────────────────────────────┐
│                         DOCKER REGISTRY                         │
│                    (Docker Hub Registry)                        │
└─────────────────────────────────────────────────────────────────┘
```

> ⚠️ **Aclaración importante:** Docker **NO** convierte la aplicación en otro tipo de arquitectura. ASP.NET Core y Vue.js continúan funcionando normalmente, pero dentro de un **ambiente aislado**.

```
  Dockerfile  ──────(BUILD)──────►  Docker Image  ──────(RUN)──────►  Docker Container
 (Instrucciones)                    (Plantilla)                      (Instancia viva)
```

---

## 📊 3. Máquinas Virtuales vs. Contenedores

```
 ┌──────────────────────────┐    ┌──────────────────────────┐
 │       APP 1   APP 2      │    │       APP 1   APP 2      │
 ├──────────────────────────┤    ├──────────────────────────┤
 │     Guest OS (Linux)     │    │  Docker Engine / Daemon  │
 ├──────────────────────────┤    ├──────────────────────────┤
 │        Hypervisor        │    │    Host OS (Windows/Linux)│
 ├──────────────────────────┤    ├──────────────────────────┤
 │    Physical Hardware     │    │    Physical Hardware     │
 └──────────────────────────┘    └──────────────────────────┘
      MÁQUINA VIRTUAL                    CONTENEDOR
```

### Tabla Comparativa:

| Característica | Máquina Virtual | Contenedor |
|----------------|-----------------|------------|
| **Sistema Operativo completo** | Sí (Incluye Kernel independiente) | No necesariamente (Comparte el Kernel anfitrión) |
| **Tiempo de Inicio** | Mayor (Minutos) | Menor (Segundos) |
| **Consumo de Recursos** | Alto (Memoria y CPU reservados) | Generalmente menor y dinámico |
| **Portabilidad** | Buena | Muy buena (Garantizada por la imagen) |
| **Uso en Proyectos Web** | Posible | Muy frecuente (Estándar de la industria) |
| **Aislamiento** | Completo (Nivel de Hardware) | A nivel de procesos del SO |

---

## 🧩 4. Principio de Contenedores del Proyecto EcoWash

En el proyecto **EcoWash Móvil** se aplican principios de responsabilidad única (Single Responsibility Principle) dividiendo el sistema en **3 contenedores principales**:

```
                       ┌─────────────────────────┐
                       │   Navegador Web (User)  │
                       └────────────┬────────────┘
                                    │ HTTP :80 / :5173
                       ┌────────────▼────────────┐
                       │   CONTENEDOR 1: FRONTEND│
                       │   Vue 3 + Vite + Nginx  │
                       │   - Renderizado JS SPA  │
                       │   - Validaciones UI     │
                       │   - Servir estáticos    │
                       └────────────┬────────────┘
                                    │ Proxy HTTP /api/ (Red Interna)
                       ┌────────────▼────────────┐
                       │   CONTENEDOR 2: BACKEND │
                       │   C# ASP.NET Core 8 API │
                       │   - Controladores REST  │
                       │   - JWT Auth / Business │
                       │   - DTOs & Services     │
                       └────────────┬────────────┘
                                    │ DB Connection (Puerto 3306)
                       ┌────────────▼────────────┐
                       │   CONTENEDOR 3: BASE BD │
                       │   MySQL 8.0 Database    │
                       │   - Tablas 3FN          │
                       │   - Persistencia Datos  │
                       └─────────────────────────┘
```

### Responsabilidad de cada Contenedor:

1. **Contenedor 1 — Frontend (Vue.js + Nginx):**
   - Ejecuta código JavaScript del lado del cliente.
   - Procesa validaciones de formularios UI.
   - Controladores y modelos reactivos de la interfaz.
   - Escucha en el puerto HTTP (80/5173) y realiza proxy inverso hacia el backend API.

2. **Contenedor 2 — Backend API (ASP.NET Core 8):**
   - Procesa la lógica de negocio y endpoints REST API.
   - Valida JWT Tokens de autenticación y autorizaciones por rol.
   - Se comunica con la base de datos mediante Entity Framework Core.

3. **Contenedor 3 — Base de Datos (MySQL 8.0):**
   - Se encarga de guardar usuarios, vehículos, servicios y reservas.
   - Mantiene el esquema relacional en Tercera Forma Normal (3FN).
   - Conserva los registros y datos financieros/inventario.

---

## ⚙️ 5. Dockerfile, Volúmenes y Redes Internas

### Dockerfile
Archivo que contiene las instrucciones para construir una **imagen personalizada** del servicio:
- **Backend (`BackendApi/Dockerfile`):** Multi-stage build con SDK .NET 8 para compilar y runtime ASP.NET Core 8 para ejecutar.
- **Frontend (`Frontend/Dockerfile`):** Multi-stage build con Node.js 18 para compilar la SPA de Vue 3 y Nginx Alpine para servir los archivos compilados.

### Volumen (Persistencia de Datos)
Permite conservar la información fuera del ciclo de vida del contenedor:
- En la base de datos se utiliza un volumen llamado `db_data` enlazado a `/var/lib/mysql`.
- *Sin volumen, al eliminar el contenedor se perderían los registros guardados.*

### Red Interna (Docker Bridge Network)
Docker crea una red privada interna (`ecowash_network`) para que los contenedores se comuniquen entre sí utilizando **nombres de servicio** en lugar de direcciones IP:
- Nginx -> `http://backend:80/api/`
- Backend -> `Server=database;Database=ecowash_db;User=root;Password=root;`
- El parámetro `DB_HOST` toma el valor del servicio `database`.

---

## 🚀 6. Docker Compose — Orquestación Multi-Contenedor

**Docker Compose** permite definir y administrar los 3 contenedores desde un solo archivo `docker-compose.yml`. En lugar de ejecutar cada contenedor manualmente, se puede iniciar todo el ecosistema con un solo comando:

### Comandos de Ejecución

```bash
# 1. Iniciar todos los contenedores en segundo plano
docker compose up -d

# 2. Ver estado de los contenedores
docker compose ps

# 3. Ver registros (logs) en tiempo real
docker compose logs -f

# 4. Detener y remover los contenedores y la red
docker compose down
```

---

## ☁️ 7. Despliegue en Producción (AWS EC2)

Docker no reemplaza a las Máquinas Virtuales. En un entorno de producción real como Amazon Web Services (AWS):

```
┌──────────────────────────────────────────────────────────┐
│                   AWS EC2 Instance                       │
│             (Máquina Virtual Ubuntu 22.04)               │
│ ┌──────────────────────────────────────────────────────┐ │
│ │                  Docker Engine                       │ │
│ │ ┌────────────────┐ ┌────────────────┐ ┌────────────┐ │ │
│ │ │ Contenedor Vue │ │ Contenedor API │ │ Contenedor │ │ │
│ │ │   (Frontend)   │ │   (Backend)    │ │ (MySQL DB) │ │ │
│ │ └────────────────┘ └────────────────┘ └────────────┘ │ │
│ └──────────────────────────────────────────────────────┘ │
└──────────────────────────────────────────────────────────┘
```

Se combinan ambas tecnologías: **AWS EC2 (VM Ubuntu)** aloja el motor de **Docker Engine**, el cual ejecuta la pila de **contenedores aislados** del proyecto EcoWash.
