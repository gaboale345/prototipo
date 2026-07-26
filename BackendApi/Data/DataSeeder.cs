using BackendApi.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BackendApi.Data
{
    public static class DataSeeder
    {
        public static async Task SeedClientesAsync(EcoWashDbContext context)
        {
            // Solo sembrar si hay menos de 100 clientes
            var clienteCount = await context.Clientes.CountAsync();
            if (clienteCount >= 100)
            {
                return;
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword("Cliente@1234");
            var rand = new Random(42); // Semilla fija para reproducibilidad

            var nombres = new[]
            {
                "Carlos", "Ana", "Juan", "María", "Luis", "Sofía", "Fernando", "Patricia", "Diego", "Gabriela",
                "Mateo", "Valeria", "Alejandro", "Natalia", "Lucas", "Camila", "Sebastián", "Isabella", "Daniel", "Jorge",
                "Ricardo", "Claudia", "Gonzalo", "Andrea", "Roberto", "Paola", "Hugo", "Elena", "Óscar", "Mónica",
                "Marcos", "Lourdes", "Ramiro", "Fabiola", "Esteban", "Lucía", "Adrián", "Renata", "Victor", "Silvia",
                "Javier", "Teresa", "Mauricio", "Karla", "Gabriel", "Ximena", "Erick", "Pamela", "Julio", "Vanesa"
            };

            var apellidos = new[]
            {
                "Mendoza", "Vargas", "Flores", "Suárez", "Aguilera", "Rojas", "Chávez", "Guzmán", "Banzer", "Roca",
                "Gutiérrez", "Justiniano", "Mercado", "Cuéllar", "Saucedo", "Hurtado", "Rivera", "Vaca", "Dorado", "Soliz",
                "Villagómez", "Paz", "Torrico", "Peña", "Cardozo", "Montero", "Zabala", "Baldivieso", "Campero", "Clarés",
                "Siles", "Cabrera", "Montaño", "Romero", "Pinto", "Salatiel", "Ramos", "Espinoza", "Alvarez", "Miranda"
            };

            var zonas = new[]
            {
                "Equipetrol", "Urbari", "Las Palmas", "Av. Banzer 4to Anillo", "Sirari", "Hamacas", "Barrio Norte",
                "Plan 3000", "Villa 1ro de Mayo", "Doble Vía a La Guardia", "Av. Busch 3er Anillo",
                "Av. Cristóbal de Mendoza", "Radial 26", "Radial 19", "Av. Roca y Coronado", "Barrio Sur", "Centro Histórico"
            };

            var marcasModelos = new Dictionary<string, string[]>
            {
                { "Toyota", new[] { "Corolla", "Hilux", "RAV4", "Land Cruiser", "Yaris", "Prado" } },
                { "Suzuki", new[] { "Grand Vitara", "Jimny", "Swift", "Carry", "S-Cross" } },
                { "Nissan", new[] { "Frontier", "Kicks", "Sentra", "X-Trail", "Versa" } },
                { "Hyundai", new[] { "Tucson", "Creta", "Accent", "Santa Fe", "Elantra" } },
                { "Kia", new[] { "Sportage", "Sorento", "Rio", "Picanto", "Seltos" } },
                { "Ford", new[] { "Ranger", "Explorer", "F-150", "EcoSport" } },
                { "Chevrolet", new[] { "Tracker", "S10", "Onix", "Colorado" } },
                { "Volkswagen", new[] { "Gol", "T-Cross", "Amarok", "Tiguan" } },
                { "BMW", new[] { "X3", "X5", "Serie 3", "X1" } }
            };

            var marcasKeys = marcasModelos.Keys.ToArray();
            var servicios = await context.Servicios.ToListAsync();
            var empleados = await context.Empleados.Include(e => e.Usuario).ToListAsync();
            var metodosPago = await context.MetodosPago.ToListAsync();

            if (!servicios.Any() || !metodosPago.Any())
            {
                return;
            }

            int clientesAFaltar = 100 - clienteCount;
            int startIdx = clienteCount + 1;

            var nuevosUsuarios = new List<Usuario>();
            var nuevosClientes = new List<Cliente>();
            var nuevasUbicaciones = new List<Ubicacion>();
            var nuevosVehiculos = new List<Vehiculo>();
            var nuevasReservas = new List<Reserva>();
            var nuevasVentas = new List<Venta>();
            var nuevosPagos = new List<Pago>();

            DateTime fechaBase = new DateTime(2025, 1, 15, 8, 0, 0, DateTimeKind.Utc);

            for (int i = 0; i < clientesAFaltar; i++)
            {
                int currentNumber = startIdx + i;
                string nom = nombres[rand.Next(nombres.Length)];
                string ape = apellidos[rand.Next(apellidos.Length)];
                string email = $"{nom.ToLower().Replace("á","a").Replace("é","e").Replace("í","i").Replace("ó","o").Replace("ú","u")}.{ape.ToLower().Replace("á","a").Replace("é","e").Replace("í","i").Replace("ó","o").Replace("ú","u")}{currentNumber}@gmail.com";
                string telefono = $"+591 7{rand.Next(1000000, 9999999)}";
                string ci = $"{rand.Next(1000000, 9999999)} SC";
                string zona = zonas[rand.Next(zonas.Length)];
                string direccion = $"{zona}, Calle {rand.Next(1, 15)} #{rand.Next(10, 200)}";

                var usuario = new Usuario
                {
                    EmprendimientoId = 1,
                    RolId = 3, // Cliente
                    Nombre = nom,
                    Apellido = ape,
                    Email = email,
                    PasswordHash = passwordHash,
                    Telefono = telefono,
                    Activo = true,
                    EmailVerificado = true,
                    FechaCreacion = fechaBase.AddDays(rand.Next(0, 180))
                };
                nuevosUsuarios.Add(usuario);
            }

            await context.Usuarios.AddRangeAsync(nuevosUsuarios);
            await context.SaveChangesAsync();

            for (int i = 0; i < nuevosUsuarios.Count; i++)
            {
                var usuario = nuevosUsuarios[i];
                string zona = zonas[rand.Next(zonas.Length)];
                string direccion = $"{zona}, Calle {rand.Next(1, 15)} #{rand.Next(10, 200)}";
                string ci = $"{rand.Next(1000000, 9999999)} SC";

                var cliente = new Cliente
                {
                    UsuarioId = usuario.Id,
                    Ci = ci,
                    Direccion = direccion,
                    Ciudad = "Santa Cruz de la Sierra",
                    FechaRegistro = usuario.FechaCreacion,
                    Activo = true
                };
                nuevosClientes.Add(cliente);
            }

            await context.Clientes.AddRangeAsync(nuevosClientes);
            await context.SaveChangesAsync();

            // Asignar Ubicaciones, Vehículos, Reservas y Ventas a cada cliente
            int ventaSeq = await context.Ventas.CountAsync() + 1000;

            foreach (var cliente in nuevosClientes)
            {
                string zona = zonas[rand.Next(zonas.Length)];
                var ubicacion = new Ubicacion
                {
                    ClienteId = cliente.Id,
                    Direccion = cliente.Direccion ?? $"{zona}, Av. Principal #{rand.Next(10, 999)}",
                    Zona = zona,
                    Referencia = $"Cerca a plaza principal de {zona}",
                    EsPrincipal = true,
                    Activo = true
                };
                nuevasUbicaciones.Add(ubicacion);

                // 1 a 3 vehículos por cliente
                int cantVehiculos = rand.Next(1, 4);
                var vehiculosCliente = new List<Vehiculo>();
                for (int v = 0; v < cantVehiculos; v++)
                {
                    string marca = marcasKeys[rand.Next(marcasKeys.Length)];
                    string[] modelos = marcasModelos[marca];
                    string modelo = modelos[rand.Next(modelos.Length)];
                    string placa = $"{rand.Next(1000, 9999)}-{((char)('A' + rand.Next(0, 26)))}{((char)('A' + rand.Next(0, 26)))}{((char)('A' + rand.Next(0, 26)))}";
                    string[] colores = new[] { "Blanco", "Negro", "Gris", "Plata", "Rojo", "Azul" };
                    string[] tipos = new[] { "Auto", "Vagoneta", "Camioneta", "SUV" };

                    var vehiculo = new Vehiculo
                    {
                        ClienteId = cliente.Id,
                        Placa = placa,
                        Tipo = tipos[rand.Next(tipos.Length)],
                        Marca = marca,
                        Modelo = modelo,
                        Año = rand.Next(2015, 2025).ToString(),
                        Color = colores[rand.Next(colores.Length)],
                        Activo = true,
                        FechaRegistro = cliente.FechaRegistro
                    };
                    nuevosVehiculos.Add(vehiculo);
                    vehiculosCliente.Add(vehiculo);
                }
            }

            await context.Ubicaciones.AddRangeAsync(nuevasUbicaciones);
            await context.Vehiculos.AddRangeAsync(nuevosVehiculos);
            await context.SaveChangesAsync();

            // Crear 1 a 4 Reservas y Ventas/Pagos por cliente
            foreach (var cliente in nuevosClientes)
            {
                var vehiculosCli = nuevosVehiculos.Where(v => v.ClienteId == cliente.Id).ToList();
                var ubicacionCli = nuevasUbicaciones.FirstOrDefault(u => u.ClienteId == cliente.Id);
                if (!vehiculosCli.Any() || ubicacionCli == null) continue;

                int cantReservas = rand.Next(1, 5);
                string[] estados = new[] { "Finalizada", "Finalizada", "Finalizada", "EnProceso", "Pendiente", "Aceptada" };

                for (int r = 0; r < cantReservas; r++)
                {
                    var servicio = servicios[rand.Next(servicios.Count)];
                    var vehiculo = vehiculosCli[rand.Next(vehiculosCli.Count)];
                    var emp = empleados.Any() ? empleados[rand.Next(empleados.Count)] : null;
                    string estado = estados[rand.Next(estados.Length)];
                    DateTime fechaReserva = cliente.FechaRegistro.AddDays(rand.Next(1, 120)).AddHours(rand.Next(8, 17));

                    var reserva = new Reserva
                    {
                        ClienteId = cliente.Id,
                        EmpleadoId = emp?.Id,
                        VehiculoId = vehiculo.Id,
                        UbicacionId = ubicacionCli.Id,
                        ServicioId = servicio.Id,
                        FechaProgramada = fechaReserva,
                        Estado = estado,
                        Observaciones = "Lavado solicitado vía app web EcoWash",
                        PrecioTotal = servicio.Precio,
                        FechaCreacion = fechaReserva.AddDays(-1)
                    };
                    nuevasReservas.Add(reserva);
                }
            }

            await context.Reservas.AddRangeAsync(nuevasReservas);
            await context.SaveChangesAsync();

            // Generar Ventas y Pagos para las reservas finalizadas
            foreach (var r in nuevasReservas.Where(r => r.Estado == "Finalizada"))
            {
                ventaSeq++;
                var metodo = metodosPago[rand.Next(metodosPago.Count)];
                var venta = new Venta
                {
                    ReservaId = r.Id,
                    ClienteId = r.ClienteId,
                    NumeroVenta = $"V-2025-{ventaSeq}",
                    FechaVenta = r.FechaProgramada,
                    Subtotal = r.PrecioTotal,
                    Descuento = 0,
                    Total = r.PrecioTotal,
                    Estado = "Pagada"
                };
                nuevasVentas.Add(venta);
            }

            await context.Ventas.AddRangeAsync(nuevasVentas);
            await context.SaveChangesAsync();

            foreach (var v in nuevasVentas)
            {
                var metodo = metodosPago[rand.Next(metodosPago.Count)];
                var pago = new Pago
                {
                    VentaId = v.Id,
                    ReservaId = v.ReservaId,
                    MetodoPagoId = metodo.Id,
                    Monto = v.Total,
                    FechaPago = v.FechaVenta,
                    Estado = "Completado",
                    Referencia = $"REF-{rand.Next(100000, 999999)}"
                };
                nuevosPagos.Add(pago);
            }

            await context.Pagos.AddRangeAsync(nuevosPagos);
            await context.SaveChangesAsync();
        }
    }
}
