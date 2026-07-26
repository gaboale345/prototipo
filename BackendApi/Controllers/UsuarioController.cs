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
    public class UsuarioController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public UsuarioController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<List<UsuarioDto>>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)
                .Select(u => new UsuarioDto
                {
                    Id = u.Id,
                    Nombre = u.Nombre,
                    Apellido = u.Apellido,
                    Email = u.Email,
                    Telefono = u.Telefono,
                    FotoUrl = u.FotoUrl,
                    Rol = u.Rol.Nombre,
                    Activo = u.Activo,
                    FechaCreacion = u.FechaCreacion
                }).ToListAsync();

            return Ok(ApiResponse<List<UsuarioDto>>.Ok(usuarios));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.Include(u => u.Rol).FirstOrDefaultAsync(u => u.Id == id);
            if (usuario == null) return NotFound(ApiResponse<UsuarioDto>.Fail("Usuario no encontrado"));

            var dto = new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                FotoUrl = usuario.FotoUrl,
                Rol = usuario.Rol.Nombre,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion
            };

            return Ok(ApiResponse<UsuarioDto>.Ok(dto));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<UsuarioDto>>> CrearUsuario([FromBody] CrearUsuarioDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                return BadRequest(ApiResponse<UsuarioDto>.Fail("El email ya está registrado"));

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Telefono = dto.Telefono,
                RolId = dto.RolId,
                EmprendimientoId = dto.EmprendimientoId ?? 1,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("CrearUsuario", "Usuarios", "Usuario", usuario.Id, null, dto, currentUserId);

            return CreatedAtAction(nameof(GetUsuario), new { id = usuario.Id }, ApiResponse<UsuarioDto>.Ok(new UsuarioDto
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                Rol = (await _context.Roles.FindAsync(usuario.RolId))?.Nombre ?? "",
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion
            }));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> ActualizarUsuario(int id, [FromBody] ActualizarUsuarioDto dto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            if (currentUserId != id && userRole != "Administrador")
                return Forbid();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(ApiResponse<string>.Fail("Usuario no encontrado"));

            if (dto.Nombre != null) usuario.Nombre = dto.Nombre;
            if (dto.Apellido != null) usuario.Apellido = dto.Apellido;
            if (dto.Telefono != null) usuario.Telefono = dto.Telefono;
            if (dto.FotoUrl != null) usuario.FotoUrl = dto.FotoUrl;

            await _context.SaveChangesAsync();
            await _auditoria.RegistrarAsync("ActualizarUsuario", "Usuarios", "Usuario", id, null, dto, currentUserId);

            return Ok(ApiResponse<string>.Ok("Perfil actualizado correctamente"));
        }

        [HttpPut("{id}/cambiar-password")]
        public async Task<ActionResult<ApiResponse<string>>> CambiarPassword(int id, [FromBody] CambiarPasswordDto dto)
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            if (currentUserId != id) return Forbid();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(ApiResponse<string>.Fail("Usuario no encontrado"));

            if (!PasswordHelper.VerifyPassword(dto.PasswordActual, usuario.PasswordHash))
                return BadRequest(ApiResponse<string>.Fail("La contraseña actual es incorrecta"));

            usuario.PasswordHash = PasswordHelper.HashPassword(dto.NuevoPassword);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Contraseña actualizada exitosamente"));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<string>>> EliminarUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound(ApiResponse<string>.Fail("Usuario no encontrado"));

            usuario.Activo = false;
            await _context.SaveChangesAsync();

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("DesactivarUsuario", "Usuarios", "Usuario", id, null, null, currentUserId);

            return Ok(ApiResponse<string>.Ok("Usuario desactivado correctamente"));
        }
    }
}
