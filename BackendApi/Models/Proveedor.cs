using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un proveedor de insumos.
    /// </summary>
    [Table("proveedores")]
    public class Proveedor
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("nit")]
        public string? Nit { get; set; }

        [MaxLength(150)]
        [Column("contacto")]
        public string? Contacto { get; set; }

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(200)]
        [Column("direccion")]
        public string? Direccion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public ICollection<Compra> Compras { get; set; } = new List<Compra>();
    }
}
