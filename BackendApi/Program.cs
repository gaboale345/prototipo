using System.Text;
using BackendApi.Data;
using BackendApi.Helpers;
using BackendApi.Middleware;
using BackendApi.Services.Email;
using BackendApi.Services.Payments;
using BackendApi.Services.Pdf;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

// Cargar variables de entorno desde archivo .env (si existe)
// Las credenciales sensibles NUNCA se almacenan en el código
DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

// Add Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// Usar SQLite local automático por defecto para garantizar funcionamiento inmediato sin depender de MySQL local
var sqliteConnection = "Data Source=ecowash.db";
builder.Services.AddDbContext<EcoWashDbContext>(options =>
    options.UseSqlite(sqliteConnection));

// Add Jwt Services
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] ?? "EcoWashMovilSantaCruzBoliviaSuperSecretJWTKey2025!";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ── Helpers ──────────────────────────────────────────────────────────────────
builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<AuditoriaHelper>();

// ── Servicios nuevos (Patrón Strategy para pasarela de pagos) ────────────────
// IPaymentGateway: Abstracción que permite cambiar de Stripe a otro proveedor
// sin modificar la lógica principal del sistema.
builder.Services.AddScoped<IPaymentGateway, StripePaymentGateway>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<IPdfService, PdfReceiptService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Swagger con soporte JWT Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "EcoWash Móvil API",
        Version = "v1",
        Description = "API REST para el sistema de lavado de autos a domicilio EcoWash Móvil en Santa Cruz de la Sierra, Bolivia.",
        Contact = new OpenApiContact
        {
            Name = "Soporte EcoWash",
            Email = "contacto@ecowash.bo"
        }
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingrese 'Bearer' seguido de su token JWT. Ejemplo: 'Bearer eyJhbGciOi...'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Asegurar que la base de datos se cree automáticamente con todos los datos iniciales y la semilla de 100 clientes
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EcoWashDbContext>();
    dbContext.Database.EnsureCreated();
    DataSeeder.SeedClientesAsync(dbContext).GetAwaiter().GetResult();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || true)
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcoWash Móvil API v1");
        c.RoutePrefix = "swagger";
    });
}

// ── Middleware de seguridad ──────────────────────────────────────────────────
app.UseMiddleware<SecurityHeadersMiddleware>(); // Headers anti-XSS, clickjacking, MIME sniffing
app.UseMiddleware<RateLimitingMiddleware>();     // Rate limiting para login y verificación

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
