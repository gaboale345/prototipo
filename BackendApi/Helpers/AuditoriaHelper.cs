using System.Text.Json;
using BackendApi.Data;
using BackendApi.Models;

namespace BackendApi.Helpers
{
    /// <summary>
    /// Helper para registrar auditorías de acciones en el sistema.
    /// Todas las acciones importantes deben registrarse (regla de negocio).
    /// </summary>
    public class AuditoriaHelper
    {
        private readonly EcoWashDbContext _context;

        public AuditoriaHelper(EcoWashDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Registra una acción en la tabla de auditoría.
        /// </summary>
        public async Task RegistrarAsync(
            string accion,
            string modulo,
            string? entidad = null,
            int? entidadId = null,
            object? datosAnteriores = null,
            object? datosNuevos = null,
            int? usuarioId = null,
            string? ip = null)
        {
            var auditoria = new Auditoria
            {
                Accion = accion,
                Modulo = modulo,
                Entidad = entidad,
                EntidadId = entidadId,
                DatosAnteriores = datosAnteriores != null ? JsonSerializer.Serialize(datosAnteriores) : null,
                DatosNuevos = datosNuevos != null ? JsonSerializer.Serialize(datosNuevos) : null,
                UsuarioId = usuarioId,
                Ip = ip,
                Fecha = DateTime.UtcNow
            };

            _context.Auditorias.Add(auditoria);
            await _context.SaveChangesAsync();
        }
    }
}
