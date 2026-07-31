using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApi.Controllers
{
    /// <summary>
    /// Controlador para descarga de comprobantes PDF de pago.
    /// Los clientes solo pueden descargar sus propios comprobantes.
    /// Los administradores pueden descargar cualquier comprobante.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ReceiptController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly IPdfService _pdfService;

        public ReceiptController(EcoWashDbContext context, IPdfService pdfService)
        {
            _context = context;
            _pdfService = pdfService;
        }

        /// <summary>
        /// Descarga el comprobante PDF de una transacción de pago.
        /// Si el PDF ya fue generado, lo sirve desde disco.
        /// Si no, lo regenera dinámicamente.
        /// </summary>
        [HttpGet("{transactionId}/download")]
        public async Task<IActionResult> DownloadReceipt(int transactionId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var transaction = await _context.PaymentTransactions
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Detalles)
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(t => t.Id == transactionId);

            if (transaction == null)
                return NotFound("Transacción no encontrada.");

            // Solo el dueño o admin puede descargar el comprobante
            if (role == "Cliente" && transaction.UsuarioId != userId)
                return Forbid();

            if (transaction.Estado != "Pagado")
                return BadRequest("El comprobante solo está disponible para pagos confirmados.");

            // Intentar servir PDF desde disco
            var pdfDir = Path.Combine(Directory.GetCurrentDirectory(), "receipts");
            var pdfPath = Path.Combine(pdfDir, $"receipt_{transaction.ServiceOrder.NumeroOrden}.pdf");

            byte[] pdfBytes;
            if (System.IO.File.Exists(pdfPath))
            {
                pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfPath);
            }
            else
            {
                // Regenerar PDF dinámicamente
                var receiptData = new ReceiptData
                {
                    NumeroOrden = transaction.ServiceOrder.NumeroOrden,
                    NumeroTransaccion = transaction.TransactionId ?? transaction.Id.ToString(),
                    NombreCliente = $"{transaction.ServiceOrder.Cliente.Usuario.Nombre} {transaction.ServiceOrder.Cliente.Usuario.Apellido}",
                    EmailCliente = transaction.ServiceOrder.Cliente.Usuario.Email,
                    FechaPago = transaction.FechaPago ?? DateTime.UtcNow,
                    Servicios = transaction.ServiceOrder.Detalles.Select(d => new ReceiptItem
                    {
                        NombreServicio = d.NombreServicio,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList(),
                    TotalPagado = transaction.Monto,
                    MetodoPago = transaction.MetodoPago ?? "Tarjeta",
                    EstadoPago = "Pagado",
                    QrContent = $"ecowash://receipt/{transaction.TransactionId ?? transaction.Id.ToString()}"
                };

                pdfBytes = await _pdfService.GeneratePaymentReceiptAsync(receiptData);

                // Guardar para futuras descargas
                Directory.CreateDirectory(pdfDir);
                await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);
            }

            return File(pdfBytes, "application/pdf", $"Comprobante_{transaction.ServiceOrder.NumeroOrden}.pdf");
        }
    }
}
