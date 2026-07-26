using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa una factura generada tras el pago.
    /// </summary>
    [Table("facturas")]
    public class Factura
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }

        [Column("pago_id")]
        public int PagoId { get; set; }

        [Required]
        [MaxLength(50)]
        [Column("numero_factura")]
        public string NumeroFactura { get; set; } = string.Empty;

        [Column("fecha_emision")]
        public DateTime FechaEmision { get; set; } = DateTime.UtcNow;

        [MaxLength(150)]
        [Column("razon_social")]
        public string? RazonSocial { get; set; }

        [MaxLength(20)]
        [Column("nit")]
        public string? Nit { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        [Column("descuento")]
        public decimal Descuento { get; set; } = 0;

        [Column("total")]
        public decimal Total { get; set; }

        [MaxLength(30)]
        [Column("estado")]
        public string Estado { get; set; } = "Emitida"; // Emitida, Anulada

        // Navegación
        [ForeignKey("VentaId")]
        public Venta Venta { get; set; } = null!;

        [ForeignKey("PagoId")]
        public Pago Pago { get; set; } = null!;
    }
}
