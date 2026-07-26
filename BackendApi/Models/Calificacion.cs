using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa la calificación de un servicio por parte del cliente.
    /// </summary>
    [Table("calificaciones")]
    public class Calificacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("reserva_id")]
        public int ReservaId { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("puntuacion")]
        public int Puntuacion { get; set; } // 1-5 estrellas

        [MaxLength(500)]
        [Column("comentario")]
        public string? Comentario { get; set; }

        [Column("fecha")]
        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("ReservaId")]
        public Reserva Reserva { get; set; } = null!;

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;
    }
}
