using BackendApi.Data;
using BackendApi.DTOs;
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
    public class FacturaController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public FacturaController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<FacturaDto>>>> GetFacturas()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Factura> query = _context.Facturas
                .Include(f => f.Venta).ThenInclude(v => v.Cliente).ThenInclude(c => c.Usuario);

            if (role == "Cliente")
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cliente == null) return Ok(ApiResponse<List<FacturaDto>>.Ok(new List<FacturaDto>()));
                query = query.Where(f => f.Venta.ClienteId == cliente.Id);
            }

            var result = await query.OrderByDescending(f => f.FechaEmision)
                .Select(f => new FacturaDto
                {
                    Id = f.Id,
                    NumeroFactura = f.NumeroFactura,
                    VentaId = f.VentaId,
                    NombreCliente = f.RazonSocial ?? $"{f.Venta.Cliente.Usuario.Nombre} {f.Venta.Cliente.Usuario.Apellido}",
                    RazonSocial = f.RazonSocial,
                    Nit = f.Nit,
                    Subtotal = f.Subtotal,
                    Descuento = f.Descuento,
                    Total = f.Total,
                    Estado = f.Estado,
                    FechaEmision = f.FechaEmision
                }).ToListAsync();

            return Ok(ApiResponse<List<FacturaDto>>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<FacturaDto>>> GetFactura(int id)
        {
            var f = await _context.Facturas
                .Include(x => x.Venta).ThenInclude(v => v.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (f == null) return NotFound(ApiResponse<FacturaDto>.Fail("Factura no encontrada"));

            var dto = new FacturaDto
            {
                Id = f.Id,
                NumeroFactura = f.NumeroFactura,
                VentaId = f.VentaId,
                NombreCliente = f.RazonSocial ?? $"{f.Venta.Cliente.Usuario.Nombre} {f.Venta.Cliente.Usuario.Apellido}",
                RazonSocial = f.RazonSocial,
                Nit = f.Nit,
                Subtotal = f.Subtotal,
                Descuento = f.Descuento,
                Total = f.Total,
                Estado = f.Estado,
                FechaEmision = f.FechaEmision
            };

            return Ok(ApiResponse<FacturaDto>.Ok(dto));
        }
    }
}
