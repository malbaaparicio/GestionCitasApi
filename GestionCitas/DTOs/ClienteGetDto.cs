namespace GestionCitas.DTOs
{
    public class ClienteGetDto
    {
        public int clienteid { get; set; }
        public string? nombre { get; set; }
        public string? apellidos { get; set; }
        public string? email { get; set; }
        public string? telefono { get; set; }
        public string? notas_internas { get; set; }

    }
}
