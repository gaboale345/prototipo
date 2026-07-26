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
    public class PagoController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly AuditoriaHelper _auditoria;

        public PagoController(EcoWashDbContext context, AuditoriaHelper auditoria)
        {
            _context = context;
            _auditoria = auditoria;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<PagoDto>>>> GetPagos()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<Pago> query = _context.Pagos
                .Include(p => p.Venta)
                .Include(p => p.MetodoPago);

            if (role == "Cliente")
            {
                var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.UsuarioId == userId);
                if (cliente == null) return Ok(ApiResponse<List<PagoDto>>.Ok(new List<PagoDto>()));
                query = query.Where(p => p.Venta.ClienteId == cliente.Id);
            }

            var result = await query.OrderByDescending(p => p.FechaPago)
                .Select(p => new PagoDto
                {
                    Id = p.Id,
                    VentaId = p.VentaId,
                    ReservaId = p.ReservaId,
                    NumeroVenta = p.Venta.NumeroVenta,
                    MetodoPago = p.MetodoPago.Nombre,
                    Monto = p.Monto,
                    Estado = p.Estado,
                    Referencia = p.Referencia,
                    FechaPago = p.FechaPago
                }).ToListAsync();

            return Ok(ApiResponse<List<PagoDto>>.Ok(result));
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<PagoDto>>> RealizarPago([FromBody] CrearPagoDto dto)
        {
            var reserva = await _context.Reservas
                .Include(r => r.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(r => r.Id == dto.ReservaId);

            if (reserva == null) return NotFound(ApiResponse<PagoDto>.Fail("Reserva no encontrada"));

            // REGLA DE NEGOCIO: El pago solo puede realizarse cuando la reserva esté Aceptada, EnProceso o Finalizada.
            if (reserva.Estado == "Pendiente" || reserva.Estado == "Cancelada" || reserva.Estado == "Rechazada")
            {
                return BadRequest(ApiResponse<PagoDto>.Fail("El pago solo puede realizarse cuando la reserva esté aceptada (Regla de Negocio)"));
            }

            var venta = await _context.Ventas.FindAsync(dto.VentaId);
            if (venta == null) return NotFound(ApiResponse<PagoDto>.Fail("Venta no encontrada"));

            var pago = new Pago
            {
                VentaId = dto.VentaId,
                ReservaId = dto.ReservaId,
                MetodoPagoId = dto.MetodoPagoId,
                Monto = dto.Monto,
                Estado = "Completado",
                Referencia = dto.Referencia,
                FechaPago = DateTime.UtcNow
            };

            _context.Pagos.Add(pago);
            venta.Estado = "Pagada";
            await _context.SaveChangesAsync();

            // Generar factura automáticamente tras el pago
            var factura = new Factura
            {
                VentaId = venta.Id,
                PagoId = pago.Id,
                NumeroFactura = $"FAC-{DateTime.UtcNow:yyyyMMdd}-{pago.Id}",
                FechaEmision = DateTime.UtcNow,
                RazonSocial = $"{reserva.Cliente.Usuario.Nombre} {reserva.Cliente.Usuario.Apellido}",
                Nit = reserva.Cliente.Ci ?? "0",
                Subtotal = venta.Subtotal,
                Descuento = venta.Descuento,
                Total = venta.Total,
                Estado = "Emitida"
            };
            _context.Facturas.Add(factura);

            // Notificaciones
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            _context.Notificaciones.Add(new Notificacion
            {
                UsuarioId = reserva.Cliente.UsuarioId,
                Titulo = "Pago Recibido",
                Mensaje = $"Pago de Bs. {dto.Monto} recibido correctamente para la reserva #{reserva.Id}.",
                Tipo = "Exito",
                Fecha = DateTime.UtcNow
            });
            _context.Notificaciones.Add(new Notificacion
            {
                UsuarioId = reserva.Cliente.UsuarioId,
                Titulo = "Factura Generada",
                Mensaje = $"Se ha generado la Factura N° {factura.NumeroFactura} por tu servicio.",
                Tipo = "Info",
                Fecha = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            await _auditoria.RegistrarAsync("RealizarPago", "Pagos", "Pago", pago.Id, null, dto, userId);

            var metodo = await _context.MetodosPago.FindAsync(dto.MetodoPagoId);
            var resDto = new PagoDto
            {
                Id = pago.Id,
                VentaId = pago.VentaId,
                ReservaId = pago.ReservaId,
                NumeroVenta = venta.NumeroVenta,
                MetodoPago = metodo?.Nombre ?? "",
                Monto = pago.Monto,
                Estado = pago.Estado,
                Referencia = pago.Referencia,
                FechaPago = pago.FechaPago
            };

            return Ok(ApiResponse<PagoDto>.Ok(resDto, "Pago registrado y factura generada exitosamente"));
        }
    }
}
