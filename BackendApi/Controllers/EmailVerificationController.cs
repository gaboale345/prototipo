using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using BackendApi.Models;
using BackendApi.Services.Email;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BackendApi.Controllers
{
    /// <summary>
    /// Controlador para verificación de correo electrónico mediante código OTP.
    /// El código de 6 dígitos expira después de 10 minutos.
    /// Rate limited: 3 intentos de verificación y 2 reenvíos por minuto por IP.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EmailVerificationController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly IEmailService _emailService;
        private readonly AuditoriaHelper _auditoria;
        private readonly ILogger<EmailVerificationController> _logger;

        public EmailVerificationController(
            EcoWashDbContext context,
            IEmailService emailService,
            AuditoriaHelper auditoria,
            ILogger<EmailVerificationController> logger)
        {
            _context = context;
            _emailService = emailService;
            _auditoria = auditoria;
            _logger = logger;
        }

        /// <summary>
        /// Verifica el código OTP enviado al email del usuario.
        /// El usuario no podrá iniciar sesión hasta verificar su email.
        /// </summary>
        [HttpPost("verify")]
        public async Task<ActionResult<ApiResponse<string>>> VerifyEmail([FromBody] VerifyEmailDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (usuario == null)
                return BadRequest(ApiResponse<string>.Fail("Usuario no encontrado"));

            if (usuario.EmailVerificado)
                return Ok(ApiResponse<string>.Ok("El correo ya fue verificado anteriormente"));

            // Buscar el código OTP más reciente no usado y no expirado
            var verification = await _context.EmailVerifications
                .Where(v => v.UsuarioId == usuario.Id && !v.Usado && v.FechaExpiracion > DateTime.UtcNow)
                .OrderByDescending(v => v.FechaCreacion)
                .FirstOrDefaultAsync();

            if (verification == null)
                return BadRequest(ApiResponse<string>.Fail("No hay un código de verificación activo. Solicite un nuevo código."));

            // Verificar intentos (máximo 5 para evitar fuerza bruta)
            if (verification.Intentos >= 5)
                return BadRequest(ApiResponse<string>.Fail("Demasiados intentos fallidos. Solicite un nuevo código."));

            // Verificar código
            if (verification.Codigo != dto.Codigo)
            {
                verification.Intentos++;
                await _context.SaveChangesAsync();
                var remaining = 5 - verification.Intentos;
                return BadRequest(ApiResponse<string>.Fail($"Código incorrecto. Le quedan {remaining} intentos."));
            }

            // Código correcto: marcar email como verificado
            verification.Usado = true;
            usuario.EmailVerificado = true;
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("VerificarEmail", "Auth", "Usuario", usuario.Id, null, new { dto.Email }, usuario.Id, HttpContext.Connection.RemoteIpAddress?.ToString());

            _logger.LogInformation("Email verificado exitosamente: {Email}", dto.Email);
            return Ok(ApiResponse<string>.Ok("Correo verificado exitosamente. Ya puede iniciar sesión."));
        }

        /// <summary>
        /// Reenvía un nuevo código OTP al email del usuario.
        /// Invalida códigos anteriores.
        /// </summary>
        [HttpPost("resend")]
        public async Task<ActionResult<ApiResponse<string>>> ResendCode([FromBody] ResendCodeDto dto)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.ToLower());
            if (usuario == null)
                return Ok(ApiResponse<string>.Ok("Si el email existe, se ha enviado un nuevo código."));

            if (usuario.EmailVerificado)
                return Ok(ApiResponse<string>.Ok("El correo ya fue verificado."));

            // Invalidar códigos anteriores
            var oldCodes = await _context.EmailVerifications
                .Where(v => v.UsuarioId == usuario.Id && !v.Usado)
                .ToListAsync();
            foreach (var c in oldCodes) c.Usado = true;

            // Generar nuevo código OTP de 6 dígitos
            var otpCode = GenerateOtpCode();
            var verification = new EmailVerification
            {
                UsuarioId = usuario.Id,
                Codigo = otpCode,
                FechaCreacion = DateTime.UtcNow,
                FechaExpiracion = DateTime.UtcNow.AddMinutes(10), // Expira en 10 minutos
                Usado = false,
                Intentos = 0
            };
            _context.EmailVerifications.Add(verification);
            await _context.SaveChangesAsync();

            // Enviar email con el código
            await _emailService.SendVerificationCodeAsync(usuario.Email, usuario.Nombre, otpCode);

            _logger.LogInformation("Código OTP reenviado a: {Email}", dto.Email);
            return Ok(ApiResponse<string>.Ok("Se ha enviado un nuevo código de verificación a tu correo."));
        }

        /// <summary>
        /// Genera un código OTP aleatorio de 6 dígitos.
        /// </summary>
        public static string GenerateOtpCode()
        {
            return Random.Shared.Next(100000, 999999).ToString();
        }
    }
}
