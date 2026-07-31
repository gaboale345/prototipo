namespace BackendApi.Services.Payments
{
    /// <summary>
    /// Interfaz abstracta para la pasarela de pagos.
    /// Diseño basado en el patrón Strategy para permitir cambiar de proveedor
    /// (Stripe, PagosNet, Circle, etc.) sin modificar la lógica principal.
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Crea una sesión de pago en la pasarela.
        /// El monto se calcula exclusivamente desde el backend.
        /// </summary>
        /// <param name="orderId">ID de la orden de servicio.</param>
        /// <param name="amount">Monto total en la moneda local (ej: 150.00 BOB).</param>
        /// <param name="currency">Código de moneda (ej: "BOB").</param>
        /// <param name="description">Descripción del pago.</param>
        /// <param name="customerEmail">Correo del cliente.</param>
        /// <param name="lineItems">Detalle de servicios para la sesión de pago.</param>
        /// <param name="successUrl">URL de redirección tras pago exitoso.</param>
        /// <param name="cancelUrl">URL de redirección si el pago es cancelado.</param>
        /// <returns>Resultado con URL de pago y ID de sesión.</returns>
        Task<PaymentSessionResult> CreatePaymentSessionAsync(
            int orderId,
            decimal amount,
            string currency,
            string description,
            string customerEmail,
            List<PaymentLineItem> lineItems,
            string successUrl,
            string cancelUrl);

        /// <summary>
        /// Verifica la firma del webhook recibido desde la pasarela.
        /// Protege contra webhooks falsificados.
        /// </summary>
        /// <param name="payload">Cuerpo crudo del request.</param>
        /// <param name="signature">Firma enviada en el header.</param>
        /// <returns>Evento del webhook parseado, o null si la firma es inválida.</returns>
        Task<WebhookEventResult?> VerifyWebhookAsync(string payload, string signature);

        /// <summary>
        /// Consulta el estado actual de un pago en la pasarela.
        /// </summary>
        Task<PaymentStatusResult> GetPaymentStatusAsync(string sessionId);

        /// <summary>Nombre del proveedor de pagos (ej: "Stripe", "PagosNet").</summary>
        string ProviderName { get; }
    }

    /// <summary>Resultado de la creación de una sesión de pago.</summary>
    public class PaymentSessionResult
    {
        public bool Success { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentUrl { get; set; }
        public string? PaymentIntentId { get; set; }
        public string? ErrorMessage { get; set; }
    }

    /// <summary>Item de línea para la sesión de pago.</summary>
    public class PaymentLineItem
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; } = 1;
    }

    /// <summary>Resultado del procesamiento de un webhook.</summary>
    public class WebhookEventResult
    {
        public string EventType { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string? PaymentIntentId { get; set; }
        public string? PaymentMethodType { get; set; }
        public string Status { get; set; } = string.Empty;
        public string RawJson { get; set; } = string.Empty;
    }

    /// <summary>Resultado de consulta de estado de un pago.</summary>
    public class PaymentStatusResult
    {
        public bool Success { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethodType { get; set; }
        public decimal? AmountPaid { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
