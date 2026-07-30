# 🐳 EcoWash Móvil — Guía de Despliegue con Docker Compose

> **Documento de Pruebas y Despliegue**  
> Universidad Privada Domingo Savio (UPDS) — Programación Web II — 2026

---

## ✅ Requisitos Cumplidos

| Requisito | Estado | Detalle |
|-----------|--------|---------|
| Sistema inicia con Docker Compose | ✅ | `docker compose up -d` |
| Tres servicios activos | ✅ | `frontend`, `backend`, `database` |
| Comunicación por red Docker | ✅ | Red `ecowash_network` |
| Persistencia de datos con volumen | ✅ | Volumen `db_data` |
| Archivo `.env` no publicado en GitHub | ✅ | Incluido en `.gitignore` |
| Instrucciones claras de instalación | ✅ | Ver sección abajo |
| Operaciones funcionales y manejo de errores | ✅ | Ver sección de pruebas |

---

## 📁 Estructura del Proyecto

```
prototipo/
├── BackendApi/              # API REST — C# ASP.NET Core 8
│   ├── Dockerfile
│   └── ...
├── Frontend/                # SPA — Vue 3 + Nginx
│   ├── Dockerfile
│   └── ...
├── docker-compose.yml       # Orquestación de los 3 servicios
├── .env.example             # Plantilla de variables de entorno (SÍ se sube a GitHub)
├── .env                     # Variables reales (NO se sube a GitHub)
├── .gitignore               # Excluye .env del repositorio
└── script_ecowash_db.sql    # Script de inicialización de la base de datos
```

---

## 🔐 Paso 1 — Configurar el archivo `.env`

### ¿Por qué no se publica el `.env`?
El archivo `.env` contiene **contraseñas, claves secretas y datos sensibles** del sistema.
Publicarlo en GitHub expondría las credenciales a cualquier persona en Internet, lo cual
representa un riesgo de seguridad grave.

### Crear el archivo `.env` local

Copia el archivo de plantilla y completa los valores reales:

```bash
cp .env.example .env
```

Edita el archivo `.env` con tus valores:

```env
# Base de Datos MySQL
MYSQL_ROOT_PASSWORD=TuContraseñaSegura123
MYSQL_DATABASE=ecowash_db

# Backend ASP.NET Core
ASPNETCORE_ENVIRONMENT=Production
JWT_SECRET=TuClaveJWTSuperSecreta256bits

# Conexión a la base de datos
DB_CONNECTION=Server=database;Database=ecowash_db;User=root;Password=TuContraseñaSegura123;
```

### Verificar que `.env` está en `.gitignore`

Abre el archivo `.gitignore` y confirma que contiene:

```gitignore
# Variables de entorno — NO subir al repositorio
.env
*.env
!.env.example
```

> ⚠️ **IMPORTANTE:** Nunca hagas `git add .env`. Si ya lo subiste por error, usa:
> ```bash
> git rm --cached .env
> git commit -m "fix: eliminar .env del repositorio"
> ```

---

## 🏗️ Paso 2 — Arquitectura de los 3 Servicios Docker

```
┌─────────────────────────────────────────────────────────────┐
│                    RED: ecowash_network                      │
│                                                             │
│  ┌──────────────┐    HTTP     ┌──────────────┐             │
│  │   FRONTEND   │──proxy/api──▶   BACKEND    │             │
│  │  Vue3+Nginx  │             │  ASP.NET C8  │             │
│  │   Puerto:80  │             │  Puerto:5275 │             │
│  └──────────────┘             └──────┬───────┘             │
│                                      │ SQL                 │
│                               ┌──────▼───────┐             │
│                               │   DATABASE   │             │
│                               │   MySQL 8.0  │             │
│                               │  Puerto:3306 │             │
│                               └──────────────┘             │
└─────────────────────────────────────────────────────────────┘
                                       │
                               ┌───────▼────────┐
                               │ VOLUMEN db_data │
                               │  (persistencia) │
                               └────────────────┘
```

### Descripción de cada servicio

| Servicio | Imagen | Puerto | Función |
|----------|--------|--------|---------|
| `database` | `mysql:8.0` | `3306` | Almacena todos los datos del sistema |
| `backend` | `escorpion124/ecowash-backend` | `5275` | API REST con autenticación JWT |
| `frontend` | `escorpion124/ecowash-frontend` | `80` / `5173` | Interfaz web Vue 3 servida por Nginx |

---

## 🐳 Paso 3 — Archivo `docker-compose.yml` explicado

