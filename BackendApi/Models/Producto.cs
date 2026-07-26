using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un producto del inventario (insumos de lavado).
    /// </summary>
    [Table("productos")]
    public class Producto
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("categoria_id")]
        public int CategoriaId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(300)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [MaxLength(50)]
        [Column("unidad_medida")]
        public string? UnidadMedida { get; set; }

        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; } = 0;

        [Column("stock_actual")]
        public int StockActual { get; set; } = 0;

        [Column("stock_minimo")]
        public int StockMinimo { get; set; } = 5;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("CategoriaId")]
        public Categoria Categoria { get; set; } = null!;

        public Inventario? Inventario { get; set; }
        public ICollection<DetalleCompra> DetalleCompras { get; set; } = new List<DetalleCompra>();
        public ICollection<DetalleVenta> DetalleVentas { get; set; } = new List<DetalleVenta>();
        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    }
}
