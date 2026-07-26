using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa una venta generada al finalizar un servicio.
    /// Regla de negocio: solo se genera al finalizar el servicio.
    /// </summary>
    [Table("ventas")]
    public class Venta
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("reserva_id")]
        public int ReservaId { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("numero_venta")]
        public string NumeroVenta { get; set; } = string.Empty;

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("descuento")]
        public decimal Descuento { get; set; } = 0;

        [Column("total")]
        public decimal Total { get; set; }

        [Column("fecha_venta")]
        public DateTime FechaVenta { get; set; } = DateTime.UtcNow;

        [MaxLength(30)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Pagada, Cancelada

        // Navegación
        [ForeignKey("ReservaId")]
        public Reserva Reserva { get; set; } = null!;

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
        public Factura? Factura { get; set; }
    }
}
