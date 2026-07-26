using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un pago registrado por el cliente.
    /// Regla de negocio: solo si la reserva está Aceptada.
    /// </summary>
    [Table("pagos")]
    public class Pago
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("venta_id")]
        public int VentaId { get; set; }

        [Column("reserva_id")]
        public int ReservaId { get; set; }

        [Column("metodo_pago_id")]
        public int MetodoPagoId { get; set; }

        [Column("monto")]
        public decimal Monto { get; set; }

        [Column("fecha_pago")]
        public DateTime FechaPago { get; set; } = DateTime.UtcNow;

        [MaxLength(30)]
        [Column("estado")]
        public string Estado { get; set; } = "Completado"; // Completado, Rechazado, Pendiente

        [MaxLength(100)]
        [Column("referencia")]
        public string? Referencia { get; set; }

        // Navegación
        [ForeignKey("VentaId")]
        public Venta Venta { get; set; } = null!;

        [ForeignKey("ReservaId")]
        public Reserva Reserva { get; set; } = null!;

        [ForeignKey("MetodoPagoId")]
        public MetodoPago MetodoPago { get; set; } = null!;

        public Factura? Factura { get; set; }
    }
}
