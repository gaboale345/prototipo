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
    public class ServicioController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public ServicioController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<ServicioDto>>>> GetServicios()
        {
            var servicios = await _context.Servicios
                .Where(s => s.Activo)
                .Select(s => new ServicioDto
                {
                    Id = s.Id,
                    Nombre = s.Nombre,
                    Descripcion = s.Descripcion,
                    Precio = s.Precio,
                    DuracionMinutos = s.DuracionMinutos,
                    TipoVehiculo = s.TipoVehiculo,
                    Activo = s.Activo
                }).ToListAsync();

            return Ok(ApiResponse<List<ServicioDto>>.Ok(servicios));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ServicioDto>>> GetServicio(int id)
        {
            var s = await _context.Servicios.FindAsync(id);
            if (s == null || !s.Activo) return NotFound(ApiResponse<ServicioDto>.Fail("Servicio no encontrado"));

            return Ok(ApiResponse<ServicioDto>.Ok(new ServicioDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                Descripcion = s.Descripcion,
                Precio = s.Precio,
                DuracionMinutos = s.DuracionMinutos,
                TipoVehiculo = s.TipoVehiculo,
                Activo = s.Activo
            }));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<ServicioDto>>> CrearServicio([FromBody] CrearServicioDto dto)
        {
            var servicio = new Servicio
            {
                Nombre = dto.Nombre,
                Descripcion = dto.Descripcion,
                Precio = dto.Precio,
                DuracionMinutos = dto.DuracionMinutos,
                TipoVehiculo = dto.TipoVehiculo ?? "Todos",
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Servicios.Add(servicio);
            await _context.SaveChangesAsync();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("CrearServicio", "Servicios", "Servicio", servicio.Id, null, dto, userId);

            return Ok(ApiResponse<ServicioDto>.Ok(new ServicioDto
            {
                Id = servicio.Id,
                Nombre = servicio.Nombre,
                Descripcion = servicio.Descripcion,
                Precio = servicio.Precio,
                DuracionMinutos = servicio.DuracionMinutos,
                TipoVehiculo = servicio.TipoVehiculo,
                Activo = servicio.Activo
            }, "Servicio creado exitosamente"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<string>>> ActualizarServicio(int id, [FromBody] CrearServicioDto dto)
        {
            var s = await _context.Servicios.FindAsync(id);
            if (s == null) return NotFound(ApiResponse<string>.Fail("Servicio no encontrado"));

            s.Nombre = dto.Nombre;
            s.Descripcion = dto.Descripcion;
            s.Precio = dto.Precio;
            s.DuracionMinutos = dto.DuracionMinutos;
            s.TipoVehiculo = dto.TipoVehiculo ?? s.TipoVehiculo;

            await _context.SaveChangesAsync();
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("ActualizarServicio", "Servicios", "Servicio", id, null, dto, userId);

            return Ok(ApiResponse<string>.Ok("Servicio actualizado correctamente"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<string>>> DesactivarServicio(int id)
        {
            var s = await _context.Servicios.FindAsync(id);
            if (s == null) return NotFound(ApiResponse<string>.Fail("Servicio no encontrado"));

            s.Activo = false;
            await _context.SaveChangesAsync();

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("DesactivarServicio", "Servicios", "Servicio", id, null, null, userId);

            return Ok(ApiResponse<string>.Ok("Servicio desactivado correctamente"));
        }
    }
}
