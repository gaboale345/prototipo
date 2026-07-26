using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Detalle de productos/insumos utilizados en una venta de servicio.
    /// </summary>
    [Table("detalle_ventas")]
    public class DetalleVenta
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }

        [Column("producto_id")]
        public int? ProductoId { get; set; }

        [MaxLength(150)]
        [Column("descripcion")]
        public string Descripcion { get; set; } = string.Empty;

        [Column("cantidad")]
        public decimal Cantidad { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        // Navegación
        [ForeignKey("VentaId")]
        public Venta Venta { get; set; } = null!;

        [ForeignKey("ProductoId")]
        public Producto? Producto { get; set; }
    }
}
