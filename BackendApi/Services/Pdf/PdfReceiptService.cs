using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QRCoder;

namespace BackendApi.Services.Pdf
{
    /// <summary>
    /// Servicio para generar comprobantes de pago en PDF usando QuestPDF.
    /// Incluye logo, datos del cliente, servicios, QR y toda la información requerida.
    /// </summary>
    public class PdfReceiptService : IPdfService
    {
        private readonly ILogger<PdfReceiptService> _logger;

        public PdfReceiptService(ILogger<PdfReceiptService> logger)
        {
            _logger = logger;
            // QuestPDF Community license para uso en proyectos educativos
            QuestPDF.Settings.License = LicenseType.Community;
        }

        /// <summary>
        /// Genera un comprobante PDF de pago con:
        /// - Logo de EcoWash Direct
        /// - Número de orden y transacción
        /// - Datos del cliente
        /// - Servicios contratados con precios
        /// - QR con identificador de transacción
        /// - Estado del pago
        /// </summary>
        public Task<byte[]> GeneratePaymentReceiptAsync(ReceiptData receipt)
        {
            try
            {
                var document = Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        // Tamaño personalizado similar al diseño web apaisado
                        page.Size(650, 400); 
                        page.Margin(0);
                        page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                        // ── CONTENIDO PRINCIPAL ──
                        page.Content().Column(col =>
                        {
                            // ── HEADER VERDE OSCURO ──
                            col.Item().Background("#198754").Padding(15).Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("EcoWash Direct").FontSize(18).Bold().FontColor("#ffffff");
                                    c.Item().Text("Comprobante de Pago Electrónico").FontSize(10).FontColor("#e8f5e9");
                                });
                                row.ConstantItem(180).AlignRight().AlignMiddle().Column(c =>
                                {
                                    c.Item().Background("#212529").PaddingVertical(5).PaddingHorizontal(10).AlignCenter().Text("✓ PAGADO EXITOSAMENTE").FontSize(9).Bold().FontColor("#ffffff");
                                });
                            });

                            // ── CUERPO (Detalles Orden y Cliente) ──
                            col.Item().PaddingTop(15).Background("#f8f9fa").Border(1).BorderColor("#dee2e6").Padding(15).Row(row =>
                            {
                                // Detalles Orden
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("DETALLES DE ORDEN").Bold().FontSize(8).FontColor("#6c757d");
                                    c.Item().PaddingTop(5).Row(r => { r.RelativeItem().Text("Orden Nº:").FontSize(9).FontColor("#6c757d"); r.RelativeItem().AlignRight().Text(receipt.NumeroOrden).Bold().FontSize(9); });
                                    c.Item().Row(r => { r.RelativeItem().Text("Fecha:").FontSize(9).FontColor("#6c757d"); r.RelativeItem().AlignRight().Text($"{receipt.FechaPago:dd/MM/yyyy}").Bold().FontSize(9); });
                                    c.Item().Row(r => { r.RelativeItem().Text("Hora:").FontSize(9).FontColor("#6c757d"); r.RelativeItem().AlignRight().Text($"{receipt.FechaPago:HH:mm}").Bold().FontSize(9); });
                                });

                                row.ConstantItem(1).Background("#dee2e6"); // Linea divisoria vertical
                                row.ConstantItem(15); // Espacio

