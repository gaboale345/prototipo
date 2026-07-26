using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa los métodos de pago disponibles (Efectivo, QR, Transferencia, etc.).
    /// </summary>
    [Table("metodos_pago")]
    public class MetodoPago
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
