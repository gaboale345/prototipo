using BackendApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Data
{
    /// <summary>
    /// Contexto principal de la base de datos para EcoWash Móvil.
    /// Configura todas las entidades, relaciones y restricciones en 3FN.
    /// </summary>
    public class EcoWashDbContext : DbContext
    {
        public EcoWashDbContext(DbContextOptions<EcoWashDbContext> options) : base(options) { }

        // ── DbSets (una por entidad) ─────────────────────────────────────────────
        public DbSet<Emprendimiento> Emprendimientos { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<RolPermiso> RolPermisos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Proveedor> Proveedores { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }
        public DbSet<Inventario> Inventarios { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
        public DbSet<Vehiculo> Vehiculos { get; set; }
        public DbSet<Ubicacion> Ubicaciones { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Reserva> Reservas { get; set; }
        public DbSet<Venta> Ventas { get; set; }
        public DbSet<DetalleVenta> DetalleVentas { get; set; }
        public DbSet<MetodoPago> MetodosPago { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Factura> Facturas { get; set; }
        public DbSet<Calificacion> Calificaciones { get; set; }
        public DbSet<Notificacion> Notificaciones { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<Reporte> Reportes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ── Configuración Rol ────────────────────────────────────────────────
            modelBuilder.Entity<Rol>(entity =>
            {
                entity.HasIndex(r => r.Nombre).IsUnique();
            });

            // ── Configuración Usuario ────────────────────────────────────────────
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
                entity.HasOne(u => u.Rol)
                      .WithMany(r => r.Usuarios)
                      .HasForeignKey(u => u.RolId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(u => u.Emprendimiento)
                      .WithMany(e => e.Usuarios)
                      .HasForeignKey(u => u.EmprendimientoId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // ── Configuración Cliente ────────────────────────────────────────────
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasOne(c => c.Usuario)
                      .WithOne(u => u.Cliente)
                      .HasForeignKey<Cliente>(c => c.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Configuración Empleado ───────────────────────────────────────────
            modelBuilder.Entity<Empleado>(entity =>
            {
                entity.HasOne(e => e.Usuario)
                      .WithOne(u => u.Empleado)
                      .HasForeignKey<Empleado>(e => e.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.Property(e => e.Salario).HasPrecision(10, 2);
            });

            // ── Configuración RolPermiso ─────────────────────────────────────────
            modelBuilder.Entity<RolPermiso>(entity =>
            {
                entity.HasOne(rp => rp.Rol)
                      .WithMany(r => r.RolPermisos)
                      .HasForeignKey(rp => rp.RolId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(rp => rp.Permiso)
                      .WithMany(p => p.RolPermisos)
                      .HasForeignKey(rp => rp.PermisoId)
                      .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(rp => new { rp.RolId, rp.PermisoId }).IsUnique();
            });

            // ── Configuración Vehículo ───────────────────────────────────────────
            modelBuilder.Entity<Vehiculo>(entity =>
            {
                entity.HasIndex(v => v.Placa).IsUnique();
                entity.HasOne(v => v.Cliente)
                      .WithMany(c => c.Vehiculos)
                      .HasForeignKey(v => v.ClienteId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Configuración Inventario ─────────────────────────────────────────
            modelBuilder.Entity<Inventario>(entity =>
            {
                entity.HasOne(i => i.Producto)
                      .WithOne(p => p.Inventario)
                      .HasForeignKey<Inventario>(i => i.ProductoId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // ── Configuración Reserva ────────────────────────────────────────────
            modelBuilder.Entity<Reserva>(entity =>
            {
                entity.HasOne(r => r.Cliente)
                      .WithMany(c => c.Reservas)
                      .HasForeignKey(r => r.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Empleado)
                      .WithMany(e => e.Reservas)
                      .HasForeignKey(r => r.EmpleadoId)
                      .OnDelete(DeleteBehavior.SetNull);
                entity.HasOne(r => r.Vehiculo)
                      .WithMany(v => v.Reservas)
                      .HasForeignKey(r => r.VehiculoId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(r => r.Servicio)
                      .WithMany(s => s.Reservas)
                      .HasForeignKey(r => r.ServicioId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(r => r.PrecioTotal).HasPrecision(10, 2);
            });

            // ── Configuración Venta ──────────────────────────────────────────────
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasOne(v => v.Reserva)
                      .WithOne(r => r.Venta)
                      .HasForeignKey<Venta>(v => v.ReservaId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(v => v.NumeroVenta).IsUnique();
                entity.Property(v => v.Subtotal).HasPrecision(10, 2);
                entity.Property(v => v.Descuento).HasPrecision(10, 2);
                entity.Property(v => v.Total).HasPrecision(10, 2);
            });

            // ── Configuración Pago ───────────────────────────────────────────────
            modelBuilder.Entity<Pago>(entity =>
            {
                entity.HasOne(p => p.Venta)
                      .WithMany(v => v.Pagos)
                      .HasForeignKey(p => p.VentaId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.Property(p => p.Monto).HasPrecision(10, 2);
            });

            // ── Configuración Factura ────────────────────────────────────────────
            modelBuilder.Entity<Factura>(entity =>
            {
                entity.HasOne(f => f.Venta)
                      .WithOne(v => v.Factura)
                      .HasForeignKey<Factura>(f => f.VentaId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(f => f.Pago)
                      .WithOne(p => p.Factura)
                      .HasForeignKey<Factura>(f => f.PagoId)
                      .OnDelete(DeleteBehavior.Restrict);
                entity.HasIndex(f => f.NumeroFactura).IsUnique();
                entity.Property(f => f.Subtotal).HasPrecision(10, 2);
                entity.Property(f => f.Descuento).HasPrecision(10, 2);
                entity.Property(f => f.Total).HasPrecision(10, 2);
            });

            // ── Configuración Calificación ───────────────────────────────────────
            modelBuilder.Entity<Calificacion>(entity =>
            {
                entity.HasOne(c => c.Reserva)
                      .WithOne(r => r.Calificacion)
                      .HasForeignKey<Calificacion>(c => c.ReservaId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // ── Configuración Producto ───────────────────────────────────────────
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.PrecioUnitario).HasPrecision(10, 2);
            });

            // ── Configuración DetalleCompra ──────────────────────────────────────
            modelBuilder.Entity<DetalleCompra>(entity =>
            {
                entity.Property(d => d.PrecioUnitario).HasPrecision(10, 2);
                entity.Property(d => d.Subtotal).HasPrecision(10, 2);
            });

            // ── Configuración DetalleVenta ───────────────────────────────────────
            modelBuilder.Entity<DetalleVenta>(entity =>
            {
                entity.Property(d => d.Cantidad).HasPrecision(10, 2);
                entity.Property(d => d.PrecioUnitario).HasPrecision(10, 2);
                entity.Property(d => d.Subtotal).HasPrecision(10, 2);
            });

            // ── Configuración Compra ─────────────────────────────────────────────
            modelBuilder.Entity<Compra>(entity =>
            {
                entity.Property(c => c.Total).HasPrecision(10, 2);
            });

            // ── Configuración Servicio ───────────────────────────────────────────
            modelBuilder.Entity<Servicio>(entity =>
            {
                entity.Property(s => s.Precio).HasPrecision(10, 2);
            });

            // ── Seed Data ────────────────────────────────────────────────────────
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Roles iniciales
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Nombre = "Administrador", Descripcion = "Acceso total al sistema", Activo = true },
                new Rol { Id = 2, Nombre = "Empleado", Descripcion = "Gestión de servicios y reservas", Activo = true },
                new Rol { Id = 3, Nombre = "Cliente", Descripcion = "Reservas y consultas personales", Activo = true }
            );

            // Emprendimiento base
            modelBuilder.Entity<Emprendimiento>().HasData(
                new Emprendimiento
                {
                    Id = 1,
                    Nombre = "EcoWash Móvil",
                    Descripcion = "Servicio de lavado de autos a domicilio en Santa Cruz de la Sierra",
                    Telefono = "+591 77000000",
                    Email = "contacto@ecowash.bo",
                    Activo = true,
                    FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Servicios base
            modelBuilder.Entity<Servicio>().HasData(
                new Servicio { Id = 1, Nombre = "Lavado Básico", Descripcion = "Lavado exterior completo", Precio = 50, DuracionMinutos = 45, TipoVehiculo = "Todos", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Servicio { Id = 2, Nombre = "Lavado Completo", Descripcion = "Exterior e interior", Precio = 80, DuracionMinutos = 75, TipoVehiculo = "Todos", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Servicio { Id = 3, Nombre = "Lavado Premium", Descripcion = "Lavado + encerado + aspirado", Precio = 150, DuracionMinutos = 120, TipoVehiculo = "Auto", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Servicio { Id = 4, Nombre = "Lavado de Motocicletas", Descripcion = "Lavado completo para motos", Precio = 35, DuracionMinutos = 30, TipoVehiculo = "Moto", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Servicio { Id = 5, Nombre = "Aspirado", Descripcion = "Aspirado interior del vehículo", Precio = 30, DuracionMinutos = 30, TipoVehiculo = "Todos", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Servicio { Id = 6, Nombre = "Encerado", Descripcion = "Aplicación de cera protectora", Precio = 60, DuracionMinutos = 45, TipoVehiculo = "Auto", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Servicio { Id = 7, Nombre = "Pulido", Descripcion = "Pulido de pintura profesional", Precio = 120, DuracionMinutos = 90, TipoVehiculo = "Auto", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
                new Servicio { Id = 8, Nombre = "Lavado Ecológico", Descripcion = "Lavado con mínimo consumo de agua", Precio = 65, DuracionMinutos = 50, TipoVehiculo = "Todos", Activo = true, FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Métodos de pago
            modelBuilder.Entity<MetodoPago>().HasData(
                new MetodoPago { Id = 1, Nombre = "Efectivo", Descripcion = "Pago en efectivo", Activo = true },
                new MetodoPago { Id = 2, Nombre = "QR", Descripcion = "Pago mediante código QR", Activo = true },
                new MetodoPago { Id = 3, Nombre = "Transferencia Bancaria", Descripcion = "Transferencia a cuenta bancaria", Activo = true },
                new MetodoPago { Id = 4, Nombre = "Tarjeta", Descripcion = "Pago con tarjeta de débito/crédito", Activo = true }
            );

            // Categorías de productos
            modelBuilder.Entity<Categoria>().HasData(
                new Categoria { Id = 1, Nombre = "Detergentes", Descripcion = "Jabones y champús para lavado", Activo = true },
                new Categoria { Id = 2, Nombre = "Ceras y Protectores", Descripcion = "Ceras, selladores y abrillantadores", Activo = true },
                new Categoria { Id = 3, Nombre = "Micropaños", Descripcion = "Paños de microfibra para secado", Activo = true },
                new Categoria { Id = 4, Nombre = "Equipos", Descripcion = "Equipos y herramientas de trabajo", Activo = true }
            );

            // Usuarios por defecto (Admin, Empleado, Cliente)
            modelBuilder.Entity<Usuario>().HasData(
                new Usuario
                {
                    Id = 1,
                    EmprendimientoId = 1,
                    RolId = 1,
                    Nombre = "Admin",
                    Apellido = "EcoWash",
                    Email = "admin@ecowash.bo",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@1234"),
                    Activo = true,
                    EmailVerificado = true,
                    FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Usuario
                {
                    Id = 2,
                    EmprendimientoId = 1,
                    RolId = 2,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "empleado@ecowash.bo",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Empleado@1234"),
                    Telefono = "+591 70000002",
                    Activo = true,
                    EmailVerificado = true,
                    FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                },
                new Usuario
                {
                    Id = 3,
                    EmprendimientoId = 1,
                    RolId = 3,
                    Nombre = "Carlos",
                    Apellido = "Mendoza",
                    Email = "cliente@ecowash.bo",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Cliente@1234"),
                    Telefono = "+591 70000003",
                    Activo = true,
                    EmailVerificado = true,
                    FechaCreacion = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Empleado por defecto
            modelBuilder.Entity<Empleado>().HasData(
                new Empleado
                {
                    Id = 1,
                    UsuarioId = 2,
                    Ci = "4561238 SC",
                    Cargo = "Lavador Profesional Lead",
                    Salario = 2800.00m,
                    FechaIngreso = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Disponible = true,
                    Activo = true
                }
            );

            // Cliente por defecto
            modelBuilder.Entity<Cliente>().HasData(
                new Cliente
                {
                    Id = 1,
                    UsuarioId = 3,
                    Ci = "7894561 SC",
                    Direccion = "Av. Banzer 4to Anillo",
                    Ciudad = "Santa Cruz de la Sierra",
                    FechaRegistro = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    Activo = true
                }
            );

            // Vehículo de prueba del cliente
            modelBuilder.Entity<Vehiculo>().HasData(
                new Vehiculo
                {
                    Id = 1,
                    ClienteId = 1,
                    Placa = "4589-XYZ",
                    Tipo = "Auto",
                    Marca = "Toyota",
                    Modelo = "Corolla",
                    Año = "2022",
                    Color = "Blanco",
                    Activo = true,
                    FechaRegistro = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                }
            );

            // Ubicación de prueba del cliente
            modelBuilder.Entity<Ubicacion>().HasData(
                new Ubicacion
                {
                    Id = 1,
                    ClienteId = 1,
                    Direccion = "Av. Banzer 4to Anillo, Calle 3 #120",
                    Zona = "Norte",
                    Referencia = "Frente a la farmacia",
                    EsPrincipal = true,
                    Activo = true
                }
            );
        }
    }
}
