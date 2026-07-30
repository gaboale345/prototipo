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
    public class UbicacionController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public UbicacionController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UbicacionDto>>>> GetUbicaciones()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
            if (cliente == null) return Ok(ApiResponse<List<UbicacionDto>>.Ok(new List<UbicacionDto>()));

            var result = await _context.Ubicaciones
                .Where(u => u.ClienteId == cliente.Id && u.Activo)
                .Select(u => new UbicacionDto
                {
                    Id = u.Id,
                    ClienteId = u.ClienteId,
                    Direccion = u.Direccion,
                    Zona = u.Zona,
                    Referencia = u.Referencia,
                    Latitud = u.Latitud,
                    Longitud = u.Longitud,
                    EsPrincipal = u.EsPrincipal
                }).ToListAsync();

            return Ok(ApiResponse<List<UbicacionDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<UbicacionDto>>> RegistrarUbicacion([FromBody] CrearUbicacionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
            if (cliente == null) return BadRequest(ApiResponse<UbicacionDto>.Fail("Cliente no registrado"));

            if (dto.EsPrincipal)
            {
                var previas = await _context.Ubicaciones.Where(u => u.ClienteId == cliente.Id).ToListAsync();
                previas.ForEach(u => u.EsPrincipal = false);
            }

            var ubicacion = new Ubicacion
            {
                ClienteId = cliente.Id,
                Direccion = dto.Direccion,
                Zona = dto.Zona,
                Referencia = dto.Referencia,
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
                EsPrincipal = dto.EsPrincipal,
                Activo = true
            };

            _context.Ubicaciones.Add(ubicacion);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("RegistrarUbicacion", "Ubicaciones", "Ubicacion", ubicacion.Id, null, dto, userId);

            var res = new UbicacionDto
            {
                Id = ubicacion.Id,
                ClienteId = ubicacion.ClienteId,
                Direccion = ubicacion.Direccion,
                Zona = ubicacion.Zona,
                Referencia = ubicacion.Referencia,
                Latitud = ubicacion.Latitud,
                Longitud = ubicacion.Longitud,
                EsPrincipal = ubicacion.EsPrincipal
            };

            return Ok(ApiResponse<UbicacionDto>.Ok(res, "Ubicación registrada exitosamente"));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<UbicacionDto>>> EditarUbicacion(int id, [FromBody] CrearUbicacionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var ubicacion = await _context.Ubicaciones.FindAsync(id);
            if (ubicacion == null || !ubicacion.Activo) return NotFound(ApiResponse<UbicacionDto>.Fail("Ubicación no encontrada"));

            if (role != "Administrador" && (cliente == null || ubicacion.ClienteId != cliente.Id))
                return Forbid();

            if (dto.EsPrincipal)
            {
                var previas = await _context.Ubicaciones.Where(u => u.ClienteId == ubicacion.ClienteId && u.Id != id).ToListAsync();
                previas.ForEach(u => u.EsPrincipal = false);
            }

            var anterior = new { ubicacion.Direccion, ubicacion.Zona, ubicacion.Referencia, ubicacion.EsPrincipal };
            ubicacion.Direccion = dto.Direccion;
            ubicacion.Zona = dto.Zona;
            ubicacion.Referencia = dto.Referencia;
            ubicacion.Latitud = dto.Latitud;
            ubicacion.Longitud = dto.Longitud;
            ubicacion.EsPrincipal = dto.EsPrincipal;

            await _context.SaveChangesAsync();
            await _auditoria.RegistrarAsync("EditarUbicacion", "Ubicaciones", "Ubicacion", id, anterior, dto, userId);

            var res = new UbicacionDto
            {
                Id = ubicacion.Id,
                ClienteId = ubicacion.ClienteId,
                Direccion = ubicacion.Direccion,
                Zona = ubicacion.Zona,
                Referencia = ubicacion.Referencia,
                Latitud = ubicacion.Latitud,
                Longitud = ubicacion.Longitud,
                EsPrincipal = ubicacion.EsPrincipal
            };

            return Ok(ApiResponse<UbicacionDto>.Ok(res, "Ubicación actualizada exitosamente"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> EliminarUbicacion(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
            var role = User.FindFirstValue(ClaimTypes.Role);

            var ubicacion = await _context.Ubicaciones.FindAsync(id);
            if (ubicacion == null) return NotFound(ApiResponse<string>.Fail("Ubicación no encontrada"));

            if (role != "Administrador" && (cliente == null || ubicacion.ClienteId != cliente.Id))
                return Forbid();

            ubicacion.Activo = false;
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("EliminarUbicacion", "Ubicaciones", "Ubicacion", id, null, null, userId);

            return Ok(ApiResponse<string>.Ok("Ubicación eliminada correctamente"));
        }
    }
}
