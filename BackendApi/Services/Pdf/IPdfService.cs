namespace BackendApi.Services.Pdf
{
    /// <summary>
    /// Interfaz para generación de comprobantes PDF.
    /// </summary>
    public interface IPdfService
    {
        /// <summary>
        /// Genera un comprobante PDF de pago con todos los datos requeridos.
        /// </summary>
        /// <param name="receipt">Datos del comprobante.</param>
        /// <returns>Bytes del PDF generado.</returns>
        Task<byte[]> GeneratePaymentReceiptAsync(ReceiptData receipt);
    }

    /// <summary>Datos necesarios para generar el comprobante PDF.</summary>
    public class ReceiptData
    {
        public string NumeroOrden { get; set; } = string.Empty;
        public string NumeroTransaccion { get; set; } = string.Empty;
        public string NombreCliente { get; set; } = string.Empty;
        public string EmailCliente { get; set; } = string.Empty;
        public DateTime FechaPago { get; set; }
        public List<ReceiptItem> Servicios { get; set; } = new();
        public decimal TotalPagado { get; set; }
        public string MetodoPago { get; set; } = string.Empty;
        public string EstadoPago { get; set; } = string.Empty;
        public string? QrContent { get; set; }
    }

    /// <summary>Item de servicio para el comprobante.</summary>
    public class ReceiptItem
    {
        public string NombreServicio { get; set; } = string.Empty;
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