```yaml
version: '3.8'

services:

  # ─── SERVICIO 1: Base de Datos MySQL ─────────────────────────
  database:
    image: mysql:8.0
    container_name: ecowash_db
    restart: always
    environment:
      MYSQL_ROOT_PASSWORD: ${MYSQL_ROOT_PASSWORD}   # Viene del .env
      MYSQL_DATABASE: ${MYSQL_DATABASE}              # Viene del .env
    ports:
      - "3306:3306"
    volumes:
      - db_data:/var/lib/mysql                       # PERSISTENCIA de datos
      - ./script_ecowash_db.sql:/docker-entrypoint-initdb.d/script_ecowash_db.sql
    networks:
      - ecowash_network                              # RED interna Docker

  # ─── SERVICIO 2: Backend API ──────────────────────────────────
  backend:
    image: escorpion124/ecowash-backend:latest
    container_name: ecowash_backend
    restart: always
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - DB_HOST=database                            # Nombre del servicio en la red
      - ConnectionStrings__DefaultConnection=${DB_CONNECTION}
    ports:
      - "5275:80"
    depends_on:
      - database                                    # Espera a que DB esté lista
    networks:
      - ecowash_network                             # MISMA RED → pueden comunicarse

  # ─── SERVICIO 3: Frontend Vue 3 + Nginx ──────────────────────
  frontend:
    image: escorpion124/ecowash-frontend:latest
    container_name: ecowash_frontend
    restart: always
    ports:
      - "80:80"
      - "5173:80"
    depends_on:
      - backend                                     # Espera a que Backend esté listo
    networks:
      - ecowash_network                             # MISMA RED → proxy hacia backend

# ─── VOLÚMENES (Persistencia de datos) ───────────────────────────
volumes:
  db_data:
    driver: local      # Los datos de MySQL sobreviven aunque se elimine el contenedor

# ─── REDES (Comunicación entre servicios) ────────────────────────
networks:
  ecowash_network:
    driver: bridge     # Red privada virtual entre los 3 contenedores
```

---

## 🚀 Paso 4 — Instalación y Ejecución

### Prerrequisitos

Asegúrate de tener instalado:

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (incluye Docker y Docker Compose)
- Git

Verificar instalación:
```bash
docker --version
# Docker version 24.x.x

docker compose version
# Docker Compose version v2.x.x
```

---

### Instalación paso a paso

#### 1. Clonar el repositorio

```bash
git clone https://github.com/gaboale345/prototipo.git
cd prototipo
```

#### 2. Crear el archivo de variables de entorno

```bash
# Copiar la plantilla
copy .env.example .env      # En Windows
# cp .env.example .env      # En Linux/Mac
```

Edita `.env` con tus credenciales reales (ver Paso 1).

#### 3. Iniciar todos los servicios

```bash
docker compose up -d
```

> El flag `-d` ejecuta los contenedores en segundo plano (detached mode).

#### 4. Verificar que los servicios están corriendo

```bash
docker compose ps
```

Deberías ver algo así:

```
NAME                STATUS          PORTS
ecowash_db          Up (healthy)    0.0.0.0:3306->3306/tcp
ecowash_backend     Up              0.0.0.0:5275->80/tcp
ecowash_frontend    Up              0.0.0.0:80->80/tcp
```

#### 5. Acceder a la aplicación

| Servicio | URL |
|----------|-----|
| 🌐 **Frontend (App Web)** | http://localhost |
| ⚙️ **Backend API** | http://localhost:5275/api |
| 📖 **Swagger / Docs API** | http://localhost:5275/swagger |
| 🗄️ **Base de Datos** | `localhost:3306` (con MySQL Workbench) |

---

## 🛑 Comandos de Gestión

```bash
# Ver logs en tiempo real de todos los servicios
docker compose logs -f

# Ver logs solo del backend
docker compose logs -f backend

# Ver logs solo de la base de datos
docker compose logs -f database

# Detener los contenedores (sin eliminar datos)
docker compose stop

# Detener Y eliminar contenedores (los datos del volumen se conservan)
docker compose down

# Detener, eliminar contenedores Y eliminar volúmenes (BORRA TODOS LOS DATOS)
docker compose down -v

# Reconstruir las imágenes y reiniciar
docker compose up -d --build
```

---

## 🧪 Paso 5 — Pruebas de Operaciones Funcionales

### 5.1 Prueba de Autenticación (Login)

Abre Swagger en `http://localhost:5275/swagger` o usa curl:

```bash
curl -X POST http://localhost:5275/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"admin@ecowash.bo\", \"password\": \"Admin@1234\"}"
```

**Respuesta esperada (Éxito 200):**
```json
{
  "success": true,
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "rol": "Administrador",
    "nombre": "Administrador EcoWash",
    "expiresIn": "7 días"
  }
}
```

---

### 5.2 Credenciales de Prueba

