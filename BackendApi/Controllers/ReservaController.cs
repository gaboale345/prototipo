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
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReservaController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public ReservaController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ReservaDto>>>> GetReservas([FromQuery] string? estado = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Reserva> query = _context.Reservas
                .Include(r => r.Cliente).ThenInclude(c => c.Usuario)
                .Include(r => r.Empleado).ThenInclude(e => e!.Usuario)
                .Include(r => r.Vehiculo)
                .Include(r => r.Ubicacion)
                .Include(r => r.Servicio);

            if (role == "Cliente")
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cliente == null) return Ok(ApiResponse<List<ReservaDto>>.Ok(new List<ReservaDto>()));
                query = query.Where(r => r.ClienteId == cliente.Id);
            }
            else if (role == "Empleado")
            {
                var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.UsuarioId == userId);
                if (empleado == null) return Ok(ApiResponse<List<ReservaDto>>.Ok(new List<ReservaDto>()));
                query = query.Where(r => r.EmpleadoId == empleado.Id || r.EmpleadoId == null);
            }

            if (!string.IsNullOrEmpty(estado))
            {
                query = query.Where(r => r.Estado.ToLower() == estado.ToLower());
            }

            var result = await query.OrderByDescending(r => r.FechaProgramada)
                .Select(r => new ReservaDto
                {
                    Id = r.Id,
                    ClienteId = r.ClienteId,
                    NombreCliente = $"{r.Cliente.Usuario.Nombre} {r.Cliente.Usuario.Apellido}",
                    EmpleadoId = r.EmpleadoId,
                    NombreEmpleado = r.Empleado != null ? $"{r.Empleado.Usuario.Nombre} {r.Empleado.Usuario.Apellido}" : "Sin Asignar",
                    VehiculoId = r.VehiculoId,
                    PlacaVehiculo = $"{r.Vehiculo.Marca} {r.Vehiculo.Modelo} ({r.Vehiculo.Placa})",
                    UbicacionId = r.UbicacionId,
                    Direccion = r.Ubicacion.Direccion,
                    ServicioId = r.ServicioId,
                    NombreServicio = r.Servicio.Nombre,
                    FechaProgramada = r.FechaProgramada,
                    FechaInicio = r.FechaInicio,
                    FechaFin = r.FechaFin,
                    Estado = r.Estado,
                    PrecioTotal = r.PrecioTotal,
                    Observaciones = r.Observaciones,
                    FechaCreacion = r.FechaCreacion
                }).ToListAsync();

            return Ok(ApiResponse<List<ReservaDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ReservaDto>>> CrearReserva([FromBody] CrearReservaDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
            if (cliente == null) return BadRequest(ApiResponse<ReservaDto>.Fail("Solo los clientes registrados pueden realizar reservas (Regla de Negocio)"));

            var servicio = await _context.Servicios.FindAsync(dto.ServicioId);
            if (servicio == null || !servicio.Activo) return BadRequest(ApiResponse<ReservaDto>.Fail("Servicio no disponible"));

            var vehiculo = await _context.Vehiculos.FirstOrDefaultAsync(v => v.Id == dto.VehiculoId && v.ClienteId == cliente.Id && v.Activo);
            if (vehiculo == null) return BadRequest(ApiResponse<ReservaDto>.Fail("Vehículo no pertenece al cliente (Regla de Negocio)"));

            var ubicacion = await _context.Ubicaciones.FirstOrDefaultAsync(u => u.Id == dto.UbicacionId && u.ClienteId == cliente.Id && u.Activo);
            if (ubicacion == null) return BadRequest(ApiResponse<ReservaDto>.Fail("Ubicación inválida"));

            // Regla de Negocio: Asignación de empleado y disponibilidad de horario
            int? empleadoAsignadoId = dto.EmpleadoId;
            if (empleadoAsignadoId.HasValue)
            {
                var empleado = await _context.Empleados.FindAsync(empleadoAsignadoId.Value);
                if (empleado == null || !empleado.Disponible || !empleado.Activo)
                    return BadRequest(ApiResponse<ReservaDto>.Fail("El empleado seleccionado no está disponible (Regla de Negocio)"));

                var choqueHorario = await _context.Reservas.AnyAsync(r =>
                    r.EmpleadoId == empleadoAsignadoId.Value &&
                    r.Estado != "Cancelada" && r.Estado != "Rechazada" &&
                    Math.Abs((r.FechaProgramada - dto.FechaProgramada).TotalMinutes) < servicio.DuracionMinutos);

                if (choqueHorario)
                    return BadRequest(ApiResponse<ReservaDto>.Fail("El empleado ya tiene una reserva en ese horario (Regla de Negocio)"));
            }
            else
            {
                // Asignación automática de empleado libre
                var disponible = await _context.Empleados.FirstOrDefaultAsync(e => e.Activo && e.Disponible);
                if (disponible != null)
                {
                    empleadoAsignadoId = disponible.Id;
                }
            }

            var reserva = new Reserva
            {
                ClienteId = cliente.Id,
                EmpleadoId = empleadoAsignadoId,
                VehiculoId = vehiculo.Id,
                UbicacionId = ubicacion.Id,
                ServicioId = servicio.Id,
                FechaProgramada = dto.FechaProgramada,
                Estado = "Pendiente",
                PrecioTotal = servicio.Precio,
                Observaciones = dto.Observaciones,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Reservas.Add(reserva);
            await _context.SaveChangesAsync();

            // Notificación
            _context.Notificaciones.Add(new Notificacion
            {
                UsuarioId = userId,
                Titulo = "Reserva Creada",
                Mensaje = $"Tu reserva para {servicio.Nombre} ha sido registrada para el {dto.FechaProgramada:dd/MM/yyyy HH:mm}.",
                Tipo = "Info",
                Fecha = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("CrearReserva", "Reservas", "Reserva", reserva.Id, null, dto, userId);

            return Ok(ApiResponse<ReservaDto>.Ok(new ReservaDto
            {
                Id = reserva.Id,
                ClienteId = cliente.Id,
                NombreCliente = $"{User.FindFirstValue(ClaimTypes.Name)}",
                EmpleadoId = reserva.EmpleadoId,
                VehiculoId = vehiculo.Id,
                PlacaVehiculo = vehiculo.Placa,
                UbicacionId = ubicacion.Id,
                Direccion = ubicacion.Direccion,
                ServicioId = servicio.Id,
                NombreServicio = servicio.Nombre,
                FechaProgramada = reserva.FechaProgramada,
                Estado = reserva.Estado,
                PrecioTotal = reserva.PrecioTotal,
                FechaCreacion = reserva.FechaCreacion
            }, "Reserva realizada exitosamente"));
        }

        [HttpPut("{id}/estado")]
        public async Task<ActionResult<ApiResponse<string>>> CambiarEstado(int id, [FromBody] ActualizarEstadoReservaDto dto)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Servicio)
                .Include(r => r.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (reserva == null) return NotFound(ApiResponse<string>.Fail("Reserva no encontrada"));

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            // Transiciones de estado
            if (dto.Estado == "Aceptada")
            {
                if (role == "Empleado")
                {
                    var empleado = await _context.Empleados.FirstOrDefaultAsync(e => e.UsuarioId == userId);
                    if (empleado != null) reserva.EmpleadoId = empleado.Id;
                }
                reserva.Estado = "Aceptada";
            }
            else if (dto.Estado == "EnProceso")
            {
                reserva.Estado = "EnProceso";
                reserva.FechaInicio = DateTime.UtcNow;
            }
            else if (dto.Estado == "Finalizada")
            {
                reserva.Estado = "Finalizada";
                reserva.FechaFin = DateTime.UtcNow;

                // REGLA DE NEGOCIO: Una venta se genera únicamente al finalizar el servicio.
                var venta = new Venta
                {
                    ReservaId = reserva.Id,
                    ClienteId = reserva.ClienteId,
                    NumeroVenta = $"V-ECO-{DateTime.UtcNow.Ticks.ToString()[^6..]}",
                    Subtotal = reserva.PrecioTotal,
                    Descuento = 0,
                    Total = reserva.PrecioTotal,
                    FechaVenta = DateTime.UtcNow,
                    Estado = "Pendiente"
                };
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();

                _context.DetalleVentas.Add(new DetalleVenta
                {
                    VentaId = venta.Id,
                    Descripcion = $"Servicio: {reserva.Servicio.Nombre}",
                    Cantidad = 1,
                    PrecioUnitario = reserva.PrecioTotal,
                    Subtotal = reserva.PrecioTotal
                });

                // REGLA DE NEGOCIO: Cada servicio descuenta automáticamente productos del inventario.
                var primerProducto = await _context.Productos.Include(p => p.Inventario).FirstOrDefaultAsync(p => p.Activo && p.StockActual > 0);
                if (primerProducto != null && primerProducto.Inventario != null)
                {
                    int cantAnterior = primerProducto.StockActual;
                    primerProducto.StockActual -= 1;
                    primerProducto.Inventario.Cantidad = primerProducto.StockActual;
                    primerProducto.Inventario.UltimaActualizacion = DateTime.UtcNow;

                    _context.MovimientosInventario.Add(new MovimientoInventario
                    {
                        InventarioId = primerProducto.Inventario.Id,
                        ProductoId = primerProducto.Id,
                        UsuarioId = userId,
                        ReservaId = reserva.Id,
                        Tipo = "Salida",
                        Cantidad = 1,
                        CantidadAnterior = cantAnterior,
                        CantidadNueva = primerProducto.StockActual,
                        Motivo = $"Uso automático en Servicio #{reserva.Id}",
                        Fecha = DateTime.UtcNow
                    });

                    // REGLA DE NEGOCIO: Notificación cuando stock llega al mínimo.
                    if (primerProducto.StockActual <= primerProducto.StockMinimo)
                    {
                        var admins = await _context.Usuarios.Where(u => u.Rol.Nombre == "Administrador").ToListAsync();
                        foreach (var adm in admins)
                        {
                            _context.Notificaciones.Add(new Notificacion
                            {
                                UsuarioId = adm.Id,
                                Titulo = "Stock Bajo Detectado",
                                Mensaje = $"El producto '{primerProducto.Nombre}' tiene un stock actual de {primerProducto.StockActual} (Mínimo: {primerProducto.StockMinimo}).",
                                Tipo = "Alerta",
                                Fecha = DateTime.UtcNow
                            });
                        }
                    }
                }
            }
            else if (dto.Estado == "Cancelada" || dto.Estado == "Rechazada")
            {
                reserva.Estado = dto.Estado;
            }

            await _context.SaveChangesAsync();

            // Notificación al cliente
            _context.Notificaciones.Add(new Notificacion
            {
                UsuarioId = reserva.Cliente.UsuarioId,
                Titulo = $"Reserva {dto.Estado}",
                Mensaje = $"Tu reserva #{reserva.Id} ha cambiado su estado a: {dto.Estado}.",
                Tipo = dto.Estado == "Finalizada" ? "Exito" : "Info",
                Fecha = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("CambiarEstadoReserva", "Reservas", "Reserva", id, null, new { dto.Estado }, userId);

            return Ok(ApiResponse<string>.Ok($"Estado de la reserva cambiado a {dto.Estado}"));
        }
    }
}
