namespace TommyChat.Shared.DTOs;

public class MensajeDTO
{
    public bool Leido { get; set; } = false;
    public DateTime Fecha { get; set; } = DateTime.Now;
    public string Remitente { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
}