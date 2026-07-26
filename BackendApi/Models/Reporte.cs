using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que almacena los reportes generados por el administrador.
    /// Solo el administrador puede generar reportes (regla de negocio).
    /// </summary>
    [Table("reportes")]
    public class Reporte
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [Required]
        [MaxLength(150)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(80)]
        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty;
        // Tipos: VentasDiarias, VentasMensuales, ClientesFrecuentes, ServiciosMasSolicitados, etc.

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Column("datos")]
        public string? Datos { get; set; } // JSON con los resultados

        [Column("fecha_generacion")]
        public DateTime FechaGeneracion { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;
    }
}
