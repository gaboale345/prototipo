-- =============================================================================
-- PROYECTO UNIVERSITARIO: EcoWash Móvil
-- MATERIA: Programación Web II (UPDS - Universidad Privada Domingo Savio)
-- BASE DE DATOS: MySQL 8.0+
-- TERCERA FORMA NORMAL (3FN) - MODELO COMPLETO Y SEED DATA
-- =============================================================================

CREATE DATABASE IF NOT EXISTS `ecowash_db` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
USE `ecowash_db`;

-- -----------------------------------------------------------------------------
-- 1. TABLA: EMPRENDIMIENTOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `emprendimientos` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nombre` VARCHAR(150) NOT NULL,
  `descripcion` VARCHAR(300) NULL,
  `direccion` VARCHAR(200) NULL,
  `telefono` VARCHAR(20) NULL,
  `email` VARCHAR(150) NULL,
  `logo_url` VARCHAR(200) NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 2. TABLA: ROLES
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `roles` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nombre` VARCHAR(80) NOT NULL UNIQUE,
  `descripcion` VARCHAR(200) NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 3. TABLA: PERMISOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `permisos` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nombre` VARCHAR(100) NOT NULL,
  `descripcion` VARCHAR(200) NULL,
  `modulo` VARCHAR(100) NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 4. TABLA: ROL_PERMISOS (Muchos a Muchos)
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `rol_permisos` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `rol_id` INT NOT NULL,
  `permiso_id` INT NOT NULL,
  UNIQUE KEY `uk_rol_permiso` (`rol_id`, `permiso_id`),
  FOREIGN KEY (`rol_id`) REFERENCES `roles`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`permiso_id`) REFERENCES `permisos`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 5. TABLA: USUARIOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `usuarios` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `emprendimiento_id` INT NULL,
  `rol_id` INT NOT NULL,
  `nombre` VARCHAR(100) NOT NULL,
  `apellido` VARCHAR(100) NOT NULL,
  `email` VARCHAR(150) NOT NULL UNIQUE,
  `password_hash` VARCHAR(255) NOT NULL,
  `telefono` VARCHAR(20) NULL,
  `foto_url` VARCHAR(200) NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `email_verificado` TINYINT(1) NOT NULL DEFAULT 0,
  `token_recuperacion` VARCHAR(255) NULL,
  `token_expiracion` DATETIME NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `ultimo_acceso` DATETIME NULL,
  FOREIGN KEY (`emprendimiento_id`) REFERENCES `emprendimientos`(`id`) ON DELETE SET NULL,
  FOREIGN KEY (`rol_id`) REFERENCES `roles`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 6. TABLA: CLIENTES
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `clientes` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `usuario_id` INT NOT NULL UNIQUE,
  `ci` VARCHAR(20) NULL,
  `direccion` VARCHAR(200) NULL,
  `ciudad` VARCHAR(50) DEFAULT 'Santa Cruz de la Sierra',
  `fecha_registro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  FOREIGN KEY (`usuario_id`) REFERENCES `usuarios`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 7. TABLA: EMPLEADOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `empleados` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `usuario_id` INT NOT NULL UNIQUE,
  `ci` VARCHAR(20) NULL,
  `cargo` VARCHAR(100) DEFAULT 'Lavador Profesional',
  `salario` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `fecha_ingreso` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `disponible` TINYINT(1) NOT NULL DEFAULT 1,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  FOREIGN KEY (`usuario_id`) REFERENCES `usuarios`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 8. TABLA: CATEGORIAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `categorias` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nombre` VARCHAR(100) NOT NULL,
  `descripcion` VARCHAR(200) NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 9. TABLA: PRODUCTOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `productos` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `categoria_id` INT NOT NULL,
  `nombre` VARCHAR(150) NOT NULL,
  `descripcion` VARCHAR(300) NULL,
  `unidad_medida` VARCHAR(50) DEFAULT 'Unidad',
  `precio_unitario` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `stock_actual` INT NOT NULL DEFAULT 0,
  `stock_minimo` INT NOT NULL DEFAULT 5,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`categoria_id`) REFERENCES `categorias`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 10. TABLA: PROVEEDORES
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `proveedores` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nombre` VARCHAR(150) NOT NULL,
  `nit` VARCHAR(20) NULL,
  `contacto` VARCHAR(150) NULL,
  `telefono` VARCHAR(20) NULL,
  `email` VARCHAR(150) NULL,
  `direccion` VARCHAR(200) NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 11. TABLA: COMPRAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `compras` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `proveedor_id` INT NOT NULL,
  `usuario_id` INT NOT NULL,
  `numero_factura` VARCHAR(50) NULL,
  `fecha_compra` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `total` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `estado` VARCHAR(50) NOT NULL DEFAULT 'Pendiente',
  `observaciones` VARCHAR(300) NULL,
  FOREIGN KEY (`proveedor_id`) REFERENCES `proveedores`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`usuario_id`) REFERENCES `usuarios`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 12. TABLA: DETALLE_COMPRAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `detalle_compras` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `compra_id` INT NOT NULL,
  `producto_id` INT NOT NULL,
  `cantidad` INT NOT NULL,
  `precio_unitario` DECIMAL(10,2) NOT NULL,
  `subtotal` DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (`compra_id`) REFERENCES `compras`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`producto_id`) REFERENCES `productos`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 13. TABLA: INVENTARIOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `inventarios` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `producto_id` INT NOT NULL UNIQUE,
  `cantidad` INT NOT NULL DEFAULT 0,
  `cantidad_minima` INT NOT NULL DEFAULT 5,
  `ultima_actualizacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`producto_id`) REFERENCES `productos`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 14. TABLA: VEHICULOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `vehiculos` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `cliente_id` INT NOT NULL,
  `placa` VARCHAR(20) NOT NULL UNIQUE,
  `tipo` VARCHAR(50) NOT NULL,
  `marca` VARCHAR(80) NULL,
  `modelo` VARCHAR(80) NULL,
  `año` VARCHAR(10) NULL,
  `color` VARCHAR(50) NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `fecha_registro` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`cliente_id`) REFERENCES `clientes`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 15. TABLA: UBICACIONES
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ubicaciones` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `cliente_id` INT NOT NULL,
  `direccion` VARCHAR(200) NOT NULL,
  `zona` VARCHAR(100) NULL,
  `referencia` VARCHAR(100) NULL,
  `latitud` DOUBLE NULL,
  `longitud` DOUBLE NULL,
  `es_principal` TINYINT(1) NOT NULL DEFAULT 0,
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  FOREIGN KEY (`cliente_id`) REFERENCES `clientes`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 16. TABLA: SERVICIOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `servicios` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nombre` VARCHAR(100) NOT NULL,
  `descripcion` VARCHAR(300) NULL,
  `precio` DECIMAL(10,2) NOT NULL,
  `duracion_minutos` INT NOT NULL DEFAULT 60,
  `tipo_vehiculo` VARCHAR(50) DEFAULT 'Todos',
  `activo` TINYINT(1) NOT NULL DEFAULT 1,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 17. TABLA: RESERVAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reservas` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `cliente_id` INT NOT NULL,
  `empleado_id` INT NULL,
  `vehiculo_id` INT NOT NULL,
  `ubicacion_id` INT NOT NULL,
  `servicio_id` INT NOT NULL,
  `fecha_programada` DATETIME NOT NULL,
  `fecha_inicio` DATETIME NULL,
  `fecha_fin` DATETIME NULL,
  `estado` VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
  `precio_total` DECIMAL(10,2) NOT NULL,
  `observaciones` VARCHAR(300) NULL,
  `fecha_creacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`cliente_id`) REFERENCES `clientes`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`empleado_id`) REFERENCES `empleados`(`id`) ON DELETE SET NULL,
  FOREIGN KEY (`vehiculo_id`) REFERENCES `vehiculos`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`ubicacion_id`) REFERENCES `ubicaciones`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`servicio_id`) REFERENCES `servicios`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 18. TABLA: VENTAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `ventas` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `reserva_id` INT NOT NULL UNIQUE,
  `cliente_id` INT NOT NULL,
  `numero_venta` VARCHAR(50) NOT NULL UNIQUE,
  `subtotal` DECIMAL(10,2) NOT NULL,
  `descuento` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `total` DECIMAL(10,2) NOT NULL,
  `fecha_venta` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `estado` VARCHAR(30) NOT NULL DEFAULT 'Pendiente',
  FOREIGN KEY (`reserva_id`) REFERENCES `reservas`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`cliente_id`) REFERENCES `clientes`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 19. TABLA: DETALLE_VENTAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `detalle_ventas` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `venta_id` INT NOT NULL,
  `producto_id` INT NULL,
  `descripcion` VARCHAR(150) NOT NULL,
  `cantidad` DECIMAL(10,2) NOT NULL,
  `precio_unitario` DECIMAL(10,2) NOT NULL,
  `subtotal` DECIMAL(10,2) NOT NULL,
  FOREIGN KEY (`venta_id`) REFERENCES `ventas`(`id`) ON DELETE CASCADE,
  FOREIGN KEY (`producto_id`) REFERENCES `productos`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 20. TABLA: METODOS_PAGO
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `metodos_pago` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `nombre` VARCHAR(80) NOT NULL,
  `descripcion` VARCHAR(200) NULL,
  `activo` TINYINT(1) NOT NULL DEFAULT 1
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 21. TABLA: PAGOS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `pagos` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `venta_id` INT NOT NULL,
  `reserva_id` INT NOT NULL,
  `metodo_pago_id` INT NOT NULL,
  `monto` DECIMAL(10,2) NOT NULL,
  `fecha_pago` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `estado` VARCHAR(30) NOT NULL DEFAULT 'Completado',
  `referencia` VARCHAR(100) NULL,
  FOREIGN KEY (`venta_id`) REFERENCES `ventas`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`reserva_id`) REFERENCES `reservas`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`metodo_pago_id`) REFERENCES `metodos_pago`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 22. TABLA: FACTURAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `facturas` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `venta_id` INT NOT NULL UNIQUE,
  `pago_id` INT NOT NULL UNIQUE,
  `numero_factura` VARCHAR(50) NOT NULL UNIQUE,
  `fecha_emision` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `razon_social` VARCHAR(150) NULL,
  `nit` VARCHAR(20) NULL,
  `subtotal` DECIMAL(10,2) NOT NULL,
  `descuento` DECIMAL(10,2) NOT NULL DEFAULT 0.00,
  `total` DECIMAL(10,2) NOT NULL,
  `estado` VARCHAR(30) NOT NULL DEFAULT 'Emitida',
  FOREIGN KEY (`venta_id`) REFERENCES `ventas`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`pago_id`) REFERENCES `pagos`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 23. TABLA: CALIFICACIONES
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `calificaciones` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `reserva_id` INT NOT NULL UNIQUE,
  `cliente_id` INT NOT NULL,
  `puntuacion` INT NOT NULL CHECK (`puntuacion` BETWEEN 1 AND 5),
  `comentario` VARCHAR(500) NULL,
  `fecha` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`reserva_id`) REFERENCES `reservas`(`id`) ON DELETE RESTRICT,
  FOREIGN KEY (`cliente_id`) REFERENCES `clientes`(`id`) ON DELETE RESTRICT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 24. TABLA: NOTIFICACIONES
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `notificaciones` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `usuario_id` INT NOT NULL,
  `titulo` VARCHAR(150) NOT NULL,
  `mensaje` VARCHAR(500) NOT NULL,
  `tipo` VARCHAR(50) DEFAULT 'Info',
  `leida` TINYINT(1) NOT NULL DEFAULT 0,
  `fecha` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `referencia_url` VARCHAR(100) NULL,
  FOREIGN KEY (`usuario_id`) REFERENCES `usuarios`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 25. TABLA: AUDITORIAS
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `auditorias` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `usuario_id` INT NULL,
  `accion` VARCHAR(100) NOT NULL,
  `modulo` VARCHAR(100) NULL,
  `entidad` VARCHAR(50) NULL,
  `entidad_id` INT NULL,
  `datos_anteriores` LONGTEXT NULL,
  `datos_nuevos` LONGTEXT NULL,
  `ip` VARCHAR(45) NULL,
  `fecha` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`usuario_id`) REFERENCES `usuarios`(`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- 26. TABLA: REPORTES
