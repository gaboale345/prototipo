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
    public class RolController : ControllerBase
    {
        private readonly EcoWashDbContext _context;

        public RolController(EcoWashDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<Rol>>>> GetRoles()
        {
            var roles = await _context.Roles.Include(r => r.RolPermisos).ThenInclude(rp => rp.Permiso).ToListAsync();
            return Ok(ApiResponse<List<Rol>>.Ok(roles));
        }

        [HttpGet("permisos")]
        public async Task<ActionResult<ApiResponse<List<Permiso>>>> GetPermisos()
        {
            var permisos = await _context.Permisos.ToListAsync();
            return Ok(ApiResponse<List<Permiso>>.Ok(permisos));
        }
    }
}
