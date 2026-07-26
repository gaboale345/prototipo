using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Text.Json;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ReporteController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public ReporteController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Reporte>>>> GetReportes()
        {
            var list = await _context.Reportes.OrderByDescending(r => r.FechaGeneracion).ToListAsync();
            return Ok(ApiResponse<List<Reporte>>.Ok(list));
        }

        [HttpPost("generar")]
        public async Task<ActionResult<ApiResponse<Reporte>>> GenerarReporte([FromBody] ReporteRequestDto dto)
        {
            // REGLA DE NEGOCIO: Solo el administrador puede generar reportes.
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var inicio = dto.FechaInicio ?? DateTime.UtcNow.AddDays(-30);
            var fin = dto.FechaFin ?? DateTime.UtcNow;

            object resultadoDatos = dto.Tipo switch
            {
                "VentasDiarias" => await _context.Ventas
                    .Where(v => v.FechaVenta >= inicio && v.FechaVenta <= fin && v.Estado == "Pagada")
                    .GroupBy(v => v.FechaVenta.Date)
                    .Select(g => new { Fecha = g.Key.ToString("yyyy-MM-dd"), TotalVentas = g.Count(), TotalIngresos = g.Sum(x => x.Total) })
                    .ToListAsync(),

                "VentasMensuales" => await _context.Ventas
                    .Where(v => v.FechaVenta >= inicio && v.FechaVenta <= fin && v.Estado == "Pagada")
                    .GroupBy(v => new { v.FechaVenta.Year, v.FechaVenta.Month })
                    .Select(g => new { Año = g.Key.Year, Mes = g.Key.Month, TotalIngresos = g.Sum(x => x.Total) })
                    .ToListAsync(),

                "ServiciosMasSolicitados" => await _context.Reservas
                    .Include(r => r.Servicio)
                    .GroupBy(r => r.Servicio.Nombre)
                    .Select(g => new { Servicio = g.Key, Cantidad = g.Count() })
                    .OrderByDescending(x => x.Cantidad)
                    .ToListAsync(),

                "ClientesFrecuentes" => await _context.Reservas
                    .Include(r => r.Cliente).ThenInclude(c => c.Usuario)
                    .GroupBy(r => $"{r.Cliente.Usuario.Nombre} {r.Cliente.Usuario.Apellido}")
                    .Select(g => new { Cliente = g.Key, TotalReservas = g.Count() })
                    .OrderByDescending(x => x.TotalReservas)
                    .ToListAsync(),

                _ => new { Mensaje = "Reporte general generado", TotalVentas = await _context.Ventas.CountAsync() }
            };

            var reporte = new Reporte
            {
                UsuarioId = userId,
                Nombre = $"Reporte de {dto.Tipo}",
                Tipo = dto.Tipo,
                FechaInicio = inicio,
                FechaFin = fin,
                Datos = JsonSerializer.Serialize(resultadoDatos),
                FechaGeneracion = DateTime.UtcNow
            };

            _context.Reportes.Add(reporte);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<Reporte>.Ok(reporte, "Reporte generado con éxito"));
        }
    }
}
