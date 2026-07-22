namespace BackendApi.Models;

public class Agenda
{
    public int Id { get; set; }
    public string TipoServicio { get; set; } = "";
    public string Fecha { get; set; } = "";
    public string Hora { get; set; } = "";
    public string Direccion { get; set; } = "";
    public string Notas { get; set; } = "";
}