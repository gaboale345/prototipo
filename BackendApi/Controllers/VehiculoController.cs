using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using BackendApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiculoController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public VehiculoController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<VehiculoDto>>>> GetVehiculos()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Vehiculo> query = _context.Vehiculos
                .Include(v => v.Cliente)
                .ThenInclude(c => c.Usuario)
                .Where(v => v.Activo);

            if (role == "Cliente")
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cliente == null) return Ok(ApiResponse<List<VehiculoDto>>.Ok(new List<VehiculoDto>()));
                query = query.Where(v => v.ClienteId == cliente.Id);
            }

            var result = await query.Select(v => new VehiculoDto
            {
                Id = v.Id,
                ClienteId = v.ClienteId,
                NombreCliente = $"{v.Cliente.Usuario.Nombre} {v.Cliente.Usuario.Apellido}",
                Placa = v.Placa,
                Tipo = v.Tipo,
                Marca = v.Marca,
                Modelo = v.Modelo,
                Año = v.Año,
                Color = v.Color,
                Activo = v.Activo
            }).ToListAsync();

            return Ok(ApiResponse<List<VehiculoDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<VehiculoDto>>> RegistrarVehiculo([FromBody] CrearVehiculoDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);

            if (cliente == null)
            {
                return BadRequest(ApiResponse<VehiculoDto>.Fail("Debes estar registrado como cliente para registrar un vehículo"));
            }

            if (await _context.Vehiculos.AnyAsync(v => v.Placa.ToUpper() == dto.Placa.ToUpper() && v.Activo))
            {
                return BadRequest(ApiResponse<VehiculoDto>.Fail("Ya existe un vehículo registrado con esta placa"));
            }

            var vehiculo = new Vehiculo
            {
                ClienteId = cliente.Id,
                Placa = dto.Placa.ToUpper(),
                Tipo = dto.Tipo,
                Marca = dto.Marca,
                Modelo = dto.Modelo,
                Año = dto.Año,
                Color = dto.Color,
                Activo = true,
                FechaRegistro = DateTime.UtcNow
            };

            _context.Vehiculos.Add(vehiculo);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("RegistrarVehiculo", "Vehiculos", "Vehiculo", vehiculo.Id, null, dto, userId);

            var resDto = new VehiculoDto
            {
                Id = vehiculo.Id,
                ClienteId = vehiculo.ClienteId,
                NombreCliente = $"{User.FindFirstValue(ClaimTypes.Name)}",
                Placa = vehiculo.Placa,
                Tipo = vehiculo.Tipo,
                Marca = vehiculo.Marca,
                Modelo = vehiculo.Modelo,
                Año = vehiculo.Año,
                Color = vehiculo.Color,
                Activo = vehiculo.Activo
            };

            return Ok(ApiResponse<VehiculoDto>.Ok(resDto, "Vehículo registrado correctamente"));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> EliminarVehiculo(int id)
        {
            var vehiculo = await _context.Vehiculos.FindAsync(id);
            if (vehiculo == null) return NotFound(ApiResponse<string>.Fail("Vehículo no encontrado"));

            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
            var role = User.FindFirstValue(ClaimTypes.Role);

            if (role != "Administrador" && (cliente == null || vehiculo.ClienteId != cliente.Id))
                return Forbid();

            vehiculo.Activo = false;
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("EliminarVehiculo", "Vehiculos", "Vehiculo", id, null, null, userId);

            return Ok(ApiResponse<string>.Ok("Vehículo eliminado correctamente"));
        }
    }
}
