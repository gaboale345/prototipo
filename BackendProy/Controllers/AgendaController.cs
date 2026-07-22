using Microsoft.AspNetCore.Mvc;
using BackendApi.Models;

namespace BackendApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AgendaController : ControllerBase
{
    // GET: api/agenda
    [HttpGet]
    public IActionResult ObtenerAgendas()
    {
        var agendas = new[]
        {
            new
            {
                Id = 1,
                TipoServicio = "Lavado Básico",
                Fecha = "2026-07-20",
                Hora = "10:00 AM",
                Direccion = "Calle Ficticia 123",
                Notas = "Sin observaciones"
            },
            new
            {
                Id = 2,
                TipoServicio = "Lavado Premium",
                Fecha = "2026-07-22",
                Hora = "03:00 PM",
                Direccion = "Av. Banzer #456",
                Notas = "Lavar motor"
            },
            new
            {
                Id = 3,
                TipoServicio = "Lavado Completo",
                Fecha = "2026-07-25",
                Hora = "09:30 AM",
                Direccion = "Av. Santos Dumont",
                Notas = "Aplicar cera"
            }
        };

        return Ok(agendas);
    }

    // GET: api/agenda/1
    [HttpGet("{id}")]
    public IActionResult ObtenerAgendaPorId(int id)
    {
        var agenda = new
        {
            Id = id,
            TipoServicio = "Lavado Básico",
            Fecha = "2026-07-20",
            Hora = "10:00 AM",
            Direccion = "Calle Ficticia 123",
            Notas = "Sin observaciones"
        };

        return Ok(agenda);
    }

    // POST: api/agenda
    [HttpPost]
    public IActionResult RegistrarAgenda([FromBody] Agenda agenda)
    {
        return Ok(new
        {
            mensaje = "Lavado agendado correctamente",
            agenda
        });
    }

    // PUT: api/agenda/1
    [HttpPut("{id}")]
    public IActionResult ActualizarAgenda(int id, [FromBody] Agenda agenda)
    {
        agenda.Id = id;

        return Ok(new
        {
            mensaje = "Agenda actualizada correctamente",
            agenda
        });
    }

    // DELETE: api/agenda/1
    [HttpDelete("{id}")]
    public IActionResult EliminarAgenda(int id)
    {
        return Ok(new
        {
            mensaje = $"La agenda con ID {id} fue eliminada correctamente"
        });
    }
}