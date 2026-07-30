using System.ComponentModel.DataAnnotations;

namespace BackendApi.DTOs
{
    // ── Auth DTOs ──────────────────────────────────────────────────────────────

    public class LoginRequestDto
    {
        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress(ErrorMessage = "Formato de email inválido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(6, ErrorMessage = "Mínimo 6 caracteres")]
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int UsuarioId { get; set; }
        public DateTime Expiracion { get; set; }
    }

    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100)]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El apellido es obligatorio")]
        [MaxLength(100)]
        public string Apellido { get; set; } = string.Empty;

        [Required(ErrorMessage = "El email es obligatorio")]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [MinLength(8, ErrorMessage = "Mínimo 8 caracteres")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$",
            ErrorMessage = "La contraseña debe tener mayúsculas, minúsculas, número y símbolo")]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? Telefono { get; set; }
    }

    public class RecuperarPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }

    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string NuevoPassword { get; set; } = string.Empty;
    }

    // ── Usuario DTOs ───────────────────────────────────────────────────────────

    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Apellido { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? FotoUrl { get; set; }
        public string Rol { get; set; } = string.Empty;
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CrearUsuarioDto
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required] public string Apellido { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required][MinLength(8)] public string Password { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        [Required] public int RolId { get; set; }
        public int? EmprendimientoId { get; set; }
    }

    public class ActualizarUsuarioDto
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? FotoUrl { get; set; }
    }

    public class CambiarPasswordDto
    {
        [Required] public string PasswordActual { get; set; } = string.Empty;
        [Required][MinLength(8)] public string NuevoPassword { get; set; } = string.Empty;
    }

    // ── Cliente DTOs ───────────────────────────────────────────────────────────

    public class ClienteDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Ci { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
        public string? ZonaPrincipal { get; set; }
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public int TotalVehiculos { get; set; }
        public int TotalReservas { get; set; }
        public decimal TotalGastado { get; set; }
        public DateTime? UltimaReservaFecha { get; set; }
        public List<VehiculoDto> Vehiculos { get; set; } = new();
        public List<UbicacionDto> Ubicaciones { get; set; } = new();
    }

    public class ActualizarClienteDto
    {
        public string? Ci { get; set; }
        public string? Direccion { get; set; }
        public string? Ciudad { get; set; }
    }

    public class ClienteDashboardSummaryDto
    {
        public int TotalClientes { get; set; }
        public int ClientesActivos { get; set; }
        public int TotalVehiculosRegistrados { get; set; }
        public int TotalReservasClientes { get; set; }
        public decimal PromedioReservasPorCliente { get; set; }
        public decimal TotalIngresosClientes { get; set; }
        public List<GraficoDto> DistribucionPorZona { get; set; } = new();
        public List<ClienteDto> TopClientesFrecuentes { get; set; } = new();
        public List<ClienteDto> ClientesRecientes { get; set; } = new();
    }

    // ── Empleado DTOs ──────────────────────────────────────────────────────────

    public class EmpleadoDto
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Ci { get; set; }
        public string? Cargo { get; set; }
        public decimal Salario { get; set; }
        public bool Disponible { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaIngreso { get; set; }
    }

    public class CrearEmpleadoDto
    {
        [Required] public string Nombre { get; set; } = string.Empty;
        [Required] public string Apellido { get; set; } = string.Empty;
        [Required][EmailAddress] public string Email { get; set; } = string.Empty;
        [Required][MinLength(8)] public string Password { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public string? Ci { get; set; }
        public string? Cargo { get; set; }
        public decimal Salario { get; set; }
    }

    public class ActualizarEmpleadoDto
    {
        public string? Cargo { get; set; }
        public decimal? Salario { get; set; }
        public bool? Disponible { get; set; }
        public bool? Activo { get; set; }
    }

    // ── Vehículo DTOs ──────────────────────────────────────────────────────────

    public class VehiculoDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string Placa { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Año { get; set; }
        public string? Color { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearVehiculoDto
    {
        [Required][MaxLength(20)] public string Placa { get; set; } = string.Empty;
        [Required][MaxLength(50)] public string Tipo { get; set; } = string.Empty;
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public string? Año { get; set; }
        public string? Color { get; set; }
    }

    // ── Ubicación DTOs ─────────────────────────────────────────────────────────

    public class UbicacionDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public string? Zona { get; set; }
        public string? Referencia { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public bool EsPrincipal { get; set; }
        public bool Activo { get; set; } = true;
    }

    public class CrearUbicacionDto
    {
        [Required][MaxLength(200)] public string Direccion { get; set; } = string.Empty;
        public string? Zona { get; set; }
        public string? Referencia { get; set; }
        public double? Latitud { get; set; }
        public double? Longitud { get; set; }
        public bool EsPrincipal { get; set; } = false;
    }

    // ── Servicio DTOs ──────────────────────────────────────────────────────────

    public class ServicioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int DuracionMinutos { get; set; }
        public string? TipoVehiculo { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearServicioDto
    {
        [Required][MaxLength(100)] public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        [Required][Range(0, 99999)] public decimal Precio { get; set; }
        [Range(15, 480)] public int DuracionMinutos { get; set; } = 60;
        public string? TipoVehiculo { get; set; }
    }

    // ── Reserva DTOs ───────────────────────────────────────────────────────────

    public class ReservaDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int? EmpleadoId { get; set; }
        public string? NombreEmpleado { get; set; }
        public int VehiculoId { get; set; }
        public string PlacaVehiculo { get; set; } = string.Empty;
        public int UbicacionId { get; set; }
        public string Direccion { get; set; } = string.Empty;
        public int ServicioId { get; set; }
        public string NombreServicio { get; set; } = string.Empty;
        public DateTime FechaProgramada { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string Estado { get; set; } = string.Empty;
        public decimal PrecioTotal { get; set; }
        public string? Observaciones { get; set; }
        public DateTime FechaCreacion { get; set; }
    }

    public class CrearReservaDto
    {
        [Required] public int VehiculoId { get; set; }
        [Required] public int UbicacionId { get; set; }
        [Required] public int ServicioId { get; set; }
        [Required] public DateTime FechaProgramada { get; set; }
        public int? EmpleadoId { get; set; }
        public string? Observaciones { get; set; }
    }

    public class ActualizarEstadoReservaDto
    {
        [Required] public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
    }

    public class EditarReservaDto
    {
        [Required] public DateTime FechaProgramada { get; set; }
        public string? Observaciones { get; set; }
    }

    // ── Producto DTOs ──────────────────────────────────────────────────────────

    public class ProductoDto
    {
        public int Id { get; set; }
        public int CategoriaId { get; set; }
        public string NombreCategoria { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? UnidadMedida { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int StockActual { get; set; }
        public int StockMinimo { get; set; }
        public bool Activo { get; set; }
        public bool StockBajo => StockActual <= StockMinimo;
    }

    public class CrearProductoDto
    {
        [Required] public int CategoriaId { get; set; }
        [Required][MaxLength(150)] public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public string? UnidadMedida { get; set; }
        [Range(0, 99999)] public decimal PrecioUnitario { get; set; }
        [Range(0, int.MaxValue)] public int StockActual { get; set; }
        [Range(0, int.MaxValue)] public int StockMinimo { get; set; } = 5;
    }

    // ── Compra DTOs ────────────────────────────────────────────────────────────

    public class CompraDto
    {
        public int Id { get; set; }
        public int ProveedorId { get; set; }
        public string NombreProveedor { get; set; } = string.Empty;
        public string? NumeroFactura { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public List<DetalleCompraDto> Detalles { get; set; } = new();
    }

    public class DetalleCompraDto
    {
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class CrearCompraDto
    {
        [Required] public int ProveedorId { get; set; }
        public string? NumeroFactura { get; set; }
        public string? Observaciones { get; set; }
        [Required][MinLength(1)] public List<CrearDetalleCompraDto> Detalles { get; set; } = new();
    }

    public class CrearDetalleCompraDto
    {
        [Required] public int ProductoId { get; set; }
        [Required][Range(1, int.MaxValue)] public int Cantidad { get; set; }
        [Required][Range(0, 99999)] public decimal PrecioUnitario { get; set; }
    }

    // ── Venta DTOs ─────────────────────────────────────────────────────────────

    public class VentaDto
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string NumeroVenta { get; set; } = string.Empty;
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaVenta { get; set; }
    }

    // ── Pago DTOs ──────────────────────────────────────────────────────────────

    public class PagoDto
    {
        public int Id { get; set; }
        public int VentaId { get; set; }
        public int ReservaId { get; set; }
        public string NumeroVenta { get; set; } = string.Empty;
        public string MetodoPago { get; set; } = string.Empty;
        public decimal Monto { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Referencia { get; set; }
        public DateTime FechaPago { get; set; }
    }

    public class CrearPagoDto
    {
        [Required] public int VentaId { get; set; }
        [Required] public int ReservaId { get; set; }
        [Required] public int MetodoPagoId { get; set; }
        [Required][Range(0.01, double.MaxValue)] public decimal Monto { get; set; }
        public string? Referencia { get; set; }
    }

    // ── Factura DTOs ───────────────────────────────────────────────────────────

    public class FacturaDto
    {
        public int Id { get; set; }
        public string NumeroFactura { get; set; } = string.Empty;
        public int VentaId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public string? RazonSocial { get; set; }
        public string? Nit { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaEmision { get; set; }
    }

    // ── Calificación DTOs ──────────────────────────────────────────────────────

    public class CalificacionDto
    {
        public int Id { get; set; }
        public int ReservaId { get; set; }
        public int ClienteId { get; set; }
        public string NombreCliente { get; set; } = string.Empty;
        public int Puntuacion { get; set; }
        public string? Comentario { get; set; }
        public DateTime Fecha { get; set; }
    }

    public class CrearCalificacionDto
    {
        [Required] public int ReservaId { get; set; }
        [Required][Range(1, 5)] public int Puntuacion { get; set; }
        [MaxLength(500)] public string? Comentario { get; set; }
    }

    // ── Inventario DTOs ────────────────────────────────────────────────────────

    public class InventarioDto
    {
        public int Id { get; set; }
        public int ProductoId { get; set; }
        public string NombreProducto { get; set; } = string.Empty;
        public string? Categoria { get; set; }
        public int Cantidad { get; set; }
        public int CantidadMinima { get; set; }
        public DateTime UltimaActualizacion { get; set; }
        public bool StockBajo => Cantidad <= CantidadMinima;
    }

    public class AjustarInventarioDto
    {
        [Required] public int ProductoId { get; set; }
        [Required] public int Cantidad { get; set; }
        [Required] public string Tipo { get; set; } = "Ajuste"; // Entrada, Salida, Ajuste
        public string? Motivo { get; set; }
    }

    // ── Dashboard DTOs ─────────────────────────────────────────────────────────

    public class DashboardAdminDto
    {
        public int TotalClientes { get; set; }
        public int TotalEmpleados { get; set; }
        public int ReservasHoy { get; set; }
        public int ServiciosRealizados { get; set; }
        public decimal VentasHoy { get; set; }
        public decimal IngresosMensuales { get; set; }
        public int ProductosStockBajo { get; set; }
        public List<ReservaDto> UltimasReservas { get; set; } = new();
        public List<PagoDto> UltimosPagos { get; set; } = new();
        public List<GraficoDto> VentasSemana { get; set; } = new();
        public List<GraficoDto> ServiciosMasSolicitados { get; set; } = new();
    }

    public class GraficoDto
    {
        public string Etiqueta { get; set; } = string.Empty;
        public decimal Valor { get; set; }
    }

    // ── Notificación DTOs ──────────────────────────────────────────────────────

    public class NotificacionDto
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public bool Leida { get; set; }
        public DateTime Fecha { get; set; }
    }

    // ── Proveedor DTOs ─────────────────────────────────────────────────────────

    public class ProveedorDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
        public string? Email { get; set; }
        public string? Direccion { get; set; }
        public bool Activo { get; set; }
    }

    public class CrearProveedorDto
    {
        [Required][MaxLength(150)] public string Nombre { get; set; } = string.Empty;
        public string? Nit { get; set; }
        public string? Contacto { get; set; }
        public string? Telefono { get; set; }
        [EmailAddress] public string? Email { get; set; }
        public string? Direccion { get; set; }
    }

    // ── Reporte DTOs ───────────────────────────────────────────────────────────

    public class ReporteRequestDto
    {
        [Required] public string Tipo { get; set; } = string.Empty;
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
    }

    // ── Respuesta genérica ─────────────────────────────────────────────────────

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string> Errors { get; set; } = new();

        public static ApiResponse<T> Ok(T data, string message = "Operación exitosa")
            => new() { Success = true, Message = message, Data = data };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null)
            => new() { Success = false, Message = message, Errors = errors ?? new() };
    }
}
