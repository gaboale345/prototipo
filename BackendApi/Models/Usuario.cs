using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad base de usuario del sistema. Puede ser Cliente, Empleado o Administrador.
    /// </summary>
    [Table("usuarios")]
    public class Usuario
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("emprendimiento_id")]
        public int? EmprendimientoId { get; set; }

        [Column("rol_id")]
        public int RolId { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column("apellido")]
        public string Apellido { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        [Column("email")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        [Column("telefono")]
        public string? Telefono { get; set; }

        [MaxLength(200)]
        [Column("foto_url")]
        public string? FotoUrl { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        [Column("email_verificado")]
        public bool EmailVerificado { get; set; } = false;

        [MaxLength(255)]
        [Column("token_recuperacion")]
        public string? TokenRecuperacion { get; set; }

        [Column("token_expiracion")]
        public DateTime? TokenExpiracion { get; set; }

        [Column("fecha_creacion")]
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        [Column("ultimo_acceso")]
        public DateTime? UltimoAcceso { get; set; }

        // Navegación
        [ForeignKey("EmprendimientoId")]
        public Emprendimiento? Emprendimiento { get; set; }

        [ForeignKey("RolId")]
        public Rol Rol { get; set; } = null!;

        public Cliente? Cliente { get; set; }
        public Empleado? Empleado { get; set; }
        public ICollection<Notificacion> Notificaciones { get; set; } = new List<Notificacion>();
        public ICollection<Auditoria> Auditorias { get; set; } = new List<Auditoria>();
    }
}
