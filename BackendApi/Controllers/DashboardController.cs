using BackendApi.Data;
using BackendApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public DashboardController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet("admin")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<DashboardAdminDto>>> GetDashboardAdmin()
        {
            var hoy = DateTime.UtcNow.Date;
            var inicioMes = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1, 0, 0, 0, DateTimeKind.Utc);

            var dto = new DashboardAdminDto
            {
                TotalClientes = await _context.Clientes.CountAsync(c => c.Activo),
                TotalEmpleados = await _context.Empleados.CountAsync(e => e.Activo),
                ReservasHoy = await _context.Reservas.CountAsync(r => r.FechaProgramada.Date == hoy),
                ServiciosRealizados = await _context.Reservas.CountAsync(r => r.Estado == "Finalizada"),
                VentasHoy = (decimal)(await _context.Ventas.Where(v => v.FechaVenta.Date == hoy && v.Estado == "Pagada").SumAsync(v => (double?)v.Total) ?? 0),
                IngresosMensuales = (decimal)(await _context.Ventas.Where(v => v.FechaVenta >= inicioMes && v.Estado == "Pagada").SumAsync(v => (double?)v.Total) ?? 0),
                ProductosStockBajo = await _context.Productos.CountAsync(p => p.Activo && p.StockActual <= p.StockMinimo),

                UltimasReservas = await _context.Reservas
                    .Include(r => r.Cliente).ThenInclude(c => c.Usuario)
                    .Include(r => r.Empleado).ThenInclude(e => e!.Usuario)
                    .Include(r => r.Vehiculo)
                    .Include(r => r.Ubicacion)
                    .Include(r => r.Servicio)
                    .OrderByDescending(r => r.FechaProgramada)
                    .Take(5)
                    .Select(r => new ReservaDto
                    {
                        Id = r.Id,
                        ClienteId = r.ClienteId,
                        NombreCliente = $"{r.Cliente.Usuario.Nombre} {r.Cliente.Usuario.Apellido}",
                        EmpleadoId = r.EmpleadoId,
                        NombreEmpleado = r.Empleado != null ? $"{r.Empleado.Usuario.Nombre} {r.Empleado.Usuario.Apellido}" : "Sin Asignar",
                        VehiculoId = r.VehiculoId,
                        PlacaVehiculo = r.Vehiculo.Placa,
                        UbicacionId = r.UbicacionId,
                        Direccion = r.Ubicacion.Direccion,
                        ServicioId = r.ServicioId,
                        NombreServicio = r.Servicio.Nombre,
                        FechaProgramada = r.FechaProgramada,
                        Estado = r.Estado,
                        PrecioTotal = r.PrecioTotal,
                        FechaCreacion = r.FechaCreacion
                    }).ToListAsync(),

                UltimosPagos = await _context.Pagos
                    .Include(p => p.Venta)
                    .Include(p => p.MetodoPago)
                    .OrderByDescending(p => p.FechaPago)
                    .Take(5)
                    .Select(p => new PagoDto
                    {
                        Id = p.Id,
                        VentaId = p.VentaId,
                        ReservaId = p.ReservaId,
                        NumeroVenta = p.Venta.NumeroVenta,
                        MetodoPago = p.MetodoPago.Nombre,
                        Monto = p.Monto,
                        Estado = p.Estado,
                        Referencia = p.Referencia,
                        FechaPago = p.FechaPago
                    }).ToListAsync(),

                ServiciosMasSolicitados = await _context.Reservas
                    .Include(r => r.Servicio)
                    .GroupBy(r => r.Servicio.Nombre)
                    .Select(g => new GraficoDto { Etiqueta = g.Key, Valor = g.Count() })
                    .OrderByDescending(g => g.Valor)
                    .Take(5)
                    .ToListAsync()
            };

            return Ok(ApiResponse<DashboardAdminDto>.Ok(dto));
        }
    }
}
