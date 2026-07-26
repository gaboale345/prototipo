using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un tipo de servicio de lavado disponible.
    /// </summary>
    [Table("servicios")]
    public class Servicio
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(300)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("precio")]
        public decimal Precio { get; set; }

        [Column("duracion_minutos")]
        public int DuracionMinutos { get; set; } = 60;

        [MaxLength(50)]
        [Column("tipo_vehiculo")]
        public string? TipoVehiculo { get; set; } // Auto, Moto, Camioneta, Todos

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