| Rol | Email | Contraseña |
|-----|-------|------------|
| **Administrador** | `admin@ecowash.bo` | `Admin@1234` |
| **Empleado** | `empleado@ecowash.bo` | `Empleado@1234` |
| **Cliente** | `cliente@ecowash.bo` | `Cliente@1234` |

---

### 5.3 Prueba de Comunicación entre Servicios (Red Docker)

Verifica que el backend puede comunicarse con la base de datos:

```bash
# Entrar al contenedor del backend
docker exec -it ecowash_backend bash

# Desde dentro del contenedor, hacer ping a la base de datos por nombre de servicio
ping database
# Respuesta: PING database (172.20.0.2)... ✅
```

Esto demuestra que los servicios se comunican por **nombre de servicio** dentro de la
red `ecowash_network`, no por IP directa.

---

### 5.4 Prueba de Persistencia de Datos (Volumen)

```bash
# 1. Ingresar un dato nuevo desde la app (crear un cliente de prueba en el sistema)

# 2. Detener y eliminar los contenedores (SIN eliminar volúmenes)
docker compose down

# 3. Volver a iniciar
docker compose up -d

# 4. Verificar que el dato sigue existiendo en la base de datos ✅
```

El volumen `db_data` garantiza que los datos sobrevivan al reinicio de contenedores.

---

### 5.5 Prueba de Manejo de Errores

#### Error 401 — Token no proporcionado
```bash
curl http://localhost:5275/api/cliente
```
```json
{
  "success": false,
  "message": "No autorizado. Token JWT requerido."
}
```

#### Error 400 — Credenciales incorrectas
```bash
curl -X POST http://localhost:5275/api/auth/login \
  -H "Content-Type: application/json" \
  -d "{\"email\": \"noexiste@mail.com\", \"password\": \"wrongpass\"}"
```
```json
{
  "success": false,
  "message": "Credenciales inválidas."
}
```

#### Error 404 — Recurso no encontrado
```bash
curl -X GET http://localhost:5275/api/cliente/9999 \
  -H "Authorization: Bearer TU_TOKEN_AQUI"
```
```json
{
  "success": false,
  "message": "Cliente no encontrado."
}
```

---

## 🔒 Paso 6 — Seguridad: Verificar que `.env` no está en GitHub

### Verificar el `.gitignore`

```bash
cat .gitignore
```

Debe contener al menos:
```
.env
```

### Confirmar que Git ignora el archivo

```bash
git status
```

El archivo `.env` **NO debe aparecer** en la lista de archivos sin seguimiento.

### Verificar en GitHub

1. Ve a `https://github.com/gaboale345/prototipo`
2. Busca el archivo `.env`
3. **Debe aparecer un error 404** — eso confirma que está protegido ✅

---

## 📋 Checklist Final de Verificación

Antes de entregar, confirma cada punto:

- [ ] `docker compose up -d` inicia correctamente los **3 servicios**
- [ ] `docker compose ps` muestra `database`, `backend` y `frontend` en estado `Up`
- [ ] Se puede acceder a `http://localhost` (Frontend)
- [ ] Se puede acceder a `http://localhost:5275/swagger` (Swagger)
- [ ] El login con credenciales válidas devuelve un **Token JWT**
- [ ] El login con credenciales inválidas devuelve **error 400**
- [ ] Al detener y reiniciar, los datos **persisten** en el volumen
- [ ] El archivo `.env` **NO aparece** en el repositorio de GitHub
- [ ] El archivo `.env.example` **SÍ aparece** en GitHub (como plantilla)
- [ ] Los servicios se comunican entre sí por **nombre de red Docker**

---

## ❓ Solución de Problemas Comunes

### El contenedor de base de datos no inicia

```bash
docker compose logs database
```
Causa común: el puerto `3306` ya está en uso. Solución:
```bash
# Detener MySQL local si está corriendo (Windows)
net stop MySQL80
```

### El backend no puede conectarse a la base de datos

Verifica que el servicio `database` esté en estado `healthy`:
```bash
docker compose ps
# Si database está en "starting", espera unos segundos y reintenta
```

### La imagen no se encuentra (pull error)

```bash
docker pull escorpion124/ecowash-backend:latest
docker pull escorpion124/ecowash-frontend:latest
```

### Reconstruir todo desde cero

```bash
docker compose down -v          # Elimina contenedores y volúmenes
docker compose up -d --build    # Reconstruye y reinicia
```

---

## 📄 Referencias

- [Documentación oficial Docker Compose](https://docs.docker.com/compose/)
- [Docker Hub — escorpion124](https://hub.docker.com/u/escorpion124)
- [Repositorio GitHub del proyecto](https://github.com/gaboale345/prototipo)
- [Swagger UI del proyecto](http://localhost:5275/swagger)

---

*Documento generado para el proyecto EcoWash Móvil — UPDS 2026*
