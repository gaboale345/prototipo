using System.Net;
using System.Net.Mail;

namespace BackendApi.Services.Email
{
    /// <summary>
    /// Servicio de correo electrónico mediante SMTP (Gmail con App Password).
    /// Todas las credenciales se leen de variables de entorno (.env).
    /// Nunca se almacenan contraseñas SMTP en el código.
    /// 
    /// CONFIGURACIÓN DE GMAIL APP PASSWORD:
    /// 1. Ir a https://myaccount.google.com/security
    /// 2. Activar "Verificación en dos pasos" si no está habilitada
    /// 3. Ir a https://myaccount.google.com/apppasswords
    /// 4. Seleccionar "Correo" y "Computadora Windows"
    /// 5. Generar y copiar la contraseña de 16 caracteres
    /// 6. Pegarla en el archivo .env como SMTP_PASSWORD
    /// </summary>
    public class SmtpEmailService : IEmailService
    {
        private readonly string _host;
        private readonly int _port;
        private readonly string _username;
        private readonly string _password;
        private readonly string _fromName;
        private readonly string _fromEmail;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _logger = logger;
            // Credenciales SMTP desde variables de entorno (.env)
            _host = Environment.GetEnvironmentVariable("SMTP_HOST") ?? "smtp.gmail.com";
            _port = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
            _username = Environment.GetEnvironmentVariable("SMTP_USERNAME") ?? "";
            _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? "";
            _fromName = Environment.GetEnvironmentVariable("SMTP_FROM_NAME") ?? "EcoWash Direct";
            _fromEmail = Environment.GetEnvironmentVariable("SMTP_FROM_EMAIL") ?? _username;
        }

