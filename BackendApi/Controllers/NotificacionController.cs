using BackendApi.Data;
using BackendApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class NotificacionController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public NotificacionController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<NotificacionDto>>>> GetMisNotificaciones()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var list = await _context.Notificaciones
                .Where(n => n.UsuarioId == userId)
                .OrderByDescending(n => n.Fecha)
                .Select(n => new NotificacionDto
                {
                    Id = n.Id,
                    Titulo = n.Titulo,
                    Mensaje = n.Mensaje,
                    Tipo = n.Tipo,
                    Leida = n.Leida,
                    Fecha = n.Fecha
                }).ToListAsync();

            return Ok(ApiResponse<List<NotificacionDto>>.Ok(list));
        }

        [HttpPut("{id}/marcar-leida")]
        public async Task<ActionResult<ApiResponse<string>>> MarcarLeida(int id)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var n = await _context.Notificaciones.FirstOrDefaultAsync(x => x.Id == id && x.UsuarioId == userId);
            if (n == null) return NotFound(ApiResponse<string>.Fail("Notificación no encontrada"));

            n.Leida = true;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Notificación marcada como leída"));
        }
    }
}
