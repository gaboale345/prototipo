using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BackendApi.Models
{
    /// <summary>
    /// Tabla intermedia que relaciona roles con permisos (muchos a muchos).
    /// </summary>
    [Table("rol_permisos")]
    public class RolPermiso
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("rol_id")]
        public int RolId { get; set; }

        [Column("permiso_id")]
        public int PermisoId { get; set; }

        // Navegación
        [ForeignKey("RolId")]
        public Rol Rol { get; set; } = null!;

        [ForeignKey("PermisoId")]
        public Permiso Permiso { get; set; } = null!;
    }
}
