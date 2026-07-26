using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using BackendApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class CategoriaController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public CategoriaController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<List<Categoria>>>> GetCategorias()
        {
            var list = await _context.Categorias.Where(c => c.Activo).ToListAsync();
            return Ok(ApiResponse<List<Categoria>>.Ok(list));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<Categoria>>> CrearCategoria([FromBody] Categoria cat)
        {
            _context.Categorias.Add(cat);
            await _context.SaveChangesAsync();
            return Ok(ApiResponse<Categoria>.Ok(cat, "Categoría creada"));
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class ProveedorController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public ProveedorController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ProveedorDto>>>> GetProveedores()
        {
            var list = await _context.Proveedores.Where(p => p.Activo)
                .Select(p => new ProveedorDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    Nit = p.Nit,
                    Contacto = p.Contacto,
                    Telefono = p.Telefono,
                    Email = p.Email,
                    Direccion = p.Direccion,
                    Activo = p.Activo
                }).ToListAsync();
            return Ok(ApiResponse<List<ProveedorDto>>.Ok(list));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ProveedorDto>>> CrearProveedor([FromBody] CrearProveedorDto dto)
        {
            var p = new Proveedor
            {
                Nombre = dto.Nombre,
                Nit = dto.Nit,
                Contacto = dto.Contacto,
                Telefono = dto.Telefono,
                Email = dto.Email,
                Direccion = dto.Direccion,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };
            _context.Proveedores.Add(p);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<ProveedorDto>.Ok(new ProveedorDto
            {
                Id = p.Id,
                Nombre = p.Nombre,
                Nit = p.Nit,
                Contacto = p.Contacto,
                Telefono = p.Telefono,
                Email = p.Email,
                Direccion = p.Direccion,
                Activo = p.Activo
            }, "Proveedor creado"));
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class CompraController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public CompraController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CompraDto>>>> GetCompras()
        {
            var list = await _context.Compras
                .Include(c => c.Proveedor)
                .Include(c => c.DetalleCompras).ThenInclude(d => d.Producto)
                .OrderByDescending(c => c.FechaCompra)
                .Select(c => new CompraDto
                {
                    Id = c.Id,
                    ProveedorId = c.ProveedorId,
                    NombreProveedor = c.Proveedor.Nombre,
                    NumeroFactura = c.NumeroFactura,
                    FechaCompra = c.FechaCompra,
                    Total = c.Total,
                    Estado = c.Estado,
                    Detalles = c.DetalleCompras.Select(d => new DetalleCompraDto
                    {
                        ProductoId = d.ProductoId,
                        NombreProducto = d.Producto.Nombre,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                }).ToListAsync();

            return Ok(ApiResponse<List<CompraDto>>.Ok(list));
        }
    }
}
