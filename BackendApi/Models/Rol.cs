using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Entidad que representa un rol de usuario en el sistema (Administrador, Empleado, Cliente).
    /// </summary>
    [Table("roles")]
    public class Rol
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [MaxLength(80)]
        [Column("nombre")]
        public string Nombre { get; set; } = string.Empty;

        [MaxLength(200)]
        [Column("descripcion")]
        public string? Descripcion { get; set; }

        [Column("activo")]
        public bool Activo { get; set; } = true;

        // Navegación
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public ICollection<RolPermiso> RolPermisos { get; set; } = new List<RolPermiso>();
    }
}
