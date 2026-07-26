using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad central del sistema. Representa una reserva de lavado.
    /// Reglas de negocio: empleado disponible, sin doble asignación de horario.
    /// </summary>
    [Table("reservas")]
    public class Reserva
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("cliente_id")]
        public int ClienteId { get; set; }

        [Column("empleado_id")]
        public int? EmpleadoId { get; set; }

        [Column("vehiculo_id")]
        public int VehiculoId { get; set; }

        [Column("ubicacion_id")]
        public int UbicacionId { get; set; }

        [Column("servicio_id")]
        public int ServicioId { get; set; }

        [Column("fecha_programada")]
        public DateTime FechaProgramada { get; set; }

        [Column("fecha_inicio")]
        public DateTime? FechaInicio { get; set; }

        [Column("fecha_fin")]
        public DateTime? FechaFin { get; set; }

        [Required]
        [MaxLength(30)]
        [Column("estado")]
        public string Estado { get; set; } = "Pendiente";
        // Estados: Pendiente, Aceptada, EnProceso, Finalizada, Cancelada, Rechazada

        [Column("precio_total")]
        public decimal PrecioTotal { get; set; }

        [MaxLength(300)]
        [Column("observaciones")]
        public string? Observaciones { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        // Navegación
        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; } = null!;

        [ForeignKey("EmpleadoId")]
        public Empleado? Empleado { get; set; }

        [ForeignKey("VehiculoId")]
        public Vehiculo Vehiculo { get; set; } = null!;

        [ForeignKey("UbicacionId")]
        public Ubicacion Ubicacion { get; set; } = null!;

        [ForeignKey("ServicioId")]
        public Servicio Servicio { get; set; } = null!;

        public Venta? Venta { get; set; }
        public Calificacion? Calificacion { get; set; }
        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
