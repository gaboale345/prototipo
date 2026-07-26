using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que registra cada movimiento (entrada/salida) del inventario.
    /// </summary>
    [Table("movimientos_inventario")]
    public class MovimientoInventario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("inventario_id")]
        public int InventarioId { get; set; }

        [Column("producto_id")]
        public int ProductoId { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Column("reserva_id")]
        public int? ReservaId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("tipo")]
        public string Tipo { get; set; } = "Salida"; // Entrada, Salida, Ajuste

        [Column("cantidad")]
        public int Cantidad { get; set; }

        [Column("cantidad_anterior")]
        public int CantidadAnterior { get; set; }

        [Column("cantidad_nueva")]
        public int CantidadNueva { get; set; }

        [MaxLength(200)]
        [Column("motivo")]
        public string? Motivo { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("InventarioId")]
        public Inventario Inventario { get; set; } = null!;

        [ForeignKey("ProductoId")]
        public Producto Producto { get; set; } = null!;

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;
    }
}