        /// <summary>
        /// Envía el código OTP de verificación por email con plantilla HTML.
        /// </summary>
        public async Task<bool> SendVerificationCodeAsync(string toEmail, string userName, string otpCode)
        {
            var subject = "🔐 Código de Verificación - EcoWash Direct";
            var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
  <div style='max-width: 500px; margin: 0 auto; background: white; border-radius: 12px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
    <div style='text-align: center; margin-bottom: 20px;'>
      <h1 style='color: #2d6a4f; margin: 0;'>🌿 EcoWash Direct</h1>
      <p style='color: #888; font-size: 14px;'>Lavado Ecológico a Domicilio</p>
    </div>
    <h2 style='color: #333; text-align: center;'>Verificación de Correo Electrónico</h2>
    <p style='color: #555;'>Hola <strong>{userName}</strong>,</p>
    <p style='color: #555;'>Tu código de verificación es:</p>
    <div style='text-align: center; margin: 25px 0;'>
      <span style='font-size: 36px; font-weight: bold; letter-spacing: 8px; color: #2d6a4f; background: #e8f5e9; padding: 15px 30px; border-radius: 8px; display: inline-block;'>
        {otpCode}
      </span>
    </div>
    <p style='color: #888; font-size: 13px; text-align: center;'>
      ⏰ Este código expira en <strong>10 minutos</strong>.<br>
      Si no solicitaste este código, ignora este correo.
    </p>
    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
    <p style='color: #aaa; font-size: 11px; text-align: center;'>
      © {DateTime.UtcNow.Year} EcoWash Direct — Santa Cruz de la Sierra, Bolivia
    </p>
  </div>
</body>
</html>";
            return await SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Envía confirmación de pago exitoso por email.
        /// </summary>
        public async Task<bool> SendPaymentConfirmationAsync(string toEmail, string userName, string orderNumber, decimal amount, string paymentMethod)
        {
            var subject = "✅ Pago Confirmado - EcoWash Direct";
            var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
  <div style='max-width: 500px; margin: 0 auto; background: white; border-radius: 12px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
    <div style='text-align: center; margin-bottom: 20px;'>
      <h1 style='color: #2d6a4f; margin: 0;'>🌿 EcoWash Direct</h1>
    </div>
    <div style='text-align: center; margin: 20px 0;'>
      <div style='font-size: 60px;'>✅</div>
      <h2 style='color: #2d6a4f;'>¡Pago Exitoso!</h2>
    </div>
    <p style='color: #555;'>Hola <strong>{userName}</strong>,</p>
    <p style='color: #555;'>Tu pago ha sido procesado correctamente.</p>
    <table style='width: 100%; border-collapse: collapse; margin: 20px 0;'>
      <tr style='background: #f8f9fa;'>
        <td style='padding: 10px; border: 1px solid #dee2e6; font-weight: bold;'>Orden</td>
        <td style='padding: 10px; border: 1px solid #dee2e6;'>{orderNumber}</td>
      </tr>
      <tr>
        <td style='padding: 10px; border: 1px solid #dee2e6; font-weight: bold;'>Monto</td>
        <td style='padding: 10px; border: 1px solid #dee2e6;'>Bs. {amount:N2}</td>
      </tr>
      <tr style='background: #f8f9fa;'>
        <td style='padding: 10px; border: 1px solid #dee2e6; font-weight: bold;'>Método</td>
        <td style='padding: 10px; border: 1px solid #dee2e6;'>{paymentMethod}</td>
      </tr>
      <tr>
        <td style='padding: 10px; border: 1px solid #dee2e6; font-weight: bold;'>Fecha</td>
        <td style='padding: 10px; border: 1px solid #dee2e6;'>{DateTime.UtcNow:dd/MM/yyyy HH:mm} UTC</td>
      </tr>
    </table>
    <p style='color: #888; font-size: 13px;'>El comprobante PDF se adjuntará en un correo separado.</p>
    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
    <p style='color: #aaa; font-size: 11px; text-align: center;'>
      © {DateTime.UtcNow.Year} EcoWash Direct — Santa Cruz de la Sierra, Bolivia
    </p>
  </div>
</body>
</html>";
            return await SendEmailAsync(toEmail, subject, body);
        }

        /// <summary>
        /// Envía el comprobante PDF adjunto por email.
        /// </summary>
        public async Task<bool> SendPaymentReceiptAsync(string toEmail, string userName, string orderNumber, byte[] pdfBytes)
        {
            var subject = $"📄 Comprobante de Pago {orderNumber} - EcoWash Direct";
            var body = $@"
<!DOCTYPE html>
<html>
<head><meta charset='utf-8'></head>
<body style='font-family: Arial, sans-serif; background-color: #f4f4f4; padding: 20px;'>
  <div style='max-width: 500px; margin: 0 auto; background: white; border-radius: 12px; padding: 30px; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
    <div style='text-align: center; margin-bottom: 20px;'>
      <h1 style='color: #2d6a4f; margin: 0;'>🌿 EcoWash Direct</h1>
    </div>
    <p style='color: #555;'>Hola <strong>{userName}</strong>,</p>
    <p style='color: #555;'>Adjuntamos el comprobante de pago de tu orden <strong>{orderNumber}</strong>.</p>
    <p style='color: #555;'>Puedes descargarlo también desde la sección <strong>""Mis Pagos""</strong> en tu perfil.</p>
    <hr style='border: none; border-top: 1px solid #eee; margin: 20px 0;'>
    <p style='color: #aaa; font-size: 11px; text-align: center;'>
      © {DateTime.UtcNow.Year} EcoWash Direct — Santa Cruz de la Sierra, Bolivia
    </p>
  </div>
</body>
</html>";

            return await SendEmailAsync(toEmail, subject, body, pdfBytes, $"Comprobante_{orderNumber}.pdf");
        }

        /// <summary>
        /// Método interno que envía emails via SMTP.
        /// Soporta adjuntos opcionales (para comprobantes PDF).
        /// </summary>
        private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, byte[]? attachment = null, string? attachmentName = null)
        {
            // Validar que las credenciales SMTP estén configuradas
            if (string.IsNullOrEmpty(_username) || string.IsNullOrEmpty(_password))
            {
                _logger.LogWarning(
                    "SMTP no configurado. Email no enviado a {ToEmail}. " +
                    "Configure SMTP_USERNAME y SMTP_PASSWORD en el archivo .env", toEmail);
                return false;
            }

            try
            {
                using var smtp = new SmtpClient(_host, _port)
                {
                    Credentials = new NetworkCredential(_username, _password),
                    EnableSsl = true
                };

                var message = new MailMessage
                {
                    From = new MailAddress(_fromEmail, _fromName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };
                message.To.Add(new MailAddress(toEmail));

                // Adjuntar PDF si se proporciona
                if (attachment != null && attachmentName != null)
                {
                    var stream = new MemoryStream(attachment);
                    message.Attachments.Add(new Attachment(stream, attachmentName, "application/pdf"));
                }

                await smtp.SendMailAsync(message);
                _logger.LogInformation("Email enviado exitosamente a {ToEmail}: {Subject}", toEmail, subject);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar email a {ToEmail}: {Error}", toEmail, ex.Message);
                return false;
            }
        }
    }
}
