using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Registro completo de cada transacción de pago con la pasarela.
    /// Almacena la respuesta completa de la API para auditoría.
    /// Estados posibles: Pendiente, Pagado, Fallido, Cancelado.
    /// </summary>
    [Table("payment_transactions")]
    public class PaymentTransaction
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("service_order_id")]
        public int ServiceOrderId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        /// <summary>ID de la transacción devuelto por la pasarela de pagos.</summary>
        [MaxLength(255)]
        [Column("transaction_id")]
        public string? TransactionId { get; set; }

        /// <summary>Estado del pago: Pendiente, Pagado, Fallido, Cancelado.</summary>
        [Required]
        [MaxLength(30)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente";

        [Column("monto")]
        public decimal Monto { get; set; }

        /// <summary>Moneda del pago (BOB para Bolivianos).</summary>
        [MaxLength(10)]
        [Column("moneda")]
        public string Moneda { get; set; } = "BOB";

        /// <summary>Método de pago: tarjeta_debito, tarjeta_credito, qr.</summary>
        [MaxLength(50)]
        [Column("metodo_pago")]
        public string? MetodoPago { get; set; }

        /// <summary>Referencia devuelta por la pasarela de pagos.</summary>
        [MaxLength(255)]
        [Column("referencia_pasarela")]
        public string? ReferenciaPasarela { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fecha_pago")]
        public DateTime? FechaPago { get; set; }

        /// <summary>Respuesta JSON completa de la API de la pasarela para auditoría.</summary>
        [Column("respuesta_completa_api")]
        public string? RespuestaCompletaApi { get; set; }

        /// <summary>ID del PaymentIntent de Stripe (o equivalente de otra pasarela).</summary>
        [MaxLength(255)]
        [Column("stripe_payment_intent_id")]
        public string? StripePaymentIntentId { get; set; }

        /// <summary>ID de la sesión de Stripe Checkout.</summary>
        [MaxLength(255)]
        [Column("stripe_session_id")]
        public string? StripeSessionId { get; set; }

        /// <summary>Nombre del proveedor de pagos utilizado (ej: Stripe, PagosNet).</summary>
        [MaxLength(50)]
        [Column("proveedor_pago")]
        public string ProveedorPago { get; set; } = "Stripe";

        /// <summary>Ruta al comprobante PDF generado.</summary>
        [MaxLength(500)]
        [Column("comprobante_pdf_url")]
        public string? ComprobantePdfUrl { get; set; }

        // Navegación
        [ForeignKey("ServiceOrderId")]
        public ServiceOrder ServiceOrder { get; set; } = null!;

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;
    }
}
