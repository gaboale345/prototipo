namespace BackendApi.Services.Payments
{
    /// <summary>
    /// Pasarela de pagos ficticia / simulada para desarrollo, pruebas y demostración.
    /// No realiza transacciones reales ni contacta servicios externos.
    /// Genera identificadores de sesión y de transacción ficticios.
    /// </summary>
    public class SimulatedPaymentGateway : IPaymentGateway
    {
        private readonly ILogger<SimulatedPaymentGateway> _logger;

        public string ProviderName => "Pasarela Simulación EcoWash";

        public SimulatedPaymentGateway(ILogger<SimulatedPaymentGateway> logger)
        {
            _logger = logger;
        }

        public Task<PaymentSessionResult> CreatePaymentSessionAsync(
            int orderId,
            decimal amount,
            string currency,
            string description,
            string customerEmail,
            List<PaymentLineItem> lineItems,
            string successUrl,
            string cancelUrl)
        {
            _logger.LogInformation("Creando sesión de pago ficticia para OrdenId={OrderId}, Monto={Amount} {Currency}", orderId, amount, currency);

            var sessionId = $"SIM-SESS-{Guid.NewGuid().ToString("N")[..12].ToUpper()}";
            var transactionId = $"TX-SIM-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";

            return Task.FromResult(new PaymentSessionResult
            {
                Success = true,
                SessionId = sessionId,
                PaymentIntentId = transactionId,
                PaymentUrl = $"{successUrl}?session_id={sessionId}"
            });
        }

        public Task<WebhookEventResult?> VerifyWebhookAsync(string payload, string signature)
        {
            return Task.FromResult<WebhookEventResult?>(new WebhookEventResult
            {
                EventType = "checkout.session.completed",
                SessionId = "SIM-WEBHOOK-SESSION",
                Status = "complete",
                PaymentMethodType = "Simulado"
            });
        }

        public Task<PaymentStatusResult> GetPaymentStatusAsync(string sessionId)
        {
            return Task.FromResult(new PaymentStatusResult
            {
                Success = true,
                Status = "paid",
                PaymentMethodType = "Simulado",
                AmountPaid = 0m
            });
        }
    }
}
