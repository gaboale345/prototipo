using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa el estado actual del inventario por producto.
    /// </summary>
    [Table("inventarios")]
    public class Inventario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("cantidad")]
        public int Cantidad { get; set; } = 0;

        [Column("cantidad_minima")]
        public int CantidadMinima { get; set; } = 5;

        [Column("ultima_actualizacion")]
        public DateTime UltimaActualizacion { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; } = null!;

        public ICollection<MovimientoInventario> Movimientos { get; set; } = new List<MovimientoInventario>();
    }
}
