using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que registra las acciones importantes realizadas en el sistema (trazabilidad completa).
    /// </summary>
    [Table("auditorias")]
    public class Auditoria
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int? UsuarioId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("accion")]
        public string Accion { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("modulo")]
        public string? Modulo { get; set; }

        [MaxLength(50)]
        [Column("entidad")]
        public string? Entidad { get; set; }

        [Column("entidad_id")]
        public int? EntidadId { get; set; }

        [Column("datos_anteriores")]
        public string? DatosAnteriores { get; set; } // JSON

        [Column("datos_nuevos")]
        public string? DatosNuevos { get; set; } // JSON

        [MaxLength(45)]
        [Column("ip")]
        public string? Ip { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("UsuarioId")]
        public Usuario? Usuario { get; set; }
    }
}
