using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa una notificación enviada a un usuario del sistema.
    /// </summary>
    [Table("notificaciones")]
    public class Notificacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("titulo")]
        public string Titulo { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        [Column("mensaje")]
        public string Mensaje { get; set; } = string.Empty;

        [MaxLength(50)]
        [Column("tipo")]
        public string Tipo { get; set; } = "Info"; // Info, Alerta, Exito, Error

        [Column("leida")]
        public bool Leida { get; set; } = false;

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        [MaxLength(100)]
        [Column("referencia_url")]
        public string? ReferenciaUrl { get; set; }

        // Navegación
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;
    }
}
