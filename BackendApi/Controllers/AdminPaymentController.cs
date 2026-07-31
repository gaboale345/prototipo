using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Services.Pdf;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace BackendApi.Controllers
{
    /// <summary>
    /// Panel administrativo de pagos. Solo accesible por Administradores.
    /// Incluye: ver pagos, filtrar, buscar, estadísticas y exportación a PDF/Excel.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrador")]
    public class AdminPaymentController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly ILogger<AdminPaymentController> _logger;

        public AdminPaymentController(EcoWashDbContext context, ILogger<AdminPaymentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los pagos con filtros opcionales (estado, fecha, usuario, búsqueda).
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<ApiResponse<List<PaymentTransactionDto>>>> GetAllPayments(
            [FromQuery] string? estado = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null,
            [FromQuery] int? usuarioId = null,
            [FromQuery] string? busqueda = null)
        {
            IQueryable<Models.PaymentTransaction> query = _context.PaymentTransactions
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Detalles)
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Cliente).ThenInclude(c => c.Usuario);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(t => t.Estado == estado);
            if (fechaInicio.HasValue)
                query = query.Where(t => t.FechaCreacion >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(t => t.FechaCreacion <= fechaFin.Value);
            if (usuarioId.HasValue)
                query = query.Where(t => t.UsuarioId == usuarioId.Value);
            if (!string.IsNullOrEmpty(busqueda))
                query = query.Where(t =>
                    t.ServiceOrder.NumeroOrden.Contains(busqueda) ||
                    t.TransactionId!.Contains(busqueda) ||
                    t.ServiceOrder.Cliente.Usuario.Nombre.Contains(busqueda) ||
                    t.ServiceOrder.Cliente.Usuario.Email.Contains(busqueda));

            var transactions = await query.OrderByDescending(t => t.FechaCreacion).ToListAsync();
            var result = transactions.Select(t => new PaymentTransactionDto
            {
                Id = t.Id,
                ServiceOrderId = t.ServiceOrderId,
                NumeroOrden = t.ServiceOrder.NumeroOrden,
                TransactionId = t.TransactionId,
                Estado = t.Estado,
                Monto = t.Monto,
                Moneda = t.Moneda,
                MetodoPago = t.MetodoPago,
                ReferenciaPasarela = t.ReferenciaPasarela,
                FechaCreacion = t.FechaCreacion,
                FechaPago = t.FechaPago,
                ProveedorPago = t.ProveedorPago,
                ComprobantePdfUrl = t.ComprobantePdfUrl,
                NombreCliente = $"{t.ServiceOrder.Cliente.Usuario.Nombre} {t.ServiceOrder.Cliente.Usuario.Apellido}",
                EmailCliente = t.ServiceOrder.Cliente.Usuario.Email,
                DetallesOrden = t.ServiceOrder.Detalles.Select(d => new ServiceOrderDetailDto
                {
                    Id = d.Id,
                    ServicioId = d.ServicioId,
                    NombreServicio = d.NombreServicio,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = d.Subtotal
                }).ToList()
            }).ToList();

            return Ok(ApiResponse<List<PaymentTransactionDto>>.Ok(result));
        }

        /// <summary>
        /// Estadísticas de ingresos: total recaudado, diarios, mensuales y anuales.
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<PaymentStatsDto>>> GetPaymentStats()
        {
            var allTransactions = await _context.PaymentTransactions.ToListAsync();

            var paid = allTransactions.Where(t => t.Estado == "Pagado").ToList();

            // Ingresos diarios (últimos 30 días)
            var dailyStart = DateTime.UtcNow.AddDays(-30);
            var ingresosDiarios = paid
                .Where(t => t.FechaPago >= dailyStart)
                .GroupBy(t => t.FechaPago!.Value.Date)
                .Select(g => new GraficoDto { Etiqueta = g.Key.ToString("dd/MM"), Valor = g.Sum(x => x.Monto) })
                .OrderBy(g => g.Etiqueta)
                .ToList();

            // Ingresos mensuales (últimos 12 meses)
            var monthlyStart = DateTime.UtcNow.AddMonths(-12);
            var ingresosMensuales = paid
                .Where(t => t.FechaPago >= monthlyStart)
                .GroupBy(t => new { t.FechaPago!.Value.Year, t.FechaPago.Value.Month })
                .Select(g => new GraficoDto { Etiqueta = $"{g.Key.Month:00}/{g.Key.Year}", Valor = g.Sum(x => x.Monto) })
                .OrderBy(g => g.Etiqueta)
                .ToList();

            // Ingresos anuales
            var ingresosAnuales = paid
                .GroupBy(t => t.FechaPago!.Value.Year)
                .Select(g => new GraficoDto { Etiqueta = g.Key.ToString(), Valor = g.Sum(x => x.Monto) })
                .OrderBy(g => g.Etiqueta)
                .ToList();

            var stats = new PaymentStatsDto
            {
                TotalRecaudado = paid.Sum(t => t.Monto),
                TotalTransacciones = allTransactions.Count,
                PagosPendientes = allTransactions.Count(t => t.Estado == "Pendiente"),
                PagosExitosos = allTransactions.Count(t => t.Estado == "Pagado"),
                PagosFallidos = allTransactions.Count(t => t.Estado == "Fallido"),
                PagosCancelados = allTransactions.Count(t => t.Estado == "Cancelado"),
                IngresosDiarios = ingresosDiarios,
                IngresosMensuales = ingresosMensuales,
                IngresosAnuales = ingresosAnuales
            };

            return Ok(ApiResponse<PaymentStatsDto>.Ok(stats));
        }

        /// <summary>
        /// Exportar pagos a archivo Excel (.xlsx).
        /// </summary>
        [HttpGet("export/excel")]
        public async Task<IActionResult> ExportToExcel(
            [FromQuery] string? estado = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var transactions = await GetFilteredTransactions(estado, fechaInicio, fechaFin);

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Pagos");

            // Headers
            var headers = new[] { "ID", "Orden", "Cliente", "Email", "Monto (Bs.)", "Estado", "Método", "Fecha Pago", "Transacción", "Proveedor" };
            for (int i = 0; i < headers.Length; i++)
            {
                ws.Cell(1, i + 1).Value = headers[i];
                ws.Cell(1, i + 1).Style.Font.Bold = true;
                ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#2d6a4f");
                ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
            }

            // Data
            int row = 2;
            foreach (var t in transactions)
            {
                ws.Cell(row, 1).Value = t.Id;
                ws.Cell(row, 2).Value = t.ServiceOrder.NumeroOrden;
                ws.Cell(row, 3).Value = $"{t.ServiceOrder.Cliente.Usuario.Nombre} {t.ServiceOrder.Cliente.Usuario.Apellido}";
                ws.Cell(row, 4).Value = t.ServiceOrder.Cliente.Usuario.Email;
                ws.Cell(row, 5).Value = t.Monto;
                ws.Cell(row, 6).Value = t.Estado;
                ws.Cell(row, 7).Value = t.MetodoPago ?? "-";
                ws.Cell(row, 8).Value = t.FechaPago?.ToString("dd/MM/yyyy HH:mm") ?? "-";
                ws.Cell(row, 9).Value = t.TransactionId ?? "-";
                ws.Cell(row, 10).Value = t.ProveedorPago;
                row++;
            }

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            var content = stream.ToArray();

            return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Pagos_EcoWash_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }

        /// <summary>
        /// Exportar pagos a archivo PDF.
        /// </summary>
        [HttpGet("export/pdf")]
        public async Task<IActionResult> ExportToPdf(
            [FromQuery] string? estado = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var transactions = await GetFilteredTransactions(estado, fechaInicio, fechaFin);
            var totalRecaudado = transactions.Where(t => t.Estado == "Pagado").Sum(t => t.Monto);

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4.Landscape());
                    page.Margin(30);
                    page.DefaultTextStyle(x => x.FontSize(9));

                    page.Header().Column(col =>
                    {
                        col.Item().Row(row =>
                        {
                            row.RelativeItem().Text("🌿 EcoWash Direct — Reporte de Pagos").FontSize(16).Bold().FontColor("#2d6a4f");
                            row.ConstantItem(150).AlignRight().Text($"Generado: {DateTime.UtcNow:dd/MM/yyyy HH:mm}").FontSize(8);
                        });
                        col.Item().Text($"Total Recaudado: Bs. {totalRecaudado:N2} | Transacciones: {transactions.Count}").FontSize(10).Bold();
                        col.Item().PaddingVertical(5).LineHorizontal(1).LineColor("#2d6a4f");
                    });

                    page.Content().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.ConstantColumn(40);  // ID
                            columns.RelativeColumn(2);   // Orden
                            columns.RelativeColumn(3);   // Cliente
                            columns.RelativeColumn(1.5f);// Monto
                            columns.RelativeColumn(1.5f);// Estado
                            columns.RelativeColumn(1.5f);// Método
                            columns.RelativeColumn(2);   // Fecha
                        });

                        var headerCols = new[] { "#", "Orden", "Cliente", "Monto", "Estado", "Método", "Fecha Pago" };
                        table.Header(header =>
                        {
                            foreach (var h in headerCols)
                                header.Cell().Background("#2d6a4f").Padding(5).Text(h).FontColor("#ffffff").Bold().FontSize(8);
                        });

                        bool alt = false;
                        foreach (var t in transactions)
                        {
                            var bg = alt ? "#f8f9fa" : "#ffffff";
                            table.Cell().Background(bg).Padding(4).Text(t.Id.ToString()).FontSize(8);
                            table.Cell().Background(bg).Padding(4).Text(t.ServiceOrder.NumeroOrden).FontSize(8);
                            table.Cell().Background(bg).Padding(4).Text($"{t.ServiceOrder.Cliente.Usuario.Nombre} {t.ServiceOrder.Cliente.Usuario.Apellido}").FontSize(8);
                            table.Cell().Background(bg).Padding(4).Text($"Bs. {t.Monto:N2}").FontSize(8);
                            table.Cell().Background(bg).Padding(4).Text(t.Estado).FontSize(8);
                            table.Cell().Background(bg).Padding(4).Text(t.MetodoPago ?? "-").FontSize(8);
                            table.Cell().Background(bg).Padding(4).Text(t.FechaPago?.ToString("dd/MM/yyyy HH:mm") ?? "-").FontSize(8);
                            alt = !alt;
                        }
                    });

                    page.Footer().AlignCenter().Text($"© {DateTime.UtcNow.Year} EcoWash Direct").FontSize(7).FontColor("#aaa");
                });
            });

            var pdfBytes = document.GeneratePdf();
            return File(pdfBytes, "application/pdf", $"Pagos_EcoWash_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }

        private async Task<List<Models.PaymentTransaction>> GetFilteredTransactions(
            string? estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            IQueryable<Models.PaymentTransaction> query = _context.PaymentTransactions
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Cliente).ThenInclude(c => c.Usuario);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(t => t.Estado == estado);
            if (fechaInicio.HasValue)
                query = query.Where(t => t.FechaCreacion >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(t => t.FechaCreacion <= fechaFin.Value);

            return await query.OrderByDescending(t => t.FechaCreacion).ToListAsync();
        }
    }
}
