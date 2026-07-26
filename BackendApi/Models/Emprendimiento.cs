using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa el emprendimiento de lavado de autos.
    /// </summary>
    [Table("emprendimientos")]
    public class Emprendimiento
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(300)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [MaxLength(200)]
        [Column("direccion")]
        public string? Direccion { get; set; }

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(150)]
        [Column("email")]
        public string? Email { get; set; }

        [MaxLength(200)]
        [Column("logo_url")]
        public string? LogoUrl { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
