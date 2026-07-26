using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa una compra de insumos a un proveedor.
    /// </summary>
    [Table("compras")]
    public class Compra
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("proveedor_id")]
        public int ProveedorId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [MaxLength(50)]
        [Column("numero_factura")]
        public string? NumeroFactura { get; set; }

        [Column("fecha_compra")]
        public DateTime FechaCompra { get; set; } = DateTime.UtcNow;

        [Column("total")]
        public decimal Total { get; set; } = 0;

        [MaxLength(50)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente"; // Pendiente, Recibida, Cancelada

        [MaxLength(300)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        // Navegación
        [ForeignKey("ProveedorId")]
        public Proveedor Proveedor { get; set; } = null!;

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        public ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();
    }
}
