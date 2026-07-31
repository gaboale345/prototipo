using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using BackendApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApi.Controllers
{
    /// <summary>
    /// Controlador para gestión de órdenes de servicio.
    /// El precio se calcula SIEMPRE desde el backend usando los precios de la BD.
    /// El cliente nunca puede modificar el precio desde el frontend.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ServiceOrderController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;
        private readonly ILogger<ServiceOrderController> _logger;

        public ServiceOrderController(EcoWashDbContext context, AuditoriaHelper auditoria, ILogger<ServiceOrderController> logger)
        {
            _context = context;
            _auditoria = auditoria;
            _logger = logger;
        }

        /// <summary>
        /// Crea una orden de servicio. El total se calcula desde el backend.
        /// Solo los clientes pueden crear órdenes.
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<ApiResponse<ServiceOrderDto>>> CreateOrder([FromBody] CreateServiceOrderDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (cliente == null)
                return BadRequest(ApiResponse<ServiceOrderDto>.Fail("Solo los clientes registrados pueden crear órdenes."));

            if (dto.Items == null || dto.Items.Count == 0)
                return BadRequest(ApiResponse<ServiceOrderDto>.Fail("Debe seleccionar al menos un servicio."));

            // REGLA DE SEGURIDAD: Calcular precios desde la BD, NUNCA confiar en el frontend
            var servicioIds = dto.Items.Select(i => i.ServicioId).Distinct().ToList();
            var servicios = await _context.Servicios
                .Where(s => servicioIds.Contains(s.Id) && s.Activo)
                .ToDictionaryAsync(s => s.Id);

            // Validar que todos los servicios existan y estén activos
            var invalidIds = dto.Items
                .Where(item => !servicios.ContainsKey(item.ServicioId))
                .Select(item => item.ServicioId)
                .ToList();

            if (invalidIds.Any())
            {
                _logger.LogWarning("Intento de crear orden con servicios inválidos: {InvalidIds}", string.Join(", ", invalidIds));

                // Consultar si existen pero están inactivos para dar mejor mensaje
                var inactiveServices = await _context.Servicios
                    .Where(s => invalidIds.Contains(s.Id) && !s.Activo)
                    .Select(s => s.Nombre)
                    .ToListAsync();

                if (inactiveServices.Any())
                {
                    return BadRequest(ApiResponse<ServiceOrderDto>.Fail(
                        $"Los siguientes servicios ya no están disponibles: {string.Join(", ", inactiveServices)}. " +
                        "Por favor, actualiza tu selección desde el catálogo."));
                }

                return BadRequest(ApiResponse<ServiceOrderDto>.Fail(
                    $"Servicios con ID {string.Join(", ", invalidIds)} no encontrados. " +
                    "Es posible que el catálogo haya cambiado. Por favor, vuelve al catálogo y selecciona nuevamente."));
            }

            // Generar número de orden único
            var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{DateTime.UtcNow.Ticks.ToString()[^6..]}";

            // Crear la orden
            var order = new ServiceOrder
            {
                ClienteId = cliente.Id,
                NumeroOrden = orderNumber,
                Estado = "Pendiente",
                Observaciones = dto.Observaciones,
                FechaCreacion = DateTime.UtcNow
            };

            decimal subtotal = 0;
            var detalles = new List<ServiceOrderDetail>();

            foreach (var item in dto.Items)
            {
                var servicio = servicios[item.ServicioId];
                var itemSubtotal = servicio.Precio * item.Cantidad;

                detalles.Add(new ServiceOrderDetail
                {
                    ServicioId = servicio.Id,
                    NombreServicio = servicio.Nombre,
                    Cantidad = item.Cantidad,
                    PrecioUnitario = servicio.Precio, // Precio de BD, NO del frontend
                    Subtotal = itemSubtotal
                });

                subtotal += itemSubtotal;
            }

            order.Subtotal = subtotal;
            order.Total = subtotal; // Sin descuento por ahora
            order.Detalles = detalles;

            _context.ServiceOrders.Add(order);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("CrearOrdenServicio", "Ordenes", "ServiceOrder", order.Id, null,
                new { order.NumeroOrden, order.Total, Items = dto.Items.Count }, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString());

            _logger.LogInformation("Orden creada: {OrderNumber}, Total: {Total} BOB, Cliente: {ClienteId}",
                orderNumber, order.Total, cliente.Id);

            return Ok(ApiResponse<ServiceOrderDto>.Ok(MapToDto(order, cliente), "Orden creada exitosamente."));
        }

        /// <summary>
        /// Obtiene el resumen de una orden antes del pago.
        /// </summary>
        [HttpGet("{id}/summary")]
        public async Task<ActionResult<ApiResponse<ServiceOrderDto>>> GetOrderSummary(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var order = await _context.ServiceOrders
                .Include(o => o.Detalles)
                .Include(o => o.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(ApiResponse<ServiceOrderDto>.Fail("Orden no encontrada."));

            // Solo el dueño o admin puede ver la orden
            var role = User.FindFirstValue(ClaimTypes.Role);
            if (role == "Cliente" && order.Cliente.UsuarioId != userId)
                return Forbid();

            return Ok(ApiResponse<ServiceOrderDto>.Ok(MapToDto(order, order.Cliente)));
        }

        /// <summary>
        /// Obtiene las órdenes del cliente autenticado.
        /// </summary>
        [HttpGet("my-orders")]
        public async Task<ActionResult<ApiResponse<List<ServiceOrderDto>>>> GetMyOrders(
            [FromQuery] string? estado = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes
                .Include(c => c.Usuario)
                .FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (cliente == null)
                return Ok(ApiResponse<List<ServiceOrderDto>>.Ok(new List<ServiceOrderDto>()));

            IQueryable<ServiceOrder> query = _context.ServiceOrders
                .Include(o => o.Detalles)
                .Include(o => o.Cliente).ThenInclude(c => c.Usuario)
                .Where(o => o.ClienteId == cliente.Id);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(o => o.Estado == estado);
            if (fechaInicio.HasValue)
                query = query.Where(o => o.FechaCreacion >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(o => o.FechaCreacion <= fechaFin.Value);

            var orders = await query.OrderByDescending(o => o.FechaCreacion).ToListAsync();
            var result = orders.Select(o => MapToDto(o, o.Cliente)).ToList();

            return Ok(ApiResponse<List<ServiceOrderDto>>.Ok(result));
        }

        /// <summary>
        /// Obtiene el detalle completo de una orden.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ServiceOrderDto>>> GetOrder(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var order = await _context.ServiceOrders
                .Include(o => o.Detalles)
                .Include(o => o.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null)
                return NotFound(ApiResponse<ServiceOrderDto>.Fail("Orden no encontrada."));

            if (role == "Cliente" && order.Cliente.UsuarioId != userId)
                return Forbid();

            return Ok(ApiResponse<ServiceOrderDto>.Ok(MapToDto(order, order.Cliente)));
        }

        /// <summary>Mapea una ServiceOrder a su DTO correspondiente.</summary>
        private static ServiceOrderDto MapToDto(ServiceOrder order, Cliente cliente)
        {
            return new ServiceOrderDto
            {
                Id = order.Id,
                ClienteId = order.ClienteId,
                NombreCliente = $"{cliente.Usuario.Nombre} {cliente.Usuario.Apellido}",
                EmailCliente = cliente.Usuario.Email,
                NumeroOrden = order.NumeroOrden,
                Subtotal = order.Subtotal,
                Total = order.Total,
                Estado = order.Estado,
                FechaCreacion = order.FechaCreacion,
                FechaPago = order.FechaPago,
                Observaciones = order.Observaciones,
                Detalles = order.Detalles.Select(d => new ServiceOrderDetailDto
                {
                    Id = d.Id,
                    ServicioId = d.ServicioId,
                    NombreServicio = d.NombreServicio,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            };
        }
    }
}
