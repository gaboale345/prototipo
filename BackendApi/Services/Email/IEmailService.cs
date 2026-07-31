namespace BackendApi.Services.Email
{
    /// <summary>
    /// Interfaz para el servicio de envío de correos electrónicos.
    /// Abstrae el proveedor SMTP para permitir cambiar de Gmail a SendGrid, etc.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Envía el código OTP de verificación de email al usuario registrado.
        /// </summary>
        /// <param name="toEmail">Correo destino.</param>
        /// <param name="userName">Nombre del usuario.</param>
        /// <param name="otpCode">Código OTP de 6 dígitos.</param>
        Task<bool> SendVerificationCodeAsync(string toEmail, string userName, string otpCode);

        /// <summary>
        /// Envía la confirmación de pago exitoso al cliente.
        /// </summary>
        /// <param name="toEmail">Correo del cliente.</param>
        /// <param name="userName">Nombre del cliente.</param>
        /// <param name="orderNumber">Número de orden.</param>
        /// <param name="amount">Monto pagado.</param>
        /// <param name="paymentMethod">Método de pago utilizado.</param>
        Task<bool> SendPaymentConfirmationAsync(string toEmail, string userName, string orderNumber, decimal amount, string paymentMethod);

        /// <summary>
        /// Envía el comprobante PDF de pago adjunto al correo del cliente.
        /// </summary>
        /// <param name="toEmail">Correo del cliente.</param>
        /// <param name="userName">Nombre del cliente.</param>
        /// <param name="orderNumber">Número de orden.</param>
        /// <param name="pdfBytes">Bytes del PDF generado.</param>
        Task<bool> SendPaymentReceiptAsync(string toEmail, string userName, string orderNumber, byte[] pdfBytes);
    }
}
