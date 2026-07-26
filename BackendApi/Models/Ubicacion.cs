using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa una ubicación registrada por un cliente para el servicio a domicilio.
    /// </summary>
    [Table("ubicaciones")]
    public class Ubicacion
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Required]
        [MaxLength(200)]
        [Column("direccion")]
        public string Direccion { get; set; } = string.Empty;

        [MaxLength(100)]
        [Column("zona")]
        public string? Zona { get; set; }

        [MaxLength(100)]
        [Column("referencia")]
        public string? Referencia { get; set; }

        [Column("latitud")]
        public double? Latitud { get; set; }

        [Column("longitud")]
        public double? Longitud { get; set; }

        [Column("es_principal")]
        public bool EsPrincipal { get; set; } = false;

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        public ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}
