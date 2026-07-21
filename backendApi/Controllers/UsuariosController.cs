using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace backendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    // Almacén en memoria para demostración. Reemplazar por EF Core/DB en producción.
    private static readonly ConcurrentDictionary<int, UsuarioDto> Store = new();
    private static int _lastId = 0;

    [HttpGet]
    public ActionResult<IEnumerable<UsuarioDto>> GetAll()
    {
        return Ok(Store.Values);
    }

    [HttpGet("{id:int}")]
    public ActionResult<UsuarioDto> Get(int id)
    {
        if (Store.TryGetValue(id, out var usuario)) return Ok(usuario);
        return NotFound(new { message = "Usuario no encontrado" });
    }

    [HttpPost]
    public ActionResult<UsuarioDto> Create([FromBody] UsuarioCreateDto input)
    {
        if (string.IsNullOrWhiteSpace(input.Nombre) || string.IsNullOrWhiteSpace(input.Email) || string.IsNullOrWhiteSpace(input.Password))
            return BadRequest(new { message = "Nombre, email y password son requeridos" });

        var id = System.Threading.Interlocked.Increment(ref _lastId);
        var usuario = new UsuarioDto
        {
            Id = id,
            Nombre = input.Nombre,
            Email = input.Email,
            PasswordHash = HashPassword(input.Password)
        };
        Store[id] = usuario;
        return CreatedAtAction(nameof(Get), new { id = usuario.Id }, usuario);
    }

    [HttpPut("{id:int}")]
    public ActionResult<UsuarioDto> Update(int id, [FromBody] UsuarioUpdateDto input)
    {
        if (!Store.TryGetValue(id, out var existing)) return NotFound(new { message = "Usuario no encontrado" });
        existing.Nombre = input.Nombre ?? existing.Nombre;
        existing.Email = input.Email ?? existing.Email;
        if (!string.IsNullOrWhiteSpace(input.Password)) existing.PasswordHash = HashPassword(input.Password);
        Store[id] = existing;
        return Ok(existing);
    }

    [HttpDelete("{id:int}")]
    public IActionResult Delete(int id)
    {
        if (Store.TryRemove(id, out _)) return NoContent();
        return NotFound(new { message = "Usuario no encontrado" });
    }

    // Método de autenticación simple (demo)
    [HttpPost("login")]
    public ActionResult Login([FromBody] LoginDto input)
    {
        var user = Store.Values.FirstOrDefault(u => u.Email == input.Email);
        if (user is null) return Unauthorized(new { message = "Credenciales inválidas" });
        if (VerifyPassword(input.Password, user.PasswordHash))
            return Ok(new { message = "Autenticación exitosa", user = new { user.Id, user.Nombre, user.Email } });
        return Unauthorized(new { message = "Credenciales inválidas" });
    }

    // Ejemplo: GET /api/usuarios/example
    [HttpGet("example")]
    public ActionResult Example()
    {
        var ejemplo = new UsuarioDto
        {
            Id = 0,
            Nombre = "Juan Pérez",
            Email = "juan.perez@example.com",
            PasswordHash = "<hashed-password-hidden>"
        };
        return Ok(new {
            message = "Usuario de ejemplo",
            ejemplo,
            instrucciones = new {
                crear = "POST /api/usuarios body: { nombre, email, password }",
                login = "POST /api/usuarios/login body: { email, password }",
                obtener = "GET /api/usuarios and GET /api/usuarios/{id}"
            }
        });
    }

    // Hash simple SHA256 para demo. En producción usar un algoritmo adaptativo (BCrypt/Argon2) o ASP.NET Identity.
    private static string HashPassword(string password)
    {
        using var sha = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        return HashPassword(password) == hash;
    }

    // DTOs locales
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        // PasswordHash no debe devolverse en APIs reales; incluido aquí por simplicidad.
        public string PasswordHash { get; set; } = string.Empty;
    }

    public class UsuarioCreateDto
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class UsuarioUpdateDto
    {
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
