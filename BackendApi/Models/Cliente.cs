using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un cliente del servicio de lavado.
    /// </summary>
    [Table("clientes")]
    public class Cliente
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [MaxLength(20)]
        [Column("ci")]
        public string? Ci { get; set; }

        [MaxLength(200)]
        [Column("direccion")]
        public string? Direccion { get; set; }

        [MaxLength(50)]
        [Column("ciudad")]
        public string? Ciudad { get; set; }

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
        public ICollection<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();
        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
        public ICollection<Calificacion> Calificaciones { get; set; } = new List<Calificacion>();
    }
}
