using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un empleado del emprendimiento.
    /// </summary>
    [Table("empleados")]
    public class Empleado
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        [MaxLength(20)]
        [Column("ci")]
        public string? Ci { get; set; }

        [MaxLength(100)]
        [Column("cargo")]
        public string? Cargo { get; set; }

        [Column("salario")]
        public decimal Salario { get; set; } = 0;

        [Column("fecha_ingreso")]
        public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;

        [Column("disponible")]
        public bool Disponible { get; set; } = true;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
