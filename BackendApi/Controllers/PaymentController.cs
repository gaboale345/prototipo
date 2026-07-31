using BackendApi.Data;
using BackendApi.DTOs;
using BackendApi.Helpers;
using BackendApi.Models;
using BackendApi.Services.Email;
using BackendApi.Services.Payments;
using BackendApi.Services.Pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BackendApi.Controllers
{
    /// <summary>
    /// Controlador de pagos con pasarela integrada (Stripe).
    /// - Crea sesiones de pago con montos calculados desde el backend.
    /// - Recibe webhooks para actualizar estados automáticamente.
    /// - Genera comprobantes PDF y envía correos de confirmación.
    /// - Nunca confía en precios enviados desde el frontend.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly EcoWashDbContext _context;
        private readonly IPaymentGateway _paymentGateway;
        private readonly IEmailService _emailService;
        private readonly IPdfService _pdfService;
        private readonly AuditoriaHelper _auditoria;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(
            EcoWashDbContext context,
            IPaymentGateway paymentGateway,
            IEmailService emailService,
            IPdfService pdfService,
            AuditoriaHelper auditoria,
            ILogger<PaymentController> logger)
        {
            _context = context;
            _paymentGateway = paymentGateway;
            _emailService = emailService;
            _pdfService = pdfService;
            _auditoria = auditoria;
            _logger = logger;
        }

        /// <summary>
        /// Procesa un pago simulado/ficticio sin realizar cobros reales ni contactar pasarelas externas.
        /// Valida los datos del formulario (tarjeta o QR), genera un ID de transacción ficticio,
        /// actualiza los estados en base de datos como Pagado, genera el comprobante PDF y envía el correo.
        /// </summary>
        [HttpPost("process-simulated")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<SimulatedPaymentResponseDto>>> ProcessSimulatedPayment([FromBody] ProcessSimulatedPaymentDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            ServiceOrder? order = null;

            if (dto.OrderId > 0)
            {
                order = await _context.ServiceOrders
                    .Include(o => o.Detalles)
                    .Include(o => o.Cliente).ThenInclude(c => c.Usuario)
                    .FirstOrDefaultAsync(o => o.Id == dto.OrderId);
            }

            // Si vino desde una Reserva directamente sin ServiceOrder previa
            if (order == null && dto.ReservaId.HasValue && dto.ReservaId.Value > 0)
            {
                var reserva = await _context.Reservas
                    .Include(r => r.Servicio)
                    .Include(r => r.Cliente).ThenInclude(c => c.Usuario)
                    .FirstOrDefaultAsync(r => r.Id == dto.ReservaId.Value);

                if (reserva != null)
                {
                    // Buscar o crear la ServiceOrder para la reserva
                    order = await _context.ServiceOrders
                        .Include(o => o.Detalles)
                        .Include(o => o.Cliente).ThenInclude(c => c.Usuario)
                        .FirstOrDefaultAsync(o => o.Observaciones == $"Reserva#{reserva.Id}");

                    if (order == null)
                    {
                        order = new ServiceOrder
                        {
                            ClienteId = reserva.ClienteId,
                            NumeroOrden = $"ORD-RES-{reserva.Id}-{DateTime.UtcNow.Ticks.ToString()[^4..]}",
                            Subtotal = reserva.PrecioTotal,
                            Total = reserva.PrecioTotal,
                            Estado = "Pendiente",
                            Observaciones = $"Reserva#{reserva.Id}",
                            FechaCreacion = DateTime.UtcNow,
                            Detalles = new List<ServiceOrderDetail>
                            {
                                new ServiceOrderDetail
                                {
                                    ServicioId = reserva.ServicioId,
                                    NombreServicio = reserva.Servicio.Nombre,
                                    Cantidad = 1,
                                    PrecioUnitario = reserva.Servicio.Precio,
                                    Subtotal = reserva.PrecioTotal
                                }
                            }
                        };
                        _context.ServiceOrders.Add(order);
                        await _context.SaveChangesAsync();

                        // Re-cargar con inclusiones
                        order = await _context.ServiceOrders
                            .Include(o => o.Detalles)
                            .Include(o => o.Cliente).ThenInclude(c => c.Usuario)
                            .FirstOrDefaultAsync(o => o.Id == order.Id);
                    }
                }
            }

            if (order == null)
                return NotFound(ApiResponse<SimulatedPaymentResponseDto>.Fail("Orden de servicio no encontrada."));

            if (order.Cliente.UsuarioId != userId)
                return Forbid();

            if (order.Estado == "Pagada")
                return BadRequest(ApiResponse<SimulatedPaymentResponseDto>.Fail("Esta orden ya ha sido pagada previamente."));

            if (order.Estado == "Cancelada")
                return BadRequest(ApiResponse<SimulatedPaymentResponseDto>.Fail("Esta orden fue cancelada y no se puede pagar."));

            var fechaPago = DateTime.UtcNow;
            var transactionId = $"TX-SIM-{Guid.NewGuid().ToString("N")[..10].ToUpper()}";
            var metodoPagoNombre = dto.MetodoPago.ToLower() == "qr" ? "QR Bancario (Simulado)" : "Tarjeta Débito/Crédito (Simulada)";

            // 1. Crear la transacción de pago ficticia
            var transaction = new PaymentTransaction
            {
                ServiceOrderId = order.Id,
                UsuarioId = userId,
                TransactionId = transactionId,
                Estado = "Pagado",
                Monto = order.Total,
                Moneda = "BOB",
                MetodoPago = metodoPagoNombre,
                ReferenciaPasarela = $"REF-DEMO-{DateTime.UtcNow:yyyyMMddHHmmss}",
                ProveedorPago = "Pasarela Simulación EcoWash",
                FechaCreacion = fechaPago,
                FechaPago = fechaPago,
                RespuestaCompletaApi = $"{{\"simulado\": true, \"metodo\": \"{dto.MetodoPago}\", \"titular\": \"{dto.TitularTarjeta ?? "Titular Demo"}\"}}"
            };

            _context.PaymentTransactions.Add(transaction);

            // 2. Actualizar estado de la orden
            order.Estado = "Pagada";
            order.FechaPago = fechaPago;

            // 3. Si viene vinculada a una Reserva, actualizar Venta y Reserva
            if (!string.IsNullOrEmpty(order.Observaciones) && order.Observaciones.StartsWith("Reserva#"))
            {
                var reservaIdStr = order.Observaciones.Replace("Reserva#", "");
                if (int.TryParse(reservaIdStr, out var resId))
                {
                    var venta = await _context.Ventas.FirstOrDefaultAsync(v => v.ReservaId == resId);
                    if (venta != null)
                    {
                        venta.Estado = "Pagada";
                        var pagoExistente = await _context.Pagos.AnyAsync(p => p.ReservaId == resId);
                        if (!pagoExistente)
                        {
                            _context.Pagos.Add(new Pago
                            {
                                VentaId = venta.Id,
                                ReservaId = resId,
                                MetodoPagoId = dto.MetodoPago.ToLower() == "qr" ? 2 : 4,
                                Monto = order.Total,
                                Estado = "Completado",
                                Referencia = transactionId,
                                FechaPago = fechaPago
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();

            // 4. Generar recibo PDF oficial
            string? pdfUrl = null;
            try
            {
                var receiptData = new ReceiptData
                {
                    NumeroOrden = order.NumeroOrden,
                    NumeroTransaccion = transactionId,
                    NombreCliente = $"{order.Cliente.Usuario.Nombre} {order.Cliente.Usuario.Apellido}",
                    EmailCliente = order.Cliente.Usuario.Email,
                    FechaPago = fechaPago,
                    TotalPagado = order.Total,
                    MetodoPago = metodoPagoNombre,
                    EstadoPago = "Pagado",
                    QrContent = $"EcoWash-Orden-{order.NumeroOrden}-Tx-{transactionId}",
                    Servicios = order.Detalles.Select(d => new ReceiptItem
                    {
                        NombreServicio = d.NombreServicio,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario,
                        Subtotal = d.Subtotal
                    }).ToList()
                };

                var pdfBytes = await _pdfService.GeneratePaymentReceiptAsync(receiptData);
                var receiptsDir = Path.Combine(Directory.GetCurrentDirectory(), "receipts");
                if (!Directory.Exists(receiptsDir)) Directory.CreateDirectory(receiptsDir);
                var pdfPath = Path.Combine(receiptsDir, $"recibo_{transaction.Id}.pdf");
                await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);

                transaction.ComprobantePdfUrl = $"/api/Receipt/{transaction.Id}/download";
                pdfUrl = transaction.ComprobantePdfUrl;
                await _context.SaveChangesAsync();

                // Enviar confirmación por correo (asíncrono)
                await _emailService.SendPaymentConfirmationAsync(
                    order.Cliente.Usuario.Email,
                    order.Cliente.Usuario.Nombre,
                    order.NumeroOrden,
                    order.Total,
                    metodoPagoNombre
                );

                await _emailService.SendPaymentReceiptAsync(
                    order.Cliente.Usuario.Email,
                    order.Cliente.Usuario.Nombre,
                    order.NumeroOrden,
                    pdfBytes
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar comprobante PDF en pago simulado para la orden {OrderId}", order.Id);
            }

            // 5. Registrar notificación en sistema
            _context.Notificaciones.Add(new Notificacion
            {
                UsuarioId = userId,
                Titulo = "✓ Pago Simulado Exitoso",
                Mensaje = $"Tu pago de Bs. {order.Total:N2} para la orden {order.NumeroOrden} fue procesado correctamente.",
                Tipo = "Exito",
                Fecha = fechaPago
            });
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("PagoSimuladoCompletado", "Pagos", "PaymentTransaction",
                transaction.Id, null, new { order.NumeroOrden, order.Total, Metodo = dto.MetodoPago }, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString());

            return Ok(ApiResponse<SimulatedPaymentResponseDto>.Ok(new SimulatedPaymentResponseDto
            {
                Success = true,
                TransactionId = transactionId,
                OrderId = order.Id,
                NumeroOrden = order.NumeroOrden,
                MontoTotal = order.Total,
                MetodoPago = metodoPagoNombre,
                FechaPago = fechaPago,
                Estado = "Pagado",
                ComprobantePdfUrl = pdfUrl,
                Mensaje = "✓ Pago realizado correctamente (Modo Simulación)"
            }, "Pago simulado completado exitosamente."));
        }

        /// <summary>
        /// Crea una sesión de pago en la pasarela para una orden existente.
        /// El monto se toma de la orden (calculado previamente desde el backend).
        /// </summary>
        [HttpPost("create-session")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaymentSessionResponseDto>>> CreatePaymentSession([FromBody] CreatePaymentSessionDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var order = await _context.ServiceOrders
                .Include(o => o.Detalles)
                .Include(o => o.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(o => o.Id == dto.OrderId);

            if (order == null)
                return NotFound(ApiResponse<PaymentSessionResponseDto>.Fail("Orden no encontrada."));

            // Solo el dueño puede pagar su orden
            if (order.Cliente.UsuarioId != userId)
                return Forbid();

            if (order.Estado == "Pagada")
                return BadRequest(ApiResponse<PaymentSessionResponseDto>.Fail("Esta orden ya fue pagada."));

            if (order.Estado == "Cancelada")
                return BadRequest(ApiResponse<PaymentSessionResponseDto>.Fail("Esta orden fue cancelada."));

            // SEGURIDAD: El monto viene de la BD, NO del frontend
            var baseUrl = Environment.GetEnvironmentVariable("APP_BASE_URL") ?? "http://localhost:5173";
            var lineItems = order.Detalles.Select(d => new PaymentLineItem
            {
                Name = d.NombreServicio,
                Description = $"Servicio EcoWash Direct",
                UnitPrice = d.PrecioUnitario,
                Quantity = d.Cantidad
            }).ToList();

            var currency = Environment.GetEnvironmentVariable("PAYMENT_CURRENCY") ?? "bob";

            var result = await _paymentGateway.CreatePaymentSessionAsync(
                order.Id,
                order.Total,
                currency,
                $"Pago Orden {order.NumeroOrden}",
                order.Cliente.Usuario.Email,
                lineItems,
                $"{baseUrl}/cliente/pago/exito",
                $"{baseUrl}/cliente/pago/cancelado"
            );

            if (!result.Success)
            {
                await _auditoria.RegistrarAsync("PagoFallido_CrearSesion", "Pagos", "ServiceOrder", order.Id,
                    null, new { Error = result.ErrorMessage }, userId,
                    HttpContext.Connection.RemoteIpAddress?.ToString());

                return BadRequest(ApiResponse<PaymentSessionResponseDto>.Fail(result.ErrorMessage ?? "Error al crear sesión de pago."));
            }

            // Crear registro de transacción en estado Pendiente
            var transaction = new PaymentTransaction
            {
                ServiceOrderId = order.Id,
                UsuarioId = userId,
                Estado = "Pendiente",
                Monto = order.Total,
                Moneda = currency.ToUpper(),
                MetodoPago = dto.MetodoPago,
                StripeSessionId = result.SessionId,
                StripePaymentIntentId = result.PaymentIntentId,
                ProveedorPago = _paymentGateway.ProviderName,
                FechaCreacion = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            await _auditoria.RegistrarAsync("CrearSesionPago", "Pagos", "PaymentTransaction", transaction.Id,
                null, new { order.NumeroOrden, order.Total, SessionId = result.SessionId }, userId,
                HttpContext.Connection.RemoteIpAddress?.ToString());

            return Ok(ApiResponse<PaymentSessionResponseDto>.Ok(new PaymentSessionResponseDto
            {
                SessionId = result.SessionId!,
                PaymentUrl = result.PaymentUrl!,
                OrderId = order.Id
            }, "Sesión de pago creada. Redirigir al usuario a la URL de pago."));
        }

        /// <summary>
        /// Crea una sesión de pago en la pasarela Stripe para una Reserva existente.
        /// El monto se toma de la reserva (calculado desde el backend).
        /// </summary>
        [HttpPost("create-session-reserva/{reservaId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaymentSessionResponseDto>>> CreateReservaPaymentSession(int reservaId, [FromQuery] string metodoPago = "card")
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var reserva = await _context.Reservas
                .Include(r => r.Servicio)
                .Include(r => r.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(r => r.Id == reservaId);

            if (reserva == null)
                return NotFound(ApiResponse<PaymentSessionResponseDto>.Fail("Reserva no encontrada."));

            if (reserva.Cliente.UsuarioId != userId)
                return Forbid();

            // REGLA DE NEGOCIO: La reserva no debe estar cancelada ni rechazada
            if (reserva.Estado == "Cancelada" || reserva.Estado == "Rechazada")
                return BadRequest(ApiResponse<PaymentSessionResponseDto>.Fail("No se puede pagar una reserva cancelada o rechazada."));

            // Verificar o crear Venta para la reserva
            var venta = await _context.Ventas.FirstOrDefaultAsync(v => v.ReservaId == reservaId);
            if (venta == null)
            {
                venta = new Venta
                {
                    ReservaId = reserva.Id,
                    ClienteId = reserva.ClienteId,
                    NumeroVenta = $"VTA-{DateTime.UtcNow:yyyyMMdd}-{reserva.Id}",
                    FechaVenta = DateTime.UtcNow,
                    Subtotal = reserva.PrecioTotal,
                    Descuento = 0,
                    Total = reserva.PrecioTotal,
                    Estado = "Pendiente"
                };
                _context.Ventas.Add(venta);
                await _context.SaveChangesAsync();
            }

            // Buscar o crear ServiceOrder asociada a la reserva
            var order = await _context.ServiceOrders.FirstOrDefaultAsync(o => o.Observaciones == $"Reserva#{reservaId}");
            if (order == null)
            {
                order = new ServiceOrder
                {
                    ClienteId = reserva.ClienteId,
                    NumeroOrden = $"ORD-RES-{reservaId}-{DateTime.UtcNow.Ticks.ToString()[^4..]}",
                    Subtotal = reserva.PrecioTotal,
                    Total = reserva.PrecioTotal,
                    Estado = "Pendiente",
                    Observaciones = $"Reserva#{reservaId}",
                    FechaCreacion = DateTime.UtcNow,
                    Detalles = new List<ServiceOrderDetail>
                    {
                        new ServiceOrderDetail
                        {
                            ServicioId = reserva.ServicioId,
                            NombreServicio = reserva.Servicio.Nombre,
                            Cantidad = 1,
                            PrecioUnitario = reserva.Servicio.Precio,
                            Subtotal = reserva.PrecioTotal
                        }
                    }
                };
                _context.ServiceOrders.Add(order);
                await _context.SaveChangesAsync();
            }

            var baseUrl = Environment.GetEnvironmentVariable("APP_BASE_URL") ?? "http://localhost:5173";
            var currency = Environment.GetEnvironmentVariable("PAYMENT_CURRENCY") ?? "bob";

            var lineItems = new List<PaymentLineItem>
            {
                new PaymentLineItem
                {
                    Name = $"Reserva #{reserva.Id} - {reserva.Servicio.Nombre}",
                    Description = $"Servicio de lavado para vehículo {reserva.VehiculoId}",
                    UnitPrice = reserva.PrecioTotal,
                    Quantity = 1
                }
            };

            var result = await _paymentGateway.CreatePaymentSessionAsync(
                order.Id,
                order.Total,
                currency,
                $"Pago Reserva #{reserva.Id}",
                reserva.Cliente.Usuario.Email,
                lineItems,
                $"{baseUrl}/cliente/pago/exito",
                $"{baseUrl}/cliente/pago/cancelado"
            );

            if (!result.Success)
            {
                return BadRequest(ApiResponse<PaymentSessionResponseDto>.Fail(result.ErrorMessage ?? "Error al crear sesión de pago."));
            }

            var transaction = new PaymentTransaction
            {
                ServiceOrderId = order.Id,
                UsuarioId = userId,
                Estado = "Pendiente",
                Monto = order.Total,
                Moneda = currency.ToUpper(),
                MetodoPago = metodoPago,
                StripeSessionId = result.SessionId,
                StripePaymentIntentId = result.PaymentIntentId,
                ProveedorPago = _paymentGateway.ProviderName,
                FechaCreacion = DateTime.UtcNow
            };

            _context.PaymentTransactions.Add(transaction);
            await _context.SaveChangesAsync();

            return Ok(ApiResponse<PaymentSessionResponseDto>.Ok(new PaymentSessionResponseDto
            {
                SessionId = result.SessionId!,
                PaymentUrl = result.PaymentUrl!,
                OrderId = order.Id
            }, "Sesión de pago Stripe creada."));
        }

        /// <summary>
        /// Confirma el pago de una sesión (usado por el cliente tras retornar de la pasarela o en modo Sandbox).
        /// Marca la transacción como Pagada, genera el PDF y envía el correo.
        /// </summary>
        [HttpPost("confirm-session/{sessionId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> ConfirmSession(string sessionId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

            var transaction = await _context.PaymentTransactions
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Detalles)
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Cliente).ThenInclude(c => c.Usuario)
                .FirstOrDefaultAsync(t => t.StripeSessionId == sessionId || t.ServiceOrderId.ToString() == sessionId);

            if (transaction == null)
                return NotFound(ApiResponse<string>.Fail("Transacción de pago no encontrada."));

            if (transaction.UsuarioId != userId)
                return Forbid();

            if (transaction.Estado != "Pagado")
            {
                transaction.Estado = "Pagado";
                transaction.FechaPago = DateTime.UtcNow;
                transaction.TransactionId = transaction.TransactionId ?? $"TX-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
                transaction.ServiceOrder.Estado = "Pagada";
                transaction.ServiceOrder.FechaPago = DateTime.UtcNow;

                // Si la orden está vinculada a una reserva, actualizar también la Venta y Reserva
                if (transaction.ServiceOrder.Observaciones != null && transaction.ServiceOrder.Observaciones.StartsWith("Reserva#"))
                {
                    var reservaIdStr = transaction.ServiceOrder.Observaciones.Replace("Reserva#", "");
                    if (int.TryParse(reservaIdStr, out var reservaId))
                    {
                        var venta = await _context.Ventas.FirstOrDefaultAsync(v => v.ReservaId == reservaId);
                        if (venta != null)
                        {
                            venta.Estado = "Pagada";
                            // Crear pago tradicional
                            var pagoExistente = await _context.Pagos.AnyAsync(p => p.ReservaId == reservaId);
                            if (!pagoExistente)
                            {
                                var pago = new Pago
                                {
                                    VentaId = venta.Id,
                                    ReservaId = reservaId,
                                    MetodoPagoId = 4, // Tarjeta / Pasarela
                                    Monto = transaction.Monto,
                                    Estado = "Completado",
                                    Referencia = transaction.TransactionId,
                                    FechaPago = DateTime.UtcNow
                                };
                                _context.Pagos.Add(pago);
                            }
                        }
                    }
                }

                await _context.SaveChangesAsync();

                // Generar PDF
                try
                {
                    var receiptData = new ReceiptData
                    {
                        NumeroOrden = transaction.ServiceOrder.NumeroOrden,
                        NumeroTransaccion = transaction.TransactionId ?? $"TX-{transaction.Id}",
                        NombreCliente = $"{transaction.ServiceOrder.Cliente.Usuario.Nombre} {transaction.ServiceOrder.Cliente.Usuario.Apellido}",
                        EmailCliente = transaction.ServiceOrder.Cliente.Usuario.Email,
                        FechaPago = transaction.FechaPago ?? DateTime.UtcNow,
                        TotalPagado = transaction.Monto,
                        MetodoPago = transaction.MetodoPago ?? "Tarjeta",
                        EstadoPago = transaction.Estado ?? "Pagado",
                        QrContent = $"EcoWash-Orden-{transaction.ServiceOrder.NumeroOrden}-Tx-{transaction.TransactionId}",
                        Servicios = transaction.ServiceOrder.Detalles.Select(d => new ReceiptItem
                        {
                            NombreServicio = d.NombreServicio,
                            Cantidad = d.Cantidad,
                            PrecioUnitario = d.PrecioUnitario,
                            Subtotal = d.Subtotal
                        }).ToList()
                    };

                    var pdfBytes = await _pdfService.GeneratePaymentReceiptAsync(receiptData);
                    var receiptsDir = Path.Combine(Directory.GetCurrentDirectory(), "receipts");
                    if (!Directory.Exists(receiptsDir)) Directory.CreateDirectory(receiptsDir);
                    var pdfPath = Path.Combine(receiptsDir, $"recibo_{transaction.Id}.pdf");
                    await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);
                    transaction.ComprobantePdfUrl = $"/api/Receipt/{transaction.Id}/download";
                    await _context.SaveChangesAsync();

                    await _emailService.SendPaymentReceiptAsync(
                        transaction.ServiceOrder.Cliente.Usuario.Email,
                        transaction.ServiceOrder.Cliente.Usuario.Nombre,
                        transaction.ServiceOrder.NumeroOrden,
                        pdfBytes
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al generar/enviar recibo PDF para transacción #{Id}", transaction.Id);
                }

                await _auditoria.RegistrarAsync("ConfirmarPagoSession", "Pagos", "PaymentTransaction", transaction.Id, null, new { transaction.StripeSessionId }, userId);
            }

            return Ok(ApiResponse<string>.Ok("Pago verificado y completado exitosamente."));
        }

        /// <summary>
        /// Webhook endpoint para recibir notificaciones de la pasarela de pagos.
        /// Actualiza automáticamente el estado del pago cuando la pasarela confirma.
        /// Valida la firma del webhook para prevenir falsificación.
        /// </summary>
        [HttpPost("webhook")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleWebhook()
        {
            string payload;
            using (var reader = new StreamReader(HttpContext.Request.Body))
            {
                payload = await reader.ReadToEndAsync();
            }

            var signature = HttpContext.Request.Headers["Stripe-Signature"].FirstOrDefault() ?? "";

            // SEGURIDAD: Verificar firma del webhook
            var webhookResult = await _paymentGateway.VerifyWebhookAsync(payload, signature);
            if (webhookResult == null)
            {
                _logger.LogWarning("Webhook con firma inválida rechazado. IP={IP}",
                    HttpContext.Connection.RemoteIpAddress?.ToString());
                return BadRequest("Firma de webhook inválida.");
            }

            _logger.LogInformation("Webhook recibido: Type={Type}, SessionId={SessionId}",
                webhookResult.EventType, webhookResult.SessionId);

            // Procesar evento de pago completado
            if (webhookResult.EventType == "checkout.session.completed")
            {
                var transaction = await _context.PaymentTransactions
                    .Include(t => t.ServiceOrder).ThenInclude(o => o.Detalles)
                    .Include(t => t.ServiceOrder).ThenInclude(o => o.Cliente).ThenInclude(c => c.Usuario)
                    .FirstOrDefaultAsync(t => t.StripeSessionId == webhookResult.SessionId);

                if (transaction != null && transaction.Estado != "Pagado")
                {
                    // Actualizar transacción
                    transaction.Estado = "Pagado";
                    transaction.FechaPago = DateTime.UtcNow;
                    transaction.TransactionId = webhookResult.PaymentIntentId;
                    transaction.ReferenciaPasarela = webhookResult.SessionId;
                    transaction.RespuestaCompletaApi = webhookResult.RawJson;
                    transaction.MetodoPago = webhookResult.PaymentMethodType ?? transaction.MetodoPago;

                    // Actualizar orden
                    transaction.ServiceOrder.Estado = "Pagada";
                    transaction.ServiceOrder.FechaPago = DateTime.UtcNow;

                    await _context.SaveChangesAsync();

                    // Generar comprobante PDF
                    try
                    {
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

                        var pdfBytes = await _pdfService.GeneratePaymentReceiptAsync(receiptData);

                        // Guardar PDF en disco
                        var pdfDir = Path.Combine(Directory.GetCurrentDirectory(), "receipts");
                        Directory.CreateDirectory(pdfDir);
                        var pdfPath = Path.Combine(pdfDir, $"receipt_{transaction.ServiceOrder.NumeroOrden}.pdf");
                        await System.IO.File.WriteAllBytesAsync(pdfPath, pdfBytes);
                        transaction.ComprobantePdfUrl = $"/api/Receipt/{transaction.Id}/download";
                        await _context.SaveChangesAsync();

                        // Enviar correo de confirmación
                        var clientEmail = transaction.ServiceOrder.Cliente.Usuario.Email;
                        var clientName = transaction.ServiceOrder.Cliente.Usuario.Nombre;
                        await _emailService.SendPaymentConfirmationAsync(
                            clientEmail, clientName,
                            transaction.ServiceOrder.NumeroOrden,
                            transaction.Monto,
                            transaction.MetodoPago ?? "Tarjeta");

                        // Enviar comprobante PDF adjunto
                        await _emailService.SendPaymentReceiptAsync(
                            clientEmail, clientName,
                            transaction.ServiceOrder.NumeroOrden, pdfBytes);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error al generar/enviar comprobante para transacción {TransactionId}", transaction.Id);
                    }

                    // Notificación en el sistema
                    _context.Notificaciones.Add(new Notificacion
                    {
                        UsuarioId = transaction.ServiceOrder.Cliente.UsuarioId,
                        Titulo = "Pago Confirmado",
                        Mensaje = $"Tu pago de Bs. {transaction.Monto:N2} para la orden {transaction.ServiceOrder.NumeroOrden} ha sido confirmado.",
                        Tipo = "Exito",
                        Fecha = DateTime.UtcNow
                    });
                    await _context.SaveChangesAsync();

                    await _auditoria.RegistrarAsync("PagoConfirmado_Webhook", "Pagos", "PaymentTransaction",
                        transaction.Id, null, new { transaction.TransactionId, transaction.Monto, transaction.MetodoPago },
                        transaction.UsuarioId);

                    _logger.LogInformation("Pago confirmado via webhook: Order={OrderNumber}, Amount={Amount}",
                        transaction.ServiceOrder.NumeroOrden, transaction.Monto);
                }
            }
            else if (webhookResult.EventType == "checkout.session.expired" ||
                     webhookResult.EventType == "payment_intent.payment_failed")
            {
                var transaction = await _context.PaymentTransactions
                    .Include(t => t.ServiceOrder)
                    .FirstOrDefaultAsync(t => t.StripeSessionId == webhookResult.SessionId);

                if (transaction != null && transaction.Estado == "Pendiente")
                {
                    var newStatus = webhookResult.EventType == "checkout.session.expired" ? "Cancelado" : "Fallido";
                    transaction.Estado = newStatus;
                    transaction.RespuestaCompletaApi = webhookResult.RawJson;
                    await _context.SaveChangesAsync();

                    await _auditoria.RegistrarAsync($"Pago{newStatus}_Webhook", "Pagos", "PaymentTransaction",
                        transaction.Id, null, new { webhookResult.EventType }, transaction.UsuarioId);
                }
            }

            return Ok();
        }

        /// <summary>
        /// Consulta el estado actual del pago de una orden (polling desde frontend).
        /// </summary>
        [HttpGet("status/{orderId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaymentStatusDto>>> GetPaymentStatus(int orderId)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            var order = await _context.ServiceOrders
                .Include(o => o.Cliente)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                return NotFound(ApiResponse<PaymentStatusDto>.Fail("Orden no encontrada."));

            if (role == "Cliente" && order.Cliente.UsuarioId != userId)
                return Forbid();

            var transaction = await _context.PaymentTransactions
                .Where(t => t.ServiceOrderId == orderId)
                .OrderByDescending(t => t.FechaCreacion)
                .FirstOrDefaultAsync();

            return Ok(ApiResponse<PaymentStatusDto>.Ok(new PaymentStatusDto
            {
                OrderId = order.Id,
                NumeroOrden = order.NumeroOrden,
                Estado = transaction?.Estado ?? order.Estado,
                Monto = order.Total,
                MetodoPago = transaction?.MetodoPago,
                FechaPago = transaction?.FechaPago
            }));
        }

        /// <summary>
        /// Historial de pagos del cliente autenticado con filtros.
        /// </summary>
        [HttpGet("history")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<List<PaymentTransactionDto>>>> GetPaymentHistory(
            [FromQuery] string? estado = null,
            [FromQuery] DateTime? fechaInicio = null,
            [FromQuery] DateTime? fechaFin = null)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var role = User.FindFirstValue(ClaimTypes.Role);

            IQueryable<PaymentTransaction> query = _context.PaymentTransactions
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Detalles)
                .Include(t => t.ServiceOrder).ThenInclude(o => o.Cliente).ThenInclude(c => c.Usuario);

            if (role == "Cliente")
                query = query.Where(t => t.UsuarioId == userId);

            if (!string.IsNullOrEmpty(estado))
                query = query.Where(t => t.Estado == estado);
            if (fechaInicio.HasValue)
                query = query.Where(t => t.FechaCreacion >= fechaInicio.Value);
            if (fechaFin.HasValue)
                query = query.Where(t => t.FechaCreacion <= fechaFin.Value);

            var transactions = await query.OrderByDescending(t => t.FechaCreacion).ToListAsync();
            var result = transactions.Select(MapToDto).ToList();

            return Ok(ApiResponse<List<PaymentTransactionDto>>.Ok(result));
        }

        private static PaymentTransactionDto MapToDto(PaymentTransaction t)
        {
            return new PaymentTransactionDto
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
            };
        }
    }
}
