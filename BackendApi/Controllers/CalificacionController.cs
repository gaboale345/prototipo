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
    public class CalificacionController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public CalificacionController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<CalificacionDto>>>> GetCalificaciones()
        {
            var list = await _context.Calificaciones
                .Include(c => c.Cliente).ThenInclude(cl => cl.Usuario)
                .OrderByDescending(c => c.Fecha)
                .Select(c => new CalificacionDto
                {
                    Id = c.Id,
                    ReservaId = c.ReservaId,
                    ClienteId = c.ClienteId,
                    NombreCliente = $"{c.Cliente.Usuario.Nombre} {c.Cliente.Usuario.Apellido}",
                    Puntuacion = c.Puntuacion,
                    Comentario = c.Comentario,
                    Fecha = c.Fecha
                }).ToListAsync();

            return Ok(ApiResponse<List<CalificacionDto>>.Ok(list));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<CalificacionDto>>> CalificarServicio([FromBody] CrearCalificacionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
            if (cliente == null) return BadRequest(ApiResponse<CalificacionDto>.Fail("Solo clientes pueden calificar"));

            var reserva = await _context.Reservas.FirstOrDefaultAsync(r => r.Id == dto.ReservaId && r.ClienteId == cliente.Id);
            if (reserva == null) return NotFound(ApiResponse<CalificacionDto>.Fail("Reserva no encontrada"));

            if (reserva.Estado != "Finalizada")
                return BadRequest(ApiResponse<CalificacionDto>.Fail("Solo puedes calificar servicios finalizados"));

            if (await _context.Calificaciones.AnyAsync(c => c.ReservaId == dto.ReservaId))
                return BadRequest(ApiResponse<CalificacionDto>.Fail("Esta reserva ya fue calificada"));

            var cal = new Calificacion
            {
                ReservaId = dto.ReservaId,
                ClienteId = cliente.Id,
                Puntuacion = dto.Puntuacion,
                Comentario = dto.Comentario,
                Fecha = DateTime.UtcNow
            };

            _context.Calificaciones.Add(cal);
            await _context.SaveChangesAsync();
            await _auditoria.RegistrarAsync("CalificarServicio", "Calificaciones", "Calificacion", cal.Id, null, dto, userId);

            var resDto = new CalificacionDto
            {
                Id = cal.Id,
                ReservaId = cal.ReservaId,
                ClienteId = cal.ClienteId,
                NombreCliente = $"{User.FindFirstValue(ClaimTypes.Name)}",
                Puntuacion = cal.Puntuacion,
                Comentario = cal.Comentario,
                Fecha = cal.Fecha
            };

            return Ok(ApiResponse<CalificacionDto>.Ok(resDto, "¡Gracias por calificar el servicio!"));
        }
    }
}