                                // Datos del Cliente
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("DATOS DEL CLIENTE").Bold().FontSize(8).FontColor("#6c757d");
                                    c.Item().PaddingTop(5).Row(r => { r.RelativeItem().Text("Nombre:").FontSize(9).FontColor("#6c757d"); r.RelativeItem().AlignRight().Text(receipt.NombreCliente).Bold().FontSize(9); });
                                    c.Item().Row(r => { r.RelativeItem().Text("Email:").FontSize(9).FontColor("#6c757d"); r.RelativeItem().AlignRight().Text(receipt.EmailCliente).Bold().FontSize(9); });
                                });
                            });

                            // ── TABLA DE SERVICIOS ──
                            col.Item().PaddingTop(15).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3); // Servicio
                                    columns.RelativeColumn(1); // Cantidad
                                    columns.RelativeColumn(1.5f); // Precio Unit.
                                    columns.RelativeColumn(1.5f); // Subtotal
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Background("#198754").Padding(8).Text("SERVICIO").FontColor("#ffffff").Bold().FontSize(9);
                                    header.Cell().Background("#198754").Padding(8).AlignCenter().Text("CANT.").FontColor("#ffffff").Bold().FontSize(9);
                                    header.Cell().Background("#198754").Padding(8).AlignRight().Text("P. UNIT.").FontColor("#ffffff").Bold().FontSize(9);
                                    header.Cell().Background("#198754").Padding(8).AlignRight().Text("SUBTOTAL").FontColor("#ffffff").Bold().FontSize(9);
                                });

                                // Filas
                                foreach (var item in receipt.Servicios)
                                {
                                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(8).Text(item.NombreServicio).FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(8).AlignCenter().Text(item.Cantidad.ToString()).FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(8).AlignRight().Text($"Bs. {item.PrecioUnitario:N2}").FontSize(9);
                                    table.Cell().BorderBottom(1).BorderColor("#dee2e6").Padding(8).AlignRight().Text($"Bs. {item.Subtotal:N2}").Bold().FontSize(9);
                                }
                            });

                            // Total
                            col.Item().Background("#f8f9fa").Padding(10).AlignRight().Row(row =>
                            {
                                row.RelativeItem();
                                row.ConstantItem(250).Row(r =>
                                {
                                    r.RelativeItem().AlignRight().Text("TOTAL PAGADO").Bold().FontSize(10).FontColor("#6c757d");
                                    r.ConstantItem(15);
                                    r.ConstantItem(100).AlignRight().Text($"Bs. {receipt.TotalPagado:N2}").Bold().FontSize(14).FontColor("#212529");
                                });
                            });

                            // ── INFO DE PAGO Y QR ──
                            col.Item().PaddingTop(15).Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("INFORMACIÓN DE PAGO").Bold().FontSize(8).FontColor("#6c757d");
                                    c.Item().PaddingTop(5).Row(r => { r.ConstantItem(80).Text("Método:").FontSize(9).FontColor("#6c757d"); r.RelativeItem().Text(receipt.MetodoPago).Bold().FontSize(9); });
                                    c.Item().Row(r => { r.ConstantItem(80).Text("Transacción ID:").FontSize(9).FontColor("#6c757d"); r.RelativeItem().Text(receipt.NumeroTransaccion).FontSize(9).FontColor("#6c757d"); });
                                });

                                // QR Code Box
                                if (!string.IsNullOrEmpty(receipt.QrContent))
                                {
                                    row.ConstantItem(180).Background("#f8f9fa").Border(1).BorderColor("#dee2e6").Padding(5).Row(r =>
                                    {
                                        var qrBytes = GenerateQrCode(receipt.QrContent);
                                        if (qrBytes != null)
                                        {
                                            r.ConstantItem(40).Background("#ffffff").Border(1).BorderColor("#dee2e6").Padding(2).Image(qrBytes).FitWidth();
                                        }
                                        r.RelativeItem().PaddingLeft(10).AlignMiddle().Text("Escanea el código QR para validar la autenticidad de este comprobante electrónico.").FontSize(7).FontColor("#6c757d");
                                    });
                                }
                            });
                        });

                        // ── PIE DE PÁGINA ──
                        page.Footer().Column(col =>
                        {
                            col.Item().LineHorizontal(1).LineColor("#dee2e6");
                            col.Item().PaddingTop(5).Row(row =>
                            {
                                row.RelativeItem().Text($"© {DateTime.UtcNow.Year} EcoWash Direct").FontSize(8).FontColor("#aaaaaa");
                                row.RelativeItem().AlignRight().Text("www.ecowash.bo").FontSize(8).FontColor("#aaaaaa");
                            });
                        });
                    });
                });

                var pdfBytes = document.GeneratePdf();
                _logger.LogInformation("Comprobante PDF generado para orden {OrderNumber}", receipt.NumeroOrden);
                return Task.FromResult(pdfBytes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar comprobante PDF para orden {OrderNumber}", receipt.NumeroOrden);
                throw;
            }
        }

        /// <summary>
        /// Genera una imagen QR en formato PNG a partir de un texto.
        /// Se usa para incluir el identificador de transacción en el comprobante.
        /// </summary>
        private byte[]? GenerateQrCode(string content)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                var qrCodeData = qrGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.M);
                using var qrCode = new PngByteQRCode(qrCodeData);
                return qrCode.GetGraphic(5);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al generar código QR: {Error}", ex.Message);
                return null;
            }
        }
    }
}
