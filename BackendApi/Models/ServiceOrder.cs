using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Orden de servicio creada cuando un cliente solicita uno o más servicios.
    /// El total se calcula exclusivamente en el backend a partir de los precios en BD.
    /// </summary>
    [Table("service_orders")]
    public class ServiceOrder
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        /// <summary>Número de orden único generado automáticamente (ej: ORD-20260730-001).</summary>
        [Required]
        [MaxLength(50)]
        [Column("numero_orden")]
        public string NumeroOrden { get; set; } = string.Empty;

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("total")]
        public decimal Total { get; set; }

        /// <summary>Estado de la orden: Pendiente, Pagada, Cancelada.</summary>
        [Required]
        [MaxLength(30)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente";

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("fecha_pago")]
        public DateTime? FechaPago { get; set; }

        [MaxLength(300)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // Navegación
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<ServiceOrderDetail> Detalles { get; set; } = new List<ServiceOrderDetail>();
        public ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();
    }
}
