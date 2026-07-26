using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa el detalle de productos en una compra.
    /// </summary>
    [Table("detalle_compras")]
    public class DetalleCompra
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("compra_id")]
        public int CompraId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        // Navegación
        [ForeignKey("CompraId")]
        public Compra Compra { get; set; } = null!;

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; } = null!;
    }
}
