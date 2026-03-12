namespace GestionCitas.DTOs
{
    public class EmpleadoGetDto
    {
        public int empleadoid { get; set; }
        public string? nombre { get; set; }
        public string? apellidos { get; set; }
        public string? telefono { get; set; }
        public int negocioid { get; set; }
        public string? color_agenda { get; set; }
        public string? estado { get; set; }
    }
}
