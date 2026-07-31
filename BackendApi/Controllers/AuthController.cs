using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using BackendApi.Models;
using BackendApi.Services.Email;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly JwtHelper _jwtHelper;
        private readonly AuditoriaHelper _auditoria;
        private readonly IEmailService _emailService;

        public AuthController(EcoWashDbContext context, JwtHelper jwtHelper, AuditoriaHelper auditoria, IEmailService emailService)
        {
            _context = context;
            _jwtHelper = jwtHelper;
            _auditoria = auditoria;
            _emailService = emailService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login([FromBody] LoginRequestDto dto)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());

            if (usuario == null || !PasswordHelper.VerifyPassword(dto.Password, usuario.PasswordHash))
            {
                return BadRequest(ApiResponse<LoginResponseDto>.Fail("Credenciales inválidas"));
            }

            if (!usuario.Activo)
            {
                return BadRequest(ApiResponse<LoginResponseDto>.Fail("El usuario se encuentra inactivo"));
            }

            // NUEVA VALIDACIÓN: Verificar que el email esté verificado antes de permitir login
            if (!usuario.EmailVerificado)
            {
                return BadRequest(ApiResponse<LoginResponseDto>.Fail("EMAIL_NO_VERIFICADO"));
            }

            usuario.UltimoAcceso = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            var token = _jwtHelper.GenerarToken(usuario);

            await _auditoria.RegistrarAsync("Login", "Auth", "Usuario", usuario.Id, null, null, usuario.Id, HttpContext.Connection.RemoteIpAddress?.ToString());

            var response = new LoginResponseDto
            {
                Token = token,
                UsuarioId = usuario.Id,
                Nombre = $"{usuario.Nombre} {usuario.Apellido}",
                Email = usuario.Email,
                Rol = usuario.Rol.Nombre,
                Expiracion = DateTime.UtcNow.AddDays(7)
            };

            return Ok(ApiResponse<LoginResponseDto>.Ok(response, "Inicio de sesión exitoso"));
        }

        [HttpPost("registro")]
        public async Task<ActionResult<ApiResponse<string>>> Registro([FromBody] RegisterRequestDto dto)
        {
            if (await _context.Usuarios.AnyAsync(u => u.Email.ToLower() == dto.Email.ToLower()))
            {
                return BadRequest(ApiResponse<string>.Fail("El correo electrónico ya está registrado"));
            }

            var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Cliente");
            if (rolCliente == null)
            {
                return StatusCode(500, ApiResponse<string>.Fail("Rol Cliente no configurado en el sistema"));
            }

            var usuario = new Usuario
            {
                Nombre = dto.Nombre,
                Apellido = dto.Apellido,
                Email = dto.Email,
                PasswordHash = PasswordHelper.HashPassword(dto.Password),
                Telefono = dto.Telefono,
                RolId = rolCliente.Id,
                EmprendimientoId = 1,
                Activo = true,
                EmailVerificado = false, // CAMBIO: email NO verificado inicialmente
                FechaCreacion = DateTime.UtcNow
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            var cliente = new Cliente
            {
                UsuarioId = usuario.Id,
                FechaRegistro = DateTime.UtcNow,
                Activo = true
            };
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            // NUEVO: Generar código OTP y enviarlo por email
            var otpCode = EmailVerificationController.GenerateOtpCode();
            var verification = new EmailVerification
            {
                UsuarioId = usuario.Id,
                Codigo = otpCode,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(10),
                Usado = false,
                Intentos = 0
            };
            _context.EmailVerifications.Add(verification);
            await _context.SaveChangesAsync();

            // Enviar código OTP por email
            await _emailService.SendVerificationCodeAsync(usuario.Email, usuario.Nombre, otpCode);

            await _auditoria.RegistrarAsync("RegistroCliente", "Auth", "Cliente", cliente.Id, null, new { dto.Nombre, dto.Email }, usuario.Id, HttpContext.Connection.RemoteIpAddress?.ToString());

            return Ok(ApiResponse<string>.Ok("Registro completado. Se ha enviado un código de verificación a tu correo electrónico.", "Usuario creado con éxito"));
        }

        [HttpPost("recuperar-password")]
        public async Task<ActionResult<ApiResponse<string>>> RecuperarPassword([FromBody] RecuperarPasswordDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (usuario == null)
            {
                return Ok(ApiResponse<string>.Ok("Si el email existe, se ha enviado un enlace de recuperación"));
            }

            var token = PasswordHelper.GenerarTokenRecuperacion();
            usuario.TokenRecuperacion = token;
            usuario.TokenExpiracion = DateTime.UtcNow.AddHours(24);

            await _context.SaveChangesAsync();

            return Ok(ApiResponse<string>.Ok("Token de recuperación generado correctamente. Token sim: " + token));
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ApiResponse<string>>> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.TokenRecuperacion == dto.Token && u.TokenExpiracion > DateTime.UtcNow);
            if (usuario == null)
            {
                return BadRequest(ApiResponse<string>.Fail("Token inválido o expirado"));
            }

            usuario.PasswordHash = PasswordHelper.HashPassword(dto.NuevoPassword);
            usuario.TokenRecuperacion = null;
            usuario.TokenExpiracion = null;

            await _context.SaveChangesAsync();

            // Auditoría de cambio de contraseña
            await _auditoria.RegistrarAsync("CambioPassword", "Auth", "Usuario", usuario.Id, null, null, usuario.Id, HttpContext.Connection.RemoteIpAddress?.ToString());

            return Ok(ApiResponse<string>.Ok("Contraseña actualizada con éxito"));
        }

        /// <summary>
        /// Registra el cierre de sesión en el sistema de auditoría.
        /// </summary>
        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> Logout()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _auditoria.RegistrarAsync("Logout", "Auth", "Usuario", userId, null, null, userId, HttpContext.Connection.RemoteIpAddress?.ToString());
            return Ok(ApiResponse<string>.Ok("Sesión cerrada correctamente"));
        }
    }
}
