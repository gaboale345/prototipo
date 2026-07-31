using System.Collections.Concurrent;

namespace BackendApi.Middleware
{
    /// <summary>
    /// Middleware de Rate Limiting basado en IP para proteger endpoints sensibles.
    /// Limita intentos de login y verificación de email para prevenir ataques de fuerza bruta.
    /// 
    /// Configuración:
    /// - Login: 5 intentos por minuto por IP
    /// - Verificación de email: 3 intentos por minuto por IP
    /// </summary>
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;

        // Almacena contadores de peticiones por IP y endpoint
        private static readonly ConcurrentDictionary<string, RateLimitEntry> _rateLimitStore = new();

        // Configuración de límites por endpoint
        private static readonly Dictionary<string, (int MaxRequests, int WindowSeconds)> _limitConfig = new()
        {
            { "/api/auth/login", (5, 60) },           // 5 intentos/minuto
            { "/api/emailverification/verify", (3, 60) }, // 3 intentos/minuto
            { "/api/emailverification/resend", (2, 60) }, // 2 reenvíos/minuto
        };

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? "";
            var method = context.Request.Method;

            // Solo aplicar rate limiting a endpoints POST configurados
            if (method == "POST" && _limitConfig.TryGetValue(path, out var config))
            {
                var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                var key = $"{ip}:{path}";

                var entry = _rateLimitStore.GetOrAdd(key, _ => new RateLimitEntry());

                lock (entry)
                {
                    // Limpiar entradas expiradas
                    var now = DateTime.UtcNow;
                    entry.Requests.RemoveAll(t => (now - t).TotalSeconds > config.WindowSeconds);

                    if (entry.Requests.Count >= config.MaxRequests)
                    {
                        _logger.LogWarning(
                            "Rate limit excedido. IP={IP}, Endpoint={Endpoint}, Intentos={Count}/{Max}",
                            ip, path, entry.Requests.Count, config.MaxRequests);

                        context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                        context.Response.ContentType = "application/json";
                        var response = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            success = false,
                            message = "Demasiados intentos. Por favor espere un momento antes de intentar nuevamente.",
                            retryAfterSeconds = config.WindowSeconds
                        });
                        context.Response.WriteAsync(response).GetAwaiter().GetResult();
                        return;
                    }

                    entry.Requests.Add(now);
                }
            }

            await _next(context);
        }

        /// <summary>Entrada de control de rate limiting por IP+endpoint.</summary>
        private class RateLimitEntry
        {
            public List<DateTime> Requests { get; set; } = new();
        }
    }
}
