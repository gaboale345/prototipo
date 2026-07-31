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
                        page.Size(PageSizes.A4);
                        page.Margin(40);
                        page.DefaultTextStyle(x => x.FontSize(11));

                        // ── ENCABEZADO ──
                        page.Header().Column(col =>
                        {
                            col.Item().Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("🌿 EcoWash Direct").FontSize(24).Bold().FontColor("#2d6a4f");
                                    c.Item().Text("Lavado Ecológico a Domicilio").FontSize(10).FontColor("#888888");
                                    c.Item().Text("Santa Cruz de la Sierra, Bolivia").FontSize(9).FontColor("#aaaaaa");
                                });
                                row.ConstantItem(120).AlignRight().Column(c =>
                                {
                                    c.Item().Text("COMPROBANTE").FontSize(14).Bold().FontColor("#2d6a4f");
                                    c.Item().Text("DE PAGO").FontSize(14).Bold().FontColor("#2d6a4f");
                                });
                            });
                            col.Item().PaddingVertical(10).LineHorizontal(2).LineColor("#2d6a4f");
                        });

                        // ── CONTENIDO ──
                        page.Content().Column(col =>
                        {
                            // Información de la orden
                            col.Item().PaddingTop(10).Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text($"Orden: {receipt.NumeroOrden}").Bold().FontSize(12);
                                    c.Item().Text($"Transacción: {receipt.NumeroTransaccion}").FontSize(10).FontColor("#555555");
                                });
                                row.RelativeItem().AlignRight().Column(c =>
                                {
                                    c.Item().Text($"Fecha: {receipt.FechaPago:dd/MM/yyyy}").FontSize(10);
                                    c.Item().Text($"Hora: {receipt.FechaPago:HH:mm:ss} UTC").FontSize(10);
                                });
                            });

                            // Datos del cliente
                            col.Item().PaddingTop(15).Background("#f8f9fa").Padding(12).Column(c =>
                            {
                                c.Item().Text("DATOS DEL CLIENTE").Bold().FontSize(10).FontColor("#2d6a4f");
                                c.Item().PaddingTop(5).Text($"Nombre: {receipt.NombreCliente}").FontSize(10);
                                c.Item().Text($"Email: {receipt.EmailCliente}").FontSize(10);
                            });

                            // Tabla de servicios
                            col.Item().PaddingTop(15).Text("SERVICIOS CONTRATADOS").Bold().FontSize(10).FontColor("#2d6a4f");
                            col.Item().PaddingTop(5).Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(4); // Servicio
                                    columns.RelativeColumn(1); // Cantidad
                                    columns.RelativeColumn(2); // Precio Unit.
                                    columns.RelativeColumn(2); // Subtotal
                                });

                                // Header
                                table.Header(header =>
                                {
                                    header.Cell().Background("#2d6a4f").Padding(6).Text("Servicio").FontColor("#ffffff").Bold().FontSize(10);
                                    header.Cell().Background("#2d6a4f").Padding(6).AlignCenter().Text("Cant.").FontColor("#ffffff").Bold().FontSize(10);
                                    header.Cell().Background("#2d6a4f").Padding(6).AlignRight().Text("P. Unit.").FontColor("#ffffff").Bold().FontSize(10);
                                    header.Cell().Background("#2d6a4f").Padding(6).AlignRight().Text("Subtotal").FontColor("#ffffff").Bold().FontSize(10);
                                });

                                // Filas
                                bool alt = false;
                                foreach (var item in receipt.Servicios)
                                {
                                    var bg = alt ? "#f8f9fa" : "#ffffff";
                                    table.Cell().Background(bg).Padding(6).Text(item.NombreServicio).FontSize(10);
                                    table.Cell().Background(bg).Padding(6).AlignCenter().Text(item.Cantidad.ToString()).FontSize(10);
                                    table.Cell().Background(bg).Padding(6).AlignRight().Text($"Bs. {item.PrecioUnitario:N2}").FontSize(10);
                                    table.Cell().Background(bg).Padding(6).AlignRight().Text($"Bs. {item.Subtotal:N2}").FontSize(10);
                                    alt = !alt;
                                }
                            });

                            // Total
                            col.Item().PaddingTop(10).AlignRight().Row(row =>
                            {
                                row.RelativeItem();
                                row.ConstantItem(200).Background("#e8f5e9").Padding(12).Column(c =>
                                {
                                    c.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text("TOTAL PAGADO:").Bold().FontSize(13).FontColor("#2d6a4f");
                                        r.RelativeItem().AlignRight().Text($"Bs. {receipt.TotalPagado:N2}").Bold().FontSize(13).FontColor("#2d6a4f");
                                    });
                                });
                            });

                            // Info de pago
                            col.Item().PaddingTop(15).Row(row =>
                            {
                                row.RelativeItem().Column(c =>
                                {
                                    c.Item().Text("INFORMACIÓN DE PAGO").Bold().FontSize(10).FontColor("#2d6a4f");
                                    c.Item().PaddingTop(5).Text($"Método: {receipt.MetodoPago}").FontSize(10);
                                    c.Item().Text($"Estado: {receipt.EstadoPago}").FontSize(10);
                                });

                                // QR Code
                                if (!string.IsNullOrEmpty(receipt.QrContent))
                                {
                                    row.ConstantItem(100).AlignRight().Column(c =>
                                    {
                                        var qrBytes = GenerateQrCode(receipt.QrContent);
                                        if (qrBytes != null)
                                        {
                                            c.Item().Image(qrBytes).FitWidth();
                                            c.Item().AlignCenter().Text("Escanear para consultar").FontSize(7).FontColor("#888888");
                                        }
                                    });
                                }
                            });

                            // Nota final
                            col.Item().PaddingTop(20).Background("#fff3cd").Padding(10).Text(
                                "Este comprobante es un documento válido de su transacción con EcoWash Direct. " +
                                "Conserve este documento para cualquier reclamo o consulta."
                            ).FontSize(8).FontColor("#856404");
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
