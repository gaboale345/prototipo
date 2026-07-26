# 🚗 EcoWash Móvil — Plataforma de Gestión Comercial de Lavado de Autos a Domicilio

**Proyecto Universitario de Ingeniería de Software / Programación Web II**  
**Universidad Privada Domingo Savio (UPDS) — Santa Cruz de la Sierra, Bolivia**

---

## 📋 Descripción del Proyecto

**EcoWash Móvil** es una solución tecnológica integral y escalable diseñada para resolver la problemática de los emprendimientos de lavado de vehículos a domicilio en Santa Cruz de la Sierra. La plataforma reemplaza los registros manuales y llamadas telefónicas por un flujo automatizado que abarca desde la reserva inteligente del cliente hasta la facturación, descuento de inventario, auditoría y reportes estadísticos.

---

## 🏛️ Arquitectura del Sistema (MVC + REST API)

El proyecto sigue una **Arquitectura Desacoplada de Capas**:

```
 ┌─────────────────────────────────────────────────────────┐
 │               Frontend (Vue 3 + Vite)                   │
 │   - Views & Components (Bootstrap 5 + Custom Design)    │
 │   - State Management (Pinia Store)                      │
 │   - HTTP Client (Axios + JWT Interceptors)              │
 └────────────────────────────┬────────────────────────────┘
                              │ HTTP REST / JSON
 ┌────────────────────────────▼────────────────────────────┐
 │            Backend API (C# ASP.NET Core 8)             │
 │   - Controllers (22 API Controllers)                    │
 │   - Services & Helpers (JWT, BCrypt, Auditoria)         │
 │   - Data Transfer Objects (DTOs con DataAnnotations)    │
 │   - Entity Framework Core 8 (ORM + MySql Provider)      │
 └────────────────────────────┬────────────────────────────┘
                              │ SQL / Entity Framework
 ┌────────────────────────────▼────────────────────────────┐
 │               Base de Datos (MySQL 8.0)                 │
 │   - 27 Entidades / Tablas en 3FN                        │
 │   - Triggers & Foreign Keys con Integridad Referencial  │
 └─────────────────────────────────────────────────────────┘
```

---

## ⚡ Tecnologías Utilizadas

| Capa | Tecnología / Herramientas |
|------|---------------------------|
| **Backend** | C# .NET 8 (ASP.NET Core Web API), Entity Framework Core 8, JWT Bearer Auth, BCrypt.Net |
| **Frontend** | Vue 3 (Composition API), Vite, Pinia, Vue Router 4, Axios, Bootstrap 5.3, Bootstrap Icons |
| **Base de Datos** | MySQL 8.0 (Cumplimiento estricto 3FN) |
| **Documentación API** | Swagger UI / OpenAPI v1 |
| **Diseño UX/UI** | Inspirado en Uber, Booking y PedidosYa (Paleta `#2563EB`, Glassmorphism, Micro-animaciones) |

---

## 👥 Actores del Sistema

1. **Cliente:** Registra sus vehículos y múltiples ubicaciones, solicita servicios de lavado, realiza pagos por QR/Efectivo/Tarjeta, consulta su historial y califica la atención.
2. **Empleado:** Revisa reservas asignadas, acepta/rechaza servicios, inicia el lavado y marca la finalización.
3. **Administrador:** Acceso completo a todos los CRUDs (Usuarios, Empleados, Clientes, Productos, Inventario, Compras, Ventas, Pagos, Facturas), generación de reportes y auditoría del sistema.

---

## 🛡️ Reglas de Negocio Implementadas

- **RN-01:** Un cliente debe estar autenticado para reservar un servicio.
- **RN-02:** Un vehículo pertenece a un solo cliente (Clave foránea única).
- **RN-03:** Una reserva solo puede asignarse a un empleado activo y disponible.
- **RN-04:** No se permiten dos reservas para el mismo empleado en el mismo rango de horario.
- **RN-05:** El pago solo puede realizarse cuando la reserva está en estado Aceptada o posterior.
- **RN-06:** La venta se genera automáticamente únicamente al **finalizar** el servicio.
- **RN-07:** Cada servicio finalizado descuenta automáticamente insumos del inventario.
- **RN-08:** Cuando el stock de un producto alcanza el nivel mínimo, se emite una notificación de alerta al administrador.
- **RN-09:** Exclusividad de reportes para el rol Administrador.
- **RN-10:** Registro automático de auditoría en cada acción crítica del sistema (Login, Creación, Modificación, Eliminación).

---

## 📊 Entidades del Modelo de Datos (3FN)

1. `Emprendimiento`
2. `Rol`
3. `Permiso`
4. `RolPermiso`
5. `Usuario`
6. `Cliente`
7. `Empleado`
8. `Categoria`
9. `Producto`
10. `Proveedor`
11. `Compra`
12. `DetalleCompra`
13. `Inventario`
14. `MovimientoInventario`
15. `Vehiculo`
16. `Ubicacion`
17. `Servicio`
18. `Reserva`
19. `Venta`
20. `DetalleVenta`
21. `MetodoPago`
22. `Pago`
23. `Factura`
24. `Calificacion`
25. `Notificacion`
26. `Auditoria`
27. `Reporte`

---

## 🚀 Guía de Instalación y Ejecución Local

### Prerrequisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js v18+ & npm](https://nodejs.org/)
- [MySQL Server 8.0+](https://dev.mysql.com/downloads/mysql/)

### 1. Configurar la Base de Datos
Ejecutar el script SQL incluido en el proyecto:
```bash
mysql -u root -p < script_ecowash_db.sql
```

### 2. Iniciar el Backend API (C# .NET 8)
```bash
cd BackendApi
dotnet run
```
> El backend estará disponible en `http://localhost:5000` (o el puerto configurado) y la documentación interactiva en `http://localhost:5000/swagger`.

### 3. Iniciar el Frontend (Vue 3)
```bash
cd Frontend
npm install --legacy-peer-deps
npm run dev
```
> El frontend estará disponible en `http://localhost:5173`.

---

## 🔐 Credenciales de Prueba (Seed Data)

| Rol | Email | Contraseña |
|-----|-------|------------|
| **Administrador** | `admin@ecowash.bo` | `Admin@1234` |
| **Empleado** | `empleado@ecowash.bo` | `Empleado@1234` |
| **Cliente** | `cliente@ecowash.bo` | `Cliente@1234` |

---

## 📄 Créditos Universitarios
- **Institución:** Universidad Privada Domingo Savio (UPDS)
- **Sede:** Santa Cruz de la Sierra, Bolivia
- **Materia:** Programación Web II
- **Año:** 2026
