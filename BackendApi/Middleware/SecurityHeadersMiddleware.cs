namespace BackendApi.Middleware
{
    /// <summary>
    /// Middleware que agrega headers de seguridad a todas las respuestas HTTP.
    /// Protege contra ataques XSS, clickjacking, MIME sniffing y otros vectores.
    /// </summary>
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Protección contra MIME type sniffing
            context.Response.Headers.Append("X-Content-Type-Options", "nosniff");

            // Protección contra clickjacking
            context.Response.Headers.Append("X-Frame-Options", "DENY");

            // Protección XSS del navegador (legacy, pero sigue siendo útil)
            context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");

            // Content Security Policy básica
            context.Response.Headers.Append("Content-Security-Policy",
                "default-src 'self'; script-src 'self' 'unsafe-inline' https://js.stripe.com; " +
                "style-src 'self' 'unsafe-inline' https://fonts.googleapis.com; " +
                "font-src 'self' https://fonts.gstatic.com; " +
                "img-src 'self' data: blob:; " +
                "frame-src https://js.stripe.com https://hooks.stripe.com; " +
                "connect-src 'self' https://api.stripe.com;");

            // Referrer Policy
            context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");

            // Permissions Policy
            context.Response.Headers.Append("Permissions-Policy",
                "camera=(), microphone=(), geolocation=(self)");

            await _next(context);
        }
    }
}
