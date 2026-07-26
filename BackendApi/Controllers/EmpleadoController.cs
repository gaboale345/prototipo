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
    public class EmpleadoController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public EmpleadoController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<EmpleadoDto>>>> GetEmpleados()
        {
            var empleados = await _context.Empleados
                .Include(e => e.Usuario)
                .Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    UsuarioId = e.UsuarioId,
                    NombreCompleto = $"{e.Usuario.Nombre} {e.Usuario.Apellido}",
                    Email = e.Usuario.Email,
                    Telefono = e.Usuario.Telefono,
                    Ci = e.Ci,
                    Cargo = e.Cargo,
                    Salario = e.Salario,
                    Disponible = e.Disponible,
                    Activo = e.Activo,
                    FechaIngreso = e.FechaIngreso
                }).ToListAsync();

            return Ok(ApiResponse<List<EmpleadoDto>>.Ok(empleados));
        }

        [HttpGet("disponibles")]
        public async Task<ActionResult<ApiResponse<List<EmpleadoDto>>>> GetEmpleadosDisponibles()
        {
            var empleados = await _context.Empleados
                .Include(e => e.Usuario)
                .Where(e => e.Activo && e.Disponible)
                .Select(e => new EmpleadoDto
                {
                    Id = e.Id,
                    UsuarioId = e.UsuarioId,
                    NombreCompleto = $"{e.Usuario.Nombre} {e.Usuario.Apellido}",
                    Email = e.Usuario.Email,
                    Telefono = e.Usuario.Telefono,
                    Ci = e.Ci,
                    Cargo = e.Cargo,
                    Salario = e.Salario,
                    Disponible = e.Disponible,
                    Activo = e.Activo,
                    FechaIngreso = e.FechaIngreso
                }).ToListAsync();

            return Ok(ApiResponse<List<EmpleadoDto>>.Ok(empleados));
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<EmpleadoDto>>> CrearEmpleado([FromBody] CrearEmpleadoDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
                return BadRequest(ApiResponse<EmpleadoDto>.Fail("El email ya está registrado"));

            var rolEmpleado = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Empleado");
            if (rolEmpleado == null) return BadRequest(ApiResponse<EmpleadoDto>.Fail("Rol Empleado no encontrado"));

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Telefono = dto.Telefono,
                RolId = rolEmpleado.Id,
                EmprendimientoId = 1,
                Activo = true,
                FechaCreacion = DateTime.UtcNow
            };
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var empleado = new Empleado
            {
                UsuarioId = usuario.Id,
                Ci = dto.Ci,
                Cargo = dto.Cargo ?? "Lavador",
                Salario = dto.Salario,
                Disponible = true,
                Activo = true,
                FechaIngreso = DateTime.UtcNow
            };
            _context.Empleados.Add(empleado);
            await _context.SaveChangesAsync();

            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("CrearEmpleado", "Empleados", "Empleado", empleado.Id, null, dto, currentUserId);

            return Ok(ApiResponse<EmpleadoDto>.Ok(new EmpleadoDto
            {
                Id = empleado.Id,
                UsuarioId = usuario.Id,
                NombreCompleto = $"{usuario.Nombre} {usuario.Apellido}",
                Email = usuario.Email,
                Telefono = usuario.Telefono,
                Ci = empleado.Ci,
                Cargo = empleado.Cargo,
                Salario = empleado.Salario,
                Disponible = empleado.Disponible,
                Activo = empleado.Activo,
                FechaIngreso = empleado.FechaIngreso
            }, "Empleado creado exitosamente"));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Administrador")]
        public async Task<ActionResult<ApiResponse<string>>> ActualizarEmpleado(int id, [FromBody] ActualizarEmpleadoDto dto)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound(ApiResponse<string>.Fail("Empleado no encontrado"));

            if (dto.Cargo != null) empleado.Cargo = dto.Cargo;
            if (dto.Salario.HasValue) empleado.Salario = dto.Salario.Value;
            if (dto.Disponible.HasValue) empleado.Disponible = dto.Disponible.Value;
            if (dto.Activo.HasValue) empleado.Activo = dto.Activo.Value;

            await _context.SaveChangesAsync();
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("ActualizarEmpleado", "Empleados", "Empleado", id, null, dto, currentUserId);

            return Ok(ApiResponse<string>.Ok("Empleado actualizado correctamente"));
        }

        [HttpPut("{id}/disponibilidad")]
        public async Task<ActionResult<ApiResponse<string>>> CambiarDisponibilidad(int id, [FromBody] bool disponible)
        {
            var empleado = await _context.Empleados.FindAsync(id);
            if (empleado == null) return NotFound(ApiResponse<string>.Fail("Empleado no encontrado"));

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (empleado.UsuarioId != userId && role != "Administrador") return Forbid();

            empleado.Disponible = disponible;
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok($"Estado de disponibilidad actualizado a: {(disponible ? "Disponible" : "Ocupado/No disponible")}"));
        }
    }
}
