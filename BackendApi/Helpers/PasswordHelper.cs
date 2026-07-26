namespace BackendApi.Helpers
{
    /// <summary>
    /// Helper para operaciones de hash de contraseñas con BCrypt.
    /// Factor de trabajo 12 para mayor seguridad.
    /// </summary>
    public static class PasswordHelper
    {
        private const int WorkFactor = 12;

        /// <summary>
        /// Genera el hash seguro de una contraseña.
        /// </summary>
        public static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        /// <summary>
        /// Verifica una contraseña contra su hash almacenado.
        /// </summary>
        public static bool VerifyPassword(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }

        /// <summary>
        /// Genera un token seguro aleatorio para recuperación de contraseña.
        /// </summary>
        public static string GenerarTokenRecuperacion()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray())
                   .Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
    }
}
