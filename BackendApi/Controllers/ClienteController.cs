using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ClienteController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public ClienteController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        private async Task<List<ClienteDto>> GetClientesListInternalAsync()
        {
            var clientesRaw = await _context.Clientes
                .Include(c => c.Usuario)
                .Include(c => c.Vehiculos)
                .Include(c => c.Reservas)
                .Include(c => c.Ubicaciones)
                .OrderByDescending(c => c.FechaRegistro)
                .ToListAsync();

            var reservasPagadas = await _context.Reservas
                .Include(r => r.Venta)
                .Where(r => r.Venta != null && r.Venta.Estado == "Pagada")
                .Select(r => new { r.ClienteId, r.PrecioTotal, r.FechaProgramada })
                .ToListAsync();

            var gastosPorCliente = reservasPagadas
                .GroupBy(r => r.ClienteId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.PrecioTotal));

            var ultimaReservaPorCliente = reservasPagadas
                .GroupBy(r => r.ClienteId)
                .ToDictionary(g => g.Key, g => g.Max(x => (DateTime?)x.FechaProgramada));

            return clientesRaw.Select(c => new ClienteDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                NombreCompleto = c.Usuario != null ? $"{c.Usuario.Nombre} {c.Usuario.Apellido}" : "Cliente Sin Nombre",
                Email = c.Usuario?.Email ?? "sin-email@ecowash.bo",
                Telefono = c.Usuario?.Telefono,
                Ci = c.Ci,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                ZonaPrincipal = c.Ubicaciones?.FirstOrDefault(u => u.EsPrincipal)?.Zona ?? c.Ubicaciones?.FirstOrDefault()?.Zona ?? "Santa Cruz",
                FechaRegistro = c.FechaRegistro,
                Activo = c.Activo,
                TotalVehiculos = c.Vehiculos?.Count ?? 0,
                TotalReservas = c.Reservas?.Count ?? 0,
                TotalGastado = gastosPorCliente.GetValueOrDefault(c.Id, 0m),
                UltimaReservaFecha = ultimaReservaPorCliente.GetValueOrDefault(c.Id),
                Vehiculos = c.Vehiculos?.Select(v => new VehiculoDto
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    NombreCliente = c.Usuario != null ? $"{c.Usuario.Nombre} {c.Usuario.Apellido}" : "",
                    Placa = v.Placa,
                    Tipo = v.Tipo,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Año = v.Año,
                    Color = v.Color,
                    Activo = v.Activo
                }).ToList() ?? new List<VehiculoDto>(),
                Ubicaciones = c.Ubicaciones?.Select(u => new UbicacionDto
                {
                    Id = u.Id,
                    ClienteId = u.ClienteId,
                    Direccion = u.Direccion,
                    Zona = u.Zona,
                    Referencia = u.Referencia,
                    EsPrincipal = u.EsPrincipal,
                    Activo = u.Activo
                }).ToList() ?? new List<UbicacionDto>()
            }).ToList();
        }

        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<ActionResult<ApiResponse<List<ClienteDto>>>> GetClientes()
        {
            var dtos = await GetClientesListInternalAsync();
            return Ok(ApiResponse<List<ClienteDto>>.Ok(dtos));
        }

        [HttpGet("dashboard")]
        [Authorize(Roles = "Administrador,Empleado")]
        public async Task<ActionResult<ApiResponse<ClienteDashboardSummaryDto>>> GetDashboardClientes()
        {
            var totalClientes = await _context.Clientes.CountAsync();
            var clientesActivos = await _context.Clientes.CountAsync(c => c.Activo);
            var totalVehiculos = await _context.Vehiculos.CountAsync(v => v.Activo);
            var totalReservas = await _context.Reservas.CountAsync();

            var ventasPagadasTotales = await _context.Ventas.Where(v => v.Estado == "Pagada").Select(v => v.Total).ToListAsync();
            var totalIngresos = ventasPagadasTotales.Sum();
            decimal promedioReservas = totalClientes > 0 ? (decimal)totalReservas / totalClientes : 0m;

            var distribucionZonaRaw = await _context.Ubicaciones
                .Where(u => !string.IsNullOrEmpty(u.Zona))
                .GroupBy(u => u.Zona)
                .Select(g => new GraficoDto { Etiqueta = g.Key!, Valor = g.Select(u => u.ClienteId).Distinct().Count() })
                .ToListAsync();

            var distribucionZona = distribucionZonaRaw
                .OrderByDescending(g => g.Valor)
                .Take(7)
                .ToList();

            var allClientes = await GetClientesListInternalAsync();

            var topClientes = allClientes.OrderByDescending(c => c.TotalReservas).ThenByDescending(c => c.TotalGastado).Take(5).ToList();
            var recientes = allClientes.OrderByDescending(c => c.FechaRegistro).Take(5).ToList();

            var summary = new ClienteDashboardSummaryDto
            {
                TotalClientes = totalClientes,
                ClientesActivos = clientesActivos,
                TotalVehiculosRegistrados = totalVehiculos,
                TotalReservasClientes = totalReservas,
                PromedioReservasPorCliente = Math.Round(promedioReservas, 1),
                TotalIngresosClientes = totalIngresos,
                DistribucionPorZona = distribucionZona,
                TopClientesFrecuentes = topClientes,
                ClientesRecientes = recientes
            };

            return Ok(ApiResponse<ClienteDashboardSummaryDto>.Ok(summary));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ClienteDto>>> GetCliente(int id)
        {
            var c = await _context.Clientes
                .Include(x => x.Usuario)
                .Include(x => x.Vehiculos)
                .Include(x => x.Reservas)
                .Include(x => x.Ubicaciones)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (c == null) return NotFound(ApiResponse<ClienteDto>.Fail("Cliente no encontrado"));

            var preciosReservas = await _context.Reservas
                .Where(r => r.ClienteId == id && r.Venta != null && r.Venta.Estado == "Pagada")
                .Select(r => r.PrecioTotal)
                .ToListAsync();
            var totalGastado = preciosReservas.Sum();

            var dto = new ClienteDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                NombreCompleto = c.Usuario != null ? $"{c.Usuario.Nombre} {c.Usuario.Apellido}" : "Cliente Sin Nombre",
                Email = c.Usuario?.Email ?? "sin-email@ecowash.bo",
                Telefono = c.Usuario?.Telefono,
                Ci = c.Ci,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                ZonaPrincipal = c.Ubicaciones?.FirstOrDefault(u => u.EsPrincipal)?.Zona ?? c.Ubicaciones?.FirstOrDefault()?.Zona ?? "Santa Cruz",
                FechaRegistro = c.FechaRegistro,
                Activo = c.Activo,
                TotalVehiculos = c.Vehiculos?.Count ?? 0,
                TotalReservas = c.Reservas?.Count ?? 0,
                TotalGastado = totalGastado,
                UltimaReservaFecha = c.Reservas?.OrderByDescending(r => r.FechaProgramada).FirstOrDefault()?.FechaProgramada,
                Vehiculos = c.Vehiculos?.Select(v => new VehiculoDto
                {
                    Id = v.Id,
                    ClienteId = v.ClienteId,
                    NombreCliente = c.Usuario != null ? $"{c.Usuario.Nombre} {c.Usuario.Apellido}" : "",
                    Placa = v.Placa,
                    Tipo = v.Tipo,
                    Marca = v.Marca,
                    Modelo = v.Modelo,
                    Año = v.Año,
                    Color = v.Color,
                    Activo = v.Activo
                }).ToList() ?? new List<VehiculoDto>(),
                Ubicaciones = c.Ubicaciones?.Select(u => new UbicacionDto
                {
                    Id = u.Id,
                    ClienteId = u.ClienteId,
                    Direccion = u.Direccion,
                    Zona = u.Zona,
                    Referencia = u.Referencia,
                    EsPrincipal = u.EsPrincipal,
                    Activo = u.Activo
                }).ToList() ?? new List<UbicacionDto>()
            };

            return Ok(ApiResponse<ClienteDto>.Ok(dto));
        }

        [HttpGet("me")]
        public async Task<ActionResult<ApiResponse<ClienteDto>>> GetMiPerfilCliente()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var c = await _context.Clientes
                .Include(x => x.Usuario)
                .Include(x => x.Vehiculos)
                .Include(x => x.Reservas)
                .Include(x => x.Ubicaciones)
                .FirstOrDefaultAsync(x => x.UsuarioId == userId);

            if (c == null) return NotFound(ApiResponse<ClienteDto>.Fail("Cliente no encontrado"));

            var dto = new ClienteDto
            {
                Id = c.Id,
                UsuarioId = c.UsuarioId,
                NombreCompleto = c.Usuario != null ? $"{c.Usuario.Nombre} {c.Usuario.Apellido}" : "Cliente",
                Email = c.Usuario?.Email ?? "",
                Telefono = c.Usuario?.Telefono,
                Ci = c.Ci,
                Direccion = c.Direccion,
                Ciudad = c.Ciudad,
                ZonaPrincipal = c.Ubicaciones?.FirstOrDefault(u => u.EsPrincipal)?.Zona ?? "Santa Cruz",
                FechaRegistro = c.FechaRegistro,
                Activo = c.Activo,
                TotalVehiculos = c.Vehiculos?.Count ?? 0,
                TotalReservas = c.Reservas?.Count ?? 0
            };

            return Ok(ApiResponse<ClienteDto>.Ok(dto));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> ActualizarCliente(int id, [FromBody] ActualizarClienteDto dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound(ApiResponse<string>.Fail("Cliente no encontrado"));

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (cliente.UsuarioId != userId && role != "Administrador") return Forbid();

            if (dto.Ci != null) cliente.Ci = dto.Ci;
            if (dto.Direccion != null) cliente.Direccion = dto.Direccion;
            if (dto.Ciudad != null) cliente.Ciudad = dto.Ciudad;

            await _context.SaveChangesAsync();
            await _auditoria.RegistrarAsync("ActualizarCliente", "Clientes", "Cliente", id, null, dto, userId);

            return Ok(ApiResponse<string>.Ok("Datos del cliente actualizados"));
        }
    }
}
