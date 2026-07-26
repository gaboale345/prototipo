using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AuditoriaController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public AuditoriaController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Auditoria>>>> GetAuditorias([FromQuery] string? modulo = null)
        {
            // REGLA DE NEGOCIO: Todas las acciones importantes deben registrarse en auditoría.
            IQueryable<Auditoria> query = _context.Auditorias.Include(a => a.Usuario);

            if (!string.IsNullOrEmpty(modulo))
                query = query.Where(a => a.Modulo.ToLower() == modulo.ToLower());

            var result = await query.OrderByDescending(a => a.Fecha).Take(200).ToListAsync();
            return Ok(ApiResponse<List<Auditoria>>.Ok(result));
        }
    }
}