-- -----------------------------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reportes` (
  `id` INT AUTO_INCREMENT PRIMARY KEY,
  `usuario_id` INT NOT NULL,
  `nombre` VARCHAR(150) NOT NULL,
  `tipo` VARCHAR(80) NOT NULL,
  `fecha_inicio` DATETIME NULL,
  `fecha_fin` DATETIME NULL,
  `datos` LONGTEXT NULL,
  `fecha_generacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  FOREIGN KEY (`usuario_id`) REFERENCES `usuarios`(`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- -----------------------------------------------------------------------------
-- SEED DATA (DATOS INICIALES PARA PRUEBAS Y EVALUACIÓN UNIVERSITARIA)
-- -----------------------------------------------------------------------------
INSERT INTO `roles` (`id`, `nombre`, `descripcion`) VALUES
(1, 'Administrador', 'Acceso completo al sistema'),
(2, 'Empleado', 'Gestión de servicios de lavado'),
(3, 'Cliente', 'Reservas y consultas personales');

INSERT INTO `emprendimientos` (`id`, `nombre`, `descripcion`, `telefono`, `email`) VALUES
(1, 'EcoWash Móvil', 'Servicio de lavado de autos a domicilio en Santa Cruz de la Sierra', '+591 77000000', 'contacto@ecowash.bo');

INSERT INTO `servicios` (`id`, `nombre`, `descripcion`, `precio`, `duracion_minutos`, `tipo_vehiculo`) VALUES
(1, 'Lavado Básico', 'Lavado exterior completo con espuma activa', 50.00, 45, 'Todos'),
(2, 'Lavado Completo', 'Lavado exterior e interior profundo + aspirado', 80.00, 75, 'Todos'),
(3, 'Lavado Premium', 'Lavado completo + encerado + brillo de llantas', 150.00, 120, 'Auto'),
(4, 'Lavado de Motocicletas', 'Lavado especializado para motos', 35.00, 30, 'Moto'),
(5, 'Aspirado', 'Aspirado interior y limpieza de alfombras', 30.00, 30, 'Todos'),
(6, 'Encerado', 'Aplicación de cera protectora UV', 60.00, 45, 'Auto'),
(7, 'Pulido', 'Pulido de carrocería profesional', 120.00, 90, 'Auto'),
(8, 'Lavado Ecológico', 'Lavado en seco con mínimo consumo de agua', 65.00, 50, 'Todos');

INSERT INTO `metodos_pago` (`id`, `nombre`, `descripcion`) VALUES
(1, 'Efectivo', 'Pago presencial al lavador'),
(2, 'QR', 'Transferencia rápida por código QR'),
(3, 'Transferencia Bancaria', 'Depósito o transferencia directa'),
(4, 'Tarjeta', 'Pago con tarjeta de débito/crédito');

INSERT INTO `categorias` (`id`, `nombre`, `descripcion`) VALUES
(1, 'Detergentes', 'Champús y desengrasantes automotrices'),
(2, 'Ceras y Protectores', 'Ceras de teflón y siliconas de llantas'),
(3, 'Micropaños', 'Paños de microfibra de alto absorbente'),
(4, 'Equipos', 'Hidrolavadoras y aspiradoras portátiles');

-- Hashes con contraseña para pruebas: Admin@1234, Empleado@1234, Cliente@1234
INSERT INTO `usuarios` (`id`, `emprendimiento_id`, `rol_id`, `nombre`, `apellido`, `email`, `password_hash`, `telefono`) VALUES
(1, 1, 1, 'Admin', 'EcoWash', 'admin@ecowash.bo', '$2a$12$K1rZgGgD1xX3Z.zJ8yqU4.F/J/QxYl7bN5l9eZ8qU4.F/J/QxYl7b', '+591 70000001'),
(2, 1, 2, 'Juan', 'Pérez', 'empleado@ecowash.bo', '$2a$12$K1rZgGgD1xX3Z.zJ8yqU4.F/J/QxYl7bN5l9eZ8qU4.F/J/QxYl7b', '+591 70000002'),
(3, 1, 3, 'Carlos', 'Mendoza', 'cliente@ecowash.bo', '$2a$12$K1rZgGgD1xX3Z.zJ8yqU4.F/J/QxYl7bN5l9eZ8qU4.F/J/QxYl7b', '+591 70000003');

INSERT INTO `clientes` (`id`, `usuario_id`, `ci`, `direccion`, `ciudad`) VALUES
(1, 3, '7894561 SC', 'Av. Banzer 4to Anillo', 'Santa Cruz de la Sierra');

INSERT INTO `empleados` (`id`, `usuario_id`, `ci`, `cargo`, `salario`) VALUES
(1, 2, '4561238 SC', 'Lavador Profesional Lead', 2800.00);

INSERT INTO `vehiculos` (`id`, `cliente_id`, `placa`, `tipo`, `marca`, `modelo`, `año`, `color`) VALUES
(1, 1, '4589-XYZ', 'Auto', 'Toyota', 'Corolla', '2022', 'Blanco');

INSERT INTO `ubicaciones` (`id`, `cliente_id`, `direccion`, `zona`, `referencia`, `es_principal`) VALUES
(1, 1, 'Av. Banzer 4to Anillo, Calle 3 #120', 'Norte', 'Frente a la farmacia', 1);
