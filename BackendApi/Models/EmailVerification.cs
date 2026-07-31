using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad para verificación de correo electrónico mediante código OTP de 6 dígitos.
    /// El código expira después de 10 minutos.
    /// </summary>
    [Table("email_verifications")]
    public class EmailVerification
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("usuario_id")]
        public int UsuarioId { get; set; }

        /// <summary>Código OTP de 6 dígitos enviado al correo del usuario.</summary>
        [Required]
        [MaxLength(6)]
        [Column("codigo")]
        public string Codigo { get; set; } = string.Empty;

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>El código expira 10 minutos después de su creación.</summary>
        [Column("fecha_expiracion")]
        public DateTime FechaExpiracion { get; set; }

        /// <summary>Indica si el código ya fue utilizado para verificar el email.</summary>
        [Column("usado")]
        public bool Usado { get; set; } = false;

        /// <summary>Número de intentos fallidos de verificación para este código.</summary>
        [Column("intentos")]
        public int Intentos { get; set; } = 0;

        // Navegación
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; } = null!;
    }
}
