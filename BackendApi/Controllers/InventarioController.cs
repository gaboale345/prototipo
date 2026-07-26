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
    public class InventarioController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public InventarioController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<InventarioDto>>>> GetInventario()
        {
            var items = await _context.Inventarios
                .Include(i => i.Producto).ThenInclude(p => p.Categoria)
                .Where(i => i.Producto.Activo)
                .Select(i => new InventarioDto
                {
                    Id = i.Id,
                    ProductoId = i.ProductoId,
                    NombreProducto = i.Producto.Nombre,
                    Categoria = i.Producto.Categoria.Nombre,
                    Cantidad = i.Cantidad,
                    CantidadMinima = i.CantidadMinima,
                    UltimaActualizacion = i.UltimaActualizacion
                }).ToListAsync();

            return Ok(ApiResponse<List<InventarioDto>>.Ok(items));
        }

        [HttpGet("stock-bajo")]
        public async Task<ActionResult<ApiResponse<List<InventarioDto>>>> GetStockBajo()
        {
            var items = await _context.Inventarios
                .Include(i => i.Producto).ThenInclude(p => p.Categoria)
                .Where(i => i.Producto.Activo && i.Cantidad <= i.CantidadMinima)
                .Select(i => new InventarioDto
                {
                    Id = i.Id,
                    ProductoId = i.ProductoId,
                    NombreProducto = i.Producto.Nombre,
                    Categoria = i.Producto.Categoria.Nombre,
                    Cantidad = i.Cantidad,
                    CantidadMinima = i.CantidadMinima,
                    UltimaActualizacion = i.UltimaActualizacion
                }).ToListAsync();

            return Ok(ApiResponse<List<InventarioDto>>.Ok(items));
        }

        [HttpPost("ajustar")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<string>>> AjustarStock([FromBody] AjustarInventarioDto dto)
        {
            var producto = await _context.Productos.Include(p => p.Inventario).FirstOrDefaultAsync(p => p.Id == dto.ProductoId);
            if (producto == null) return NotFound(ApiResponse<string>.Fail("Producto no encontrado"));

            int cantAnterior = producto.StockActual;
            int cantNueva = dto.Tipo == "Entrada" ? cantAnterior + dto.Cantidad :
                            dto.Tipo == "Salida" ? cantAnterior - dto.Cantidad : dto.Cantidad;

            if (cantNueva < 0) return BadRequest(ApiResponse<string>.Fail("El stock no puede ser negativo"));

            producto.StockActual = cantNueva;
            if (producto.Inventario != null)
            {
                producto.Inventario.Cantidad = cantNueva;
                producto.Inventario.UltimaActualizacion = DateTime.UtcNow;
            }

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            _context.MovimientosInventario.Add(new MovimientoInventario
            {
                InventarioId = producto.Inventario?.Id ?? 0,
                ProductoId = producto.Id,
                UsuarioId = userId,
                Tipo = dto.Tipo,
                Cantidad = dto.Cantidad,
                CantidadAnterior = cantAnterior,
                CantidadNueva = cantNueva,
                Motivo = dto.Motivo ?? "Ajuste manual de inventario",
                Fecha = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await _auditoria.RegistrarAsync("AjusteInventario", "Inventario", "Producto", producto.Id, new { cantAnterior }, new { cantNueva }, userId);

            return Ok(ApiResponse<string>.Ok($"Stock actualizado de {cantAnterior} a {cantNueva}"));
        }
    }
}
