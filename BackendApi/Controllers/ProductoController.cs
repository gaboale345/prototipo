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
    public class ProductoController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public ProductoController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProductoDto>>>> GetProductos()
        {
            var result = await _context.Productos
                .Include(p => p.Categoria)
                .Where(p => p.Activo)
                .Select(p => new ProductoDto
                {
                    Id = p.Id,
                    CategoriaId = p.CategoriaId,
                    NombreCategoria = p.Categoria.Nombre,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    UnidadMedida = p.UnidadMedida,
                    PrecioUnitario = p.PrecioUnitario,
                    StockActual = p.StockActual,
                    StockMinimo = p.StockMinimo,
                    Activo = p.Activo
                }).ToListAsync();

            return Ok(ApiResponse<List<ProductoDto>>.Ok(result));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<ProductoDto>>> CrearProducto([FromBody] CrearProductoDto dto)
        {
            var p = new Producto
            {
                CategoriaId = dto.CategoriaId,
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                UnidadMedida = dto.UnidadMedida ?? "Unidad",
                PrecioUnitario = dto.PrecioUnitario,
                StockActual = dto.StockActual,
                StockMinimo = dto.StockMinimo,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Productos.Add(p);
            await _context.SaveChangesAsync();

            var inv = new Inventario
            {
                ProductoId = p.Id,
                Cantidad = p.StockActual,
                CantidadMinima = p.StockMinimo,
                UltimaActualizacion = DateTime.UtcNow
            };
            _context.Inventarios.Add(inv);
            await _context.SaveChangesAsync();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("CrearProducto", "Productos", "Producto", p.Id, null, dto, userId);

            var cat = await _context.Categorias.FindAsync(p.CategoriaId);
            var res = new ProductoDto
            {
                Id = p.Id,
                CategoriaId = p.CategoriaId,
                NombreCategoria = cat?.Nombre ?? "",
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                UnidadMedida = p.UnidadMedida,
                PrecioUnitario = p.PrecioUnitario,
                StockActual = p.StockActual,
                StockMinimo = p.StockMinimo,
                Activo = p.Activo
            };

            return Ok(ApiResponse<ProductoDto>.Ok(res, "Producto creado exitosamente"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<string>>> ActualizarProducto(int id, [FromBody] CrearProductoDto dto)
        {
            var p = await _context.Productos.Include(x => x.Inventario).FirstOrDefaultAsync(x => x.Id == id);
            if (p == null) return NotFound(ApiResponse<string>.Fail("Producto no encontrado"));

            p.CategoriaId = dto.CategoriaId;
            p.Nombre = dto.Nombre;
            p.Descripcion = dto.Descripcion;
            p.UnidadMedida = dto.UnidadMedida ?? p.UnidadMedida;
            p.PrecioUnitario = dto.PrecioUnitario;
            p.StockActual = dto.StockActual;
            p.StockMinimo = dto.StockMinimo;

            if (p.Inventario != null)
            {
                p.Inventario.Cantidad = dto.StockActual;
                p.Inventario.CantidadMinima = dto.StockMinimo;
                p.Inventario.UltimaActualizacion = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("ActualizarProducto", "Productos", "Producto", id, null, dto, userId);

            return Ok(ApiResponse<string>.Ok("Producto actualizado correctamente"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<string>>> DesactivarProducto(int id)
        {
            var p = await _context.Productos.FindAsync(id);
            if (p == null) return NotFound(ApiResponse<string>.Fail("Producto no encontrado"));

            p.Activo = false;
            await _context.SaveChangesAsync();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("DesactivarProducto", "Productos", "Producto", id, null, null, userId);

            return Ok(ApiResponse<string>.Ok("Producto desactivado correctamente"));
        }
    }
}
