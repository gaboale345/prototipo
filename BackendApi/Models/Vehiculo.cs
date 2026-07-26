using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un vehículo registrado por un cliente.
    /// Un vehículo pertenece a un único cliente (regla de negocio).
    /// </summary>
    [Table("vehiculos")]
    public class Vehiculo
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Required]
        [MaxLength(20)]
        [Column("placa")]
        public string Placa { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("tipo")]
        public string Tipo { get; set; } = string.Empty; // Auto, Camioneta, Moto, SUV, etc.

        [MaxLength(80)]
        [Column("marca")]
        public string? Marca { get; set; }

        [MaxLength(80)]
        [Column("modelo")]
        public string? Modelo { get; set; }

        [MaxLength(10)]
        [Column("año")]
        public string? Año { get; set; }

        [MaxLength(50)]
        [Column("color")]
        public string? Color { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("fecha_registro")]
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
