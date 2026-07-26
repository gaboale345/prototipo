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
    public class VentaController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public VentaController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<VentaDto>>>> GetVentas()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Venta> query = _context.Ventas
                .Include(v => v.Cliente).ThenInclude(c => c.Usuario);

            if (role == "Cliente")
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cliente == null) return Ok(ApiResponse<List<VentaDto>>.Ok(new List<VentaDto>()));
                query = query.Where(v => v.ClienteId == cliente.Id);
            }

            var result = await query.OrderByDescending(v => v.FechaVenta)
                .Select(v => new VentaDto
                {
                    Id = v.Id,
                    ReservaId = v.ReservaId,
                    ClienteId = v.ClienteId,
                    NombreCliente = $"{v.Cliente.Usuario.Nombre} {v.Cliente.Usuario.Apellido}",
                    NumeroVenta = v.NumeroVenta,
                    Subtotal = v.Subtotal,
                    Descuento = v.Descuento,
                    Total = v.Total,
                    Estado = v.Estado,
                    FechaVenta = v.FechaVenta
                }).ToListAsync();

            return Ok(ApiResponse<List<VentaDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<VentaDto>>> GetVenta(int id)
        {
            var v = await _context.Ventas
                .Include(x => x.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (v == null) return NotFound(ApiResponse<VentaDto>.Fail("Venta no encontrada"));

            var dto = new VentaDto
            {
                Id = v.Id,
                ReservaId = v.ReservaId,
                ClienteId = v.ClienteId,
                NombreCliente = $"{v.Cliente.Usuario.Nombre} {v.Cliente.Usuario.Apellido}",
                NumeroVenta = v.NumeroVenta,
                Subtotal = v.Subtotal,
                Descuento = v.Descuento,
                Total = v.Total,
                Estado = v.Estado,
                FechaVenta = v.FechaVenta
            };

            return Ok(ApiResponse<VentaDto>.Ok(dto));
        }
    }
}
