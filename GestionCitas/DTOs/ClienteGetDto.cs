namespace GestionCitas.DTOs
{
    public class ClienteGetDto
    {
        public int ClienteId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellidos { get; set; }
        public string? Email { get; set; }
        public string? Telefono { get; set; }
        public string? notas_internas { get; set; }

    }
}
