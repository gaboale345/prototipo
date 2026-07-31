using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Detalle de cada servicio dentro de una orden.
    /// El precio se captura al momento de la compra desde la tabla servicios.
    /// </summary>
    [Table("service_order_details")]
    public class ServiceOrderDetail
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("service_order_id")]
        public int ServiceOrderId { get; set; }

        [Column("servicio_id")]
        public int ServicioId { get; set; }

        /// <summary>Nombre del servicio capturado al momento de la compra.</summary>
        [Required]
        [MaxLength(150)]
        [Column("nombre_servicio")]
        public string NombreServicio { get; set; } = string.Empty;

        [Column("cantidad")]
        public int Cantidad { get; set; } = 1;

        /// <summary>Precio unitario tomado de la BD al momento de confirmar la compra.</summary>
        [Column("precio_unitario")]
        public decimal PrecioUnitario { get; set; }

        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        // Navegación
        [ForeignKey("ServiceOrderId")]
        public ServiceOrder ServiceOrder { get; set; } = null!;

        [ForeignKey("ServicioId")]
        public Servicio Servicio { get; set; } = null!;
    }
}
